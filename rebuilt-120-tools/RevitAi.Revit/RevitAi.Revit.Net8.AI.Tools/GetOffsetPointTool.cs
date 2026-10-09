using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_offset_point", Category = "元素查询", Description = "从基准点向指定方向偏移一定距离，获取新位置坐标。支持方位词（北、南、东、西等）和角度。返回单位：毫米", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetOffsetPointTool : IAITool
{
	private readonly IElementService ielementService_0;

	public string Name => "get_offset_point";

	public string Category => "元素查询";

	public string Description => "从基准点向指定方向偏移一定距离，获取新位置坐标";

	public string ParametersSchema => "\r\n{\r\n    \"type\": \"object\",\r\n    \"properties\": {\r\n        \"base_point\": {\r\n            \"type\": \"object\",\r\n            \"description\": \"基准点坐标 {x, y, z}，单位：毫米\",\r\n            \"properties\": {\r\n                \"x\": {\"type\": \"number\"},\r\n                \"y\": {\"type\": \"number\"},\r\n                \"z\": {\"type\": \"number\"}\r\n            },\r\n            \"required\": [\"x\", \"y\", \"z\"]\r\n        },\r\n        \"direction\": {\r\n            \"type\": \"string\",\r\n            \"description\": \"方向：方位词（北、南、东、西、东北、西北、东南、西南）或角度（0-360度，0度为东，逆时针）。常见表达转换：北偏东30度=60度，北偏西45度=135度，南偏东20度=290度，南偏西60度=240度\"\r\n        },\r\n        \"distance\": {\r\n            \"type\": \"number\",\r\n            \"description\": \"偏移距离，单位：毫米\"\r\n        },\r\n        \"angle\": {\r\n            \"type\": \"number\",\r\n            \"description\": \"可选：直接指定角度（0-360度），与 direction 参数二选一。角度系统：0度=东，90度=北，180度=西，270度=南，逆时针为正方向\"\r\n        }\r\n    },\r\n    \"required\": [\"base_point\", \"direction\", \"distance\"]\r\n}";

	public GetOffsetPointTool(IElementService elementService)
	{
		ielementService_0 = elementService ?? throw new ArgumentNullException("elementService");
	}

	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken)
	{
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Expected O, but got Unknown
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Expected O, but got Unknown
		try
		{
			if (!context.Parameters.TryGetValue("base_point", out var value) || value == null)
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("缺少必需参数: base_point"));
			}
			if (!method_0(value, out (double, double, double) valueTuple_))
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("base_point 参数格式错误，应为包含 x, y, z 的对象"));
			}
			if (!context.Parameters.TryGetValue("distance", out var value2) || value2 == null)
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("缺少必需参数: distance"));
			}
			if (!double.TryParse(value2.ToString(), out var result))
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("distance 参数格式错误"));
			}
			if (result <= 0.0)
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("distance 必须大于0"));
			}
			double double_;
			if (context.Parameters.TryGetValue("angle", out var value3) && value3 != null)
			{
				if (!double.TryParse(value3.ToString(), out double_))
				{
					return Task.FromResult<AIToolResult>(AIToolResult.Fail("angle 参数格式错误"));
				}
			}
			else
			{
				if (!context.Parameters.TryGetValue("direction", out var value4) || value4 == null)
				{
					return Task.FromResult<AIToolResult>(AIToolResult.Fail("缺少必需参数: direction 或 angle"));
				}
				string text = value4.ToString() ?? "";
				if (!method_2(text, out double_))
				{
					return Task.FromResult<AIToolResult>(AIToolResult.Fail("无法识别的方向: " + text + "。支持的方位词：北、南、东、西、东北、西北、东南、西南，或直接使用0-360度角度"));
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 5);
			defaultInterpolatedStringHandler.AppendLiteral("[GetOffsetPoint] 计算偏移点，基准点: (");
			defaultInterpolatedStringHandler.AppendFormatted(valueTuple_.Item1);
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(valueTuple_.Item2);
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(valueTuple_.Item3);
			defaultInterpolatedStringHandler.AppendLiteral(")，方向: ");
			defaultInterpolatedStringHandler.AppendFormatted(double_);
			defaultInterpolatedStringHandler.AppendLiteral("°，距离: ");
			defaultInterpolatedStringHandler.AppendFormatted(result);
			defaultInterpolatedStringHandler.AppendLiteral("mm");
			Logger.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			XYZ val = new XYZ(valueTuple_.Item1 / 304.8, valueTuple_.Item2 / 304.8, valueTuple_.Item3 / 304.8);
			double num = result / 304.8;
			double num2 = double_ * Math.PI / 180.0;
			XYZ val2 = new XYZ(Math.Cos(num2) * num, Math.Sin(num2) * num, 0.0);
			XYZ val3 = val + val2;
			Class35<double, double, double, string> gparam_ = new Class35<double, double, double, string>(Math.Round(val3.X * 304.8, 0), Math.Round(val3.Y * 304.8, 0), Math.Round(val3.Z * 304.8, 0), "millimeters");
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(29, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("成功计算偏移点位置：从基准点向");
			defaultInterpolatedStringHandler2.AppendFormatted(double_);
			defaultInterpolatedStringHandler2.AppendLiteral("°方向偏移");
			defaultInterpolatedStringHandler2.AppendFormatted(result);
			defaultInterpolatedStringHandler2.AppendLiteral("毫米（单位：毫米）");
			return Task.FromResult<AIToolResult>(AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class189<Class35<double, double, double, string>, double, double, Class35<double, double, double, string>>(new Class35<double, double, double, string>(Math.Round(valueTuple_.Item1, 0), Math.Round(valueTuple_.Item2, 0), Math.Round(valueTuple_.Item3, 0), "millimeters"), double_, result, gparam_)));
		}
		catch (Exception ex)
		{
			Logger.Error("[GetOffsetPoint] 执行失败", ex);
			return Task.FromResult<AIToolResult>(AIToolResult.Fail("计算偏移点失败: " + ex.Message));
		}
	}

	private bool method_0(object object_0, out (double X, double Y, double Z) valueTuple_0)
	{
		valueTuple_0 = (X: 0.0, Y: 0.0, Z: 0.0);
		try
		{
			if (object_0 == null)
			{
				return false;
			}
			Type type = object_0.GetType();
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<, >) && object_0 is Dictionary<string, object> dictionary)
			{
				if (method_1(dictionary, "x", out var double_) && method_1(dictionary, "y", out var double_2) && method_1(dictionary, "z", out var double_3))
				{
					valueTuple_0 = (X: double_, Y: double_2, Z: double_3);
					return true;
				}
				string[] array = new string[5]
				{
					"intersection",
					"location",
					"point",
					"position",
					"coordinates"
				};
				string[] array2 = array;
				int num = 0;
				while (true)
				{
					if (num < array2.Length)
					{
						string key = array2[num];
						if (dictionary.TryGetValue(key, out var value) && value != null && method_0(value, out valueTuple_0))
						{
							break;
						}
						num++;
						continue;
					}
					return false;
				}
				return true;
			}
			PropertyInfo propertyInfo = type.GetProperty("x") ?? type.GetProperty("X");
			PropertyInfo propertyInfo2 = type.GetProperty("y") ?? type.GetProperty("Y");
			PropertyInfo propertyInfo3 = type.GetProperty("z") ?? type.GetProperty("Z");
			if (propertyInfo != null && propertyInfo2 != null && propertyInfo3 != null)
			{
				double item = Convert.ToDouble(propertyInfo.GetValue(object_0));
				double item2 = Convert.ToDouble(propertyInfo2.GetValue(object_0));
				double item3 = Convert.ToDouble(propertyInfo3.GetValue(object_0));
				valueTuple_0 = (X: item, Y: item2, Z: item3);
				return true;
			}
			string[] array3 = new string[5]
			{
				"intersection",
				"location",
				"point",
				"position",
				"coordinates"
			};
			string[] array4 = array3;
			int num2 = 0;
			object value2;
			PropertyInfo propertyInfo4;
			PropertyInfo propertyInfo5;
			PropertyInfo propertyInfo6;
			while (true)
			{
				if (num2 < array4.Length)
				{
					string name = array4[num2];
					PropertyInfo property = type.GetProperty(name);
					if (property != null)
					{
						value2 = property.GetValue(object_0);
						if (value2 != null)
						{
							Type type2 = value2.GetType();
							propertyInfo4 = type2.GetProperty("x") ?? type2.GetProperty("X");
							propertyInfo5 = type2.GetProperty("y") ?? type2.GetProperty("Y");
							propertyInfo6 = type2.GetProperty("z") ?? type2.GetProperty("Z");
							if (propertyInfo4 != null && propertyInfo5 != null && propertyInfo6 != null)
							{
								break;
							}
						}
					}
					num2++;
					continue;
				}
				return false;
			}
			double item4 = Convert.ToDouble(propertyInfo4.GetValue(value2));
			double item5 = Convert.ToDouble(propertyInfo5.GetValue(value2));
			double item6 = Convert.ToDouble(propertyInfo6.GetValue(value2));
			valueTuple_0 = (X: item4, Y: item5, Z: item6);
			return true;
		}
		catch
		{
			return false;
		}
	}

	private bool method_1(Dictionary<string, object> dictionary_0, string string_0, out double double_0)
	{
		double_0 = 0.0;
		if (dictionary_0.TryGetValue(string_0, out object value) && value != null && double.TryParse(value.ToString(), out double_0))
		{
			return true;
		}
		foreach (KeyValuePair<string, object> item in dictionary_0)
		{
			if (string.Equals(item.Key, string_0, StringComparison.OrdinalIgnoreCase) && item.Value != null && double.TryParse(item.Value.ToString(), out double_0))
			{
				return true;
			}
		}
		return false;
	}

	private bool method_2(string string_0, out double double_0)
	{
		double_0 = 0.0;
		if (string.IsNullOrWhiteSpace(string_0))
		{
			return false;
		}
		string text = string_0.Trim().ToLower();
		string text2 = text;
		string text3 = text2;
		uint num = Class653.smethod_0(text3);
		if (num <= 1479958588)
		{
			if (num <= 660332187)
			{
				if (num <= 237448570)
				{
					if (num != 5468732)
					{
						if (num != 143421009)
						{
							if (num == 237448570 && text3 == "south")
							{
								goto IL_0475;
							}
						}
						else if (text3 == "northeast")
						{
							goto IL_02a2;
						}
					}
					else if (text3 == "east")
					{
						goto IL_0310;
					}
				}
				else if (num != 307860630)
				{
					if (num != 425819803)
					{
						if (num == 660332187 && text3 == "西北")
						{
							goto IL_03e6;
						}
					}
					else if (text3 == "东")
					{
						goto IL_0310;
					}
				}
				else if (text3 == "北")
				{
					goto IL_0391;
				}
			}
			else if (num <= 1245071922)
			{
				if (num != 1012009637)
				{
					if (num != 1210827699)
					{
						if (num == 1245071922 && text3 == "nw")
						{
							goto IL_03e6;
						}
					}
					else if (text3 == "southeast")
					{
						goto IL_027b;
					}
				}
				else if (text3 == "se")
				{
					goto IL_027b;
				}
			}
			else if (num != 1246896303)
			{
				if (num != 1466833169)
				{
					if (num == 1479958588 && text3 == "ne")
					{
						goto IL_02a2;
					}
				}
				else if (text3 == "southwest")
				{
					goto IL_02e9;
				}
			}
			else if (text3 == "sw")
			{
				goto IL_02e9;
			}
		}
		else if (num <= 3881531867u)
		{
			if (num <= 3529060310u)
			{
				if (num != 2147136356)
				{
					if (num != 3220903972u)
					{
						if (num == 3529060310u && text3 == "南")
						{
							goto IL_0475;
						}
					}
					else if (text3 == "东南")
					{
						goto IL_027b;
					}
				}
				else if (text3 == "东北")
				{
					goto IL_02a2;
				}
			}
			else if (num != 3673133828u)
			{
				if (num != 3758891744u)
				{
					if (num == 3881531867u && text3 == "西南")
					{
						goto IL_02e9;
					}
				}
				else if (text3 == "e")
				{
					goto IL_0310;
				}
			}
			else if (text3 == "north")
			{
				goto IL_0391;
			}
		}
		else if (num <= 4060888886u)
		{
			if (num != 3941157294u)
			{
				if (num != 3943445553u)
				{
					if (num == 4060888886u && text3 == "w")
					{
						goto IL_040a;
					}
				}
				else if (text3 == "n")
				{
					goto IL_0391;
				}
			}
			else if (text3 == "west")
			{
				goto IL_040a;
			}
		}
		else if (num != 4127999362u)
		{
			if (num != 4180975822u)
			{
				if (num == 4182382835u && text3 == "northwest")
				{
					goto IL_03e6;
				}
			}
			else if (text3 == "西")
			{
				goto IL_040a;
			}
		}
		else if (text3 == "s")
		{
			goto IL_0475;
		}
		if (method_3(text, out double_0))
		{
			return true;
		}
		if (double.TryParse(string_0, out double_0))
		{
			double_0 %= 360.0;
			if (double_0 < 0.0)
			{
				double_0 += 360.0;
			}
			return true;
		}
		return false;
		IL_03e6:
		double_0 = 135.0;
		return true;
		IL_027b:
		double_0 = 315.0;
		return true;
		IL_02a2:
		double_0 = 45.0;
		return true;
		IL_040a:
		double_0 = 180.0;
		return true;
		IL_02e9:
		double_0 = 225.0;
		return true;
		IL_0391:
		double_0 = 90.0;
		return true;
		IL_0310:
		double_0 = 0.0;
		return true;
		IL_0475:
		double_0 = 270.0;
		return true;
	}

	private bool method_3(string string_0, out double double_0)
	{
		double_0 = 0.0;
		try
		{
			string[] array = new string[16]
			{
				"北偏东(\\d+(?:\\.\\d+)?)度?",
				"北偏西(\\d+(?:\\.\\d+)?)度?",
				"南偏东(\\d+(?:\\.\\d+)?)度?",
				"南偏西(\\d+(?:\\.\\d+)?)度?",
				"东偏北(\\d+(?:\\.\\d+)?)度?",
				"东偏南(\\d+(?:\\.\\d+)?)度?",
				"西偏北(\\d+(?:\\.\\d+)?)度?",
				"西偏南(\\d+(?:\\.\\d+)?)度?",
				"north\\s+by\\s+east\\s+(\\d+(?:\\.\\d+)?)",
				"north\\s+by\\s+west\\s+(\\d+(?:\\.\\d+)?)",
				"south\\s+by\\s+east\\s+(\\d+(?:\\.\\d+)?)",
				"south\\s+by\\s+west\\s+(\\d+(?:\\.\\d+)?)",
				"east\\s+by\\s+north\\s+(\\d+(?:\\.\\d+)?)",
				"east\\s+by\\s+south\\s+(\\d+(?:\\.\\d+)?)",
				"west\\s+by\\s+north\\s+(\\d+(?:\\.\\d+)?)",
				"west\\s+by\\s+south\\s+(\\d+(?:\\.\\d+)?)"
			};
			double[] array2 = new double[16]
			{
				90.0, 90.0, 270.0, 270.0, 0.0, 0.0, 180.0, 180.0, 90.0, 90.0,
				270.0, 270.0, 0.0, 0.0, 180.0, 180.0
			};
			double[] array3 = new double[16]
			{
				-1.0, 1.0, 1.0, -1.0, 1.0, -1.0, -1.0, 1.0, -1.0, 1.0,
				1.0, -1.0, 1.0, -1.0, -1.0, 1.0
			};
			int num = 0;
			double result;
			while (true)
			{
				if (num < array.Length)
				{
					Match match = Regex.Match(string_0, array[num], RegexOptions.IgnoreCase);
					if (match.Success && double.TryParse(match.Groups[1].Value, out result))
					{
						break;
					}
					num++;
					continue;
				}
				return false;
			}
			double_0 = array2[num] + result * array3[num];
			double_0 %= 360.0;
			if (double_0 < 0.0)
			{
				double_0 += 360.0;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[GetOffsetPoint] 解析相对方位: ");
			defaultInterpolatedStringHandler.AppendFormatted(string_0);
			defaultInterpolatedStringHandler.AppendLiteral(" -> ");
			defaultInterpolatedStringHandler.AppendFormatted(double_0);
			defaultInterpolatedStringHandler.AppendLiteral("°");
			Logger.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			return true;
		}
		catch
		{
			return false;
		}
	}
}
