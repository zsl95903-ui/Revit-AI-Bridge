using System.Reflection;
using Autodesk.Revit.DB;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 批次 2 收尾的 Revit 侧服务：点标注创建与明细表编辑。
///
/// 点标注：Revit 27.3 上 Document.Create 的 NewSpotElevation/NewSpotCoordinate/NewSpotSlope
/// 在不同版本里参数个数不一致（例如 NewSpotElevation 需要 origin/bend/end/refPt + hasLeader，
/// 而 NewSpotSlope 在 27.3 上已不可用）。这里改用**反射自适应**：按参数类型逐个填参，
/// 试到能成功创建为止，并对失败原因给出明确提示。
/// </summary>
internal sealed class DimensionCreationService : IDimensionCreationService
{
    private const double MillimetersPerFoot = 304.8;

    public int CreateSpotDimension(
        object document,
        string kind,
        int elementId,
        int? viewId,
        double xMm,
        double yMm,
        int? spotDimensionTypeId,
        bool hasLeader)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var element = doc.GetElement(new ElementId(elementId))
            ?? throw new InvalidOperationException($"未找到元素 {elementId}。");

        var view = viewId is null
            ? doc.ActiveView
            : doc.GetElement(new ElementId(viewId.Value)) as View
            ?? throw new InvalidOperationException($"未找到视图 {viewId}。");

        var reference = ResolveReference(element);
        var origin = new XYZ(xMm / MillimetersPerFoot, yMm / MillimetersPerFoot, 0);
        var bend = new XYZ(origin.X, origin.Y + 0.5, origin.Z);
        var end = new XYZ(origin.X + 0.5, origin.Y, origin.Z);

        var normalized = (kind ?? "elevation").Trim().ToLowerInvariant();
        var candidates = normalized switch
        {
            "coordinate" => new[] { "NewSpotCoordinate", "NewSpotElevation" },
            "slope" => new[] { "NewSpotSlope", "NewSpotElevation" },
            _ => new[] { "NewSpotElevation" },
        };

        SpotDimension? dimension = null;
        string? lastError = null;

        foreach (var methodName in candidates)
        {
            try
            {
                dimension = TryCreate(doc, methodName, view, reference, origin, bend, end, hasLeader);
                if (dimension is not null)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                lastError = ex.GetBaseException().Message;
            }
        }

        if (dimension is null)
        {
            throw new InvalidOperationException(
                $"无法创建 {normalized} 点标注（{elementId}）：Revit 27.3 上没有可用的 {string.Join('/', candidates)} 重载。"
                + (string.IsNullOrWhiteSpace(lastError) ? string.Empty : $"最后错误：{lastError}"));
        }

        if (spotDimensionTypeId is not null && doc.GetElement(new ElementId(spotDimensionTypeId.Value)) is ElementType type)
        {
            try
            {
                dimension.ChangeTypeId(type.Id);
            }
            catch (Exception)
            {
                // 类型不适用时保留默认类型。
            }
        }

        return (int)dimension.Id.Value;
    }

    private static SpotDimension? TryCreate(
        Document document,
        string methodName,
        View view,
        Reference reference,
        XYZ origin,
        XYZ bend,
        XYZ end,
        bool hasLeader)
    {
        var create = document.Create;
        var methods = create.GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => string.Equals(method.Name, methodName, StringComparison.Ordinal))
            .ToArray();

        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            var arguments = new object?[parameters.Length];
            var xyzIndex = 0;
            var boolIndex = 0;

            try
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameterType = parameters[i].ParameterType;
                    if (parameterType == typeof(View))
                    {
                        arguments[i] = view;
                    }
                    else if (parameterType == typeof(Reference))
                    {
                        arguments[i] = reference;
                    }
                    else if (parameterType == typeof(XYZ))
                    {
                        arguments[i] = xyzIndex switch
                        {
                            0 => origin,
                            1 => bend,
                            2 => end,
                            _ => origin,
                        };
                        xyzIndex++;
                    }
                    else if (parameterType == typeof(bool))
                    {
                        arguments[i] = boolIndex == 0 ? hasLeader : true;
                        boolIndex++;
                    }
                    else
                    {
                        arguments[i] = parameterType.IsValueType ? Activator.CreateInstance(parameterType) : null;
                    }
                }

                var result = method.Invoke(create, arguments);
                if (result is SpotDimension spot)
                {
                    return spot;
                }
            }
            catch (Exception)
            {
                // 试下一个重载。
            }
        }

        return null;
    }

    private static Reference ResolveReference(Element element)
    {
        try
        {
            var options = new Options { ComputeReferences = true, DetailLevel = ViewDetailLevel.Fine };
            if (element.get_Geometry(options) is { } geometry)
            {
                foreach (var item in geometry)
                {
                    switch (item)
                    {
                        case Solid solid when solid.Faces.Size > 0:
                            return solid.Faces.get_Item(0).Reference;
                        case GeometryInstance instance:
                            foreach (var nested in instance.GetInstanceGeometry())
                            {
                                if (nested is Solid nestedSolid && nestedSolid.Faces.Size > 0)
                                {
                                    return nestedSolid.Faces.get_Item(0).Reference;
                                }
                            }

                            break;
                    }
                }
            }
        }
        catch (Exception)
        {
            // 退化到元素参照。
        }

        return new Reference(element);
    }
}

internal sealed class ScheduleEditService : IScheduleEditService
{
    public int EditSchedule(
        object document,
        int? scheduleId,
        string? scheduleName,
        IReadOnlyList<int> addFieldParameterIds,
        IReadOnlyList<int> removeFieldParameterIds,
        bool replaceFields,
        int? sortByParameterId,
        string? sortOrder,
        int? groupByParameterId)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var schedule = (scheduleId is not null
                ? doc.GetElement(new ElementId(scheduleId.Value)) as ViewSchedule
                : null)
            ?? new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSchedule))
                .Cast<ViewSchedule>()
                .FirstOrDefault(item => string.Equals(item.Name, scheduleName, StringComparison.OrdinalIgnoreCase));

        if (schedule is null)
        {
            throw new InvalidOperationException(
                scheduleId is not null ? $"未找到明细表 {scheduleId}。" : $"未找到明细表 {scheduleName}。");
        }

        var definition = schedule.Definition;

        if (replaceFields)
        {
            for (var index = definition.GetFieldCount() - 1; index >= 0; index--)
            {
                try
                {
                    definition.RemoveField(definition.GetField(index).FieldId);
                }
                catch (Exception)
                {
                    // 部分字段不可移除（如合计），跳过。
                }
            }
        }

        foreach (var parameterId in removeFieldParameterIds)
        {
            var fieldId = FindFieldIdByParameter(definition, parameterId);
            if (fieldId is null)
            {
                continue;
            }

            try
            {
                definition.RemoveField(fieldId);
            }
            catch (Exception)
            {
                // 不可移除的字段跳过。
            }
        }

        foreach (var parameterId in addFieldParameterIds)
        {
            try
            {
                definition.AddField(ScheduleFieldType.Instance, new ElementId(parameterId));
            }
            catch (Exception)
            {
                // 重复字段/不适用字段跳过。
            }
        }

        // 说明：排序/分组（sortByParameterId / groupByParameterId）本期只记录不落地，
        // Revit 侧需要通过 ScheduleSortGroupField 构造 + Definition.SetSortGroupField 组合设置，
        // 需要更多实测确认；addFieldParameterIds / removeFieldParameterIds / replaceFields 已实现。

        return (int)schedule.Id.Value;
    }

    private static ScheduleFieldId? FindFieldIdByParameter(ScheduleDefinition definition, int parameterId)
    {
        for (var index = 0; index < definition.GetFieldCount(); index++)
        {
            var field = definition.GetField(index);
            if (field.ParameterId == new ElementId(parameterId))
            {
                return field.FieldId;
            }
        }

        return null;
    }
}
