using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Revit.WallToRoad;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns1;
using ns6;

namespace RevitAi.Revit.Services;

public sealed class WallToRoadService
{
	private class FloorWarningSuppressor : IFailuresPreprocessor
	{
		public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Invalid comparison between Unknown and I4
			IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();
			foreach (FailureMessageAccessor item in failureMessages)
			{
				if ((int)item.GetSeverity() == 1)
				{
					failuresAccessor.DeleteWarning(item);
				}
			}
			return (FailureProcessingResult)0;
		}
	}

	private readonly UIApplication _application;

	private readonly Document _document;

	private readonly ILogger _logger;

	private const double MillimetersPerFoot = 304.8;

	private const double MetersPerFoot = 0.3048;

	public WallToRoadService(UIApplication application, Document document)
	{
		_application = application;
		_document = document;
		_logger = ServiceProvider.GetLogger();
	}

	public Result<List<int>> CreateRoadFromWallsWithTransaction(Document document, WallToRoadRequest request, object externalTransaction)
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			_logger.Info("[WallToRoadService] 开始从墙创建道路网络（使用外部事务）");
			List<Wall> wallsByIds = GetWallsByIds(document, request.WallElementIds);
			if (wallsByIds.Count == 0)
			{
				return Result<List<int>>.Failure("未找到有效的墙元素");
			}
			ILogger logger = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[WallToRoadService] 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(wallsByIds.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 道墙");
			logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			Level levelNearestZero = GetLevelNearestZero(document);
			if (levelNearestZero == null)
			{
				return Result<List<int>>.Failure("无法获取标高");
			}
			List<int> list = new List<int>();
			FailureHandlingOptions failureHandlingOptions = ((Transaction)externalTransaction).GetFailureHandlingOptions();
			failureHandlingOptions.SetFailuresPreprocessor((IFailuresPreprocessor)(object)new FloorWarningSuppressor());
			((Transaction)externalTransaction).SetFailureHandlingOptions(failureHandlingOptions);
			FloorType orCreateFloorType = GetOrCreateFloorType(document, "AST_道路");
			FloorType orCreateFloorType2 = GetOrCreateFloorType(document, "AST_人行道");
			FloorType orCreateFloorType3 = GetOrCreateFloorType(document, "AST_标线_黄");
			FloorType orCreateFloorType4 = GetOrCreateFloorType(document, "AST_人行横道");
			FloorType orCreateFloorType5 = GetOrCreateFloorType(document, "AST_标线_白");
			FloorType orCreateFloorType6 = GetOrCreateFloorType(document, "AST_指向箭头");
			FloorType orCreateFloorType7 = GetOrCreateFloorType(document, "AST_场地");
			WallType orCreateCurbWallType = GetOrCreateCurbWallType(document, "AST_路沿石");
			RoadNetworkEngine roadNetworkEngine = new RoadNetworkEngine(document);
			double laneWidthMillimeters = request.LaneWidthMeters * 1000.0;
			double sidewalkWidthMillimeters = request.SidewalkWidthMeters * 1000.0;
			double curbWidthMillimeters = request.CurbWidthMillimeters;
			double intersectionRadiusMeters = request.IntersectionRadiusMeters;
			double defaultRoadWidthMillimeters = request.DefaultRoadWidthMeters * 1000.0;
			ElementId id = ((Element)levelNearestZero).Id;
			ElementId id2 = ((Element)orCreateFloorType).Id;
			bool createSidewalks = request.CreateSidewalks;
			object obj;
			if (orCreateFloorType2 == null)
			{
				obj = null;
			}
			else
			{
				obj = ((Element)orCreateFloorType2).Id;
				if (obj != null)
				{
					goto IL_01fa;
				}
			}
			obj = ((Element)orCreateFloorType).Id;
			goto IL_01fa;
			IL_02b9:
			bool createDirectionArrows = request.CreateDirectionArrows;
			double arrowSpacingMeters = request.ArrowSpacingMeters;
			double arrowSizeMeters = request.ArrowSizeMeters;
			object obj2;
			if (orCreateFloorType6 == null)
			{
				obj2 = null;
			}
			else
			{
				obj2 = ((Element)orCreateFloorType6).Id;
				if (obj2 != null)
				{
					goto IL_02e4;
				}
			}
			obj2 = ((Element)orCreateFloorType).Id;
			goto IL_02e4;
			IL_0309:
			double sidewalkFloorOffsetMillimeters;
			int createCurbs;
			object curbWallTypeId;
			double curbHeightMillimeters;
			double curbBaseOffsetMillimeters;
			int createCenterMarkings;
			double centerMarkingWidthMillimeters;
			double centerMarkingThicknessMillimeters;
			object obj3;
			int isCenterMarkingDoubleLine;
			int createCrosswalks;
			double crosswalkLineWidthMillimeters;
			double crosswalkSpacingMillimeters;
			double crosswalkLengthMeters;
			double crosswalkDistanceFromMarkingMeters;
			object obj4;
			int createLaneMarkings;
			double laneMarkingWidthMillimeters;
			double laneMarkingSolidLengthMeters;
			double laneMarkingGapLengthMeters;
			object obj5;
			int createSiteFloor;
			double siteFloorExtensionMeters;
			object obj6;
			List<int> list2 = roadNetworkEngine.GenerateRoadFromWalls(wallsByIds, laneWidthMillimeters, defaultRoadWidthMillimeters, sidewalkWidthMillimeters, curbWidthMillimeters, intersectionRadiusMeters, id, id2, createSidewalks, (ElementId)obj, sidewalkFloorOffsetMillimeters, (byte)createCurbs != 0, (ElementId)curbWallTypeId, curbHeightMillimeters, curbBaseOffsetMillimeters, (byte)createCenterMarkings != 0, centerMarkingWidthMillimeters, centerMarkingThicknessMillimeters, (ElementId)obj3, (byte)isCenterMarkingDoubleLine != 0, (byte)createCrosswalks != 0, crosswalkLineWidthMillimeters, crosswalkSpacingMillimeters, crosswalkLengthMeters, crosswalkDistanceFromMarkingMeters, (ElementId)obj4, (byte)createLaneMarkings != 0, laneMarkingWidthMillimeters, laneMarkingSolidLengthMeters, laneMarkingGapLengthMeters, (ElementId)obj5, createDirectionArrows, arrowSpacingMeters, arrowSizeMeters, (ElementId)obj2, (byte)createSiteFloor != 0, siteFloorExtensionMeters, (ElementId)obj6, request.RoadFloorOffsetMillimeters, request.SiteFloorOffsetMillimeters);
			if (list2.Count > 0)
			{
				list.AddRange(list2);
				request.CreatedLaneFloorIds.AddRange(list2);
			}
			if (request.DeleteOriginalWalls)
			{
				DeleteOriginalWalls(document, wallsByIds);
			}
			ILogger logger2 = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(35, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("[WallToRoadService] 成功创建 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个楼板（外部事务）");
			logger2.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			return Result<List<int>>.Success(list);
			IL_024b:
			isCenterMarkingDoubleLine = (request.IsCenterMarkingDoubleLine ? 1 : 0);
			createCrosswalks = (request.CreateCrosswalks ? 1 : 0);
			crosswalkLineWidthMillimeters = request.CrosswalkLineWidthMillimeters;
			crosswalkSpacingMillimeters = request.CrosswalkSpacingMillimeters;
			crosswalkLengthMeters = request.CrosswalkLengthMeters;
			crosswalkDistanceFromMarkingMeters = request.CrosswalkDistanceFromMarkingMeters;
			if (orCreateFloorType4 == null)
			{
				obj4 = null;
			}
			else
			{
				obj4 = ((Element)orCreateFloorType4).Id;
				if (obj4 != null)
				{
					goto IL_0288;
				}
			}
			obj4 = ((Element)orCreateFloorType).Id;
			goto IL_0288;
			IL_0288:
			createLaneMarkings = (request.CreateLaneMarkings ? 1 : 0);
			laneMarkingWidthMillimeters = request.LaneMarkingWidthMillimeters;
			laneMarkingSolidLengthMeters = request.LaneMarkingSolidLengthMeters;
			laneMarkingGapLengthMeters = request.LaneMarkingGapLengthMeters;
			if (orCreateFloorType5 == null)
			{
				obj5 = null;
			}
			else
			{
				obj5 = ((Element)orCreateFloorType5).Id;
				if (obj5 != null)
				{
					goto IL_02b9;
				}
			}
			obj5 = ((Element)orCreateFloorType).Id;
			goto IL_02b9;
			IL_02e4:
			createSiteFloor = (request.CreateSiteFloor ? 1 : 0);
			siteFloorExtensionMeters = request.SiteFloorExtensionMeters;
			if (orCreateFloorType7 == null)
			{
				obj6 = null;
			}
			else
			{
				obj6 = ((Element)orCreateFloorType7).Id;
				if (obj6 != null)
				{
					goto IL_0309;
				}
			}
			obj6 = ((Element)orCreateFloorType).Id;
			goto IL_0309;
			IL_01fa:
			sidewalkFloorOffsetMillimeters = request.SidewalkFloorOffsetMillimeters;
			createCurbs = (request.CreateCurbs ? 1 : 0);
			curbWallTypeId = ((orCreateCurbWallType != null) ? ((Element)orCreateCurbWallType).Id : null);
			curbHeightMillimeters = request.CurbHeightMillimeters;
			curbBaseOffsetMillimeters = request.CurbBaseOffsetMillimeters;
			createCenterMarkings = (request.CreateCenterMarkings ? 1 : 0);
			centerMarkingWidthMillimeters = request.CenterMarkingWidthMillimeters;
			centerMarkingThicknessMillimeters = request.CenterMarkingThicknessMillimeters;
			if (orCreateFloorType3 == null)
			{
				obj3 = null;
			}
			else
			{
				obj3 = ((Element)orCreateFloorType3).Id;
				if (obj3 != null)
				{
					goto IL_024b;
				}
			}
			obj3 = ((Element)orCreateFloorType).Id;
			goto IL_024b;
		}
		catch (Exception ex)
		{
			_logger.Error("[WallToRoadService] 创建道路网络失败（外部事务）", ex);
			return Result<List<int>>.Failure("创建失败: " + ex.Message);
		}
	}

	public Result<List<int>> CreateRoadFromWalls(Document document, WallToRoadRequest request)
	{
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			_logger.Info("[WallToRoadService] 开始从墙创建道路网络");
			List<Wall> wallsByIds = GetWallsByIds(document, request.WallElementIds);
			if (wallsByIds.Count == 0)
			{
				return Result<List<int>>.Failure("未找到有效的墙元素");
			}
			ILogger logger = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[WallToRoadService] 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(wallsByIds.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 道墙");
			logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			Level levelNearestZero = GetLevelNearestZero(document);
			if (levelNearestZero == null)
			{
				return Result<List<int>>.Failure("无法获取标高");
			}
			List<int> list = new List<int>();
			Transaction val = new Transaction(document, "从墙生成道路网络");
			try
			{
				val.Start();
				FailureHandlingOptions failureHandlingOptions = val.GetFailureHandlingOptions();
				failureHandlingOptions.SetFailuresPreprocessor((IFailuresPreprocessor)(object)new FloorWarningSuppressor());
				val.SetFailureHandlingOptions(failureHandlingOptions);
				FloorType orCreateFloorType = GetOrCreateFloorType(document, "AST_道路");
				FloorType orCreateFloorType2 = GetOrCreateFloorType(document, "AST_人行道");
				FloorType orCreateFloorType3 = GetOrCreateFloorType(document, "AST_标线_黄");
				FloorType orCreateFloorType4 = GetOrCreateFloorType(document, "AST_人行横道");
				FloorType orCreateFloorType5 = GetOrCreateFloorType(document, "AST_标线_白");
				FloorType orCreateFloorType6 = GetOrCreateFloorType(document, "AST_指向箭头");
				FloorType orCreateFloorType7 = GetOrCreateFloorType(document, "AST_场地");
				WallType orCreateCurbWallType = GetOrCreateCurbWallType(document, "AST_路沿石");
				try
				{
					RoadNetworkEngine roadNetworkEngine = new RoadNetworkEngine(document);
					double laneWidthMillimeters = request.LaneWidthMeters * 1000.0;
					double sidewalkWidthMillimeters = request.SidewalkWidthMeters * 1000.0;
					double curbWidthMillimeters = request.CurbWidthMillimeters;
					double intersectionRadiusMeters = request.IntersectionRadiusMeters;
					double defaultRoadWidthMillimeters = request.DefaultRoadWidthMeters * 1000.0;
					ElementId id = ((Element)levelNearestZero).Id;
					ElementId id2 = ((Element)orCreateFloorType).Id;
					bool createSidewalks = request.CreateSidewalks;
					object obj;
					if (orCreateFloorType2 == null)
					{
						obj = null;
					}
					else
					{
						obj = ((Element)orCreateFloorType2).Id;
						if (obj != null)
						{
							goto IL_020c;
						}
					}
					obj = ((Element)orCreateFloorType).Id;
					goto IL_020c;
					IL_02f6:
					bool createSiteFloor = request.CreateSiteFloor;
					double siteFloorExtensionMeters = request.SiteFloorExtensionMeters;
					object obj2;
					if (orCreateFloorType7 == null)
					{
						obj2 = null;
					}
					else
					{
						obj2 = ((Element)orCreateFloorType7).Id;
						if (obj2 != null)
						{
							goto IL_031b;
						}
					}
					obj2 = ((Element)orCreateFloorType).Id;
					goto IL_031b;
					IL_029a:
					bool createLaneMarkings = request.CreateLaneMarkings;
					double laneMarkingWidthMillimeters = request.LaneMarkingWidthMillimeters;
					double laneMarkingSolidLengthMeters = request.LaneMarkingSolidLengthMeters;
					double laneMarkingGapLengthMeters = request.LaneMarkingGapLengthMeters;
					object obj3;
					if (orCreateFloorType5 == null)
					{
						obj3 = null;
					}
					else
					{
						obj3 = ((Element)orCreateFloorType5).Id;
						if (obj3 != null)
						{
							goto IL_02cb;
						}
					}
					obj3 = ((Element)orCreateFloorType).Id;
					goto IL_02cb;
					IL_031b:
					double sidewalkFloorOffsetMillimeters;
					int createCurbs;
					object curbWallTypeId;
					double curbHeightMillimeters;
					double curbBaseOffsetMillimeters;
					int createCenterMarkings;
					double centerMarkingWidthMillimeters;
					double centerMarkingThicknessMillimeters;
					object obj4;
					int isCenterMarkingDoubleLine;
					int createCrosswalks;
					double crosswalkLineWidthMillimeters;
					double crosswalkSpacingMillimeters;
					double crosswalkLengthMeters;
					double crosswalkDistanceFromMarkingMeters;
					object obj5;
					int createDirectionArrows;
					double arrowSpacingMeters;
					double arrowSizeMeters;
					object obj6;
					List<int> list2 = roadNetworkEngine.GenerateRoadFromWalls(wallsByIds, laneWidthMillimeters, defaultRoadWidthMillimeters, sidewalkWidthMillimeters, curbWidthMillimeters, intersectionRadiusMeters, id, id2, createSidewalks, (ElementId)obj, sidewalkFloorOffsetMillimeters, (byte)createCurbs != 0, (ElementId)curbWallTypeId, curbHeightMillimeters, curbBaseOffsetMillimeters, (byte)createCenterMarkings != 0, centerMarkingWidthMillimeters, centerMarkingThicknessMillimeters, (ElementId)obj4, (byte)isCenterMarkingDoubleLine != 0, (byte)createCrosswalks != 0, crosswalkLineWidthMillimeters, crosswalkSpacingMillimeters, crosswalkLengthMeters, crosswalkDistanceFromMarkingMeters, (ElementId)obj5, createLaneMarkings, laneMarkingWidthMillimeters, laneMarkingSolidLengthMeters, laneMarkingGapLengthMeters, (ElementId)obj3, (byte)createDirectionArrows != 0, arrowSpacingMeters, arrowSizeMeters, (ElementId)obj6, createSiteFloor, siteFloorExtensionMeters, (ElementId)obj2, request.RoadFloorOffsetMillimeters, request.SiteFloorOffsetMillimeters);
					if (list2.Count > 0)
					{
						list.AddRange(list2);
						request.CreatedLaneFloorIds.AddRange(list2);
					}
					if (request.DeleteOriginalWalls)
					{
						DeleteOriginalWalls(document, wallsByIds);
					}
					val.Commit();
					ILogger logger2 = _logger;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(29, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("[WallToRoadService] 成功创建 ");
					defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
					defaultInterpolatedStringHandler2.AppendLiteral(" 个楼板");
					logger2.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					goto end_IL_0189;
					IL_020c:
					sidewalkFloorOffsetMillimeters = request.SidewalkFloorOffsetMillimeters;
					createCurbs = (request.CreateCurbs ? 1 : 0);
					curbWallTypeId = ((orCreateCurbWallType != null) ? ((Element)orCreateCurbWallType).Id : null);
					curbHeightMillimeters = request.CurbHeightMillimeters;
					curbBaseOffsetMillimeters = request.CurbBaseOffsetMillimeters;
					createCenterMarkings = (request.CreateCenterMarkings ? 1 : 0);
					centerMarkingWidthMillimeters = request.CenterMarkingWidthMillimeters;
					centerMarkingThicknessMillimeters = request.CenterMarkingThicknessMillimeters;
					if (orCreateFloorType3 == null)
					{
						obj4 = null;
					}
					else
					{
						obj4 = ((Element)orCreateFloorType3).Id;
						if (obj4 != null)
						{
							goto IL_025d;
						}
					}
					obj4 = ((Element)orCreateFloorType).Id;
					goto IL_025d;
					IL_02cb:
					createDirectionArrows = (request.CreateDirectionArrows ? 1 : 0);
					arrowSpacingMeters = request.ArrowSpacingMeters;
					arrowSizeMeters = request.ArrowSizeMeters;
					if (orCreateFloorType6 == null)
					{
						obj6 = null;
					}
					else
					{
						obj6 = ((Element)orCreateFloorType6).Id;
						if (obj6 != null)
						{
							goto IL_02f6;
						}
					}
					obj6 = ((Element)orCreateFloorType).Id;
					goto IL_02f6;
					IL_025d:
					isCenterMarkingDoubleLine = (request.IsCenterMarkingDoubleLine ? 1 : 0);
					createCrosswalks = (request.CreateCrosswalks ? 1 : 0);
					crosswalkLineWidthMillimeters = request.CrosswalkLineWidthMillimeters;
					crosswalkSpacingMillimeters = request.CrosswalkSpacingMillimeters;
					crosswalkLengthMeters = request.CrosswalkLengthMeters;
					crosswalkDistanceFromMarkingMeters = request.CrosswalkDistanceFromMarkingMeters;
					if (orCreateFloorType4 == null)
					{
						obj5 = null;
					}
					else
					{
						obj5 = ((Element)orCreateFloorType4).Id;
						if (obj5 != null)
						{
							goto IL_029a;
						}
					}
					obj5 = ((Element)orCreateFloorType).Id;
					goto IL_029a;
					end_IL_0189:;
				}
				catch (Exception ex)
				{
					val.RollBack();
					_logger.Error("[WallToRoadService] 创建失败，已回滚", ex);
					return Result<List<int>>.Failure("创建失败: " + ex.Message);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			return Result<List<int>>.Success(list);
		}
		catch (Exception ex2)
		{
			_logger.Error("[WallToRoadService] 创建道路网络失败", ex2);
			return Result<List<int>>.Failure("创建失败: " + ex2.Message);
		}
	}

	private List<Wall> GetWallsByIds(Document document, List<int> wallIds)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		List<Wall> list = new List<Wall>();
		foreach (int wallId in wallIds)
		{
			Element element = document.GetElement(new ElementId((long)wallId));
			Wall val = (Wall)(object)((element is Wall) ? element : null);
			if (val != null)
			{
				list.Add(val);
			}
		}
		return list;
	}

	private Level GetLevelNearestZero(Document document)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		List<Level> source = (from Level l in (IEnumerable)new FilteredElementCollector(document).OfClass(typeof(Level)).WhereElementIsNotElementType()
			orderby Math.Abs(l.Elevation)
			select l).ToList();
		Level val = source.FirstOrDefault();
		if (val != null)
		{
			ILogger logger = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[WallToRoadService] 找到距离0米最近的标高: ");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val).Name);
			defaultInterpolatedStringHandler.AppendLiteral(" (高度: ");
			defaultInterpolatedStringHandler.AppendFormatted(val.Elevation * 0.3048, "F3");
			defaultInterpolatedStringHandler.AppendLiteral("m)");
			logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		return val;
	}

	private Level GetTargetLevel(Document document, int levelId)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Invalid comparison between Unknown and I4
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Invalid comparison between Unknown and I4
		if (levelId > 0)
		{
			Element element = document.GetElement(new ElementId((long)levelId));
			Level val = (Level)(object)((element is Level) ? element : null);
			if (val != null)
			{
				return val;
			}
		}
		View activeView = document.ActiveView;
		if (activeView != null)
		{
			Element element2 = document.GetElement(((Element)activeView).LevelId);
			Level val2 = (Level)(object)((element2 is Level) ? element2 : null);
			if (val2 != null)
			{
				return val2;
			}
			if ((int)activeView.ViewType == 1)
			{
				try
				{
					foreach (Parameter parameter in ((Element)activeView).Parameters)
					{
						Parameter val3 = parameter;
						if ((val3.Definition.Name.Contains("Level") || val3.Definition.Name.Contains("标高")) && (int)val3.StorageType == 4)
						{
							Element element3 = document.GetElement(val3.AsElementId());
							Level val4 = (Level)(object)((element3 is Level) ? element3 : null);
							if (val4 != null)
							{
								return val4;
							}
						}
					}
				}
				catch
				{
				}
			}
		}
		List<Level> source = (from Level l in (IEnumerable)new FilteredElementCollector(document).OfClass(typeof(Level)).WhereElementIsNotElementType()
			orderby l.Elevation
			select l).ToList();
		return source.FirstOrDefault();
	}

	private FloorType GetOrCreateFloorType(Document document, string typeName)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected O, but got Unknown
		FloorType val = ((IEnumerable)new FilteredElementCollector(document).OfClass(typeof(FloorType)).WhereElementIsElementType()).Cast<FloorType>().FirstOrDefault((FloorType ft) => ((Element)ft).Name == typeName);
		if (val != null)
		{
			_logger.Info("[WallToRoadService] 找到已有楼板类型: " + typeName);
			return val;
		}
		FloorType val2 = ((IEnumerable)new FilteredElementCollector(document).OfClass(typeof(FloorType)).WhereElementIsElementType()).Cast<FloorType>().FirstOrDefault((FloorType ft) => ((ElementType)ft).FamilyName == "楼板" || ((ElementType)ft).FamilyName == "Floor");
		if (val2 == null)
		{
			throw new InvalidOperationException("无法找到默认楼板类型");
		}
		ElementType obj = ((ElementType)val2).Duplicate(typeName);
		FloorType val3 = (FloorType)(object)((obj is FloorType) ? obj : null);
		if (val3 == null)
		{
			_logger.Warning("[WallToRoadService] 无法复制楼板类型 '" + typeName + "'，使用默认类型");
			return val2;
		}
		string materialName = typeName + "材质";
		Material orCreateMaterial = GetOrCreateMaterial(document, materialName);
		if (orCreateMaterial != null)
		{
			SetMaterialColorByType(orCreateMaterial, typeName);
			CompoundStructure compoundStructure = ((HostObjAttributes)val3).GetCompoundStructure();
			if (compoundStructure != null)
			{
				try
				{
					double num = 125.0 / 381.0;
					CompoundStructureLayer item = new CompoundStructureLayer(num, (MaterialFunctionAssignment)1, ((Element)orCreateMaterial).Id);
					compoundStructure.SetLayers((IList<CompoundStructureLayer>)new List<CompoundStructureLayer> { item });
					((HostObjAttributes)val3).SetCompoundStructure(compoundStructure);
					_logger.Info("[WallToRoadService] 已设置楼板类型 '" + typeName + "' 复合结构为单层100mm");
				}
				catch (Exception ex)
				{
					_logger.Warning("[WallToRoadService] 设置复合结构失败: " + ex.Message);
				}
			}
		}
		_logger.Info("[WallToRoadService] 成功创建楼板类型: " + typeName);
		return val3;
	}

	private Material? GetOrCreateMaterial(Document document, string materialName)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		Material val = ((IEnumerable)new FilteredElementCollector(document).OfClass(typeof(Material))).Cast<Material>().FirstOrDefault((Material m) => ((Element)m).Name == materialName);
		if (val != null)
		{
			_logger.Info("[WallToRoadService] 找到已有材质: " + materialName);
			return val;
		}
		Material val2 = ((IEnumerable)new FilteredElementCollector(document).OfClass(typeof(Material))).Cast<Material>().FirstOrDefault();
		if (val2 == null)
		{
			_logger.Warning("[WallToRoadService] 无法找到默认材质模板");
			return null;
		}
		Material val3 = val2.Duplicate(materialName);
		if (val3 != null)
		{
			_logger.Info("[WallToRoadService] 成功创建材质: " + materialName);
		}
		return val3;
	}

	private void SetMaterialColorByType(Material material, string typeName)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Expected O, but got Unknown
		try
		{
			material.UseRenderAppearanceForShading = false;
			if (typeName == "AST_道路")
			{
				material.Color = new Color((byte)80, (byte)80, (byte)80);
			}
			else if (typeName == "AST_人行道")
			{
				material.Color = new Color((byte)200, (byte)160, (byte)140);
			}
			else if (typeName == "AST_标线_黄")
			{
				material.Color = new Color(byte.MaxValue, (byte)220, (byte)0);
			}
			else if (typeName == "AST_标线_白")
			{
				material.Color = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
			}
			else if (typeName == "AST_人行横道")
			{
				material.Color = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
			}
			else if (typeName == "AST_路沿石")
			{
				material.Color = new Color((byte)120, (byte)120, (byte)120);
			}
			else if (typeName == "AST_指向箭头")
			{
				material.Color = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
			}
			else if (typeName == "AST_场地")
			{
				material.Color = new Color((byte)120, (byte)180, (byte)100);
			}
			else if (typeName.Contains("AST_"))
			{
				material.Color = new Color((byte)180, (byte)180, (byte)180);
			}
			_logger.Info("[WallToRoadService] 已设置材质 '" + ((Element)material).Name + "' 颜色");
		}
		catch (Exception ex)
		{
			_logger.Warning("[WallToRoadService] 设置材质颜色失败: " + ex.Message);
		}
	}

	private WallType GetOrCreateCurbWallType(Document document, string typeName)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected O, but got Unknown
		WallType val = ((IEnumerable)new FilteredElementCollector(document).OfClass(typeof(WallType)).WhereElementIsElementType()).Cast<WallType>().FirstOrDefault((WallType wt) => ((Element)wt).Name == typeName);
		if (val != null)
		{
			_logger.Info("[WallToRoadService] 找到已有墙类型: " + typeName);
			return val;
		}
		WallType val2 = ((IEnumerable)new FilteredElementCollector(document).OfClass(typeof(WallType)).WhereElementIsElementType()).Cast<WallType>().FirstOrDefault(delegate(WallType wt)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			return (int)wt.Kind == 0;
		});
		if (val2 == null)
		{
			throw new InvalidOperationException("无法找到基本墙类型");
		}
		ElementType obj = ((ElementType)val2).Duplicate(typeName);
		WallType val3 = (WallType)(object)((obj is WallType) ? obj : null);
		if (val3 == null)
		{
			_logger.Warning("[WallToRoadService] 无法复制墙类型 '" + typeName + "'，使用默认类型");
			return val2;
		}
		string materialName = typeName + "材质";
		Material orCreateMaterial = GetOrCreateMaterial(document, materialName);
		if (orCreateMaterial != null)
		{
			SetMaterialColorByType(orCreateMaterial, typeName);
			CompoundStructure compoundStructure = ((HostObjAttributes)val3).GetCompoundStructure();
			if (compoundStructure != null)
			{
				try
				{
					double num = 0.49212598425196846;
					CompoundStructureLayer item = new CompoundStructureLayer(num, (MaterialFunctionAssignment)1, ((Element)orCreateMaterial).Id);
					compoundStructure.SetLayers((IList<CompoundStructureLayer>)new List<CompoundStructureLayer> { item });
					((HostObjAttributes)val3).SetCompoundStructure(compoundStructure);
					_logger.Info("[WallToRoadService] 已设置墙类型 '" + typeName + "' 复合结构为单层150mm");
				}
				catch (Exception ex)
				{
					_logger.Warning("[WallToRoadService] 设置墙复合结构失败: " + ex.Message);
				}
			}
		}
		_logger.Info("[WallToRoadService] 成功创建墙类型: " + typeName);
		return val3;
	}

	private void DeleteOriginalWalls(Document document, List<Wall> walls)
	{
		foreach (Wall wall in walls)
		{
			try
			{
				document.Delete(((Element)wall).Id);
			}
			catch (Exception ex)
			{
				ILogger logger = _logger;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[WallToRoadService] 删除墙 ");
				defaultInterpolatedStringHandler.AppendFormatted(((Element)wall).Id.smethod_0());
				defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
				defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
				logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
		ILogger logger2 = _logger;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(29, 1);
		defaultInterpolatedStringHandler2.AppendLiteral("[WallToRoadService] 删除了 ");
		defaultInterpolatedStringHandler2.AppendFormatted(walls.Count);
		defaultInterpolatedStringHandler2.AppendLiteral(" 道原始墙");
		logger2.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
	}
}
