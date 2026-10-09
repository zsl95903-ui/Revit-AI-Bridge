using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.DB;
using Clipper2Lib;
using Microsoft.CSharp.RuntimeBinder;
using ns0;
using ns1;
using ns6;

using JoinType = Clipper2Lib.JoinType;
namespace RevitAi.Revit.Services;

public sealed class RoadNetworkEngine
{
	private class TopoTrackElement
	{
		public int Index { get; set; }

		public Curve PlanarCurve { get; set; }

		public double Width { get; set; }

		public List<double> Params { get; set; }
	}

	private struct CenterMarkingData
	{
		public Curve Centerline;

		public double StartShrinkFeet;

		public double EndShrinkFeet;

		public int SplitCurveIndex;

		public double OriginalStartParam;

		public double OriginalEndParam;
	}

	[CompilerGenerated]
	public static class _003C_003Eo__94
	{
		public static CallSite<Func<CallSite, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, double, object>> _003C_003Ep__1;

		public static CallSite<Func<CallSite, object, double>> _003C_003Ep__2;

		public static CallSite<Func<CallSite, object, object>> _003C_003Ep__3;

		public static CallSite<Func<CallSite, object, double, object>> _003C_003Ep__4;

		public static CallSite<Func<CallSite, object, double>> _003C_003Ep__5;
	}

	private readonly ILogger _logger;

	private readonly Document _document;

	private const double MillimetersPerFoot = 304.8;

	private const double ClipperScale = 100000.0;

	private const double RevitTolerance = 0.001;

	private const double MinCurveLength = 0.01;

	private double GetClipperArcTolerance()
	{
		return _document.Application.ShortCurveTolerance * 20.0 * 100000.0;
	}

	public RoadNetworkEngine(Document document)
	{
		_document = document;
		_logger = ServiceProvider.GetLogger();
	}

	private Paths64 PolygonsToClipperPaths(List<List<XYZ>> polygons)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Paths64 val = new Paths64();
		foreach (List<XYZ> polygon in polygons)
		{
			Path64 val2 = new Path64();
			foreach (XYZ item in polygon)
			{
				((List<Point64>)(object)val2).Add(new Point64((long)(item.X * 100000.0), (long)(item.Y * 100000.0)));
			}
			val2 = Clipper.StripDuplicates(val2, true);
			if (((List<Point64>)(object)val2).Count >= 3)
			{
				((List<Path64>)(object)val).Add(val2);
			}
		}
		return val;
	}

	private Paths64 ClipperPolyTreeToPaths(PolyTree64 polyTree)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		Paths64 val = new Paths64();
		foreach (PolyPath64 item in (PolyPathBase)polyTree)
		{
			PolyPath64 node = item;
			AddPolyPathRecursive(node, val);
		}
		return val;
	}

	private void AddPolyPathRecursive(PolyPath64 node, Paths64 paths)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		if (node.Polygon != null && ((List<Point64>)(object)node.Polygon).Count >= 3)
		{
			((List<Path64>)(object)paths).Add(node.Polygon);
		}
		foreach (PolyPath64 item in (PolyPathBase)node)
		{
			PolyPath64 node2 = item;
			AddPolyPathRecursive(node2, paths);
		}
	}

	public List<int> GenerateRoadFromWalls(List<Wall> walls, double laneWidthMillimeters, double defaultRoadWidthMillimeters, double sidewalkWidthMillimeters, double curbWidthMillimeters, double intersectionRadiusMeters, ElementId levelId, ElementId floorTypeId, bool createSidewalks = false, ElementId sidewalkFloorTypeId = null, double sidewalkFloorOffsetMillimeters = 100.0, bool createCurbs = false, ElementId curbWallTypeId = null, double curbHeightMillimeters = 200.0, double curbBaseOffsetMillimeters = -100.0, bool createCenterMarkings = false, double centerMarkingWidthMillimeters = 150.0, double centerMarkingThicknessMillimeters = 10.0, ElementId centerMarkingFloorTypeId = null, bool isCenterMarkingDoubleLine = false, bool createCrosswalks = false, double crosswalkLineWidthMillimeters = 450.0, double crosswalkSpacingMillimeters = 1050.0, double crosswalkLengthMeters = 5.0, double crosswalkDistanceFromMarkingMeters = 1.0, ElementId crosswalkFloorTypeId = null, bool createLaneMarkings = false, double laneMarkingWidthMillimeters = 150.0, double laneMarkingSolidLengthMeters = 3.0, double laneMarkingGapLengthMeters = 3.0, ElementId laneMarkingFloorTypeId = null, bool createDirectionArrows = false, double arrowSpacingMeters = 20.0, double arrowSizeMeters = 6.0, ElementId arrowFloorTypeId = null, bool createSiteFloor = false, double siteFloorExtensionMeters = 20.0, ElementId siteFloorTypeId = null, double roadFloorOffsetMillimeters = 0.0, double siteFloorOffsetMillimeters = 0.0)
	{
		try
		{
			List<CurveWithWidth> list = ExtractWallCenterlines(walls, defaultRoadWidthMillimeters);
			if (list.Count == 0)
			{
				return new List<int>();
			}
			List<CurveWithWidth> splitCurves = SplitAllCurvesAtIntersections(list);
			List<RouteChain> list2 = BuildRouteChains(splitCurves);
			List<List<XYZ>> list3 = new List<List<XYZ>>();
			foreach (RouteChain item4 in list2)
			{
				List<XYZ> list4 = GenerateRoadPolygonWithClipper(item4);
				if (list4 != null && list4.Count >= 3)
				{
					list3.Add(list4);
				}
			}
			double num = intersectionRadiusMeters / 0.3048;
			List<List<XYZ>> list5 = ((list3.Count <= 0) ? list3 : PerformGlobalBooleanUnion(list3, num));
			List<int> list6 = new List<int>();
			List<Floor> list7 = CreateFloorFromPolyTreeWithOffset(list5, floorTypeId, levelId, roadFloorOffsetMillimeters);
			if (list7 != null && list7.Count > 0)
			{
				foreach (Floor item5 in list7)
				{
					list6.Add(((Element)item5).Id.smethod_0());
				}
			}
			List<List<XYZ>> list8 = null;
			List<List<XYZ>> sidewalkPolygons = null;
			List<List<XYZ>> list9 = null;
			if (createSidewalks && sidewalkWidthMillimeters > 0.0)
			{
				try
				{
					(List<List<XYZ>> sidewalkPolygons, List<List<XYZ>> curbPolygons, List<List<XYZ>> mergedSidewalkPolygons) tuple = GenerateSidewalkAndCurbPolygons(list2, sidewalkWidthMillimeters, curbWidthMillimeters, num, list5);
					List<List<XYZ>> item = tuple.sidewalkPolygons;
					List<List<XYZ>> item2 = tuple.curbPolygons;
					List<List<XYZ>> item3 = tuple.mergedSidewalkPolygons;
					list8 = item;
					sidewalkPolygons = item2;
					list9 = item3;
					if (list8 != null && list8.Count > 0)
					{
						List<Floor> list10 = CreateFloorsFromPolygonsWithOffset(list8, (sidewalkFloorTypeId != (ElementId)null) ? sidewalkFloorTypeId : floorTypeId, levelId, sidewalkFloorOffsetMillimeters);
						if (list10 != null && list10.Count > 0)
						{
							foreach (Floor item6 in list10)
							{
								list6.Add(((Element)item6).Id.smethod_0());
							}
						}
					}
				}
				catch (Exception ex)
				{
					_logger.Error("[RoadNetworkEngine] 人行道生成失败", ex);
				}
			}
			if (createCurbs && curbWidthMillimeters > 0.0 && curbWallTypeId != (ElementId)null)
			{
				try
				{
					List<int> list11 = GenerateCurbWalls(sidewalkPolygons, curbWallTypeId, levelId, curbHeightMillimeters, curbBaseOffsetMillimeters);
					if (list11 != null && list11.Count > 0)
					{
						list6.AddRange(list11);
					}
				}
				catch (Exception ex2)
				{
					_logger.Error("[RoadNetworkEngine] 路沿石墙生成失败", ex2);
				}
			}
			List<CenterMarkingData> list12 = null;
			if ((createCenterMarkings && !(centerMarkingWidthMillimeters <= 0.0)) || (createCrosswalks && crosswalkLineWidthMillimeters > 0.0))
			{
				list12 = GenerateCenterMarkingCenterlines(splitCurves, intersectionRadiusMeters);
			}
			if (createCenterMarkings && centerMarkingWidthMillimeters > 0.0 && list12 != null && list12.Count > 0)
			{
				try
				{
					List<List<XYZ>> list13 = new List<List<XYZ>>();
					if (isCenterMarkingDoubleLine)
					{
						double num2 = centerMarkingWidthMillimeters / 2.0 + 100.0;
						double offsetFeet = num2 / 304.8;
						foreach (CenterMarkingData item7 in list12)
						{
							Curve centerline = item7.Centerline;
							Curve val = OffsetCurve(centerline, offsetFeet, leftSide: true);
							if ((GeometryObject)(object)val != (GeometryObject)null)
							{
								List<XYZ> list14 = GenerateMarkingPolygonFromCurve(val, centerMarkingWidthMillimeters);
								if (list14 != null && list14.Count >= 3)
								{
									list13.Add(list14);
								}
							}
							Curve val2 = OffsetCurve(centerline, offsetFeet, leftSide: false);
							if ((GeometryObject)(object)val2 != (GeometryObject)null)
							{
								List<XYZ> list15 = GenerateMarkingPolygonFromCurve(val2, centerMarkingWidthMillimeters);
								if (list15 != null && list15.Count >= 3)
								{
									list13.Add(list15);
								}
							}
						}
					}
					else
					{
						foreach (CenterMarkingData item8 in list12)
						{
							List<XYZ> list16 = GenerateMarkingPolygonFromCurve(item8.Centerline, centerMarkingWidthMillimeters);
							if (list16 != null && list16.Count >= 3)
							{
								list13.Add(list16);
							}
						}
					}
					if (list13.Count > 0)
					{
						ElementId floorTypeId2 = ((centerMarkingFloorTypeId != (ElementId)null) ? centerMarkingFloorTypeId : floorTypeId);
						foreach (List<XYZ> item9 in list13)
						{
							Floor val3 = CreateSingleFloorWithOffset(item9, floorTypeId2, levelId, centerMarkingThicknessMillimeters);
							if (val3 != null)
							{
								list6.Add(((Element)val3).Id.smethod_0());
							}
						}
					}
				}
				catch (Exception ex3)
				{
					_logger.Error("[RoadNetworkEngine] 路中心标线生成失败", ex3);
				}
			}
			if (createCrosswalks && crosswalkLineWidthMillimeters > 0.0 && list12 != null && list12.Count > 0)
			{
				try
				{
					List<List<XYZ>> list17 = GenerateCrosswalkPolygons(splitCurves, list12, crosswalkLineWidthMillimeters, crosswalkSpacingMillimeters, crosswalkLengthMeters, crosswalkDistanceFromMarkingMeters);
					if (list17 != null && list17.Count > 0)
					{
						ElementId floorTypeId3 = ((crosswalkFloorTypeId != (ElementId)null) ? crosswalkFloorTypeId : floorTypeId);
						foreach (List<XYZ> item10 in list17)
						{
							Floor val4 = CreateSingleFloorWithOffset(item10, floorTypeId3, levelId, centerMarkingThicknessMillimeters);
							if (val4 != null)
							{
								list6.Add(((Element)val4).Id.smethod_0());
							}
						}
					}
				}
				catch (Exception ex4)
				{
					_logger.Error("[RoadNetworkEngine] 人行横道生成失败", ex4);
				}
			}
			if (createCenterMarkings && list12 != null && list12.Count > 0)
			{
				try
				{
					double stopLineWidthMillimeters = 150.0;
					List<List<XYZ>> list18 = GenerateStopLinePolygons(splitCurves, list12, stopLineWidthMillimeters, centerMarkingWidthMillimeters, isCenterMarkingDoubleLine);
					if (list18 != null && list18.Count > 0)
					{
						ElementId floorTypeId4 = ((laneMarkingFloorTypeId != (ElementId)null) ? laneMarkingFloorTypeId : floorTypeId);
						foreach (List<XYZ> item11 in list18)
						{
							Floor val5 = CreateSingleFloorWithOffset(item11, floorTypeId4, levelId, centerMarkingThicknessMillimeters);
							if (val5 != null)
							{
								list6.Add(((Element)val5).Id.smethod_0());
							}
						}
					}
				}
				catch (Exception ex5)
				{
					_logger.Error("[RoadNetworkEngine] 停止线生成失败", ex5);
				}
			}
			if (createLaneMarkings && list12 != null && list12.Count > 0)
			{
				try
				{
					List<List<XYZ>> list19 = GenerateLaneMarkingPolygons(splitCurves, list12, laneWidthMillimeters, laneMarkingWidthMillimeters, laneMarkingSolidLengthMeters, laneMarkingGapLengthMeters);
					if (list19 != null && list19.Count > 0)
					{
						ElementId floorTypeId5 = ((laneMarkingFloorTypeId != (ElementId)null) ? laneMarkingFloorTypeId : floorTypeId);
						foreach (List<XYZ> item12 in list19)
						{
							Floor val6 = CreateSingleFloorWithOffset(item12, floorTypeId5, levelId, centerMarkingThicknessMillimeters);
							if (val6 != null)
							{
								list6.Add(((Element)val6).Id.smethod_0());
							}
						}
					}
				}
				catch (Exception ex6)
				{
					_logger.Error("[RoadNetworkEngine] 车道分界线生成失败", ex6);
				}
			}
			if (createDirectionArrows && list12 != null && list12.Count > 0)
			{
				try
				{
					List<List<XYZ>> list20 = GenerateDirectionArrowPolygons(splitCurves, list12, laneWidthMillimeters, arrowSpacingMeters, arrowSizeMeters);
					if (list20 != null && list20.Count > 0)
					{
						ElementId floorTypeId6 = ((arrowFloorTypeId != (ElementId)null) ? arrowFloorTypeId : floorTypeId);
						foreach (List<XYZ> item13 in list20)
						{
							Floor val7 = CreateSingleFloorWithOffset(item13, floorTypeId6, levelId, centerMarkingThicknessMillimeters);
							if (val7 != null)
							{
								list6.Add(((Element)val7).Id.smethod_0());
							}
						}
					}
				}
				catch (Exception ex7)
				{
					_logger.Error("[RoadNetworkEngine] 指向箭头生成失败", ex7);
				}
			}
			if (createSiteFloor && siteFloorExtensionMeters > 0.0)
			{
				try
				{
					List<List<XYZ>> list21 = list9;
					if (list21 == null || list21.Count == 0)
					{
						list21 = new List<List<XYZ>>();
						foreach (RouteChain item14 in list2)
						{
							List<XYZ> list22 = GenerateRoadPolygonWithClipper(item14, sidewalkWidthMillimeters);
							if (list22 != null && list22.Count >= 3)
							{
								list21.Add(list22);
							}
						}
						if (list21.Count > 0)
						{
							list21 = PerformGlobalBooleanUnion(list21, num);
						}
					}
					if (list21 != null && list21.Count > 0)
					{
						List<Floor> list23 = CreateSiteFloor(list21, siteFloorExtensionMeters, (siteFloorTypeId != (ElementId)null) ? siteFloorTypeId : floorTypeId, levelId, siteFloorOffsetMillimeters);
						if (list23 != null && list23.Count > 0)
						{
							foreach (Floor item15 in list23)
							{
								list6.Add(((Element)item15).Id.smethod_0());
							}
							ILogger logger = _logger;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
							defaultInterpolatedStringHandler.AppendLiteral("[RoadNetworkEngine] 成功创建 ");
							defaultInterpolatedStringHandler.AppendFormatted(list23.Count);
							defaultInterpolatedStringHandler.AppendLiteral(" 个场地楼板");
							logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						}
					}
				}
				catch (Exception ex8)
				{
					_logger.Error("[RoadNetworkEngine] 场地楼板生成失败", ex8);
				}
			}
			return list6;
		}
		catch (Exception ex9)
		{
			_logger.Error("[RoadNetworkEngine] 路网生成失败", ex9);
			return new List<int>();
		}
	}

	private List<CurveWithWidth> SplitAllCurvesAtIntersections(List<CurveWithWidth> rawRouteData)
	{
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Invalid comparison between Unknown and I4
		try
		{
			if (rawRouteData == null || rawRouteData.Count == 0)
			{
				return new List<CurveWithWidth>();
			}
			List<TopoTrackElement> list = new List<TopoTrackElement>();
			for (int i = 0; i < rawRouteData.Count; i++)
			{
				CurveWithWidth curveWithWidth = rawRouteData[i];
				Curve val = FlattenCurveToXYPlane(curveWithWidth.Curve);
				if ((GeometryObject)(object)val != (GeometryObject)null)
				{
					list.Add(new TopoTrackElement
					{
						Index = i,
						PlanarCurve = val,
						Width = curveWithWidth.Width,
						Params = new List<double>
						{
							val.GetEndParameter(0),
							val.GetEndParameter(1)
						}
					});
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				TopoTrackElement topoTrackElement = list[j];
				for (int k = j + 1; k < list.Count; k++)
				{
					TopoTrackElement topoTrackElement2 = list[k];
					CurveIntersectResult val2 = topoTrackElement.PlanarCurve.Intersect(topoTrackElement2.PlanarCurve, (CurveIntersectResultOption)1);
					if (val2 == null || (int)val2.Result == 4)
					{
						continue;
					}
					IList<CurveOverlapPoint> overlaps = val2.GetOverlaps();
					if (overlaps == null)
					{
						continue;
					}
					foreach (CurveOverlapPoint item in overlaps)
					{
						XYZ point = item.Point;
						IntersectionResult val3 = topoTrackElement.PlanarCurve.Project(point);
						IntersectionResult val4 = topoTrackElement2.PlanarCurve.Project(point);
						if (val3 != null && IsParamInBounds(topoTrackElement.PlanarCurve, val3.Parameter))
						{
							topoTrackElement.Params.Add(val3.Parameter);
						}
						if (val4 != null && IsParamInBounds(topoTrackElement2.PlanarCurve, val4.Parameter))
						{
							topoTrackElement2.Params.Add(val4.Parameter);
						}
					}
				}
			}
			List<CurveWithWidth> list2 = new List<CurveWithWidth>();
			foreach (TopoTrackElement item2 in list)
			{
				List<double> list3 = (from p in item2.Params.Distinct()
					orderby p
					select p).ToList();
				for (int num = 0; num < list3.Count - 1; num++)
				{
					double num2 = list3[num];
					double num3 = list3[num + 1];
					if (num2 >= num3 || num3 - num2 < 0.001)
					{
						continue;
					}
					try
					{
						Curve val5 = item2.PlanarCurve.Clone();
						val5.MakeBound(num2, num3);
						if (val5.Length > 0.01 && !IsDuplicateWithWidth(val5, item2.Width, list2))
						{
							list2.Add(new CurveWithWidth
							{
								Curve = val5,
								Width = item2.Width
							});
						}
					}
					catch (Exception)
					{
						XYZ val6 = item2.PlanarCurve.Evaluate(num2, false);
						XYZ val7 = item2.PlanarCurve.Evaluate(num3, false);
						if (val6.DistanceTo(val7) > 0.01)
						{
							Line curve = Line.CreateBound(val6, val7);
							if (!IsDuplicateWithWidth((Curve)(object)curve, item2.Width, list2))
							{
								list2.Add(new CurveWithWidth
								{
									Curve = (Curve)(object)curve,
									Width = item2.Width
								});
							}
						}
					}
				}
			}
			return list2;
		}
		catch (Exception ex2)
		{
			_logger.Error("[RoadNetworkEngine] 严重拓扑打断异常", ex2);
			return new List<CurveWithWidth>();
		}
	}

	private bool IsParamInBounds(Curve curve, double param, double tolerance = 0.0001)
	{
		double endParameter = curve.GetEndParameter(0);
		double endParameter2 = curve.GetEndParameter(1);
		double num = Math.Min(endParameter, endParameter2);
		double num2 = Math.Max(endParameter, endParameter2);
		return param >= num - tolerance && param <= num2 + tolerance;
	}

	private bool IsDuplicateWithWidth(Curve curve, double width, List<CurveWithWidth> existing)
	{
		XYZ endPoint = curve.GetEndPoint(0);
		XYZ endPoint2 = curve.GetEndPoint(1);
		foreach (CurveWithWidth item in existing)
		{
			if (!(Math.Abs(item.Width - width) > 0.001))
			{
				XYZ endPoint3 = item.Curve.GetEndPoint(0);
				XYZ endPoint4 = item.Curve.GetEndPoint(1);
				if ((endPoint.IsAlmostEqualTo(endPoint3, 0.001) && endPoint2.IsAlmostEqualTo(endPoint4, 0.001)) || (endPoint.IsAlmostEqualTo(endPoint4, 0.001) && endPoint2.IsAlmostEqualTo(endPoint3, 0.001)))
				{
					return true;
				}
			}
		}
		return false;
	}

	private Curve FlattenCurveToXYPlane(Curve curve)
	{
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Expected O, but got Unknown
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Expected O, but got Unknown
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Expected O, but got Unknown
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Expected O, but got Unknown
		if ((GeometryObject)(object)curve == (GeometryObject)null)
		{
			return null;
		}
		Line val = (Line)(object)((curve is Line) ? curve : null);
		if (val != null)
		{
			XYZ endPoint = ((Curve)val).GetEndPoint(0);
			XYZ endPoint2 = ((Curve)val).GetEndPoint(1);
			XYZ val2 = new XYZ(endPoint.X, endPoint.Y, 0.0);
			XYZ val3 = new XYZ(endPoint2.X, endPoint2.Y, 0.0);
			if (val2.DistanceTo(val3) < 0.001)
			{
				return null;
			}
			return (Curve)(object)Line.CreateBound(val2, val3);
		}
		Arc val4 = (Arc)(object)((curve is Arc) ? curve : null);
		if (val4 != null)
		{
			try
			{
				double radius = val4.Radius;
				XYZ val5 = new XYZ(val4.Center.X, val4.Center.Y, 0.0);
				XYZ val6 = new XYZ(((Curve)val4).GetEndPoint(0).X, ((Curve)val4).GetEndPoint(0).Y, 0.0);
				XYZ val7 = new XYZ(((Curve)val4).GetEndPoint(1).X, ((Curve)val4).GetEndPoint(1).Y, 0.0);
				if (val6.DistanceTo(val5) < 0.001 || val7.DistanceTo(val5) < 0.001)
				{
					return (Curve)(object)((val6.DistanceTo(val7) > 0.01) ? Line.CreateBound(val6, val7) : null);
				}
				XYZ val8 = (val6 - val5).Normalize();
				XYZ val9 = (val7 - val5).Normalize();
				double num = val8.AngleTo(val9);
				if (num < 0.001)
				{
					num = 0.0;
				}
				bool flag = val4.Normal.Z >= 0.0;
				XYZ val10 = val8;
				XYZ val11 = ((!flag) ? new XYZ(val8.Y, 0.0 - val8.X, 0.0).Normalize() : new XYZ(0.0 - val8.Y, val8.X, 0.0).Normalize());
				XYZ val12 = ((Curve)val4).Evaluate(0.5 * (((Curve)val4).GetEndParameter(0) + ((Curve)val4).GetEndParameter(1)), false);
				XYZ val13 = new XYZ(val12.X, val12.Y, 0.0);
				Arc val14 = Arc.Create(val5, radius, 0.0, num, val10, val11);
				XYZ val15 = ((Curve)val14).Evaluate(0.5 * num, false);
				if (val15.DistanceTo(val13) > 0.01)
				{
					double num2 = Math.PI * 2.0 - num;
					val14 = Arc.Create(val5, radius, 0.0, num2, val10, val11);
				}
				if (((Curve)val14).GetEndPoint(1).IsAlmostEqualTo(val7, 0.005))
				{
					return (Curve)(object)val14;
				}
			}
			catch (Exception ex)
			{
				_logger.Error("[RoadNetworkEngine] 圆弧平面解析重建失败: " + ex.Message + "，启动安全退化机制", (Exception)null);
			}
			XYZ val16 = new XYZ(curve.GetEndPoint(0).X, curve.GetEndPoint(0).Y, 0.0);
			XYZ val17 = new XYZ(curve.GetEndPoint(1).X, curve.GetEndPoint(1).Y, 0.0);
			return (Curve)(object)((val16.DistanceTo(val17) > 0.01) ? Line.CreateBound(val16, val17) : null);
		}
		try
		{
			IList<XYZ> list = curve.Tessellate();
			if (list != null && list.Count >= 2)
			{
				XYZ val18 = new XYZ(list[0].X, list[0].Y, 0.0);
				XYZ val19 = new XYZ(list[list.Count - 1].X, list[list.Count - 1].Y, 0.0);
				if (val18.DistanceTo(val19) > 0.01)
				{
					return (Curve)(object)Line.CreateBound(val18, val19);
				}
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	private List<RouteChain> BuildRouteChains(List<CurveWithWidth> splitCurves)
	{
		try
		{
			List<RouteChain> list = new List<RouteChain>();
			int count = splitCurves.Count;
			for (int i = 0; i < count; i++)
			{
				CurveWithWidth curveWithWidth = splitCurves[i];
				XYZ endPoint = curveWithWidth.Curve.GetEndPoint(0);
				XYZ endPoint2 = curveWithWidth.Curve.GetEndPoint(1);
				List<CurveWithWidth> list2 = FindConnectedCurvesAtPoint(splitCurves, i, endPoint);
				List<CurveWithWidth> list3 = FindConnectedCurvesAtPoint(splitCurves, i, endPoint2);
				foreach (CurveWithWidth item in list2)
				{
					RouteChain routeChain = CreateRouteChain(curveWithWidth, item, endPoint);
					if (routeChain != null && !IsChainDuplicate(list, routeChain))
					{
						list.Add(routeChain);
					}
				}
				foreach (CurveWithWidth item2 in list3)
				{
					RouteChain routeChain2 = CreateRouteChain(curveWithWidth, item2, endPoint2);
					if (routeChain2 != null && !IsChainDuplicate(list, routeChain2))
					{
						list.Add(routeChain2);
					}
				}
			}
			HashSet<CurveWithWidth> hashSet = new HashSet<CurveWithWidth>();
			foreach (RouteChain item3 in list)
			{
				foreach (Curve curve in item3.Curves)
				{
					CurveWithWidth curveWithWidth2 = splitCurves.FirstOrDefault((CurveWithWidth c) => c.Curve.GetEndPoint(0).IsAlmostEqualTo(curve.GetEndPoint(0)) && c.Curve.GetEndPoint(1).IsAlmostEqualTo(curve.GetEndPoint(1)));
					if (curveWithWidth2 != null)
					{
						hashSet.Add(curveWithWidth2);
					}
				}
			}
			foreach (CurveWithWidth splitCurf in splitCurves)
			{
				if (!hashSet.Contains(splitCurf))
				{
					list.Add(new RouteChain
					{
						Curves = new List<Curve> { splitCurf.Curve },
						Width = splitCurf.Width,
						CurveWidths = new List<double> { splitCurf.Width }
					});
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] 串线链处理失败", ex);
			return splitCurves.Select((CurveWithWidth c) => new RouteChain
			{
				Curves = new List<Curve> { c.Curve },
				Width = c.Width,
				CurveWidths = new List<double> { c.Width }
			}).ToList();
		}
	}

	private List<CurveWithWidth> FindConnectedCurvesAtPoint(List<CurveWithWidth> curves, int excludeIndex, XYZ point)
	{
		List<CurveWithWidth> list = new List<CurveWithWidth>();
		double num = 0.01;
		for (int i = 0; i < curves.Count; i++)
		{
			if (i != excludeIndex)
			{
				CurveWithWidth curveWithWidth = curves[i];
				XYZ endPoint = curveWithWidth.Curve.GetEndPoint(0);
				XYZ endPoint2 = curveWithWidth.Curve.GetEndPoint(1);
				if (endPoint.DistanceTo(point) < num || endPoint2.DistanceTo(point) < num)
				{
					list.Add(curveWithWidth);
				}
			}
		}
		return list;
	}

	private RouteChain CreateRouteChain(CurveWithWidth curve1, CurveWithWidth curve2, XYZ connectionPoint)
	{
		XYZ endPoint = curve1.Curve.GetEndPoint(0);
		XYZ endPoint2 = curve1.Curve.GetEndPoint(1);
		XYZ endPoint3 = curve2.Curve.GetEndPoint(0);
		XYZ endPoint4 = curve2.Curve.GetEndPoint(1);
		double num = 0.01;
		double width = Math.Min(curve1.Width, curve2.Width);
		bool flag = endPoint.DistanceTo(connectionPoint) < num;
		bool flag2 = endPoint2.DistanceTo(connectionPoint) < num;
		bool flag3 = endPoint3.DistanceTo(connectionPoint) < num;
		bool flag4 = endPoint4.DistanceTo(connectionPoint) < num;
		Curve item;
		Curve item2;
		double width2;
		double width3;
		if (flag2 & flag3)
		{
			item = curve1.Curve;
			item2 = curve2.Curve;
			width2 = curve1.Width;
			width3 = curve2.Width;
		}
		else if (flag2 & flag4)
		{
			item = curve1.Curve;
			item2 = curve2.Curve.CreateReversed();
			width2 = curve1.Width;
			width3 = curve2.Width;
		}
		else if (flag & flag3)
		{
			item = curve1.Curve.CreateReversed();
			item2 = curve2.Curve;
			width2 = curve1.Width;
			width3 = curve2.Width;
		}
		else if (flag & flag4)
		{
			item = curve2.Curve;
			item2 = curve1.Curve;
			width2 = curve2.Width;
			width3 = curve1.Width;
		}
		else
		{
			item = curve1.Curve;
			item2 = curve2.Curve;
			width2 = curve1.Width;
			width3 = curve2.Width;
		}
		return new RouteChain
		{
			Curves = new List<Curve> { item, item2 },
			Width = width,
			CurveWidths = new List<double> { width2, width3 }
		};
	}

	private bool IsChainDuplicate(List<RouteChain> chains, RouteChain newChain)
	{
		if (newChain.Curves.Count != 2)
		{
			return false;
		}
		foreach (RouteChain chain in chains)
		{
			if (chain.Curves.Count == 2)
			{
				bool flag = CurvesAreSame(chain.Curves[0], newChain.Curves[0]) && CurvesAreSame(chain.Curves[1], newChain.Curves[1]);
				bool flag2 = CurvesAreSame(chain.Curves[0], newChain.Curves[1]) && CurvesAreSame(chain.Curves[1], newChain.Curves[0]);
				if (flag | flag2)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool CurvesAreSame(Curve c1, Curve c2)
	{
		XYZ endPoint = c1.GetEndPoint(0);
		XYZ endPoint2 = c1.GetEndPoint(1);
		XYZ endPoint3 = c2.GetEndPoint(0);
		XYZ endPoint4 = c2.GetEndPoint(1);
		bool flag = endPoint.IsAlmostEqualTo(endPoint3) && endPoint2.IsAlmostEqualTo(endPoint4);
		bool flag2 = endPoint.IsAlmostEqualTo(endPoint4) && endPoint2.IsAlmostEqualTo(endPoint3);
		return flag | flag2;
	}

	private bool DoCurvesIntersect(Curve c1, Curve c2)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		try
		{
			try
			{
				CurveIntersectResult val = c1.Intersect(c2, (CurveIntersectResultOption)0);
				return val != null && (int)val.Result != 4;
			}
			catch
			{
				return false;
			}
		}
		catch
		{
			return false;
		}
	}

	private List<XYZ> GenerateRoadPolygonWithClipper(RouteChain chain, double? widthOverride = null)
	{
		try
		{
			if (chain?.Curves == null || chain.Curves.Count == 0)
			{
				return null;
			}
			if (widthOverride.HasValue)
			{
				List<double> list = new List<double>();
				double num = double.MaxValue;
				for (int i = 0; i < chain.Curves.Count; i++)
				{
					double num2 = ((chain.CurveWidths.Count > i) ? chain.CurveWidths[i] : chain.Width);
					double num3 = num2 + widthOverride.Value * 2.0;
					list.Add(num3);
					if (num3 < num)
					{
						num = num3;
					}
				}
				RouteChain chain2 = new RouteChain
				{
					Curves = chain.Curves,
					Width = num,
					CurveWidths = list
				};
				if (chain.Curves.Count == 1)
				{
					return GenerateSingleCurvePolygon(chain2);
				}
				return GenerateTwoCurvePolygon(chain2);
			}
			if (chain.Curves.Count == 1)
			{
				return GenerateSingleCurvePolygon(chain);
			}
			return GenerateTwoCurvePolygon(chain);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] 生成道路多边形失败", ex);
			return null;
		}
	}

	private List<XYZ> GenerateSingleCurvePolygon(RouteChain chain, double? widthOverride = null)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0064: Expected O, but got Unknown
		try
		{
			Curve curve = chain.Curves[0];
			Path64 item = CurveToClipperPath(curve);
			double num = widthOverride ?? chain.Width;
			double num2 = num / 304.8;
			double num3 = num2 / 2.0 * 100000.0;
			Paths64 val = new Paths64();
			((List<Path64>)val).Add(item);
			Paths64 val2 = val;
			Paths64 val3 = Clipper.InflatePaths(val2, num3, (JoinType)0, (EndType)2, 5.0, GetClipperArcTolerance());
			if (val3 == null || ((List<Path64>)(object)val3).Count == 0)
			{
				return null;
			}
			return ClipperPathToXYZ(((List<Path64>)(object)val3)[0]);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private List<XYZ> GenerateTwoCurvePolygon(RouteChain chain)
	{
		try
		{
			Curve c = chain.Curves[0];
			Curve c2 = chain.Curves[1];
			double num = ((chain.CurveWidths.Count > 0) ? chain.CurveWidths[0] : chain.Width);
			double num2 = ((chain.CurveWidths.Count > 1) ? chain.CurveWidths[1] : chain.Width);
			if (Math.Abs(num - num2) < 0.01)
			{
				return GenerateMultiCurvePolygon(chain);
			}
			double num3 = Math.Min(num, num2);
			double num4 = Math.Max(num, num2);
			double widthDiff = num4 - num3;
			return GenerateDifferentWidthTwoCurvePolygon(c, c2, num, num2, num3, widthDiff);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] 两条曲线生成多边形失败", ex);
			return null;
		}
	}

	private List<XYZ> GenerateDifferentWidthTwoCurvePolygon(Curve c1, Curve c2, double w1, double w2, double minWidth, double widthDiff)
	{
		try
		{
			Curve val = FlattenCurveZ(c1);
			Curve val2 = FlattenCurveZ(c2);
			if ((GeometryObject)(object)val == (GeometryObject)null || (GeometryObject)(object)val2 == (GeometryObject)null)
			{
				return null;
			}
			Curve curve;
			Curve curve2;
			if (w1 > w2)
			{
				curve = val;
				curve2 = val2;
			}
			else
			{
				curve = val2;
				curve2 = val;
			}
			double offsetFeet = widthDiff / 2.0 / 304.8;
			Curve val3 = OffsetCurve(curve, offsetFeet, leftSide: true);
			Curve val4 = OffsetCurve(curve, offsetFeet, leftSide: false);
			if ((GeometryObject)(object)val3 == (GeometryObject)null || (GeometryObject)(object)val4 == (GeometryObject)null)
			{
				return null;
			}
			List<Curve> list = ExtendAndConnect(val3, curve2, minWidth);
			List<Curve> list2 = ExtendAndConnect(val4, curve2, minWidth);
			if (list == null || list2 == null)
			{
				return null;
			}
			List<XYZ> list3 = GenerateMultiCurvePolygon(new RouteChain
			{
				Curves = list,
				Width = minWidth,
				CurveWidths = new List<double> { minWidth, minWidth }
			});
			List<XYZ> list4 = GenerateMultiCurvePolygon(new RouteChain
			{
				Curves = list2,
				Width = minWidth,
				CurveWidths = new List<double> { minWidth, minWidth }
			});
			if (list3 == null || list4 == null)
			{
				return list3 ?? list4;
			}
			return UnionPolygons(new List<List<XYZ>> { list3, list4 });
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] 不同宽度双曲线生成失败", ex);
			return null;
		}
	}

	private List<Curve> ExtendAndConnect(Curve curve1, Curve curve2, double routeWidthMm)
	{
		try
		{
			XYZ endPoint = curve1.GetEndPoint(0);
			XYZ endPoint2 = curve1.GetEndPoint(1);
			XYZ endPoint3 = curve2.GetEndPoint(0);
			XYZ endPoint4 = curve2.GetEndPoint(1);
			double num = endPoint.DistanceTo(endPoint3);
			double num2 = endPoint.DistanceTo(endPoint4);
			double num3 = endPoint2.DistanceTo(endPoint3);
			double val = endPoint2.DistanceTo(endPoint4);
			double num4 = Math.Min(Math.Min(num, num2), Math.Min(num3, val));
			bool flag;
			bool flag2;
			if (num4 == num)
			{
				flag = false;
				flag2 = false;
			}
			else if (num4 == num2)
			{
				flag = false;
				flag2 = true;
			}
			else if (num4 == num3)
			{
				flag = true;
				flag2 = false;
			}
			else
			{
				flag = true;
				flag2 = true;
			}
			List<XYZ> curveIntersections = GetCurveIntersections(curve1, curve2);
			if (curveIntersections.Count >= 1)
			{
				XYZ val2 = curveIntersections[0];
				Curve val3 = TrimCurveToIntersection(curve1, null, val2, flag);
				Curve val4 = TrimCurveToIntersection(curve2, null, val2, flag2);
				if ((GeometryObject)(object)val3 != (GeometryObject)null && (GeometryObject)(object)val4 != (GeometryObject)null)
				{
					val3 = SafeReverseCurve(val3, val2, 1);
					val4 = SafeReverseCurve(val4, val2, 2);
					return new List<Curve> { val3, val4 };
				}
				return new List<Curve> { curve1, curve2 };
			}
			double num5 = num4;
			double extendFeet = num5 * 1.5;
			List<XYZ> list = null;
			Curve val5 = null;
			Curve val6 = null;
			val5 = ExtendCurveOneEnd(curve1, extendFeet, flag);
			val6 = ExtendCurveOneEnd(curve2, extendFeet, flag2);
			if ((GeometryObject)(object)val5 == (GeometryObject)null || (GeometryObject)(object)val6 == (GeometryObject)null)
			{
				extendFeet = num5 * 3.0;
				val5 = ExtendCurveOneEnd(curve1, extendFeet, flag);
				val6 = ExtendCurveOneEnd(curve2, extendFeet, flag2);
			}
			if ((GeometryObject)(object)val5 != (GeometryObject)null && (GeometryObject)(object)val6 != (GeometryObject)null)
			{
				list = GetCurveIntersections(val5, val6);
				if (list.Count > 0)
				{
				}
			}
			if (list == null || list.Count == 0)
			{
				return null;
			}
			if ((GeometryObject)(object)val5 == (GeometryObject)null || (GeometryObject)(object)val6 == (GeometryObject)null)
			{
				return null;
			}
			XYZ val7 = list[0];
			Curve val8 = TrimCurveToIntersection(curve1, val5, val7, flag);
			Curve val9 = TrimCurveToIntersection(curve2, val6, val7, flag2);
			if ((GeometryObject)(object)val8 == (GeometryObject)null || (GeometryObject)(object)val9 == (GeometryObject)null)
			{
				return null;
			}
			val8 = SafeReverseCurve(val8, val7, 1);
			val9 = SafeReverseCurve(val9, val7, 2);
			return new List<Curve> { val8, val9 };
		}
		catch (Exception)
		{
			return null;
		}
	}

	private Curve ExtendCurveOneEnd(Curve curve, double extendFeet, bool extendEnd)
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		if ((GeometryObject)(object)curve == (GeometryObject)null || extendFeet <= 0.0)
		{
			return curve;
		}
		try
		{
			Line val = (Line)(object)((curve is Line) ? curve : null);
			if (val != null)
			{
				XYZ endPoint = ((Curve)val).GetEndPoint(0);
				XYZ endPoint2 = ((Curve)val).GetEndPoint(1);
				XYZ val2 = (endPoint2 - endPoint).Normalize();
				if (extendEnd)
				{
					return (Curve)(object)Line.CreateBound(endPoint, endPoint2 + val2 * extendFeet);
				}
				return (Curve)(object)Line.CreateBound(endPoint - val2 * extendFeet, endPoint2);
			}
			Arc val3 = (Arc)(object)((curve is Arc) ? curve : null);
			if (val3 != null)
			{
				XYZ val4 = new XYZ(val3.Center.X, val3.Center.Y, 0.0);
				double radius = val3.Radius;
				double endParameter = ((Curve)val3).GetEndParameter(0);
				double endParameter2 = ((Curve)val3).GetEndParameter(1);
				double num = extendFeet / radius;
				double num2;
				double num3;
				if (extendEnd)
				{
					num2 = endParameter;
					num3 = endParameter2 + num;
				}
				else
				{
					num2 = endParameter - num;
					num3 = endParameter2;
				}
				if (num3 - num2 >= 6.282185307179586)
				{
					num3 = num2 + 6.2731853071795864;
				}
				XYZ val5 = new XYZ(val3.XDirection.X, val3.XDirection.Y, 0.0).Normalize();
				XYZ val6 = new XYZ(val3.YDirection.X, val3.YDirection.Y, 0.0).Normalize();
				return (Curve)(object)Arc.Create(val4, radius, num2, num3, val5, val6);
			}
			XYZ endPoint3 = curve.GetEndPoint(0);
			XYZ endPoint4 = curve.GetEndPoint(1);
			if (extendEnd)
			{
				XYZ val7 = curve.ComputeDerivatives(1.0, true).BasisX.Normalize();
				return (Curve)(object)Line.CreateBound(endPoint3, endPoint4 + val7 * extendFeet);
			}
			XYZ val8 = curve.ComputeDerivatives(0.0, true).BasisX.Normalize();
			return (Curve)(object)Line.CreateBound(endPoint3 - val8 * extendFeet, endPoint4);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] 曲线单端延伸失败，输入类型: " + ((object)curve)?.GetType().Name + "，错误原因: " + ex.Message, (Exception)null);
			return null;
		}
	}

	private Curve SafeReverseCurve(Curve curve, XYZ targetJointPoint, int curveOrder)
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		if ((GeometryObject)(object)curve == (GeometryObject)null)
		{
			return null;
		}
		bool flag = false;
		switch (curveOrder)
		{
		case 1:
			if (curve.GetEndPoint(0).DistanceTo(targetJointPoint) < curve.GetEndPoint(1).DistanceTo(targetJointPoint))
			{
				flag = true;
			}
			break;
		case 2:
			if (curve.GetEndPoint(1).DistanceTo(targetJointPoint) < curve.GetEndPoint(0).DistanceTo(targetJointPoint))
			{
				flag = true;
			}
			break;
		}
		if (!flag)
		{
			return curve;
		}
		Line val = (Line)(object)((curve is Line) ? curve : null);
		if (val != null)
		{
			return ((Curve)val).CreateReversed();
		}
		Arc val2 = (Arc)(object)((curve is Arc) ? curve : null);
		if (val2 != null)
		{
			try
			{
				XYZ val3 = new XYZ(val2.Center.X, val2.Center.Y, 0.0);
				double radius = val2.Radius;
				double endParameter = ((Curve)val2).GetEndParameter(0);
				double endParameter2 = ((Curve)val2).GetEndParameter(1);
				XYZ val4 = new XYZ(val2.XDirection.X, val2.XDirection.Y, 0.0).Normalize();
				XYZ val5 = new XYZ(val2.YDirection.X, val2.YDirection.Y, 0.0).Normalize();
				return (Curve)(object)Arc.Create(val3, radius, endParameter2, endParameter, val4, val5.Negate());
			}
			catch (Exception)
			{
				return ((Curve)val2).CreateReversed();
			}
		}
		return curve.CreateReversed();
	}

	private Curve TrimCurveToIntersection(Curve originalCurve, Curve extendedCurve, XYZ intersection, bool connectionEnd)
	{
		if ((GeometryObject)(object)originalCurve == (GeometryObject)null || intersection == null)
		{
			return null;
		}
		try
		{
			XYZ val = (connectionEnd ? originalCurve.GetEndPoint(0) : originalCurve.GetEndPoint(1));
			double num = 0.1;
			if (intersection.DistanceTo(val) < num)
			{
				return originalCurve;
			}
			if (originalCurve is Line || extendedCurve is Line)
			{
				XYZ val2 = (connectionEnd ? val : intersection);
				XYZ val3 = (connectionEnd ? intersection : val);
				if (val2.DistanceTo(val3) < 0.001)
				{
					return originalCurve;
				}
				return (Curve)(object)Line.CreateBound(val2, val3);
			}
			Curve val4 = extendedCurve ?? originalCurve;
			IntersectionResult val5 = val4.Project(intersection);
			IntersectionResult val6 = val4.Project(val);
			if (val5 == null || val6 == null)
			{
				return originalCurve;
			}
			double parameter = val5.Parameter;
			double parameter2 = val6.Parameter;
			Curve val7 = val4.Clone();
			double num2 = Math.Min(parameter, parameter2);
			double num3 = Math.Max(parameter, parameter2);
			val7.MakeBound(num2, num3);
			return val7;
		}
		catch (Exception)
		{
			return originalCurve;
		}
	}

	private List<XYZ> GetCurveIntersections(Curve curve1, Curve curve2)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		List<XYZ> list = new List<XYZ>();
		try
		{
			CurveIntersectResult val = curve1.Intersect(curve2, (CurveIntersectResultOption)1);
			if (val != null && (int)val.Result != 4)
			{
				IList<CurveOverlapPoint> overlaps = val.GetOverlaps();
				if (overlaps != null)
				{
					foreach (CurveOverlapPoint item in overlaps)
					{
						if (item != null && item.Point != null)
						{
							list.Add(item.Point);
						}
					}
				}
			}
		}
		catch (Exception)
		{
		}
		return list;
	}

	private List<XYZ> GenerateMultiCurvePolygon(RouteChain chain, double? widthOverride = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		//IL_0178: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			double num = widthOverride ?? chain.Width;
			Path64 val = new Path64();
			XYZ val2 = null;
			foreach (Curve curf in chain.Curves)
			{
				XYZ endPoint = curf.GetEndPoint(0);
				XYZ endPoint2 = curf.GetEndPoint(1);
				Curve val3 = curf;
				if (val2 != null)
				{
					double num2 = val2.DistanceTo(endPoint);
					double num3 = val2.DistanceTo(endPoint2);
					if (num3 < num2)
					{
						val3 = curf.CreateReversed();
					}
				}
				List<XYZ> list = ExtractCurvePoints(val3);
				foreach (XYZ item in list)
				{
					if (((List<Point64>)(object)val).Count == 0 || !IsPointDuplicate(((List<Point64>)(object)val)[((List<Point64>)(object)val).Count - 1], item))
					{
						long num4 = (long)(item.X * 100000.0);
						long num5 = (long)(item.Y * 100000.0);
						((List<Point64>)(object)val).Add(new Point64(num4, num5));
					}
				}
				val2 = val3.GetEndPoint(1);
			}
			val = Clipper.StripDuplicates(val, true);
			double num6 = num / 304.8;
			double num7 = num6 / 2.0 * 100000.0;
			Paths64 val4 = new Paths64();
			((List<Path64>)val4).Add(val);
			Paths64 val5 = val4;
			Paths64 val6 = Clipper.InflatePaths(val5, num7, (JoinType)0, (EndType)2, 5.0, GetClipperArcTolerance());
			if (val6 == null || ((List<Path64>)(object)val6).Count == 0)
			{
				return null;
			}
			return ClipperPathToXYZ(((List<Path64>)(object)val6)[0]);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] 多曲线生成失败", ex);
			return null;
		}
	}

	private Curve OffsetCurve(Curve curve, double offsetFeet, bool leftSide)
	{
		if ((GeometryObject)(object)curve == (GeometryObject)null || offsetFeet <= 0.0)
		{
			return null;
		}
		try
		{
			Curve val = FlattenCurveZ(curve);
			if ((GeometryObject)(object)val == (GeometryObject)null)
			{
				return null;
			}
			Arc val2 = (Arc)(object)((val is Arc) ? val : null);
			if (val2 != null)
			{
				return OffsetArcMathematically(val2, offsetFeet, leftSide);
			}
			Line val3 = (Line)(object)((val is Line) ? val : null);
			if (val3 != null)
			{
				return OffsetLineMathematically(val3, offsetFeet, leftSide);
			}
			XYZ val4 = (leftSide ? XYZ.BasisZ : XYZ.BasisZ.Negate());
			Curve val5 = val.CreateOffset(offsetFeet, val4);
			if ((GeometryObject)(object)val5 != (GeometryObject)null && val5.Length > 0.001)
			{
				return val5;
			}
			return null;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private Curve OffsetLineMathematically(Line line, double offsetFeet, bool leftSide)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		XYZ val = new XYZ(((Curve)line).GetEndPoint(0).X, ((Curve)line).GetEndPoint(0).Y, 0.0);
		XYZ val2 = new XYZ(((Curve)line).GetEndPoint(1).X, ((Curve)line).GetEndPoint(1).Y, 0.0);
		if (val.DistanceTo(val2) < 0.001)
		{
			return null;
		}
		XYZ val3 = (val2 - val).Normalize();
		XYZ val4 = val3.CrossProduct(XYZ.BasisZ).Normalize();
		XYZ val5 = (leftSide ? val4 : val4.Negate());
		return (Curve)(object)Line.CreateBound(val + val5 * offsetFeet, val2 + val5 * offsetFeet);
	}

	private Curve OffsetArcMathematically(Arc arc, double offsetFeet, bool leftSide)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		if ((GeometryObject)(object)arc == (GeometryObject)null || Math.Abs(offsetFeet) < 1E-09)
		{
			return (Curve)(object)arc;
		}
		try
		{
			XYZ val = new XYZ(arc.Center.X, arc.Center.Y, 0.0);
			double radius = arc.Radius;
			bool flag = arc.Normal.Z >= 0.0;
			double num = ((leftSide == flag) ? offsetFeet : (0.0 - offsetFeet));
			double num2 = radius + num;
			if (num2 < 0.001)
			{
				return null;
			}
			double endParameter = ((Curve)arc).GetEndParameter(0);
			double endParameter2 = ((Curve)arc).GetEndParameter(1);
			XYZ val2 = new XYZ(arc.XDirection.X, arc.XDirection.Y, 0.0).Normalize();
			XYZ val3 = new XYZ(arc.YDirection.X, arc.YDirection.Y, 0.0).Normalize();
			return (Curve)(object)Arc.Create(val, num2, endParameter, endParameter2, val2, val3);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] 数学偏移圆弧失败: " + ex.Message, (Exception)null);
			return null;
		}
	}

	private Path64 CurveToClipperPath(Curve curve)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Path64 val = new Path64();
		List<XYZ> list = ExtractCurvePoints(curve);
		foreach (XYZ item in list)
		{
			long num = (long)(item.X * 100000.0);
			long num2 = (long)(item.Y * 100000.0);
			((List<Point64>)(object)val).Add(new Point64(num, num2));
		}
		return val;
	}

	private bool IsPointDuplicate(Point64 lastPt, XYZ currentPt)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		double num = (double)lastPt.X / 100000.0;
		double num2 = (double)lastPt.Y / 100000.0;
		return Math.Abs(num - currentPt.X) < 0.001 && Math.Abs(num2 - currentPt.Y) < 0.001;
	}

	private List<XYZ> ClipperPathToXYZ(Path64 path)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		List<XYZ> list = new List<XYZ>();
		foreach (Point64 item in (List<Point64>)(object)path)
		{
			double num = (double)item.X / 100000.0;
			double num2 = (double)item.Y / 100000.0;
			list.Add(new XYZ(num, num2, 0.0));
		}
		return list;
	}

	private List<XYZ> UnionPolygons(List<List<XYZ>> polygons)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (polygons.Count == 1)
			{
				return polygons[0];
			}
			Paths64 val = new Paths64();
			foreach (List<XYZ> polygon in polygons)
			{
				Path64 val2 = new Path64();
				foreach (XYZ item in polygon)
				{
					long num = (long)(item.X * 100000.0);
					long num2 = (long)(item.Y * 100000.0);
					((List<Point64>)(object)val2).Add(new Point64(num, num2));
				}
				((List<Path64>)(object)val).Add(val2);
			}
			Paths64 val3 = Clipper.Union(val, (FillRule)1);
			if (val3 == null || ((List<Path64>)(object)val3).Count == 0)
			{
				return polygons[0];
			}
			return ClipperPathToXYZ(((List<Path64>)(object)val3)[0]);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] 多边形布尔并集失败", ex);
			return polygons[0];
		}
	}

	private List<List<XYZ>> PerformGlobalBooleanUnion(List<List<XYZ>> polygons, double intersectionRadius, bool applyMorphologicalClosing = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Expected O, but got Unknown
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Expected O, but got Unknown
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Expected O, but got Unknown
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Expected O, but got Unknown
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Expected O, but got Unknown
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		try
		{
			Paths64 val = new Paths64();
			foreach (List<XYZ> polygon in polygons)
			{
				Path64 val2 = new Path64();
				foreach (XYZ item in polygon)
				{
					long num = (long)(item.X * 100000.0);
					long num2 = (long)(item.Y * 100000.0);
					((List<Point64>)(object)val2).Add(new Point64(num, num2));
				}
				val2 = Clipper.StripDuplicates(val2, true);
				if (((List<Point64>)(object)val2).Count >= 3)
				{
					((List<Path64>)(object)val).Add(val2);
				}
			}
			PolyTree64 val3 = new PolyTree64();
			Clipper64 val4 = new Clipper64();
			val4.AddSubject(val);
			val4.Execute((ClipType)2, (FillRule)1, val3);
			if (((PolyPathBase)val3).Count == 0)
			{
				return polygons;
			}
			Paths64 val5 = ClipperPolyTreeToPaths(val3);
			if (!applyMorphologicalClosing)
			{
				List<List<XYZ>> list = new List<List<XYZ>>();
				foreach (Path64 item2 in (List<Path64>)(object)val5)
				{
					List<XYZ> list2 = new List<XYZ>();
					foreach (Point64 item3 in (List<Point64>)(object)item2)
					{
						list2.Add(new XYZ((double)item3.X / 100000.0, (double)item3.Y / 100000.0, 0.0));
					}
					if (list2.Count >= 3)
					{
						list.Add(list2);
					}
				}
				return list;
			}
			double num3 = intersectionRadius * 100000.0;
			double clipperArcTolerance = GetClipperArcTolerance();
			ClipperOffset val6 = new ClipperOffset(2.0, clipperArcTolerance, false, false);
			val6.AddPaths(val5, (JoinType)3, (EndType)0);
			Paths64 val7 = new Paths64();
			val6.Execute(num3, val7);
			val6.Clear();
			val6.AddPaths(val7, (JoinType)3, (EndType)0);
			Paths64 val8 = new Paths64();
			val6.Execute(0.0 - num3, val8);
			PolyTree64 val9 = new PolyTree64();
			Clipper64 val10 = new Clipper64();
			val10.AddSubject(val8);
			val10.Execute((ClipType)2, (FillRule)1, val9);
			Paths64 val11 = ClipperPolyTreeToPaths(val9);
			List<List<XYZ>> list3 = new List<List<XYZ>>();
			int num4 = 0;
			foreach (Path64 item4 in (List<Path64>)(object)val11)
			{
				List<XYZ> list4 = new List<XYZ>();
				foreach (Point64 item5 in (List<Point64>)(object)item4)
				{
					double num5 = (double)item5.X / 100000.0;
					double num6 = (double)item5.Y / 100000.0;
					list4.Add(new XYZ(num5, num6, 0.0));
				}
				if (list4.Count >= 3)
				{
					list3.Add(list4);
					num4++;
				}
			}
			return list3;
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] 全局布尔并集失败", ex);
			return polygons;
		}
	}

	private Curve TrimCurveOneEnd(Curve curve, double trimLengthMm, bool trimStart)
	{
		if ((GeometryObject)(object)curve == (GeometryObject)null)
		{
			return null;
		}
		try
		{
			double num = trimLengthMm / 304.8;
			if (curve.Length <= num + 0.001)
			{
				return null;
			}
			double endParameter = curve.GetEndParameter(0);
			double endParameter2 = curve.GetEndParameter(1);
			double num2 = endParameter2 - endParameter;
			double num3 = num2 * (num / curve.Length);
			double num4 = (trimStart ? (endParameter + num3) : endParameter);
			double num5 = (trimStart ? endParameter2 : (endParameter2 - num3));
			if (num5 - num4 < 0.001)
			{
				return null;
			}
			Curve val = curve.Clone();
			val.MakeBound(num4, num5);
			return val;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private RouteChain TrimChainOpenEnds(RouteChain chain, double trimLengthMm)
	{
		if (chain == null || chain.Curves == null || chain.Curves.Count == 0)
		{
			return null;
		}
		List<Curve> list = new List<Curve>();
		List<double> list2 = new List<double>();
		for (int i = 0; i < chain.Curves.Count; i++)
		{
			Curve val = chain.Curves[i];
			double item = ((chain.CurveWidths.Count > i) ? chain.CurveWidths[i] : chain.Width);
			Curve val2 = val;
			if (i == 0)
			{
				val2 = TrimCurveOneEnd(val, trimLengthMm, trimStart: true);
			}
			if (i == chain.Curves.Count - 1)
			{
				val2 = TrimCurveOneEnd(val2 ?? val, trimLengthMm, trimStart: false);
			}
			if ((GeometryObject)(object)val2 != (GeometryObject)null)
			{
				list.Add(val2);
				list2.Add(item);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		return new RouteChain
		{
			Curves = list,
			Width = list2.Min(),
			CurveWidths = list2
		};
	}

	private (List<List<XYZ>> sidewalkPolygons, List<List<XYZ>> curbPolygons, List<List<XYZ>> mergedSidewalkPolygons) GenerateSidewalkAndCurbPolygons(List<RouteChain> chains, double sidewalkWidthMm, double curbWidthMm, double intersectionRadiusFeet, List<List<XYZ>> lanePolygons)
	{
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected O, but got Unknown
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Expected O, but got Unknown
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Expected O, but got Unknown
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Expected O, but got Unknown
		try
		{
			if (chains == null || chains.Count == 0)
			{
				return (sidewalkPolygons: null, curbPolygons: null, mergedSidewalkPolygons: null);
			}
			if (lanePolygons == null || lanePolygons.Count == 0)
			{
				return (sidewalkPolygons: null, curbPolygons: null, mergedSidewalkPolygons: null);
			}
			List<List<XYZ>> list = new List<List<XYZ>>();
			foreach (RouteChain chain in chains)
			{
				RouteChain routeChain = TrimChainOpenEnds(chain, 10.0);
				if (routeChain != null)
				{
					List<XYZ> list2 = GenerateRoadPolygonWithClipper(routeChain, sidewalkWidthMm);
					if (list2 != null && list2.Count >= 3)
					{
						list.Add(list2);
					}
				}
			}
			if (list.Count == 0)
			{
				return (sidewalkPolygons: null, curbPolygons: null, mergedSidewalkPolygons: null);
			}
			List<List<XYZ>> list3 = ((list.Count <= 0) ? list : PerformGlobalBooleanUnion(list, intersectionRadiusFeet));
			if (list3 == null || list3.Count == 0)
			{
				return (sidewalkPolygons: null, curbPolygons: null, mergedSidewalkPolygons: null);
			}
			Paths64 val = PolygonsToClipperPaths(list3);
			Paths64 val2 = PolygonsToClipperPaths(lanePolygons);
			PolyTree64 val3 = new PolyTree64();
			Clipper64 val4 = new Clipper64();
			val4.AddSubject(val);
			val4.AddClip(val2);
			val4.Execute((ClipType)3, (FillRule)1, val3);
			if (((PolyPathBase)val3).Count == 0)
			{
				return (sidewalkPolygons: null, curbPolygons: null, mergedSidewalkPolygons: null);
			}
			Paths64 val5 = ClipperPolyTreeToPaths(val3);
			List<List<XYZ>> list4 = null;
			List<List<XYZ>> list5 = null;
			if (curbWidthMm > 0.0)
			{
				double num = curbWidthMm / 304.8;
				double num2 = num * 100000.0;
				double num3 = num2 / 2.0;
				double clipperArcTolerance = GetClipperArcTolerance();
				ClipperOffset val6 = new ClipperOffset(2.0, clipperArcTolerance, false, false);
				val6.AddPaths(val5, (JoinType)3, (EndType)0);
				Paths64 val7 = new Paths64();
				val6.Execute(0.0 - num3, val7);
				if (((List<Path64>)(object)val7).Count > 0)
				{
					list5 = new List<List<XYZ>>();
					foreach (Path64 item in (List<Path64>)(object)val7)
					{
						List<XYZ> list6 = ClipperPathToXYZ(item);
						if (list6 != null && list6.Count >= 3)
						{
							list5.Add(list6);
						}
					}
				}
				ClipperOffset val8 = new ClipperOffset(2.0, clipperArcTolerance, false, false);
				val8.AddPaths(val5, (JoinType)3, (EndType)0);
				Paths64 val9 = new Paths64();
				val8.Execute(0.0 - num2, val9);
				if (((List<Path64>)(object)val9).Count > 0)
				{
					list4 = new List<List<XYZ>>();
					foreach (Path64 item2 in (List<Path64>)(object)val9)
					{
						List<XYZ> list7 = ClipperPathToXYZ(item2);
						if (list7 != null && list7.Count >= 3)
						{
							list4.Add(list7);
						}
					}
				}
			}
			else
			{
				list4 = new List<List<XYZ>>();
				foreach (Path64 item3 in (List<Path64>)(object)val5)
				{
					List<XYZ> list8 = ClipperPathToXYZ(item3);
					if (list8 != null && list8.Count >= 3)
					{
						list4.Add(list8);
					}
				}
			}
			return ValueTuple.Create(list4, list5, list3);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] 生成人行道和路沿石多边形失败", ex);
			return ValueTuple.Create<List<List<XYZ>>, List<List<XYZ>>, List<List<XYZ>>>(null, null, null);
		}
	}

	private Path64 BuildChainCenterlinePath(RouteChain chain)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (chain?.Curves == null || chain.Curves.Count == 0)
			{
				return null;
			}
			if (chain.Curves.Count == 1)
			{
				return CurveToClipperPath(chain.Curves[0]);
			}
			Path64 val = new Path64();
			XYZ val2 = null;
			foreach (Curve curf in chain.Curves)
			{
				XYZ endPoint = curf.GetEndPoint(0);
				XYZ endPoint2 = curf.GetEndPoint(1);
				Curve val3 = curf;
				if (val2 != null)
				{
					double num = val2.DistanceTo(endPoint);
					double num2 = val2.DistanceTo(endPoint2);
					if (num2 < num)
					{
						val3 = curf.CreateReversed();
					}
				}
				List<XYZ> list = ExtractCurvePoints(val3);
				foreach (XYZ item in list)
				{
					if (((List<Point64>)(object)val).Count == 0 || !IsPointDuplicate(((List<Point64>)(object)val)[((List<Point64>)(object)val).Count - 1], item))
					{
						((List<Point64>)(object)val).Add(new Point64((long)(item.X * 100000.0), (long)(item.Y * 100000.0)));
					}
				}
				val2 = val3.GetEndPoint(1);
			}
			return Clipper.StripDuplicates(val, true);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private List<XYZ> ExtractCurvePoints(Curve curve)
	{
		List<XYZ> list = new List<XYZ>();
		try
		{
			Line val = (Line)(object)((curve is Line) ? curve : null);
			if (val != null)
			{
				list.Add(((Curve)val).GetEndPoint(0));
				list.Add(((Curve)val).GetEndPoint(1));
			}
			else
			{
				Arc val2 = (Arc)(object)((curve is Arc) ? curve : null);
				if (val2 != null)
				{
					int num = Math.Max(12, (int)(((Curve)val2).Length * 1.0));
					for (int i = 0; i <= num; i++)
					{
						double num2 = (double)i / (double)num;
						XYZ item = ((Curve)val2).Evaluate(num2, true);
						list.Add(item);
					}
				}
				else
				{
					IList<XYZ> collection = curve.Tessellate();
					list.AddRange(collection);
				}
			}
		}
		catch (Exception)
		{
			list.Add(curve.GetEndPoint(0));
			list.Add(curve.GetEndPoint(1));
		}
		return list;
	}

	private List<Curve> OptimizePolygonToCurves(List<XYZ> rawPoints)
	{
		if (rawPoints == null || rawPoints.Count < 2)
		{
			return new List<Curve>();
		}
		List<Curve> list = new List<Curve>();
		int count = rawPoints.Count;
		int num = 0;
		double num2 = 5.0 / 762.0;
		XYZ val = rawPoints[0];
		while (num < count)
		{
			int num3 = num + 1;
			Curve val2 = null;
			for (int i = num + 2; i <= count; i++)
			{
				XYZ val3 = rawPoints[num];
				XYZ val4 = ((i == count) ? rawPoints[0] : rawPoints[i]);
				int index = num + (i - num) / 2;
				XYZ val5 = rawPoints[index];
				Curve val6 = null;
				if (IsCollinear(val3, val5, val4))
				{
					val6 = (Curve)(object)Line.CreateBound(val3, val4);
				}
				else
				{
					try
					{
						val6 = (Curve)(object)Arc.Create(val3, val5, val4);
					}
					catch
					{
						break;
					}
				}
				bool flag = true;
				for (int j = num + 1; j < i; j++)
				{
					XYZ val7 = rawPoints[j];
					try
					{
						IntersectionResult val8 = val6.Project(val7);
						if (val8 == null || val8.Distance > num2)
						{
							flag = false;
							break;
						}
					}
					catch
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					break;
				}
				num3 = i;
				val2 = val6;
			}
			if ((GeometryObject)(object)val2 != (GeometryObject)null)
			{
				if (num3 == count && list.Count > 0)
				{
					XYZ endPoint = val2.GetEndPoint(0);
					XYZ val9 = val;
					int index2 = num + (count - num) / 2;
					XYZ val10 = rawPoints[index2];
					if (IsCollinear(endPoint, val10, val9))
					{
						val2 = (Curve)(object)Line.CreateBound(endPoint, val9);
					}
					else
					{
						try
						{
							val2 = (Curve)(object)Arc.Create(endPoint, val10, val9);
						}
						catch
						{
							val2 = (Curve)(object)Line.CreateBound(endPoint, val9);
						}
					}
				}
				list.Add(val2);
				num = num3;
			}
			else
			{
				XYZ val11 = rawPoints[num];
				XYZ val12 = ((num + 1 == count) ? rawPoints[0] : rawPoints[num + 1]);
				list.Add((Curve)(object)Line.CreateBound(val11, val12));
				num++;
			}
		}
		return list;
	}

	private bool IsCollinear(XYZ p1, XYZ p2, XYZ p3)
	{
		XYZ val = (p2 - p1).Normalize();
		XYZ val2 = (p3 - p1).Normalize();
		return val.CrossProduct(val2).GetLength() < 0.005;
	}

	private CurveLoop ConvertPolygonToCurveLoop(List<XYZ> polygonPoints)
	{
		//IL_0b7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b80: Invalid comparison between Unknown and I4
		try
		{
			if (polygonPoints == null || polygonPoints.Count < 3)
			{
				return null;
			}
			double num = _document.Application.ShortCurveTolerance * 2.0;
			List<double> list = new List<double>();
			for (int i = 0; i < polygonPoints.Count - 1; i++)
			{
				double item = polygonPoints[i].DistanceTo(polygonPoints[i + 1]);
				list.Add(item);
			}
			if (list.Count == 0)
			{
				return null;
			}
			list.Average();
			HashSet<int> hashSet = new HashSet<int>
			{
				0,
				polygonPoints.Count - 1
			};
			for (int j = 1; j < list.Count; j++)
			{
				double num2 = Math.Abs(list[j] - list[j - 1]) / list[j - 1];
				if (num2 > 0.05)
				{
					hashSet.Add(j);
					hashSet.Add(j + 1);
				}
			}
			List<XYZ> list2 = new List<XYZ>();
			for (int k = 0; k < polygonPoints.Count; k++)
			{
				XYZ val = polygonPoints[k];
				if (hashSet.Contains(k))
				{
					if (list2.Count == 0 || val.DistanceTo(list2.Last()) > num)
					{
						list2.Add(val);
					}
				}
				else if (list2.Count == 0 || val.DistanceTo(list2.Last()) > num)
				{
					list2.Add(val);
				}
			}
			if (list2.Count > 1 && list2.Last().DistanceTo(list2[0]) <= num)
			{
				list2.RemoveAt(list2.Count - 1);
			}
			if (list2.Count < 3)
			{
				return null;
			}
			List<Curve> list3 = new List<Curve>();
			List<double> list4 = new List<double>();
			for (int l = 0; l < list2.Count - 1; l++)
			{
				double item2 = list2[l].DistanceTo(list2[l + 1]);
				list4.Add(item2);
			}
			if (list4.Count == 0)
			{
				return null;
			}
			list4.Average();
			list4.Min();
			list4.Max();
			List<List<int>> list5 = new List<List<int>>();
			List<int> list6 = new List<int> { 0 };
			_ = list4[0];
			List<double> list7 = new List<double> { list4[0] };
			for (int m = 0; m < list4.Count; m++)
			{
				double num3 = list4[m];
				bool flag = false;
				if (list7.Count > 0)
				{
					double num4 = list7.Average();
					double num5 = Math.Abs(num3 - num4) / num4;
					if (num5 > 0.05)
					{
						flag = true;
					}
				}
				if (flag)
				{
					list5.Add(list6);
					list6 = new List<int>
					{
						m,
						m + 1
					};
					list7 = new List<double> { num3 };
				}
				else
				{
					list6.Add(m + 1);
					list7.Add(num3);
				}
			}
			if (list6.Count > 0)
			{
				list5.Add(list6);
			}
			for (int n = 0; n < list5.Count; n++)
			{
				List<int> list8 = list5[n];
				XYZ val2 = list2[list8[0]];
				XYZ val3 = list2[list8[list8.Count - 1]];
				List<double> list9 = new List<double>();
				for (int num6 = 0; num6 < list8.Count - 1; num6++)
				{
					if (list8[num6] < list4.Count)
					{
						list9.Add(list4[list8[num6]]);
					}
				}
				if (list9.Count > 0)
				{
					list9.Average();
				}
				else
					_ = 0.0;
				if (list8.Count == 2)
				{
					Line item3 = Line.CreateBound(val2, val3);
					list3.Add((Curve)(object)item3);
				}
				else
				{
					if (list8.Count < 3)
					{
						continue;
					}
					int index = list8[list8.Count / 2];
					XYZ mid = list2[index];
					try
					{
						Arc val4 = CreateArcFromThreePoints(val2, mid, val3);
						if (!((GeometryObject)(object)val4 != (GeometryObject)null))
						{
							throw new Exception("CreateArcFromThreePoints返回null");
						}
						list3.Add((Curve)(object)val4);
					}
					catch (Exception ex)
					{
						ILogger logger = _logger;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[RoadNetworkEngine] 第");
						defaultInterpolatedStringHandler.AppendFormatted(n + 1);
						defaultInterpolatedStringHandler.AppendLiteral("段圆弧创建失败(");
						defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
						defaultInterpolatedStringHandler.AppendLiteral(")，降级为直线段");
						logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
						for (int num7 = 0; num7 < list8.Count - 1; num7++)
						{
							Line item4 = Line.CreateBound(list2[list8[num7]], list2[list8[num7 + 1]]);
							list3.Add((Curve)(object)item4);
						}
					}
				}
			}
			double num8 = Math.Cos(0.00034906585039886593);
			double num9 = num;
			int num10 = 0;
			XYZ val5 = list2[0];
			while (num10 < list2.Count - 1)
			{
				bool flag2 = false;
				XYZ val6 = null;
				int num11 = Math.Min(5, list2.Count - num10 - 1);
				List<double> list10 = new List<double>();
				for (int num12 = 0; num12 < num11; num12++)
				{
					XYZ val7 = list2[num10 + num12];
					XYZ val8 = list2[num10 + num12 + 1];
					XYZ val9 = (val8 - val7).Normalize();
					if (val6 != null)
					{
						double num13 = Math.Abs(val6.DotProduct(val9));
						double item5 = Math.Acos(Math.Min(1.0, Math.Max(-1.0, num13))) * 180.0 / Math.PI;
						list10.Add(item5);
						if (num13 < num8)
						{
							flag2 = true;
						}
					}
					val6 = val9;
				}
				if (list10.Count > 0)
				{
					list10.Max();
					list10.Average();
				}
				int num14 = num10 + 1;
				for (int num15 = num10 + 2; num15 < list2.Count; num15++)
				{
					XYZ val10 = list2[num15];
					try
					{
						Line val11 = Line.CreateBound(val5, val10);
						bool flag3 = true;
						if (!flag2)
						{
							XYZ val12 = (val10 - val5).Normalize();
							for (int num16 = num10; num16 < num15; num16++)
							{
								XYZ val13 = (list2[num16 + 1] - list2[num16]).Normalize();
								double num17 = Math.Abs(val12.DotProduct(val13));
								if (num17 < num8)
								{
									flag3 = false;
									break;
								}
							}
						}
						if (flag3)
						{
							for (int num18 = num10; num18 < num15; num18++)
							{
								XYZ val14 = list2[num18];
								XYZ val15 = list2[num18 + 1];
								if (num18 > num10)
								{
									IntersectionResult val16 = ((Curve)val11).Project(val14);
									if (val16 == null || val16.Distance > num)
									{
										flag3 = false;
										break;
									}
								}
								XYZ val17 = (val14 + val15) / 2.0;
								IntersectionResult val18 = ((Curve)val11).Project(val17);
								if (val18 == null || val18.Distance > num)
								{
									flag3 = false;
									break;
								}
							}
						}
						if (flag3)
						{
							num14 = num15;
							continue;
						}
					}
					catch
					{
					}
					break;
				}
				int num19 = num10;
				Curve val19 = null;
				int num20 = 0;
				int num21 = 0;
				int num22 = 0;
				for (int num23 = num10 + 2; num23 < list2.Count; num23++)
				{
					XYZ val20 = list2[num23];
					int num24 = num10 + (num23 - num10) / 2;
					XYZ val21 = list2[num24];
					if (num24 == num10 || num24 == num23)
					{
						continue;
					}
					num21++;
					try
					{
						Arc val22 = Arc.Create(val5, val21, val20);
						num22++;
						int num25 = Math.Max(1, (num23 - num10) / 10);
						bool flag4 = true;
						double val23 = 0.0;
						for (int num26 = num10; num26 <= num23; num26 += num25)
						{
							XYZ val24 = list2[num26];
							double num27 = ((Curve)val22).Distance(val24);
							val23 = Math.Max(val23, num27);
							if (num27 > num9)
							{
								flag4 = false;
								break;
							}
						}
						if (flag4)
						{
							num19 = num23;
							val19 = (Curve)(object)val22;
							num20 = 0;
						}
						else if ((GeometryObject)(object)val19 != (GeometryObject)null)
						{
							num20++;
							if (num20 >= 2)
							{
								break;
							}
						}
					}
					catch
					{
						if ((GeometryObject)(object)val19 != (GeometryObject)null)
						{
							num20++;
							if (num20 >= 2)
							{
								break;
							}
						}
					}
				}
				int num28 = num14 - num10;
				int num29 = num19 - num10;
				if ((GeometryObject)(object)val19 != (GeometryObject)null && num29 > num28)
				{
					list3.Add(val19);
					val5 = val19.GetEndPoint(1);
					num10 = num19;
				}
				else
				{
					Curve val25 = (Curve)(object)Line.CreateBound(val5, list2[num14]);
					list3.Add(val25);
					val5 = val25.GetEndPoint(1);
					num10 = num14;
				}
			}
			XYZ endPoint = list3[0].GetEndPoint(0);
			XYZ endPoint2 = list3[list3.Count - 1].GetEndPoint(1);
			if (endPoint2.DistanceTo(endPoint) > num)
			{
				try
				{
					XYZ val26 = (endPoint2 + endPoint) / 2.0;
					Curve item6 = (Curve)((!IsCollinear(endPoint2, val26, endPoint)) ? ((object)Arc.Create(endPoint2, val26, endPoint)) : ((object)Line.CreateBound(endPoint2, endPoint)));
					list3.Add(item6);
				}
				catch
				{
					list3.Add((Curve)(object)Line.CreateBound(endPoint2, endPoint));
				}
			}
			if (list3.Count >= 3)
			{
				try
				{
					for (int num30 = 0; num30 < list3.Count; num30++)
					{
						Curve val27 = list3[num30];
						Curve val28 = list3[(num30 + 1) % list3.Count];
						XYZ endPoint3 = val27.GetEndPoint(1);
						XYZ endPoint4 = val28.GetEndPoint(0);
						double num31 = endPoint3.DistanceTo(endPoint4);
						if (num31 > 0.01)
						{
							ILogger logger2 = _logger;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(38, 3);
							defaultInterpolatedStringHandler2.AppendLiteral("[RoadNetworkEngine] 曲线 ");
							defaultInterpolatedStringHandler2.AppendFormatted(num30);
							defaultInterpolatedStringHandler2.AppendLiteral(" 和 ");
							defaultInterpolatedStringHandler2.AppendFormatted((num30 + 1) % list3.Count);
							defaultInterpolatedStringHandler2.AppendLiteral(" 之间存在间隙: ");
							defaultInterpolatedStringHandler2.AppendFormatted(num31 * 304.8, "F2");
							defaultInterpolatedStringHandler2.AppendLiteral(" mm");
							logger2.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
						}
					}
					for (int num32 = 0; num32 < list3.Count; num32++)
					{
						for (int num33 = num32 + 2; num33 < list3.Count; num33++)
						{
							if (num32 == 0 && num33 == list3.Count - 1)
							{
								continue;
							}
							Curve val29 = list3[num32];
							Curve val30 = list3[num33];
							try
							{
								CurveIntersectResult val31 = val29.Intersect(val30, (CurveIntersectResultOption)1);
								if (val31 != null && (int)val31.Result != 4)
								{
									IList<CurveOverlapPoint> overlaps = val31.GetOverlaps();
									if (overlaps != null && overlaps.Count > 0)
									{
										ILogger logger3 = _logger;
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(46, 3);
										defaultInterpolatedStringHandler3.AppendLiteral("[RoadNetworkEngine] 检测到曲线交叉！曲线 ");
										defaultInterpolatedStringHandler3.AppendFormatted(num32);
										defaultInterpolatedStringHandler3.AppendLiteral(" 与曲线 ");
										defaultInterpolatedStringHandler3.AppendFormatted(num33);
										defaultInterpolatedStringHandler3.AppendLiteral(" 相交，重叠点数: ");
										defaultInterpolatedStringHandler3.AppendFormatted(overlaps.Count);
										logger3.Error(defaultInterpolatedStringHandler3.ToStringAndClear(), (Exception)null);
									}
								}
							}
							catch (Exception ex2)
							{
								ILogger logger4 = _logger;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(35, 3);
								defaultInterpolatedStringHandler4.AppendLiteral("[RoadNetworkEngine] 检查曲线 ");
								defaultInterpolatedStringHandler4.AppendFormatted(num32);
								defaultInterpolatedStringHandler4.AppendLiteral(" 和 ");
								defaultInterpolatedStringHandler4.AppendFormatted(num33);
								defaultInterpolatedStringHandler4.AppendLiteral(" 相交失败: ");
								defaultInterpolatedStringHandler4.AppendFormatted(ex2.Message);
								logger4.Warning(defaultInterpolatedStringHandler4.ToStringAndClear());
							}
						}
					}
					for (int num34 = 0; num34 < list3.Count; num34++)
					{
						try
						{
							Curve val32 = list3[num34];
							val32.GetEndPoint(0);
							val32.GetEndPoint(1);
							_ = ((object)val32).GetType().Name;
							_ = val32.Length;
						}
						catch (Exception ex3)
						{
							ILogger logger5 = _logger;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(14, 2);
							defaultInterpolatedStringHandler5.AppendLiteral("  曲线 ");
							defaultInterpolatedStringHandler5.AppendFormatted(num34);
							defaultInterpolatedStringHandler5.AppendLiteral(" 信息获取失败: ");
							defaultInterpolatedStringHandler5.AppendFormatted(ex3.Message);
							logger5.Error(defaultInterpolatedStringHandler5.ToStringAndClear(), (Exception)null);
						}
					}
				}
				catch (Exception ex4)
				{
					_logger.Error("[RoadNetworkEngine] 曲线连续性检查失败: " + ex4.Message, (Exception)null);
				}
				try
				{
					return CurveLoop.Create((IList<Curve>)list3);
				}
				catch (Exception ex5)
				{
					ILogger logger6 = _logger;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(48, 2);
					defaultInterpolatedStringHandler6.AppendLiteral("[RoadNetworkEngine] CurveLoop.Create 失败: ");
					defaultInterpolatedStringHandler6.AppendFormatted(ex5.Message);
					defaultInterpolatedStringHandler6.AppendLiteral("，曲线数量: ");
					defaultInterpolatedStringHandler6.AppendFormatted(list3.Count);
					logger6.Error(defaultInterpolatedStringHandler6.ToStringAndClear(), (Exception)null);
					for (int num35 = 0; num35 < list3.Count; num35++)
					{
						try
						{
							_ = list3[num35];
						}
						catch (Exception ex6)
						{
							ILogger logger7 = _logger;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(32, 2);
							defaultInterpolatedStringHandler7.AppendLiteral("[RoadNetworkEngine] 曲线 ");
							defaultInterpolatedStringHandler7.AppendFormatted(num35);
							defaultInterpolatedStringHandler7.AppendLiteral(" 信息获取失败: ");
							defaultInterpolatedStringHandler7.AppendFormatted(ex6.Message);
							logger7.Error(defaultInterpolatedStringHandler7.ToStringAndClear(), (Exception)null);
						}
					}
					throw;
				}
			}
			return null;
		}
		catch (Exception ex7)
		{
			_logger.Error("[RoadNetworkEngine] 转换 CurveLoop 严重失败: " + ex7.Message, (Exception)null);
			return null;
		}
	}

	private List<Floor> CreateFloorFromPolyTree(List<List<XYZ>> polygons, ElementId floorTypeId, ElementId levelId)
	{
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Expected O, but got Unknown
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Expected O, but got Unknown
		try
		{
			if (polygons == null || polygons.Count == 0)
			{
				return new List<Floor>();
			}
			List<(CurveLoop, double)> list = new List<(CurveLoop, double)>();
			foreach (List<XYZ> polygon in polygons)
			{
				CurveLoop val = FitPolygonToCurveLoop(polygon);
				if (val != null)
				{
					double item = CalculateCurveLoopArea(val);
					list.Add((val, item));
				}
			}
			if (list.Count == 0)
			{
				return new List<Floor>();
			}
			List<CurveLoop> list2 = new List<CurveLoop>();
			List<CurveLoop> list3 = new List<CurveLoop>();
			foreach (var item2 in list)
			{
				if (item2.Item2 > 0.0)
				{
					list2.Add(item2.Item1);
				}
				else if (item2.Item2 < 0.0)
				{
					list3.Add(item2.Item1);
				}
			}
			if (list2.Count == 0)
			{
				List<(CurveLoop, double)> list4 = list.OrderByDescending<(CurveLoop, double), double>(((CurveLoop loop, double area) x) => Math.Abs(x.area)).ToList();
				list2.Add(list4[0].Item1);
				for (int num = 1; num < list4.Count; num++)
				{
					list3.Add(list4[num].Item1);
				}
			}
			List<CurveLoop> list5 = new List<CurveLoop>();
			foreach (CurveLoop item3 in list2)
			{
				CurveLoop val2 = EnsureCurveLoopDirection(item3, counterClockwise: true);
				if (IsCurveLoopClosed(val2))
				{
					list5.Add(val2);
				}
			}
			if (list5.Count == 0)
			{
				return null;
			}
			List<CurveLoop> list6 = new List<CurveLoop>();
			foreach (CurveLoop item4 in list3)
			{
				CurveLoop val3 = EnsureCurveLoopDirection(item4, counterClockwise: false);
				if (IsCurveLoopClosed(val3))
				{
					list6.Add(val3);
				}
			}
			Dictionary<int, List<CurveLoop>> dictionary = new Dictionary<int, List<CurveLoop>>();
			for (int num2 = 0; num2 < list5.Count; num2++)
			{
				dictionary[num2] = new List<CurveLoop>();
			}
			List<CurveLoop> list7 = new List<CurveLoop>();
			foreach (CurveLoop item5 in list6)
			{
				XYZ firstPointOfCurveLoop = GetFirstPointOfCurveLoop(item5);
				if (firstPointOfCurveLoop == null)
				{
					continue;
				}
				bool flag = false;
				for (int num3 = 0; num3 < list5.Count; num3++)
				{
					if (IsPointInsideCurveLoop(list5[num3], firstPointOfCurveLoop))
					{
						dictionary[num3].Add(item5);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list7.Add(item5);
				}
			}
			List<Floor> list8 = new List<Floor>();
			int num4 = 0;
			foreach (KeyValuePair<int, List<CurveLoop>> item6 in dictionary)
			{
				CurveLoop val4 = list5[item6.Key];
				List<CurveLoop> value = item6.Value;
				Floor val5 = null;
				try
				{
					int num5 = 0;
					foreach (Curve item7 in val4)
					{
						_ = item7;
						num5++;
					}
					ILogger logger = _logger;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[RoadNetworkEngine] 尝试创建楼板，CurveLoop 包含 ");
					defaultInterpolatedStringHandler.AppendFormatted(num5);
					defaultInterpolatedStringHandler.AppendLiteral(" 条曲线");
					logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					if (!IsCurveLoopClosed(val4))
					{
						_logger.Error("[RoadNetworkEngine] CurveLoop 未闭合，无法创建楼板", (Exception)null);
						continue;
					}
					Element element = _document.GetElement(floorTypeId);
					if (element == null)
					{
						ILogger logger2 = _logger;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("[RoadNetworkEngine] floorTypeId ");
						defaultInterpolatedStringHandler2.AppendFormatted(floorTypeId.smethod_0());
						defaultInterpolatedStringHandler2.AppendLiteral(" 对应元素不存在");
						logger2.Error(defaultInterpolatedStringHandler2.ToStringAndClear(), (Exception)null);
						continue;
					}
					FloorType val6 = (FloorType)(object)((element is FloorType) ? element : null);
					ILogger logger3;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3;
					object obj;
					if (val6 == null)
					{
						logger3 = _logger;
						defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(58, 3);
						defaultInterpolatedStringHandler3.AppendLiteral("[RoadNetworkEngine] floorTypeId ");
						defaultInterpolatedStringHandler3.AppendFormatted(floorTypeId.smethod_0());
						defaultInterpolatedStringHandler3.AppendLiteral(" 不是 FloorType，实际类型: ");
						defaultInterpolatedStringHandler3.AppendFormatted(((object)element).GetType().Name);
						defaultInterpolatedStringHandler3.AppendLiteral(", 类别: ");
						Category category = element.Category;
						if (category == null)
						{
							obj = null;
						}
						else
						{
							obj = category.Name;
							if (obj != null)
							{
								goto IL_04e6;
							}
						}
						obj = "null";
						goto IL_04e6;
					}
					val5 = Floor.Create(_document, (IList<CurveLoop>)new List<CurveLoop> { val4 }, floorTypeId, levelId);
					if (val5 == null)
					{
						_logger.Error("[RoadNetworkEngine] Floor.Create 返回 null", (Exception)null);
						continue;
					}
					goto end_IL_0344;
					IL_04e6:
					defaultInterpolatedStringHandler3.AppendFormatted((string?)obj);
					logger3.Error(defaultInterpolatedStringHandler3.ToStringAndClear(), (Exception)null);
					continue;
					end_IL_0344:;
				}
				catch (Exception ex)
				{
					_logger.Error("[RoadNetworkEngine] 创建楼板失败: " + ex.Message, (Exception)null);
					continue;
				}
				list8.Add(val5);
				_document.Regenerate();
				foreach (CurveLoop item8 in value)
				{
					try
					{
						CurveArray val7 = new CurveArray();
						foreach (Curve item9 in item8)
						{
							val7.Append(item9);
						}
						Opening val8 = _document.Create.NewOpening((Element)(object)val5, val7, false);
						if (val8 != null)
						{
							num4++;
							_document.Regenerate();
						}
					}
					catch (Exception)
					{
					}
				}
			}
			Floor val9 = list8.FirstOrDefault();
			if (list7.Count > 0 && val9 != null)
			{
				foreach (CurveLoop item10 in list7)
				{
					try
					{
						CurveArray val10 = new CurveArray();
						foreach (Curve item11 in item10)
						{
							val10.Append(item11);
						}
						Opening val11 = _document.Create.NewOpening((Element)(object)val9, val10, false);
						if (val11 != null)
						{
							num4++;
							_document.Regenerate();
						}
					}
					catch (Exception)
					{
					}
				}
			}
			return list8;
		}
		catch (Exception ex4)
		{
			_logger.Error("[RoadNetworkEngine] 创建楼板失败", ex4);
			return new List<Floor>();
		}
	}

	private Floor CreateSingleFloorWithOffset(List<XYZ> polygon, ElementId floorTypeId, ElementId levelId, double thicknessMillimeters)
	{
		try
		{
			CurveLoop val = FitPolygonToCurveLoop(polygon);
			if (val == null)
			{
				return null;
			}
			if (!IsCurveLoopClosed(val))
			{
				return null;
			}
			Floor val2 = Floor.Create(_document, (IList<CurveLoop>)new List<CurveLoop> { val }, floorTypeId, levelId);
			if (val2 == null)
			{
				return null;
			}
			double num = thicknessMillimeters / 304.8;
			Parameter val3 = ((Element)val2).get_Parameter((BuiltInParameter)(-1001951L));
			if (val3 != null)
			{
				val3.Set(num);
			}
			return val2;
		}
		catch (Exception ex)
		{
			_logger.Warning("[RoadNetworkEngine] 创建单个楼板失败: " + ex.Message);
			return null;
		}
	}

	private List<Floor> CreateFloorFromPolyTreeWithOffset(List<List<XYZ>> polygons, ElementId floorTypeId, ElementId levelId, double thicknessMillimeters)
	{
		try
		{
			List<Floor> list = CreateFloorFromPolyTree(polygons, floorTypeId, levelId);
			if (list == null || list.Count == 0)
			{
				return new List<Floor>();
			}
			double num = thicknessMillimeters / 304.8;
			foreach (Floor item in list)
			{
				Parameter val = ((Element)item).get_Parameter((BuiltInParameter)(-1001951L));
				if (val != null)
				{
					val.Set(num);
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] 创建偏移楼板失败", ex);
			return new List<Floor>();
		}
	}

	private List<Floor> CreateFloorsFromPolygonsWithOffset(List<List<XYZ>> polygons, ElementId floorTypeId, ElementId levelId, double offsetMillimeters)
	{
		List<Floor> list = new List<Floor>();
		if (polygons == null || polygons.Count == 0)
		{
			return list;
		}
		List<Floor> list2 = CreateFloorFromPolyTreeWithOffset(polygons, floorTypeId, levelId, offsetMillimeters);
		if (list2 != null && list2.Count > 0)
		{
			list.AddRange(list2);
		}
		return list;
	}

	private List<CenterMarkingData> GenerateCenterMarkingCenterlines(List<CurveWithWidth> splitCurves, double filletRadiusMeters)
	{
		List<CenterMarkingData> list = new List<CenterMarkingData>();
		double filletRadiusFeet = filletRadiusMeters / 0.3048;
		for (int i = 0; i < splitCurves.Count; i++)
		{
			CurveWithWidth curveWithWidth = splitCurves[i];
			Curve curve = curveWithWidth.Curve;
			XYZ endPoint = curve.GetEndPoint(0);
			XYZ endPoint2 = curve.GetEndPoint(1);
			double length = curve.Length;
			if (length < 0.01)
			{
				continue;
			}
			double num = CalculateEndpointShrinkage(splitCurves, i, endPoint, isStartPoint: true, filletRadiusFeet);
			double num2 = CalculateEndpointShrinkage(splitCurves, i, endPoint2, isStartPoint: false, filletRadiusFeet);
			double num3 = 19.68503937007874;
			if (num < num3)
			{
				num = 0.0;
			}
			if (num2 < num3)
			{
				num2 = 0.0;
			}
			double num4 = num + num2;
			if (num4 >= length - 0.01)
			{
				continue;
			}
			try
			{
				double endParameter = curve.GetEndParameter(0);
				double endParameter2 = curve.GetEndParameter(1);
				double num5 = endParameter2 - endParameter;
				double num6 = num / length * num5;
				double num7 = num2 / length * num5;
				double num8 = endParameter + num6;
				double num9 = endParameter2 - num7;
				if (!(num9 - num8 < 0.001))
				{
					Curve val = curve.Clone();
					val.MakeBound(num8, num9);
					if (val.Length > 0.01)
					{
						list.Add(new CenterMarkingData
						{
							Centerline = val,
							StartShrinkFeet = num,
							EndShrinkFeet = num2,
							SplitCurveIndex = i,
							OriginalStartParam = num8,
							OriginalEndParam = num9
						});
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Warning("[RoadNetworkEngine] 标线中心线收缩失败: " + ex.Message);
			}
		}
		ILogger logger = _logger;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[RoadNetworkEngine] 生成 ");
		defaultInterpolatedStringHandler.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 条标线中心线");
		logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		return list;
	}

	private double CalculateEndpointShrinkage(List<CurveWithWidth> splitCurves, int currentIndex, XYZ endpoint, bool isStartPoint, double filletRadiusFeet)
	{
		double num = 0.01;
		double num2 = 0.0;
		Curve curve = splitCurves[currentIndex].Curve;
		XYZ inwardTangent = GetInwardTangent(curve, isStartPoint);
		double num3 = splitCurves[currentIndex].Width / 609.6;
		for (int i = 0; i < splitCurves.Count; i++)
		{
			if (i == currentIndex)
			{
				continue;
			}
			CurveWithWidth curveWithWidth = splitCurves[i];
			XYZ endPoint = curveWithWidth.Curve.GetEndPoint(0);
			XYZ endPoint2 = curveWithWidth.Curve.GetEndPoint(1);
			bool flag = endPoint.DistanceTo(endpoint) < num;
			bool flag2 = endPoint2.DistanceTo(endpoint) < num;
			if (!flag && !flag2)
			{
				continue;
			}
			bool isStartPoint2 = flag;
			XYZ inwardTangent2 = GetInwardTangent(curveWithWidth.Curve, isStartPoint2);
			double num4 = inwardTangent.AngleTo(inwardTangent2);
			if (!(num4 < Math.PI / 18.0) && !(num4 > Math.PI * 17.0 / 18.0))
			{
				double num5 = num3 + filletRadiusFeet;
				double num6 = curveWithWidth.Width / 609.6;
				double num7 = num6 + filletRadiusFeet;
				double num8 = num5 / Math.Tan(num4) + num7 / Math.Sin(num4);
				if (num8 > num2)
				{
					num2 = num8;
				}
			}
		}
		return num2;
	}

	private XYZ GetInwardTangent(Curve curve, bool isStartPoint)
	{
		if (isStartPoint)
		{
			return (curve.GetEndPoint(1) - curve.GetEndPoint(0)).Normalize();
		}
		return (curve.GetEndPoint(0) - curve.GetEndPoint(1)).Normalize();
	}

	private List<XYZ> GenerateMarkingPolygonFromCurve(Curve curve, double markingWidthMillimeters)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0038: Expected O, but got Unknown
		try
		{
			Path64 item = CurveToClipperPath(curve);
			double num = markingWidthMillimeters / 304.8;
			double num2 = num / 2.0 * 100000.0;
			Paths64 val = new Paths64();
			((List<Path64>)val).Add(item);
			Paths64 val2 = val;
			Paths64 val3 = Clipper.InflatePaths(val2, num2, (JoinType)0, (EndType)2, 5.0, GetClipperArcTolerance());
			if (val3 == null || ((List<Path64>)(object)val3).Count == 0)
			{
				return null;
			}
			return ClipperPathToXYZ(((List<Path64>)(object)val3)[0]);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private List<List<XYZ>> GenerateCrosswalkPolygons(List<CurveWithWidth> splitCurves, List<CenterMarkingData> markingDataList, double lineWidthMillimeters, double spacingMillimeters, double lengthMeters, double distanceFromMarkingMeters)
	{
		List<List<XYZ>> list = new List<List<XYZ>>();
		double num = distanceFromMarkingMeters / 0.3048;
		double num2 = lengthMeters / 0.3048;
		double halfLineWidthFeet = lineWidthMillimeters / 609.6;
		double spacingFeet = spacingMillimeters / 304.8;
		double num3 = num + num2;
		foreach (CenterMarkingData markingData in markingDataList)
		{
			int splitCurveIndex = markingData.SplitCurveIndex;
			Curve curve = splitCurves[splitCurveIndex].Curve;
			double width = splitCurves[splitCurveIndex].Width;
			int num4 = (int)Math.Floor(width / spacingMillimeters);
			if (num4 < 1)
			{
				num4 = 1;
			}
			if (markingData.StartShrinkFeet >= num3)
			{
				Curve val = ExtractCrosswalkBaselineOutward(curve, markingData.OriginalStartParam, isStart: true, num, num2);
				if ((GeometryObject)(object)val != (GeometryObject)null)
				{
					GenerateCrosswalkStripes(list, val, num4, spacingFeet, halfLineWidthFeet);
				}
			}
			if (markingData.EndShrinkFeet >= num3)
			{
				Curve val2 = ExtractCrosswalkBaselineOutward(curve, markingData.OriginalEndParam, isStart: false, num, num2);
				if ((GeometryObject)(object)val2 != (GeometryObject)null)
				{
					GenerateCrosswalkStripes(list, val2, num4, spacingFeet, halfLineWidthFeet);
				}
			}
		}
		ILogger logger = _logger;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[RoadNetworkEngine] 生成 ");
		defaultInterpolatedStringHandler.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 个人行横道多边形");
		logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		return list;
	}

	private Curve ExtractCrosswalkBaselineOutward(Curve originalCurve, double markingParam, bool isStart, double distanceFeet, double lengthFeet)
	{
		try
		{
			double endParameter = originalCurve.GetEndParameter(0);
			double endParameter2 = originalCurve.GetEndParameter(1);
			double num;
			double num2;
			if (isStart)
			{
				num = markingParam - distanceFeet / originalCurve.Length * (endParameter2 - endParameter);
				num2 = num - lengthFeet / originalCurve.Length * (endParameter2 - endParameter);
			}
			else
			{
				num2 = markingParam + distanceFeet / originalCurve.Length * (endParameter2 - endParameter);
				num = num2 + lengthFeet / originalCurve.Length * (endParameter2 - endParameter);
			}
			if (num - num2 < 0.001)
			{
				return null;
			}
			Curve val = originalCurve.Clone();
			val.MakeBound(num2, num);
			if (val.Length > 0.01)
			{
				return val;
			}
		}
		catch (Exception ex)
		{
			_logger.Warning("[RoadNetworkEngine] 提取人行横道基准线失败: " + ex.Message);
		}
		return null;
	}

	private void GenerateCrosswalkStripes(List<List<XYZ>> allPolygons, Curve baseline, int lineCount, double spacingFeet, double halfLineWidthFeet)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0087: Expected O, but got Unknown
		for (int i = 0; i < lineCount; i++)
		{
			double num = ((double)i - (double)(lineCount - 1) / 2.0) * spacingFeet;
			Curve val = ((!(Math.Abs(num) < 0.001)) ? OffsetCurve(baseline, Math.Abs(num), num > 0.0) : baseline.Clone());
			if ((GeometryObject)(object)val == (GeometryObject)null)
			{
				continue;
			}
			try
			{
				Path64 item = CurveToClipperPath(val);
				double num2 = halfLineWidthFeet * 100000.0;
				Paths64 val2 = new Paths64();
				((List<Path64>)val2).Add(item);
				Paths64 val3 = val2;
				Paths64 val4 = Clipper.InflatePaths(val3, num2, (JoinType)0, (EndType)2, 5.0, GetClipperArcTolerance());
				if (val4 != null && ((List<Path64>)(object)val4).Count > 0)
				{
					List<XYZ> list = ClipperPathToXYZ(((List<Path64>)(object)val4)[0]);
					if (list != null && list.Count >= 3)
					{
						allPolygons.Add(list);
					}
				}
			}
			catch
			{
			}
		}
	}

	private List<List<XYZ>> GenerateStopLinePolygons(List<CurveWithWidth> splitCurves, List<CenterMarkingData> markingDataList, double stopLineWidthMillimeters, double centerMarkingWidthMillimeters, bool isDoubleLine)
	{
		List<List<XYZ>> list = new List<List<XYZ>>();
		double halfStopLineWidthFeet = stopLineWidthMillimeters / 609.6;
		double num = (isDoubleLine ? (centerMarkingWidthMillimeters + 100.0) : (centerMarkingWidthMillimeters / 2.0));
		double leftExtensionFeet = num / 304.8;
		foreach (CenterMarkingData markingData in markingDataList)
		{
			Curve centerline = markingData.Centerline;
			if (!((GeometryObject)(object)centerline == (GeometryObject)null))
			{
				double width = splitCurves[markingData.SplitCurveIndex].Width;
				double halfRoadWidthFeet = width / 609.6;
				double num2 = 19.68503937007874;
				if (markingData.StartShrinkFeet >= num2)
				{
					GenerateSingleStopLine(list, centerline, isStart: true, halfRoadWidthFeet, leftExtensionFeet, halfStopLineWidthFeet);
				}
				if (markingData.EndShrinkFeet >= num2)
				{
					GenerateSingleStopLine(list, centerline, isStart: false, halfRoadWidthFeet, leftExtensionFeet, halfStopLineWidthFeet);
				}
			}
		}
		ILogger logger = _logger;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[RoadNetworkEngine] 生成 ");
		defaultInterpolatedStringHandler.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 条停止线");
		logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		return list;
	}

	private void GenerateSingleStopLine(List<List<XYZ>> allPolygons, Curve curve, bool isStart, double halfRoadWidthFeet, double leftExtensionFeet, double halfStopLineWidthFeet)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00eb: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_0108: Expected O, but got Unknown
		try
		{
			double num = (isStart ? curve.GetEndParameter(0) : curve.GetEndParameter(1));
			XYZ val = curve.Evaluate(num, false);
			XYZ val2 = curve.ComputeDerivatives(num, false).BasisX.Normalize();
			XYZ val3 = ((!isStart) ? val2 : (-val2));
			XYZ val4 = val3.CrossProduct(XYZ.BasisZ).Normalize();
			XYZ val5 = val + val3 * halfStopLineWidthFeet;
			XYZ val6 = val5 + val4 * (0.0 - leftExtensionFeet);
			XYZ val7 = val5 + val4 * halfRoadWidthFeet;
			Path64 val8 = new Path64();
			((List<Point64>)val8).Add(new Point64((long)(val6.X * 100000.0), (long)(val6.Y * 100000.0)));
			((List<Point64>)val8).Add(new Point64((long)(val7.X * 100000.0), (long)(val7.Y * 100000.0)));
			Path64 item = val8;
			double num2 = halfStopLineWidthFeet * 100000.0;
			Paths64 val9 = new Paths64();
			((List<Path64>)val9).Add(item);
			Paths64 val10 = val9;
			Paths64 val11 = Clipper.InflatePaths(val10, num2, (JoinType)0, (EndType)2, 5.0, GetClipperArcTolerance());
			if (val11 != null && ((List<Path64>)(object)val11).Count > 0)
			{
				List<XYZ> list = ClipperPathToXYZ(((List<Path64>)(object)val11)[0]);
				if (list != null && list.Count >= 3)
				{
					allPolygons.Add(list);
				}
			}
		}
		catch (Exception ex)
		{
			_logger.Warning("[RoadNetworkEngine] 停止线生成失败: " + ex.Message);
		}
	}

	private List<List<XYZ>> GenerateLaneMarkingPolygons(List<CurveWithWidth> splitCurves, List<CenterMarkingData> markingDataList, double laneWidthMillimeters, double markingWidthMillimeters, double solidLengthMeters, double gapLengthMeters)
	{
		List<List<XYZ>> list = new List<List<XYZ>>();
		double halfWidthFeet = markingWidthMillimeters / 609.6;
		double solidLengthFeet = solidLengthMeters / 0.3048;
		double gapLengthFeet = gapLengthMeters / 0.3048;
		foreach (CenterMarkingData markingData in markingDataList)
		{
			int splitCurveIndex = markingData.SplitCurveIndex;
			Curve centerline = markingData.Centerline;
			double width = splitCurves[splitCurveIndex].Width;
			double num = laneWidthMillimeters / 304.8;
			double num2 = width / 2.0 / 304.8;
			int num3 = (int)Math.Floor(num2 / num);
			if (num3 < 1)
			{
				num3 = 1;
			}
			double offsetFeet = (double)num3 * num;
			GenerateEdgeLinePolygons(list, centerline, offsetFeet, halfWidthFeet);
			for (int i = 1; i < num3; i++)
			{
				double offsetFeet2 = (double)i * num;
				GenerateDashedLaneMarking(list, centerline, offsetFeet2, solidLengthFeet, gapLengthFeet, halfWidthFeet);
			}
			if (markingData.StartShrinkFeet > 0.001)
			{
				for (int j = 1; j < num3; j++)
				{
					double offsetFeet3 = (double)j * num;
					GenerateStopLineDivider(list, centerline, markingData.StartShrinkFeet, offsetFeet3, halfWidthFeet, isStart: true);
				}
			}
			if (markingData.EndShrinkFeet > 0.001)
			{
				for (int k = 1; k < num3; k++)
				{
					double offsetFeet4 = (double)k * num;
					GenerateStopLineDivider(list, centerline, markingData.EndShrinkFeet, offsetFeet4, halfWidthFeet, isStart: false);
				}
			}
		}
		if (list.Count > 1)
		{
			List<List<XYZ>> list2 = UnionPolygonsMultiple(list);
			ILogger logger = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[RoadNetworkEngine] 车道分界线：");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral("个原始轮廓，合并为");
			defaultInterpolatedStringHandler.AppendFormatted(list2.Count);
			defaultInterpolatedStringHandler.AppendLiteral("个独立区域");
			logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return list2;
		}
		ILogger logger2 = _logger;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(33, 1);
		defaultInterpolatedStringHandler2.AppendLiteral("[RoadNetworkEngine] 生成 ");
		defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler2.AppendLiteral(" 个车道分界线多边形");
		logger2.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
		return list;
	}

	private List<List<XYZ>> UnionPolygonsMultiple(List<List<XYZ>> polygons)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (polygons.Count == 0)
			{
				return new List<List<XYZ>>();
			}
			if (polygons.Count == 1)
			{
				return polygons;
			}
			Paths64 val = new Paths64();
			foreach (List<XYZ> polygon in polygons)
			{
				Path64 val2 = new Path64();
				foreach (XYZ item in polygon)
				{
					long num = (long)(item.X * 100000.0);
					long num2 = (long)(item.Y * 100000.0);
					((List<Point64>)(object)val2).Add(new Point64(num, num2));
				}
				((List<Path64>)(object)val).Add(val2);
			}
			Paths64 val3 = Clipper.Union(val, (FillRule)1);
			if (val3 == null || ((List<Path64>)(object)val3).Count == 0)
			{
				return polygons;
			}
			List<List<XYZ>> list = new List<List<XYZ>>();
			foreach (Path64 item2 in (List<Path64>)(object)val3)
			{
				List<XYZ> list2 = ClipperPathToXYZ(item2);
				if (list2 != null && list2.Count >= 3)
				{
					list.Add(list2);
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] 多边形布尔并集失败", ex);
			return polygons;
		}
	}

	private void GenerateEdgeLinePolygons(List<List<XYZ>> allPolygons, Curve centerline, double offsetFeet, double halfWidthFeet)
	{
		Curve val = OffsetCurve(centerline, offsetFeet, leftSide: false);
		if ((GeometryObject)(object)val != (GeometryObject)null)
		{
			List<XYZ> list = GenerateMarkingPolygonFromCurve(val, halfWidthFeet * 2.0 * 304.8);
			if (list != null && list.Count >= 3)
			{
				allPolygons.Add(list);
			}
		}
		Curve val2 = OffsetCurve(centerline, offsetFeet, leftSide: true);
		if ((GeometryObject)(object)val2 != (GeometryObject)null)
		{
			List<XYZ> list2 = GenerateMarkingPolygonFromCurve(val2, halfWidthFeet * 2.0 * 304.8);
			if (list2 != null && list2.Count >= 3)
			{
				allPolygons.Add(list2);
			}
		}
	}

	private void GenerateDashedLaneMarking(List<List<XYZ>> allPolygons, Curve centerline, double offsetFeet, double solidLengthFeet, double gapLengthFeet, double halfWidthFeet)
	{
		Curve val = OffsetCurve(centerline, offsetFeet, leftSide: false);
		if ((GeometryObject)(object)val == (GeometryObject)null)
		{
			return;
		}
		double num = solidLengthFeet + gapLengthFeet;
		double length = val.Length;
		for (double num2 = 0.0; num2 + solidLengthFeet <= length + 0.001; num2 += num)
		{
			double distance = num2;
			double val2 = num2 + solidLengthFeet;
			double num3 = ComputeParameterAtDistance(val, distance);
			double num4 = ComputeParameterAtDistance(val, Math.Min(val2, length));
			double endParameter = val.GetEndParameter(1);
			if (!(num4 > num3) || !(num4 <= endParameter + 0.001))
			{
				continue;
			}
			try
			{
				Curve val3 = val.Clone();
				val3.MakeBound(num3, num4);
				if (val3.Length > 0.01)
				{
					List<XYZ> list = GenerateMarkingPolygonFromCurve(val3, halfWidthFeet * 2.0 * 304.8);
					if (list != null && list.Count >= 3)
					{
						allPolygons.Add(list);
					}
				}
			}
			catch
			{
			}
		}
		Curve val4 = OffsetCurve(centerline, offsetFeet, leftSide: true);
		if ((GeometryObject)(object)val4 == (GeometryObject)null)
		{
			return;
		}
		length = val4.Length;
		for (double num2 = 0.0; num2 + solidLengthFeet <= length + 0.001; num2 += num)
		{
			double distance2 = num2;
			double val5 = num2 + solidLengthFeet;
			double num5 = ComputeParameterAtDistance(val4, distance2);
			double num6 = ComputeParameterAtDistance(val4, Math.Min(val5, length));
			double endParameter2 = val4.GetEndParameter(1);
			if (!(num6 > num5) || !(num6 <= endParameter2 + 0.001))
			{
				continue;
			}
			try
			{
				Curve val6 = val4.Clone();
				val6.MakeBound(num5, num6);
				if (val6.Length > 0.01)
				{
					List<XYZ> list2 = GenerateMarkingPolygonFromCurve(val6, halfWidthFeet * 2.0 * 304.8);
					if (list2 != null && list2.Count >= 3)
					{
						allPolygons.Add(list2);
					}
				}
			}
			catch
			{
			}
		}
	}

	private void GenerateStopLineDivider(List<List<XYZ>> allPolygons, Curve centerline, double shrinkFeet, double offsetFeet, double halfWidthFeet, bool isStart)
	{
		double val = 65.61679790026247;
		double length = centerline.Length;
		double endParameter = centerline.GetEndParameter(0);
		double endParameter2 = centerline.GetEndParameter(1);
		double num = Math.Min(val, length);
		if (num <= 0.001)
		{
			return;
		}
		double num2;
		double num3;
		if (isStart)
		{
			num2 = endParameter;
			num3 = ComputeParameterAtDistance(centerline, num);
		}
		else
		{
			num3 = endParameter2;
			num2 = ComputeParameterAtDistance(centerline, length - num);
		}
		if (num2 < endParameter || num3 > endParameter2 || num3 <= num2)
		{
			return;
		}
		try
		{
			Curve val2 = centerline.Clone();
			val2.MakeBound(num2, num3);
			if (val2.Length < 0.01)
			{
				return;
			}
			bool leftSide = !isStart;
			Curve val3 = OffsetCurve(val2, offsetFeet, leftSide);
			if ((GeometryObject)(object)val3 != (GeometryObject)null)
			{
				List<XYZ> list = GenerateMarkingPolygonFromCurve(val3, halfWidthFeet * 2.0 * 304.8);
				if (list != null && list.Count >= 3)
				{
					allPolygons.Add(list);
				}
			}
		}
		catch
		{
		}
	}

	private double ComputeParameterAtDistance(Curve curve, double distance)
	{
		double length = curve.Length;
		if (length <= 0.001)
		{
			return curve.GetEndParameter(0);
		}
		distance = Math.Max(0.0, Math.Min(distance, length));
		double num = distance / length;
		double endParameter = curve.GetEndParameter(0);
		double endParameter2 = curve.GetEndParameter(1);
		return endParameter + (endParameter2 - endParameter) * num;
	}

	private XYZ GetFirstPointOfCurveLoop(CurveLoop loop)
	{
		using (IEnumerator<Curve> enumerator = loop.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				Curve current = enumerator.Current;
				return current.GetEndPoint(0);
			}
		}
		return null;
	}

	private bool IsPointInsideCurveLoop(CurveLoop loop, XYZ point)
	{
		try
		{
			List<XYZ> list = new List<XYZ>();
			foreach (Curve item in loop)
			{
				list.Add(item.GetEndPoint(0));
			}
			int count = list.Count;
			if (count < 3)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				XYZ val = list[i];
				XYZ val2 = list[(i + 1) % count];
				if (val.Y > point.Y != val2.Y > point.Y)
				{
					double num2 = val.X + (point.Y - val.Y) / (val2.Y - val.Y) * (val2.X - val.X);
					if (point.X < num2)
					{
						num++;
					}
				}
			}
			return num % 2 == 1;
		}
		catch
		{
			return false;
		}
	}

	private CurveLoop EnsureCurveLoopDirection(CurveLoop loop, bool counterClockwise)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		try
		{
			if (loop == null)
			{
				return loop;
			}
			double num = CalculateCurveLoopArea(loop);
			bool flag = num > 0.0;
			bool flag2 = false;
			if (counterClockwise && !flag)
			{
				flag2 = true;
			}
			else if (!counterClockwise & flag)
			{
				flag2 = true;
			}
			if (!flag2)
			{
				return loop;
			}
			List<Curve> list = new List<Curve>();
			foreach (Curve item in loop)
			{
				list.Add(item.CreateReversed());
			}
			CurveLoop val = new CurveLoop();
			for (int num2 = list.Count - 1; num2 >= 0; num2--)
			{
				val.Append(list[num2]);
			}
			return val;
		}
		catch (Exception)
		{
			return loop;
		}
	}

	private double PolygonArea(List<XYZ> polygon)
	{
		try
		{
			if (polygon == null || polygon.Count < 3)
			{
				return 0.0;
			}
			double num = 0.0;
			for (int i = 0; i < polygon.Count; i++)
			{
				int index = (i + 1) % polygon.Count;
				num += polygon[i].X * polygon[index].Y;
				num -= polygon[i].Y * polygon[index].X;
			}
			return num / 2.0;
		}
		catch
		{
			return 0.0;
		}
	}

	private double CalculateCurveLoopArea(CurveLoop loop)
	{
		try
		{
			if (loop == null)
			{
				return 0.0;
			}
			int num = 0;
			List<XYZ> list = new List<XYZ>();
			foreach (Curve item in loop)
			{
				list.Add(item.GetEndPoint(0));
				num++;
			}
			if (num < 3)
			{
				return 0.0;
			}
			double num2 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				int index = (i + 1) % list.Count;
				num2 += list[i].X * list[index].Y;
				num2 -= list[i].Y * list[index].X;
			}
			return num2 / 2.0;
		}
		catch (Exception)
		{
			return 0.0;
		}
	}

	private List<Floor> CreateSiteFloor(List<List<XYZ>> subtractPolygons, double extensionMeters, ElementId floorTypeId, ElementId levelId, double offsetMillimeters = 0.0)
	{
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected O, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Expected O, but got Unknown
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Expected O, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Expected O, but got Unknown
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Expected O, but got Unknown
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Expected O, but got Unknown
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Expected O, but got Unknown
		try
		{
			if (subtractPolygons == null || subtractPolygons.Count == 0)
			{
				_logger.Warning("[RoadNetworkEngine] 无法创建场地楼板：没有减去多边形");
				return new List<Floor>();
			}
			double num = double.MaxValue;
			double num2 = double.MaxValue;
			double num3 = double.MinValue;
			double num4 = double.MinValue;
			foreach (List<XYZ> subtractPolygon in subtractPolygons)
			{
				foreach (XYZ item in subtractPolygon)
				{
					if (item.X < num)
					{
						num = item.X;
					}
					if (item.Y < num2)
					{
						num2 = item.Y;
					}
					if (item.X > num3)
					{
						num3 = item.X;
					}
					if (item.Y > num4)
					{
						num4 = item.Y;
					}
				}
			}
			double num5 = extensionMeters / 0.3048;
			num -= num5;
			num2 -= num5;
			num3 += num5;
			num4 += num5;
			List<XYZ> list = new List<XYZ>
			{
				new XYZ(num, num2, 0.0),
				new XYZ(num3, num2, 0.0),
				new XYZ(num3, num4, 0.0),
				new XYZ(num, num4, 0.0)
			};
			Paths64 val = new Paths64();
			Path64 val2 = new Path64();
			foreach (XYZ item2 in list)
			{
				((List<Point64>)(object)val2).Add(new Point64((long)(item2.X * 100000.0), (long)(item2.Y * 100000.0)));
			}
			val2 = Clipper.StripDuplicates(val2, true);
			if (((List<Point64>)(object)val2).Count >= 3)
			{
				((List<Path64>)(object)val).Add(val2);
			}
			Paths64 val3 = PolygonsToClipperPaths(subtractPolygons);
			PolyTree64 val4 = new PolyTree64();
			Clipper64 val5 = new Clipper64();
			val5.AddSubject(val);
			val5.AddClip(val3);
			val5.Execute((ClipType)3, (FillRule)1, val4);
			if (((PolyPathBase)val4).Count == 0)
			{
				_logger.Warning("[RoadNetworkEngine] 布尔差集结果为空，无法创建场地楼板");
				return new List<Floor>();
			}
			Paths64 val6 = ClipperPolyTreeToPaths(val4);
			List<List<XYZ>> list2 = new List<List<XYZ>>();
			foreach (Path64 item3 in (List<Path64>)(object)val6)
			{
				List<XYZ> list3 = new List<XYZ>();
				foreach (Point64 item4 in (List<Point64>)(object)item3)
				{
					list3.Add(new XYZ((double)item4.X / 100000.0, (double)item4.Y / 100000.0, 0.0));
				}
				if (list3.Count >= 3)
				{
					list2.Add(list3);
				}
			}
			if (list2.Count == 0)
			{
				_logger.Warning("[RoadNetworkEngine] 转换后的场地楼板多边形为空");
				return new List<Floor>();
			}
			return CreateFloorFromPolyTreeWithOffset(list2, floorTypeId, levelId, offsetMillimeters);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] 场地楼板创建失败", ex);
			return new List<Floor>();
		}
	}

	private bool IsSamePoint(XYZ p1, XYZ p2)
	{
		return Math.Abs(p1.X - p2.X) < 0.001 && Math.Abs(p1.Y - p2.Y) < 0.001 && Math.Abs(p1.Z - p2.Z) < 0.001;
	}

	private Floor CreateFloorFromCurveLoops(List<CurveLoop> curveLoops, ElementId floorTypeId, ElementId levelId)
	{
		try
		{
			if (curveLoops == null || curveLoops.Count == 0)
			{
				return null;
			}
			foreach (CurveLoop curveLoop in curveLoops)
			{
				if (!IsCurveLoopClosed(curveLoop))
				{
					return null;
				}
			}
			return Floor.Create(_document, (IList<CurveLoop>)curveLoops, floorTypeId, levelId);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] 创建楼板失败", ex);
			return null;
		}
	}

	private bool IsCurveLoopClosed(CurveLoop loop)
	{
		try
		{
			if (loop == null)
			{
				return false;
			}
			int num = 0;
			Curve val = null;
			Curve val2 = null;
			foreach (Curve item in loop)
			{
				num++;
				if ((GeometryObject)(object)val == (GeometryObject)null)
				{
					val = item;
				}
				val2 = item;
			}
			if (num < 2 || (GeometryObject)(object)val == (GeometryObject)null || (GeometryObject)(object)val2 == (GeometryObject)null)
			{
				return false;
			}
			XYZ endPoint = val.GetEndPoint(0);
			XYZ endPoint2 = val2.GetEndPoint(1);
			return endPoint.DistanceTo(endPoint2) < 0.001;
		}
		catch
		{
			return false;
		}
	}

	private (int StartIndex, int Count) TryExtractArcPoints(List<XYZ> points, int startIndex)
	{
		if (points.Count < 3)
		{
			return (StartIndex: 0, Count: 0);
		}
		double num = 0.1;
		int num2 = 50;
		int num3 = 3;
		while (num3 <= num2 && startIndex + num3 < points.Count + startIndex)
		{
			int index = (startIndex + num3 - 2) % points.Count;
			int index2 = (startIndex + num3 - 1) % points.Count;
			int index3 = (startIndex + num3) % points.Count;
			XYZ val = points[index];
			XYZ val2 = points[index2];
			XYZ val3 = points[index3];
			XYZ val4 = (val2 - val).CrossProduct(val3 - val);
			if (val4.Z < 0.001)
			{
				break;
			}
			try
			{
				Arc val5 = Arc.Create(val, val2, val3);
				XYZ center = val5.Center;
				double radius = val5.Radius;
				if (!(Math.Abs(val3.DistanceTo(center) - radius) < num))
				{
					break;
				}
				num3++;
				continue;
			}
			catch
			{
			}
			break;
		}
		if (num3 >= 3)
		{
			return (StartIndex: startIndex, Count: num3);
		}
		return (StartIndex: 0, Count: 0);
	}

	private Curve FlattenCurveZ(Curve curve)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Expected O, but got Unknown
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Expected O, but got Unknown
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Expected O, but got Unknown
		if ((GeometryObject)(object)curve == (GeometryObject)null)
		{
			return null;
		}
		try
		{
			Line val = (Line)(object)((curve is Line) ? curve : null);
			if (val != null)
			{
				XYZ val2 = new XYZ(((Curve)val).GetEndPoint(0).X, ((Curve)val).GetEndPoint(0).Y, 0.0);
				XYZ val3 = new XYZ(((Curve)val).GetEndPoint(1).X, ((Curve)val).GetEndPoint(1).Y, 0.0);
				if (val2.DistanceTo(val3) < 0.001)
				{
					return null;
				}
				return (Curve)(object)Line.CreateBound(val2, val3);
			}
			Arc val4 = (Arc)(object)((curve is Arc) ? curve : null);
			if (val4 != null)
			{
				double radius = val4.Radius;
				XYZ val5 = new XYZ(val4.Center.X, val4.Center.Y, 0.0);
				XYZ val6 = new XYZ(((Curve)val4).GetEndPoint(0).X, ((Curve)val4).GetEndPoint(0).Y, 0.0);
				XYZ val7 = new XYZ(((Curve)val4).GetEndPoint(1).X, ((Curve)val4).GetEndPoint(1).Y, 0.0);
				if (val6.DistanceTo(val5) < 0.001 || val7.DistanceTo(val5) < 0.001)
				{
					return (Curve)(object)((val6.DistanceTo(val7) > 0.1) ? Line.CreateBound(val6, val7) : null);
				}
				XYZ val8 = (val6 - val5).Normalize();
				XYZ val9 = (val7 - val5).Normalize();
				double num = val8.AngleTo(val9);
				if (num < 0.001)
				{
					num = 0.0;
				}
				bool flag = val4.Normal.Z >= 0.0;
				XYZ val10 = val8;
				XYZ val11 = (flag ? new XYZ(0.0 - val8.Y, val8.X, 0.0).Normalize() : new XYZ(val8.Y, 0.0 - val8.X, 0.0).Normalize());
				XYZ val12 = ((Curve)val4).Evaluate(0.5 * (((Curve)val4).GetEndParameter(0) + ((Curve)val4).GetEndParameter(1)), false);
				XYZ val13 = new XYZ(val12.X, val12.Y, 0.0);
				Arc val14 = Arc.Create(val5, radius, 0.0, num, val10, val11);
				XYZ val15 = ((Curve)val14).Evaluate(0.5 * num, false);
				if (val15.DistanceTo(val13) > 0.01)
				{
					double num2 = Math.PI * 2.0 - num;
					val14 = Arc.Create(val5, radius, 0.0, num2, val10, val11);
				}
				return (Curve)(object)val14;
			}
			IList<XYZ> list = curve.Tessellate();
			if (list != null && list.Count >= 2)
			{
				XYZ val16 = new XYZ(list[0].X, list[0].Y, 0.0);
				XYZ val17 = new XYZ(list[list.Count - 1].X, list[list.Count - 1].Y, 0.0);
				if (val16.DistanceTo(val17) > 0.1)
				{
					return (Curve)(object)Line.CreateBound(val16, val17);
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] Z轴归零异常: " + ex.Message, (Exception)null);
			return null;
		}
	}

	private List<CurveWithWidth> ExtractWallCenterlines(List<Wall> walls, double defaultRoadWidthMillimeters)
	{
		List<CurveWithWidth> list = new List<CurveWithWidth>();
		foreach (Wall wall in walls)
		{
			try
			{
				Location location = ((Element)wall).Location;
				LocationCurve val = (LocationCurve)(object)((location is LocationCurve) ? location : null);
				if ((GeometryObject)(object)((val != null) ? val.Curve : null) != (GeometryObject)null)
				{
					double num = ExtractWidthFromWallComment(wall);
					if (num <= 0.0)
					{
						num = defaultRoadWidthMillimeters;
					}
					Curve curve = val.Curve.Clone();
					curve = FlattenCurveZ(curve);
					if (!((GeometryObject)(object)curve == (GeometryObject)null))
					{
						list.Add(new CurveWithWidth
						{
							Curve = curve,
							Width = num
						});
					}
				}
			}
			catch (Exception)
			{
			}
		}
		return list;
	}

	private double ExtractWidthFromWallComment(Wall wall)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		try
		{
			Parameter val = null;
			foreach (Parameter parameter in ((Element)wall).Parameters)
			{
				Parameter val2 = parameter;
				if (val2 == null || !val2.HasValue)
				{
					continue;
				}
				try
				{
					Definition definition = val2.Definition;
					if (definition != null)
					{
						string name = definition.Name;
						if (name.Equals("注释", StringComparison.OrdinalIgnoreCase) || name.Equals("Comments", StringComparison.OrdinalIgnoreCase))
						{
							val = val2;
							break;
						}
					}
				}
				catch
				{
				}
			}
			if (val == null)
			{
				val = ((Element)wall).get_Parameter((BuiltInParameter)(-1001203L));
			}
			if (val == null || string.IsNullOrWhiteSpace(val.AsString()))
			{
				return 0.0;
			}
			string input = val.AsString().Trim();
			Match match = Regex.Match(input, "[\\d.]+");
			if (!match.Success)
			{
				return 0.0;
			}
			if (!double.TryParse(match.Value, out var result))
			{
				return 0.0;
			}
			return (!(result > 1000.0)) ? (result * 1000.0) : result;
		}
		catch (Exception)
		{
			return 0.0;
		}
	}

	private List<XYZ> SmartDeduplicatePoints(List<XYZ> polygonPoints, double fitTolerance)
	{
		List<double> list = new List<double>();
		for (int i = 0; i < polygonPoints.Count - 1; i++)
		{
			double item = polygonPoints[i].DistanceTo(polygonPoints[i + 1]);
			list.Add(item);
		}
		if (list.Count == 0)
		{
			return polygonPoints;
		}
		HashSet<int> hashSet = new HashSet<int>
		{
			0,
			polygonPoints.Count - 1
		};
		for (int j = 1; j < list.Count; j++)
		{
			double num = Math.Abs(list[j] - list[j - 1]) / list[j - 1];
			if (num > 0.05)
			{
				hashSet.Add(j);
				hashSet.Add(j + 1);
			}
		}
		List<XYZ> list2 = new List<XYZ>();
		for (int k = 0; k < polygonPoints.Count; k++)
		{
			XYZ val = polygonPoints[k];
			if (list2.Count == 0 || val.DistanceTo(list2[list2.Count - 1]) > fitTolerance)
			{
				list2.Add(val);
			}
		}
		if (list2.Count > 1 && list2[list2.Count - 1].DistanceTo(list2[0]) <= fitTolerance)
		{
			list2.RemoveAt(list2.Count - 1);
		}
		return list2;
	}

	private List<Curve> FitClipperPolygonToCurves(List<XYZ> polygonPoints)
	{
		if (polygonPoints.Count < 3)
		{
			return new List<Curve>();
		}
		double num = 0.0032808398950131233;
		List<XYZ> list = SmartDeduplicatePoints(polygonPoints, num);
		if (list.Count < 3)
		{
			return new List<Curve>();
		}
		List<double> list2 = new List<double>();
		for (int i = 0; i < list.Count - 1; i++)
		{
			list2.Add(list[i].DistanceTo(list[i + 1]));
		}
		if (list2.Count == 0)
		{
			return new List<Curve>();
		}
		List<List<int>> list3 = new List<List<int>>();
		List<int> list4 = new List<int> { 0 };
		List<double> list5 = new List<double> { list2[0] };
		for (int j = 0; j < list2.Count; j++)
		{
			double num2 = list2[j];
			bool flag = false;
			if (list5.Count > 0)
			{
				double num3 = list5.Average();
				double num4 = Math.Abs(num2 - num3) / num3;
				if (num4 > 0.05)
				{
					flag = true;
				}
			}
			if (flag)
			{
				list3.Add(list4);
				list4 = new List<int>
				{
					j,
					j + 1
				};
				list5 = new List<double> { num2 };
			}
			else
			{
				list4.Add(j + 1);
				list5.Add(num2);
			}
		}
		if (list4.Count > 0)
		{
			list3.Add(list4);
		}
		List<Curve> list6 = new List<Curve>();
		for (int k = 0; k < list3.Count; k++)
		{
			List<int> list7 = list3[k];
			XYZ val = list[list7[0]];
			XYZ val2 = list[list7[list7.Count - 1]];
			if (list7.Count == 2)
			{
				if (val.DistanceTo(val2) > num)
				{
					list6.Add((Curve)(object)Line.CreateBound(val, val2));
				}
				continue;
			}
			int index = list7[list7.Count / 2];
			XYZ mid = list[index];
			Arc val3 = CreateArcFromThreePoints(val, mid, val2);
			if ((GeometryObject)(object)val3 != (GeometryObject)null && ((Curve)val3).Length > num)
			{
				list6.Add((Curve)(object)val3);
			}
			else if (val.DistanceTo(val2) > num)
			{
				list6.Add((Curve)(object)Line.CreateBound(val, val2));
			}
		}
		if (list6.Count > 0)
		{
			XYZ endPoint = list6[list6.Count - 1].GetEndPoint(1);
			XYZ endPoint2 = list6[0].GetEndPoint(0);
			if (endPoint.DistanceTo(endPoint2) > num)
			{
				list6.Add((Curve)(object)Line.CreateBound(endPoint, endPoint2));
			}
		}
		return list6;
	}

	private List<Curve> FitClipperPolygonToCurvesArcThenNurbs(List<XYZ> polygonPoints)
	{
		double num = 0.0032808398950131233;
		List<XYZ> list = SmartDeduplicatePoints(polygonPoints, num);
		if (list.Count < 3)
		{
			return new List<Curve>();
		}
		List<int> list2 = DetectCornerPoints(list);
		List<List<XYZ>> list3 = new List<List<XYZ>>();
		for (int i = 0; i < list2.Count; i++)
		{
			int num2 = list2[i];
			int num3 = list2[(i + 1) % list2.Count];
			List<XYZ> list4 = new List<XYZ>();
			int num4 = num2;
			while (true)
			{
				list4.Add(list[num4]);
				if (num4 == num3)
				{
					break;
				}
				num4 = (num4 + 1) % list.Count;
			}
			if (list4.Count >= 2)
			{
				list3.Add(list4);
			}
		}
		List<Curve> list5 = new List<Curve>();
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		foreach (List<XYZ> item in list3)
		{
			List<double> list6 = new List<double>();
			for (int j = 0; j < item.Count - 1; j++)
			{
				list6.Add(item[j].DistanceTo(item[j + 1]));
			}
			if (list6.Count == 0)
			{
				continue;
			}
			list6.Average();
			list6.OrderBy((double d) => d).ElementAt(list6.Count / 2);
			list6.Max();
			list6.Min();
			List<double> list7 = new List<double>();
			for (int num8 = 0; num8 < list6.Count - 1; num8++)
			{
				list7.Add(Math.Abs(list6[num8 + 1] - list6[num8]));
			}
			double num9 = 1.3123359580052494;
			List<List<XYZ>> list8 = new List<List<XYZ>>();
			List<XYZ> list9 = new List<XYZ> { item[0] };
			for (int num10 = 0; num10 < list6.Count; num10++)
			{
				list9.Add(item[num10 + 1]);
				bool flag = false;
				if (num10 < list7.Count && list7[num10] > num9)
				{
					flag = true;
				}
				if (flag && list9.Count >= 2)
				{
					list8.Add(list9);
					list9 = new List<XYZ> { item[num10 + 1] };
				}
			}
			if (list9.Count >= 2)
			{
				list8.Add(list9);
			}
			foreach (List<XYZ> item2 in list8)
			{
				if (item2.Count < 2)
				{
					continue;
				}
				XYZ val = item2[0];
				XYZ val2 = item2[item2.Count - 1];
				List<double> list10 = new List<double>();
				for (int num11 = 0; num11 < item2.Count - 1; num11++)
				{
					list10.Add(item2[num11].DistanceTo(item2[num11 + 1]));
				}
				if (list10.Count > 0)
				{
					list10.Average();
				}
				else
					_ = 0.0;
				double num12 = list10.Sum();
				double num13 = val.DistanceTo(val2);
				double num14 = Math.Abs(num12 - num13) / num13;
				bool flag2 = num14 < 0.005;
				if ((item2.Count == 2) | flag2)
				{
					if (val.DistanceTo(val2) > 0.01)
					{
						list5.Add((Curve)(object)Line.CreateBound(val, val2));
						num7++;
					}
				}
				else
				{
					if (item2.Count < 3)
					{
						continue;
					}
					int index = item2.Count / 2;
					XYZ mid = item2[index];
					Arc val3 = CreateArcFromThreePoints(val, mid, val2);
					double num15 = double.MaxValue;
					if ((GeometryObject)(object)val3 != (GeometryObject)null && ((Curve)val3).Length > num)
					{
						num15 = ComputeCurveFittingError(item2, (Curve)(object)val3);
					}
					Curve val4 = null;
					double num16 = double.MaxValue;
					if (item2.Count >= 4)
					{
						val4 = FitPointsWithNurbsSpline(item2);
						if ((GeometryObject)(object)val4 != (GeometryObject)null)
						{
							num16 = ComputeCurveFittingError(item2, val4);
						}
					}
					double num17 = Math.Abs(num15 - num16);
					double num18 = 0.00032808398950131233;
					if ((GeometryObject)(object)val3 != (GeometryObject)null && num15 < double.MaxValue && ((GeometryObject)(object)val4 == (GeometryObject)null || num16 >= double.MaxValue || num15 < num16 - num18 || num17 <= num18))
					{
						if (((Curve)val3).Length > 0.01)
						{
							list5.Add((Curve)(object)val3);
							num5++;
						}
						else if (val.DistanceTo(val2) > 0.01)
						{
							list5.Add((Curve)(object)Line.CreateBound(val, val2));
							num7++;
						}
					}
					else if ((GeometryObject)(object)val4 != (GeometryObject)null && num16 < double.MaxValue)
					{
						if (val4.Length > 0.01)
						{
							list5.Add(val4);
							num6++;
						}
						else if (val.DistanceTo(val2) > 0.01)
						{
							list5.Add((Curve)(object)Line.CreateBound(val, val2));
							num7++;
						}
					}
					else if (val.DistanceTo(val2) > 0.01)
					{
						list5.Add((Curve)(object)Line.CreateBound(val, val2));
						num7++;
					}
				}
			}
		}
		if (list5.Count > 0)
		{
			XYZ endPoint = list5[list5.Count - 1].GetEndPoint(1);
			XYZ endPoint2 = list5[0].GetEndPoint(0);
			double num19 = endPoint.DistanceTo(endPoint2);
			if (num19 > 0.01)
			{
				list5.Add((Curve)(object)Line.CreateBound(endPoint, endPoint2));
			}
		}
		return list5;
	}

	private List<int> DetectCornerPoints(List<XYZ> points)
	{
		List<int> list = new List<int>();
		if (points.Count < 3)
		{
			return list;
		}
		double num = Math.PI / 12.0;
		for (int i = 0; i < points.Count; i++)
		{
			XYZ val = points[(i - 1 + points.Count) % points.Count];
			XYZ val2 = points[i];
			XYZ val3 = points[(i + 1) % points.Count];
			XYZ val4 = (val2 - val).Normalize();
			XYZ val5 = (val3 - val2).Normalize();
			double num2 = val4.AngleTo(val5);
			if (num2 > num)
			{
				list.Add(i);
			}
		}
		if (list.Count == 0)
		{
			int num3 = Math.Max(4, points.Count / 100);
			for (int j = 0; j < num3; j++)
			{
				int item = j * points.Count / num3;
				list.Add(item);
			}
		}
		return list;
	}

	private Curve FitPointsWithNurbsSpline(List<XYZ> points, int degree = 3)
	{
		if (points == null || points.Count < degree + 1)
		{
			return null;
		}
		try
		{
			List<XYZ> list = SimplifyPointsForNurbs(points, 125.0 / 381.0);
			if (list.Count < degree + 1)
			{
				return null;
			}
			if (list.Count >= 4)
			{
				return (Curve)(object)HermiteSpline.Create((IList<XYZ>)list, false);
			}
			return null;
		}
		catch (Exception ex)
		{
			_logger.Warning("[FitPointsWithNurbsSpline] 创建失败: " + ex.Message);
			return null;
		}
	}

	private List<XYZ> SimplifyPointsForNurbs(List<XYZ> points, double minDistance)
	{
		List<XYZ> list = new List<XYZ>();
		if (points.Count == 0)
		{
			return list;
		}
		list.Add(points[0]);
		for (int i = 1; i < points.Count; i++)
		{
			if (points[i].DistanceTo(list[list.Count - 1]) >= minDistance)
			{
				list.Add(points[i]);
			}
		}
		if (list.Count > 0 && list[list.Count - 1].DistanceTo(points[points.Count - 1]) > 0.001)
		{
			list.Add(points[points.Count - 1]);
		}
		return list;
	}

	private double ComputeCurveFittingError(List<XYZ> points, Curve curve)
	{
		if (points == null || (GeometryObject)(object)curve == (GeometryObject)null || points.Count < 2)
		{
			return double.MaxValue;
		}
		try
		{
			double num = 0.0;
			int num2 = 0;
			foreach (XYZ point in points)
			{
				IntersectionResult val = curve.Project(point);
				if (val != null)
				{
					double num3 = point.DistanceTo(val.XYZPoint);
					num += num3;
					num2++;
				}
			}
			if (num2 == 0)
			{
				return double.MaxValue;
			}
			return num / (double)num2;
		}
		catch (Exception ex)
		{
			_logger.Warning("[ComputeCurveFittingError] 计算失败: " + ex.Message);
			return double.MaxValue;
		}
	}

	private CurveLoop FitPolygonToCurveLoop(List<XYZ> polygonPoints)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		try
		{
			List<Curve> list = FitClipperPolygonToCurvesArcThenNurbs(polygonPoints);
			if (list == null || list.Count < 3)
			{
				return null;
			}
			CurveLoop val = new CurveLoop();
			XYZ val2 = null;
			foreach (Curve item in list)
			{
				Curve val3 = item;
				if (val2 != null)
				{
					double num = val2.DistanceTo(item.GetEndPoint(0));
					if (num > 1E-06)
					{
						Line val4 = (Line)(object)((item is Line) ? item : null);
						if (val4 != null)
						{
							val3 = (Curve)(object)Line.CreateBound(val2, ((Curve)val4).GetEndPoint(1));
						}
						else
						{
							Line val5 = Line.CreateBound(val2, item.GetEndPoint(0));
							val.Append((Curve)(object)val5);
							val2 = ((Curve)val5).GetEndPoint(1);
						}
					}
				}
				val.Append(val3);
				val2 = val3.GetEndPoint(1);
			}
			return val;
		}
		catch (Exception ex)
		{
			_logger.Warning("[RoadNetworkEngine] 拟合多边形为CurveLoop失败: " + ex.Message);
			return null;
		}
	}

	private Arc CreateArcFromThreePoints(XYZ start, XYZ mid, XYZ end)
	{
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			XYZ val = ComputeCircleCenter(start, mid, end);
			if (val == null)
			{
				return null;
			}
			double num = start.DistanceTo(val);
			if (num < 0.001 || num > 30000.0)
			{
				return null;
			}
			XYZ val2 = (start - val).Normalize();
			XYZ val3 = (end - val).Normalize();
			double num2 = val2.AngleTo(val3);
			if (num2 < 0.001)
			{
				num2 = 0.0;
			}
			XYZ val4 = val2.CrossProduct(val3);
			bool flag = val4.Z >= 0.0;
			XYZ val5 = val2;
			XYZ val6 = ((!flag) ? new XYZ(val2.Y, 0.0 - val2.X, 0.0).Normalize() : new XYZ(0.0 - val2.Y, val2.X, 0.0).Normalize());
			Arc val7 = Arc.Create(val, num, 0.0, num2, val5, val6);
			XYZ val8 = ((Curve)val7).Evaluate(0.5 * num2, false);
			double num3 = val8.DistanceTo(mid);
			Arc val9;
			if (num3 > 0.01)
			{
				double num4 = Math.PI * 2.0 - num2;
				val9 = Arc.Create(val, num, 0.0, num4, val5, val6);
				XYZ val10 = ((Curve)val9).Evaluate(0.5 * num4, false);
				double num5 = val10.DistanceTo(mid);
				if (num5 > num3)
				{
					val9 = val7;
				}
			}
			else
			{
				val9 = val7;
			}
			XYZ endPoint = ((Curve)val9).GetEndPoint(1);
			double num6 = endPoint.DistanceTo(end);
			if (num6 < 0.005)
			{
				return val9;
			}
			return null;
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadNetworkEngine] CreateArcFromThreePoints异常: " + ex.Message, (Exception)null);
			return null;
		}
	}

	private XYZ ComputeCircleCenter(XYZ p1, XYZ p2, XYZ p3)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		try
		{
			double x = p1.X;
			double y = p1.Y;
			double x2 = p2.X;
			double y2 = p2.Y;
			double x3 = p3.X;
			double y3 = p3.Y;
			double num = 2.0 * (x * (y2 - y3) + x2 * (y3 - y) + x3 * (y - y2));
			if (Math.Abs(num) < 0.001)
			{
				return null;
			}
			double num2 = ((x * x + y * y) * (y2 - y3) + (x2 * x2 + y2 * y2) * (y3 - y) + (x3 * x3 + y3 * y3) * (y - y2)) / num;
			double num3 = ((x * x + y * y) * (x3 - x2) + (x2 * x2 + y2 * y2) * (x - x3) + (x3 * x3 + y3 * y3) * (x2 - x)) / num;
			return new XYZ(num2, num3, 0.0);
		}
		catch
		{
			return null;
		}
	}

	private List<int> GenerateCurbWalls(List<List<XYZ>> sidewalkPolygons, ElementId wallTypeId, ElementId levelId, double curbHeightMillimeters, double curbBaseOffsetMillimeters)
	{
		try
		{
			if (sidewalkPolygons == null || sidewalkPolygons.Count == 0)
			{
				return new List<int>();
			}
			List<int> list = new List<int>();
			double num = curbHeightMillimeters / 304.8;
			double num2 = curbBaseOffsetMillimeters / 304.8;
			Element element = _document.GetElement(levelId);
			Level val = (Level)(object)((element is Level) ? element : null);
			if (val == null)
			{
				_logger.Warning("[RoadNetworkEngine] 无法获取标高，跳过路沿石创建");
				return list;
			}
			double num3 = num;
			double num4 = 0.3116797900262467;
			foreach (List<XYZ> sidewalkPolygon in sidewalkPolygons)
			{
				try
				{
					CurveLoop val2 = FitPolygonToCurveLoop(sidewalkPolygon);
					if (val2 == null)
					{
						continue;
					}
					foreach (Curve item in val2)
					{
						try
						{
							if (!(item.Length < num4))
							{
								Wall val3 = Wall.Create(_document, item, wallTypeId, levelId, num3, num2, false, false);
								if (val3 != null)
								{
									list.Add(((Element)val3).Id.smethod_0());
								}
							}
						}
						catch (Exception ex)
						{
							_logger.Warning("[RoadNetworkEngine] 创建单条路沿石墙失败: " + ex.Message);
						}
					}
				}
				catch (Exception ex2)
				{
					_logger.Warning("[RoadNetworkEngine] 处理单个人行道多边形失败: " + ex2.Message);
				}
			}
			ILogger logger = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[RoadNetworkEngine] 成功创建 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 段路沿石墙");
			logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex3)
		{
			_logger.Error("[RoadNetworkEngine] 生成路沿石墙失败", ex3);
			return new List<int>();
		}
	}

	private List<List<XYZ>> GenerateDirectionArrowPolygons(List<CurveWithWidth> splitCurves, List<CenterMarkingData> markingDataList, double laneWidthMillimeters, double arrowSpacingMeters, double arrowSizeMeters)
	{
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Expected O, but got Unknown
		try
		{
			List<List<XYZ>> list = new List<List<XYZ>>();
			Class329<int, int>[] source = new Class329<int, int>[8]
			{
				new Class329<int, int>(0, 0),
				new Class329<int, int>(225, -1200),
				new Class329<int, int>(75, -1200),
				new Class329<int, int>(75, -3000),
				new Class329<int, int>(-75, -3000),
				new Class329<int, int>(-75, -1200),
				new Class329<int, int>(-225, -1200),
				new Class329<int, int>(0, 0)
			};
			double scaleFactor = arrowSizeMeters / 3.0;
			Class329<double, double>[] array = source.Select((Class329<int, int> p) => new Class329<double, double>((double)p.X * scaleFactor, (double)p.Y * scaleFactor)).ToArray();
			double num = laneWidthMillimeters / 304.8;
			foreach (CenterMarkingData markingData in markingDataList)
			{
				try
				{
					Curve centerline = markingData.Centerline;
					if ((GeometryObject)(object)centerline == (GeometryObject)null || centerline.Length < arrowSizeMeters * 1.5)
					{
						continue;
					}
					int splitCurveIndex = markingData.SplitCurveIndex;
					double width = splitCurves[splitCurveIndex].Width;
					int num2 = (int)Math.Floor(width / 2.0 / laneWidthMillimeters);
					if (num2 < 1)
					{
						num2 = 1;
					}
					double[] array2 = new double[4] { 0.0, 1.0, 2.0, 3.0 };
					double[] array3 = array2;
					foreach (double num4 in array3)
					{
						bool flag = num4 == 0.0 || num4 == 2.0;
						bool flag2 = num4 == 0.0 || num4 == 1.0;
						centerline.GetEndParameter(0);
						double length = centerline.Length;
						double num5 = (flag ? ((!flag2) ? ((arrowSizeMeters + 1.0) / 0.3048) : 3.280839895013123) : ((!flag2) ? ((arrowSizeMeters + 1.0) / 0.3048) : 3.280839895013123));
						if (length <= num5)
						{
							continue;
						}
						double num6 = ((!flag) ? ComputeParameterAtDistance(centerline, length - num5) : ComputeParameterAtDistance(centerline, num5));
						XYZ val = centerline.Evaluate(num6, false);
						XYZ val2 = centerline.ComputeDerivatives(num6, false).BasisX.Normalize();
						XYZ val3 = (flag ? (-val2) : val2);
						XYZ val4 = val3.CrossProduct(XYZ.BasisZ).Normalize();
						for (int num7 = 1; num7 <= num2; num7++)
						{
							try
							{
								double num8 = ((double)num7 - 0.5) * num;
								XYZ val5 = val + (flag2 ? val4 : (-val4)) * num8;
								XYZ targetDirection = (flag2 ? val3 : (-val3));
								List<XYZ> list2 = new List<XYZ>();
								Class329<double, double>[] array4 = array;
								foreach (Class329<double, double> @class in array4)
								{
									double num10 = @class.X / 304.8;
									double num11 = @class.Y / 304.8;
									XYZ val6 = RotatePointFromDirection(new XYZ(num10, num11, 0.0), XYZ.BasisY, targetDirection);
									XYZ item = val5 + val6;
									list2.Add(item);
								}
								if (list2.Count >= 3)
								{
									list.Add(list2);
								}
							}
							catch (Exception ex)
							{
								ILogger logger = _logger;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[RoadNetworkEngine] 生成车道");
								defaultInterpolatedStringHandler.AppendFormatted(num7);
								defaultInterpolatedStringHandler.AppendLiteral("箭头失败: ");
								defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
								logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
							}
						}
					}
					double num12 = centerline.Length * 0.3048;
					double arrowSpacingFeet = arrowSpacingMeters / 0.3048;
					double d = (num12 - arrowSizeMeters * 2.0 - arrowSpacingMeters) / arrowSpacingMeters;
					int num13 = (int)Math.Floor(d);
					if (num13 > 2)
					{
						num13 = 2;
					}
					if (num13 > 0)
					{
						double baseOffsetFeet = (1.0 + arrowSpacingMeters) / 0.3048;
						if (markingData.StartShrinkFeet > 0.001)
						{
							object[] arrowPointsMm = array;
							GenerateStopLineArrows(list, centerline, arrowPointsMm, isStart: true, baseOffsetFeet, arrowSpacingFeet, num13, num, num2);
						}
						if (markingData.EndShrinkFeet > 0.001)
						{
							object[] arrowPointsMm = array;
							GenerateStopLineArrows(list, centerline, arrowPointsMm, isStart: false, baseOffsetFeet, arrowSpacingFeet, num13, num, num2);
						}
					}
				}
				catch (Exception ex2)
				{
					_logger.Warning("[RoadNetworkEngine] 处理标线箭头失败: " + ex2.Message);
				}
			}
			ILogger logger2 = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(31, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("[RoadNetworkEngine] 成功生成 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个指向箭头");
			logger2.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			return list;
		}
		catch (Exception ex3)
		{
			_logger.Error("[RoadNetworkEngine] 生成指向箭头失败", ex3);
			return new List<List<XYZ>>();
		}
	}

	private void GenerateStopLineArrows(List<List<XYZ>> result, Curve centerline, dynamic[] arrowPointsMm, bool isStart, double baseOffsetFeet, double arrowSpacingFeet, int arrowCount, double laneWidthFeet, int laneCount)
	{
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Expected O, but got Unknown
		double length = centerline.Length;
		for (int i = 0; i < arrowCount; i++)
		{
			double num = baseOffsetFeet + (double)i * arrowSpacingFeet;
			if (length <= num)
			{
				continue;
			}
			double num2 = ((!isStart) ? ComputeParameterAtDistance(centerline, length - num) : ComputeParameterAtDistance(centerline, num));
			XYZ val = centerline.Evaluate(num2, false);
			XYZ val2 = centerline.ComputeDerivatives(num2, false).BasisX.Normalize();
			XYZ val3 = val2.CrossProduct(XYZ.BasisZ).Normalize();
			bool flag;
			XYZ val4 = ((flag = !isStart) ? val3 : (-val3));
			XYZ targetDirection = (flag ? val2 : (-val2));
			for (int j = 1; j <= laneCount; j++)
			{
				try
				{
					double num3 = ((double)j - 0.5) * laneWidthFeet;
					XYZ val5 = val + val4 * num3;
					List<XYZ> list = new List<XYZ>();
					foreach (object arg in arrowPointsMm)
					{
						if (_003C_003Eo__94._003C_003Ep__0 == null)
						{
							_003C_003Eo__94._003C_003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "X", typeof(RoadNetworkEngine), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						double num4 = (dynamic)_003C_003Eo__94._003C_003Ep__0.Target(_003C_003Eo__94._003C_003Ep__0, arg) / 304.8;
						if (_003C_003Eo__94._003C_003Ep__3 == null)
						{
							_003C_003Eo__94._003C_003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Y", typeof(RoadNetworkEngine), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						double num5 = (dynamic)_003C_003Eo__94._003C_003Ep__3.Target(_003C_003Eo__94._003C_003Ep__3, arg) / 304.8;
						XYZ val6 = RotatePointFromDirection(new XYZ(num4, num5, 0.0), XYZ.BasisY, targetDirection);
						list.Add(val5 + val6);
					}
					if (list.Count >= 3)
					{
						result.Add(list);
					}
				}
				catch (Exception ex)
				{
					_logger.Warning("[RoadNetworkEngine] 停止线箭头生成失败: " + ex.Message);
				}
			}
		}
	}

	private XYZ RotatePointFromDirection(XYZ point, XYZ initialDirection, XYZ targetDirection)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		try
		{
			XYZ val = initialDirection.CrossProduct(targetDirection).Normalize();
			double num = initialDirection.AngleTo(targetDirection);
			if (val.GetLength() < 0.001)
			{
				if (num < 0.001)
				{
					return point;
				}
				return new XYZ(0.0 - point.X, 0.0 - point.Y, point.Z);
			}
			XYZ val2 = val;
			XYZ val3 = point * Math.Cos(num);
			XYZ val4 = val2.CrossProduct(point) * Math.Sin(num);
			XYZ val5 = val2 * val2.DotProduct(point) * (1.0 - Math.Cos(num));
			return val3 + val4 + val5;
		}
		catch (Exception)
		{
			return point;
		}
	}

	private XYZ RotatePoint(XYZ point, XYZ targetDirection)
	{
		return RotatePointFromDirection(point, XYZ.BasisY, targetDirection);
	}
}
