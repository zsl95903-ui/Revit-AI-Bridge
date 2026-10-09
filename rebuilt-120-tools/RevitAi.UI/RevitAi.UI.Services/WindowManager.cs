using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.UI.ViewModels;
using RevitAi.UI.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

using ServiceProvider = RevitAi.Abstractions.Loader.ServiceProvider;

namespace RevitAi.UI.Services;

public class WindowManager : IWindowManager
{
	private readonly IServiceProvider _serviceProvider;

	private AboutWindow? _aboutWindow;

	private FeatureStoreWindow? _featureStoreWindow;

	private BatchLinkModelsWindow? _batchLinkModelsWindow;

	private ManageModelLinksWindow? _manageModelLinksWindow;

	private RoadProjectManagementWindow? _roadProjectManagementWindow;

	private CurveInputWindow? _curveInputWindow;

	private StationElevationInputWindow? _stationElevationInputWindow;

	private RoadModelPlacementWindow? _roadModelPlacementWindow;

	private MunicipalPipelineNetworkWindow? _municipalPipelineNetworkWindow;

	private CreateBridgeComponentWindow? _createBridgeComponentWindow;

	private DefinePileWindow? _definePileWindow;

	private DefineFoundationWindow? _defineFoundationWindow;

	private DefinePierWindow? _definePierWindow;

	private DefineBeamWindow? _defineBeamWindow;

	private DefineBearingWindow? _defineBearingWindow;

	private DefineBridgeTypeWindow? _defineBridgeTypeWindow;

	private EditBridgeComponentsWindow? _editBridgeComponentsWindow;

	private FamilyLibraryWindow? _familyLibraryWindow;

	private TopographyFromFloorWindow? _topographyFromFloorWindow;

	private ExportFamilyMetadataWindow? _exportFamilyMetadataWindow;

	private WallToRoadWindow? _wallToRoadWindow;

	private CreateSubgradeModelWindow? _createSubgradeModelWindow;

	private MapSelectionWindow? _mapSelectionWindow;

	private CreateAncillaryStructureWindow? _createAncillaryStructureWindow;

	private RoadSurfaceRefinementWindow? _roadSurfaceRefinementWindow;

	private InsulationWindow? _insulationWindow;

	public WindowManager(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public void ShowLoginWindow()
	{
		ShowAboutWindow();
	}

	public void CloseLoginWindow()
	{
	}

	public void ShowMainWindow()
	{
	}

	public void CloseMainWindow()
	{
	}

	public void ShowAboutWindow()
	{
		try
		{
			if (_aboutWindow == null || !_aboutWindow.IsLoaded)
			{
				if (_aboutWindow != null)
				{
					try
					{
						_aboutWindow.Close();
					}
					catch
					{
					}
					_aboutWindow = null;
				}
				_aboutWindow = new AboutWindow();
				_aboutWindow.Closed += delegate
				{
					_aboutWindow = null;
				};
				IAuthManager requiredService = _serviceProvider.GetRequiredService<IAuthManager>();
				IDialogService requiredService2 = _serviceProvider.GetRequiredService<IDialogService>();
				_aboutWindow.DataContext = new AboutViewModel(requiredService, requiredService2, null, _aboutWindow);
			}
			if (_aboutWindow.IsVisible)
			{
				_aboutWindow.Activate();
			}
			else
			{
				_aboutWindow.Show();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowAboutWindow 异常", ex);
			throw;
		}
	}

	public void CloseAboutWindow()
	{
		_aboutWindow?.Close();
		_aboutWindow = null;
	}

	public void ShowSettingsWindow()
	{
		ShowAboutWindow();
	}

	public void CloseSettingsWindow()
	{
	}

	public void ShowFeaturePanelWindow()
	{
		ShowFeatureStoreWindow();
	}

	public void CloseFeaturePanelWindow()
	{
		CloseFeatureStoreWindow();
	}

	public void ShowAIChatPanelWindow()
	{
		try
		{
			(Type.GetType("RevitAi.Abstractions.Loader.ModuleLoader, RevitAi.Abstractions")?.GetMethod("ShowDockablePaneAIChat"))?.Invoke(null, null);
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] 显示 DockablePane 失败: " + ex.Message);
		}
	}

	public void CloseAIChatPanelWindow()
	{
		try
		{
			(Type.GetType("RevitAi.Abstractions.Loader.ModuleLoader, RevitAi.Abstractions")?.GetMethod("HideDockablePaneAIChat"))?.Invoke(null, null);
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] 隐藏 DockablePane 失败: " + ex.Message);
		}
	}

	public void ShowFeatureStoreWindow()
	{
		if (_featureStoreWindow == null)
		{
			_featureStoreWindow = new FeatureStoreWindow
			{
				DataContext = _serviceProvider.GetService<FeatureStoreViewModel>()
			};
			_featureStoreWindow.Closed += delegate
			{
				_featureStoreWindow = null;
			};
		}
		_featureStoreWindow.Show();
	}

	public void CloseFeatureStoreWindow()
	{
		_featureStoreWindow?.Close();
		_featureStoreWindow = null;
	}

	public void ShowBatchLinkModelsWindow()
	{
		if (_batchLinkModelsWindow == null)
		{
			_batchLinkModelsWindow = new BatchLinkModelsWindow
			{
				DataContext = _serviceProvider.GetService<BatchLinkModelsViewModel>()
			};
			_batchLinkModelsWindow.Closed += delegate
			{
				_batchLinkModelsWindow = null;
			};
		}
		_batchLinkModelsWindow.Show();
	}

	public void CloseBatchLinkModelsWindow()
	{
		_batchLinkModelsWindow?.Close();
		_batchLinkModelsWindow = null;
	}

	public void ShowManageModelLinksWindow()
	{
		try
		{
			if (_manageModelLinksWindow == null)
			{
				ManageModelLinksViewModel service = _serviceProvider.GetService<ManageModelLinksViewModel>();
				_manageModelLinksWindow = new ManageModelLinksWindow();
				_manageModelLinksWindow.DataContext = service;
				_manageModelLinksWindow.Closed += delegate
				{
					_manageModelLinksWindow = null;
				};
			}
			_manageModelLinksWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] 显示管理链接模型窗口失败", ex);
			throw;
		}
	}

	public void CloseManageModelLinksWindow()
	{
		_manageModelLinksWindow?.Close();
		_manageModelLinksWindow = null;
	}

	public void ShowRoadProjectManagementWindow()
	{
		try
		{
			if (_roadProjectManagementWindow == null)
			{
				RoadProjectManagementViewModel service = _serviceProvider.GetService<RoadProjectManagementViewModel>();
				if (service == null)
				{
					Logger.Error("[WindowManager] 无法从服务提供者获取 RoadProjectManagementViewModel");
					throw new InvalidOperationException("无法获取 RoadProjectManagementViewModel");
				}
				_roadProjectManagementWindow = new RoadProjectManagementWindow
				{
					DataContext = service
				};
				_roadProjectManagementWindow.Closed += delegate
				{
					_roadProjectManagementWindow = null;
				};
			}
			_roadProjectManagementWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowRoadProjectManagementWindow 执行失败", ex);
			throw;
		}
	}

	public void CloseRoadProjectManagementWindow()
	{
		_roadProjectManagementWindow?.Close();
		_roadProjectManagementWindow = null;
	}

	public void SetAIChatTopMost(bool topMost)
	{
		Logger.Info("[WindowManager] SetAIChatTopMost 不支持 DockablePane 模式");
	}

	public void SwitchToDockableMode()
	{
		ShowAIChatPanelWindow();
	}

	public void SwitchToWindowMode()
	{
		ShowAIChatPanelWindow();
	}

	public void ToggleAIChatWindow()
	{
		if (IsDockablePaneOpen())
		{
			HideAllAIWindows();
		}
		else
		{
			ShowAIChatPanelWindow();
		}
	}

	private bool IsDockablePaneOpen()
	{
		try
		{
			Type type = Assembly.Load("RevitAi.Revit")?.GetType("RevitAi.Revit.UI.AIDockablePaneHelper");
			if (type == null)
			{
				return false;
			}
			_ = type.GetMethod("ShowPane") == null;
			return false;
		}
		catch
		{
			return false;
		}
	}

	private bool IsStandaloneWindowOpen()
	{
		return false;
	}

	private void HideAllAIWindows()
	{
		try
		{
			(Type.GetType("RevitAi.Abstractions.Loader.ModuleLoader, RevitAi.Abstractions")?.GetMethod("HideDockablePaneAIChat"))?.Invoke(null, null);
		}
		catch (Exception)
		{
		}
	}

	public void HideAIChatWindow()
	{
		HideAllAIWindows();
	}

	public bool IsAIChatWindowOpen()
	{
		if (!IsStandaloneWindowOpen())
		{
			return IsDockablePaneOpen();
		}
		return true;
	}

	private void ShowAIChatWindowBasedOnConfig()
	{
		ShowAIChatPanelWindow();
	}

	private void UpdateWindowDisplayMode(string mode)
	{
		Logger.Debug("[WindowManager] UpdateWindowDisplayMode 不再支持: " + mode);
	}

	public void ShowCurveInputWindow(double offsetX, double offsetY, double offsetZ)
	{
		if (_curveInputWindow == null)
		{
			CurveInputViewModel service = _serviceProvider.GetService<CurveInputViewModel>();
			if (service == null)
			{
				Logger.Error("[WindowManager] 无法从服务提供者获取 CurveInputViewModel");
				return;
			}
			service.OriginOffsetX = offsetX;
			service.OriginOffsetY = offsetY;
			service.OriginOffsetZ = offsetZ;
			_curveInputWindow = new CurveInputWindow
			{
				DataContext = service
			};
			_curveInputWindow.Closed += delegate
			{
				_curveInputWindow = null;
			};
		}
		else if (_curveInputWindow.DataContext is CurveInputViewModel curveInputViewModel)
		{
			curveInputViewModel.OriginOffsetX = offsetX;
			curveInputViewModel.OriginOffsetY = offsetY;
			curveInputViewModel.OriginOffsetZ = offsetZ;
		}
		_curveInputWindow.Show();
	}

	public void CloseCurveInputWindow()
	{
		_curveInputWindow?.Close();
		_curveInputWindow = null;
	}

	public void ShowStationElevationInputWindow(double offsetX, double offsetY, double offsetZ)
	{
		if (_stationElevationInputWindow == null)
		{
			StationElevationInputViewModel service = _serviceProvider.GetService<StationElevationInputViewModel>();
			if (service == null)
			{
				Logger.Error("[WindowManager] 无法从服务提供者获取 StationElevationInputViewModel");
				return;
			}
			service.OriginOffsetX = offsetX;
			service.OriginOffsetY = offsetY;
			service.OriginOffsetZ = offsetZ;
			_stationElevationInputWindow = new StationElevationInputWindow
			{
				DataContext = service
			};
			_stationElevationInputWindow.Closed += delegate
			{
				_stationElevationInputWindow = null;
			};
		}
		else if (_stationElevationInputWindow.DataContext is StationElevationInputViewModel stationElevationInputViewModel)
		{
			stationElevationInputViewModel.OriginOffsetX = offsetX;
			stationElevationInputViewModel.OriginOffsetY = offsetY;
			stationElevationInputViewModel.OriginOffsetZ = offsetZ;
		}
		_stationElevationInputWindow.Show();
	}

	public void CloseStationElevationInputWindow()
	{
		_stationElevationInputWindow?.Close();
		_stationElevationInputWindow = null;
	}

	public void ShowPaymentDialog(string message, string commandText)
	{
		try
		{
			if (MessageBox.Show(message + "\n\n功能：" + commandText + "\n\n点击\"确定\"前往购买授权，点击\"取消\"返回。", "授权提示", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk) == MessageBoxResult.OK)
			{
				ShowPurchaseLicenseWindow();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] 显示授权对话框失败", ex);
		}
	}

	private void ShowPurchaseLicenseWindow()
	{
		try
		{
			PurchaseLicenseWindow purchaseLicenseWindow = new PurchaseLicenseWindow();
			IDialogService service = _serviceProvider.GetService<IDialogService>();
			if (service == null)
			{
				Logger.Error("[WindowManager] 无法获取 IDialogService 服务");
				return;
			}
			IPaymentService service2 = _serviceProvider.GetService<IPaymentService>();
			IPaymentCompletionService service3 = _serviceProvider.GetService<IPaymentCompletionService>();
			PurchaseLicenseViewModel dataContext = new PurchaseLicenseViewModel(service, purchaseLicenseWindow, service2, service3);
			purchaseLicenseWindow.DataContext = dataContext;
			purchaseLicenseWindow.ShowDialog();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] 显示购买授权窗口失败", ex);
		}
	}

	public void ShowRoadModelPlacementWindow()
	{
		try
		{
			if (_roadModelPlacementWindow == null)
			{
				RoadModelPlacementViewModel service = _serviceProvider.GetService<RoadModelPlacementViewModel>();
				if (service == null)
				{
					Logger.Error("[WindowManager] 无法从服务提供者获取 RoadModelPlacementViewModel");
					return;
				}
				_roadModelPlacementWindow = new RoadModelPlacementWindow
				{
					DataContext = service
				};
				_roadModelPlacementWindow.Closed += delegate
				{
					_roadModelPlacementWindow = null;
				};
			}
			_roadModelPlacementWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowRoadModelPlacementWindow 执行失败", ex);
			throw;
		}
	}

	public void CloseRoadModelPlacementWindow()
	{
		_roadModelPlacementWindow?.Close();
		_roadModelPlacementWindow = null;
	}

	public void ShowMunicipalPipelineNetworkWindow()
	{
		try
		{
			if (_municipalPipelineNetworkWindow == null)
			{
				MunicipalPipelineNetworkViewModel service = _serviceProvider.GetService<MunicipalPipelineNetworkViewModel>();
				if (service == null)
				{
					Logger.Error("[WindowManager] 无法从服务提供者获取 MunicipalPipelineNetworkViewModel");
					return;
				}
				_municipalPipelineNetworkWindow = new MunicipalPipelineNetworkWindow
				{
					DataContext = service
				};
				_municipalPipelineNetworkWindow.Closed += delegate
				{
					_municipalPipelineNetworkWindow = null;
				};
			}
			_municipalPipelineNetworkWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowMunicipalPipelineNetworkWindow 执行失败", ex);
			throw;
		}
	}

	public void CloseMunicipalPipelineNetworkWindow()
	{
		_municipalPipelineNetworkWindow?.Close();
		_municipalPipelineNetworkWindow = null;
	}

	public void ShowCreateBridgeComponentWindow()
	{
		try
		{
			if (_createBridgeComponentWindow == null)
			{
				CreateBridgeComponentViewModel service = _serviceProvider.GetService<CreateBridgeComponentViewModel>();
				if (service == null)
				{
					Logger.Error("[WindowManager] 无法从服务提供者获取 CreateBridgeComponentViewModel");
					return;
				}
				_createBridgeComponentWindow = new CreateBridgeComponentWindow
				{
					DataContext = service
				};
				_createBridgeComponentWindow.Closed += delegate
				{
					_createBridgeComponentWindow = null;
				};
			}
			_createBridgeComponentWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowCreateBridgeComponentWindow 执行失败", ex);
			throw;
		}
	}

	public void CloseCreateBridgeComponentWindow()
	{
		_createBridgeComponentWindow?.Close();
		_createBridgeComponentWindow = null;
	}

	public void ShowDefinePileWindow()
	{
		try
		{
			if (_definePileWindow == null)
			{
				DefinePileViewModel service = _serviceProvider.GetService<DefinePileViewModel>();
				if (service == null)
				{
					Logger.Error("[WindowManager] 无法从服务提供者获取 DefinePileViewModel");
					return;
				}
				_definePileWindow = new DefinePileWindow
				{
					DataContext = service
				};
				_definePileWindow.Closed += delegate
				{
					_definePileWindow = null;
				};
			}
			_definePileWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowDefinePileWindow 执行失败", ex);
			throw;
		}
	}

	public void CloseDefinePileWindow()
	{
		_definePileWindow?.Close();
		_definePileWindow = null;
	}

	public void ShowDefineFoundationWindow()
	{
		try
		{
			if (_defineFoundationWindow == null)
			{
				DefineFoundationViewModel service = _serviceProvider.GetService<DefineFoundationViewModel>();
				if (service == null)
				{
					Logger.Error("[WindowManager] 无法从服务提供者获取 DefineFoundationViewModel");
					return;
				}
				_defineFoundationWindow = new DefineFoundationWindow
				{
					DataContext = service
				};
				_defineFoundationWindow.Closed += delegate
				{
					_defineFoundationWindow = null;
				};
			}
			_defineFoundationWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowDefineFoundationWindow 执行失败", ex);
			throw;
		}
	}

	public void CloseDefineFoundationWindow()
	{
		_defineFoundationWindow?.Close();
		_defineFoundationWindow = null;
	}

	public void ShowDefinePierWindow()
	{
		if (_definePierWindow == null)
		{
			DefinePierViewModel service = _serviceProvider.GetService<DefinePierViewModel>();
			if (service == null)
			{
				Logger.Error("[WindowManager] 无法从服务提供者获取 DefinePierViewModel");
				return;
			}
			_definePierWindow = new DefinePierWindow
			{
				DataContext = service
			};
			_definePierWindow.Closed += delegate
			{
				_definePierWindow = null;
			};
		}
		_definePierWindow.Show();
	}

	public void CloseDefinePierWindow()
	{
		_definePierWindow?.Close();
		_definePierWindow = null;
	}

	public void ShowDefineBeamWindow()
	{
		if (_defineBeamWindow == null)
		{
			DefineBeamViewModel service = _serviceProvider.GetService<DefineBeamViewModel>();
			if (service == null)
			{
				Logger.Error("[WindowManager] 无法从服务提供者获取 DefineBeamViewModel");
				return;
			}
			_defineBeamWindow = new DefineBeamWindow
			{
				DataContext = service
			};
			_defineBeamWindow.Closed += delegate
			{
				_defineBeamWindow = null;
			};
		}
		_defineBeamWindow.Show();
	}

	public void CloseDefineBeamWindow()
	{
		_defineBeamWindow?.Close();
		_defineBeamWindow = null;
	}

	public void ShowDefineBearingWindow()
	{
		if (_defineBearingWindow == null)
		{
			DefineBearingViewModel service = _serviceProvider.GetService<DefineBearingViewModel>();
			if (service == null)
			{
				Logger.Error("[WindowManager] 无法从服务提供者获取 DefineBearingViewModel");
				return;
			}
			_defineBearingWindow = new DefineBearingWindow
			{
				DataContext = service
			};
			_defineBearingWindow.Closed += delegate
			{
				_defineBearingWindow = null;
			};
		}
		_defineBearingWindow.Show();
	}

	public void CloseDefineBearingWindow()
	{
		_defineBearingWindow?.Close();
		_defineBearingWindow = null;
	}

	public void ShowDefineBridgeTypeWindow()
	{
		if (_defineBridgeTypeWindow == null)
		{
			DefineBridgeTypeViewModel service = _serviceProvider.GetService<DefineBridgeTypeViewModel>();
			if (service == null)
			{
				Logger.Error("[WindowManager] 无法从服务提供者获取 DefineBridgeTypeViewModel");
				return;
			}
			_defineBridgeTypeWindow = new DefineBridgeTypeWindow
			{
				DataContext = service
			};
			_defineBridgeTypeWindow.Closed += delegate
			{
				_defineBridgeTypeWindow = null;
			};
		}
		_defineBridgeTypeWindow.Show();
	}

	public void CloseDefineBridgeTypeWindow()
	{
		_defineBridgeTypeWindow?.Close();
		_defineBridgeTypeWindow = null;
	}

	public void ShowEditBridgeComponentsWindow(RoadProject roadProject)
	{
		if (_editBridgeComponentsWindow == null)
		{
			EditBridgeComponentsViewModel dataContext = new EditBridgeComponentsViewModel(roadProject);
			_editBridgeComponentsWindow = new EditBridgeComponentsWindow
			{
				DataContext = dataContext
			};
			_editBridgeComponentsWindow.Closed += delegate
			{
				_editBridgeComponentsWindow = null;
			};
		}
		_editBridgeComponentsWindow.Show();
	}

	public void ShowFamilyLibraryWindow()
	{
		try
		{
			if (_familyLibraryWindow == null)
			{
				FamilyLibraryViewModel requiredService = _serviceProvider.GetRequiredService<FamilyLibraryViewModel>();
				_familyLibraryWindow = new FamilyLibraryWindow(requiredService);
				_familyLibraryWindow.Closed += delegate
				{
					_familyLibraryWindow = null;
				};
			}
			_familyLibraryWindow.Show();
			_familyLibraryWindow.Activate();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] 显示族库窗口失败", ex);
		}
	}

	public void CloseFamilyLibraryWindow()
	{
		_familyLibraryWindow?.Close();
	}

	public void ShowTopographyFromFloorWindow()
	{
		try
		{
			if (_topographyFromFloorWindow == null)
			{
				TopographyFromFloorViewModel service = _serviceProvider.GetService<TopographyFromFloorViewModel>();
				if (service == null)
				{
					Logger.Error("[WindowManager] 无法从服务提供者获取 TopographyFromFloorViewModel");
					throw new InvalidOperationException("无法获取 TopographyFromFloorViewModel");
				}
				_topographyFromFloorWindow = new TopographyFromFloorWindow
				{
					DataContext = service
				};
				_topographyFromFloorWindow.Closed += delegate
				{
					_topographyFromFloorWindow = null;
				};
			}
			_topographyFromFloorWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowTopographyFromFloorWindow 执行失败", ex);
			throw;
		}
	}

	public void CloseTopographyFromFloorWindow()
	{
		_topographyFromFloorWindow?.Close();
		_topographyFromFloorWindow = null;
	}

	public void ShowExportFamilyMetadataWindow()
	{
		try
		{
			if (_exportFamilyMetadataWindow == null)
			{
				_exportFamilyMetadataWindow = new ExportFamilyMetadataWindow();
				_exportFamilyMetadataWindow.Closed += delegate
				{
					_exportFamilyMetadataWindow = null;
				};
			}
			_exportFamilyMetadataWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowExportFamilyMetadataWindow 执行失败", ex);
			throw;
		}
	}

	public void ShowMapSelectionWindow()
	{
		try
		{
			if (_mapSelectionWindow == null)
			{
				_mapSelectionWindow = new MapSelectionWindow();
				_mapSelectionWindow.Closed += delegate
				{
					_mapSelectionWindow = null;
				};
			}
			_mapSelectionWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowMapSelectionWindow 执行失败", ex);
			throw;
		}
	}

	public void ShowMapSelectionWindowWithCallback(Action<MapSelectionCompletedEventArgs> callback, double? initialLat = null, double? initialLon = null)
	{
		try
		{
			if (_mapSelectionWindow == null)
			{
				_mapSelectionWindow = new MapSelectionWindow();
				_mapSelectionWindow.Closed += delegate
				{
					_mapSelectionWindow = null;
				};
			}
			_mapSelectionWindow.InitialLat = initialLat;
			_mapSelectionWindow.InitialLon = initialLon;
			_mapSelectionWindow.ClearMapSelectionCompletedHandlers();
			_mapSelectionWindow.MapSelectionCompleted += delegate(object? s, MapSelectionCompletedEventArgs e)
			{
				callback?.Invoke(e);
			};
			_mapSelectionWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowMapSelectionWindowWithCallback 执行失败", ex);
			throw;
		}
	}

	public void CloseMapSelectionWindow()
	{
		try
		{
			if (_mapSelectionWindow != null)
			{
				((DispatcherObject)_mapSelectionWindow).Dispatcher.Invoke((Action)delegate
				{
					_mapSelectionWindow.Close();
				});
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[WindowManager] 关闭地图选择窗口失败: " + ex.Message);
		}
	}

	public void ShowWallToRoadWindow()
	{
		try
		{
			if (_wallToRoadWindow == null)
			{
				WallToRoadViewModel service = _serviceProvider.GetService<WallToRoadViewModel>();
				if (service == null)
				{
					Logger.Error("[WindowManager] 无法从服务提供者获取 WallToRoadViewModel");
					return;
				}
				_wallToRoadWindow = new WallToRoadWindow
				{
					DataContext = service
				};
				_wallToRoadWindow.Closed += delegate
				{
					_wallToRoadWindow = null;
				};
			}
			_wallToRoadWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowWallToRoadWindow 执行失败", ex);
			throw;
		}
	}

	public void ShowSubgradeModelWindow()
	{
		try
		{
			if (_createSubgradeModelWindow == null)
			{
				CreateSubgradeModelViewModel service = _serviceProvider.GetService<CreateSubgradeModelViewModel>();
				if (service == null)
				{
					Logger.Error("[WindowManager] 无法从服务提供者获取 CreateSubgradeModelViewModel");
					return;
				}
				_createSubgradeModelWindow = new CreateSubgradeModelWindow
				{
					DataContext = service
				};
				_createSubgradeModelWindow.Closed += delegate
				{
					_createSubgradeModelWindow = null;
				};
			}
			_createSubgradeModelWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowSubgradeModelWindow 执行失败", ex);
			throw;
		}
	}

	public void CloseSubgradeModelWindow()
	{
		_createSubgradeModelWindow?.Close();
		_createSubgradeModelWindow = null;
	}

	public void ShowAncillaryStructureWindow()
	{
		try
		{
			if (_createAncillaryStructureWindow == null)
			{
				CreateAncillaryStructureViewModel service = _serviceProvider.GetService<CreateAncillaryStructureViewModel>();
				if (service == null)
				{
					Logger.Error("[WindowManager] 无法从服务提供者获取 CreateAncillaryStructureViewModel");
					return;
				}
				_createAncillaryStructureWindow = new CreateAncillaryStructureWindow
				{
					DataContext = service
				};
				_createAncillaryStructureWindow.Closed += delegate
				{
					_createAncillaryStructureWindow = null;
				};
			}
			_createAncillaryStructureWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowAncillaryStructureWindow 执行失败", ex);
			throw;
		}
	}

	public void CloseAncillaryStructureWindow()
	{
		_createAncillaryStructureWindow?.Close();
		_createAncillaryStructureWindow = null;
	}

	public void ShowRoadSurfaceRefinementWindow()
	{
		try
		{
			if (_roadSurfaceRefinementWindow == null)
			{
				RoadSurfaceRefinementViewModel service = _serviceProvider.GetService<RoadSurfaceRefinementViewModel>();
				if (service == null)
				{
					Logger.Error("[WindowManager] 无法从服务提供者获取 RoadSurfaceRefinementViewModel");
					return;
				}
				_roadSurfaceRefinementWindow = new RoadSurfaceRefinementWindow
				{
					DataContext = service
				};
				_roadSurfaceRefinementWindow.Closed += delegate
				{
					_roadSurfaceRefinementWindow = null;
				};
			}
			_roadSurfaceRefinementWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] ShowRoadSurfaceRefinementWindow 执行失败", ex);
			throw;
		}
	}

	public void CloseRoadSurfaceRefinementWindow()
	{
		_roadSurfaceRefinementWindow?.Close();
		_roadSurfaceRefinementWindow = null;
	}

	public void ShowInsulationWindow()
	{
		try
		{
			if (_insulationWindow != null)
			{
				_insulationWindow.Activate();
				return;
			}
			IModuleLoader moduleLoader = ServiceProvider.GetModuleLoader();
			if (moduleLoader == null)
			{
				MessageBox.Show("无法获取模块加载器", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			if (moduleLoader.RevitAdapter == null)
			{
				MessageBox.Show("无法获取 Revit 适配器", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			_insulationWindow = new InsulationWindow();
			_insulationWindow.Closed += delegate
			{
				_insulationWindow = null;
			};
			_insulationWindow.Show();
		}
		catch (Exception ex)
		{
			Logger.Error("[WindowManager] 显示保温工具窗口失败: " + ex.Message, ex);
			MessageBox.Show("显示保温工具窗口失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	public void CloseInsulationWindow()
	{
		_insulationWindow?.Close();
		_insulationWindow = null;
	}
}
