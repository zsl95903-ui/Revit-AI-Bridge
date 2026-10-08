using System.Globalization;
using Autodesk.Revit.DB;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 参数读写服务。要点：
///   * 参数值统一按"参数自身的数据类型 + 项目显示单位"解释；显式给 unit（mm/m/cm/ft/inch）时按该单位换算；
///   * Double 参数通过 UnitUtils.ConvertToInternalUnits 转换到 Revit 内部单位（英尺体系）；
///   * 只读参数跳过并计入失败清单，不抛异常中断其它赋值。
/// API 校正记录：Revit 27.3 的 ElementId 用 (int)Id.Value（IntegerValue 已移除）；
/// Element 没有 get_Parameter(ElementId)，按名称/遍历匹配；全局参数用 FilteredElementCollector 枚举。
/// </summary>
internal sealed class ParameterService : IParameterService
{
    public IReadOnlyList<ParameterInfo> QueryParameters(
        object document,
        int elementId,
        IReadOnlyList<string> names,
        bool includeTypeParameters)
    {
        if (document is not Document doc)
        {
            return Array.Empty<ParameterInfo>();
        }

        var element = doc.GetElement(new ElementId(elementId));
        if (element is null)
        {
            return Array.Empty<ParameterInfo>();
        }

        var result = new List<ParameterInfo>();
        Collect(element, elementId, names, false, result);

        if (includeTypeParameters && element.GetTypeId() is { } typeId && typeId != ElementId.InvalidElementId &&
            doc.GetElement(typeId) is { } type)
        {
            Collect(type, elementId, names, true, result);
        }

        return result;
    }

    private static void Collect(Element source, int elementId, IReadOnlyList<string> names, bool isType, List<ParameterInfo> result)
    {
        foreach (Parameter parameter in source.GetOrderedParameters())
        {
            var definition = parameter.Definition;
            var name = definition?.Name ?? string.Empty;
            if (names.Count > 0 && !names.Any(filter => string.Equals(filter, name, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            result.Add(new ParameterInfo(
                elementId,
                (int)parameter.Id.Value,
                name,
                parameter.StorageType.ToString(),
                ReadValue(parameter),
                parameter.IsReadOnly,
                parameter.IsShared,
                isType,
                definition?.GetGroupTypeId()?.TypeId ?? string.Empty));
        }
    }

    private static string ReadValue(Parameter parameter)
    {
        try
        {
            return parameter.StorageType switch
            {
                StorageType.String => parameter.AsString() ?? string.Empty,
                StorageType.Integer => parameter.AsInteger().ToString(CultureInfo.InvariantCulture),
                StorageType.Double => parameter.AsValueString() ?? parameter.AsDouble().ToString(CultureInfo.InvariantCulture),
                StorageType.ElementId => parameter.AsElementId() is { } id ? ((int)id.Value).ToString(CultureInfo.InvariantCulture) : string.Empty,
                _ => string.Empty,
            };
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public ParameterSetResult SetParameterValues(object document, IReadOnlyList<ParameterAssignment> assignments)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var applied = 0;
        var failures = new List<string>();

        foreach (var assignment in assignments)
        {
            var element = doc.GetElement(new ElementId(assignment.ElementId));
            if (element is null)
            {
                failures.Add($"元素 {assignment.ElementId} 不存在");
                continue;
            }

            var target = assignment.IsTypeParameter && element.GetTypeId() is { } typeId && typeId != ElementId.InvalidElementId
                ? doc.GetElement(typeId) ?? element
                : element;

            var parameter = FindParameter(target, assignment);
            if (parameter is null)
            {
                failures.Add($"元素 {assignment.ElementId} 上找不到参数 {assignment.ParameterName ?? assignment.ParameterId?.ToString() ?? "?"}");
                continue;
            }

            if (parameter.IsReadOnly)
            {
                failures.Add($"参数 {parameter.Definition?.Name} 为只读");
                continue;
            }

            if (TrySet(doc, parameter, assignment.Value, assignment.Unit, out var error))
            {
                applied++;
            }
            else
            {
                failures.Add($"参数 {parameter.Definition?.Name} 赋值失败：{error}");
            }
        }

        return new ParameterSetResult(assignments.Count, applied, failures);
    }

    private static Parameter? FindParameter(Element element, ParameterAssignment assignment)
    {
        if (!string.IsNullOrWhiteSpace(assignment.ParameterName))
        {
            var byName = element.LookupParameter(assignment.ParameterName!);
            if (byName is not null)
            {
                return byName;
            }
        }

        if (assignment.ParameterId is null)
        {
            return null;
        }

        foreach (Parameter parameter in element.GetOrderedParameters())
        {
            if ((int)parameter.Id.Value == assignment.ParameterId.Value)
            {
                return parameter;
            }
        }

        return null;
    }

    private static bool TrySet(Document document, Parameter parameter, string? value, string? unit, out string error)
    {
        error = string.Empty;
        var text = value ?? string.Empty;

        try
        {
            switch (parameter.StorageType)
            {
                case StorageType.String:
                    return parameter.Set(text);

                case StorageType.Integer:
                    if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer))
                    {
                        return parameter.Set(integer);
                    }

                    if (bool.TryParse(text, out var boolean))
                    {
                        return parameter.Set(boolean ? 1 : 0);
                    }

                    error = "需要整数或布尔值";
                    return false;

                case StorageType.Double:
                    if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
                    {
                        error = "需要数值";
                        return false;
                    }

                    return parameter.Set(ConvertToInternalUnits(document, parameter, number, unit));

                case StorageType.ElementId:
                    if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var elementId))
                    {
                        return parameter.Set(new ElementId(elementId));
                    }

                    error = "需要元素 ID";
                    return false;

                default:
                    error = "不支持的数据类型";
                    return false;
            }
        }
        catch (Exception ex)
        {
            error = ex.GetBaseException().Message;
            return false;
        }
    }

    private static double ConvertToInternalUnits(Document document, Parameter parameter, double value, string? unit)
    {
        var unitTypeId = ResolveUnitTypeId(unit);
        if (unitTypeId is not null)
        {
            return UnitUtils.ConvertToInternalUnits(value, unitTypeId);
        }

        // 未显式指定单位：按该参数在项目里的显示单位解释。
        try
        {
            var specTypeId = parameter.Definition?.GetDataType();
            if (specTypeId is not null)
            {
                var projectUnit = document.GetUnits().GetFormatOptions(specTypeId).GetUnitTypeId();
                return UnitUtils.ConvertToInternalUnits(value, projectUnit);
            }
        }
        catch (Exception)
        {
            // 取不到规格/单位时按原值处理。
        }

        return value;
    }

    private static ForgeTypeId? ResolveUnitTypeId(string? unit)
    {
        if (string.IsNullOrWhiteSpace(unit))
        {
            return null;
        }

        return unit.Trim().ToLowerInvariant() switch
        {
            "mm" or "毫米" => UnitTypeId.Millimeters,
            "cm" or "厘米" => UnitTypeId.Centimeters,
            "m" or "米" => UnitTypeId.Meters,
            "ft" or "英尺" => UnitTypeId.Feet,
            "inch" or "in" or "英寸" => UnitTypeId.Inches,
            "degree" or "deg" or "度" => UnitTypeId.Degrees,
            _ => null,
        };
    }
}

/// <summary>
/// 全局参数服务：create / list / set_value / delete。
/// 说明：数值型全局参数按项目显示单位解释（与 set_parameter_values 同一约定）。
/// </summary>
internal sealed class GlobalParameterService : IGlobalParameterService
{
    public IReadOnlyList<GlobalParameterInfo> List(object document)
    {
        if (document is not Document doc)
        {
            return Array.Empty<GlobalParameterInfo>();
        }

        return new FilteredElementCollector(doc)
            .OfClass(typeof(GlobalParameter))
            .Cast<GlobalParameter>()
            .Select(Describe)
            .ToArray();
    }

    public GlobalParameterInfo Create(object document, string name, string? parameterType, string? formula, string? value)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var specTypeId = ResolveSpecTypeId(parameterType);
        var parameter = GlobalParameter.Create(doc, name, specTypeId);

        if (!string.IsNullOrWhiteSpace(formula))
        {
            try
            {
                parameter.SetFormula(formula!);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"公式设置失败：{ex.GetBaseException().Message}");
            }
        }
        else if (!string.IsNullOrWhiteSpace(value))
        {
            ApplyValue(doc, parameter, value!, specTypeId);
        }

        return Describe(parameter);
    }

    public GlobalParameterInfo? SetValue(object document, string name, string? value, string? formula)
    {
        var parameter = Find(document, name);
        if (parameter is null || document is not Document doc)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(formula))
        {
            parameter.SetFormula(formula!);
        }
        else if (!string.IsNullOrWhiteSpace(value))
        {
            ApplyValue(doc, parameter, value!, null);
        }

        return Describe(parameter);
    }

    public bool Delete(object document, string name)
    {
        if (document is not Document doc)
        {
            return false;
        }

        var parameter = Find(document, name);
        if (parameter is null)
        {
            return false;
        }

        doc.Delete(parameter.Id);
        return true;
    }

    private static GlobalParameter? Find(object? document, string name)
    {
        if (document is not Document doc || string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return new FilteredElementCollector(doc)
            .OfClass(typeof(GlobalParameter))
            .Cast<GlobalParameter>()
            .FirstOrDefault(parameter => string.Equals(parameter.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    private static void ApplyValue(Document document, GlobalParameter parameter, string value, ForgeTypeId? specTypeId)
    {
        var spec = specTypeId ?? parameter.GetDefinition()?.GetDataType() ?? SpecTypeId.Number;

        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
        {
            var internalValue = number;
            try
            {
                var projectUnit = document.GetUnits().GetFormatOptions(spec).GetUnitTypeId();
                internalValue = UnitUtils.ConvertToInternalUnits(number, projectUnit);
            }
            catch (Exception)
            {
                // 取不到单位时按原值。
            }

            parameter.SetValue(new DoubleParameterValue(internalValue));
            return;
        }

        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer))
        {
            parameter.SetValue(new IntegerParameterValue(integer));
            return;
        }

        parameter.SetValue(new StringParameterValue(value));
    }

    private static ForgeTypeId ResolveSpecTypeId(string? parameterType)
    {
        return (parameterType ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "length" or "长度" => SpecTypeId.Length,
            "angle" or "角度" => SpecTypeId.Angle,
            "integer" or "int" or "整数" => SpecTypeId.Int.Integer,
            "text" or "string" or "文本" => SpecTypeId.String.Text,
            "yesno" or "boolean" or "是/否" => SpecTypeId.Boolean.YesNo,
            _ => SpecTypeId.Number,
        };
    }

    private static GlobalParameterInfo Describe(GlobalParameter parameter)
    {
        var value = string.Empty;
        var formula = string.Empty;

        try
        {
            value = parameter.GetValue() switch
            {
                DoubleParameterValue d => d.Value.ToString(CultureInfo.InvariantCulture),
                IntegerParameterValue i => i.Value.ToString(CultureInfo.InvariantCulture),
                StringParameterValue s => s.Value ?? string.Empty,
                _ => string.Empty,
            };
        }
        catch (Exception)
        {
            // 公式驱动的参数取不到直接值。
        }

        try
        {
            formula = parameter.GetFormula() ?? string.Empty;
        }
        catch (Exception)
        {
            // 忽略。
        }

        return new GlobalParameterInfo(
            (int)parameter.Id.Value,
            parameter.Name,
            value,
            formula,
            parameter.GetDefinition()?.GetDataType()?.TypeId ?? string.Empty);
    }
}
