using System.Globalization;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 版本自适应调用器：按"方法名 + 参数可赋值"匹配真实重载，用于 StairsEditScope / FootPrintRoof /
/// ImageType 这类跨版本签名不稳定的 API。找不到时返回 null，由调用方给出可读错误。
/// </summary>
internal static class ApiInvoker
{
    public static Type? FindType(string fullName)
        => Type.GetType($"{fullName}, RevitAPI") ?? Type.GetType($"{fullName}, RevitAPIUI");

    public static object? InvokeStatic(Type type, string methodName, params object?[] arguments)
    {
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
        {
            if (!string.Equals(method.Name, methodName, StringComparison.Ordinal))
            {
                continue;
            }

            if (TryBind(method.GetParameters(), arguments, out var bound))
            {
                return method.Invoke(null, bound);
            }
        }

        return null;
    }

    public static object? Invoke(object instance, string methodName, params object?[] arguments)
    {
        foreach (var method in instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!string.Equals(method.Name, methodName, StringComparison.Ordinal))
            {
                continue;
            }

            if (TryBind(method.GetParameters(), arguments, out var bound))
            {
                return method.Invoke(instance, bound);
            }
        }

        return null;
    }

    public static object? GetProperty(object instance, string propertyName)
    {
        var property = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        return property?.GetValue(instance);
    }

    public static object? CreateInstance(Type type, params object?[] arguments)
    {
        foreach (var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
        {
            if (TryBind(constructor.GetParameters(), arguments, out var bound))
            {
                return constructor.Invoke(bound);
            }
        }

        return null;
    }

    // 注意：Service 层有同名类型 ParameterInfo（参数查询 DTO），这里必须写全限定名。
    private static bool TryBind(System.Reflection.ParameterInfo[] parameters, object?[] arguments, out object?[] bound)
    {
        bound = Array.Empty<object?>();
        if (parameters.Length != arguments.Length)
        {
            return false;
        }

        var result = new object?[parameters.Length];
        for (var index = 0; index < parameters.Length; index++)
        {
            var parameterType = parameters[index].ParameterType;
            var argument = arguments[index];

            if (argument is null)
            {
                if (parameterType.IsValueType && Nullable.GetUnderlyingType(parameterType) is null)
                {
                    return false;
                }

                result[index] = null;
                continue;
            }

            if (parameterType.IsInstanceOfType(argument))
            {
                result[index] = argument;
                continue;
            }

            // 枚举参数允许用字符串名匹配。
            if (parameterType.IsEnum && argument is string text)
            {
                try
                {
                    result[index] = Enum.Parse(parameterType, text, true);
                    continue;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            var underlying = Nullable.GetUnderlyingType(parameterType);
            if (underlying is not null && underlying.IsInstanceOfType(argument))
            {
                result[index] = argument;
                continue;
            }

            // 数值宽松匹配（int/long/double）。
            if ((parameterType == typeof(double) || parameterType == typeof(float)) && argument is int or long)
            {
                result[index] = Convert.ToDouble(argument, CultureInfo.InvariantCulture);
                continue;
            }

            if (parameterType == typeof(int) && argument is long longValue)
            {
                result[index] = (int)longValue;
                continue;
            }

            if ((parameterType == typeof(int) || parameterType == typeof(long)) && argument is double number)
            {
                result[index] = Convert.ToInt32(number, CultureInfo.InvariantCulture);
                continue;
            }

            return false;
        }

        bound = result;
        return true;
    }
}

/// <summary>
/// 楼梯服务：用 StairsEditScope（按名称跨程序集查找）+ StairsRun.CreateStraightRun 创建直跑楼梯，
/// 并用 StairsType 复制生成新的楼梯类型。所有步骤在宿主事务内执行。
/// API 注意：27.3 没有 STAIRS_ACTUAL_WIDTH / STAIRS_ATTR_RISER_TREAD_DETAIL 内置参数，
/// 因此宽度/厚度都改为按参数名查找。
/// </summary>
internal sealed class StairService : IStairService
{
    public StairResult CreateStair(
        object document,
        double totalHeightMm,
        int runCount,
        double? treadDepthMm,
        double? runWidthMm,
        double? wellWidthMm,
        double? landingWidthMm,
        double insertPointX,
        double insertPointY)
    {
        var doc = ModelingService.RequireDocument(document);
        if (totalHeightMm <= 0 || runCount <= 0)
        {
            throw new InvalidOperationException("totalHeightMm 与 runCount 必须为正数。");
        }

        var level = ModelingService.ResolveLevel(doc, null);
        _ = new FilteredElementCollector(doc)
            .OfClass(typeof(StairsRunType))
            .Cast<StairsRunType>()
            .FirstOrDefault()
            ?? throw new InvalidOperationException("项目里没有可用的楼梯梯段类型（StairsRunType）。");

        var treadDepth = (treadDepthMm ?? 280) / 304.8;
        var start = new XYZ(ModelingService.Mm(insertPointX), ModelingService.Mm(insertPointY), level.Elevation);
        var direction = new XYZ(1, 0, 0);

        var scopeType = ApiInvoker.FindType("Autodesk.Revit.DB.Architecture.StairsEditScope")
                        ?? ApiInvoker.FindType("Autodesk.Revit.UI.StairsEditScope");

        if (scopeType is null)
        {
            throw new InvalidOperationException(
                "当前 Revit 版本里找不到 StairsEditScope（RevitAPI / RevitAPIUI 均无）；"
                + "楼梯需要它来创建，请改用 Revit 界面手工建楼梯，或在支持该类型的版本上运行。");
        }

        var scope = ApiInvoker.CreateInstance(scopeType, doc, "RevitToolSet 创建楼梯")
                    ?? throw new InvalidOperationException("StairsEditScope 构造失败（版本签名不匹配）。");

        var baseLevelId = level.Id;
        var topLevelId = new FilteredElementCollector(doc)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .OrderBy(item => item.Elevation)
            .FirstOrDefault(item => item.Elevation > level.Elevation)?.Id ?? level.Id;

        object? stairsId = null;
        try
        {
            stairsId = ApiInvoker.Invoke(scope, "StartNewStairs", doc, baseLevelId, topLevelId);
            if (stairsId is null)
            {
                throw new InvalidOperationException("StairsEditScope.StartNewStairs 调用失败（版本签名不匹配）。");
            }

            var run = StairsRun.CreateStraightRun(
                doc,
                (ElementId)stairsId,
                Line.CreateBound(start, start + direction.Multiply(treadDepth * runCount * 0.6)),
                StairsRunJustification.Center);

            if (runWidthMm is > 0)
            {
                var widthParameter = run.GetParameters("宽度").FirstOrDefault()
                                     ?? run.GetParameters("Width").FirstOrDefault();
                if (widthParameter is { IsReadOnly: false })
                {
                    widthParameter.Set(ModelingService.Mm(runWidthMm.Value));
                }
            }

            ApiInvoker.Invoke(scope, "Commit", new object?[] { null });

            return new StairResult(
                (int)((ElementId)stairsId).Value,
                (int)run.Id.Value,
                runCount,
                totalHeightMm,
                $"{scopeType.FullName}.StartNewStairs + StairsRun.CreateStraightRun");
        }
        catch (Exception ex)
        {
            try
            {
                ApiInvoker.Invoke(scope, "Cancel");
            }
            catch (Exception)
            {
                // 取消失败不影响错误上报。
            }

            throw new InvalidOperationException(
                $"创建楼梯失败：{ex.GetBaseException().Message}"
                + (wellWidthMm is not null || landingWidthMm is not null
                    ? "（wellWidthMm/landingWidthMm 已记录但本实现只创建直跑梯段）"
                    : string.Empty));
        }
    }

    public StairTypeResult CreateStairType(
        object document,
        string newStairTypeName,
        string? baseStairTypeName,
        double? runThicknessMm,
        double? landingThicknessMm)
    {
        var doc = ModelingService.RequireDocument(document);
        if (string.IsNullOrWhiteSpace(newStairTypeName))
        {
            throw new InvalidOperationException("必须提供 newStairTypeName。");
        }

        var baseType = new FilteredElementCollector(doc)
            .OfClass(typeof(StairsType))
            .Cast<StairsType>()
            .FirstOrDefault(type => string.IsNullOrWhiteSpace(baseStairTypeName) ||
                                    string.Equals(type.Name, baseStairTypeName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"未找到基础楼梯类型 {baseStairTypeName ?? "(任意)"}。");

        var duplicated = baseType.Duplicate(newStairTypeName);
        var notes = new List<string>();

        // 梯段/平台厚度在 StairsType.RunType / LandingType 上；不同版本成员名可能不同，用自适应调用。
        if (runThicknessMm is > 0)
        {
            if (ApplyThickness(duplicated, "RunType", runThicknessMm.Value))
            {
                notes.Add($"梯段厚度已设为 {runThicknessMm} mm");
            }
            else
            {
                notes.Add("梯段厚度设置失败（未找到 RunType 的厚度参数）");
            }
        }

        if (landingThicknessMm is > 0)
        {
            if (ApplyThickness(duplicated, "LandingType", landingThicknessMm.Value))
            {
                notes.Add($"平台厚度已设为 {landingThicknessMm} mm");
            }
            else
            {
                notes.Add("平台厚度设置失败（未找到 LandingType 的厚度参数）");
            }
        }

        return new StairTypeResult(
            (int)duplicated.Id.Value,
            duplicated.Name,
            runThicknessMm,
            landingThicknessMm,
            notes);
    }

    private static bool ApplyThickness(ElementType stairsType, string propertyName, double thicknessMm)
    {
        try
        {
            var subTypeElement = ApiInvoker.GetProperty(stairsType, propertyName) as Element;
            if (subTypeElement is null)
            {
                return false;
            }

            var thicknessParameter = subTypeElement.GetParameters("厚度").FirstOrDefault()
                                     ?? subTypeElement.GetParameters("Thickness").FirstOrDefault();

            if (thicknessParameter is null || thicknessParameter.IsReadOnly)
            {
                return false;
            }

            thicknessParameter.Set(ModelingService.Mm(thicknessMm));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

/// <summary>
/// 屋顶坡度服务：FootPrintRoof 的边缘坡度（DefinesSlope/SetSlopeAngle 在不同版本签名不同，用自适应调用）。
/// </summary>
internal sealed class RoofEditService : IRoofEditService
{
    public RoofSlopeResult ModifyRoofSlope(
        object document,
        int roofId,
        int? edgeIndex,
        bool definesSlope,
        double? slopeAngleDegrees,
        bool applyToAllEdges)
    {
        var doc = ModelingService.RequireDocument(document);
        var roof = doc.GetElement(new ElementId(roofId)) as FootPrintRoof
                   ?? throw new InvalidOperationException(
                       $"元素 {roofId} 不是足迹屋顶（FootPrintRoof）；拉伸屋顶请用 Revit 界面编辑其轮廓。");

        var curves = new List<ModelCurve>();
        if (ApiInvoker.Invoke(roof, "GetProfiles") is System.Collections.IEnumerable profiles)
        {
            foreach (var profile in profiles)
            {
                if (profile is not CurveArray array)
                {
                    continue;
                }

                foreach (var item in array)
                {
                    if (item is ModelCurve modelCurve)
                    {
                        curves.Add(modelCurve);
                    }
                }
            }
        }

        if (curves.Count == 0)
        {
            throw new InvalidOperationException($"屋顶 {roofId} 没有可编辑的轮廓边（GetProfiles 为空）。");
        }

        var targets = applyToAllEdges || edgeIndex is null
            ? curves
            : new List<ModelCurve> { curves[Math.Clamp(edgeIndex.Value, 0, curves.Count - 1)] };

        var notes = new List<string>();
        var changed = 0;

        foreach (var curve in targets)
        {
            try
            {
                var defines = ApiInvoker.Invoke(roof, "DefinesSlope", curve, definesSlope);
                if (defines is null)
                {
                    notes.Add($"DefinesSlope 调用失败（版本签名不匹配），共 {curves.Count} 条边");
                    break;
                }

                if (definesSlope && slopeAngleDegrees is not null)
                {
                    var radians = slopeAngleDegrees.Value * Math.PI / 180.0;
                    if (ApiInvoker.Invoke(roof, "SetSlopeAngle", curve, radians) is null)
                    {
                        notes.Add("SetSlopeAngle 调用失败（版本签名不匹配）");
                    }
                }

                changed++;
            }
            catch (Exception ex)
            {
                notes.Add($"{curve.Id.Value}: {ex.GetBaseException().Message}");
            }
        }

        return new RoofSlopeResult(roofId, changed, applyToAllEdges || edgeIndex is null, slopeAngleDegrees, notes);
    }
}

/// <summary>
/// 卫星图导入服务：Revit API 没有网络下载能力，因此本实现只支持"本地栅格 → 图像实例"导入，
/// 并把 location/lat/lon/source/zoom/radius 作为地理参考信息记录在结果里。
/// </summary>
internal sealed class SatelliteImportService : ISatelliteImportService
{
    public SatelliteImportResult Import(
        object document,
        string? imagePath,
        string? locationName,
        double? latitude,
        double? longitude,
        string? mapSource,
        int? zoomLevel,
        double? radiusKm)
    {
        var doc = ModelingService.RequireDocument(document);

        var geo = new List<string>
        {
            $"location={locationName ?? "(未提供)"}",
            $"lat={latitude?.ToString(CultureInfo.InvariantCulture) ?? "(未提供)"}",
            $"lon={longitude?.ToString(CultureInfo.InvariantCulture) ?? "(未提供)"}",
            $"source={mapSource ?? "unknown"}",
            $"zoom={zoomLevel?.ToString(CultureInfo.InvariantCulture) ?? "(未提供)"}",
            $"radius_km={radiusKm?.ToString(CultureInfo.InvariantCulture) ?? "(未提供)"}",
        };

        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return new SatelliteImportResult(
                null,
                "needs-image",
                "Revit API 不提供网络能力，无法直接下载卫星图瓦片；请先准备本地栅格图片（png/jpg）"
                + "并在参数里给出 image_path/file_path 后重试。",
                geo);
        }

        if (!System.IO.File.Exists(imagePath))
        {
            throw new InvalidOperationException($"图片文件不存在：{imagePath}");
        }

        var view = doc.ActiveView ?? throw new InvalidOperationException("当前没有活动视图，无法导入图像。");

        var imageTypeType = ApiInvoker.FindType("Autodesk.Revit.DB.ImageType")
                            ?? throw new InvalidOperationException("当前版本没有 ImageType（无法导入栅格图像）。");
        var optionsType = ApiInvoker.FindType("Autodesk.Revit.DB.ImageTypeOptions");

        object? imageType = null;
        if (optionsType is not null)
        {
            var options = ApiInvoker.CreateInstance(optionsType, imagePath, false, "Import")
                          ?? ApiInvoker.CreateInstance(optionsType, imagePath, false);
            if (options is not null)
            {
                imageType = ApiInvoker.InvokeStatic(imageTypeType, "Create", doc, options);
            }
        }

        if (imageType is not ElementId typeId)
        {
            throw new InvalidOperationException(
                "ImageType 创建失败（版本签名不匹配）：请改用 Revit 界面「插入 → 图像」，"
                + "或提供可导入的图片格式后重试。");
        }

        var placementType = ApiInvoker.FindType("Autodesk.Revit.DB.ImagePlacementOptions");
        object? placement = placementType is not null
            ? ApiInvoker.CreateInstance(placementType, XYZ.Zero, "Center")
            : null;

        if (placement is null)
        {
            throw new InvalidOperationException("ImagePlacementOptions 构造失败（版本签名不匹配）。");
        }

        if (ApiInvoker.InvokeStatic(typeof(ImageInstance), "Create", doc, view, typeId, placement) is not ImageInstance image)
        {
            throw new InvalidOperationException("ImageInstance 创建失败（版本签名不匹配）。");
        }

        return new SatelliteImportResult(
            (int)image.Id.Value,
            "imported",
            $"已把本地栅格图导入为图像实例 {(int)image.Id.Value}（地理参考信息仅记录，不做坐标配准）",
            geo);
    }
}
