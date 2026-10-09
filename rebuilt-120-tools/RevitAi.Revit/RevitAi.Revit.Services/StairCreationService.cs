using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Revit.Models;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using ns6;

namespace RevitAi.Revit.Services;

public class StairCreationService : IStairCreationService
{
	private readonly Document _document;

	public StairCreationService(Document document)
	{
		_document = document ?? throw new ArgumentNullException("document");
	}

	public ElementId CreateStair(StairCreationParameters parameters, ElementId bottomLevelId, ElementId? topLevelId = null)
	{
		if (!parameters.Validate(out string errorMessage))
		{
			throw new ArgumentException("参数验证失败: " + errorMessage, "parameters");
		}
		Element element = _document.GetElement(bottomLevelId);
		Level val = (Level)(object)((element is Level) ? element : null);
		if (val == null)
		{
			throw new ArgumentException("无效的底部标高ID");
		}
		double targetElevation = val.Elevation + parameters.TotalHeightMm / 304.8;
		Level val2 = FindNearestLevelAbove(targetElevation);
		if (val2 == null)
		{
			throw new InvalidOperationException("文档中没有找到合适的顶部标高");
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
		defaultInterpolatedStringHandler.AppendLiteral("[楼梯创建] 底部标高: ");
		defaultInterpolatedStringHandler.AppendFormatted(((Element)val).Name);
		defaultInterpolatedStringHandler.AppendLiteral(" (");
		defaultInterpolatedStringHandler.AppendFormatted(val.Elevation * 304.8, "F0");
		defaultInterpolatedStringHandler.AppendLiteral("mm)");
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(18, 2);
		defaultInterpolatedStringHandler2.AppendLiteral("[楼梯创建] 顶部标高: ");
		defaultInterpolatedStringHandler2.AppendFormatted(((Element)val2).Name);
		defaultInterpolatedStringHandler2.AppendLiteral(" (");
		defaultInterpolatedStringHandler2.AppendFormatted(val2.Elevation * 304.8, "F0");
		defaultInterpolatedStringHandler2.AppendLiteral("mm)");
		Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
		Stairs val3 = CreateStairsUsingSketch(parameters, val, val2);
		object obj;
		if (val3 == null)
		{
			obj = null;
		}
		else
		{
			obj = ((Element)val3).Id;
			if (obj != null)
			{
				goto IL_019b;
			}
		}
		obj = ElementId.InvalidElementId;
		goto IL_019b;
		IL_019b:
		return (ElementId)obj;
	}

	private Level? FindNearestLevelAbove(double targetElevation)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		List<Level> list = (from Level l in (IEnumerable)new FilteredElementCollector(_document).OfClass(typeof(Level)).WhereElementIsNotElementType()
			where l.Elevation >= targetElevation
			orderby l.Elevation
			select l).ToList();
		if (list.Count > 0)
		{
			return list[0];
		}
		return (from Level l in (IEnumerable)new FilteredElementCollector(_document).OfClass(typeof(Level)).WhereElementIsNotElementType()
			orderby l.Elevation descending
			select l).FirstOrDefault();
	}

	private Stairs? CreateStairsUsingSketch(StairCreationParameters parameters, Level bottomLevel, Level? topLevel)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("[楼梯创建] 开始创建楼梯");
		StairsType stairsType = GetStairsType();
		Stairs val = null;
		StairsEditScope val2 = new StairsEditScope(_document, "创建参数化楼梯");
		try
		{
			ElementId id = ((Element)bottomLevel).Id;
			object obj;
			if (topLevel == null)
			{
				obj = null;
			}
			else
			{
				obj = ((Element)topLevel).Id;
				if (obj != null)
				{
					goto IL_004a;
				}
			}
			obj = ElementId.InvalidElementId;
			goto IL_004a;
			IL_004a:
			ElementId val3 = val2.Start(id, (ElementId)obj);
			Element element = _document.GetElement(val3);
			val = (Stairs)(object)((element is Stairs) ? element : null);
			if (val == null)
			{
				throw new InvalidOperationException("无法创建楼梯元素");
			}
			Transaction val4 = new Transaction(_document, "创建梯段和平台");
			try
			{
				val4.Start();
				try
				{
					SetupStairParameters(val, stairsType, parameters, bottomLevel, topLevel);
					List<ElementId> list = CreateSingleRun(val, parameters, bottomLevel);
					_document.Regenerate();
					val.DesiredRisersNumber = parameters.TotalSteps;
					_document.Regenerate();
					if (list != null && list.Count > 1)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[楼梯创建] 开始创建自动平台，梯段数量: ");
						defaultInterpolatedStringHandler.AppendFormatted(list.Count);
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						CreateLandingsInSameScope(list);
					}
					val4.Commit();
				}
				catch
				{
					val4.RollBack();
					throw;
				}
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
			((EditScope)val2).Commit((IFailuresPreprocessor)(object)new StairsEditScopeFailuresPreprocessor());
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		return val;
	}

	private void CreateLandingsInSameScope(List<ElementId> runIds)
	{
		for (int i = 0; i < runIds.Count - 1; i++)
		{
			try
			{
				IList<ElementId> list = StairsLanding.CreateAutomaticLanding(_document, runIds[i], runIds[i + 1]);
				if (list != null && list.Count > 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 3);
					defaultInterpolatedStringHandler.AppendLiteral("[楼梯创建] 成功创建平台连接第 ");
					defaultInterpolatedStringHandler.AppendFormatted(i + 1);
					defaultInterpolatedStringHandler.AppendLiteral(" 跑和第 ");
					defaultInterpolatedStringHandler.AppendFormatted(i + 2);
					defaultInterpolatedStringHandler.AppendLiteral(" 跑，平台数量: ");
					defaultInterpolatedStringHandler.AppendFormatted(list.Count);
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(26, 3);
				defaultInterpolatedStringHandler2.AppendLiteral("[楼梯创建] 创建平台失败（第 ");
				defaultInterpolatedStringHandler2.AppendFormatted(i + 1);
				defaultInterpolatedStringHandler2.AppendLiteral(" 跑到第 ");
				defaultInterpolatedStringHandler2.AppendFormatted(i + 2);
				defaultInterpolatedStringHandler2.AppendLiteral(" 跑）: ");
				defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
				Logger.Error(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
		}
	}

	private void SetupStairParameters(Stairs stairs, StairsType? stairType, StairCreationParameters parameters, Level bottomLevel, Level? topLevel)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Invalid comparison between Unknown and I4
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Invalid comparison between Unknown and I4
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Invalid comparison between Unknown and I4
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Invalid comparison between Unknown and I4
		Parameter val = ((Element)stairs).LookupParameter("实际踏板深度") ?? ((Element)stairs).LookupParameter("Actual Tread Depth");
		if (val != null && (int)val.StorageType == 2)
		{
			val.Set(parameters.TreadDepthMm / 304.8);
		}
		double num = bottomLevel.Elevation + parameters.BaseElevationM;
		double num2 = num + parameters.TotalHeightMm / 304.8;
		Parameter val2 = ((Element)stairs).LookupParameter("底部标高");
		if (val2 != null && (int)val2.StorageType == 2)
		{
			val2.Set(num);
		}
		Parameter val3 = ((Element)stairs).LookupParameter("底部偏移");
		if (val3 != null && (int)val3.StorageType == 2)
		{
			double num3 = num - bottomLevel.Elevation;
			val3.Set(num3);
		}
		Parameter val4 = ((Element)stairs).LookupParameter("顶部标高");
		if (val4 != null && (int)val4.StorageType == 2)
		{
			val4.Set(num2);
		}
		Parameter val5 = ((Element)stairs).LookupParameter("顶部偏移");
		if (val5 != null && (int)val5.StorageType == 2 && topLevel != null)
		{
			double num4 = num2 - topLevel.Elevation;
			val5.Set(num4);
		}
	}

	private List<ElementId> CreateSingleRun(Stairs stairs, StairCreationParameters parameters, Level bottomLevel)
	{
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Expected O, but got Unknown
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[楼梯创建] 开始创建梯段，总跑数: ");
		defaultInterpolatedStringHandler.AppendFormatted(parameters.StepsPerRun.Count);
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		double num = parameters.TreadDepthMm / 304.8;
		double actualRunWidth = parameters.RunWidthMm / 304.8;
		double num2 = parameters.RiserHeightMm / 304.8;
		XYZ insertPoint = parameters.InsertPoint;
		double num3 = bottomLevel.Elevation + parameters.BaseElevationM;
		double num4 = parameters.WellWidthMm / 304.8 + parameters.RunWidthMm / 304.8;
		XYZ val = null;
		List<ElementId> list = new List<ElementId>();
		for (int i = 0; i < parameters.StepsPerRun.Count; i++)
		{
			int num5 = parameters.StepsPerRun[i];
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(19, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[楼梯创建] 创建第 ");
			defaultInterpolatedStringHandler2.AppendFormatted(i + 1);
			defaultInterpolatedStringHandler2.AppendLiteral(" 跑，踏步数: ");
			defaultInterpolatedStringHandler2.AppendFormatted(num5);
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			XYZ runDirection = GetRunDirection(parameters, i);
			XYZ offsetDirection = GetOffsetDirection(parameters, i);
			double num6 = ((i == 0) ? (0.0 - num4) : num4);
			XYZ val2;
			XYZ val3;
			if (i == 0)
			{
				val2 = new XYZ(insertPoint.X + offsetDirection.X * num6, insertPoint.Y + offsetDirection.Y * num6, num3);
				val3 = val2 + runDirection * ((double)(num5 - 1) * num);
			}
			else
			{
				double num7 = num3 + num2 * 2.0;
				if (val == null)
				{
					throw new InvalidOperationException("前一跑终点不能为空");
				}
				val2 = new XYZ(val.X + offsetDirection.X * num6, val.Y + offsetDirection.Y * num6, num7);
				val3 = val2 + runDirection * ((double)(num5 - 1) * num);
			}
			Line val4 = Line.CreateBound(val2, val3);
			StairsRun val5 = null;
			try
			{
				val5 = StairsRun.CreateStraightRun(_document, ((Element)stairs).Id, val4, (StairsRunJustification)1);
				if (val5 != null)
				{
					val5.ActualRunWidth = actualRunWidth;
					list.Add(((Element)val5).Id);
				}
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(17, 2);
				defaultInterpolatedStringHandler3.AppendLiteral("[楼梯创建] 第 ");
				defaultInterpolatedStringHandler3.AppendFormatted(i + 1);
				defaultInterpolatedStringHandler3.AppendLiteral(" 跑创建失败: ");
				defaultInterpolatedStringHandler3.AppendFormatted(ex.Message);
				Logger.Error(defaultInterpolatedStringHandler3.ToStringAndClear());
			}
			val = val3;
			num3 += num2 * (double)num5;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(24, 1);
		defaultInterpolatedStringHandler4.AppendLiteral("[楼梯创建] 所有梯段创建完成，最终标高: ");
		defaultInterpolatedStringHandler4.AppendFormatted(num3 * 304.8, "F0");
		defaultInterpolatedStringHandler4.AppendLiteral("mm");
		Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
		return list;
	}

	private XYZ GetRunDirection(StairCreationParameters parameters, int runIndex)
	{
		bool flag = runIndex % 2 == 1;
		XYZ val = parameters.StartDirection ?? XYZ.BasisX;
		return flag ? (-val) : val;
	}

	private XYZ GetOffsetDirection(StairCreationParameters parameters, int runIndex)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		bool flag = parameters.Direction == StairDirection.Left;
		XYZ runDirection = GetRunDirection(parameters, runIndex);
		if (flag)
		{
			return new XYZ(0.0 - runDirection.Y, runDirection.X, 0.0);
		}
		return new XYZ(runDirection.Y, 0.0 - runDirection.X, 0.0);
	}

	private StairsType? GetStairsType()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Element obj = new FilteredElementCollector(_document).OfClass(typeof(StairsType)).FirstElement();
		return (StairsType?)(object)((obj is StairsType) ? obj : null);
	}
}
