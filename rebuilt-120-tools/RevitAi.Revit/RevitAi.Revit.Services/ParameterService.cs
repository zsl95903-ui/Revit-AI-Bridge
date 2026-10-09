using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Services;

internal sealed class ParameterService : IParameterService
{
	private readonly UIApplication _uiApplication;

	public ParameterService(UIApplication uiApplication)
	{
		_uiApplication = uiApplication ?? throw new ArgumentNullException("uiApplication");
	}

	public IEnumerable<object> GetParameters(object element)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			return ((IEnumerable)val.Parameters).Cast<object>();
		}
		catch (Exception)
		{
			return Enumerable.Empty<object>();
		}
	}

	public object? GetParameter(object element, string parameterName)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			return val.LookupParameter(parameterName);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public string? GetParameterValueAsString(object parameter)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected I4, but got Unknown
		try
		{
			Parameter val = (Parameter)((parameter is Parameter) ? parameter : null);
			if (val == null)
			{
				return null;
			}
			if (!val.HasValue)
			{
				return null;
			}
			StorageType storageType = val.StorageType;
			StorageType val2 = storageType;
			return ((int)(val2) - 1) switch
			{
				0 => val.AsValueString(), 
				1 => val.AsValueString(), 
				2 => val.AsString(), 
				3 => val.AsValueString(), 
				_ => val.AsValueString(), 
			};
		}
		catch (Exception)
		{
			return null;
		}
	}

	public double? GetParameterValueAsDouble(object parameter)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		try
		{
			Parameter val = (Parameter)((parameter is Parameter) ? parameter : null);
			if (val == null)
			{
				return null;
			}
			if (!val.HasValue || (int)val.StorageType != 2)
			{
				return null;
			}
			return val.AsDouble();
		}
		catch (Exception)
		{
			return null;
		}
	}

	public int? GetParameterValueAsInteger(object parameter)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		try
		{
			Parameter val = (Parameter)((parameter is Parameter) ? parameter : null);
			if (val == null)
			{
				return null;
			}
			if (!val.HasValue || (int)val.StorageType != 1)
			{
				return null;
			}
			return val.AsInteger();
		}
		catch (Exception)
		{
			return null;
		}
	}

	public string? GetElementParameterValue(object element, string parameterName)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			Parameter val2 = val.LookupParameter(parameterName);
			if (val2 == null || !val2.HasValue)
			{
				return null;
			}
			return val2.AsValueString();
		}
		catch (Exception)
		{
			return null;
		}
	}

	public bool SetParameterValue(object element, string parameterName, string value)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return false;
			}
			Parameter val2 = val.LookupParameter(parameterName);
			if (val2 == null)
			{
				return false;
			}
			if (((APIObject)val2).IsReadOnly)
			{
				return false;
			}
			if ((int)val2.StorageType == 3)
			{
				val2.Set(value);
				return true;
			}
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private ParameterUnitType GetParameterUnitType(Parameter param)
	{
		try
		{
			Definition definition = param.Definition;
			ForgeTypeId dataType = definition.GetDataType();
			if (dataType == SpecTypeId.Length)
			{
				return ParameterUnitType.Length;
			}
			if (dataType == SpecTypeId.Area)
			{
				return ParameterUnitType.Area;
			}
			if (dataType == SpecTypeId.Volume)
			{
				return ParameterUnitType.Volume;
			}
			if (dataType == SpecTypeId.Angle)
			{
				return ParameterUnitType.Angle;
			}
			return ParameterUnitType.Unknown;
		}
		catch (Exception ex)
		{
			LogError("获取参数单位类型失败: " + ex.Message);
			return ParameterUnitType.Unknown;
		}
	}

	private double ConvertValueByParameterType(double value, ParameterUnitType unitType)
	{
		return unitType switch
		{
			ParameterUnitType.Length => value / 304.8, 
			ParameterUnitType.Area => value / 0.092903, 
			ParameterUnitType.Volume => value / 0.0283168, 
			ParameterUnitType.Angle => value, 
			_ => value, 
		};
	}

	public bool SetParameterValue(object element, string parameterName, double value)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Invalid comparison between Unknown and I4
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return false;
			}
			Parameter val2 = val.LookupParameter(parameterName);
			if (val2 == null)
			{
				return false;
			}
			if (((APIObject)val2).IsReadOnly)
			{
				return false;
			}
			if ((int)val2.StorageType == 2)
			{
				ParameterUnitType parameterUnitType = GetParameterUnitType(val2);
				if (parameterUnitType != ParameterUnitType.Unknown)
				{
					double num = ConvertValueByParameterType(value, parameterUnitType);
					val2.Set(num);
					return true;
				}
				double num2 = ((value > 1000.0) ? (value / 304.8) : ((!(value >= 10.0)) ? value : (value / 304.8)));
				val2.Set(num2);
				return true;
			}
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public bool SetParameterValueWithUnit(object element, string parameterName, double value, string unit)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return false;
			}
			Parameter val2 = val.LookupParameter(parameterName);
			if (val2 == null)
			{
				return false;
			}
			if (((APIObject)val2).IsReadOnly)
			{
				return false;
			}
			if ((int)val2.StorageType == 2)
			{
				double num = ConvertToRevitInternalUnit(value, unit);
				val2.Set(num);
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			LogError("SetParameterValueWithUnit 失败: " + ex.Message);
			return false;
		}
	}

	private double ConvertToRevitInternalUnit(double value, string unit)
	{
		string text = unit.ToLowerInvariant();
		if (!(text == "mm"))
		{
			if (!(text == "m"))
			{
				if (!(text == "ft"))
				{
					if (!(text == "in"))
					{
						if (!(text == "deg"))
						{
							if (!(text == "rad"))
							{
								return value;
							}
							return value * (180.0 / Math.PI);
						}
						return value;
					}
					return value / 12.0;
				}
				return value;
			}
			return value / 0.3048;
		}
		return value / 304.8;
	}

	public bool SetParameterValue(object element, string parameterName, int value)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return false;
			}
			Parameter val2 = val.LookupParameter(parameterName);
			if (val2 == null)
			{
				return false;
			}
			if (((APIObject)val2).IsReadOnly)
			{
				return false;
			}
			if ((int)val2.StorageType == 1)
			{
				val2.Set(value);
				return true;
			}
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public bool SetParameterValueById(object element, string parameterName, int elementId)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return false;
			}
			Parameter val2 = val.LookupParameter(parameterName);
			if (val2 == null)
			{
				return false;
			}
			if (((APIObject)val2).IsReadOnly)
			{
				return false;
			}
			if ((int)val2.StorageType == 4)
			{
				val2.Set(new ElementId((long)elementId));
				return true;
			}
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public string? GetParameterName(object parameter)
	{
		try
		{
			Parameter val = (Parameter)((parameter is Parameter) ? parameter : null);
			if (val == null)
			{
				return null;
			}
			Definition definition = val.Definition;
			return (definition != null) ? definition.Name : null;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public string? GetParameterType(object parameter)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Parameter val = (Parameter)((parameter is Parameter) ? parameter : null);
			if (val == null)
			{
				return null;
			}
			return ((object)val.StorageType/*cast due to constrained. prefix*/).ToString();
		}
		catch (Exception)
		{
			try
			{
				Type type = parameter.GetType();
				PropertyInfo property = type.GetProperty("StorageType");
				if (property != null)
				{
					object value = property.GetValue(parameter);
					if (value != null)
					{
						return value.ToString();
					}
				}
			}
			catch (Exception ex2)
			{
				LogError("GetParameterType 反射访问也失败: " + ex2.Message);
			}
			return null;
		}
	}

	public string? GetParameterDefinitionType(object parameter)
	{
		try
		{
			Parameter val = (Parameter)((parameter is Parameter) ? parameter : null);
			if (val == null)
			{
				return null;
			}
			Definition definition = val.Definition;
			if (definition == null)
			{
				return null;
			}
			ForgeTypeId dataType = definition.GetDataType();
			if (dataType == SpecTypeId.Length)
			{
				return "Length";
			}
			if (dataType == SpecTypeId.Area)
			{
				return "Area";
			}
			if (dataType == SpecTypeId.Volume)
			{
				return "Volume";
			}
			if (dataType == SpecTypeId.Angle)
			{
				return "Angle";
			}
			if (dataType == SpecTypeId.Number)
			{
				return "Number";
			}
			if (dataType == SpecTypeId.String.Text)
			{
				return "Text";
			}
			if (dataType == SpecTypeId.Int.Integer)
			{
				return "Integer";
			}
			if (dataType == SpecTypeId.Boolean.YesNo)
			{
				return "YesNo";
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("获取参数定义类型失败: " + ex.Message);
			return null;
		}
	}

	public bool IsParameterReadOnly(object parameter)
	{
		try
		{
			Parameter val = (Parameter)((parameter is Parameter) ? parameter : null);
			if (val == null)
			{
				return true;
			}
			return ((APIObject)val).IsReadOnly;
		}
		catch (Exception)
		{
			return true;
		}
	}

	private static void LogError(string message)
	{
		Logger.Warning("[ParameterService] " + message);
	}
}
