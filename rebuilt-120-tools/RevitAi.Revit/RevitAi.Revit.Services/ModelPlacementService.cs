using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using ns6;

using Document = Autodesk.Revit.DB.Document;

namespace RevitAi.Revit.Services;

public sealed class ModelPlacementService : IModelPlacementService
{
	private readonly IRoadProjectManager _projectManager;

	private readonly IRoadProjectService _projectService;

	private readonly ILogger _logger;

	public ModelPlacementService(IRoadProjectManager projectManager, IRoadProjectService projectService)
	{
		_projectManager = projectManager;
		_projectService = projectService;
		_logger = ServiceProvider.GetLogger();
	}

	public Result<ModelPlacementResult> PlaceModelAtStation(object document, ModelPlacementConfig config)
	{
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Result<ModelPlacementResult>.Failure("文档类型无效");
			}
			if (config == null)
			{
				return Result<ModelPlacementResult>.Failure("配置不能为空");
			}
			RoadProject project = _projectManager.GetProject(config.ProjectId);
			if (project == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendLiteral("项目 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(config.ProjectId);
				defaultInterpolatedStringHandler.AppendLiteral(" 不存在");
				return Result<ModelPlacementResult>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			if (!project.IsGenerated)
			{
				Result<List<RoadCenterlinePoint3D>> val2 = _projectService.Calculate3DCenterline(project);
				if (val2.IsFailure)
				{
					return Result<ModelPlacementResult>.Failure("项目曲线计算失败: " + val2.Error);
				}
			}
			Result<RoadCenterlinePoint3D> pointAtStation = _projectService.GetPointAtStation(project, config.StationKm);
			if (pointAtStation.IsFailure)
			{
				return Result<ModelPlacementResult>.Failure("无法获取桩号位置: " + pointAtStation.Error);
			}
			RoadCenterlinePoint3D value = pointAtStation.Value;
			(double, double, double) positionMeters = CalculatePositionWithOffset(value, config.LateralOffset, config.VerticalOffset, project.GlobalOriginOffset);
			FamilySymbol val3 = FindFamilySymbol(val, config.FamilyName, config.FamilyTypeName);
			if (val3 == null)
			{
				return Result<ModelPlacementResult>.Failure("找不到族: " + config.FamilyName + " - " + config.FamilyTypeName);
			}
			if (!val3.IsActive)
			{
				val3.Activate();
				val.Regenerate();
			}
			FamilyInstance val4 = CreateFamilyInstanceAtPosition(val, val3, positionMeters, config.RotationAngle, value.Azimuth);
			if (val4 == null)
			{
				return Result<ModelPlacementResult>.Failure("创建族实例失败");
			}
			ILogger logger = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(38, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[ModelPlacementService] 在桩号 ");
			defaultInterpolatedStringHandler2.AppendFormatted(config.StationKm, "F3");
			defaultInterpolatedStringHandler2.AppendLiteral(" (");
			defaultInterpolatedStringHandler2.AppendFormatted(value.GetFormattedStation());
			defaultInterpolatedStringHandler2.AppendLiteral(") 放置模型成功");
			logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			return Result<ModelPlacementResult>.Success(new ModelPlacementResult
			{
				IsSuccess = true,
				ElementId = ((Element)val4).Id,
				StationKm = config.StationKm,
				Position = (value.X, value.Y, value.Z)
			});
		}
		catch (Exception ex)
		{
			_logger.Error("[ModelPlacementService] 单个放置失败", ex);
			return Result<ModelPlacementResult>.Failure("放置失败: " + ex.Message);
		}
	}

	public Result<List<ModelPlacementResult>> PlaceModelsAlongRoad(object document, Guid projectId, double startStationKm, double endStationKm, double interval, string familyName, string familyTypeName, double lateralOffset = 0.0)
	{
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Expected O, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Result<List<ModelPlacementResult>>.Failure("文档类型无效");
			}
			if (interval <= 0.0)
			{
				return Result<List<ModelPlacementResult>>.Failure("间距必须大于0");
			}
			RoadProject project = _projectManager.GetProject(projectId);
			if (project == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendLiteral("项目 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(projectId);
				defaultInterpolatedStringHandler.AppendLiteral(" 不存在");
				return Result<List<ModelPlacementResult>>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			if (!project.IsGenerated)
			{
				Result<List<RoadCenterlinePoint3D>> val2 = _projectService.Calculate3DCenterline(project);
				if (val2.IsFailure)
				{
					return Result<List<ModelPlacementResult>>.Failure("项目曲线计算失败: " + val2.Error);
				}
			}
			Result<List<RoadCenterlinePoint3D>> pointsInRange = _projectService.GetPointsInRange(project, startStationKm, endStationKm);
			if (pointsInRange.IsFailure)
			{
				return Result<List<ModelPlacementResult>>.Failure("无法获取范围内的点: " + pointsInRange.Error);
			}
			_ = pointsInRange.Value;
			List<double> list = new List<double>();
			for (double num = startStationKm; num <= endStationKm; num += interval / 1000.0)
			{
				list.Add(num);
			}
			FamilySymbol val3 = FindFamilySymbol(val, familyName, familyTypeName);
			if (val3 == null)
			{
				return Result<List<ModelPlacementResult>>.Failure("找不到族: " + familyName + " - " + familyTypeName);
			}
			if (!val3.IsActive)
			{
				val3.Activate();
				val.Regenerate();
			}
			List<ModelPlacementResult> list2 = new List<ModelPlacementResult>();
			Transaction val4 = new Transaction(val, "批量放置模型");
			try
			{
				val4.Start();
				foreach (double item in list)
				{
					Result<RoadCenterlinePoint3D> pointAtStation = _projectService.GetPointAtStation(project, item);
					if (pointAtStation.IsFailure)
					{
						ILogger logger = _logger;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(7, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("跳过桩号 ");
						defaultInterpolatedStringHandler2.AppendFormatted(item, "F3");
						defaultInterpolatedStringHandler2.AppendLiteral(": ");
						defaultInterpolatedStringHandler2.AppendFormatted(pointAtStation.Error);
						logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
						continue;
					}
					RoadCenterlinePoint3D value = pointAtStation.Value;
					(double, double, double) positionMeters = CalculatePositionWithOffset(value, lateralOffset, 0.0, project.GlobalOriginOffset);
					FamilyInstance val5 = CreateFamilyInstanceAtPosition(val, val3, positionMeters, 0.0, value.Azimuth);
					if (val5 != null)
					{
						list2.Add(new ModelPlacementResult
						{
							IsSuccess = true,
							ElementId = ((Element)val5).Id,
							StationKm = item,
							Position = (value.X, value.Y, value.Z)
						});
					}
				}
				val4.Commit();
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
			ILogger logger2 = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(38, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("[ModelPlacementService] 等间距放置完成，成功 ");
			defaultInterpolatedStringHandler3.AppendFormatted(list2.Count);
			defaultInterpolatedStringHandler3.AppendLiteral("/");
			defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler3.AppendLiteral(" 个");
			logger2.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
			return Result<List<ModelPlacementResult>>.Success(list2);
		}
		catch (Exception ex)
		{
			_logger.Error("[ModelPlacementService] 等间距放置失败", ex);
			return Result<List<ModelPlacementResult>>.Failure("放置失败: " + ex.Message);
		}
	}

	public Result<List<ModelPlacementResult>> PlaceMultipleModels(object document, List<ModelPlacementConfig> configs)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Result<List<ModelPlacementResult>>.Failure("文档类型无效");
			}
			if (configs == null || configs.Count == 0)
			{
				return Result<List<ModelPlacementResult>>.Failure("配置列表不能为空");
			}
			List<ModelPlacementResult> list = new List<ModelPlacementResult>();
			int num = 0;
			Transaction val2 = new Transaction(val, "批量放置模型");
			try
			{
				val2.Start();
				foreach (ModelPlacementConfig config in configs)
				{
					Result<ModelPlacementResult> val3 = PlaceModelAtStation(document, config);
					if (val3.IsSuccess)
					{
						list.Add(val3.Value);
						continue;
					}
					num++;
					list.Add(new ModelPlacementResult
					{
						IsSuccess = false,
						Error = val3.Error,
						StationKm = config.StationKm
					});
				}
				val2.Commit();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			ILogger logger = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[ModelPlacementService] 批量放置完成，成功 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count - num);
			defaultInterpolatedStringHandler.AppendLiteral("/");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个");
			logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return Result<List<ModelPlacementResult>>.Success(list);
		}
		catch (Exception ex)
		{
			_logger.Error("[ModelPlacementService] 批量放置失败", ex);
			return Result<List<ModelPlacementResult>>.Failure("放置失败: " + ex.Message);
		}
	}

	public Result<ModelPlacementPreview> PreviewPlacement(Guid projectId, double stationKm, double lateralOffset = 0.0)
	{
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		try
		{
			RoadProject project = _projectManager.GetProject(projectId);
			if (project == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendLiteral("项目 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(projectId);
				defaultInterpolatedStringHandler.AppendLiteral(" 不存在");
				return Result<ModelPlacementPreview>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			if (!project.IsGenerated)
			{
				Result<List<RoadCenterlinePoint3D>> val = _projectService.Calculate3DCenterline(project);
				if (val.IsFailure)
				{
					return Result<ModelPlacementPreview>.Failure("项目曲线计算失败: " + val.Error);
				}
			}
			Result<RoadCenterlinePoint3D> pointAtStation = _projectService.GetPointAtStation(project, stationKm);
			if (pointAtStation.IsFailure)
			{
				return Result<ModelPlacementPreview>.Failure("无法获取桩号位置: " + pointAtStation.Error);
			}
			RoadCenterlinePoint3D value = pointAtStation.Value;
			CalculatePositionWithOffset(value, lateralOffset, 0.0, project.GlobalOriginOffset);
			return Result<ModelPlacementPreview>.Success(new ModelPlacementPreview
			{
				StationKm = stationKm,
				Position = (value.X, value.Y, value.Z),
				Azimuth = value.Azimuth * 180.0 / Math.PI,
				FormattedStation = value.GetFormattedStation()
			});
		}
		catch (Exception ex)
		{
			_logger.Error("[ModelPlacementService] 预览失败", ex);
			return Result<ModelPlacementPreview>.Failure("预览失败: " + ex.Message);
		}
	}

	private FamilySymbol? FindFamilySymbol(Document doc, string? familyName, string? typeName)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!string.IsNullOrEmpty(familyName) && !string.IsNullOrEmpty(typeName))
			{
				List<FamilySymbol> source = (from FamilySymbol fs in (IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol))
					where ((ElementType)fs).FamilyName == familyName && ((Element)fs).Name == typeName
					select fs).ToList();
				return source.FirstOrDefault();
			}
			if (!string.IsNullOrEmpty(typeName))
			{
				List<FamilySymbol> source2 = (from FamilySymbol fs in (IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol))
					where ((Element)fs).Name == typeName
					select fs).ToList();
				return source2.FirstOrDefault();
			}
			return null;
		}
		catch (Exception ex)
		{
			_logger.Error("[ModelPlacementService] 查找族符号失败: " + ex.Message, (Exception)null);
			return null;
		}
	}

	private FamilyInstance? CreateFamilyInstanceAtPosition(Document doc, FamilySymbol symbol, (double X, double Y, double Z) positionMeters, double rotationAngleDeg, double azimuthRad)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		try
		{
			XYZ val = new XYZ(positionMeters.X / 0.3048, positionMeters.Y / 0.3048, positionMeters.Z / 0.3048);
			double num = rotationAngleDeg * Math.PI / 180.0 + azimuthRad;
			FamilyInstance val2 = ((ItemFactoryBase)doc.Create).NewFamilyInstance(val, symbol, (StructuralType)0);
			if (val2 != null && Math.Abs(num) > 1E-06)
			{
				Line val3 = Line.CreateBound(val, new XYZ(val.X, val.Y, val.Z + 1.0));
				ElementTransformUtils.RotateElement(doc, ((Element)val2).Id, val3, num);
			}
			return val2;
		}
		catch (Exception ex)
		{
			_logger.Error("[ModelPlacementService] 创建族实例失败: " + ex.Message, (Exception)null);
			return null;
		}
	}

	private (double X, double Y, double Z) CalculatePositionWithOffset(RoadCenterlinePoint3D point3D, double lateralOffsetMeters, double verticalOffsetMeters, (double X, double Y, double Z) globalOriginOffset)
	{
		double azimuth = point3D.Azimuth;
		double num = (0.0 - lateralOffsetMeters) * Math.Sin(azimuth);
		double num2 = lateralOffsetMeters * Math.Cos(azimuth);
		return (X: point3D.X + num + globalOriginOffset.X, Y: point3D.Y + num2 + globalOriginOffset.Y, Z: point3D.Z + verticalOffsetMeters + globalOriginOffset.Z);
	}
}
