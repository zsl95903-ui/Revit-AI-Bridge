using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Services;

internal sealed class GeometryService : IGeometryService
{
	private readonly UIApplication _application;

	public GeometryService(UIApplication application)
	{
		_application = application ?? throw new ArgumentNullException("application");
	}

	public double? GetWallTopLevel(object wall)
	{
		try
		{
			Wall val = (Wall)((wall is Wall) ? wall : null);
			if (val == null)
			{
				LogError("GetWallTopLevel: wall 参数无效");
				return null;
			}
			Parameter val2 = null;
			Parameter val3 = null;
			string[] array = new string[3]
			{
				"Top Constraint",
				"Top Level",
				"Upper Level"
			};
			foreach (string text in array)
			{
				val2 = ((Element)val).LookupParameter(text);
				if (val2 != null && val2.HasValue)
				{
					break;
				}
			}
			if (val2 == null || !val2.HasValue)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
				defaultInterpolatedStringHandler.AppendLiteral("GetWallTopLevel: 墙 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)val).Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 未设置顶部约束");
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			string[] array2 = new string[3]
			{
				"Top Offset",
				"Top Offset Distance",
				"Unconnected Height"
			};
			foreach (string text2 in array2)
			{
				val3 = ((Element)val).LookupParameter(text2);
				if (val3 != null && val3.HasValue)
				{
					break;
				}
			}
			double num = ((val3 != null) ? val3.AsDouble() : 0.0);
			Document document = ((Element)val).Document;
			ElementId val4 = val2.AsElementId();
			Element element = document.GetElement(val4);
			Level val5 = (Level)(object)((element is Level) ? element : null);
			if (val5 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(31, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("GetWallTopLevel: 无法获取墙 ");
				defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(((Element)val).Id);
				defaultInterpolatedStringHandler2.AppendLiteral(" 的顶部标高对象");
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			return val5.Elevation + num;
		}
		catch (Exception ex)
		{
			LogError("GetWallTopLevel 失败: " + ex.Message);
			return null;
		}
	}

	public double? GetFloorBottomLevel(object floor)
	{
		try
		{
			Floor val = (Floor)((floor is Floor) ? floor : null);
			if (val == null)
			{
				LogError("GetFloorBottomLevel: floor 参数无效");
				return null;
			}
			Parameter val2 = null;
			string[] array = new string[4]
			{
				"Level",
				"Level Id",
				"Reference Level",
				"Floor Height"
			};
			foreach (string text in array)
			{
				val2 = ((Element)val).LookupParameter(text);
				if (val2 != null && val2.HasValue)
				{
					break;
				}
			}
			if (val2 != null && val2.HasValue)
			{
				Document document = ((Element)val).Document;
				ElementId val3 = val2.AsElementId();
				Element element = document.GetElement(val3);
				Level val4 = (Level)(object)((element is Level) ? element : null);
				if (val4 != null)
				{
					return val4.Elevation;
				}
			}
			BoundingBoxXYZ val5 = ((Element)val).get_BoundingBox((View)null);
			if (val5 != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
				defaultInterpolatedStringHandler.AppendLiteral("从边界框获取楼板 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)val).Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 底部标高: ");
				defaultInterpolatedStringHandler.AppendFormatted(val5.Min.Z, "F3");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
				return val5.Min.Z;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(42, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("GetFloorBottomLevel: 楼板 ");
			defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(((Element)val).Id);
			defaultInterpolatedStringHandler2.AppendLiteral(" 无法获取标高（参数和边界框都失败）");
			LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetFloorBottomLevel 失败: " + ex.Message);
			return null;
		}
	}

	public bool IsWallUnderFloor(object wall, object floor)
	{
		try
		{
			Element val = (Element)((wall is Element) ? wall : null);
			if (val != null)
			{
				Element val2 = (Element)((floor is Element) ? floor : null);
				if (val2 != null)
				{
					BoundingBoxXYZ val3 = val.get_BoundingBox((View)null);
					BoundingBoxXYZ val4 = val2.get_BoundingBox((View)null);
					if (val3 == null || val4 == null)
					{
						return false;
					}
					return val3.Min.X < val4.Max.X && val3.Max.X > val4.Min.X && val3.Min.Y < val4.Max.Y && val3.Max.Y > val4.Min.Y;
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			LogError("IsWallUnderFloor 失败: " + ex.Message);
			return false;
		}
	}

	public bool ModifyWallTopLevel(object document, object wall, double newLevel)
	{
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val != null)
			{
				Wall val2 = (Wall)((wall is Wall) ? wall : null);
				if (val2 != null)
				{
					Transaction val3 = new Transaction(val, "平齐墙顶到楼板");
					try
					{
						val3.Start();
						try
						{
							Level val4 = FindOrCreateLevel(val, newLevel);
							if (val4 == null)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
								defaultInterpolatedStringHandler.AppendLiteral("ModifyWallTopLevel: 无法创建或查找标高 ");
								defaultInterpolatedStringHandler.AppendFormatted(newLevel);
								LogError(defaultInterpolatedStringHandler.ToStringAndClear());
								val3.RollBack();
								return false;
							}
							Parameter val5 = null;
							string[] array = new string[3]
							{
								"Top Constraint",
								"Top Level",
								"Upper Level"
							};
							foreach (string text in array)
							{
								val5 = ((Element)val2).LookupParameter(text);
								if (val5 != null && val5.HasValue)
								{
									break;
								}
							}
							if (val5 != null && val5.HasValue)
							{
								val5.Set(((Element)val4).Id);
								Parameter val6 = null;
								string[] array2 = new string[3]
								{
									"Top Offset",
									"Top Offset Distance",
									"Unconnected Height"
								};
								foreach (string text2 in array2)
								{
									val6 = ((Element)val2).LookupParameter(text2);
									if (val6 != null && val6.HasValue)
									{
										break;
									}
								}
								if (val6 != null && val6.HasValue)
								{
									val6.Set(0.0);
								}
								val3.Commit();
								return true;
							}
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(31, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("ModifyWallTopLevel: 墙 ");
							defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(((Element)val2).Id);
							defaultInterpolatedStringHandler2.AppendLiteral(" 没有顶部约束参数");
							LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
							val3.RollBack();
							return false;
						}
						catch (Exception ex)
						{
							LogError("ModifyWallTopLevel: 事务执行失败 - " + ex.Message);
							val3.RollBack();
							return false;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
			}
			LogError("ModifyWallTopLevel: 参数无效");
			return false;
		}
		catch (Exception ex2)
		{
			LogError("ModifyWallTopLevel 失败: " + ex2.Message);
			return false;
		}
	}

	private Level? FindOrCreateLevel(Document doc, double elevation)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Level val = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(Level))).Cast<Level>().FirstOrDefault((Level l) => Math.Abs(l.Elevation - elevation) < 0.001);
			if (val == null)
			{
				try
				{
					Level val2 = Level.Create(doc, elevation);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
					defaultInterpolatedStringHandler.AppendLiteral("创建新标高: ");
					defaultInterpolatedStringHandler.AppendFormatted(elevation, "F3");
					defaultInterpolatedStringHandler.AppendLiteral(" (");
					defaultInterpolatedStringHandler.AppendFormatted(((Element)val2).Id.Value);
					defaultInterpolatedStringHandler.AppendLiteral(")");
					LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
					return val2;
				}
				catch (Exception ex)
				{
					LogError("FindOrCreateLevel: 创建标高失败 - " + ex.Message);
					return null;
				}
			}
			return val;
		}
		catch (Exception ex2)
		{
			LogError("FindOrCreateLevel 失败: " + ex2.Message);
			return null;
		}
	}

	private void LogError(string message)
	{
		try
		{
			Logger.Error("[GeometryService] " + message);
		}
		catch
		{
		}
	}

	public double? GetDistanceBetweenElements(object element1, object element2)
	{
		try
		{
			(double, double, double)? elementLocation = GetElementLocation(element1);
			(double, double, double)? elementLocation2 = GetElementLocation(element2);
			if (elementLocation.HasValue && elementLocation2.HasValue)
			{
				double num = elementLocation2.Value.Item1 - elementLocation.Value.Item1;
				double num2 = elementLocation2.Value.Item2 - elementLocation.Value.Item2;
				double num3 = elementLocation2.Value.Item3 - elementLocation.Value.Item3;
				return Math.Sqrt(num * num + num2 * num2 + num3 * num3);
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetDistanceBetweenElements 失败: " + ex.Message);
			return null;
		}
	}

	public double GetDistanceBetweenPoints(object point1, object point2)
	{
		try
		{
			if (!ParsePoint(point1, out XYZ xyz) || xyz == null)
			{
				LogError("GetDistanceBetweenPoints: 无法解析第一个点");
				return 0.0;
			}
			if (!ParsePoint(point2, out XYZ xyz2) || xyz2 == null)
			{
				LogError("GetDistanceBetweenPoints: 无法解析第二个点");
				return 0.0;
			}
			double num = xyz2.X - xyz.X;
			double num2 = xyz2.Y - xyz.Y;
			double num3 = xyz2.Z - xyz.Z;
			return Math.Sqrt(num * num + num2 * num2 + num3 * num3);
		}
		catch (Exception ex)
		{
			LogError("GetDistanceBetweenPoints 失败: " + ex.Message);
			return 0.0;
		}
	}

	private (double X, double Y, double Z)? GetElementLocation(object element)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			Location location = val.Location;
			LocationPoint val2 = (LocationPoint)(object)((location is LocationPoint) ? location : null);
			if (val2 != null)
			{
				XYZ point = val2.Point;
				return (point.X, point.Y, point.Z);
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	private bool ParsePoint(object pointObj, out XYZ? xyz)
	{
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Expected O, but got Unknown
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Expected O, but got Unknown
		try
		{
			if (pointObj == null)
			{
				LogError("ParsePoint: 点对象为 null");
				xyz = null;
				return false;
			}
			Type type = pointObj.GetType();
			LogInfo("ParsePoint: 尝试解析点对象，类型: " + type.Name);
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (pointObj is IDictionary<string, object> dictionary)
			{
				if (dictionary.TryGetValue("x", out var value) && value != null)
				{
					num = Convert.ToDouble(value);
					flag = true;
				}
				if (dictionary.TryGetValue("y", out var value2) && value2 != null)
				{
					num2 = Convert.ToDouble(value2);
					flag2 = true;
				}
				if (dictionary.TryGetValue("z", out var value3) && value3 != null)
				{
					num3 = Convert.ToDouble(value3);
					flag3 = true;
				}
				if (flag & flag2 & flag3)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 3);
					defaultInterpolatedStringHandler.AppendLiteral("ParsePoint: 成功从字典解析点坐标 (");
					defaultInterpolatedStringHandler.AppendFormatted(num);
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(num2);
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(num3);
					defaultInterpolatedStringHandler.AppendLiteral(")");
					LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
					xyz = new XYZ(num, num2, num3);
					return true;
				}
			}
			PropertyInfo property = type.GetProperty("x");
			PropertyInfo property2 = type.GetProperty("y");
			PropertyInfo property3 = type.GetProperty("z");
			if (property != null && property2 != null && property3 != null)
			{
				num = Convert.ToDouble(property.GetValue(pointObj));
				num2 = Convert.ToDouble(property2.GetValue(pointObj));
				num3 = Convert.ToDouble(property3.GetValue(pointObj));
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(26, 3);
				defaultInterpolatedStringHandler2.AppendLiteral("ParsePoint: 成功解析点坐标 (");
				defaultInterpolatedStringHandler2.AppendFormatted(num);
				defaultInterpolatedStringHandler2.AppendLiteral(", ");
				defaultInterpolatedStringHandler2.AppendFormatted(num2);
				defaultInterpolatedStringHandler2.AppendLiteral(", ");
				defaultInterpolatedStringHandler2.AppendFormatted(num3);
				defaultInterpolatedStringHandler2.AppendLiteral(")");
				LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
				xyz = new XYZ(num, num2, num3);
				return true;
			}
			PropertyInfo property4 = type.GetProperty("location");
			if (property4 != null)
			{
				object value4 = property4.GetValue(pointObj);
				if (value4 != null)
				{
					Type type2 = value4.GetType();
					PropertyInfo property5 = type2.GetProperty("x");
					PropertyInfo property6 = type2.GetProperty("y");
					PropertyInfo property7 = type2.GetProperty("z");
					if (property5 != null && property6 != null && property7 != null)
					{
						num = Convert.ToDouble(property5.GetValue(value4));
						num2 = Convert.ToDouble(property6.GetValue(value4));
						num3 = Convert.ToDouble(property7.GetValue(value4));
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(39, 3);
						defaultInterpolatedStringHandler3.AppendLiteral("ParsePoint: 从 location 属性成功解析点坐标 (");
						defaultInterpolatedStringHandler3.AppendFormatted(num);
						defaultInterpolatedStringHandler3.AppendLiteral(", ");
						defaultInterpolatedStringHandler3.AppendFormatted(num2);
						defaultInterpolatedStringHandler3.AppendLiteral(", ");
						defaultInterpolatedStringHandler3.AppendFormatted(num3);
						defaultInterpolatedStringHandler3.AppendLiteral(")");
						LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
						xyz = new XYZ(num, num2, num3);
						return true;
					}
				}
			}
			LogError("ParsePoint: 无法解析点对象（类型: " + type.Name + "，未找到 x, y, z 或 location 属性）");
			xyz = null;
			return false;
		}
		catch (Exception ex)
		{
			LogError("ParsePoint: 解析点对象时发生异常 - " + ex.GetType().Name + ": " + ex.Message);
			xyz = null;
			return false;
		}
	}

	private void LogInfo(string message)
	{
		try
		{
			Logger.Info("[GeometryService] " + message);
		}
		catch
		{
		}
	}

	public ((double MinX, double MinY, double MinZ)? Min, (double MaxX, double MaxY, double MaxZ)? Max)? GetBoundingBox(object element)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				LogError("GetBoundingBox: element 不是 Element 类型");
				return null;
			}
			BoundingBoxXYZ val2 = val.get_BoundingBox((View)null);
			if (val2 == null)
			{
				Document document = val.Document;
				if (document != null && document.ActiveView != null)
				{
					val2 = val.get_BoundingBox(document.ActiveView);
				}
				if (val2 == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
					defaultInterpolatedStringHandler.AppendLiteral("GetBoundingBox: 元素 ");
					defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val.Id);
					defaultInterpolatedStringHandler.AppendLiteral(" 没有边界框");
					LogError(defaultInterpolatedStringHandler.ToStringAndClear());
					return null;
				}
			}
			(double, double, double) value = (val2.Min.X, val2.Min.Y, val2.Min.Z);
			return new ValueTuple<(double, double, double)?, (double, double, double)?>(item2: (val2.Max.X, val2.Max.Y, val2.Max.Z), item1: value);
		}
		catch (Exception ex)
		{
			LogError("GetBoundingBox 失败: " + ex.Message);
			return null;
		}
	}

	public object? GetElementGeometry(object element, object document)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				LogError("GetElementGeometry: element 不是 Element 类型");
				return null;
			}
			Document val2 = (Document)((document is Document) ? document : null);
			if (val2 == null)
			{
				LogError("GetElementGeometry: document 不是 Document 类型");
				return null;
			}
			Options val3 = new Options();
			GeometryElement val4 = val.get_Geometry(val3);
			if ((GeometryObject)(object)val4 == (GeometryObject)null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
				defaultInterpolatedStringHandler.AppendLiteral("GetElementGeometry: 元素 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val.Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 没有几何信息");
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			return val4;
		}
		catch (Exception ex)
		{
			LogError("GetElementGeometry 失败: " + ex.Message);
			return null;
		}
	}

	public IList<IList<IDictionary<string, double>>>? GetRoomBoundaries(object document, object room)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		try
		{
			SpatialElement val = (SpatialElement)((room is SpatialElement) ? room : null);
			if (val == null)
			{
				LogError("GetRoomBoundaries: room 参数不是 SpatialElement 类型");
				return null;
			}
			Document val2 = (Document)((document is Document) ? document : null);
			if (val2 == null)
			{
				LogError("GetRoomBoundaries: document 不是 Document 类型");
				return null;
			}
			IList<IList<BoundarySegment>> boundarySegments = val.GetBoundarySegments(new SpatialElementBoundaryOptions());
			if (boundarySegments == null || boundarySegments.Count == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
				defaultInterpolatedStringHandler.AppendLiteral("GetRoomBoundaries: 房间 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)val).Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 没有边界分段");
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			List<IList<IDictionary<string, double>>> list = new List<IList<IDictionary<string, double>>>();
			foreach (IList<BoundarySegment> item in boundarySegments)
			{
				List<IDictionary<string, double>> list2 = new List<IDictionary<string, double>>();
				foreach (BoundarySegment item2 in item)
				{
					Curve curve = item2.GetCurve();
					Line val3 = (Line)(object)((curve is Line) ? curve : null);
					if (val3 != null)
					{
						XYZ endPoint = ((Curve)val3).GetEndPoint(0);
						list2.Add(new Dictionary<string, double>
						{
							["x"] = endPoint.X * 304.8,
							["y"] = endPoint.Y * 304.8,
							["z"] = endPoint.Z * 304.8
						});
						XYZ endPoint2 = ((Curve)val3).GetEndPoint(1);
						list2.Add(new Dictionary<string, double>
						{
							["x"] = endPoint2.X * 304.8,
							["y"] = endPoint2.Y * 304.8,
							["z"] = endPoint2.Z * 304.8
						});
						continue;
					}
					Arc val4 = (Arc)(object)((curve is Arc) ? curve : null);
					if (val4 != null)
					{
						XYZ endPoint3 = ((Curve)val4).GetEndPoint(0);
						list2.Add(new Dictionary<string, double>
						{
							["x"] = endPoint3.X * 304.8,
							["y"] = endPoint3.Y * 304.8,
							["z"] = endPoint3.Z * 304.8
						});
						XYZ val5 = ((Curve)val4).Evaluate(0.5, true);
						list2.Add(new Dictionary<string, double>
						{
							["x"] = val5.X * 304.8,
							["y"] = val5.Y * 304.8,
							["z"] = val5.Z * 304.8
						});
						XYZ endPoint4 = ((Curve)val4).GetEndPoint(1);
						list2.Add(new Dictionary<string, double>
						{
							["x"] = endPoint4.X * 304.8,
							["y"] = endPoint4.Y * 304.8,
							["z"] = endPoint4.Z * 304.8
						});
					}
					else
					{
						XYZ endPoint5 = curve.GetEndPoint(0);
						list2.Add(new Dictionary<string, double>
						{
							["x"] = endPoint5.X * 304.8,
							["y"] = endPoint5.Y * 304.8,
							["z"] = endPoint5.Z * 304.8
						});
						XYZ endPoint6 = curve.GetEndPoint(1);
						list2.Add(new Dictionary<string, double>
						{
							["x"] = endPoint6.X * 304.8,
							["y"] = endPoint6.Y * 304.8,
							["z"] = endPoint6.Z * 304.8
						});
					}
				}
				if (list2.Count > 0)
				{
					list.Add(list2);
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetRoomBoundaries 失败: " + ex.Message);
			return null;
		}
	}
}
