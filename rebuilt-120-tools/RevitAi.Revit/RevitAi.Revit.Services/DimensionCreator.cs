using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using ns6;

using Document = Autodesk.Revit.DB.Document;

namespace RevitAi.Revit.Services;

internal sealed class DimensionCreator
{
	private readonly Document _document;

	private readonly View _view;

	private readonly GeometryReferenceExtractor _referenceExtractor;

	public DimensionCreator(Document document, View view)
	{
		_document = document ?? throw new ArgumentNullException("document");
		_view = view ?? throw new ArgumentNullException("view");
		_referenceExtractor = new GeometryReferenceExtractor(document, view);
	}

	public Dimension? CreateDimension(IList<Element> elements, XYZ dimensionLineOrigin, XYZ dimensionDirection, ElementId? dimensionTypeId = null, double offset = 0.0)
	{
		try
		{
			List<Element> list = FilterElementsByDirection(elements, dimensionDirection);
			if (list.Count < 2)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[DimensionCreator] 经过方向过滤后，有效元素少于2个（原始：");
				defaultInterpolatedStringHandler.AppendFormatted(elements.Count);
				defaultInterpolatedStringHandler.AppendLiteral("，过滤后：");
				defaultInterpolatedStringHandler.AppendFormatted(list.Count);
				defaultInterpolatedStringHandler.AppendLiteral("）");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				if (list.Count != 0)
				{
					return null;
				}
				list = elements.ToList();
			}
			else if (list.Count < elements.Count)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(34, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("[DimensionCreator] 已根据标注方向过滤元素：");
				defaultInterpolatedStringHandler2.AppendFormatted(elements.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" → ");
				defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			List<GeometryReferencePoint> list2 = _referenceExtractor.ExtractReferences(list, dimensionDirection);
			if (list2.Count < 2)
			{
				Logger.Error("[DimensionCreator] 提取的几何参考点少于2个，无法创建标注");
				return null;
			}
			List<GeometryReferencePoint> list3 = FilterReferencesByDirection(list2, dimensionDirection);
			if (list3.Count < 2)
			{
				Logger.Error("[DimensionCreator] 过滤后有效参考点少于2个，无法创建标注");
				return null;
			}
			if (list3.Count < list2.Count)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(35, 2);
				defaultInterpolatedStringHandler3.AppendLiteral("[DimensionCreator] 已过滤方向不一致的参考点：");
				defaultInterpolatedStringHandler3.AppendFormatted(list2.Count);
				defaultInterpolatedStringHandler3.AppendLiteral(" → ");
				defaultInterpolatedStringHandler3.AppendFormatted(list3.Count);
				Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
			}
			ReferenceArray val = CreateReferenceArray(list3);
			if (val == null || val.IsEmpty)
			{
				Logger.Error("[DimensionCreator] 创建参考数组失败");
				return null;
			}
			Line val2 = CalculateDimensionLine(list3, dimensionLineOrigin, dimensionDirection, offset);
			if ((GeometryObject)(object)val2 == (GeometryObject)null)
			{
				Logger.Error("[DimensionCreator] 计算标注线位置失败");
				return null;
			}
			ElementId val3 = dimensionTypeId ?? GetDefaultDimensionTypeId();
			if (val3 == (ElementId)null || val3 == ElementId.InvalidElementId)
			{
				Logger.Error("[DimensionCreator] 无法获取标注类型");
				return null;
			}
			Dimension val5;
			if (val3 != (ElementId)null && val3 != ElementId.InvalidElementId)
			{
				Element element = _document.GetElement(val3);
				DimensionType val4 = (DimensionType)(object)((element is DimensionType) ? element : null);
				val5 = ((val4 == null) ? ((ItemFactoryBase)_document.Create).NewDimension(_view, val2, val) : ((ItemFactoryBase)_document.Create).NewDimension(_view, val2, val, val4));
			}
			else
			{
				val5 = ((ItemFactoryBase)_document.Create).NewDimension(_view, val2, val);
			}
			if (val5 == null)
			{
				Logger.Error("[DimensionCreator] 创建尺寸标注失败");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(42, 2);
			defaultInterpolatedStringHandler4.AppendLiteral("[DimensionCreator] 成功创建尺寸标注，ID: ");
			defaultInterpolatedStringHandler4.AppendFormatted(((Element)val5).Id.Value);
			defaultInterpolatedStringHandler4.AppendLiteral("，标注了 ");
			defaultInterpolatedStringHandler4.AppendFormatted(list3.Count);
			defaultInterpolatedStringHandler4.AppendLiteral(" 个参考点");
			Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
			return val5;
		}
		catch (Exception ex)
		{
			Logger.Error("[DimensionCreator] 创建尺寸标注失败: " + ex.Message);
			return null;
		}
	}

	private List<Element> FilterElementsByDirection(IList<Element> elements, XYZ dimensionDirection)
	{
		List<Element> list = new List<Element>();
		try
		{
			XYZ val = dimensionDirection.Normalize();
			AxisType primaryAxis = GetPrimaryAxis(val);
			foreach (Element element in elements)
			{
				if (IsElementAlignedWithDirection(element, val, primaryAxis))
				{
					list.Add(element);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[DimensionCreator] 过滤元素时出错: " + ex.Message);
			return elements.ToList();
		}
		return list;
	}

	private bool IsElementAlignedWithDirection(Element element, XYZ dimensionDirection, AxisType primaryAxis)
	{
		try
		{
			Wall val = (Wall)(object)((element is Wall) ? element : null);
			if (val != null)
			{
				Location location = ((Element)val).Location;
				LocationCurve val2 = (LocationCurve)(object)((location is LocationCurve) ? location : null);
				if (val2 != null)
				{
					Curve curve = val2.Curve;
					Line val3 = (Line)(object)((curve is Line) ? curve : null);
					if (val3 != null)
					{
						XYZ val4 = val3.Direction.Normalize();
						double num = Math.Abs(val4.DotProduct(dimensionDirection));
						return num < 0.3;
					}
				}
			}
			FamilyInstance val5 = (FamilyInstance)(object)((element is FamilyInstance) ? element : null);
			if (val5 != null)
			{
				Location location2 = ((Element)val5).Location;
				LocationCurve val6 = (LocationCurve)(object)((location2 is LocationCurve) ? location2 : null);
				if (val6 == null)
				{
					return true;
				}
				Curve curve2 = val6.Curve;
				Line val7 = (Line)(object)((curve2 is Line) ? curve2 : null);
				if (val7 != null)
				{
					XYZ val8 = val7.Direction.Normalize();
					double num2 = Math.Abs(val8.DotProduct(dimensionDirection));
					return num2 < 0.3;
				}
			}
			CurveElement val9 = (CurveElement)(object)((element is CurveElement) ? element : null);
			if (val9 != null)
			{
				Curve geometryCurve = val9.GeometryCurve;
				Line val10 = (Line)(object)((geometryCurve is Line) ? geometryCurve : null);
				if (val10 != null)
				{
					XYZ val11 = val10.Direction.Normalize();
					double num3 = Math.Abs(val11.DotProduct(dimensionDirection));
					return num3 < 0.3;
				}
			}
			Grid val12 = (Grid)(object)((element is Grid) ? element : null);
			if (val12 != null)
			{
				Curve curve3 = val12.Curve;
				Line val13 = (Line)(object)((curve3 is Line) ? curve3 : null);
				if (val13 != null)
				{
					XYZ val14 = val13.Direction.Normalize();
					double num4 = Math.Abs(val14.DotProduct(dimensionDirection));
					return num4 < 0.3;
				}
			}
			return true;
		}
		catch
		{
			return true;
		}
	}

	private List<GeometryReferencePoint> FilterReferencesByDirection(List<GeometryReferencePoint> referencePoints, XYZ dimensionDirection)
	{
		try
		{
			List<GeometryReferencePoint> list = referencePoints.OrderBy((GeometryReferencePoint r) => r.ProjectionOnDimensionLine).ToList();
			if (list.Count == 0)
			{
				return list;
			}
			List<double> source = list.Select((GeometryReferencePoint r) => r.ProjectionOnDimensionLine).ToList();
			double num = source.Min();
			double num2 = source.Max();
			double num3 = num2 - num;
			if (num3 < 0.1)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[DimensionCreator] 参考点投影范围太小（");
				defaultInterpolatedStringHandler.AppendFormatted(num3, "F3");
				defaultInterpolatedStringHandler.AppendLiteral(" 英尺），可能不适合创建标注");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return list;
			}
			List<GeometryReferencePoint> list2 = new List<GeometryReferencePoint>();
			list2.Add(list[0]);
			for (int num4 = 1; num4 < list.Count; num4++)
			{
				double num5 = list[num4].ProjectionOnDimensionLine - list2.Last().ProjectionOnDimensionLine;
				if (num5 > 25.0 / 762.0)
				{
					list2.Add(list[num4]);
					continue;
				}
				GeometryReferencePoint geometryReferencePoint = list2.Last();
				GeometryReferencePoint geometryReferencePoint2 = list[num4];
				if (geometryReferencePoint2.Reference != null && geometryReferencePoint.Reference == null)
				{
					list2[list2.Count - 1] = geometryReferencePoint2;
				}
			}
			if (list2.Count < list.Count)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("[DimensionCreator] 移除了 ");
				defaultInterpolatedStringHandler2.AppendFormatted(list.Count - list2.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个间距过小（<10mm）的参考点");
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			List<IGrouping<ElementId, GeometryReferencePoint>> list3 = (from r in list2
				group r by r.ElementId).ToList();
			List<GeometryReferencePoint> list4 = new List<GeometryReferencePoint>();
			foreach (IGrouping<ElementId, GeometryReferencePoint> item in list3)
			{
				List<GeometryReferencePoint> list5 = item.OrderBy((GeometryReferencePoint r) => r.ProjectionOnDimensionLine).ToList();
				if (item.Count() == 1)
				{
					list4.Add(list5[0]);
					continue;
				}
				list4.Add(list5[0]);
				list4.Add(list5[list5.Count - 1]);
			}
			return list4.OrderBy((GeometryReferencePoint r) => r.ProjectionOnDimensionLine).ToList();
		}
		catch (Exception ex)
		{
			Logger.Warning("[DimensionCreator] 过滤参考点时出错: " + ex.Message);
			return referencePoints;
		}
	}

	private AxisType GetPrimaryAxis(XYZ direction)
	{
		double num = Math.Abs(direction.X);
		double num2 = Math.Abs(direction.Y);
		double num3 = Math.Abs(direction.Z);
		if (num > num2 && num > num3)
		{
			return AxisType.X;
		}
		if (num2 > num && num2 > num3)
		{
			return AxisType.Y;
		}
		return AxisType.Z;
	}

	private ReferenceArray? CreateReferenceArray(List<GeometryReferencePoint> referencePoints)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		try
		{
			ReferenceArray val = new ReferenceArray();
			foreach (GeometryReferencePoint referencePoint in referencePoints)
			{
				if (referencePoint.Reference != null)
				{
					val.Append(referencePoint.Reference);
					continue;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[DimensionCreator] 元素 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(referencePoint.ElementId);
				defaultInterpolatedStringHandler.AppendLiteral(" 没有有效的几何参考");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			if (val.IsEmpty)
			{
				Logger.Error("[DimensionCreator] 没有有效的几何参考");
				return null;
			}
			return val;
		}
		catch (Exception ex)
		{
			Logger.Error("[DimensionCreator] 创建参考数组失败: " + ex.Message);
			return null;
		}
	}

	private Line? CalculateDimensionLine(List<GeometryReferencePoint> referencePoints, XYZ origin, XYZ direction, double offset)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		try
		{
			GeometryReferencePoint geometryReferencePoint = referencePoints.First();
			GeometryReferencePoint geometryReferencePoint2 = referencePoints.Last();
			XYZ val = direction.CrossProduct(XYZ.BasisZ).Normalize();
			XYZ val2 = new XYZ(origin.X + val.X * offset, origin.Y + val.Y * offset, origin.Z);
			double val3 = (geometryReferencePoint2.ProjectionOnDimensionLine - geometryReferencePoint.ProjectionOnDimensionLine) * 1.2;
			val3 = Math.Max(val3, 10.0);
			XYZ val4 = val2.Add(direction.Multiply(val3));
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 7);
			defaultInterpolatedStringHandler.AppendLiteral("[DimensionCreator] 标注线: 起点=(");
			defaultInterpolatedStringHandler.AppendFormatted(val2.X, "F2");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(val2.Y, "F2");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(val2.Z, "F2");
			defaultInterpolatedStringHandler.AppendLiteral("), 终点=(");
			defaultInterpolatedStringHandler.AppendFormatted(val4.X, "F2");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(val4.Y, "F2");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(val4.Z, "F2");
			defaultInterpolatedStringHandler.AppendLiteral("), 长度=");
			defaultInterpolatedStringHandler.AppendFormatted(val3, "F2");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return Line.CreateBound(val2, val4);
		}
		catch (Exception ex)
		{
			Logger.Error("[DimensionCreator] 计算标注线位置失败: " + ex.Message);
			return null;
		}
	}

	private ElementId GetDefaultDimensionTypeId()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Element obj = new FilteredElementCollector(_document).OfClass(typeof(DimensionType)).FirstElement();
			DimensionType val = (DimensionType)(object)((obj is DimensionType) ? obj : null);
			object obj2;
			if (val == null)
			{
				obj2 = null;
			}
			else
			{
				obj2 = ((Element)val).Id;
				if (obj2 != null)
				{
					goto IL_003b;
				}
			}
			obj2 = ElementId.InvalidElementId;
			goto IL_003b;
			IL_003b:
			return (ElementId)obj2;
		}
		catch
		{
			return ElementId.InvalidElementId;
		}
	}

	public static List<XYZ> DetectDimensionDirections(IList<Element> elements)
	{
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Expected O, but got Unknown
		List<XYZ> list = new List<XYZ>();
		try
		{
			List<Element> list2 = new List<Element>();
			List<XYZ> list3 = new List<XYZ>();
			List<Element> list4 = new List<Element>();
			List<XYZ> list5 = new List<XYZ>();
			foreach (Element element in elements)
			{
				XYZ val = null;
				bool flag = false;
				Wall val2 = (Wall)(object)((element is Wall) ? element : null);
				if (val2 != null)
				{
					Location location = ((Element)val2).Location;
					LocationCurve val3 = (LocationCurve)(object)((location is LocationCurve) ? location : null);
					if (val3 != null)
					{
						Curve curve = val3.Curve;
						Line val4 = (Line)(object)((curve is Line) ? curve : null);
						if (val4 != null)
						{
							val = val4.Direction.Normalize();
						}
					}
				}
				else
				{
					FamilyInstance val5 = (FamilyInstance)(object)((element is FamilyInstance) ? element : null);
					if (val5 != null)
					{
						Location location2 = ((Element)val5).Location;
						LocationCurve val6 = (LocationCurve)(object)((location2 is LocationCurve) ? location2 : null);
						if (val6 != null)
						{
							Curve curve2 = val6.Curve;
							Line val7 = (Line)(object)((curve2 is Line) ? curve2 : null);
							if (val7 != null)
							{
								val = val7.Direction.Normalize();
								goto IL_0275;
							}
						}
						if (((Element)val5).Location is LocationPoint)
						{
							flag = true;
							Element host = val5.Host;
							Wall val8 = (Wall)(object)((host is Wall) ? host : null);
							if (val8 != null)
							{
								Location location3 = ((Element)val8).Location;
								LocationCurve val9 = (LocationCurve)(object)((location3 is LocationCurve) ? location3 : null);
								if (val9 != null)
								{
									Curve curve3 = val9.Curve;
									Line val10 = (Line)(object)((curve3 is Line) ? curve3 : null);
									if (val10 != null)
									{
										val = val10.Direction.Normalize();
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 4);
										defaultInterpolatedStringHandler.AppendLiteral("[DimensionCreator] 族实例 ");
										defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)val5).Id);
										defaultInterpolatedStringHandler.AppendLiteral(" 使用所依附墙的方向（平行）: (");
										defaultInterpolatedStringHandler.AppendFormatted(val.X, "F2");
										defaultInterpolatedStringHandler.AppendLiteral(", ");
										defaultInterpolatedStringHandler.AppendFormatted(val.Y, "F2");
										defaultInterpolatedStringHandler.AppendLiteral(", ");
										defaultInterpolatedStringHandler.AppendFormatted(val.Z, "F2");
										defaultInterpolatedStringHandler.AppendLiteral(")");
										Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
									}
								}
							}
						}
					}
					else
					{
						CurveElement val11 = (CurveElement)(object)((element is CurveElement) ? element : null);
						if (val11 != null)
						{
							Curve geometryCurve = val11.GeometryCurve;
							Line val12 = (Line)(object)((geometryCurve is Line) ? geometryCurve : null);
							if (val12 != null)
							{
								val = val12.Direction.Normalize();
							}
						}
						else
						{
							Grid val13 = (Grid)(object)((element is Grid) ? element : null);
							if (val13 != null)
							{
								Curve curve4 = val13.Curve;
								Line val14 = (Line)(object)((curve4 is Line) ? curve4 : null);
								if (val14 != null)
								{
									val = val14.Direction.Normalize();
								}
							}
						}
					}
				}
				goto IL_0275;
				IL_0275:
				if (val != null)
				{
					if (flag)
					{
						list4.Add(element);
						list5.Add(val);
					}
					else
					{
						list2.Add(element);
						list3.Add(val);
					}
				}
			}
			if (list5.Count > 0)
			{
				int num = 0;
				int num2 = 0;
				foreach (XYZ item2 in list5)
				{
					double num3 = Math.Abs(item2.X);
					double num4 = Math.Abs(item2.Y);
					if (num3 > num4)
					{
						num++;
						continue;
					}
					if (num4 > num3)
					{
						num2++;
						continue;
					}
					num++;
					num2++;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(44, 3);
				defaultInterpolatedStringHandler2.AppendLiteral("[DimensionCreator] 检测到 ");
				defaultInterpolatedStringHandler2.AppendFormatted(list4.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个门窗元素，水平方向: ");
				defaultInterpolatedStringHandler2.AppendFormatted(num);
				defaultInterpolatedStringHandler2.AppendLiteral(", 垂直方向: ");
				defaultInterpolatedStringHandler2.AppendFormatted(num2);
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
				if (num > 0 && num2 == 0)
				{
					list.Add(XYZ.BasisX);
				}
				else if (num2 > 0 && num == 0)
				{
					list.Add(XYZ.BasisY);
				}
				else if (num > 0 && num2 > 0)
				{
					list.Add(XYZ.BasisX);
					list.Add(XYZ.BasisY);
				}
				return list;
			}
			if (list3.Count > 0)
			{
				int num5 = 0;
				int num6 = 0;
				foreach (XYZ item3 in list3)
				{
					double num7 = Math.Abs(item3.X);
					double num8 = Math.Abs(item3.Y);
					if (num7 > num8)
					{
						num5++;
						continue;
					}
					if (num8 > num7)
					{
						num6++;
						continue;
					}
					num5++;
					num6++;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(44, 3);
				defaultInterpolatedStringHandler3.AppendLiteral("[DimensionCreator] 检测到 ");
				defaultInterpolatedStringHandler3.AppendFormatted(list2.Count);
				defaultInterpolatedStringHandler3.AppendLiteral(" 个线性元素，水平方向: ");
				defaultInterpolatedStringHandler3.AppendFormatted(num5);
				defaultInterpolatedStringHandler3.AppendLiteral(", 垂直方向: ");
				defaultInterpolatedStringHandler3.AppendFormatted(num6);
				Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
				if (num5 > 0 && num6 == 0)
				{
					list.Add(XYZ.BasisY);
				}
				else if (num6 > 0 && num5 == 0)
				{
					list.Add(XYZ.BasisX);
				}
				else if (num5 > 0 && num6 > 0)
				{
					list.Add(XYZ.BasisX);
					list.Add(XYZ.BasisY);
				}
				else
				{
					list.Add(XYZ.BasisX);
				}
				return list;
			}
			List<XYZ> list6 = new List<XYZ>();
			foreach (Element element2 in elements)
			{
				BoundingBoxXYZ val15 = element2.get_BoundingBox((View)null);
				if (val15 != null)
				{
					XYZ item = new XYZ((val15.Min.X + val15.Max.X) / 2.0, (val15.Min.Y + val15.Max.Y) / 2.0, (val15.Min.Z + val15.Max.Z) / 2.0);
					list6.Add(item);
				}
			}
			if (list6.Count < 2)
			{
				list.Add(XYZ.BasisX);
				return list;
			}
			List<double> source = list6.Select((XYZ p) => p.X).ToList();
			List<double> source2 = list6.Select((XYZ p) => p.Y).ToList();
			double num9 = source.Max() - source.Min();
			double num10 = source2.Max() - source2.Min();
			if (num9 > 1.0)
			{
				list.Add(XYZ.BasisX);
			}
			if (num10 > 1.0)
			{
				list.Add(XYZ.BasisY);
			}
			if (list.Count == 0)
			{
				list.Add(XYZ.BasisX);
			}
			return list;
		}
		catch
		{
			list.Add(XYZ.BasisX);
			return list;
		}
	}

	public static XYZ DetectDimensionDirection(IList<Element> elements)
	{
		List<XYZ> list = DetectDimensionDirections(elements);
		return (list.Count > 0) ? list[0] : XYZ.BasisX;
	}

	public static XYZ DetectDimensionLineOrigin(IList<Element> elements, XYZ dimensionDirection, string position = "auto", View? view = null, double offsetDistanceMm = 2000.0)
	{
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Expected O, but got Unknown
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Expected O, but got Unknown
		//IL_086d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0874: Expected O, but got Unknown
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_085f: Expected O, but got Unknown
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Expected O, but got Unknown
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Expected O, but got Unknown
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Expected O, but got Unknown
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0831: Expected O, but got Unknown
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Expected O, but got Unknown
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Expected O, but got Unknown
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_081c: Expected O, but got Unknown
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Expected O, but got Unknown
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Expected O, but got Unknown
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Expected O, but got Unknown
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Expected O, but got Unknown
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Expected O, but got Unknown
		try
		{
			double num = offsetDistanceMm / 304.8;
			double num2 = 0.0;
			if (view != null)
			{
				ViewPlan val = (ViewPlan)(object)((view is ViewPlan) ? view : null);
				if (val != null)
				{
					Level genLevel = ((View)val).GenLevel;
					num2 = ((genLevel != null) ? genLevel.Elevation : 0.0);
				}
			}
			if (num2 == 0.0)
			{
				List<double> list = new List<double>();
				foreach (Element element2 in elements)
				{
					if (element2.LevelId != (ElementId)null && element2.LevelId != ElementId.InvalidElementId && element2.Document != null)
					{
						Element element = element2.Document.GetElement(element2.LevelId);
						Level val2 = (Level)(object)((element is Level) ? element : null);
						if (val2 != null)
						{
							list.Add(val2.Elevation);
						}
					}
				}
				if (list.Count > 0)
				{
					num2 = list.Min();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[DimensionCreator] 从元素获取到 ");
					defaultInterpolatedStringHandler.AppendFormatted(list.Count);
					defaultInterpolatedStringHandler.AppendLiteral(" 个标高，使用最低标高: ");
					defaultInterpolatedStringHandler.AppendFormatted(num2, "F2");
					defaultInterpolatedStringHandler.AppendLiteral(" 英尺");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					Logger.Warning("[DimensionCreator] 无法获取视图标高或元素标高，使用默认值 0");
				}
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("[DimensionCreator] 使用视图标高作为标注线 Z 坐标: ");
				defaultInterpolatedStringHandler2.AppendFormatted(num2, "F2");
				defaultInterpolatedStringHandler2.AppendLiteral(" 英尺");
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			elements.All((Element e) => e is Grid);
			if (elements.Any((Element e) => e is Grid))
			{
				List<(XYZ, XYZ)> list2 = new List<(XYZ, XYZ)>();
				foreach (Element element3 in elements)
				{
					Grid val3 = (Grid)(object)((element3 is Grid) ? element3 : null);
					if (val3 != null)
					{
						Curve curve = val3.Curve;
						Line val4 = (Line)(object)((curve is Line) ? curve : null);
						if (val4 != null)
						{
							list2.Add((((Curve)val4).GetEndPoint(0), ((Curve)val4).GetEndPoint(1)));
						}
					}
				}
				if (list2.Count > 0)
				{
					double num3 = list2.Min<(XYZ, XYZ)>(((XYZ start, XYZ end) g) => Math.Min(g.start.X, g.end.X));
					double num4 = list2.Max<(XYZ, XYZ)>(((XYZ start, XYZ end) g) => Math.Max(g.start.X, g.end.X));
					double num5 = list2.Min<(XYZ, XYZ)>(((XYZ start, XYZ end) g) => Math.Min(g.start.Y, g.end.Y));
					double num6 = list2.Max<(XYZ, XYZ)>(((XYZ start, XYZ end) g) => Math.Max(g.start.Y, g.end.Y));
					list2.Min<(XYZ, XYZ)>(((XYZ start, XYZ end) g) => Math.Min(g.start.Z, g.end.Z));
					list2.Max<(XYZ, XYZ)>(((XYZ start, XYZ end) g) => Math.Max(g.start.Z, g.end.Z));
					double num7 = num4 - num3;
					double num8 = num6 - num5;
					bool flag = num7 > num8;
					XYZ val5 = dimensionDirection.Normalize();
					bool flag2 = Math.Abs(val5.X) > Math.Abs(val5.Y);
					if (!string.IsNullOrEmpty(position) && position != "auto")
					{
						if (position == "上")
						{
							double num9 = (num3 + num4) / 2.0;
							double num10 = (num5 + num6) / 2.0;
							return new XYZ(num9, num6 + num, num2);
						}
						if (position == "下")
						{
							double num9 = (num3 + num4) / 2.0;
							double num10 = (num5 + num6) / 2.0;
							return new XYZ(num9, num5 - num, num2);
						}
						if (position == "左")
						{
							double num9 = (num3 + num4) / 2.0;
							double num10 = (num5 + num6) / 2.0;
							return new XYZ(num3 - num, num10, num2);
						}
						if (position == "右")
						{
							double num9 = (num3 + num4) / 2.0;
							double num10 = (num5 + num6) / 2.0;
							return new XYZ(num4 + num, num10, num2);
						}
					}
					if (flag & flag2)
					{
						double num11 = (num3 + num4) / 2.0;
						return new XYZ(num11, num5 - num, num2);
					}
					if (!flag & flag2)
					{
						double num12 = (num3 + num4) / 2.0;
						return new XYZ(num12, num5 - num, num2);
					}
					if (flag && !flag2)
					{
						double num13 = (num5 + num6) / 2.0;
						return new XYZ(num4 + num, num13, num2);
					}
					double num14 = (num5 + num6) / 2.0;
					return new XYZ(num4 + num, num14, num2);
				}
			}
			List<BoundingBoxXYZ> list3 = new List<BoundingBoxXYZ>();
			foreach (Element element4 in elements)
			{
				BoundingBoxXYZ val6 = element4.get_BoundingBox((View)null);
				if (val6 != null)
				{
					list3.Add(val6);
				}
			}
			if (list3.Count == 0)
			{
				return XYZ.Zero;
			}
			XYZ val7 = new XYZ(list3.Min((BoundingBoxXYZ b) => b.Min.X), list3.Min((BoundingBoxXYZ b) => b.Min.Y), list3.Min((BoundingBoxXYZ b) => b.Min.Z));
			XYZ val8 = new XYZ(list3.Max((BoundingBoxXYZ b) => b.Max.X), list3.Max((BoundingBoxXYZ b) => b.Max.Y), list3.Max((BoundingBoxXYZ b) => b.Max.Z));
			double num15 = (val7.X + val8.X) / 2.0;
			double num16 = (val7.Y + val8.Y) / 2.0;
			_ = (val7.Z + val8.Z) / 2.0;
			XYZ val9 = dimensionDirection.Normalize();
			bool flag3 = Math.Abs(val9.X) > Math.Abs(val9.Y);
			if (!string.IsNullOrEmpty(position) && position != "auto")
			{
				if (position == "上")
				{
					return new XYZ(num15, val8.Y + num, num2);
				}
				if (position == "下")
				{
					return new XYZ(num15, val7.Y - num, num2);
				}
				if (position == "左")
				{
					return new XYZ(val7.X - num, num16, num2);
				}
				if (position == "右")
				{
					return new XYZ(val8.X + num, num16, num2);
				}
			}
			if (flag3)
			{
				return new XYZ(num15, val7.Y - num, num2);
			}
			return new XYZ(val8.X + num, num16, num2);
		}
		catch
		{
			return XYZ.Zero;
		}
	}

	private static void ProcessGeometryObject(GeometryObject geomObj, Element element, XYZ dimensionDirection, List<(Element element, XYZ position, XYZ normal)> referenceFaces)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		Solid val = (Solid)(object)((geomObj is Solid) ? geomObj : null);
		if (val != null)
		{
			foreach (Face face in val.Faces)
			{
				Face val2 = face;
				PlanarFace val3 = (PlanarFace)(object)((val2 is PlanarFace) ? val2 : null);
				if (val3 != null)
				{
					XYZ val4 = val3.FaceNormal.Normalize();
					double num = Math.Abs(val4.DotProduct(dimensionDirection));
					if (num >= 0.9)
					{
						try
						{
							BoundingBoxUV boundingBox = val2.GetBoundingBox();
							UV val5 = new UV((boundingBox.Min.U + boundingBox.Max.U) / 2.0, (boundingBox.Min.V + boundingBox.Max.V) / 2.0);
							XYZ item = val2.Evaluate(val5);
							referenceFaces.Add((element, item, val4));
						}
						catch
						{
						}
					}
				}
			}
			return;
		}
		GeometryInstance val6 = (GeometryInstance)(object)((geomObj is GeometryInstance) ? geomObj : null);
		if (val6 == null)
		{
			return;
		}
		GeometryElement instanceGeometry = val6.GetInstanceGeometry();
		if (!((GeometryObject)(object)instanceGeometry != (GeometryObject)null))
		{
			return;
		}
		foreach (GeometryObject item2 in instanceGeometry)
		{
			ProcessGeometryObject(item2, element, dimensionDirection, referenceFaces);
		}
	}
}
