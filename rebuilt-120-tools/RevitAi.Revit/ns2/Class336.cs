using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Units;
using ns0;
using ns6;

namespace ns2;

internal static class Class336
{
	public unsafe static Dictionary<UnitType, ProjectUnitInfo> smethod_0(object object_0)
	{
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Expected O, but got Unknown
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Expected I4, but got Unknown
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<UnitType, ProjectUnitInfo> dictionary = new Dictionary<UnitType, ProjectUnitInfo>();
		try
		{
			MethodInfo method = object_0.GetType().GetMethod("GetUnits");
			if (method == null)
			{
				return dictionary;
			}
			object obj = method.Invoke(object_0, null);
			if (obj == null)
			{
				return dictionary;
			}
			MethodInfo method2 = obj.GetType().GetMethod("GetFormatOptions");
			if (method2 == null)
			{
				return dictionary;
			}
			(UnitType, string, string, bool, double)[] array = new(UnitType, string, string, bool, double)[5]
			{
				((UnitType)1, "长度", "mm", true, 304.8),
				((UnitType)2, "面积", "m²", true, 0.09290304),
				((UnitType)3, "体积", "m³", true, 0.0283168466),
				((UnitType)4, "角度", "°", true, 1.0),
				((UnitType)5, "坡度", "%", true, 1.0)
			};
			(UnitType, string, string, bool, double)[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				(UnitType, string, string, bool, double) tuple = array2[i];
				var (val, displayName, _, _, _) = tuple;
				_ = tuple.Item3;
				_ = tuple.Item4;
				double item = tuple.Item5;
				try
				{
					Type type = smethod_4();
					if (type == null)
					{
						continue;
					}
					object obj2 = Enum.Parse(type, ((object)(*(UnitType*)(&val))/*cast due to constrained. prefix*/).ToString());
					object obj3 = method2.Invoke(obj, new object[1] { obj2 });
					if (obj3 == null)
					{
						continue;
					}
					PropertyInfo property = obj3.GetType().GetProperty("DisplayUnits");
					if (property == null)
					{
						continue;
					}
					object value = property.GetValue(obj3);
					if (value != null)
					{
						string displayUnitSymbol = smethod_1(value);
						ProjectUnitInfo val2 = new ProjectUnitInfo
						{
							UnitType = val,
							DisplayName = displayName,
							DisplayUnitType = value,
							DisplayUnitSymbol = displayUnitSymbol,
							IsMetric = smethod_2(value),
							Precision = smethod_3(obj3),
							FormatString = "0.00"
						};
						UnitType val3 = val;
						UnitType val4 = val3;
						switch ((int)(val4) - 1)
						{
						case 0:
							val2.FactorToMillimeters = item;
							break;
						case 1:
							val2.FactorToSquareMeters = item;
							break;
						case 2:
							val2.FactorToCubicMeters = item;
							break;
						case 3:
							val2.FactorToMillimeters = 1.0;
							break;
						case 4:
							val2.FactorToMillimeters = 1.0;
							break;
						}
						dictionary[val] = val2;
					}
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2);
					defaultInterpolatedStringHandler.AppendLiteral("获取单位类型 ");
					defaultInterpolatedStringHandler.AppendFormatted<UnitType>(val);
					defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
					Logger.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
		}
		catch (Exception ex2)
		{
			Logger.Debug("获取项目单位失败: " + ex2.Message);
		}
		return dictionary;
	}

	private static string smethod_1(object object_0)
	{
		if (object_0 == null)
		{
			return "unknown";
		}
		string text = object_0.ToString() ?? "unknown";
		if (text.Contains("MILLIMETERS") || text.Contains("millimeters"))
		{
			return "mm";
		}
		if (text.Contains("CENTIMETERS") || text.Contains("centimeters"))
		{
			return "cm";
		}
		if (text.Contains("METERS") || text.Contains("meters"))
		{
			return "m";
		}
		if (text.Contains("DECIMAL_FEET") || text.Contains("feet"))
		{
			return "ft";
		}
		if (text.Contains("SQUARE_METERS") || text.Contains("square_meters"))
		{
			return "m²";
		}
		if (text.Contains("SQUARE_FEET") || text.Contains("square_feet"))
		{
			return "ft²";
		}
		if (text.Contains("CUBIC_METERS") || text.Contains("cubic_meters"))
		{
			return "m³";
		}
		if (text.Contains("CUBIC_FEET") || text.Contains("cubic_feet"))
		{
			return "ft³";
		}
		if (text.Contains("DECIMAL_DEGREES") || text.Contains("degrees"))
		{
			return "°";
		}
		if (text.Contains("RADIANS") || text.Contains("radians"))
		{
			return "rad";
		}
		if (text.Contains("SLOPE_DEGREES"))
		{
			return "°";
		}
		if (text.Contains("SLOPE_PERCENT"))
		{
			return "%";
		}
		if (text.Contains("SLOPE_RATIO"))
		{
			return "ratio";
		}
		return text;
	}

	private static bool smethod_2(object object_0)
	{
		if (object_0 == null)
		{
			return false;
		}
		string text = object_0.ToString();
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		return text.Contains("MILLIMETERS") || text.Contains("CENTIMETERS") || text.Contains("METERS") || text.Contains("SQUARE_METERS") || text.Contains("CUBIC_METERS") || text.Contains("DECIMAL_DEGREES");
	}

	private static int smethod_3(object object_0)
	{
		try
		{
			PropertyInfo property = object_0.GetType().GetProperty("Precision");
			if (property != null && property.GetValue(object_0) is int result)
			{
				return result;
			}
			PropertyInfo property2 = object_0.GetType().GetProperty("Accuracy");
			if (property2 != null && property2.GetValue(object_0) is int result2)
			{
				return result2;
			}
		}
		catch
		{
		}
		return 2;
	}

	private static Type? smethod_4()
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		int num = 0;
		Type type;
		while (true)
		{
			if (num < assemblies.Length)
			{
				Assembly assembly = assemblies[num];
				if (assembly.FullName != null && assembly.FullName.Contains("RevitAPI"))
				{
					type = assembly.GetType("Autodesk.Revit.DB.UnitType");
					if (type != null)
					{
						break;
					}
				}
				num++;
				continue;
			}
			return null;
		}
		return type;
	}

	public static object smethod_5()
	{
		return smethod_12("DUT_MILLIMETERS");
	}

	public static object smethod_6()
	{
		return smethod_12("DUT_SQUARE_METERS");
	}

	public static object smethod_7()
	{
		return smethod_12("DUT_CUBIC_METERS");
	}

	public static object smethod_8()
	{
		return smethod_12("DUT_DECIMAL_DEGREES");
	}

	public static object smethod_9()
	{
		return smethod_12("DUT_DECIMAL_FEET");
	}

	public static object smethod_10()
	{
		return smethod_12("DUT_SQUARE_FEET");
	}

	public static object smethod_11()
	{
		return smethod_12("DUT_CUBIC_FEET");
	}

	private static object? smethod_12(string string_0)
	{
		try
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				if (assembly.FullName == null || !assembly.FullName.Contains("RevitAPI"))
				{
					continue;
				}
				Type type = assembly.GetType("Autodesk.Revit.DB.DisplayUnitType");
				if (!(type != null))
				{
					Type type2 = assembly.GetType("Autodesk.Revit.DB.ForgeTypeId");
					if (!(type2 != null))
					{
						continue;
					}
					Type type3 = assembly.GetType("Autodesk.Revit.DB.UnitTypeId");
					if (type3 != null)
					{
						string name = smethod_13(string_0);
						PropertyInfo property = type3.GetProperty(name);
						if (property != null)
						{
							return property.GetValue(null);
						}
					}
					continue;
				}
				return Enum.Parse(type, string_0);
			}
		}
		catch (Exception ex)
		{
			Logger.Debug("获取 DisplayUnitType " + string_0 + " 失败: " + ex.Message);
		}
		return null;
	}

	private static string smethod_13(string string_0)
	{
		switch (Class653.smethod_0(string_0))
		{
		case 300347416u:
			if (string_0 == "DUT_SLOPE_PERCENT")
			{
				return "Percent";
			}
			goto default;
		case 5582855u:
			if (string_0 == "DUT_DECIMAL_FEET")
			{
				return "Feet";
			}
			goto default;
		case 454027395u:
			if (string_0 == "DUT_CUBIC_CENTIMETERS")
			{
				return "CubicCentimeters";
			}
			goto default;
		case 347098874u:
			if (string_0 == "DUT_DECIMETERS")
			{
				return "Decimeters";
			}
			goto default;
		case 822664355u:
			if (string_0 == "DUT_RADIANS")
			{
				return "Radians";
			}
			goto default;
		case 605204430u:
			if (string_0 == "DUT_CENTIMETERS")
			{
				return "Centimeters";
			}
			goto default;
		case 1633326568u:
			if (string_0 == "DUT_SQUARE_CENTIMETERS")
			{
				return "SquareCentimeters";
			}
			goto default;
		case 1530009958u:
			if (string_0 == "DUT_CUBIC_METERS")
			{
				return "CubicMeters";
			}
			goto default;
		case 1131545868u:
			if (string_0 == "DUT_SLOPE_DEGREES")
			{
				return "SlopeDegrees";
			}
			goto default;
		case 1941759067u:
			if (string_0 == "DUT_SQUARE_METERS")
			{
				return "SquareMeters";
			}
			goto default;
		case 1776606750u:
			if (string_0 == "DUT_SLOPE_RATIO")
			{
				return "SlopeRatio";
			}
			goto default;
		case 2992798011u:
			if (string_0 == "DUT_DECIMAL_INCHES")
			{
				return "Inches";
			}
			goto default;
		case 2613680634u:
			if (string_0 == "DUT_MILLIMETERS")
			{
				return "Millimeters";
			}
			goto default;
		case 2119350740u:
			if (string_0 == "DUT_SQUARE_MILLIMETERS")
			{
				return "SquareMillimeters";
			}
			goto default;
		case 3581212325u:
			if (string_0 == "DUT_METERS")
			{
				return "Meters";
			}
			goto default;
		case 3484777996u:
			if (string_0 == "DUT_CUBIC_FEET")
			{
				return "CubicFeet";
			}
			goto default;
		case 4105302813u:
			if (string_0 == "DUT_SQUARE_FEET")
			{
				return "SquareFeet";
			}
			goto default;
		case 3666026956u:
			if (string_0 == "DUT_DECIMAL_DEGREES")
			{
				return "Degrees";
			}
			goto default;
		case 3588641031u:
			if (string_0 == "DUT_CUBIC_MILLIMETERS")
			{
				return "CubicMillimeters";
			}
			goto default;
		default:
			return string_0;
		}
	}

	public static object? smethod_14(string string_0)
	{
		if (string.IsNullOrWhiteSpace(string_0))
		{
			return null;
		}
		string text = string_0.ToLower().Trim().Replace(" ", "")
			.Replace("　", "");
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{
				"毫米",
				"DUT_MILLIMETERS"
			},
			{
				"millimeters",
				"DUT_MILLIMETERS"
			},
			{
				"millimeter",
				"DUT_MILLIMETERS"
			},
			{
				"mm",
				"DUT_MILLIMETERS"
			},
			{
				"厘米",
				"DUT_CENTIMETERS"
			},
			{
				"centimeters",
				"DUT_CENTIMETERS"
			},
			{
				"centimeter",
				"DUT_CENTIMETERS"
			},
			{
				"cm",
				"DUT_CENTIMETERS"
			},
			{
				"米",
				"DUT_METERS"
			},
			{
				"meters",
				"DUT_METERS"
			},
			{
				"meter",
				"DUT_METERS"
			},
			{
				"m",
				"DUT_METERS"
			},
			{
				"分米",
				"DUT_DECIMETERS"
			},
			{
				"decimeters",
				"DUT_DECIMETERS"
			},
			{
				"decimeter",
				"DUT_DECIMETERS"
			},
			{
				"dm",
				"DUT_DECIMETERS"
			},
			{
				"英尺",
				"DUT_DECIMAL_FEET"
			},
			{
				"feet",
				"DUT_DECIMAL_FEET"
			},
			{
				"ft",
				"DUT_DECIMAL_FEET"
			},
			{
				"英寸",
				"DUT_DECIMAL_INCHES"
			},
			{
				"inches",
				"DUT_DECIMAL_INCHES"
			},
			{
				"inch",
				"DUT_DECIMAL_INCHES"
			},
			{
				"in",
				"DUT_DECIMAL_INCHES"
			}
		};
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>
		{
			{
				"平方米",
				"DUT_SQUARE_METERS"
			},
			{
				"square_meters",
				"DUT_SQUARE_METERS"
			},
			{
				"squaremeter",
				"DUT_SQUARE_METERS"
			},
			{
				"m²",
				"DUT_SQUARE_METERS"
			},
			{
				"m2",
				"DUT_SQUARE_METERS"
			},
			{
				"平方毫米",
				"DUT_SQUARE_MILLIMETERS"
			},
			{
				"square_millimeters",
				"DUT_SQUARE_MILLIMETERS"
			},
			{
				"squaremillimeter",
				"DUT_SQUARE_MILLIMETERS"
			},
			{
				"mm²",
				"DUT_SQUARE_MILLIMETERS"
			},
			{
				"mm2",
				"DUT_SQUARE_MILLIMETERS"
			},
			{
				"平方厘米",
				"DUT_SQUARE_CENTIMETERS"
			},
			{
				"square_centimeters",
				"DUT_SQUARE_CENTIMETERS"
			},
			{
				"squarecentimeter",
				"DUT_SQUARE_CENTIMETERS"
			},
			{
				"cm²",
				"DUT_SQUARE_CENTIMETERS"
			},
			{
				"cm2",
				"DUT_SQUARE_CENTIMETERS"
			},
			{
				"平方英尺",
				"DUT_SQUARE_FEET"
			},
			{
				"square_feet",
				"DUT_SQUARE_FEET"
			},
			{
				"squarefeet",
				"DUT_SQUARE_FEET"
			},
			{
				"ft²",
				"DUT_SQUARE_FEET"
			},
			{
				"ft2",
				"DUT_SQUARE_FEET"
			}
		};
		Dictionary<string, string> dictionary3 = new Dictionary<string, string>
		{
			{
				"立方米",
				"DUT_CUBIC_METERS"
			},
			{
				"cubic_meters",
				"DUT_CUBIC_METERS"
			},
			{
				"cubicmeter",
				"DUT_CUBIC_METERS"
			},
			{
				"m³",
				"DUT_CUBIC_METERS"
			},
			{
				"m3",
				"DUT_CUBIC_METERS"
			},
			{
				"立方毫米",
				"DUT_CUBIC_MILLIMETERS"
			},
			{
				"cubic_millimeters",
				"DUT_CUBIC_MILLIMETERS"
			},
			{
				"cubicmillimeter",
				"DUT_CUBIC_MILLIMETERS"
			},
			{
				"mm³",
				"DUT_CUBIC_MILLIMETERS"
			},
			{
				"mm3",
				"DUT_CUBIC_MILLIMETERS"
			},
			{
				"立方厘米",
				"DUT_CUBIC_CENTIMETERS"
			},
			{
				"cubic_centimeters",
				"DUT_CUBIC_CENTIMETERS"
			},
			{
				"cubiccentimeter",
				"DUT_CUBIC_CENTIMETERS"
			},
			{
				"cm³",
				"DUT_CUBIC_CENTIMETERS"
			},
			{
				"cm3",
				"DUT_CUBIC_CENTIMETERS"
			},
			{
				"立方英尺",
				"DUT_CUBIC_FEET"
			},
			{
				"cubic_feet",
				"DUT_CUBIC_FEET"
			},
			{
				"cubicfeet",
				"DUT_CUBIC_FEET"
			},
			{
				"ft³",
				"DUT_CUBIC_FEET"
			},
			{
				"ft3",
				"DUT_CUBIC_FEET"
			}
		};
		Dictionary<string, string> dictionary4 = new Dictionary<string, string>
		{
			{
				"度",
				"DUT_DECIMAL_DEGREES"
			},
			{
				"degrees",
				"DUT_DECIMAL_DEGREES"
			},
			{
				"degree",
				"DUT_DECIMAL_DEGREES"
			},
			{
				"decimal_degrees",
				"DUT_DECIMAL_DEGREES"
			},
			{
				"°",
				"DUT_DECIMAL_DEGREES"
			},
			{
				"弧度",
				"DUT_RADIANS"
			},
			{
				"radians",
				"DUT_RADIANS"
			},
			{
				"radian",
				"DUT_RADIANS"
			},
			{
				"rad",
				"DUT_RADIANS"
			}
		};
		Dictionary<string, string> dictionary5 = new Dictionary<string, string>
		{
			{
				"百分比",
				"DUT_SLOPE_PERCENT"
			},
			{
				"percent",
				"DUT_SLOPE_PERCENT"
			},
			{
				"%",
				"DUT_SLOPE_PERCENT"
			},
			{
				"slope_percent",
				"DUT_SLOPE_PERCENT"
			},
			{
				"坡度角度",
				"DUT_SLOPE_DEGREES"
			},
			{
				"slope_degrees",
				"DUT_SLOPE_DEGREES"
			},
			{
				"gradient_degrees",
				"DUT_SLOPE_DEGREES"
			},
			{
				"比例",
				"DUT_SLOPE_RATIO"
			},
			{
				"ratio",
				"DUT_SLOPE_RATIO"
			},
			{
				"slope_ratio",
				"DUT_SLOPE_RATIO"
			},
			{
				"gradient_ratio",
				"DUT_SLOPE_RATIO"
			}
		};
		Dictionary<string, string> dictionary6 = new Dictionary<string, string>();
		foreach (KeyValuePair<string, string> item in dictionary)
		{
			dictionary6[item.Key] = item.Value;
		}
		foreach (KeyValuePair<string, string> item2 in dictionary2)
		{
			dictionary6[item2.Key] = item2.Value;
		}
		foreach (KeyValuePair<string, string> item3 in dictionary3)
		{
			dictionary6[item3.Key] = item3.Value;
		}
		foreach (KeyValuePair<string, string> item4 in dictionary4)
		{
			dictionary6[item4.Key] = item4.Value;
		}
		foreach (KeyValuePair<string, string> item5 in dictionary5)
		{
			dictionary6[item5.Key] = item5.Value;
		}
		if (dictionary6.TryGetValue(text, out var value))
		{
			return smethod_12(value);
		}
		foreach (KeyValuePair<string, string> item6 in dictionary6)
		{
			if (text.Contains(item6.Key) || item6.Key.Contains(text))
			{
				return smethod_12(item6.Value);
			}
		}
		Logger.Warning("[RevitApiBridge] 无法识别单位名称: " + string_0);
		return null;
	}
}
