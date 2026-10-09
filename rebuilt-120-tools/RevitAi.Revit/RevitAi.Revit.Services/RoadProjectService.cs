using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using ns6;

namespace RevitAi.Revit.Services;

public sealed class RoadProjectService : IRoadProjectService
{
	private readonly IInfrastructureService _infrastructureService;

	private readonly ILogger _logger;

	public RoadProjectService(IInfrastructureService infrastructureService)
	{
		_infrastructureService = infrastructureService;
		_logger = ServiceProvider.GetLogger();
	}

	public Result<List<RoadCenterlinePoint3D>> Calculate3DCenterline(RoadProject project)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Invalid comparison between Unknown and I4
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Invalid comparison between Unknown and I4
		//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Expected O, but got Unknown
		try
		{
			if (project == null)
			{
				return Result<List<RoadCenterlinePoint3D>>.Failure("项目不能为空");
			}
			if (!project.Validate())
			{
				return Result<List<RoadCenterlinePoint3D>>.Failure("项目数据验证失败");
			}
			if ((int)project.DataSource == 2)
			{
				if (project.HorizontalCurveTable == null || project.VerticalCurveTable == null)
				{
					return Result<List<RoadCenterlinePoint3D>>.Failure("平曲线表或竖曲线表为空");
				}
				List<HorizontalCurveIP> sortedPoints = project.HorizontalCurveTable.GetSortedPoints();
				List<VerticalCurvePVI> sortedPoints2 = project.VerticalCurveTable.GetSortedPoints();
				if (sortedPoints.Count < 2 || sortedPoints2.Count < 2)
				{
					return Result<List<RoadCenterlinePoint3D>>.Failure("平曲线或竖曲线数据点不足");
				}
				double station = sortedPoints[0].Station;
				double station2 = sortedPoints[sortedPoints.Count - 1].Station;
				double station3 = sortedPoints2[0].Station;
				double station4 = sortedPoints2[sortedPoints2.Count - 1].Station;
				double num = Math.Max(station, station3);
				double num2 = Math.Min(station2, station4);
				if (num >= num2)
				{
					return Result<List<RoadCenterlinePoint3D>>.Failure("平曲线和竖曲线的桩号范围无交集");
				}
				double num3 = num2 - num;
				double num4 = num3 * 1000.0;
				double num5 = 0.5;
				int num6 = (int)Math.Floor(num4 / num5) + 1;
				double num7 = num3 / (double)(num6 - 1);
				double value = num7 * 1000.0;
				List<double> list = new List<double>(num6);
				for (int i = 0; i < num6; i++)
				{
					double item = num + (double)i * num7;
					list.Add(item);
				}
				list[list.Count - 1] = num2;
				ILogger logger = _logger;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 3);
				defaultInterpolatedStringHandler.AppendLiteral("[RoadProjectService] 共同桩号范围: K");
				defaultInterpolatedStringHandler.AppendFormatted(num, "F3");
				defaultInterpolatedStringHandler.AppendLiteral(" ~ K");
				defaultInterpolatedStringHandler.AppendFormatted(num2, "F3");
				defaultInterpolatedStringHandler.AppendLiteral("，总长 ");
				defaultInterpolatedStringHandler.AppendFormatted(num4, "F2");
				defaultInterpolatedStringHandler.AppendLiteral("米");
				logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				ILogger logger2 = _logger;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(37, 3);
				defaultInterpolatedStringHandler2.AppendLiteral("[RoadProjectService] 点数: ");
				defaultInterpolatedStringHandler2.AppendFormatted(num6);
				defaultInterpolatedStringHandler2.AppendLiteral("，实际间距: ");
				defaultInterpolatedStringHandler2.AppendFormatted(value, "F4");
				defaultInterpolatedStringHandler2.AppendLiteral("米（≤");
				defaultInterpolatedStringHandler2.AppendFormatted(num5);
				defaultInterpolatedStringHandler2.AppendLiteral("米）");
				logger2.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
				Result<List<RoadCenterlinePoint3D>> val = _infrastructureService.GenerateUnifiedSamplePoints(project.HorizontalCurveTable, project.VerticalCurveTable, list);
				if (val.IsSuccess && val.Value != null)
				{
					project.Centerline3DPoints = val.Value;
					ILogger logger3 = _logger;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(56, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("[RoadProjectService] ✅ 统一采样成功，生成 ");
					defaultInterpolatedStringHandler3.AppendFormatted(val.Value.Count);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个点（固定间距 0.5米，消除双重插值误差）");
					logger3.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
				}
				return val;
			}
			if ((int)project.DataSource == 1 && project.StationElevationData != null)
			{
				List<RoadCenterlinePoint3D> list2 = project.StationElevationData.OrderBy((RoadCenterlinePoint3D p) => p.StationKm).ToList();
				if (list2.Count < 2)
				{
					return Result<List<RoadCenterlinePoint3D>>.Failure("桩号高程表数据点不足，至少需要2个点");
				}
				List<RoadCenterlinePoint3D> list3 = new List<RoadCenterlinePoint3D>(list2.Count);
				for (int num8 = 0; num8 < list2.Count; num8++)
				{
					RoadCenterlinePoint3D val2 = list2[num8];
					RoadCenterlinePoint3D val3 = new RoadCenterlinePoint3D
					{
						StationKm = val2.StationKm,
						X = val2.X,
						Y = val2.Y,
						Z = val2.Z,
						PointType = (RoadCenterlinePointType)((num8 != 0) ? ((num8 == list2.Count - 1) ? 1 : 9) : 0)
					};
					if (num8 == 0)
					{
						if (list2.Count > 1)
						{
							RoadCenterlinePoint3D val4 = list2[1];
							double x = val4.X - val2.X;
							double y = val4.Y - val2.Y;
							val3.Azimuth = Math.Atan2(y, x);
							while (val3.Azimuth < 0.0)
							{
								val3.Azimuth += Math.PI * 2.0;
							}
							while (val3.Azimuth >= Math.PI * 2.0)
							{
								val3.Azimuth -= Math.PI * 2.0;
							}
						}
					}
					else if (num8 == list2.Count - 1)
					{
						RoadCenterlinePoint3D val5 = list2[num8 - 1];
						double x2 = val2.X - val5.X;
						double y2 = val2.Y - val5.Y;
						val3.Azimuth = Math.Atan2(y2, x2);
						while (val3.Azimuth < 0.0)
						{
							val3.Azimuth += Math.PI * 2.0;
						}
						while (val3.Azimuth >= Math.PI * 2.0)
						{
							val3.Azimuth -= Math.PI * 2.0;
						}
					}
					else
					{
						RoadCenterlinePoint3D val6 = list2[num8 - 1];
						RoadCenterlinePoint3D val7 = list2[num8 + 1];
						double x3 = val2.X - val6.X;
						double y3 = val2.Y - val6.Y;
						double num9 = Math.Atan2(y3, x3);
						double x4 = val7.X - val2.X;
						double y4 = val7.Y - val2.Y;
						double num10 = Math.Atan2(y4, x4);
						double num11 = num10 - num9;
						if (num11 > Math.PI)
						{
							num9 += Math.PI * 2.0;
						}
						else if (num11 < -Math.PI)
						{
							num9 -= Math.PI * 2.0;
						}
						val3.Azimuth = (num9 + num10) / 2.0;
						while (val3.Azimuth < 0.0)
						{
							val3.Azimuth += Math.PI * 2.0;
						}
						while (val3.Azimuth >= Math.PI * 2.0)
						{
							val3.Azimuth -= Math.PI * 2.0;
						}
					}
					if (num8 == 0 || num8 == list2.Count - 1)
					{
						val3.Curvature = 0.0;
					}
					else
					{
						RoadCenterlinePoint3D val8 = list2[num8 - 1];
						RoadCenterlinePoint3D val9 = list2[num8 + 1];
						double num12 = Math.Atan2(val2.Y - val8.Y, val2.X - val8.X);
						double num13 = Math.Atan2(val9.Y - val2.Y, val9.X - val2.X);
						double num14;
						for (num14 = num13 - num12; num14 > Math.PI; num14 -= Math.PI * 2.0)
						{
						}
						for (; num14 < -Math.PI; num14 += Math.PI * 2.0)
						{
						}
						double num15 = Math.Sqrt(Math.Pow(val2.X - val8.X, 2.0) + Math.Pow(val2.Y - val8.Y, 2.0));
						double num16 = Math.Sqrt(Math.Pow(val9.X - val2.X, 2.0) + Math.Pow(val9.Y - val2.Y, 2.0));
						double num17 = num15 + num16;
						if (num17 > 1E-06)
						{
							val3.Curvature = Math.Abs(num14) / num17;
						}
						else
						{
							val3.Curvature = 0.0;
						}
					}
					list3.Add(val3);
				}
				Result<List<RoadCenterlinePoint3D>> val = Result<List<RoadCenterlinePoint3D>>.Success(list3);
				project.Centerline3DPoints = list3;
				List<RoadCenterlinePoint3D> list4 = list3.Take(Math.Min(5, list3.Count)).ToList();
				string value2 = string.Join(", ", list4.Select(delegate(RoadCenterlinePoint3D p)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(4, 2);
					defaultInterpolatedStringHandler7.AppendLiteral("K");
					defaultInterpolatedStringHandler7.AppendFormatted(p.StationKm * 1000.0, "F1");
					defaultInterpolatedStringHandler7.AppendLiteral(": ");
					defaultInterpolatedStringHandler7.AppendFormatted(p.Azimuth * 180.0 / Math.PI, "F2");
					defaultInterpolatedStringHandler7.AppendLiteral("°");
					return defaultInterpolatedStringHandler7.ToStringAndClear();
				}));
				ILogger logger4 = _logger;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(55, 1);
				defaultInterpolatedStringHandler4.AppendLiteral("[RoadProjectService] 三维曲线计算成功（桩号位置高程表），生成 ");
				defaultInterpolatedStringHandler4.AppendFormatted(list3.Count);
				defaultInterpolatedStringHandler4.AppendLiteral(" 个点，已计算方位角和曲率");
				logger4.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
				ILogger logger5 = _logger;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(32, 2);
				defaultInterpolatedStringHandler5.AppendLiteral("[RoadProjectService] 前 ");
				defaultInterpolatedStringHandler5.AppendFormatted(list4.Count);
				defaultInterpolatedStringHandler5.AppendLiteral(" 个点的方位角: ");
				defaultInterpolatedStringHandler5.AppendFormatted(value2);
				logger5.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
				return val;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(11, 1);
			defaultInterpolatedStringHandler6.AppendLiteral("不支持的数据源类型: ");
			defaultInterpolatedStringHandler6.AppendFormatted<RoadCenterlineDataSource>(project.DataSource);
			return Result<List<RoadCenterlinePoint3D>>.Failure(defaultInterpolatedStringHandler6.ToStringAndClear());
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectService] 计算三维曲线失败", ex);
			return Result<List<RoadCenterlinePoint3D>>.Failure("计算三维曲线失败: " + ex.Message);
		}
	}

	public Result<RoadCenterlinePoint3D> GetPointAtStation(RoadProject project, double stationKm)
	{
		try
		{
			if (project == null)
			{
				return Result<RoadCenterlinePoint3D>.Failure("项目不能为空");
			}
			RoadCenterlinePoint3D pointAtStation = project.GetPointAtStation(stationKm);
			if (pointAtStation != null)
			{
				return Result<RoadCenterlinePoint3D>.Success(pointAtStation);
			}
			if (!project.IsGenerated)
			{
				Result<List<RoadCenterlinePoint3D>> val = Calculate3DCenterline(project);
				if (val.IsFailure)
				{
					return Result<RoadCenterlinePoint3D>.Failure("无法获取点：" + val.Error);
				}
			}
			pointAtStation = project.GetPointAtStation(stationKm);
			if (pointAtStation != null)
			{
				return Result<RoadCenterlinePoint3D>.Success(pointAtStation);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
			defaultInterpolatedStringHandler.AppendLiteral("桩号 ");
			defaultInterpolatedStringHandler.AppendFormatted(stationKm, "F3");
			defaultInterpolatedStringHandler.AppendLiteral(" 超出范围");
			return Result<RoadCenterlinePoint3D>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectService] 查找三维点失败", ex);
			return Result<RoadCenterlinePoint3D>.Failure("查找三维点失败: " + ex.Message);
		}
	}

	public Result<List<RoadCenterlinePoint3D>> GetPointsInRange(RoadProject project, double startKm, double endKm)
	{
		try
		{
			if (project == null)
			{
				return Result<List<RoadCenterlinePoint3D>>.Failure("项目不能为空");
			}
			if (!project.IsGenerated)
			{
				Result<List<RoadCenterlinePoint3D>> val = Calculate3DCenterline(project);
				if (val.IsFailure)
				{
					return Result<List<RoadCenterlinePoint3D>>.Failure("无法获取点：" + val.Error);
				}
			}
			if (project.Centerline3DPoints == null)
			{
				return Result<List<RoadCenterlinePoint3D>>.Failure("三维曲线数据为空");
			}
			List<RoadCenterlinePoint3D> list = project.Centerline3DPoints.Where((RoadCenterlinePoint3D p) => p.StationKm >= startKm && p.StationKm <= endKm).ToList();
			if (list.Count == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 2);
				defaultInterpolatedStringHandler.AppendLiteral("桩号范围 [");
				defaultInterpolatedStringHandler.AppendFormatted(startKm, "F3");
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted(endKm, "F3");
				defaultInterpolatedStringHandler.AppendLiteral("] 内无数据点");
				return Result<List<RoadCenterlinePoint3D>>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			ILogger logger = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(39, 3);
			defaultInterpolatedStringHandler2.AppendLiteral("[RoadProjectService] 获取桩号范围 [");
			defaultInterpolatedStringHandler2.AppendFormatted(startKm, "F3");
			defaultInterpolatedStringHandler2.AppendLiteral(", ");
			defaultInterpolatedStringHandler2.AppendFormatted(endKm, "F3");
			defaultInterpolatedStringHandler2.AppendLiteral("] 内的 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个点");
			logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			return Result<List<RoadCenterlinePoint3D>>.Success(list);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectService] 获取范围内点失败", ex);
			return Result<List<RoadCenterlinePoint3D>>.Failure("获取范围内点失败: " + ex.Message);
		}
	}

	public Task<Dictionary<Guid, Result<List<RoadCenterlinePoint3D>>>> CalculateMultipleProjectsAsync(IEnumerable<RoadProject> projects)
	{
		return Task.Run(delegate
		{
			Dictionary<Guid, Result<List<RoadCenterlinePoint3D>>> dictionary = new Dictionary<Guid, Result<List<RoadCenterlinePoint3D>>>();
			foreach (RoadProject project in projects)
			{
				Result<List<RoadCenterlinePoint3D>> value = Calculate3DCenterline(project);
				dictionary[project.Id] = value;
			}
			ILogger logger = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[RoadProjectService] 批量计算完成，共 ");
			defaultInterpolatedStringHandler.AppendFormatted(dictionary.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个项目");
			logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return dictionary;
		});
	}
}
