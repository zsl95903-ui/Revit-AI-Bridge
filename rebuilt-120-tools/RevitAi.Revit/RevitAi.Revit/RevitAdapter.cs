using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.FamilyLibrary;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Abstractions.Revit;
using RevitAi.Abstractions.Revit.WallToRoad;
using RevitAi.Abstractions.Services;
using RevitAi.Abstractions.UI;
using RevitAi.Abstractions.Units;
using RevitAi.Core.AI;
using RevitAi.Core.Authentication;
using RevitAi.Core.CAD;
using RevitAi.Core.FamilyLibrary;
using RevitAi.Revit.AI;
using RevitAi.Revit.Infrastructure;
using RevitAi.Revit.LinkManagement;
using RevitAi.Revit.MEP;
using RevitAi.Revit.Net8.Adapters;
using RevitAi.Revit.Revit;
using RevitAi.Revit.RoadCenterline;
using RevitAi.Revit.RoadModeling;
using RevitAi.Revit.Services;
using RevitAi.Revit.UI;
using RevitAi.Revit.WallToRoad;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using ns2;
using ns6;

namespace RevitAi.Revit;

public sealed class RevitAdapter : IRevitAdapter
{
	[CompilerGenerated]
	public sealed class _003C_003Ec__DisplayClass166_0
	{
		public TaskCompletionSource<bool> tcs;

		internal void _003CSetProjectUnitAsync_003Eb__0(bool success)
		{
			tcs.TrySetResult(success);
		}
	}

	[CompilerGenerated]
	public sealed class _003CSetProjectUnitAsync_003Ed__166 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

		public object document;

		public UnitType unitType;

		public object displayUnitType;

		public RevitAdapter _003C_003E4__this;

		private _003C_003Ec__DisplayClass166_0 _003C_003E8__1;

		private Document _003Cdoc_003E5__2;

		private SetProjectUnitRequest _003Crequest_003E5__3;

		private object _003CexternalEventObj_003E5__4;

		private ExternalEvent _003CexternalEvent_003E5__5;

		private Task _003CcompletedTask_003E5__6;

		private Task _003C_003Es__7;

		private bool _003C_003Es__8;

		private Exception _003Cex_003E5__9;

		private TaskAwaiter<Task> _003C_003Eu__1;

		private TaskAwaiter<bool> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Expected O, but got Unknown
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			if ((uint)num <= 1u)
			{
			}
			bool result;
			try
			{
				TaskAwaiter<Task> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<Task>);
					num = -1;
					_003C_003E1__state = -1;
					goto IL_01ce;
				}
				TaskAwaiter<bool> awaiter2;
				if (num == 1)
				{
					awaiter2 = _003C_003Eu__2;
					_003C_003Eu__2 = default(TaskAwaiter<bool>);
					num = -1;
					_003C_003E1__state = -1;
					goto IL_026b;
				}
				_003C_003E8__1 = new _003C_003Ec__DisplayClass166_0();
				object obj = document;
				_003Cdoc_003E5__2 = (Document)((obj is Document) ? obj : null);
				if (_003Cdoc_003E5__2 == null)
				{
					_003C_003E4__this.LogError("[RevitAdapter] SetProjectUnitAsync: 无效的文档对象");
					result = false;
				}
				else
				{
					_003C_003E8__1.tcs = new TaskCompletionSource<bool>();
					_003Crequest_003E5__3 = new SetProjectUnitRequest
					{
						Document = _003Cdoc_003E5__2,
						UnitType = unitType,
						DisplayUnitType = displayUnitType,
						OnCompleted = delegate(bool success)
						{
							_003C_003E8__1.tcs.TrySetResult(success);
						}
					};
					SetProjectUnitRequestManager.SetRequest(_003Crequest_003E5__3);
					_003CexternalEventObj_003E5__4 = _003C_003E4__this.GetSetProjectUnitExternalEvent();
					if (_003CexternalEventObj_003E5__4 == null)
					{
						_003C_003E4__this.LogError("[RevitAdapter] SetProjectUnitAsync: 无法获取 ExternalEvent");
						result = false;
					}
					else
					{
						object obj2 = _003CexternalEventObj_003E5__4;
						_003CexternalEvent_003E5__5 = (ExternalEvent)((obj2 is ExternalEvent) ? obj2 : null);
						if (_003CexternalEvent_003E5__5 != null)
						{
							_003CexternalEvent_003E5__5.Raise();
							awaiter = Task.WhenAny(_003C_003E8__1.tcs.Task, Task.Delay(5000)).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = 0;
								_003C_003E1__state = 0;
								_003C_003Eu__1 = awaiter;
								_003CSetProjectUnitAsync_003Ed__166 stateMachine = this;
								_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
								return;
							}
							goto IL_01ce;
						}
						_003C_003E4__this.LogError("[RevitAdapter] SetProjectUnitAsync: ExternalEvent 类型转换失败");
						result = false;
					}
				}
				goto end_IL_000c;
				IL_01ce:
				_003C_003Es__7 = awaiter.GetResult();
				_003CcompletedTask_003E5__6 = _003C_003Es__7;
				_003C_003Es__7 = null;
				if (_003CcompletedTask_003E5__6 == _003C_003E8__1.tcs.Task)
				{
					awaiter2 = _003C_003E8__1.tcs.Task.GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 1;
						_003C_003E1__state = 1;
						_003C_003Eu__2 = awaiter2;
						_003CSetProjectUnitAsync_003Ed__166 stateMachine = this;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
						return;
					}
					goto IL_026b;
				}
				_003C_003E4__this.LogError("[RevitAdapter] SetProjectUnitAsync: 操作超时");
				result = false;
				goto end_IL_000c;
				IL_026b:
				_003C_003Es__8 = awaiter2.GetResult();
				result = _003C_003Es__8;
				end_IL_000c:;
			}
			catch (Exception ex)
			{
				_003Cex_003E5__9 = ex;
				_003C_003E4__this.LogError("[RevitAdapter] SetProjectUnitAsync 失败: " + _003Cex_003E5__9.Message, _003Cex_003E5__9);
				result = false;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	private static UIApplication? _currentUIApplication;

	private UIApplication? _application;

	private UIControlledApplication? _controlledApplication;

	private DocumentService? _documentService;

	private ElementService? _elementService;

	private ParameterService? _parameterService;

	private ISelectionService? _selectionService;

	private InsulationService? _insulationService;

	private IGeometryService? _geometryService;

	private ApplicationService? _applicationService;

	private IViewService? _viewService;

	private IViewExportService? _viewExportService;

	private IDwgExportService? _dwgExportService;

	private ILevelService? _levelService;

	private AnnotationService? _annotationService;

	private IAuthManager? _authManager;

	private ExternalEvent? _aiToolExternalEvent;

	private ExternalEvent? _roadCenterlineExternalEvent;

	private ExternalEvent? _cadElementSelectionExternalEvent;

	private ExternalEvent? _pipeNetworkModelingExternalEvent;

	private ExternalEvent? _familyLoadExternalEvent;

	private ExternalEvent? _insulationExternalEvent;

	private ExternalEvent? _familyCheckExternalEvent;

	private ExternalEvent? _familyPlacementExternalEvent;

	private ExternalEvent? _linkManagementExternalEvent;

	private ExternalEvent? _batchLinkModelsExternalEvent;

	private ExternalEvent? _topographyOperationExternalEvent;

	private ExternalEvent? _exportFamilyMetadataExternalEvent;

	private ExternalEvent? _wallToRoadExternalEvent;

	private ExternalEvent? _roadModelingExternalEvent;

	private ExternalEvent? _setProjectUnitExternalEvent;

	private ExternalEvent? _roadSurfaceSelectionExternalEvent;

	private ExternalEvent? _roadSurfaceVoidGenerationExternalEvent;

	private ExternalEvent? _roadSurfaceCutExternalEvent;

	private ModificationService? _modificationService;

	private MaterialService? _materialService;

	private AnalysisService? _analysisService;

	private LinkService? _linkService;

	private FamilyService? _familyService;

	private IFamilyMetadataService? _familyMetadataService;

	private PhaseService? _phaseService;

	private CreationService? _creationService;

	private InfrastructureService? _infrastructureService;

	private CurveService? _curveService;

	private CADGeometryService? _cadGeometryService;

	private FamilyLoadService? _familyLoadService;

	private RoadModelingService? _roadModelingService;

	private IWallToRoadService? _wallToRoadService;

	private ITopographyService? _topographyService;

	public static UIApplication? CurrentUIApplication
	{
		get
		{
			return _currentUIApplication;
		}
		set
		{
			_currentUIApplication = value;
		}
	}

	public ICADFileService? CADFileService { get; private set; }

	public ITopographyService? TopographyService
	{
		get
		{
			if (_topographyService == null && _application != null)
			{
				try
				{
					_topographyService = (ITopographyService?)(object)new TopographyService(_application, (IParameterService?)(object)_parameterService);
				}
				catch (Exception ex)
				{
					LogError("TopographyService 延迟初始化失败: " + ex.Message);
				}
			}
			return _topographyService;
		}
		private set
		{
			_topographyService = value;
		}
	}

	public RevitVersion Version
	{
		get
		{
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Expected O, but got Unknown
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Expected O, but got Unknown
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Expected O, but got Unknown
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Expected O, but got Unknown
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Expected O, but got Unknown
			try
			{
				if (_application != null)
				{
					Application application = _application.Application;
					string versionNumber = application.VersionNumber;
					int num = ParseVersionYear(application.VersionName);
					return new RevitVersion(num, versionNumber, application.VersionBuild, "Revit " + application.VersionName);
				}
				object obj;
				if (_controlledApplication != null)
				{
					PropertyInfo property = ((object)_controlledApplication).GetType().GetProperty("VersionName");
					if (property != null)
					{
						object? value = property.GetValue(_controlledApplication);
						if (value == null)
						{
							obj = null;
						}
						else
						{
							obj = value.ToString();
							if (obj != null)
							{
								goto IL_00ae;
							}
						}
						obj = "Unknown";
						goto IL_00ae;
					}
					return new RevitVersion(2025, "2025", "Unknown", "Revit 2025");
				}
				return new RevitVersion(0, "Unknown", "Unknown", "Unknown Revit");
				IL_00ae:
				string text = (string)obj;
				int num2 = ParseVersionYear(text);
				return new RevitVersion(num2, text, "Unknown", "Revit " + text);
			}
			catch
			{
				return new RevitVersion(0, "Unknown", "Unknown", "Unknown Revit");
			}
		}
	}

	public IDocumentService? DocumentService => (IDocumentService?)(object)_documentService;

	public IElementService? ElementService => (IElementService?)(object)_elementService;

	public IParameterService? ParameterService => (IParameterService?)(object)_parameterService;

	public ISelectionService? SelectionService => _selectionService;

	public IInsulationService? InsulationService => (IInsulationService?)(object)_insulationService;

	public IGeometryService? GeometryService => _geometryService;

	public IApplicationService? ApplicationService => (IApplicationService?)(object)_applicationService;

	public IViewService? ViewService => _viewService;

	public IViewExportService? ViewExportService => _viewExportService;

	public IDwgExportService? DwgExportService => _dwgExportService;

	public ILevelService? LevelService => _levelService;

	public IAnnotationService? AnnotationService => (IAnnotationService?)(object)_annotationService;

	public IModificationService? ModificationService => (IModificationService?)(object)_modificationService;

	public IMaterialService? MaterialService => (IMaterialService?)(object)_materialService;

	public IAnalysisService? AnalysisService => (IAnalysisService?)(object)_analysisService;

	public ILinkService? LinkService => (ILinkService?)(object)_linkService;

	public IFamilyService? FamilyService => (IFamilyService?)(object)_familyService;

	public IFamilyMetadataService? FamilyMetadataService => _familyMetadataService;

	public IPhaseService? PhaseService => (IPhaseService?)(object)_phaseService;

	public ICreationService? CreationService => (ICreationService?)(object)_creationService;

	public IInfrastructureService? InfrastructureService => (IInfrastructureService?)(object)_infrastructureService;

	public CurveService? CurveService => _curveService;

	public IRoadModelingService? RoadModelingService => ServiceProvider.GetService<IRoadModelingService>();

	public ICADGeometryService? CADGeometryService => (ICADGeometryService?)(object)_cadGeometryService;

	public IAuthManager? AuthManager
	{
		get
		{
			return _authManager;
		}
		set
		{
			_authManager = value;
		}
	}

	public static event EventHandler? DocumentChanged;

	public ExternalEvent? GetAIToolExternalEvent()
	{
		if (_aiToolExternalEvent != null)
		{
			return _aiToolExternalEvent;
		}
		if (_application == null)
		{
			return null;
		}
		try
		{
			_aiToolExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)new AIToolExternalEventHandler());
			return _aiToolExternalEvent;
		}
		catch (Exception ex)
		{
			LogError("创建 AI 工具 ExternalEvent 失败: " + ex.Message);
			return null;
		}
	}

	public ExternalEvent? GetRoadCenterlineExternalEvent()
	{
		if (_roadCenterlineExternalEvent != null)
		{
			return _roadCenterlineExternalEvent;
		}
		try
		{
			_roadCenterlineExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)new RoadCenterlineExternalEventHandler());
			return _roadCenterlineExternalEvent;
		}
		catch (Exception ex)
		{
			LogError("创建道路中心线 ExternalEvent 失败: " + ex.Message);
			return null;
		}
	}

	public ExternalEvent? GetCADElementSelectionExternalEvent()
	{
		if (_cadElementSelectionExternalEvent != null)
		{
			return _cadElementSelectionExternalEvent;
		}
		try
		{
			_cadElementSelectionExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)new CADElementSelectionExternalEventHandler());
			return _cadElementSelectionExternalEvent;
		}
		catch (Exception ex)
		{
			LogError("创建 CAD 图元选择 ExternalEvent 失败: " + ex.Message);
			return null;
		}
	}

	public object? GetPipeNetworkModelingExternalEvent()
	{
		if (_pipeNetworkModelingExternalEvent != null)
		{
			return _pipeNetworkModelingExternalEvent;
		}
		try
		{
			PipeNetworkModelingExternalEventHandler pipeNetworkModelingExternalEventHandler = new PipeNetworkModelingExternalEventHandler();
			_pipeNetworkModelingExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)pipeNetworkModelingExternalEventHandler);
			return _pipeNetworkModelingExternalEvent;
		}
		catch (Exception ex)
		{
			LogError("创建管网建模 ExternalEvent 失败: " + ex.Message + "\n" + ex.StackTrace);
			return null;
		}
	}

	public object? GetFamilyLoadExternalEvent()
	{
		return _familyLoadExternalEvent;
	}

	public object? GetFamilyCheckExternalEvent()
	{
		return _familyCheckExternalEvent;
	}

	public object? GetFamilyPlacementExternalEvent()
	{
		return _familyPlacementExternalEvent;
	}

	public object? GetRoadModelingExternalEvent()
	{
		return _roadModelingExternalEvent;
	}

	public object? GetSetProjectUnitExternalEvent()
	{
		if (_setProjectUnitExternalEvent != null)
		{
			return _setProjectUnitExternalEvent;
		}
		try
		{
			_setProjectUnitExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)new SetProjectUnitExternalEventHandler());
			return _setProjectUnitExternalEvent;
		}
		catch (Exception ex)
		{
			LogError("创建 SetProjectUnit ExternalEvent 失败: " + ex.Message);
			return null;
		}
	}

	public ExternalEvent? GetInsulationExternalEvent()
	{
		if (_insulationExternalEvent != null)
		{
			return _insulationExternalEvent;
		}
		try
		{
			_insulationExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)new InsulationExternalEventHandler());
			LogInfo("✅ 保温工具 ExternalEvent 已创建");
			return _insulationExternalEvent;
		}
		catch (Exception ex)
		{
			LogError("创建保温工具 ExternalEvent 失败: " + ex.Message);
			return null;
		}
	}

	public bool TriggerInsulation(InsulationRequest request)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		ExternalEvent insulationExternalEvent = GetInsulationExternalEvent();
		if (insulationExternalEvent == null)
		{
			LogError("无法获取保温工具 ExternalEvent");
			return false;
		}
		try
		{
			InsulationRequestManager.SetRequest(request);
			insulationExternalEvent.Raise();
			return true;
		}
		catch (Exception ex)
		{
			LogError("触发保温工具操作失败: " + ex.Message);
			return false;
		}
	}

	public IWallToRoadService? GetWallToRoadService()
	{
		if (_wallToRoadService != null)
		{
			return _wallToRoadService;
		}
		try
		{
			object? activeDocument = GetActiveDocument();
			Document val = (Document)((activeDocument is Document) ? activeDocument : null);
			if (val == null || _application == null)
			{
				LogError("无法获取活动文档或应用程序");
				return null;
			}
			WallToRoadService wallToRoadService = new WallToRoadService(_application, val);
			if (wallToRoadService != null)
			{
				_wallToRoadService = (IWallToRoadService?)(object)new WallToRoadServiceAdapter(wallToRoadService);
				return _wallToRoadService;
			}
		}
		catch (Exception ex)
		{
			LogError("[RevitAdapter] 获取 WallToRoadService 失败: " + ex.Message, ex);
		}
		return null;
	}

	public ITopographyService? GetTopographyService()
	{
		return TopographyService;
	}

	public object? GetStairCreationService()
	{
		object activeDocument = GetActiveDocument();
		if (activeDocument == null)
		{
			return null;
		}
		try
		{
			Document val = (Document)((activeDocument is Document) ? activeDocument : null);
			if (val == null)
			{
				return null;
			}
			return new StairCreationService(val);
		}
		catch (Exception ex)
		{
			LogError("获取楼梯创建服务失败: " + ex.Message, ex);
			return null;
		}
	}

	public object? GetStairTypeManagerService()
	{
		object activeDocument = GetActiveDocument();
		if (activeDocument == null)
		{
			return null;
		}
		try
		{
			Document val = (Document)((activeDocument is Document) ? activeDocument : null);
			if (val == null)
			{
				return null;
			}
			return new StairTypeManagerService(val);
		}
		catch (Exception ex)
		{
			LogError("获取楼梯类型管理服务失败: " + ex.Message, ex);
			return null;
		}
	}

	public object? GetLinkManagementExternalEvent()
	{
		if (_linkManagementExternalEvent != null)
		{
			return _linkManagementExternalEvent;
		}
		try
		{
			LinkManagementExternalEventHandler linkManagementExternalEventHandler = new LinkManagementExternalEventHandler();
			_linkManagementExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)linkManagementExternalEventHandler);
			return _linkManagementExternalEvent;
		}
		catch (Exception ex)
		{
			LogError("创建链接管理 ExternalEvent 失败: " + ex.Message + "\n" + ex.StackTrace);
			return null;
		}
	}

	public object? GetBatchLinkModelsExternalEvent()
	{
		if (_batchLinkModelsExternalEvent != null)
		{
			return _batchLinkModelsExternalEvent;
		}
		try
		{
			BatchLinkModelsExternalEventHandler batchLinkModelsExternalEventHandler = new BatchLinkModelsExternalEventHandler();
			_batchLinkModelsExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)batchLinkModelsExternalEventHandler);
			return _batchLinkModelsExternalEvent;
		}
		catch (Exception ex)
		{
			LogError("创建批量链接模型 ExternalEvent 失败: " + ex.Message + "\n" + ex.StackTrace);
			return null;
		}
	}

	public object? GetTopographyOperationExternalEvent()
	{
		if (_topographyOperationExternalEvent != null)
		{
			return _topographyOperationExternalEvent;
		}
		try
		{
			TopographyOperationExternalEventHandler topographyOperationExternalEventHandler = new TopographyOperationExternalEventHandler();
			_topographyOperationExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)topographyOperationExternalEventHandler);
			return _topographyOperationExternalEvent;
		}
		catch (Exception ex)
		{
			LogError("创建地形操作 ExternalEvent 失败: " + ex.Message + "\n" + ex.StackTrace);
			return null;
		}
	}

	public object? GetRoadSurfaceSelectionExternalEvent()
	{
		if (_roadSurfaceSelectionExternalEvent != null)
		{
			return _roadSurfaceSelectionExternalEvent;
		}
		try
		{
			RoadSurfaceSelectionExternalEventHandler roadSurfaceSelectionExternalEventHandler = new RoadSurfaceSelectionExternalEventHandler();
			_roadSurfaceSelectionExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)roadSurfaceSelectionExternalEventHandler);
			return _roadSurfaceSelectionExternalEvent;
		}
		catch (Exception ex)
		{
			LogError("创建路面精修选择 ExternalEvent 失败: " + ex.Message + "\n" + ex.StackTrace);
			return null;
		}
	}

	public object? GetRoadSurfaceVoidGenerationExternalEvent()
	{
		if (_roadSurfaceVoidGenerationExternalEvent != null)
		{
			return _roadSurfaceVoidGenerationExternalEvent;
		}
		try
		{
			RoadSurfaceVoidGenerationExternalEventHandler roadSurfaceVoidGenerationExternalEventHandler = new RoadSurfaceVoidGenerationExternalEventHandler();
			_roadSurfaceVoidGenerationExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)roadSurfaceVoidGenerationExternalEventHandler);
			return _roadSurfaceVoidGenerationExternalEvent;
		}
		catch (Exception ex)
		{
			LogError("创建路面精修空心生成 ExternalEvent 失败: " + ex.Message + "\n" + ex.StackTrace);
			return null;
		}
	}

	public object? GetRoadSurfaceCutExternalEvent()
	{
		if (_roadSurfaceCutExternalEvent != null)
		{
			return _roadSurfaceCutExternalEvent;
		}
		try
		{
			RoadSurfaceCutExternalEventHandler roadSurfaceCutExternalEventHandler = new RoadSurfaceCutExternalEventHandler();
			_roadSurfaceCutExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)roadSurfaceCutExternalEventHandler);
			return _roadSurfaceCutExternalEvent;
		}
		catch (Exception ex)
		{
			LogError("创建路面精修剪切 ExternalEvent 失败: " + ex.Message + "\n" + ex.StackTrace);
			return null;
		}
	}

	public bool TriggerTopographyOperation(TopographyOperationRequest request)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		object? topographyOperationExternalEvent = GetTopographyOperationExternalEvent();
		ExternalEvent val = (ExternalEvent)((topographyOperationExternalEvent is ExternalEvent) ? topographyOperationExternalEvent : null);
		if (val == null)
		{
			LogError("无法获取地形操作 ExternalEvent");
			return false;
		}
		try
		{
			TopographyOperationRequestManager.SetRequest(request);
			val.Raise();
			return true;
		}
		catch (Exception ex)
		{
			LogError("触发地形操作失败: " + ex.Message);
			return false;
		}
	}

	public bool TriggerCreateTopography(CreateTopographyRequest request)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		object? topographyOperationExternalEvent = GetTopographyOperationExternalEvent();
		ExternalEvent val = (ExternalEvent)((topographyOperationExternalEvent is ExternalEvent) ? topographyOperationExternalEvent : null);
		if (val == null)
		{
			LogError("无法获取地形操作 ExternalEvent");
			return false;
		}
		try
		{
			TopographyOperationRequestManager.SetCreateTopographyRequest(request);
			val.Raise();
			return true;
		}
		catch (Exception ex)
		{
			LogError("触发创建地形失败: " + ex.Message);
			return false;
		}
	}

	public bool TriggerRoadSurfaceSelection()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		object? roadSurfaceSelectionExternalEvent = GetRoadSurfaceSelectionExternalEvent();
		ExternalEvent val = (ExternalEvent)((roadSurfaceSelectionExternalEvent is ExternalEvent) ? roadSurfaceSelectionExternalEvent : null);
		if (val == null)
		{
			LogError("无法获取路面精修选择 ExternalEvent");
			return false;
		}
		try
		{
			val.Raise();
			return true;
		}
		catch (Exception ex)
		{
			LogError("触发路面精修选择失败: " + ex.Message);
			return false;
		}
	}

	public bool TriggerRoadSurfaceVoidGeneration(RoadSurfaceVoidGenerationRequest request)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		object? roadSurfaceVoidGenerationExternalEvent = GetRoadSurfaceVoidGenerationExternalEvent();
		ExternalEvent val = (ExternalEvent)((roadSurfaceVoidGenerationExternalEvent is ExternalEvent) ? roadSurfaceVoidGenerationExternalEvent : null);
		if (val == null)
		{
			LogError("无法获取路面精修空心生成 ExternalEvent");
			return false;
		}
		try
		{
			RoadSurfaceRefinementRequestManager.SetVoidGenerationRequest(request);
			val.Raise();
			return true;
		}
		catch (Exception ex)
		{
			LogError("触发路面精修空心生成失败: " + ex.Message);
			return false;
		}
	}

	public bool TriggerRoadSurfaceCut()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		object? roadSurfaceCutExternalEvent = GetRoadSurfaceCutExternalEvent();
		ExternalEvent val = (ExternalEvent)((roadSurfaceCutExternalEvent is ExternalEvent) ? roadSurfaceCutExternalEvent : null);
		if (val == null)
		{
			LogError("无法获取路面精修剪切 ExternalEvent");
			return false;
		}
		try
		{
			val.Raise();
			return true;
		}
		catch (Exception ex)
		{
			LogError("触发路面精修剪切失败: " + ex.Message);
			return false;
		}
	}

	public object? GetExportFamilyMetadataExternalEvent()
	{
		if (_exportFamilyMetadataExternalEvent != null)
		{
			return _exportFamilyMetadataExternalEvent;
		}
		try
		{
			ExportFamilyMetadataExternalEventHandler exportFamilyMetadataExternalEventHandler = new ExportFamilyMetadataExternalEventHandler();
			_exportFamilyMetadataExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)exportFamilyMetadataExternalEventHandler);
			return _exportFamilyMetadataExternalEvent;
		}
		catch (Exception ex)
		{
			LogError("创建族元数据导出 ExternalEvent 失败: " + ex.Message);
			return null;
		}
	}

	public bool TriggerExportFamilyMetadata(ExportFamilyMetadataRequest request)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		object? exportFamilyMetadataExternalEvent = GetExportFamilyMetadataExternalEvent();
		ExternalEvent val = (ExternalEvent)((exportFamilyMetadataExternalEvent is ExternalEvent) ? exportFamilyMetadataExternalEvent : null);
		if (val == null)
		{
			LogError("无法获取族元数据导出 ExternalEvent");
			return false;
		}
		try
		{
			ExportFamilyMetadataRequestManager.SetRequest(request);
			val.Raise();
			return true;
		}
		catch (Exception ex)
		{
			LogError("触发族元数据导出失败: " + ex.Message);
			return false;
		}
	}

	public ExternalEvent? GetWallToRoadExternalEvent()
	{
		if (_wallToRoadExternalEvent != null)
		{
			return _wallToRoadExternalEvent;
		}
		try
		{
			_wallToRoadExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)new WallToRoadExternalEventHandler());
			return _wallToRoadExternalEvent;
		}
		catch (Exception ex)
		{
			LogError("创建从墙生成道路 ExternalEvent 失败: " + ex.Message);
			return null;
		}
	}

	public bool TriggerWallToRoadOperation(WallToRoadRequest request)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		ExternalEvent wallToRoadExternalEvent = GetWallToRoadExternalEvent();
		if (wallToRoadExternalEvent == null)
		{
			LogError("无法获取从墙生成道路 ExternalEvent");
			return false;
		}
		try
		{
			WallToRoadRequestManager.SetRequest(request);
			wallToRoadExternalEvent.Raise();
			return true;
		}
		catch (Exception ex)
		{
			LogError("触发从墙生成道路失败: " + ex.Message);
			return false;
		}
	}

	public bool TriggerCADElementSelection(CADElementSelectionRequest request)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		ExternalEvent cADElementSelectionExternalEvent = GetCADElementSelectionExternalEvent();
		if (cADElementSelectionExternalEvent == null)
		{
			LogError("无法获取 CAD 图元选择 ExternalEvent");
			return false;
		}
		try
		{
			CADRequestManager.SetRequest(request);
			cADElementSelectionExternalEvent.Raise();
			return true;
		}
		catch (Exception ex)
		{
			LogError("触发 CAD 图元选择失败: " + ex.Message);
			return false;
		}
	}

	public bool InitializeForUI(object application)
	{
		try
		{
			UIControlledApplication val = (UIControlledApplication)((application is UIControlledApplication) ? application : null);
			if (val != null)
			{
				_controlledApplication = val;
				_application = TryExtractUIApplication(val);
				if (_application != null)
				{
					_currentUIApplication = _application;
					InitializeServices();
				}
				return true;
			}
			LogError("InitializeForUI: application 不是 UIControlledApplication 类型");
			return false;
		}
		catch (Exception ex)
		{
			LogError("InitializeForUI 失败: " + ex.Message);
			return false;
		}
	}

	private void InitializeServices()
	{
		if (_application == null)
		{
			LogWarning("InitializeServices: _application 为 null，无法初始化服务");
			return;
		}
		try
		{
			_documentService = new DocumentService(_application);
			_elementService = new ElementService(_application);
			_parameterService = new ParameterService(_application);
			_selectionService = (ISelectionService?)(object)new SelectionService(_application);
			_insulationService = new InsulationService(_application);
			_geometryService = (IGeometryService?)(object)new GeometryService(_application);
			_viewService = (IViewService?)(object)new ViewService(_application);
			_viewExportService = (IViewExportService?)(object)new ViewExportService(_application);
			_dwgExportService = (IDwgExportService?)(object)new DwgExportService(_application);
			_levelService = (ILevelService?)(object)new LevelService(_application);
			_applicationService = new ApplicationService(_application);
			_infrastructureService = new InfrastructureService();
			ServiceProvider.RegisterService(typeof(IInfrastructureService), (object)_infrastructureService);
			ServiceProvider.RegisterService(typeof(ISatelliteMapImportService), (object)new SatelliteMapImportService());
			try
			{
				IFamilyLibraryService service = ServiceProvider.GetService<IFamilyLibraryService>();
				if (service == null)
				{
					IAuthManager service2 = ServiceProvider.GetService<IAuthManager>();
					ISupabaseClient service3 = ServiceProvider.GetService<ISupabaseClient>();
					if (service2 != null && service3 != null)
					{
						service = (IFamilyLibraryService)(object)new FamilyLibraryService(service3, service2);
						ServiceProvider.RegisterService(typeof(IFamilyLibraryService), (object)service);
					}
					else
					{
						LogWarning("族库服务未初始化：缺少 AuthManager 或 SupabaseClient");
					}
				}
				IFamilyLoadService service4 = ServiceProvider.GetService<IFamilyLoadService>();
				if (service4 == null)
				{
					service4 = (IFamilyLoadService)(object)new FamilyLoadService((IRevitAdapter)(object)this);
					ServiceProvider.RegisterService(typeof(IFamilyLoadService), (object)service4);
				}
			}
			catch (Exception ex)
			{
				LogError("初始化族库服务失败: " + ex.Message, ex);
			}
			_annotationService = new AnnotationService(_application, (IInfrastructureService)(object)_infrastructureService);
			_modificationService = new ModificationService(_application);
			_materialService = new MaterialService(_application);
			_analysisService = new AnalysisService(_application);
			_linkService = new LinkService(_application);
			_familyService = new FamilyService(_application, (IParameterService)(object)_parameterService);
			_familyMetadataService = (IFamilyMetadataService?)(object)new FamilyMetadataExportService(_application);
			_phaseService = new PhaseService(_application);
			_creationService = new CreationService(_application);
			_curveService = new CurveService(_application, (IInfrastructureService)(object)_infrastructureService);
			_cadGeometryService = new CADGeometryService(_application);
			CADFileService = (ICADFileService?)(object)new RevitCADFileService();
			try
			{
				IFamilyLibraryService service5 = ServiceProvider.GetService<IFamilyLibraryService>();
				IFamilyLoadService service6 = ServiceProvider.GetService<IFamilyLoadService>();
				if (service5 != null && service6 != null)
				{
					_roadModelingService = new RoadModelingService();
				}
			}
			catch (Exception ex2)
			{
				LogError("初始化道路建模服务失败: " + ex2.Message, ex2);
			}
			_topographyService = (ITopographyService?)(object)new TopographyService(_application, (IParameterService?)(object)_parameterService);
			try
			{
				_exportFamilyMetadataExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)new ExportFamilyMetadataExternalEventHandler());
			}
			catch (Exception ex3)
			{
				LogError("预先创建族元数据导出 ExternalEvent 失败: " + ex3.Message, ex3);
			}
			try
			{
				_familyLoadExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)new FamilyLoadExternalEventHandler());
			}
			catch (Exception ex4)
			{
				LogError("预先创建族加载 ExternalEvent 失败: " + ex4.Message, ex4);
			}
			try
			{
				_familyCheckExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)new FamilyCheckExternalEventHandler());
			}
			catch (Exception ex5)
			{
				LogError("预先创建族检查 ExternalEvent 失败: " + ex5.Message, ex5);
			}
			try
			{
				_familyPlacementExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)new FamilyPlacementExternalEventHandler());
			}
			catch (Exception ex6)
			{
				LogError("预先创建族放置 ExternalEvent 失败: " + ex6.Message, ex6);
			}
			try
			{
				_roadModelingExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)new RoadModelingExternalEventHandler());
			}
			catch (Exception ex7)
			{
				LogError("预先创建道路建模 ExternalEvent 失败: " + ex7.Message, ex7);
			}
			try
			{
				_setProjectUnitExternalEvent = ExternalEvent.Create((IExternalEventHandler)(object)new SetProjectUnitExternalEventHandler());
			}
			catch (Exception ex8)
			{
				LogError("预先创建设置项目单位 ExternalEvent 失败: " + ex8.Message, ex8);
			}
		}
		catch (Exception ex9)
		{
			LogError("InitializeServices 失败: " + ex9.Message);
		}
	}

	public bool InitializeForCommand(object commandData)
	{
		try
		{
			ExternalCommandData val = (ExternalCommandData)((commandData is ExternalCommandData) ? commandData : null);
			if (val != null)
			{
				_application = val.Application;
				_currentUIApplication = _application;
				if (_annotationService == null)
				{
					InitializeServices();
				}
				else
				{
					LogInfo("✅ 服务已在 InitializeForUI 中初始化，跳过重复初始化");
				}
				StartDocumentChangeMonitoring();
				GetRoadSurfaceSelectionExternalEvent();
				GetRoadSurfaceVoidGenerationExternalEvent();
				GetRoadSurfaceCutExternalEvent();
				return true;
			}
			LogError("InitializeForCommand: commandData 不是 ExternalCommandData 类型");
			return false;
		}
		catch (Exception ex)
		{
			LogError("InitializeForCommand 失败: " + ex.Message);
			return false;
		}
	}

	public IUIApplication CreateUIApplication(object application)
	{
		UIControlledApplication val = (UIControlledApplication)((application is UIControlledApplication) ? application : null);
		if (val == null)
		{
			throw new ArgumentException("application 必须是 UIControlledApplication 类型", "application");
		}
		return (IUIApplication)(object)new RevitUIApplicationAdapter(val);
	}

	[Obsolete("请使用 InitializeForUI 或 InitializeForCommand")]
	public bool Initialize(object application)
	{
		if (application is UIControlledApplication)
		{
			return InitializeForUI(application);
		}
		if (application is ExternalCommandData)
		{
			return InitializeForCommand(application);
		}
		LogError("Initialize: application 类型不支持");
		return false;
	}

	public bool Shutdown()
	{
		try
		{
			StopDocumentChangeMonitoring();
			_aiToolExternalEvent = null;
			_roadCenterlineExternalEvent = null;
			_cadElementSelectionExternalEvent = null;
			_pipeNetworkModelingExternalEvent = null;
			_familyLoadExternalEvent = null;
			_familyCheckExternalEvent = null;
			_familyPlacementExternalEvent = null;
			_linkManagementExternalEvent = null;
			_batchLinkModelsExternalEvent = null;
			_topographyOperationExternalEvent = null;
			_exportFamilyMetadataExternalEvent = null;
			_documentService = null;
			_elementService = null;
			_parameterService = null;
			_selectionService = null;
			_insulationService = null;
			_geometryService = null;
			_viewService = null;
			_levelService = null;
			_annotationService = null;
			_modificationService = null;
			_materialService = null;
			_analysisService = null;
			_linkService = null;
			_familyService = null;
			_phaseService = null;
			_creationService = null;
			_application = null;
			_controlledApplication = null;
			_currentUIApplication = null;
			return true;
		}
		catch (Exception ex)
		{
			LogError("Shutdown 失败: " + ex.Message);
			return false;
		}
	}

	private void StartDocumentChangeMonitoring()
	{
		try
		{
			if (_application != null)
			{
				StopDocumentChangeMonitoring();
				_application.Application.DocumentChanged += OnDocumentChanged;
				_application.ViewActivated += OnViewActivated;
			}
		}
		catch (Exception ex)
		{
			LogError("启动文档变更监听失败: " + ex.Message);
		}
	}

	private void StopDocumentChangeMonitoring()
	{
		try
		{
			if (_application != null)
			{
				_application.Application.DocumentChanged -= OnDocumentChanged;
				_application.ViewActivated -= OnViewActivated;
			}
		}
		catch (Exception ex)
		{
			LogError("停止文档变更监听失败: " + ex.Message);
		}
	}

	private void OnDocumentChanged(object sender, object e)
	{
		try
		{
			DocumentChanged?.Invoke(this, EventArgs.Empty);
		}
		catch (Exception ex)
		{
			LogError("文档变更事件处理失败: " + ex.Message);
		}
	}

	private void OnViewActivated(object sender, ViewActivatedEventArgs e)
	{
		try
		{
			Document document = ((RevitAPIPostDocEventArgs)e).Document;
			if (document == null)
			{
				return;
			}
			if (_documentService == null)
			{
				LogWarning("[RevitAdapter] ⚠️ 文档切换时检测到服务未初始化，尝试重新初始化");
				if (_application != null)
				{
					try
					{
						_documentService = new DocumentService(_application);
						_elementService = new ElementService(_application);
						_parameterService = new ParameterService(_application);
						_selectionService = (ISelectionService?)(object)new SelectionService(_application);
						_insulationService = new InsulationService(_application);
						_geometryService = (IGeometryService?)(object)new GeometryService(_application);
						_viewService = (IViewService?)(object)new ViewService(_application);
						_viewExportService = (IViewExportService?)(object)new ViewExportService(_application);
						_dwgExportService = (IDwgExportService?)(object)new DwgExportService(_application);
						_levelService = (ILevelService?)(object)new LevelService(_application);
						_applicationService = new ApplicationService(_application);
						_infrastructureService = new InfrastructureService();
						ServiceProvider.RegisterService(typeof(IInfrastructureService), (object)_infrastructureService);
						_annotationService = new AnnotationService(_application, (IInfrastructureService)(object)_infrastructureService);
						_modificationService = new ModificationService(_application);
						_materialService = new MaterialService(_application);
						_analysisService = new AnalysisService(_application);
						_linkService = new LinkService(_application);
						_familyService = new FamilyService(_application, (IParameterService)(object)_parameterService);
						_familyMetadataService = (IFamilyMetadataService?)(object)new FamilyMetadataExportService(_application);
						_phaseService = new PhaseService(_application);
						_creationService = new CreationService(_application);
						_curveService = new CurveService(_application, (IInfrastructureService)(object)_infrastructureService);
						_cadGeometryService = new CADGeometryService(_application);
						_topographyService = (ITopographyService?)(object)new TopographyService(_application, (IParameterService?)(object)_parameterService);
						LogInfo("[RevitAdapter] ✅ 文档切换后服务重新初始化成功");
					}
					catch (Exception ex)
					{
						LogError("[RevitAdapter] ❌ 文档切换后服务重新初始化失败: " + ex.Message);
					}
				}
			}
			else
			{
				LogInfo("[RevitAdapter] ✅ 文档切换完成，服务已就绪");
			}
			try
			{
				Type type = typeof(RevitAdapter).Assembly.GetType("RevitAi.Revit.UI.AIDockablePaneHelper");
				if (!(type != null))
				{
					return;
				}
				PropertyInfo property = type.GetProperty("WasPaneEverOpened", BindingFlags.Static | BindingFlags.Public);
				if (property != null && (bool)(property.GetValue(null) ?? ((object)false)))
				{
					LogInfo("[RevitAdapter] 🔔 检测到 AI DockablePane 曾被打开，尝试恢复显示");
					MethodInfo method = type.GetMethod("RestorePaneIfNeeded", BindingFlags.Static | BindingFlags.Public);
					if (method != null && _application != null)
					{
						method.Invoke(null, new object[1] { _application });
					}
				}
			}
			catch (Exception ex2)
			{
				LogWarning("[RevitAdapter] 恢复 AI DockablePane 失败: " + ex2.Message);
			}
		}
		catch (Exception ex3)
		{
			LogError("视图激活事件处理失败: " + ex3.Message);
		}
	}

	public IFamilyLoadService? GetFamilyLoadService()
	{
		if (_familyLoadService != null)
		{
			return (IFamilyLoadService?)(object)_familyLoadService;
		}
		if (_application == null)
		{
			return null;
		}
		try
		{
			_familyLoadService = new FamilyLoadService((IRevitAdapter)(object)this);
			return (IFamilyLoadService?)(object)_familyLoadService;
		}
		catch (Exception ex)
		{
			LogError("创建 FamilyLoadService 失败: " + ex.Message);
			return null;
		}
	}

	public object? GetActiveDocument()
	{
		try
		{
			if (_application != null)
			{
				UIDocument activeUIDocument = _application.ActiveUIDocument;
				if (activeUIDocument != null)
				{
					return activeUIDocument.Document;
				}
			}
			if (_controlledApplication != null)
			{
				string[] array = new string[5]
				{
					"Application",
					"m_application",
					"InternalApplication",
					"RevitApplication",
					"UIApplication"
				};
				string[] array2 = array;
				foreach (string text in array2)
				{
					try
					{
						PropertyInfo property = ((object)_controlledApplication).GetType().GetProperty(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						if (!(property != null))
						{
							continue;
						}
						object? value = property.GetValue(_controlledApplication);
						UIApplication val = (UIApplication)((value is UIApplication) ? value : null);
						if (val != null)
						{
							UIDocument activeUIDocument2 = val.ActiveUIDocument;
							if (activeUIDocument2 != null)
							{
								LogInfo("✅ 通过反射获取到活动文档（属性名: " + text + "）");
								return activeUIDocument2.Document;
							}
						}
					}
					catch
					{
					}
				}
				if (_currentUIApplication != null)
				{
					UIDocument activeUIDocument3 = _currentUIApplication.ActiveUIDocument;
					if (activeUIDocument3 != null)
					{
						return activeUIDocument3.Document;
					}
				}
				LogWarning("⚠️ 无法获取活动文档：反射失败或未打开文档");
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("获取活动文档失败: " + ex.Message);
			return null;
		}
	}

	public List<CADInstanceInfo> GetCADImportInstances()
	{
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Expected O, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Expected O, but got Unknown
		try
		{
			object? activeDocument = GetActiveDocument();
			Document val = (Document)((activeDocument is Document) ? activeDocument : null);
			if (val == null)
			{
				LogWarning("无法获取文档，返回空的 CAD 导入实例列表");
				return new List<CADInstanceInfo>();
			}
			List<CADInstanceInfo> list = new List<CADInstanceInfo>();
			FilteredElementCollector val2 = new FilteredElementCollector(val);
			List<ImportInstance> list2 = ((IEnumerable)val2.OfClass(typeof(ImportInstance))).Cast<ImportInstance>().ToList();
			ICADFileService cADFileService = CADFileService;
			if (cADFileService == null)
			{
				LogWarning("CADFileService 为 null，无法获取 CAD 文件路径");
				foreach (ImportInstance item in list2)
				{
					list.Add(new CADInstanceInfo
					{
						ElementId = ((int)((Element)item).Id.Value).ToString(),
						ImportInstance = item
					});
				}
				return list;
			}
			foreach (ImportInstance item2 in list2)
			{
				try
				{
					CADFileInfo cADFilePath = cADFileService.GetCADFilePath((object)item2, (object)val);
					CADInstanceInfo val3 = new CADInstanceInfo
					{
						ElementId = ((int)((Element)item2).Id.Value).ToString()
					};
					object obj;
					if (cADFilePath == null)
					{
						obj = null;
					}
					else
					{
						obj = cADFilePath.FilePath;
						if (obj != null)
						{
							goto IL_0138;
						}
					}
					obj = string.Empty;
					goto IL_0138;
					IL_0138:
					val3.FilePath = (string)obj;
					string fileName;
					if (cADFilePath != null && !string.IsNullOrEmpty(cADFilePath.FilePath))
					{
						fileName = Path.GetFileName(cADFilePath.FilePath);
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 1);
						defaultInterpolatedStringHandler.AppendLiteral("导入实例 ");
						defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)item2).Id);
						fileName = defaultInterpolatedStringHandler.ToStringAndClear();
					}
					val3.FileName = fileName;
					val3.ImportInstance = item2;
					list.Add(val3);
				}
				catch (Exception ex)
				{
					LogError("处理导入实例失败: " + ex.Message);
					list.Add(new CADInstanceInfo
					{
						ElementId = ((int)((Element)item2).Id.Value).ToString(),
						ImportInstance = item2
					});
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(16, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("成功获取 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个 CAD 导入实例");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return list;
		}
		catch (Exception ex2)
		{
			LogError("获取 CAD 导入实例失败: " + ex2.Message);
			return new List<CADInstanceInfo>();
		}
	}

	public IExcelDataService? GetExcelDataService()
	{
		try
		{
			return (IExcelDataService?)(object)new ExcelDataService();
		}
		catch (Exception ex)
		{
			LogError("创建 ExcelDataService 失败: " + ex.Message);
			return null;
		}
	}

	public IFileAttachmentService? GetFileAttachmentService()
	{
		try
		{
			return (IFileAttachmentService?)(object)new FileAttachmentService();
		}
		catch (Exception ex)
		{
			LogError("创建 FileAttachmentService 失败: " + ex.Message);
			return null;
		}
	}

	public IPipeNetworkModelingService? GetPipeNetworkModelingService()
	{
		try
		{
			IExcelDataService excelDataService = GetExcelDataService();
			if (excelDataService == null)
			{
				LogError("❌ 无法创建管网建模服务：Excel 数据服务不可用");
				return null;
			}
			if (FamilyService == null || ParameterService == null)
			{
				LogError("❌ 无法创建管网建模服务：FamilyService 或 ParameterService 不可用");
				return null;
			}
			IPipeCreationService pipeCreationService = GetPipeCreationService();
			PipeNetworkModelingService coreService = new PipeNetworkModelingService(excelDataService);
			return (IPipeNetworkModelingService?)(object)new PipeNetworkModelingServiceAdapter(coreService, FamilyService, ParameterService, pipeCreationService);
		}
		catch (Exception ex)
		{
			LogError("创建 PipeNetworkModelingService 失败: " + ex.Message);
			return null;
		}
	}

	public IPipeCreationService? GetPipeCreationService()
	{
		try
		{
			return (IPipeCreationService?)(object)new PipeCreationService();
		}
		catch (Exception ex)
		{
			LogError("创建 PipeCreationService 失败: " + ex.Message);
			return null;
		}
	}

	private UIApplication? TryExtractUIApplication(UIControlledApplication controlledApp)
	{
		if (controlledApp == null)
		{
			return null;
		}
		try
		{
			string[] array = new string[6]
			{
				"m_uiapplication",
				"m_application",
				"UIApplication",
				"_application",
				"m_uiApplication",
				"InternalApplication"
			};
			string[] array2 = array;
			foreach (string name in array2)
			{
				try
				{
					FieldInfo field = ((object)controlledApp).GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (field != null)
					{
						object value = field.GetValue(controlledApp);
						UIApplication val = (UIApplication)((value is UIApplication) ? value : null);
						if (val != null)
						{
							return val;
						}
					}
					PropertyInfo property = ((object)controlledApp).GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (property != null)
					{
						object value2 = property.GetValue(controlledApp);
						UIApplication val2 = (UIApplication)((value2 is UIApplication) ? value2 : null);
						if (val2 != null)
						{
							return val2;
						}
					}
				}
				catch
				{
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("反射获取 UIApplication 失败: " + ex.Message);
			return null;
		}
	}

	private static int ParseVersionYear(string versionName)
	{
		string[] array = versionName.Split(' ');
		string[] array2 = array;
		int num = 0;
		int result;
		while (true)
		{
			if (num < array2.Length)
			{
				string s = array2[num];
				if (int.TryParse(s, out result) && result >= 2010 && result <= 2100)
				{
					break;
				}
				num++;
				continue;
			}
			return 0;
		}
		return result;
	}

	public Dictionary<UnitType, ProjectUnitInfo> GetProjectUnits(object document)
	{
		return Class336.smethod_0(document);
	}

	[AsyncStateMachine(typeof(_003CSetProjectUnitAsync_003Ed__166))]
	[DebuggerStepThrough]
	public Task<bool> SetProjectUnitAsync(object document, UnitType unitType, object displayUnitType)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		_003CSetProjectUnitAsync_003Ed__166 stateMachine = new _003CSetProjectUnitAsync_003Ed__166();
		stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<bool>.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine.document = document;
		stateMachine.unitType = unitType;
		stateMachine.displayUnitType = displayUnitType;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
		return stateMachine._003C_003Et__builder.Task;
	}

	public void LogDebug(string message)
	{
		try
		{
			Logger.Debug(message);
		}
		catch
		{
		}
	}

	public void LogInfo(string message)
	{
		try
		{
			Logger.Info(message);
		}
		catch
		{
		}
	}

	public void LogWarning(string message)
	{
		try
		{
			Logger.Warning(message);
		}
		catch
		{
		}
	}

	public void LogError(string message, Exception? ex = null)
	{
		try
		{
			if (ex != null)
			{
				Logger.Error(message, ex);
			}
			else
			{
				Logger.Error(message);
			}
		}
		catch
		{
		}
	}
}
