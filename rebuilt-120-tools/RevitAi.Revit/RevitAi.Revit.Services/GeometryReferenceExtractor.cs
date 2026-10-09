using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using ns6;

namespace RevitAi.Revit.Services;

internal sealed class GeometryReferenceExtractor
{
	private readonly Document _document;

	private readonly View _view;

	public GeometryReferenceExtractor(Document document, View view)
	{
		_document = document ?? throw new ArgumentNullException("document");
		_view = view ?? throw new ArgumentNullException("view");
	}

	public List<GeometryReferencePoint> ExtractReferences(IList<Element> elements, XYZ dimensionDirection)
	{
		List<GeometryReferencePoint> list = new List<GeometryReferencePoint>();
		try
		{
			foreach (Element element in elements)
			{
				List<GeometryReferencePoint> collection = ExtractElementReferences(element, dimensionDirection);
				list.AddRange(collection);
			}
			return list.OrderBy((GeometryReferencePoint r) => r.ProjectionOnDimensionLine).ToList();
		}
		catch (Exception ex)
		{
			Logger.Error("[GeometryReferenceExtractor] 提取几何参考失败: " + ex.Message);
			return list;
		}
	}

	private List<GeometryReferencePoint> ExtractElementReferences(Element element, XYZ dimensionDirection)
	{
		List<GeometryReferencePoint> list = new List<GeometryReferencePoint>();
		try
		{
			FamilyInstance val = (FamilyInstance)(object)((element is FamilyInstance) ? element : null);
			if (val != null && IsWindowOrDoor(val))
			{
				List<GeometryReferenceWithPoint> list2 = ExtractWindowDoorGeometryReferences(val, dimensionDirection);
				if (list2.Count > 0)
				{
					foreach (GeometryReferenceWithPoint item in list2)
					{
						double projectionOnDimensionLine = item.Point.DotProduct(dimensionDirection);
						list.Add(new GeometryReferencePoint
						{
							Position = item.Point,
							Reference = item.Reference,
							ProjectionOnDimensionLine = projectionOnDimensionLine,
							ElementId = element.Id
						});
					}
					return list;
				}
				list.AddRange(ExtractFamilyInstanceLocationFallback(val, dimensionDirection));
				return list;
			}
			List<GeometryReferenceWithPoint> list3 = ExtractGeometryReferences(element, dimensionDirection);
			if (list3.Count > 0)
			{
				foreach (GeometryReferenceWithPoint item2 in list3)
				{
					double projectionOnDimensionLine2 = item2.Point.DotProduct(dimensionDirection);
					list.Add(new GeometryReferencePoint
					{
						Position = item2.Point,
						Reference = item2.Reference,
						ProjectionOnDimensionLine = projectionOnDimensionLine2,
						ElementId = element.Id
					});
				}
				return list;
			}
			Wall val2 = (Wall)(object)((element is Wall) ? element : null);
			if (val2 != null)
			{
				list.AddRange(ExtractWallLocationFallback(val2, dimensionDirection));
			}
			else
			{
				FamilyInstance val3 = (FamilyInstance)(object)((element is FamilyInstance) ? element : null);
				if (val3 != null)
				{
					list.AddRange(ExtractFamilyInstanceLocationFallback(val3, dimensionDirection));
				}
				else
				{
					CurveElement val4 = (CurveElement)(object)((element is CurveElement) ? element : null);
					if (val4 != null)
					{
						list.AddRange(ExtractCurveElementReferences(val4, dimensionDirection));
					}
					else
					{
						Grid val5 = (Grid)(object)((element is Grid) ? element : null);
						if (val5 != null)
						{
							list.AddRange(ExtractGridReferences(val5, dimensionDirection));
						}
						else
						{
							Pipe val6 = (Pipe)(object)((element is Pipe) ? element : null);
							if (val6 != null)
							{
								list.AddRange(ExtractMEPElementReferences((Element)(object)val6, dimensionDirection, "管道"));
							}
							else
							{
								Duct val7 = (Duct)(object)((element is Duct) ? element : null);
								if (val7 != null)
								{
									list.AddRange(ExtractMEPElementReferences((Element)(object)val7, dimensionDirection, "风管"));
								}
								else
								{
									Conduit val8 = (Conduit)(object)((element is Conduit) ? element : null);
									if (val8 != null)
									{
										list.AddRange(ExtractMEPElementReferences((Element)(object)val8, dimensionDirection, "线管"));
									}
									else
									{
										CableTray val9 = (CableTray)(object)((element is CableTray) ? element : null);
										if (val9 != null)
										{
											list.AddRange(ExtractMEPElementReferences((Element)(object)val9, dimensionDirection, "电缆桥架"));
										}
									}
								}
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[GeometryReferenceExtractor] 提取元素 ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(element.Id);
			defaultInterpolatedStringHandler.AppendLiteral(" 的几何参考失败: ");
			defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
			Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		return list;
	}

	private List<GeometryReferenceWithPoint> ExtractGeometryReferences(Element element, XYZ dimensionDirection)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		List<GeometryReferenceWithPoint> result = new List<GeometryReferenceWithPoint>();
		try
		{
			Options val = new Options
			{
				ComputeReferences = true
			};
			GeometryElement val2 = element.get_Geometry(val);
			if ((GeometryObject)(object)val2 == (GeometryObject)null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[GeometryReferenceExtractor] 元素 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(element.Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 无法获取几何信息");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return result;
			}
			XYZ targetNormal = dimensionDirection.Normalize();
			ProcessGeometryObject(val2, targetNormal, result);
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(44, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[GeometryReferenceExtractor] 提取元素 ");
			defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(element.Id);
			defaultInterpolatedStringHandler2.AppendLiteral(" 的几何参考失败: ");
			defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
			Logger.Error(defaultInterpolatedStringHandler2.ToStringAndClear());
		}
		return result;
	}

	private bool IsWindowOrDoor(FamilyInstance familyInstance)
	{
		try
		{
			Category category = ((Element)familyInstance).Category;
			if (category == null)
			{
				return false;
			}
			string name = category.Name;
			return name.Contains("窗") || name.Contains("门") || name.Contains("Windows") || name.Contains("Doors");
		}
		catch
		{
			return false;
		}
	}

	private List<GeometryReferenceWithPoint> ExtractWindowDoorGeometryReferences(FamilyInstance familyInstance, XYZ dimensionDirection)
	{
		List<GeometryReferenceWithPoint> list = new List<GeometryReferenceWithPoint>();
		try
		{
			Location location = ((Element)familyInstance).Location;
			LocationPoint val = (LocationPoint)(object)((location is LocationPoint) ? location : null);
			object obj;
			if (val == null)
			{
				obj = null;
			}
			else
			{
				obj = val.Point;
				if (obj != null)
				{
					goto IL_0027;
				}
			}
			obj = XYZ.Zero;
			goto IL_0027;
			IL_0027:
			XYZ point = (XYZ)obj;
			XYZ val2 = dimensionDirection.Normalize();
			if (Math.Abs(val2.X) > Math.Abs(val2.Y))
			{
				IList<Reference> references = familyInstance.GetReferences((FamilyInstanceReferenceType)0);
				IList<Reference> references2 = familyInstance.GetReferences((FamilyInstanceReferenceType)2);
				if (references != null && references.Count > 0 && references2 != null && references2.Count > 0)
				{
					List<GeometryReferenceWithPoint> referencePointsFromGeometry = GetReferencePointsFromGeometry(familyInstance, references, references2, dimensionDirection);
					foreach (GeometryReferenceWithPoint item in referencePointsFromGeometry)
					{
						list.Add(item);
					}
					return list;
				}
				IList<Reference> references3 = familyInstance.GetReferences((FamilyInstanceReferenceType)1);
				if (references3 != null && references3.Count > 0)
				{
					foreach (Reference item2 in references3)
					{
						list.Add(new GeometryReferenceWithPoint
						{
							Point = point,
							Reference = item2
						});
					}
					return list;
				}
			}
			else
			{
				IList<Reference> references4 = familyInstance.GetReferences((FamilyInstanceReferenceType)3);
				IList<Reference> references5 = familyInstance.GetReferences((FamilyInstanceReferenceType)5);
				if (references4 != null && references4.Count > 0 && references5 != null && references5.Count > 0)
				{
					List<GeometryReferenceWithPoint> referencePointsFromGeometry2 = GetReferencePointsFromGeometry(familyInstance, references4, references5, dimensionDirection);
					foreach (GeometryReferenceWithPoint item3 in referencePointsFromGeometry2)
					{
						list.Add(item3);
					}
					return list;
				}
				IList<Reference> references6 = familyInstance.GetReferences((FamilyInstanceReferenceType)4);
				if (references6 != null && references6.Count > 0)
				{
					foreach (Reference item4 in references6)
					{
						list.Add(new GeometryReferenceWithPoint
						{
							Point = point,
							Reference = item4
						});
					}
					return list;
				}
			}
			IList<Reference> references7 = familyInstance.GetReferences((FamilyInstanceReferenceType)8);
			IList<Reference> references8 = familyInstance.GetReferences((FamilyInstanceReferenceType)6);
			if ((references7 != null && references7.Count > 0) || (references8 != null && references8.Count > 0))
			{
				if (references7 != null)
				{
					foreach (Reference item5 in references7)
					{
						list.Add(new GeometryReferenceWithPoint
						{
							Point = point,
							Reference = item5
						});
					}
				}
				if (references8 != null)
				{
					foreach (Reference item6 in references8)
					{
						list.Add(new GeometryReferenceWithPoint
						{
							Point = point,
							Reference = item6
						});
					}
				}
				return list;
			}
			IList<Reference> references9 = familyInstance.GetReferences((FamilyInstanceReferenceType)9);
			if (references9 != null && references9.Count > 0)
			{
				foreach (Reference item7 in references9)
				{
					list.Add(new GeometryReferenceWithPoint
					{
						Point = point,
						Reference = item7
					});
				}
				return list;
			}
			return list;
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[GeometryReferenceExtractor] 提取门窗 ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)familyInstance).Id);
			defaultInterpolatedStringHandler.AppendLiteral(" 的几何参考失败: ");
			defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
			Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		return list;
	}

	private List<GeometryReferenceWithPoint> GetReferencePointsFromGeometry(FamilyInstance familyInstance, IList<Reference> refs1, IList<Reference> refs2, XYZ dimensionDirection)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		List<GeometryReferenceWithPoint> list = new List<GeometryReferenceWithPoint>();
		try
		{
			Options val = new Options
			{
				ComputeReferences = true,
				DetailLevel = (ViewDetailLevel)3
			};
			GeometryElement val2 = ((Element)familyInstance).get_Geometry(val);
			if ((GeometryObject)(object)val2 == (GeometryObject)null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[GeometryReferenceExtractor] 门窗 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)familyInstance).Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 无法获取几何信息来计算参照点位置");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return list;
			}
			XYZ targetNormal = dimensionDirection.Normalize();
			List<(Face, PlanarFace, Transform)> list2 = new List<(Face, PlanarFace, Transform)>();
			CollectFacesWithTransform(val2, targetNormal, list2, Transform.Identity, 0, ((Element)familyInstance).Id);
			if (list2.Count == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(42, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("[GeometryReferenceExtractor] 门窗 ");
				defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(((Element)familyInstance).Id);
				defaultInterpolatedStringHandler2.AppendLiteral(" 未找到符合方向的面");
				Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
				return list;
			}
			var source = (from x in Enumerable.Select(list2, delegate((Face Face, PlanarFace PlanarFace, Transform Transform) f)
				{
					//IL_004e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0054: Expected O, but got Unknown
					BoundingBoxUV boundingBox = f.Face.GetBoundingBox();
					UV val3 = new UV((boundingBox.Min.U + boundingBox.Max.U) / 2.0, (boundingBox.Min.V + boundingBox.Max.V) / 2.0);
					XYZ val4 = f.Face.Evaluate(val3);
					if (f.Transform != null)
					{
						val4 = f.Transform.OfPoint(val4);
					}
					double projection = val4.DotProduct(dimensionDirection);
					return new
					{
						Face = f,
						Center = val4,
						Projection = projection
					};
				})
				orderby x.Projection
				select x).ToList();
			var anon = source.First();
			var anon2 = source.Last();
			foreach (Reference item in refs1)
			{
				list.Add(new GeometryReferenceWithPoint
				{
					Point = anon.Center,
					Reference = item
				});
			}
			foreach (Reference item2 in refs2)
			{
				list.Add(new GeometryReferenceWithPoint
				{
					Point = anon2.Center,
					Reference = item2
				});
			}
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(45, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("[GeometryReferenceExtractor] 计算门窗 ");
			defaultInterpolatedStringHandler3.AppendFormatted<ElementId>(((Element)familyInstance).Id);
			defaultInterpolatedStringHandler3.AppendLiteral(" 的参照点位置失败: ");
			defaultInterpolatedStringHandler3.AppendFormatted(ex.Message);
			Logger.Error(defaultInterpolatedStringHandler3.ToStringAndClear());
		}
		return list;
	}

	private void CollectFacesWithTransform(GeometryElement geomElement, XYZ targetNormal, List<(Face Face, PlanarFace PlanarFace, Transform? Transform)> result, Transform? transform = null, int depth = 0, ElementId? elementId = null)
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		if (depth > 10)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[GeometryReferenceExtractor] 达到最大递归深度 ");
			defaultInterpolatedStringHandler.AppendFormatted(10);
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (GeometryObject item in geomElement)
		{
			Solid val = (Solid)(object)((item is Solid) ? item : null);
			if (val != null)
			{
				if (val.Volume < 0.0 || Math.Abs(val.Volume) < 0.0001)
				{
					continue;
				}
				num++;
				foreach (Face face in val.Faces)
				{
					Face val2 = face;
					num2++;
					PlanarFace val3 = (PlanarFace)(object)((val2 is PlanarFace) ? val2 : null);
					if (val3 != null)
					{
						num3++;
						XYZ val4 = val3.FaceNormal.Normalize();
						if (transform != null)
						{
							val4 = transform.OfVector(val4).Normalize();
						}
						double num5 = Math.Abs(val4.DotProduct(targetNormal));
						if (num5 >= 0.7)
						{
							num4++;
							result.Add((val2, val3, transform));
						}
					}
				}
				continue;
			}
			GeometryInstance val5 = (GeometryInstance)(object)((item is GeometryInstance) ? item : null);
			if (val5 != null)
			{
				Transform transform2 = val5.Transform;
				Transform transform3 = ((transform != null) ? transform.Multiply(transform2) : transform2);
				GeometryElement instanceGeometry = val5.GetInstanceGeometry();
				if ((GeometryObject)(object)instanceGeometry != (GeometryObject)null)
				{
					CollectFacesWithTransform(instanceGeometry, targetNormal, result, transform3, depth + 1, elementId);
				}
			}
		}
	}

	private void CollectWindowDoorFaces(GeometryElement geomElement, XYZ targetNormal, List<(GeometryReferenceWithPoint, double)> result, Transform? transform = null, int depth = 0)
	{
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Expected O, but got Unknown
		if (depth > 10)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[GeometryReferenceExtractor] 达到最大递归深度 ");
			defaultInterpolatedStringHandler.AppendFormatted(10);
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		foreach (GeometryObject item2 in geomElement)
		{
			Solid val = (Solid)(object)((item2 is Solid) ? item2 : null);
			if (val != null)
			{
				if (val.Volume < 0.0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("[GeometryReferenceExtractor] 跳过空心几何，体积: ");
					defaultInterpolatedStringHandler2.AppendFormatted(val.Volume, "F2");
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					continue;
				}
				if (Math.Abs(val.Volume) < 0.0001)
				{
					Logger.Info("[GeometryReferenceExtractor] 跳过体积为 0 的几何");
					continue;
				}
				num++;
				foreach (Face face in val.Faces)
				{
					Face val2 = face;
					num3++;
					PlanarFace val3 = (PlanarFace)(object)((val2 is PlanarFace) ? val2 : null);
					if (val3 == null)
					{
						continue;
					}
					XYZ val4 = val3.FaceNormal.Normalize();
					if (transform != null)
					{
						val4 = transform.OfVector(val4).Normalize();
					}
					double num6 = Math.Abs(val4.DotProduct(targetNormal));
					if (depth == 0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(58, 7);
						defaultInterpolatedStringHandler3.AppendLiteral("[GeometryReferenceExtractor] 面法线: (");
						defaultInterpolatedStringHandler3.AppendFormatted(val4.X, "F2");
						defaultInterpolatedStringHandler3.AppendLiteral(", ");
						defaultInterpolatedStringHandler3.AppendFormatted(val4.Y, "F2");
						defaultInterpolatedStringHandler3.AppendLiteral(", ");
						defaultInterpolatedStringHandler3.AppendFormatted(val4.Z, "F2");
						defaultInterpolatedStringHandler3.AppendLiteral("), 目标: (");
						defaultInterpolatedStringHandler3.AppendFormatted(targetNormal.X, "F2");
						defaultInterpolatedStringHandler3.AppendLiteral(", ");
						defaultInterpolatedStringHandler3.AppendFormatted(targetNormal.Y, "F2");
						defaultInterpolatedStringHandler3.AppendLiteral(", ");
						defaultInterpolatedStringHandler3.AppendFormatted(targetNormal.Z, "F2");
						defaultInterpolatedStringHandler3.AppendLiteral("), 点积: ");
						defaultInterpolatedStringHandler3.AppendFormatted(num6, "F3");
						Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
					}
					if (num6 < 0.9)
					{
						continue;
					}
					num4++;
					try
					{
						Reference reference = val2.Reference;
						if (reference != null)
						{
							num5++;
							BoundingBoxUV boundingBox = val2.GetBoundingBox();
							UV val5 = new UV((boundingBox.Min.U + boundingBox.Max.U) / 2.0, (boundingBox.Min.V + boundingBox.Max.V) / 2.0);
							XYZ val6 = val2.Evaluate(val5);
							if (transform != null)
							{
								val6 = transform.OfPoint(val6);
							}
							GeometryReferenceWithPoint item = new GeometryReferenceWithPoint
							{
								Point = val6,
								Reference = reference
							};
							result.Add((item, ((Face)val3).Area));
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(36, 1);
							defaultInterpolatedStringHandler4.AppendLiteral("[GeometryReferenceExtractor] 面 ");
							defaultInterpolatedStringHandler4.AppendFormatted(num3);
							defaultInterpolatedStringHandler4.AppendLiteral(" 没有引用");
							Logger.Warning(defaultInterpolatedStringHandler4.ToStringAndClear());
						}
					}
					catch (Exception ex)
					{
						Logger.Warning("[GeometryReferenceExtractor] 处理面时出错: " + ex.Message);
					}
				}
				continue;
			}
			GeometryInstance val7 = (GeometryInstance)(object)((item2 is GeometryInstance) ? item2 : null);
			if (val7 != null)
			{
				num2++;
				Transform transform2 = val7.Transform;
				Transform transform3 = ((transform != null) ? transform.Multiply(transform2) : transform2);
				GeometryElement instanceGeometry = val7.GetInstanceGeometry();
				if ((GeometryObject)(object)instanceGeometry != (GeometryObject)null)
				{
					CollectWindowDoorFaces(instanceGeometry, targetNormal, result, transform3, depth + 1);
				}
			}
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(78, 6);
		defaultInterpolatedStringHandler5.AppendLiteral("[GeometryReferenceExtractor] 深度 ");
		defaultInterpolatedStringHandler5.AppendFormatted(depth);
		defaultInterpolatedStringHandler5.AppendLiteral(": Solid=");
		defaultInterpolatedStringHandler5.AppendFormatted(num);
		defaultInterpolatedStringHandler5.AppendLiteral(", GeometryInstance=");
		defaultInterpolatedStringHandler5.AppendFormatted(num2);
		defaultInterpolatedStringHandler5.AppendLiteral(", 总面数=");
		defaultInterpolatedStringHandler5.AppendFormatted(num3);
		defaultInterpolatedStringHandler5.AppendLiteral(", 符合法线=");
		defaultInterpolatedStringHandler5.AppendFormatted(num4);
		defaultInterpolatedStringHandler5.AppendLiteral(", 有引用=");
		defaultInterpolatedStringHandler5.AppendFormatted(num5);
		Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
	}

	private void ProcessGeometryObject(GeometryElement geomElement, XYZ targetNormal, List<GeometryReferenceWithPoint> result, Transform? transform = null, int depth = 0)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		if (depth > 10)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[GeometryReferenceExtractor] 达到最大递归深度 ");
			defaultInterpolatedStringHandler.AppendFormatted(10);
			defaultInterpolatedStringHandler.AppendLiteral("，停止处理");
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			return;
		}
		foreach (GeometryObject item in geomElement)
		{
			Solid val = (Solid)(object)((item is Solid) ? item : null);
			if (val != null)
			{
				foreach (Face face in val.Faces)
				{
					Face val2 = face;
					PlanarFace val3 = (PlanarFace)(object)((val2 is PlanarFace) ? val2 : null);
					if (val3 == null)
					{
						continue;
					}
					XYZ val4 = val3.FaceNormal.Normalize();
					if (transform != null)
					{
						val4 = transform.OfVector(val4).Normalize();
					}
					double num = Math.Abs(val4.DotProduct(targetNormal));
					if (num < 0.9)
					{
						continue;
					}
					try
					{
						Reference reference = val2.Reference;
						if (reference != null)
						{
							BoundingBoxUV boundingBox = val2.GetBoundingBox();
							UV val5 = new UV((boundingBox.Min.U + boundingBox.Max.U) / 2.0, (boundingBox.Min.V + boundingBox.Max.V) / 2.0);
							XYZ val6 = val2.Evaluate(val5);
							if (transform != null)
							{
								val6 = transform.OfPoint(val6);
							}
							double area = ((Face)val3).Area;
							if (area > 50.0)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(54, 1);
								defaultInterpolatedStringHandler2.AppendLiteral("[GeometryReferenceExtractor] 过滤掉大面积面（可能是墙参照），面积: ");
								defaultInterpolatedStringHandler2.AppendFormatted(area, "F2");
								defaultInterpolatedStringHandler2.AppendLiteral(" 平方英尺");
								Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
							}
							else
							{
								result.Add(new GeometryReferenceWithPoint
								{
									Point = val6,
									Reference = reference
								});
							}
						}
					}
					catch
					{
					}
				}
				continue;
			}
			GeometryInstance val7 = (GeometryInstance)(object)((item is GeometryInstance) ? item : null);
			if (val7 != null)
			{
				Transform transform2 = val7.Transform;
				Transform transform3 = ((transform != null) ? transform.Multiply(transform2) : transform2);
				GeometryElement instanceGeometry = val7.GetInstanceGeometry();
				if ((GeometryObject)(object)instanceGeometry != (GeometryObject)null)
				{
					ProcessGeometryObject(instanceGeometry, targetNormal, result, transform3, depth + 1);
				}
			}
		}
	}

	private List<GeometryReferencePoint> ExtractWallLocationFallback(Wall wall, XYZ dimensionDirection)
	{
		List<GeometryReferencePoint> list = new List<GeometryReferencePoint>();
		try
		{
			Location location = ((Element)wall).Location;
			LocationCurve val = (LocationCurve)(object)((location is LocationCurve) ? location : null);
			if (val != null)
			{
				Curve curve = val.Curve;
				Line val2 = (Line)(object)((curve is Line) ? curve : null);
				if (val2 != null)
				{
					list.Add(new GeometryReferencePoint
					{
						Position = ((Curve)val2).GetEndPoint(0),
						Reference = null,
						ProjectionOnDimensionLine = ((Curve)val2).GetEndPoint(0).DotProduct(dimensionDirection),
						ElementId = ((Element)wall).Id
					});
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[GeometryReferenceExtractor] 墙 ");
					defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)wall).Id);
					defaultInterpolatedStringHandler.AppendLiteral(" 使用位置曲线作为备选");
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[GeometryReferenceExtractor] 墙 ");
			defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(((Element)wall).Id);
			defaultInterpolatedStringHandler2.AppendLiteral(" 备选方案失败: ");
			defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
			Logger.Error(defaultInterpolatedStringHandler2.ToStringAndClear());
		}
		return list;
	}

	private List<GeometryReferencePoint> ExtractFamilyInstanceLocationFallback(FamilyInstance familyInstance, XYZ dimensionDirection)
	{
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Expected O, but got Unknown
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Expected O, but got Unknown
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Expected O, but got Unknown
		List<GeometryReferencePoint> list = new List<GeometryReferencePoint>();
		try
		{
			Location location = ((Element)familyInstance).Location;
			LocationPoint val = (LocationPoint)(object)((location is LocationPoint) ? location : null);
			XYZ val2 = ((val != null) ? val.Point : null);
			Location location2 = ((Element)familyInstance).Location;
			LocationCurve val3 = (LocationCurve)(object)((location2 is LocationCurve) ? location2 : null);
			XYZ val4 = dimensionDirection.Normalize();
			bool flag;
			if (flag = Math.Abs(val4.X) > Math.Abs(val4.Y))
			{
				IList<Reference> references = familyInstance.GetReferences((FamilyInstanceReferenceType)1);
				if (references != null && references.Count > 0)
				{
					foreach (Reference item in references)
					{
						list.Add(new GeometryReferencePoint
						{
							Position = (val2 ?? XYZ.Zero),
							Reference = item,
							ProjectionOnDimensionLine = (val2 ?? XYZ.Zero).DotProduct(dimensionDirection),
							ElementId = ((Element)familyInstance).Id
						});
					}
					return list;
				}
			}
			else
			{
				IList<Reference> references2 = familyInstance.GetReferences((FamilyInstanceReferenceType)4);
				if (references2 != null && references2.Count > 0)
				{
					foreach (Reference item2 in references2)
					{
						list.Add(new GeometryReferencePoint
						{
							Position = (val2 ?? XYZ.Zero),
							Reference = item2,
							ProjectionOnDimensionLine = (val2 ?? XYZ.Zero).DotProduct(dimensionDirection),
							ElementId = ((Element)familyInstance).Id
						});
					}
					return list;
				}
			}
			IList<Reference> references3 = familyInstance.GetReferences((FamilyInstanceReferenceType)9);
			if (references3 != null && references3.Count > 0)
			{
				foreach (Reference item3 in references3)
				{
					list.Add(new GeometryReferencePoint
					{
						Position = (val2 ?? XYZ.Zero),
						Reference = item3,
						ProjectionOnDimensionLine = (val2 ?? XYZ.Zero).DotProduct(dimensionDirection),
						ElementId = ((Element)familyInstance).Id
					});
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[GeometryReferenceExtractor] 族实例 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)familyInstance).Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 使用 StrongReference 参照");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				return list;
			}
			if (flag)
			{
				IList<Reference> references4 = familyInstance.GetReferences((FamilyInstanceReferenceType)0);
				IList<Reference> references5 = familyInstance.GetReferences((FamilyInstanceReferenceType)2);
				if (references4 != null && references4.Count > 0 && references5 != null && references5.Count > 0)
				{
					foreach (Reference item4 in references4)
					{
						list.Add(new GeometryReferencePoint
						{
							Position = (val2 ?? XYZ.Zero),
							Reference = item4,
							ProjectionOnDimensionLine = (val2 ?? XYZ.Zero).DotProduct(dimensionDirection),
							ElementId = ((Element)familyInstance).Id
						});
					}
					foreach (Reference item5 in references5)
					{
						list.Add(new GeometryReferencePoint
						{
							Position = (val2 ?? XYZ.Zero),
							Reference = item5,
							ProjectionOnDimensionLine = (val2 ?? XYZ.Zero).DotProduct(dimensionDirection),
							ElementId = ((Element)familyInstance).Id
						});
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(50, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("[GeometryReferenceExtractor] 族实例 ");
					defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(((Element)familyInstance).Id);
					defaultInterpolatedStringHandler2.AppendLiteral(" 使用 Left/Right 参照");
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					return list;
				}
			}
			else
			{
				IList<Reference> references6 = familyInstance.GetReferences((FamilyInstanceReferenceType)3);
				IList<Reference> references7 = familyInstance.GetReferences((FamilyInstanceReferenceType)5);
				if (references6 != null && references6.Count > 0 && references7 != null && references7.Count > 0)
				{
					foreach (Reference item6 in references6)
					{
						list.Add(new GeometryReferencePoint
						{
							Position = (val2 ?? XYZ.Zero),
							Reference = item6,
							ProjectionOnDimensionLine = (val2 ?? XYZ.Zero).DotProduct(dimensionDirection),
							ElementId = ((Element)familyInstance).Id
						});
					}
					foreach (Reference item7 in references7)
					{
						list.Add(new GeometryReferencePoint
						{
							Position = (val2 ?? XYZ.Zero),
							Reference = item7,
							ProjectionOnDimensionLine = (val2 ?? XYZ.Zero).DotProduct(dimensionDirection),
							ElementId = ((Element)familyInstance).Id
						});
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(50, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("[GeometryReferenceExtractor] 族实例 ");
					defaultInterpolatedStringHandler3.AppendFormatted<ElementId>(((Element)familyInstance).Id);
					defaultInterpolatedStringHandler3.AppendLiteral(" 使用 Front/Back 参照");
					Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
					return list;
				}
			}
			if (val3 != null)
			{
				Curve curve = val3.Curve;
				Line val5 = (Line)(object)((curve is Line) ? curve : null);
				if (val5 != null)
				{
					list.Add(new GeometryReferencePoint
					{
						Position = ((Curve)val5).GetEndPoint(0),
						Reference = new Reference((Element)(object)familyInstance),
						ProjectionOnDimensionLine = ((Curve)val5).GetEndPoint(0).DotProduct(dimensionDirection),
						ElementId = ((Element)familyInstance).Id
					});
					list.Add(new GeometryReferencePoint
					{
						Position = ((Curve)val5).GetEndPoint(1),
						Reference = new Reference((Element)(object)familyInstance),
						ProjectionOnDimensionLine = ((Curve)val5).GetEndPoint(1).DotProduct(dimensionDirection),
						ElementId = ((Element)familyInstance).Id
					});
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(44, 1);
					defaultInterpolatedStringHandler4.AppendLiteral("[GeometryReferenceExtractor] 族实例 ");
					defaultInterpolatedStringHandler4.AppendFormatted<ElementId>(((Element)familyInstance).Id);
					defaultInterpolatedStringHandler4.AppendLiteral(" 使用位置曲线作为备选");
					Logger.Warning(defaultInterpolatedStringHandler4.ToStringAndClear());
					return list;
				}
			}
			if (val != null)
			{
				list.Add(new GeometryReferencePoint
				{
					Position = val.Point,
					Reference = new Reference((Element)(object)familyInstance),
					ProjectionOnDimensionLine = val.Point.DotProduct(dimensionDirection),
					ElementId = ((Element)familyInstance).Id
				});
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(57, 1);
				defaultInterpolatedStringHandler5.AppendLiteral("[GeometryReferenceExtractor] 族实例 ");
				defaultInterpolatedStringHandler5.AppendFormatted<ElementId>(((Element)familyInstance).Id);
				defaultInterpolatedStringHandler5.AppendLiteral(" 使用位置点和元素参照作为最后备选（可能不支持）");
				Logger.Warning(defaultInterpolatedStringHandler5.ToStringAndClear());
			}
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(42, 2);
			defaultInterpolatedStringHandler6.AppendLiteral("[GeometryReferenceExtractor] 族实例 ");
			defaultInterpolatedStringHandler6.AppendFormatted<ElementId>(((Element)familyInstance).Id);
			defaultInterpolatedStringHandler6.AppendLiteral(" 备选方案失败: ");
			defaultInterpolatedStringHandler6.AppendFormatted(ex.Message);
			Logger.Error(defaultInterpolatedStringHandler6.ToStringAndClear());
		}
		return list;
	}

	private List<GeometryReferencePoint> ExtractCurveElementReferences(CurveElement curveElement, XYZ dimensionDirection)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		List<GeometryReferencePoint> list = new List<GeometryReferencePoint>();
		try
		{
			Curve geometryCurve = curveElement.GeometryCurve;
			Line val = (Line)(object)((geometryCurve is Line) ? geometryCurve : null);
			if (val != null)
			{
				list.Add(new GeometryReferencePoint
				{
					Position = ((Curve)val).GetEndPoint(0),
					Reference = new Reference((Element)(object)curveElement),
					ProjectionOnDimensionLine = ((Curve)val).GetEndPoint(0).DotProduct(dimensionDirection),
					ElementId = ((Element)curveElement).Id
				});
				list.Add(new GeometryReferencePoint
				{
					Position = ((Curve)val).GetEndPoint(1),
					Reference = new Reference((Element)(object)curveElement),
					ProjectionOnDimensionLine = ((Curve)val).GetEndPoint(1).DotProduct(dimensionDirection),
					ElementId = ((Element)curveElement).Id
				});
			}
			else
			{
				Arc val2 = (Arc)(object)((geometryCurve is Arc) ? geometryCurve : null);
				if (val2 != null)
				{
					list.Add(new GeometryReferencePoint
					{
						Position = ((Curve)val2).GetEndPoint(0),
						Reference = new Reference((Element)(object)curveElement),
						ProjectionOnDimensionLine = ((Curve)val2).GetEndPoint(0).DotProduct(dimensionDirection),
						ElementId = ((Element)curveElement).Id
					});
					list.Add(new GeometryReferencePoint
					{
						Position = ((Curve)val2).Evaluate(0.5, true),
						Reference = new Reference((Element)(object)curveElement),
						ProjectionOnDimensionLine = ((Curve)val2).Evaluate(0.5, true).DotProduct(dimensionDirection),
						ElementId = ((Element)curveElement).Id
					});
					list.Add(new GeometryReferencePoint
					{
						Position = ((Curve)val2).GetEndPoint(1),
						Reference = new Reference((Element)(object)curveElement),
						ProjectionOnDimensionLine = ((Curve)val2).GetEndPoint(1).DotProduct(dimensionDirection),
						ElementId = ((Element)curveElement).Id
					});
				}
			}
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[GeometryReferenceExtractor] 提取曲线元素 ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)curveElement).Id);
			defaultInterpolatedStringHandler.AppendLiteral(" 的参考失败: ");
			defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
			Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		return list;
	}

	private List<GeometryReferencePoint> ExtractGridReferences(Grid grid, XYZ dimensionDirection)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		List<GeometryReferencePoint> list = new List<GeometryReferencePoint>();
		try
		{
			Curve curve = grid.Curve;
			Line val = (Line)(object)((curve is Line) ? curve : null);
			if (val != null)
			{
				list.Add(new GeometryReferencePoint
				{
					Position = ((Curve)val).GetEndPoint(0),
					Reference = new Reference((Element)(object)grid),
					ProjectionOnDimensionLine = ((Curve)val).GetEndPoint(0).DotProduct(dimensionDirection),
					ElementId = ((Element)grid).Id
				});
			}
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[GeometryReferenceExtractor] 提取轴网 ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)grid).Id);
			defaultInterpolatedStringHandler.AppendLiteral(" 的参考失败: ");
			defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
			Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		return list;
	}

	public GeometryReferencePoint? ExtractElementCenterReference(Element element, XYZ direction)
	{
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
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
						XYZ position = ((Curve)val3).Evaluate(0.5, true);
						return new GeometryReferencePoint
						{
							Position = position,
							Reference = new Reference((Element)(object)val),
							ProjectionOnDimensionLine = 0.0,
							ElementId = element.Id
						};
					}
				}
			}
			FamilyInstance val4 = (FamilyInstance)(object)((element is FamilyInstance) ? element : null);
			if (val4 != null)
			{
				Location location2 = ((Element)val4).Location;
				LocationCurve val5 = (LocationCurve)(object)((location2 is LocationCurve) ? location2 : null);
				if (val5 != null)
				{
					Curve curve2 = val5.Curve;
					Line val6 = (Line)(object)((curve2 is Line) ? curve2 : null);
					if (val6 != null)
					{
						XYZ position2 = ((Curve)val6).Evaluate(0.5, true);
						return new GeometryReferencePoint
						{
							Position = position2,
							Reference = new Reference((Element)(object)val4),
							ProjectionOnDimensionLine = 0.0,
							ElementId = element.Id
						};
					}
				}
				Location location3 = ((Element)val4).Location;
				LocationPoint val7 = (LocationPoint)(object)((location3 is LocationPoint) ? location3 : null);
				if (val7 != null)
				{
					return new GeometryReferencePoint
					{
						Position = val7.Point,
						Reference = new Reference((Element)(object)val4),
						ProjectionOnDimensionLine = 0.0,
						ElementId = element.Id
					};
				}
			}
			CurveElement val8 = (CurveElement)(object)((element is CurveElement) ? element : null);
			if (val8 != null)
			{
				Curve geometryCurve = val8.GeometryCurve;
				Line val9 = (Line)(object)((geometryCurve is Line) ? geometryCurve : null);
				if (val9 != null)
				{
					XYZ position3 = ((Curve)val9).Evaluate(0.5, true);
					return new GeometryReferencePoint
					{
						Position = position3,
						Reference = new Reference((Element)(object)val8),
						ProjectionOnDimensionLine = 0.0,
						ElementId = element.Id
					};
				}
			}
			Grid val10 = (Grid)(object)((element is Grid) ? element : null);
			if (val10 != null)
			{
				Curve curve3 = val10.Curve;
				Line val11 = (Line)(object)((curve3 is Line) ? curve3 : null);
				if (val11 != null)
				{
					XYZ position4 = ((Curve)val11).Evaluate(0.5, true);
					return new GeometryReferencePoint
					{
						Position = position4,
						Reference = new Reference((Element)(object)val10),
						ProjectionOnDimensionLine = 0.0,
						ElementId = element.Id
					};
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			Logger.Warning("[GeometryReferenceExtractor] 提取元素中心参考失败: " + ex.Message);
			return null;
		}
	}

	private List<GeometryReferencePoint> ExtractMEPElementReferences(Element element, XYZ dimensionDirection, string elementTypeName)
	{
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Expected O, but got Unknown
		List<GeometryReferencePoint> list = new List<GeometryReferencePoint>();
		try
		{
			Location location = element.Location;
			LocationCurve val = (LocationCurve)(object)((location is LocationCurve) ? location : null);
			if (val == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[GeometryReferenceExtractor] ");
				defaultInterpolatedStringHandler.AppendFormatted(elementTypeName);
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(element.Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 没有位置曲线");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return list;
			}
			Curve curve = val.Curve;
			if ((GeometryObject)(object)curve == (GeometryObject)null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(37, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("[GeometryReferenceExtractor] ");
				defaultInterpolatedStringHandler2.AppendFormatted(elementTypeName);
				defaultInterpolatedStringHandler2.AppendLiteral(" ");
				defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(element.Id);
				defaultInterpolatedStringHandler2.AppendLiteral(" 位置曲线为空");
				Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
				return list;
			}
			Line val2 = (Line)(object)((curve is Line) ? curve : null);
			XYZ val3;
			if (val2 != null)
			{
				val3 = ((Curve)val2).Evaluate(0.5, true);
			}
			else
			{
				Arc val4 = (Arc)(object)((curve is Arc) ? curve : null);
				if (val4 != null)
				{
					val3 = ((Curve)val4).Evaluate(0.5, true);
				}
				else
				{
					val3 = curve.GetEndPoint(0);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(53, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("[GeometryReferenceExtractor] ");
					defaultInterpolatedStringHandler3.AppendFormatted(elementTypeName);
					defaultInterpolatedStringHandler3.AppendLiteral(" ");
					defaultInterpolatedStringHandler3.AppendFormatted<ElementId>(element.Id);
					defaultInterpolatedStringHandler3.AppendLiteral(" 曲线类型不是 Line 或 Arc，使用起点");
					Logger.Warning(defaultInterpolatedStringHandler3.ToStringAndClear());
				}
			}
			double projectionOnDimensionLine = val3.DotProduct(dimensionDirection);
			GeometryReferencePoint item = new GeometryReferencePoint
			{
				Position = val3,
				Reference = new Reference(element),
				ProjectionOnDimensionLine = projectionOnDimensionLine,
				ElementId = element.Id
			};
			list.Add(item);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(47, 5);
			defaultInterpolatedStringHandler4.AppendLiteral("[GeometryReferenceExtractor] ");
			defaultInterpolatedStringHandler4.AppendFormatted(elementTypeName);
			defaultInterpolatedStringHandler4.AppendLiteral(" ");
			defaultInterpolatedStringHandler4.AppendFormatted<ElementId>(element.Id);
			defaultInterpolatedStringHandler4.AppendLiteral(" 使用元素参照，位置=(");
			defaultInterpolatedStringHandler4.AppendFormatted(val3.X, "F2");
			defaultInterpolatedStringHandler4.AppendLiteral(", ");
			defaultInterpolatedStringHandler4.AppendFormatted(val3.Y, "F2");
			defaultInterpolatedStringHandler4.AppendLiteral(", ");
			defaultInterpolatedStringHandler4.AppendFormatted(val3.Z, "F2");
			defaultInterpolatedStringHandler4.AppendLiteral(")");
			Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(41, 3);
			defaultInterpolatedStringHandler5.AppendLiteral("[GeometryReferenceExtractor] 提取 ");
			defaultInterpolatedStringHandler5.AppendFormatted(elementTypeName);
			defaultInterpolatedStringHandler5.AppendLiteral(" ");
			defaultInterpolatedStringHandler5.AppendFormatted<ElementId>(element.Id);
			defaultInterpolatedStringHandler5.AppendLiteral(" 的参照失败: ");
			defaultInterpolatedStringHandler5.AppendFormatted(ex.Message);
			Logger.Error(defaultInterpolatedStringHandler5.ToStringAndClear());
		}
		return list;
	}
}
