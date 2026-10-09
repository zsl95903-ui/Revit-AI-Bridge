using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using ns0;
using ns6;

using Document = Autodesk.Revit.DB.Document;

namespace RevitAi.Revit.Services;

public sealed class CurveService : ICurveService
{
	private class MassWarningHandler : IFailuresPreprocessor
	{
		public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
		{
			IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();
			foreach (FailureMessageAccessor item in failureMessages)
			{
				string descriptionText = item.GetDescriptionText();
				if (descriptionText.Contains("体量中不包含实心几何图形") || descriptionText.Contains("Mass contains no solid geometry") || descriptionText.Contains("将不会计算体量楼层") || descriptionText.Contains("will not calculate mass floors"))
				{
					failuresAccessor.DeleteWarning(item);
				}
			}
			return (FailureProcessingResult)0;
		}
	}

	private readonly UIApplication _application;

	private readonly IInfrastructureService _infrastructureService;

	public CurveService(UIApplication application, IInfrastructureService infrastructureService)
	{
		_application = application ?? throw new ArgumentNullException("application");
		_infrastructureService = infrastructureService ?? throw new ArgumentNullException("infrastructureService");
	}

	public Result<object> CreateLine(object document, double x1, double y1, double z1, double x2, double y2, double z2)
	{
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				Logger.Error("CreateLine: document 不是 Document 类型");
				return Result<object>.Failure("document 类型无效");
			}
			XYZ val2 = new XYZ(x1 / 3.28084, y1 / 3.28084, z1 / 3.28084);
			XYZ val3 = new XYZ(x2 / 3.28084, y2 / 3.28084, z2 / 3.28084);
			Line val4 = Line.CreateBound(val2, val3);
			if ((GeometryObject)(object)val4 == (GeometryObject)null)
			{
				Logger.Error("CreateLine: Line.CreateBound 返回 null");
				return Result<object>.Failure("创建直线失败");
			}
			XYZ basisZ = XYZ.BasisZ;
			Plane val5 = Plane.CreateByNormalAndOrigin(basisZ, val2);
			SketchPlane val6 = SketchPlane.Create(val, val5);
			if (val6 != null)
			{
				Transaction val7 = new Transaction(val, "创建直线");
				try
				{
					val7.Start();
					try
					{
						ModelCurve val8 = ((ItemFactoryBase)val.FamilyCreate).NewModelCurve((Curve)(object)val4, val6);
						if (val8 == null)
						{
							Logger.Error("CreateLine: NewModelCurve 返回 null");
							val7.RollBack();
							return Result<object>.Failure("创建模型线失败");
						}
						val7.Commit();
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
						defaultInterpolatedStringHandler.AppendLiteral("成功创建直线，ID: ");
						defaultInterpolatedStringHandler.AppendFormatted(((Element)val8).Id.Value);
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						return Result<object>.Success((object)val8);
					}
					catch (Exception ex)
					{
						Logger.Error("CreateLine: 事务执行失败 - " + ex.Message);
						val7.RollBack();
						return Result<object>.Failure(ex.Message);
					}
				}
				finally
				{
					((IDisposable)val7)?.Dispose();
				}
			}
			Logger.Error("CreateLine: 创建草图平面失败");
			return Result<object>.Failure("创建草图平面失败");
		}
		catch (Exception ex2)
		{
			Logger.Error("CreateLine 失败: " + ex2.Message);
			return Result<object>.Failure(ex2.Message);
		}
	}

	public Result<object> CreateArc(object document, double centerX, double centerY, double centerZ, double radius, double startAngle, double endAngle)
	{
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Expected O, but got Unknown
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				Logger.Error("CreateArc: document 不是 Document 类型");
				return Result<object>.Failure("document 类型无效");
			}
			XYZ val2 = new XYZ(centerX / 3.28084, centerY / 3.28084, centerZ / 3.28084);
			double num = radius / 3.28084;
			double num2 = startAngle * Math.PI / 180.0;
			double num3 = endAngle * Math.PI / 180.0;
			double num4 = val2.X + num * Math.Cos(num2);
			double num5 = val2.Y + num * Math.Sin(num2);
			double num6 = val2.X + num * Math.Cos(num3);
			double num7 = val2.Y + num * Math.Sin(num3);
			XYZ val3 = new XYZ(num4, num5, val2.Z);
			XYZ val4 = new XYZ(num6, num7, val2.Z);
			double num8 = (num2 + num3) / 2.0;
			double num9 = val2.X + num * Math.Cos(num8);
			double num10 = val2.Y + num * Math.Sin(num8);
			XYZ val5 = new XYZ(num9, num10, val2.Z);
			Arc val6 = Arc.Create(val3, val4, val5);
			if ((GeometryObject)(object)val6 == (GeometryObject)null)
			{
				Logger.Error("CreateArc: Arc.Create 返回 null");
				return Result<object>.Failure("创建圆弧失败");
			}
			Plane val7 = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, val2);
			SketchPlane val8 = SketchPlane.Create(val, val7);
			if (val8 != null)
			{
				Transaction val9 = new Transaction(val, "创建圆弧");
				try
				{
					val9.Start();
					try
					{
						ModelCurve val10 = ((ItemFactoryBase)val.FamilyCreate).NewModelCurve((Curve)(object)val6, val8);
						if (val10 == null)
						{
							Logger.Error("CreateArc: NewModelCurve 返回 null");
							val9.RollBack();
							return Result<object>.Failure("创建模型线失败");
						}
						val9.Commit();
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
						defaultInterpolatedStringHandler.AppendLiteral("成功创建圆弧，ID: ");
						defaultInterpolatedStringHandler.AppendFormatted(((Element)val10).Id.Value);
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						return Result<object>.Success((object)val10);
					}
					catch (Exception ex)
					{
						Logger.Error("CreateArc: 事务执行失败 - " + ex.Message);
						val9.RollBack();
						return Result<object>.Failure(ex.Message);
					}
				}
				finally
				{
					((IDisposable)val9)?.Dispose();
				}
			}
			Logger.Error("CreateArc: 创建草图平面失败");
			return Result<object>.Failure("创建草图平面失败");
		}
		catch (Exception ex2)
		{
			Logger.Error("CreateArc 失败: " + ex2.Message);
			return Result<object>.Failure(ex2.Message);
		}
	}

	public Result<object> CreateSplineCurve(object document, List<(double X, double Y, double Z)> points)
	{
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				Logger.Error("CreateSplineCurve: document 不是 Document 类型");
				return Result<object>.Failure("document 类型无效");
			}
			if (points.Count < 2)
			{
				Logger.Error("CreateSplineCurve: 至少需要2个点");
				return Result<object>.Failure("至少需要2个点");
			}
			List<XYZ> list = new List<XYZ>();
			foreach (var (num, num2, num3) in points)
			{
				list.Add(new XYZ(num / 3.28084, num2 / 3.28084, num3 / 3.28084));
			}
			Plane val2 = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, list[0]);
			SketchPlane val3 = SketchPlane.Create(val, val2);
			if (val3 != null)
			{
				Transaction val4 = new Transaction(val, "创建样条曲线");
				try
				{
					val4.Start();
					try
					{
						ModelCurve val5 = null;
						int num4 = 0;
						while (true)
						{
							if (num4 < list.Count - 1)
							{
								Line val6 = Line.CreateBound(list[num4], list[num4 + 1]);
								ModelCurve val7 = ((ItemFactoryBase)val.FamilyCreate).NewModelCurve((Curve)(object)val6, val3);
								if (val7 == null)
								{
									break;
								}
								if (val5 == null)
								{
									val5 = val7;
								}
								num4++;
								continue;
							}
							val4.Commit();
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
							defaultInterpolatedStringHandler.AppendLiteral("成功创建");
							defaultInterpolatedStringHandler.AppendFormatted(list.Count - 1);
							defaultInterpolatedStringHandler.AppendLiteral("段直线");
							Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
							if (val5 == null)
							{
								return Result<object>.Failure("未能创建任何曲线");
							}
							return Result<object>.Success((object)val5);
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(51, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("CreateSplineCurve: NewModelCurve 返回 null (segment ");
						defaultInterpolatedStringHandler2.AppendFormatted(num4);
						defaultInterpolatedStringHandler2.AppendLiteral(")");
						Logger.Error(defaultInterpolatedStringHandler2.ToStringAndClear());
						val4.RollBack();
						return Result<object>.Failure("创建模型线失败");
					}
					catch (Exception ex)
					{
						Logger.Error("CreateSplineCurve: 事务执行失败 - " + ex.Message);
						val4.RollBack();
						return Result<object>.Failure(ex.Message);
					}
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
			}
			Logger.Error("CreateSplineCurve: 创建草图平面失败");
			return Result<object>.Failure("创建草图平面失败");
		}
		catch (Exception ex2)
		{
			Logger.Error("CreateSplineCurve 失败: " + ex2.Message);
			return Result<object>.Failure(ex2.Message);
		}
	}

	public Result<List<object>> CreateRoadCenterlineCurves(object document, List<RoadCenterlinePoint3D> points3D, bool convertToFeet = true)
	{
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				Logger.Error("CreateRoadCenterlineCurves: document 不是 Document 类型");
				return Result<List<object>>.Failure("document 类型无效");
			}
			if (points3D.Count >= 2)
			{
				List<object> list = new List<object>();
				Transaction val2 = new Transaction(val, "创建道路中心线");
				try
				{
					val2.Start();
					try
					{
						RoadCenterlinePoint3D val3 = points3D[0];
						XYZ val4 = (convertToFeet ? new XYZ(val3.X / 304.8, val3.Y / 304.8, val3.Z / 304.8) : new XYZ(val3.X, val3.Y, val3.Z));
						Plane val5 = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, val4);
						SketchPlane val6 = SketchPlane.Create(val, val5);
						if (val6 == null)
						{
							Logger.Error("CreateRoadCenterlineCurves: 创建草图平面失败");
							val2.RollBack();
							return Result<List<object>>.Failure("创建草图平面失败");
						}
						List<RoadCenterlinePoint3D> list2 = new List<RoadCenterlinePoint3D>();
						double curvature = points3D[0].Curvature;
						for (int i = 0; i < points3D.Count; i++)
						{
							RoadCenterlinePoint3D val7 = points3D[i];
							if (i > 0 && Math.Abs(val7.Curvature - curvature) > 0.0001)
							{
								if (list2.Count >= 2)
								{
									ModelCurve val8 = CreateCurveSegment(val, val6, list2, convertToFeet, 304.8);
									if (val8 != null)
									{
										list.Add(val8);
									}
								}
								list2.Clear();
								curvature = val7.Curvature;
							}
							list2.Add(val7);
							if (i == points3D.Count - 1 && list2.Count >= 2)
							{
								ModelCurve val9 = CreateCurveSegment(val, val6, list2, convertToFeet, 304.8);
								if (val9 != null)
								{
									list.Add(val9);
								}
							}
						}
						val2.Commit();
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
						defaultInterpolatedStringHandler.AppendLiteral("成功创建道路中心线，共");
						defaultInterpolatedStringHandler.AppendFormatted(list.Count);
						defaultInterpolatedStringHandler.AppendLiteral("段曲线");
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						return Result<List<object>>.Success(list);
					}
					catch (Exception ex)
					{
						Logger.Error("CreateRoadCenterlineCurves: 事务执行失败 - " + ex.Message);
						val2.RollBack();
						return Result<List<object>>.Failure(ex.Message);
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			Logger.Error("CreateRoadCenterlineCurves: 至少需要2个点");
			return Result<List<object>>.Failure("至少需要2个点");
		}
		catch (Exception ex2)
		{
			Logger.Error("CreateRoadCenterlineCurves 失败: " + ex2.Message);
			return Result<List<object>>.Failure(ex2.Message);
		}
	}

	private ModelCurve? CreateCurveSegment(Document doc, SketchPlane sketchPlane, List<RoadCenterlinePoint3D> points, bool convertToFeet, double mToFt)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Expected O, but got Unknown
		//IL_009b: Expected O, but got Unknown
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Expected O, but got Unknown
		try
		{
			if (points.Count == 2)
			{
				RoadCenterlinePoint3D val = points[0];
				RoadCenterlinePoint3D val2 = points[1];
				XYZ val3 = (convertToFeet ? new XYZ(val.X / mToFt, val.Y / mToFt, val.Z / mToFt) : new XYZ(val.X, val.Y, val.Z));
				XYZ val4 = (convertToFeet ? new XYZ(val2.X / mToFt, val2.Y / mToFt, val2.Z / mToFt) : new XYZ(val2.X, val2.Y, val2.Z));
				double num = val3.DistanceTo(val4);
				if (num < 0.05)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
					defaultInterpolatedStringHandler.AppendLiteral("CreateCurveSegment: 跳过过短线段，长度=");
					defaultInterpolatedStringHandler.AppendFormatted(num * 304.8, "F2");
					defaultInterpolatedStringHandler.AppendLiteral(" 毫米");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					return null;
				}
				Line val5 = Line.CreateBound(val3, val4);
				return ((ItemFactoryBase)doc.FamilyCreate).NewModelCurve((Curve)(object)val5, sketchPlane);
			}
			ModelCurve val6 = null;
			for (int i = 0; i < points.Count - 1; i++)
			{
				RoadCenterlinePoint3D val7 = points[i];
				RoadCenterlinePoint3D val8 = points[i + 1];
				XYZ val9 = (convertToFeet ? new XYZ(val7.X / mToFt, val7.Y / mToFt, val7.Z / mToFt) : new XYZ(val7.X, val7.Y, val7.Z));
				XYZ val10 = (convertToFeet ? new XYZ(val8.X / mToFt, val8.Y / mToFt, val8.Z / mToFt) : new XYZ(val8.X, val8.Y, val8.Z));
				double num2 = val9.DistanceTo(val10);
				if (num2 < 0.05)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(32, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("CreateCurveSegment: 跳过过短线段，长度=");
					defaultInterpolatedStringHandler2.AppendFormatted(num2 / 3.28084, "F2");
					defaultInterpolatedStringHandler2.AppendLiteral(" 米");
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
				else
				{
					Line val11 = Line.CreateBound(val9, val10);
					ModelCurve val12 = ((ItemFactoryBase)doc.FamilyCreate).NewModelCurve((Curve)(object)val11, sketchPlane);
					if (val6 == null)
					{
						val6 = val12;
					}
				}
			}
			return val6;
		}
		catch (Exception ex)
		{
			Logger.Error("CreateCurveSegment 失败: " + ex.Message);
			return null;
		}
	}

	public Result<RoadCenterlineMassInfo> CreateRoadCenterlineInMass(object document, List<RoadCenterlinePoint3D> points3D, string? massTemplateName = null, string? roadProjectName = null)
	{
		//IL_0f4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f25: Unknown result type (might be due to invalid IL or missing references)
		//IL_1038: Unknown result type (might be due to invalid IL or missing references)
		//IL_103d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1045: Unknown result type (might be due to invalid IL or missing references)
		//IL_104d: Unknown result type (might be due to invalid IL or missing references)
		//IL_105f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1069: Expected O, but got Unknown
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Expected O, but got Unknown
		//IL_09fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a03: Expected O, but got Unknown
		//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2a: Expected O, but got Unknown
		//IL_0a7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b67: Expected O, but got Unknown
		//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Expected O, but got Unknown
		//IL_0bb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c91: Expected O, but got Unknown
		//IL_0cab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e01: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				Logger.Error("CreateRoadCenterlineInMass: document 不是 Document 类型");
				return Result<RoadCenterlineMassInfo>.Failure("document 类型无效");
			}
			if (points3D.Count < 2)
			{
				Logger.Error("CreateRoadCenterlineInMass: 至少需要2个点");
				return Result<RoadCenterlineMassInfo>.Failure("至少需要2个点");
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
			defaultInterpolatedStringHandler.AppendLiteral("CreateRoadCenterlineInMass: 输入点数=");
			defaultInterpolatedStringHandler.AppendFormatted(points3D.Count);
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(28, 5);
			defaultInterpolatedStringHandler2.AppendLiteral("  起点: 桩号=");
			defaultInterpolatedStringHandler2.AppendFormatted(points3D.First().StationKm, "F3");
			defaultInterpolatedStringHandler2.AppendLiteral(" 公里 (");
			defaultInterpolatedStringHandler2.AppendFormatted(points3D.First().GetFormattedStation());
			defaultInterpolatedStringHandler2.AppendLiteral("), 坐标=(");
			defaultInterpolatedStringHandler2.AppendFormatted(points3D.First().X, "F2");
			defaultInterpolatedStringHandler2.AppendLiteral(", ");
			defaultInterpolatedStringHandler2.AppendFormatted(points3D.First().Y, "F2");
			defaultInterpolatedStringHandler2.AppendLiteral(", ");
			defaultInterpolatedStringHandler2.AppendFormatted(points3D.First().Z, "F2");
			defaultInterpolatedStringHandler2.AppendLiteral(") 米");
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(28, 5);
			defaultInterpolatedStringHandler3.AppendLiteral("  终点: 桩号=");
			defaultInterpolatedStringHandler3.AppendFormatted(points3D.Last().StationKm, "F3");
			defaultInterpolatedStringHandler3.AppendLiteral(" 公里 (");
			defaultInterpolatedStringHandler3.AppendFormatted(points3D.Last().GetFormattedStation());
			defaultInterpolatedStringHandler3.AppendLiteral("), 坐标=(");
			defaultInterpolatedStringHandler3.AppendFormatted(points3D.Last().X, "F2");
			defaultInterpolatedStringHandler3.AppendLiteral(", ");
			defaultInterpolatedStringHandler3.AppendFormatted(points3D.Last().Y, "F2");
			defaultInterpolatedStringHandler3.AppendLiteral(", ");
			defaultInterpolatedStringHandler3.AppendFormatted(points3D.Last().Z, "F2");
			defaultInterpolatedStringHandler3.AppendLiteral(") 米");
			Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
			double num = points3D.Last().X - points3D.First().X;
			double num2 = points3D.Last().Y - points3D.First().Y;
			double num3 = points3D.Last().Z - points3D.First().Z;
			double num4 = Math.Sqrt(num * num + num2 * num2 + num3 * num3);
			double num5 = (points3D.Last().StationKm - points3D.First().StationKm) * 1000.0;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler4.AppendLiteral("  桩号差值: ");
			defaultInterpolatedStringHandler4.AppendFormatted(num5, "F2");
			defaultInterpolatedStringHandler4.AppendLiteral(" 米");
			Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler5.AppendLiteral("  几何距离: ");
			defaultInterpolatedStringHandler5.AppendFormatted(num4, "F2");
			defaultInterpolatedStringHandler5.AppendLiteral(" 米");
			Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(11, 1);
			defaultInterpolatedStringHandler6.AppendLiteral("  点间距: ");
			defaultInterpolatedStringHandler6.AppendFormatted(num4 / (double)(points3D.Count - 1), "F3");
			defaultInterpolatedStringHandler6.AppendLiteral(" 米/点");
			Logger.Info(defaultInterpolatedStringHandler6.ToStringAndClear());
			if (Math.Abs(num5 - num4) > num4 * 0.1)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(34, 2);
				defaultInterpolatedStringHandler7.AppendLiteral("⚠️ 桩号差值(");
				defaultInterpolatedStringHandler7.AppendFormatted(num5, "F2");
				defaultInterpolatedStringHandler7.AppendLiteral("米)与几何距离(");
				defaultInterpolatedStringHandler7.AppendFormatted(num4, "F2");
				defaultInterpolatedStringHandler7.AppendLiteral("米)相差较大！可能存在单位转换问题。");
				Logger.Warning(defaultInterpolatedStringHandler7.ToStringAndClear());
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(47, 2);
			defaultInterpolatedStringHandler8.AppendLiteral("CreateRoadCenterlineInMass: 开始点去重，原始点数=");
			defaultInterpolatedStringHandler8.AppendFormatted(points3D.Count);
			defaultInterpolatedStringHandler8.AppendLiteral("，最小间距=");
			defaultInterpolatedStringHandler8.AppendFormatted(0.0100584, "F3");
			defaultInterpolatedStringHandler8.AppendLiteral(" 米");
			Logger.Info(defaultInterpolatedStringHandler8.ToStringAndClear());
			List<RoadCenterlinePoint3D> list = new List<RoadCenterlinePoint3D>();
			int num6 = 0;
			int num7 = 0;
			foreach (RoadCenterlinePoint3D item2 in points3D)
			{
				if (list.Count == 0)
				{
					list.Add(item2);
					continue;
				}
				RoadCenterlinePoint3D val2 = list[list.Count - 1];
				double num8 = item2.X - val2.X;
				double num9 = item2.Y - val2.Y;
				double num10 = item2.Z - val2.Z;
				double num11 = Math.Sqrt(num8 * num8 + num9 * num9 + num10 * num10);
				if (num11 >= 0.0100584)
				{
					list.Add(item2);
					num7 = 0;
				}
				else
				{
					num6++;
					num7++;
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(49, 2);
			defaultInterpolatedStringHandler9.AppendLiteral("CreateRoadCenterlineInMass: 点去重完成，过滤后点数=");
			defaultInterpolatedStringHandler9.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler9.AppendLiteral("，跳过=");
			defaultInterpolatedStringHandler9.AppendFormatted(num6);
			defaultInterpolatedStringHandler9.AppendLiteral(" 个过近点");
			Logger.Info(defaultInterpolatedStringHandler9.ToStringAndClear());
			if (list.Count < 2)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(48, 1);
				defaultInterpolatedStringHandler10.AppendLiteral("CreateRoadCenterlineInMass: 去重后点数不足，至少需要2个点（当前=");
				defaultInterpolatedStringHandler10.AppendFormatted(list.Count);
				defaultInterpolatedStringHandler10.AppendLiteral("）");
				Logger.Error(defaultInterpolatedStringHandler10.ToStringAndClear());
				return Result<RoadCenterlineMassInfo>.Failure("去重后点数不足，无法创建曲线");
			}
			List<XYZ> list2 = new List<XYZ>();
			foreach (RoadCenterlinePoint3D item3 in list)
			{
				list2.Add(new XYZ(item3.X / 0.3048, item3.Y / 0.3048, item3.Z / 0.3048));
			}
			string text = massTemplateName;
			if (string.IsNullOrEmpty(text))
			{
				string location = Assembly.GetExecutingAssembly().Location;
				string directoryName = Path.GetDirectoryName(location);
				if (directoryName != null)
				{
					string[] obj = new string[3]
					{
						"Resources\\FamilyTemplates\\公制体量.rft",
						null,
						null
					};
					InlineArray6<string> gparam_ = default(InlineArray6<string>);
					Class653.smethod_2<InlineArray6<string>, string>(ref gparam_, 0) = directoryName;
					Class653.smethod_2<InlineArray6<string>, string>(ref gparam_, 1) = "..";
					Class653.smethod_2<InlineArray6<string>, string>(ref gparam_, 2) = "..";
					Class653.smethod_2<InlineArray6<string>, string>(ref gparam_, 3) = "Resources";
					Class653.smethod_2<InlineArray6<string>, string>(ref gparam_, 4) = "FamilyTemplates";
					Class653.smethod_2<InlineArray6<string>, string>(ref gparam_, 5) = "公制体量.rft";
					obj[1] = Path.Combine(Class653.smethod_1<InlineArray6<string>, string>(in gparam_, 6));
					InlineArray5<string> gparam_2 = default(InlineArray5<string>);
					Class653.smethod_2<InlineArray5<string>, string>(ref gparam_2, 0) = directoryName;
					Class653.smethod_2<InlineArray5<string>, string>(ref gparam_2, 1) = "..";
					Class653.smethod_2<InlineArray5<string>, string>(ref gparam_2, 2) = "Resources";
					Class653.smethod_2<InlineArray5<string>, string>(ref gparam_2, 3) = "FamilyTemplates";
					Class653.smethod_2<InlineArray5<string>, string>(ref gparam_2, 4) = "公制体量.rft";
					obj[2] = Path.Combine(Class653.smethod_1<InlineArray5<string>, string>(in gparam_2, 5));
					string[] array = obj;
					string[] array2 = array;
					foreach (string path in array2)
					{
						try
						{
							string fullPath = Path.GetFullPath(path);
							if (File.Exists(fullPath))
							{
								text = fullPath;
								break;
							}
						}
						catch
						{
						}
					}
				}
			}
			if (!string.IsNullOrEmpty(text) && File.Exists(text))
			{
				string tempPath = Path.GetTempPath();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler11 = new DefaultInterpolatedStringHandler(28, 2);
				defaultInterpolatedStringHandler11.AppendLiteral("AS_Tools_RoadCenterline_");
				defaultInterpolatedStringHandler11.AppendFormatted(string.IsNullOrEmpty(roadProjectName) ? "" : (roadProjectName + "_"));
				defaultInterpolatedStringHandler11.AppendFormatted(Guid.NewGuid(), "N");
				defaultInterpolatedStringHandler11.AppendLiteral(".rfa");
				string text2 = Path.Combine(tempPath, defaultInterpolatedStringHandler11.ToStringAndClear());
				try
				{
					Document val3 = _application.Application.NewFamilyDocument(text);
					if (val3 == null)
					{
						Logger.Error("CreateRoadCenterlineInMass: NewFamilyDocument 返回 null");
						return Result<RoadCenterlineMassInfo>.Failure("创建族文档失败");
					}
					List<Curve> list3 = new List<Curve>();
					for (int j = 0; j < list2.Count - 1; j++)
					{
						Line item = Line.CreateBound(list2[j], list2[j + 1]);
						list3.Add((Curve)(object)item);
					}
					if (list3.Count == 0)
					{
						Logger.Error("CreateRoadCenterlineInMass: 创建曲线失败");
						val3.Close(false);
						return Result<RoadCenterlineMassInfo>.Failure("创建曲线失败");
					}
					Curve curve3D = list3[0];
					Transaction val4 = new Transaction(val3, "创建道路中心线");
					try
					{
						val4.SetFailureHandlingOptions(val4.GetFailureHandlingOptions().SetFailuresPreprocessor((IFailuresPreprocessor)(object)new MassWarningHandler()));
						val4.Start();
						try
						{
							ReferencePointArray val5 = new ReferencePointArray();
							foreach (XYZ item4 in list2)
							{
								ReferencePoint val6 = val3.FamilyCreate.NewReferencePoint(item4);
								if (val6 != null)
								{
									val5.Append(val6);
									continue;
								}
								Logger.Error("CreateRoadCenterlineInMass: NewReferencePoint 返回 null");
								val4.RollBack();
								val3.Close(false);
								return Result<RoadCenterlineMassInfo>.Failure("创建参考点失败");
							}
							CurveByPoints val7 = val3.FamilyCreate.NewCurveByPoints(val5);
							if (val7 == null)
							{
								Logger.Error("CreateRoadCenterlineInMass: NewCurveByPoints 返回 null");
								val4.RollBack();
								val3.Close(false);
								return Result<RoadCenterlineMassInfo>.Failure("创建 3D 曲线失败");
							}
							val4.Commit();
						}
						catch (Exception ex)
						{
							Logger.Error("CreateRoadCenterlineInMass: 事务执行失败 - " + ex.Message);
							val4.RollBack();
							val3.Close(false);
							return Result<RoadCenterlineMassInfo>.Failure("事务执行失败: " + ex.Message);
						}
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
					SaveAsOptions val8 = new SaveAsOptions();
					val8.OverwriteExistingFile = true;
					val3.SaveAs(text2, val8);
					val3.Close(false);
					Family val9 = null;
					Transaction val10 = new Transaction(val, "加载体量族");
					try
					{
						val10.SetFailureHandlingOptions(val10.GetFailureHandlingOptions().SetFailuresPreprocessor((IFailuresPreprocessor)(object)new MassWarningHandler()));
						val10.Start();
						try
						{
							if (!val.LoadFamily(text2, out val9))
							{
								Logger.Error("CreateRoadCenterlineInMass: LoadFamily 失败");
								val10.RollBack();
								return Result<RoadCenterlineMassInfo>.Failure("加载体量族失败");
							}
							val10.Commit();
						}
						catch (Exception ex2)
						{
							Logger.Error("CreateRoadCenterlineInMass: 加载族事务失败 - " + ex2.Message);
							val10.RollBack();
							return Result<RoadCenterlineMassInfo>.Failure("加载族失败: " + ex2.Message);
						}
					}
					finally
					{
						((IDisposable)val10)?.Dispose();
					}
					if (val9 == null)
					{
						Logger.Error("CreateRoadCenterlineInMass: family 为 null");
						return Result<RoadCenterlineMassInfo>.Failure("族加载失败");
					}
					FamilyInstance val11 = null;
					Transaction val12 = new Transaction(val, "放置体量族实例");
					try
					{
						val12.SetFailureHandlingOptions(val12.GetFailureHandlingOptions().SetFailuresPreprocessor((IFailuresPreprocessor)(object)new MassWarningHandler()));
						val12.Start();
						try
						{
							ElementId val13 = val9.GetFamilySymbolIds().FirstOrDefault();
							if (val13 == ElementId.InvalidElementId)
							{
								Logger.Error("CreateRoadCenterlineInMass: 族没有有效的类型");
								val12.RollBack();
								return Result<RoadCenterlineMassInfo>.Failure("族没有有效的类型");
							}
							Element element = val.GetElement(val13);
							FamilySymbol val14 = (FamilySymbol)(object)((element is FamilySymbol) ? element : null);
							if (val14 == null)
							{
								Logger.Error("CreateRoadCenterlineInMass: 无法获取族类型");
								val12.RollBack();
								return Result<RoadCenterlineMassInfo>.Failure("无法获取族类型");
							}
							if (!val14.IsActive)
							{
								val14.Activate();
							}
							XYZ zero = XYZ.Zero;
							MethodInfo method = ((object)val.Create).GetType().GetMethod("NewFamilyInstance", new Type[3]
							{
								typeof(XYZ),
								typeof(FamilySymbol),
								typeof(int)
							});
							if (method != null)
							{
								object? obj3 = method.Invoke(val.Create, new object[3] { zero, val14, 0 });
								val11 = (FamilyInstance)((obj3 is FamilyInstance) ? obj3 : null);
							}
							else
							{
								val11 = ((ItemFactoryBase)val.Create).NewFamilyInstance(zero, val14, (StructuralType)0);
							}
							if (val11 == null)
							{
								Logger.Error("CreateRoadCenterlineInMass: NewFamilyInstance 返回 null");
								val12.RollBack();
								return Result<RoadCenterlineMassInfo>.Failure("放置族实例失败");
							}
							try
							{
								View activeView = val.ActiveView;
								if (activeView != null)
								{
									Category val15 = val.Settings.Categories.get_Item((BuiltInCategory)(-2003400L));
									if (val15 != null && activeView != null)
									{
										View val16 = activeView;
										if (true)
										{
											val16.SetCategoryHidden(val15.Id, false);
											Logger.Info("已自动打开当前视图中体量的可见性");
										}
									}
								}
							}
							catch (Exception ex3)
							{
								Logger.Warning("设置体量可见性时出现警告: " + ex3.Message);
							}
							if (!string.IsNullOrEmpty(roadProjectName))
							{
								try
								{
									((Element)val11).Name = "RoadCenterline_" + roadProjectName;
									Parameter val17 = ((Element)val11).get_Parameter((BuiltInParameter)(-1010106L));
									if (val17 != null)
									{
										val17.Set(roadProjectName);
									}
									Logger.Info("已设置体量实例名称: RoadCenterline_" + roadProjectName + "，注释: " + roadProjectName);
								}
								catch (Exception ex4)
								{
									Logger.Warning("设置体量实例名称或注释参数时出现警告: " + ex4.Message);
								}
							}
							val12.Commit();
						}
						catch (Exception ex5)
						{
							Logger.Error("CreateRoadCenterlineInMass: 放置实例事务失败 - " + ex5.Message);
							val12.RollBack();
							return Result<RoadCenterlineMassInfo>.Failure("放置实例失败: " + ex5.Message);
						}
					}
					finally
					{
						((IDisposable)val12)?.Dispose();
					}
					double num12 = 0.0;
					foreach (Curve item5 in list3)
					{
						IList<XYZ> list4 = item5.Tessellate();
						for (int k = 1; k < list4.Count; k++)
						{
							double num13 = list4[k].X - list4[k - 1].X;
							double num14 = list4[k].Y - list4[k - 1].Y;
							num12 += Math.Sqrt(num13 * num13 + num14 * num14);
						}
					}
					RoadCenterlineMassInfo val18 = new RoadCenterlineMassInfo
					{
						MassInstance = val11,
						Curve3D = curve3D,
						TotalHorizontalLengthMeters = num12 * 0.3048,
						TempFamilyPath = text2
					};
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler12 = new DefaultInterpolatedStringHandler(23, 1);
					defaultInterpolatedStringHandler12.AppendLiteral("成功创建体量族中的道路中心线，水平长度: ");
					defaultInterpolatedStringHandler12.AppendFormatted(val18.TotalHorizontalLengthMeters, "F2");
					defaultInterpolatedStringHandler12.AppendLiteral(" 米");
					Logger.Info(defaultInterpolatedStringHandler12.ToStringAndClear());
					return Result<RoadCenterlineMassInfo>.Success(val18);
				}
				catch (Exception ex6)
				{
					Logger.Error("CreateRoadCenterlineInMass 失败: " + ex6.Message);
					return Result<RoadCenterlineMassInfo>.Failure(ex6.Message);
				}
			}
			Logger.Error("CreateRoadCenterlineInMass: 体量样板不存在 - " + text);
			return Result<RoadCenterlineMassInfo>.Failure("体量样板不存在: " + text);
		}
		catch (Exception ex7)
		{
			Logger.Error("CreateRoadCenterlineInMass 异常: " + ex7.Message);
			return Result<RoadCenterlineMassInfo>.Failure(ex7.Message);
		}
	}

	public Result<int> PlaceStationAnnotations(object document, object curve, List<RoadCenterlinePoint3D> points3D, string familyPath, double stationInterval = 100.0, string? symbolName = null, RoadProject? roadProject = null)
	{
		//IL_0f5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Invalid comparison between Unknown and I4
		//IL_0d69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd2: Expected O, but got Unknown
		//IL_0dd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Expected O, but got Unknown
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Expected O, but got Unknown
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Expected O, but got Unknown
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Expected O, but got Unknown
		//IL_0e1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b62: Invalid comparison between Unknown and I4
		//IL_0b74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7a: Invalid comparison between Unknown and I4
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Expected O, but got Unknown
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		string text = null;
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				Logger.Error("PlaceStationAnnotations: document 不是 Document 类型");
				return Result<int>.Failure("document 类型无效");
			}
			if (points3D == null || points3D.Count < 2)
			{
				Logger.Error("PlaceStationAnnotations: points3D 数据无效");
				return Result<int>.Failure("points3D 数据无效");
			}
			List<XYZ> list = new List<XYZ>();
			List<double> list2 = new List<double> { 0.0 };
			foreach (RoadCenterlinePoint3D item4 in points3D)
			{
				list.Add(new XYZ(item4.X / 0.3048, item4.Y / 0.3048, item4.Z / 0.3048));
				if (list.Count > 1)
				{
					int num = list.Count - 1;
					double num2 = list[num].X - list[num - 1].X;
					double num3 = list[num].Y - list[num - 1].Y;
					double num4 = list[num].Z - list[num - 1].Z;
					double num5 = Math.Sqrt(num2 * num2 + num3 * num3 + num4 * num4);
					list2.Add(list2[num - 1] + num5);
				}
			}
			string text2 = "AST_R_桩号标注";
			Family val2 = null;
			FilteredElementCollector val3 = new FilteredElementCollector(val);
			val3.OfClass(typeof(Family));
			foreach (Element item5 in val3)
			{
				Family val4 = (Family)(object)((item5 is Family) ? item5 : null);
				if (val4 != null && ((Element)val4).Name.Equals(text2, StringComparison.OrdinalIgnoreCase))
				{
					val2 = val4;
					break;
				}
			}
			Family val5 = val2;
			string text3 = null;
			if (val5 == null)
			{
				if (!File.Exists(familyPath))
				{
					Logger.Error("PlaceStationAnnotations: 族文件不存在 - " + familyPath);
					return Result<int>.Failure("族文件不存在: " + familyPath);
				}
				try
				{
					string text4 = string.Join("_", text2.Split(Path.GetInvalidFileNameChars()));
					if (string.IsNullOrWhiteSpace(text4))
					{
						text4 = Guid.NewGuid().ToString("N");
					}
					text = Path.Combine(Path.GetTempPath(), text4 + ".rfa");
					File.Copy(familyPath, text, overwrite: true);
					text3 = text;
				}
				catch (Exception ex)
				{
					Logger.Error("PlaceStationAnnotations: 复制族文件失败 - " + ex.Message);
					return Result<int>.Failure("复制族文件失败: " + ex.Message);
				}
			}
			else
			{
				Logger.Info("PlaceStationAnnotations: 项目中已存在族 '" + text2 + "'，跳过加载");
			}
			List<double> list3 = new List<double>();
			double stationKm = points3D[0].StationKm;
			double stationKm2 = points3D[points3D.Count - 1].StationKm;
			double num6 = stationInterval * 0.3048;
			double num7 = stationKm * 1000.0;
			double num8 = stationKm2 * 1000.0;
			for (double num9 = num7; num9 <= num8; num9 += num6)
			{
				list3.Add(num9);
			}
			if (!list3.Contains(num8))
			{
				list3.Add(num8);
			}
			int num10 = 0;
			List<ElementId> list4 = new List<ElementId>();
			Transaction val6 = new Transaction(val, "放置桩号标注");
			try
			{
				val6.Start();
				try
				{
					if (val5 == null && text3 != null)
					{
						try
						{
							FamilyLoadOptions familyLoadOptions = new FamilyLoadOptions();
							if (!val.LoadFamily(text3, (IFamilyLoadOptions)(object)familyLoadOptions, out val5))
							{
								Logger.Error("PlaceStationAnnotations: LoadFamily 失败 - " + text3);
								val6.RollBack();
								return Result<int>.Failure("加载族文件失败");
							}
						}
						catch (Exception ex2)
						{
							Logger.Error("PlaceStationAnnotations: 加载族文件失败 - " + ex2.Message);
							val6.RollBack();
							return Result<int>.Failure("加载族文件失败: " + ex2.Message);
						}
					}
					if (val5 == null)
					{
						Logger.Error("PlaceStationAnnotations: 族不可用");
						val6.RollBack();
						return Result<int>.Failure("族不可用");
					}
					FamilySymbol val7 = null;
					foreach (ElementId familySymbolId in val5.GetFamilySymbolIds())
					{
						Element element = val.GetElement(familySymbolId);
						FamilySymbol val8 = (FamilySymbol)(object)((element is FamilySymbol) ? element : null);
						if (val8 != null && (string.IsNullOrEmpty(symbolName) || ((Element)val8).Name == symbolName))
						{
							val7 = val8;
							break;
						}
					}
					if (val7 == null)
					{
						Logger.Error("PlaceStationAnnotations: 无法获取族类型");
						val6.RollBack();
						return Result<int>.Failure("无法获取族类型");
					}
					if (!val7.IsActive)
					{
						val7.Activate();
					}
					foreach (double item6 in list3)
					{
						double num11 = item6 / 1000.0;
						XYZ val9 = null;
						double num12 = 0.0;
						if (roadProject != null && (int)roadProject.DataSource == 2 && roadProject.HorizontalCurveTable != null && roadProject.VerticalCurveTable != null)
						{
							Result<RoadCenterlinePoint3D> val10 = _infrastructureService.GeneratePointAtStation(roadProject.HorizontalCurveTable, roadProject.VerticalCurveTable, num11);
							if (val10.IsSuccess && val10.Value != null)
							{
								RoadCenterlinePoint3D value = val10.Value;
								double item = roadProject.GlobalOriginOffset.Item1;
								double item2 = roadProject.GlobalOriginOffset.Item2;
								double item3 = roadProject.GlobalOriginOffset.Item3;
								val9 = new XYZ((value.X - item) / 0.3048, (value.Y - item2) / 0.3048, (value.Z - item3) / 0.3048);
								num12 = value.Azimuth * 180.0 / Math.PI + 90.0;
							}
							else
							{
								Logger.Warning("  ⚠️ 精确计算失败: " + val10.Error + "，降级使用线性插值");
							}
						}
						if (val9 == null)
						{
							for (int i = 0; i < points3D.Count - 1; i++)
							{
								RoadCenterlinePoint3D val11 = points3D[i];
								RoadCenterlinePoint3D val12 = points3D[i + 1];
								if (num11 >= val11.StationKm && num11 <= val12.StationKm)
								{
									double num13 = (num11 - val11.StationKm) / (val12.StationKm - val11.StationKm);
									XYZ val13 = new XYZ(val11.X / 0.3048, val11.Y / 0.3048, val11.Z / 0.3048);
									XYZ val14 = new XYZ(val12.X / 0.3048, val12.Y / 0.3048, val12.Z / 0.3048);
									val9 = val13 + (val14 - val13) * num13;
									XYZ val15 = (val14 - val13).Normalize();
									num12 = Math.Atan2(val15.Y, val15.X) * 180.0 / Math.PI + 90.0;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 7);
									defaultInterpolatedStringHandler.AppendLiteral("  找到插值点: 点");
									defaultInterpolatedStringHandler.AppendFormatted(i);
									defaultInterpolatedStringHandler.AppendLiteral(" (桩号=");
									defaultInterpolatedStringHandler.AppendFormatted(val11.StationKm, "F3");
									defaultInterpolatedStringHandler.AppendLiteral(") 到 点");
									defaultInterpolatedStringHandler.AppendFormatted(i + 1);
									defaultInterpolatedStringHandler.AppendLiteral(" (桩号=");
									defaultInterpolatedStringHandler.AppendFormatted(val12.StationKm, "F3");
									defaultInterpolatedStringHandler.AppendLiteral("), 比例=");
									defaultInterpolatedStringHandler.AppendFormatted(num13, "F3");
									defaultInterpolatedStringHandler.AppendLiteral(", 原始切线角度=");
									defaultInterpolatedStringHandler.AppendFormatted(num12 - 90.0, "F2");
									defaultInterpolatedStringHandler.AppendLiteral("°, 旋转后角度=");
									defaultInterpolatedStringHandler.AppendFormatted(num12, "F2");
									defaultInterpolatedStringHandler.AppendLiteral("°");
									Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
									break;
								}
							}
						}
						if (val9 == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(41, 3);
							defaultInterpolatedStringHandler2.AppendLiteral("  ❌ 无法找到桩号 ");
							defaultInterpolatedStringHandler2.AppendFormatted(item6, "F1");
							defaultInterpolatedStringHandler2.AppendLiteral(" 米 对应的点（points3D 桩号范围: ");
							defaultInterpolatedStringHandler2.AppendFormatted(points3D.First().StationKm, "F3");
							defaultInterpolatedStringHandler2.AppendLiteral(" - ");
							defaultInterpolatedStringHandler2.AppendFormatted(points3D.Last().StationKm, "F3");
							defaultInterpolatedStringHandler2.AppendLiteral(" 公里）");
							Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
						}
						else
						{
							if (val9 == null)
							{
								continue;
							}
							try
							{
								FamilyInstance val16 = null;
								MethodInfo method = ((object)val.Create).GetType().GetMethod("NewFamilyInstance", new Type[3]
								{
									typeof(XYZ),
									typeof(FamilySymbol),
									typeof(int)
								});
								if (method != null)
								{
									object? obj = method.Invoke(val.Create, new object[3] { val9, val7, 0 });
									val16 = (FamilyInstance)((obj is FamilyInstance) ? obj : null);
								}
								else
								{
									val16 = ((ItemFactoryBase)val.Create).NewFamilyInstance(val9, val7, (StructuralType)0);
								}
								if (val16 == null)
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(43, 1);
									defaultInterpolatedStringHandler3.AppendLiteral("  ❌ NewFamilyInstance 返回 null，无法在桩号 ");
									defaultInterpolatedStringHandler3.AppendFormatted(item6, "F1");
									defaultInterpolatedStringHandler3.AppendLiteral(" 米处创建标注");
									Logger.Warning(defaultInterpolatedStringHandler3.ToStringAndClear());
									continue;
								}
								num10++;
								list4.Add(((Element)val16).Id);
								try
								{
									double num14 = item6;
									Parameter val17 = ((Element)val16).LookupParameter("桩号");
									if (val17 != null && !((APIObject)val17).IsReadOnly)
									{
										int value2 = (int)(num14 / 1000.0);
										double value3 = num14 % 1000.0;
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(2, 2);
										defaultInterpolatedStringHandler4.AppendLiteral("K");
										defaultInterpolatedStringHandler4.AppendFormatted(value2);
										defaultInterpolatedStringHandler4.AppendLiteral("+");
										defaultInterpolatedStringHandler4.AppendFormatted(value3, "F3");
										string text5 = defaultInterpolatedStringHandler4.ToStringAndClear();
										if ((int)val17.StorageType == 3)
										{
											val17.Set(text5);
										}
										else if ((int)val17.StorageType == 2)
										{
											val17.Set(num14 / 1000.0);
										}
									}
								}
								catch
								{
								}
								try
								{
									Line val18 = Line.CreateUnbound(val9, XYZ.BasisZ);
									double num15 = num12 * Math.PI / 180.0;
									try
									{
										ElementTransformUtils.RotateElement(val, ((Element)val16).Id, val18, num15);
									}
									catch
									{
										MethodInfo method2 = typeof(ElementTransformUtils).GetMethod("RotateElement", new Type[4]
										{
											typeof(Document),
											typeof(ElementId),
											typeof(Line),
											typeof(double)
										});
										if (method2 != null)
										{
											method2.Invoke(null, new object[4]
											{
												val,
												((Element)val16).Id,
												val18,
												num15
											});
										}
										else
										{
											Logger.Warning("  ❌ 找不到 RotateElement 方法");
										}
									}
								}
								catch (Exception ex3)
								{
									Logger.Warning("  ⚠️  旋转桩号标注失败: " + ex3.GetType().Name + " - " + ex3.Message);
								}
							}
							catch (Exception ex4)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(23, 3);
								defaultInterpolatedStringHandler5.AppendLiteral("  ❌ 放置桩号标注异常在桩号 ");
								defaultInterpolatedStringHandler5.AppendFormatted(item6, "F1");
								defaultInterpolatedStringHandler5.AppendLiteral(" 米: ");
								defaultInterpolatedStringHandler5.AppendFormatted(ex4.GetType().Name);
								defaultInterpolatedStringHandler5.AppendLiteral(" - ");
								defaultInterpolatedStringHandler5.AppendFormatted(ex4.Message);
								Logger.Warning(defaultInterpolatedStringHandler5.ToStringAndClear());
								Logger.Warning("     堆栈: " + ex4.StackTrace);
							}
						}
					}
					val6.Commit();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(11, 1);
					defaultInterpolatedStringHandler6.AppendLiteral("成功放置 ");
					defaultInterpolatedStringHandler6.AppendFormatted(num10);
					defaultInterpolatedStringHandler6.AppendLiteral(" 个桩号标注");
					Logger.Info(defaultInterpolatedStringHandler6.ToStringAndClear());
					if (list4.Count > 0)
					{
						try
						{
							Transaction val19 = new Transaction(val, "创建桩号标注组");
							try
							{
								val19.Start();
								try
								{
									string text6 = ((roadProject != null) ? roadProject.Name : null);
									string targetGroupName = (string.IsNullOrEmpty(text6) ? "AST_桩号标注" : ("AST_桩号标注_" + text6));
									GroupType val20 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(GroupType))).Cast<GroupType>().FirstOrDefault((GroupType gt) => ((Element)gt).Name == targetGroupName);
									Group val21 = ((ItemFactoryBase)val.Create).NewGroup((ICollection<ElementId>)list4);
									if (val21 != null)
									{
										try
										{
											if (val20 != null && ((Element)val20).Id != ((Element)val21.GroupType).Id)
											{
												DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(1, 2);
												defaultInterpolatedStringHandler7.AppendFormatted(targetGroupName);
												defaultInterpolatedStringHandler7.AppendLiteral("_");
												defaultInterpolatedStringHandler7.AppendFormatted(DateTime.Now, "yyyyMMdd_HHmmss");
												string name = defaultInterpolatedStringHandler7.ToStringAndClear();
												((Element)val21.GroupType).Name = name;
											}
											else
											{
												((Element)val21.GroupType).Name = targetGroupName;
											}
										}
										catch (Exception ex5)
										{
											Logger.Warning("⚠️ 设置组类型名称失败: " + ex5.Message + "，当前名称: " + ((Element)val21.GroupType).Name);
										}
									}
									val19.Commit();
								}
								catch (Exception ex6)
								{
									Logger.Warning("创建模型组失败: " + ex6.Message + "，标注已成功放置但未分组");
									val19.RollBack();
								}
							}
							finally
							{
								((IDisposable)val19)?.Dispose();
							}
						}
						catch (Exception ex7)
						{
							Logger.Warning("创建模型组事务失败: " + ex7.Message + "，标注已成功放置但未分组");
						}
					}
					return Result<int>.Success(num10);
				}
				catch (Exception ex8)
				{
					Logger.Error("PlaceStationAnnotations: 事务执行失败 - " + ex8.Message);
					val6.RollBack();
					return Result<int>.Failure("事务执行失败: " + ex8.Message);
				}
			}
			finally
			{
				((IDisposable)val6)?.Dispose();
			}
		}
		catch (Exception ex9)
		{
			Logger.Error("PlaceStationAnnotations 失败: " + ex9.Message);
			return Result<int>.Failure(ex9.Message);
		}
		finally
		{
			if (text != null && File.Exists(text))
			{
				try
				{
					File.Delete(text);
				}
				catch (Exception ex10)
				{
					Logger.Warning("PlaceStationAnnotations: 删除临时族文件失败: " + ex10.Message);
				}
			}
		}
	}
}
