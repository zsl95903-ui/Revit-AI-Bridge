using System;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Services;

public static class SatelliteMapImportHelper
{
	private static SatelliteMapImportEventHandler? _currentHandler;

	private static ExternalEvent? _currentEvent;

	public static void PrepareImport()
	{
		try
		{
			_currentHandler = new SatelliteMapImportEventHandler();
			_currentEvent = SatelliteMapImportEventHandler.CreateEvent(_currentHandler);
		}
		catch (Exception ex)
		{
			Logger.Error("[SatelliteMapImportHelper] 预创建 ExternalEvent 失败: " + ex.Message);
			throw;
		}
	}

	public static void StartImport(BoundingBox boundingBox, string mapSource, int zoomLevel = 18, string? tiandituToken = null, string? googleMapsApiKey = null, Action? onCompleted = null)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (_currentHandler == null || _currentEvent == null)
			{
				Logger.Error("[SatelliteMapImportHelper] ExternalEvent 尚未预创建，请先调用 PrepareImport()");
				throw new InvalidOperationException("ExternalEvent 尚未预创建。必须在 Revit API 上下文中先调用 PrepareImport()。");
			}
			_currentHandler.BoundingBox = boundingBox;
			_currentHandler.MapSource = mapSource;
			_currentHandler.ZoomLevel = zoomLevel;
			_currentHandler.TiandituToken = tiandituToken;
			_currentHandler.GoogleMapsApiKey = googleMapsApiKey;
			_currentHandler.OnCompleted = onCompleted;
			_currentEvent.Raise();
		}
		catch (Exception ex)
		{
			Logger.Error("[SatelliteMapImportHelper] 启动导入失败: " + ex.Message);
			throw;
		}
	}

	public static void Cleanup()
	{
		try
		{
			_currentHandler = null;
			_currentEvent = null;
			Logger.Info("[SatelliteMapImportHelper] 已清理事件处理器");
		}
		catch (Exception ex)
		{
			Logger.Warning("[SatelliteMapImportHelper] 清理失败: " + ex.Message);
		}
	}
}
