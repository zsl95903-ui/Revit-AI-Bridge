using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Services;

internal sealed class AnalysisService : IAnalysisService
{
	private readonly UIApplication _application;

	public AnalysisService(UIApplication application)
	{
		_application = application ?? throw new ArgumentNullException("application");
	}

	public double? GetElementArea(object element)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			Parameter val2 = val.get_Parameter((BuiltInParameter)(-1012805L));
			if (val2 != null && (int)val2.StorageType == 2)
			{
				return val2.AsDouble();
			}
			Options val3 = new Options();
			GeometryElement val4 = val.get_Geometry(val3);
			if ((GeometryObject)(object)val4 != (GeometryObject)null)
			{
				foreach (GeometryObject item in val4)
				{
					Solid val5 = (Solid)(object)((item is Solid) ? item : null);
					if (val5 == null || !(val5.Volume > 0.0))
					{
						continue;
					}
					double num = 0.0;
					foreach (Face face in val5.Faces)
					{
						Face val6 = face;
						num += val6.Area;
					}
					return num;
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetElementArea 失败: " + ex.Message);
			return null;
		}
	}

	public double? GetElementVolume(object element)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			Parameter val2 = val.get_Parameter((BuiltInParameter)(-1012806L));
			if (val2 != null && (int)val2.StorageType == 2)
			{
				return val2.AsDouble();
			}
			Options val3 = new Options();
			GeometryElement val4 = val.get_Geometry(val3);
			if ((GeometryObject)(object)val4 != (GeometryObject)null)
			{
				foreach (GeometryObject item in val4)
				{
					Solid val5 = (Solid)(object)((item is Solid) ? item : null);
					if (val5 != null && val5.Volume > 0.0)
					{
						return val5.Volume;
					}
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetElementVolume 失败: " + ex.Message);
			return null;
		}
	}

	public double? GetElementLength(object element)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Invalid comparison between Unknown and I4
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			CurveElement val2 = (CurveElement)(object)((val is CurveElement) ? val : null);
			if (val2 != null)
			{
				return val2.GeometryCurve.Length;
			}
			Parameter val3 = val.get_Parameter((BuiltInParameter)(-1004005L));
			if (val3 != null && (int)val3.StorageType == 2)
			{
				return val3.AsDouble();
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetElementLength 失败: " + ex.Message);
			return null;
		}
	}

	public (bool HasCollision, IEnumerable<(double X, double Y, double Z)> CollisionPoints) CheckCollision(object document, object element1, object element2)
	{
		try
		{
			LogError("CheckCollision: 方法尚未实现");
			return (HasCollision: false, CollisionPoints: Enumerable.Empty<(double, double, double)>());
		}
		catch (Exception ex)
		{
			LogError("CheckCollision 失败: " + ex.Message);
			return (HasCollision: false, CollisionPoints: Enumerable.Empty<(double, double, double)>());
		}
	}

	public object? GetElementGeometry(object element)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			Options val2 = new Options();
			return val.get_Geometry(val2);
		}
		catch (Exception ex)
		{
			LogError("GetElementGeometry 失败: " + ex.Message);
			return null;
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

	public ((double MinX, double MinY, double MinZ)? Min, (double MaxX, double MaxY, double MaxZ)? Max)? GetBoundingBox(object element)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			BoundingBoxXYZ val2 = val.get_BoundingBox((View)null);
			if (val2 != null)
			{
				return ((val2.Min.X, val2.Min.Y, val2.Min.Z), (val2.Max.X, val2.Max.Y, val2.Max.Z));
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetBoundingBox 失败: " + ex.Message);
			return null;
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

	public IEnumerable<(int ElementId1, int ElementId2, IEnumerable<(double X, double Y, double Z)> CollisionPoints)> CheckCollisions(object document, IEnumerable<int> elementIds)
	{
		Document doc = (Document)((document is Document) ? document : null);
		if (doc == null)
		{
			Logger.Error("[AnalysisService] CheckCollisions: document 不是 Document 类型");
			yield break;
		}
		List<int> elementIdList = elementIds.ToList();
		if (elementIdList.Count < 2)
		{
			Logger.Warning("[AnalysisService] CheckCollisions: 至少需要2个元素才能检测碰撞");
			yield break;
		}
		List<(int, int, List<(double, double, double)>)> collisions = new List<(int, int, List<(double, double, double)>)>();
		try
		{
			List<ElementId> elementIdsRevit = ((IEnumerable<int>)elementIdList).Select((Func<int, ElementId>)delegate(int num)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				return new ElementId((long)num);
			}).ToList();
			for (int i = 0; i < elementIdsRevit.Count; i++)
			{
				Element elem1 = doc.GetElement(elementIdsRevit[i]);
				if (elem1 == null)
				{
					continue;
				}
				List<ElementId> remainingIds = elementIdsRevit.Skip(i + 1).ToList();
				if (remainingIds.Count == 0)
				{
					continue;
				}
				FilteredElementCollector collector = new FilteredElementCollector(doc, (ICollection<ElementId>)remainingIds);
				ElementIntersectsElementFilter filter = new ElementIntersectsElementFilter(elem1, false);
				IList<Element> intersectingElements = collector.WherePasses((ElementFilter)(object)filter).ToElements();
				foreach (Element elem2 in intersectingElements)
				{
					int id1 = (int)elem1.Id.Value;
					int id2 = (int)elem2.Id.Value;
					List<(double X, double Y, double Z)> collisionPoints = ExtractCollisionPoints(elem1, elem2);
					collisions.Add((id1, id2, collisionPoints));
				}
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			Logger.Error("[AnalysisService] CheckCollisions 失败: " + ex2.Message);
		}
		foreach (var collision in collisions)
		{
			yield return (ElementId1: collision.Item1, ElementId2: collision.Item2, CollisionPoints: collision.Item3.Distinct().ToList());
		}
	}

	public IEnumerable<(int DocumentIndex1, int ElementId1, int DocumentIndex2, int ElementId2, IEnumerable<(double X, double Y, double Z)> CollisionPoints)> CheckCollisionsCrossDocument(IList<object> documents, IDictionary<int, IEnumerable<int>> elementIdsByDocument)
	{
		if (documents == null || documents.Count < 2)
		{
			Logger.Warning("[AnalysisService] CheckCollisionsCrossDocument: 至少需要2个文档");
			yield break;
		}
		if (elementIdsByDocument == null || elementIdsByDocument.Count == 0)
		{
			Logger.Warning("[AnalysisService] CheckCollisionsCrossDocument: 元素ID列表为空");
			yield break;
		}
		List<(int, int, int, int, List<(double, double, double)>)> collisions = new List<(int, int, int, int, List<(double, double, double)>)>();
		try
		{
			List<(int DocIndex, int ElementId, List<Solid> Solids, BoundingBoxXYZ BBox)> allGeometries = new List<(int, int, List<Solid>, BoundingBoxXYZ)>();
			Options opt = new Options
			{
				DetailLevel = (ViewDetailLevel)3,
				ComputeReferences = false
			};
			foreach (KeyValuePair<int, IEnumerable<int>> kvp in elementIdsByDocument)
			{
				int docIndex = kvp.Key;
				IEnumerable<int> elementIds = kvp.Value;
				if (docIndex < 0 || docIndex >= documents.Count)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(58, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[AnalysisService] CheckCollisionsCrossDocument: 文档索引 ");
					defaultInterpolatedStringHandler.AppendFormatted(docIndex);
					defaultInterpolatedStringHandler.AppendLiteral(" 超出范围");
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
					continue;
				}
				object obj = documents[docIndex];
				Document doc = (Document)((obj is Document) ? obj : null);
				if (doc == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(68, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("[AnalysisService] CheckCollisionsCrossDocument: 文档索引 ");
					defaultInterpolatedStringHandler2.AppendFormatted(docIndex);
					defaultInterpolatedStringHandler2.AppendLiteral(" 不是 Document 类型");
					Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
					continue;
				}
				foreach (int elementId in elementIds)
				{
					Element element = doc.GetElement(new ElementId((long)elementId));
					if (element != null)
					{
						List<Solid> solids = GetElementSolids(element, opt);
						if (solids.Count > 0)
						{
							BoundingBoxXYZ bbox = element.get_BoundingBox((View)null);
							allGeometries.Add((docIndex, elementId, solids, bbox));
						}
					}
				}
			}
			for (int i = 0; i < allGeometries.Count; i++)
			{
				for (int j = i + 1; j < allGeometries.Count; j++)
				{
					(int DocIndex, int ElementId, List<Solid> Solids, BoundingBoxXYZ BBox) geom1 = allGeometries[i];
					(int DocIndex, int ElementId, List<Solid> Solids, BoundingBoxXYZ BBox) geom2 = allGeometries[j];
					if (!BoundingBoxesIntersect(geom1.BBox, geom2.BBox))
					{
						continue;
					}
					List<(double, double, double)> collisionPoints = new List<(double, double, double)>();
					foreach (Solid solid1 in geom1.Solids)
					{
						foreach (Solid solid2 in geom2.Solids)
						{
							if (CheckSolidsCollision(solid1, solid2, out List<(double X, double Y, double Z)> points))
							{
								collisionPoints.AddRange(points);
							}
							points = null;
						}
					}
					if (collisionPoints.Count > 0)
					{
						collisions.Add((geom1.DocIndex, geom1.ElementId, geom2.DocIndex, geom2.ElementId, collisionPoints));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			Logger.Error("[AnalysisService] CheckCollisionsCrossDocument 失败: " + ex2.Message);
		}
		foreach (var collision in collisions)
		{
			yield return (DocumentIndex1: collision.Item1, ElementId1: collision.Item2, DocumentIndex2: collision.Item3, ElementId2: collision.Item4, CollisionPoints: collision.Item5.Distinct().ToList());
		}
	}

	private List<Solid> GetElementSolids(Element element, Options options)
	{
		List<Solid> list = new List<Solid>();
		try
		{
			GeometryElement val = element.get_Geometry(options);
			if ((GeometryObject)(object)val == (GeometryObject)null)
			{
				return list;
			}
			foreach (GeometryObject item in val)
			{
				Solid val2 = (Solid)(object)((item is Solid) ? item : null);
				if (val2 != null && val2.Volume > 0.0)
				{
					list.Add(val2);
					continue;
				}
				GeometryInstance val3 = (GeometryInstance)(object)((item is GeometryInstance) ? item : null);
				if (val3 != null)
				{
					GeometryElement instanceGeometry = val3.GetInstanceGeometry();
					if (!((GeometryObject)(object)instanceGeometry != (GeometryObject)null))
					{
						continue;
					}
					foreach (GeometryObject item2 in instanceGeometry)
					{
						Solid val4 = (Solid)(object)((item2 is Solid) ? item2 : null);
						if (val4 != null && val4.Volume > 0.0)
						{
							list.Add(val4);
						}
					}
					continue;
				}
				GeometryElement val5 = (GeometryElement)(object)((item is GeometryElement) ? item : null);
				if (val5 == null)
				{
					continue;
				}
				foreach (GeometryObject item3 in val5)
				{
					Solid val6 = (Solid)(object)((item3 is Solid) ? item3 : null);
					if (val6 != null && val6.Volume > 0.0)
					{
						list.Add(val6);
					}
				}
			}
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[AnalysisService] GetElementSolids 失败 (元素ID: ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(element.Id);
			defaultInterpolatedStringHandler.AppendLiteral("): ");
			defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		return list;
	}

	private bool CheckSolidsCollision(Solid solid1, Solid solid2, out List<(double X, double Y, double Z)> collisionPoints)
	{
		collisionPoints = new List<(double, double, double)>();
		try
		{
			Solid val = BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, (BooleanOperationsType)2);
			if ((GeometryObject)(object)val != (GeometryObject)null && val.Volume > 1E-06)
			{
				(double, double, double)? solidCentroid = GetSolidCentroid(val);
				if (solidCentroid.HasValue)
				{
					collisionPoints.Add((solidCentroid.Value.Item1, solidCentroid.Value.Item2, solidCentroid.Value.Item3));
				}
				List<(double, double, double)> collection = SampleSolidPoints(val);
				collisionPoints.AddRange(collection);
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logger.Warning("[AnalysisService] CheckSolidsCollision 失败: " + ex.Message);
			return false;
		}
	}

	private (double X, double Y, double Z)? GetSolidCentroid(Solid solid)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		try
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			foreach (Face face in solid.Faces)
			{
				Face val = face;
				double area = val.Area;
				(double, double, double)? tuple = EvaluateFaceCentroid(val);
				if (tuple.HasValue)
				{
					num2 += tuple.Value.Item1 * area;
					num3 += tuple.Value.Item2 * area;
					num4 += tuple.Value.Item3 * area;
					num += area;
				}
			}
			if (num > 0.0)
			{
				return (num2 / num, num3 / num, num4 / num);
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	private (double X, double Y, double Z)? EvaluateFaceCentroid(Face face)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		try
		{
			BoundingBoxUV boundingBox = face.GetBoundingBox();
			if (boundingBox != null)
			{
				UV val = new UV((boundingBox.Min.U + boundingBox.Max.U) / 2.0, (boundingBox.Min.V + boundingBox.Max.V) / 2.0);
				XYZ val2 = face.Evaluate(val);
				return (val2.X, val2.Y, val2.Z);
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	private List<(double X, double Y, double Z)> SampleSolidPoints(Solid solid, int maxPoints = 5)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		List<(double, double, double)> list = new List<(double, double, double)>();
		try
		{
			BoundingBoxXYZ boundingBox = solid.GetBoundingBox();
			if (boundingBox == null)
			{
				return list;
			}
			double num = (boundingBox.Max.X - boundingBox.Min.X) / (double)(maxPoints + 1);
			double num2 = (boundingBox.Max.Y - boundingBox.Min.Y) / (double)(maxPoints + 1);
			double num3 = (boundingBox.Max.Z - boundingBox.Min.Z) / (double)(maxPoints + 1);
			for (int i = 1; i <= maxPoints; i++)
			{
				for (int j = 1; j <= maxPoints; j++)
				{
					for (int k = 1; k <= maxPoints; k++)
					{
						XYZ val = new XYZ(boundingBox.Min.X + (double)i * num, boundingBox.Min.Y + (double)j * num2, boundingBox.Min.Z + (double)k * num3);
						if (IsPointInSolid(val, solid))
						{
							list.Add((val.X, val.Y, val.Z));
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[AnalysisService] SampleSolidPoints 失败: " + ex.Message);
		}
		return list;
	}

	private bool IsPointInSolid(XYZ point, Solid solid)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		try
		{
			XYZ basisX = XYZ.BasisX;
			Line val = Line.CreateBound(point, point.Add(basisX.Multiply(1000.0)));
			SolidCurveIntersection val2 = solid.IntersectWithCurve((Curve)(object)val, new SolidCurveIntersectionOptions());
			return val2.SegmentCount > 0;
		}
		catch
		{
			return false;
		}
	}

	private bool BoundingBoxesIntersect(BoundingBoxXYZ bbox1, BoundingBoxXYZ bbox2)
	{
		if (bbox1 == null || bbox2 == null)
		{
			return false;
		}
		return !(bbox1.Max.X < bbox2.Min.X) && !(bbox2.Max.X < bbox1.Min.X) && !(bbox1.Max.Y < bbox2.Min.Y) && !(bbox2.Max.Y < bbox1.Min.Y) && !(bbox1.Max.Z < bbox2.Min.Z) && !(bbox2.Max.Z < bbox1.Min.Z);
	}

	private List<(double X, double Y, double Z)> ExtractCollisionPoints(Element element1, Element element2)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		List<(double, double, double)> list = new List<(double, double, double)>();
		try
		{
			Options options = new Options
			{
				DetailLevel = (ViewDetailLevel)3,
				ComputeReferences = false
			};
			List<Solid> elementSolids = GetElementSolids(element1, options);
			List<Solid> elementSolids2 = GetElementSolids(element2, options);
			if (elementSolids.Count == 0 || elementSolids2.Count == 0)
			{
				return list;
			}
			foreach (Solid item in elementSolids)
			{
				foreach (Solid item2 in elementSolids2)
				{
					if (CheckSolidsCollision(item, item2, out List<(double, double, double)> collisionPoints))
					{
						list.AddRange(collisionPoints);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[AnalysisService] ExtractCollisionPoints 失败: " + ex.Message);
		}
		return list;
	}

	private List<(double X, double Y, double Z)> CheckGeometryCollision(Element element1, Element element2)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		List<(double, double, double)> list = new List<(double, double, double)>();
		try
		{
			Options options = new Options
			{
				DetailLevel = (ViewDetailLevel)3,
				ComputeReferences = false
			};
			List<Solid> elementSolids = GetElementSolids(element1, options);
			List<Solid> elementSolids2 = GetElementSolids(element2, options);
			if (elementSolids.Count == 0 || elementSolids2.Count == 0)
			{
				return list;
			}
			BoundingBoxXYZ bbox = element1.get_BoundingBox((View)null);
			BoundingBoxXYZ bbox2 = element2.get_BoundingBox((View)null);
			if (!BoundingBoxesIntersect(bbox, bbox2))
			{
				return list;
			}
			foreach (Solid item in elementSolids)
			{
				foreach (Solid item2 in elementSolids2)
				{
					if (CheckSolidsCollision(item, item2, out List<(double, double, double)> collisionPoints))
					{
						list.AddRange(collisionPoints);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[AnalysisService] CheckGeometryCollision 失败: " + ex.Message);
		}
		return list;
	}

	public IEnumerable<(int GroupId1, int ElementId1, int GroupId2, int ElementId2, IEnumerable<(double X, double Y, double Z)> CollisionPoints)> CheckCollisionsWithGroups(object document, IDictionary<int, IEnumerable<int>> elementsByGroup)
	{
		Document doc = (Document)((document is Document) ? document : null);
		if (doc == null)
		{
			Logger.Error("[AnalysisService] CheckCollisionsWithGroups: document 不是 Document 类型");
			yield break;
		}
		if (elementsByGroup == null || elementsByGroup.Count < 2)
		{
			Logger.Warning("[AnalysisService] CheckCollisionsWithGroups: 至少需要2个组才能进行分组碰撞检测");
			yield break;
		}
		List<(int, int, int, int, List<(double, double, double)>)> collisions = new List<(int, int, int, int, List<(double, double, double)>)>();
		try
		{
			List<int> groupIds = elementsByGroup.Keys.ToList();
			for (int i = 0; i < groupIds.Count; i++)
			{
				for (int j = i + 1; j < groupIds.Count; j++)
				{
					int group1Id = groupIds[i];
					int group2Id = groupIds[j];
					List<ElementId> elementIds1 = elementsByGroup[group1Id].Select((Func<int, ElementId>)delegate(int num)
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0008: Expected O, but got Unknown
						return new ElementId((long)num);
					}).ToList();
					List<ElementId> elementIds2 = elementsByGroup[group2Id].Select((Func<int, ElementId>)delegate(int num)
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0008: Expected O, but got Unknown
						return new ElementId((long)num);
					}).ToList();
					if (elementIds1.Count == 0 || elementIds2.Count == 0)
					{
						continue;
					}
					foreach (ElementId elem1Id in elementIds1)
					{
						Element elem1 = doc.GetElement(elem1Id);
						if (elem1 == null)
						{
							continue;
						}
						FilteredElementCollector collector = new FilteredElementCollector(doc, (ICollection<ElementId>)elementIds2);
						ElementIntersectsElementFilter filter = new ElementIntersectsElementFilter(elem1, false);
						IList<Element> intersectingElements = collector.WherePasses((ElementFilter)(object)filter).ToElements();
						foreach (Element elem2 in intersectingElements)
						{
							int id1 = (int)elem1.Id.Value;
							int id2 = (int)elem2.Id.Value;
							List<(double X, double Y, double Z)> collisionPoints = ExtractCollisionPoints(elem1, elem2);
							collisions.Add((group1Id, id1, group2Id, id2, collisionPoints));
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			Logger.Error("[AnalysisService] CheckCollisionsWithGroups 失败: " + ex2.Message);
		}
		foreach (var collision in collisions)
		{
			yield return (GroupId1: collision.Item1, ElementId1: collision.Item2, GroupId2: collision.Item3, ElementId2: collision.Item4, CollisionPoints: collision.Item5.Distinct().ToList());
		}
	}

	public IDictionary<int, double> CalculateQuantities(object document, IEnumerable<int> elementIds, string quantityType)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CalculateQuantities: document 不是 Document 类型");
				return new Dictionary<int, double>();
			}
			Dictionary<int, double> dictionary = new Dictionary<int, double>();
			foreach (int elementId in elementIds)
			{
				Element element = val.GetElement(new ElementId((long)elementId));
				if (element == null)
				{
					continue;
				}
				double? num = null;
				string text = quantityType.ToLower();
				string text2 = text;
				if (!(text2 == "length") && !(text2 == "长度"))
				{
					if (!(text2 == "area") && !(text2 == "面积"))
					{
						if (!(text2 == "volume") && !(text2 == "体积"))
						{
							LogError("CalculateQuantities: 不支持的数量类型 '" + quantityType + "'");
							continue;
						}
						num = GetElementVolume(element);
					}
					else
					{
						num = GetElementArea(element);
					}
				}
				else
				{
					num = GetElementLength(element);
				}
				if (num.HasValue)
				{
					dictionary[elementId] = num.Value;
				}
			}
			return dictionary;
		}
		catch (Exception ex)
		{
			LogError("CalculateQuantities 失败: " + ex.Message);
			return new Dictionary<int, double>();
		}
	}

	public double? GetElementArea(object document, object element, string areaType = "surface")
	{
		return GetElementArea(element);
	}

	public double? GetElementVolume(object document, object element)
	{
		return GetElementVolume(element);
	}

	private static void LogError(string message)
	{
	}
}
