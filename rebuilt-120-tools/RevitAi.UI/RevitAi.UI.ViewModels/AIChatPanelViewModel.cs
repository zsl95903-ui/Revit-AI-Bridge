using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.AI.Models;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.FamilyLibrary;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Network;
using RevitAi.Abstractions.Services;
using RevitAi.Core;
using RevitAi.Core.AI;
using RevitAi.Core.AI.Models;
using RevitAi.Core.AI.Tools;
using RevitAi.Core.Authentication;
using RevitAi.Core.Authentication.Models;
using RevitAi.Core.Feedback;
using RevitAi.Core.Security;
using RevitAi.UI.Models;
using RevitAi.UI.Services;
using RevitAi.UI.Views;
using RevitAi.UI.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using ServiceProvider = RevitAi.Abstractions.Loader.ServiceProvider;

namespace RevitAi.UI.ViewModels;

public class AIChatPanelViewModel : ObservableObject
{
	private readonly IWindowManager _windowManager;

	private readonly IDialogService _dialogService;

	private readonly IAIService? _aiService;

	private readonly IAIServiceEx? _aiServiceEx;

	private readonly AIToolInvocationHandler? _toolInvocationHandler;

	private IRevitAdapter? _revitAdapter;

	private object? _externalEvent;

	private readonly AIChatSessionManager? _sessionManager;

	private AIChatSession? _currentSession;

	private readonly IAIToolDataCache? _dataCache;

	private readonly TokenUsageService? _tokenUsageService;

	private SessionTokenAccumulator? _sessionAccumulator;

	private DateTime _requestStartTime;

	[ObservableProperty]
	private string _userInput = string.Empty;

	[ObservableProperty]
	private bool _isProcessing;

	[ObservableProperty]
	private bool _isTopMost;

	[ObservableProperty]
	private WindowDisplayMode _displayMode;

	[ObservableProperty]
	private ObservableCollection<AIConfig> _availableAIConfigs = new ObservableCollection<AIConfig>();

	[ObservableProperty]
	private AIConfig? _selectedAIConfig;

	[ObservableProperty]
	private Visibility _attachFileButtonVisible = Visibility.Collapsed;

	[ObservableProperty]
	private bool _isLoadingAIConfigs;

	[ObservableProperty]
	private bool _useStreamingResponse = true;

	[ObservableProperty]
	private bool _enableToolCalling = true;

	[ObservableProperty]
	private CodeSnippetLibraryViewModel? _codeSnippetLibraryViewModel;

	[ObservableProperty]
	private CodeMarketViewModel? _codeMarketViewModel;

	private CancellationTokenSource? _cts;

	private int _currentTaskId;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? cancelGenerationCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? attachFileCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<string>? removeAttachmentCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? sendMessageCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? clearChatCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? newSessionCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? manageSessionsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closePanelCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? exportChatCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? toggleTopMostCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? switchToDockableCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? switchToWindowCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? reloadAIConfigsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? manageAIProvidersCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? saveSessionCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? restoreSessionCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<string>? attachFileByPathCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<string?>? saveCodeSnippetCommand;

	public ObservableCollection<FileAttachment> Attachments { get; } = new ObservableCollection<FileAttachment>();

	public ObservableCollection<ChatMessage> Messages { get; } = new ObservableCollection<ChatMessage>();

	public string StatusColor
	{
		get
		{
			if (!IsProcessing)
			{
				return "#FF4CAF50";
			}
			return "#FFFF9800";
		}
	}

	public string StatusText
	{
		get
		{
			if (!IsProcessing)
			{
				return "就绪";
			}
			return "处理中...";
		}
	}

	public bool CanSendMessage
	{
		get
		{
			if (!IsProcessing)
			{
				return !string.IsNullOrWhiteSpace(UserInput);
			}
			return false;
		}
	}

	public ICommand? PrimaryButtonCommand
	{
		get
		{
			if (!IsProcessing)
			{
				return SendMessageCommand;
			}
			return CancelGenerationCommand;
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string UserInput
	{
		get
		{
			return _userInput;
		}
		[MemberNotNull("_userInput")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_userInput, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.UserInput);
				_userInput = value;
				OnUserInputChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.UserInput);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsProcessing
	{
		get
		{
			return _isProcessing;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isProcessing, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsProcessing);
				_isProcessing = value;
				OnIsProcessingChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsProcessing);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsTopMost
	{
		get
		{
			return _isTopMost;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isTopMost, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsTopMost);
				_isTopMost = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsTopMost);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public WindowDisplayMode DisplayMode
	{
		get
		{
			return _displayMode;
		}
		set
		{
			if (!EqualityComparer<WindowDisplayMode>.Default.Equals(_displayMode, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.DisplayMode);
				_displayMode = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.DisplayMode);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<AIConfig> AvailableAIConfigs
	{
		get
		{
			return _availableAIConfigs;
		}
		[MemberNotNull("_availableAIConfigs")]
		set
		{
			if (!EqualityComparer<ObservableCollection<AIConfig>>.Default.Equals(_availableAIConfigs, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AvailableAIConfigs);
				_availableAIConfigs = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AvailableAIConfigs);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public AIConfig? SelectedAIConfig
	{
		get
		{
			return _selectedAIConfig;
		}
		set
		{
			if (!EqualityComparer<AIConfig>.Default.Equals(_selectedAIConfig, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedAIConfig);
				_selectedAIConfig = value;
				OnSelectedAIConfigChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedAIConfig);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public Visibility AttachFileButtonVisible
	{
		get
		{
			return _attachFileButtonVisible;
		}
		set
		{
			if (!EqualityComparer<Visibility>.Default.Equals(_attachFileButtonVisible, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AttachFileButtonVisible);
				_attachFileButtonVisible = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AttachFileButtonVisible);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsLoadingAIConfigs
	{
		get
		{
			return _isLoadingAIConfigs;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isLoadingAIConfigs, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsLoadingAIConfigs);
				_isLoadingAIConfigs = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsLoadingAIConfigs);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool UseStreamingResponse
	{
		get
		{
			return _useStreamingResponse;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_useStreamingResponse, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.UseStreamingResponse);
				_useStreamingResponse = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.UseStreamingResponse);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool EnableToolCalling
	{
		get
		{
			return _enableToolCalling;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_enableToolCalling, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.EnableToolCalling);
				_enableToolCalling = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.EnableToolCalling);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public CodeSnippetLibraryViewModel? CodeSnippetLibraryViewModel
	{
		get
		{
			return _codeSnippetLibraryViewModel;
		}
		set
		{
			if (!EqualityComparer<RevitAi.UI.ViewModels.CodeSnippetLibraryViewModel>.Default.Equals(_codeSnippetLibraryViewModel, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CodeSnippetLibraryViewModel);
				_codeSnippetLibraryViewModel = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CodeSnippetLibraryViewModel);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public CodeMarketViewModel? CodeMarketViewModel
	{
		get
		{
			return _codeMarketViewModel;
		}
		set
		{
			if (!EqualityComparer<RevitAi.UI.ViewModels.CodeMarketViewModel>.Default.Equals(_codeMarketViewModel, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CodeMarketViewModel);
				_codeMarketViewModel = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CodeMarketViewModel);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CancelGenerationCommand => cancelGenerationCommand ?? (cancelGenerationCommand = new RelayCommand(CancelGeneration));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand AttachFileCommand => attachFileCommand ?? (attachFileCommand = new AsyncRelayCommand(AttachFileAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<string> RemoveAttachmentCommand => removeAttachmentCommand ?? (removeAttachmentCommand = new AsyncRelayCommand<string>(RemoveAttachmentAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand SendMessageCommand => sendMessageCommand ?? (sendMessageCommand = new AsyncRelayCommand(SendMessageAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand ClearChatCommand => clearChatCommand ?? (clearChatCommand = new AsyncRelayCommand(ClearChatAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand NewSessionCommand => newSessionCommand ?? (newSessionCommand = new AsyncRelayCommand(NewSessionAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ManageSessionsCommand => manageSessionsCommand ?? (manageSessionsCommand = new RelayCommand(ManageSessions));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ClosePanelCommand => closePanelCommand ?? (closePanelCommand = new RelayCommand(ClosePanel));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ExportChatCommand => exportChatCommand ?? (exportChatCommand = new RelayCommand(ExportChat));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ToggleTopMostCommand => toggleTopMostCommand ?? (toggleTopMostCommand = new RelayCommand(ToggleTopMost));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SwitchToDockableCommand => switchToDockableCommand ?? (switchToDockableCommand = new RelayCommand(SwitchToDockable));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SwitchToWindowCommand => switchToWindowCommand ?? (switchToWindowCommand = new RelayCommand(SwitchToWindow));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand ReloadAIConfigsCommand => reloadAIConfigsCommand ?? (reloadAIConfigsCommand = new AsyncRelayCommand(ReloadAIConfigsAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ManageAIProvidersCommand => manageAIProvidersCommand ?? (manageAIProvidersCommand = new RelayCommand(ManageAIProviders));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand SaveSessionCommand => saveSessionCommand ?? (saveSessionCommand = new AsyncRelayCommand(SaveSession));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand RestoreSessionCommand => restoreSessionCommand ?? (restoreSessionCommand = new AsyncRelayCommand(RestoreSession));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<string> AttachFileByPathCommand => attachFileByPathCommand ?? (attachFileByPathCommand = new AsyncRelayCommand<string>(AttachFileByPathAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<string?> SaveCodeSnippetCommand => saveCodeSnippetCommand ?? (saveCodeSnippetCommand = new AsyncRelayCommand<string>(SaveCodeSnippetAsync));

	public void SetRevitAdapter(IRevitAdapter revitAdapter, object? externalEvent = null)
	{
		_revitAdapter = revitAdapter;
		_externalEvent = externalEvent;
	}

	private void EnsureRevitServicesInitialized()
	{
		try
		{
			if (_revitAdapter == null)
			{
				try
				{
					Assembly assembly = Assembly.Load("RevitAi.Main");
					if (assembly != null)
					{
						Type type = assembly.GetType("RevitAi.MainApplication");
						if (type != null)
						{
							PropertyInfo property = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
							if (property != null)
							{
								object value = property.GetValue(null);
								if (value != null)
								{
									FieldInfo field = type.GetField("_adapter", BindingFlags.Instance | BindingFlags.NonPublic);
									if (field != null)
									{
										object value2 = field.GetValue(value);
										if (value2 != null)
										{
											_revitAdapter = value2 as IRevitAdapter;
											_ = _revitAdapter;
										}
									}
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("[AIChatPanel] ⚠\ufe0f 尝试获取 RevitAdapter 失败: " + ex.Message);
				}
				if (_revitAdapter == null)
				{
					Logger.Warning("[AIChatPanel] ⚠\ufe0f RevitAdapter 未设置，无法初始化服务");
					return;
				}
			}
			if (_revitAdapter.DocumentService != null && _revitAdapter.ElementService != null)
			{
				return;
			}
			Type type2 = _revitAdapter.GetType();
			PropertyInfo property2 = type2.GetProperty("CurrentUIApplication", BindingFlags.Static | BindingFlags.Public);
			if (property2 == null)
			{
				Logger.Warning("[AIChatPanel] ⚠\ufe0f 未找到 CurrentUIApplication 属性");
				return;
			}
			object value3 = property2.GetValue(null);
			if (value3 == null)
			{
				Logger.Warning("[AIChatPanel] ⚠\ufe0f CurrentUIApplication 为 null");
			}
			else
			{
				if (_revitAdapter.DocumentService != null)
				{
					return;
				}
				Assembly assembly2 = type2.Assembly;
				(string, string)[] array = new(string, string)[15]
				{
					("RevitAi.Revit.Services.DocumentService", "_documentService"),
					("RevitAi.Revit.Services.ElementService", "_elementService"),
					("RevitAi.Revit.Services.ParameterService", "_parameterService"),
					("RevitAi.Revit.Services.SelectionService", "_selectionService"),
					("RevitAi.Revit.Services.GeometryService", "_geometryService"),
					("RevitAi.Revit.Services.ViewService", "_viewService"),
					("RevitAi.Revit.Services.LevelService", "_levelService"),
					("RevitAi.Revit.Services.ModificationService", "_modificationService"),
					("RevitAi.Revit.Services.CreationService", "_creationService"),
					("RevitAi.Revit.Services.AnnotationService", "_annotationService"),
					("RevitAi.Revit.Services.LinkService", "_linkService"),
					("RevitAi.Revit.Services.MaterialService", "_materialService"),
					("RevitAi.Revit.Services.AnalysisService", "_analysisService"),
					("RevitAi.Revit.Services.FamilyService", "_familyService"),
					("RevitAi.Revit.Services.PhaseService", "_phaseService")
				};
				for (int i = 0; i < array.Length; i++)
				{
					var (text, name) = array[i];
					try
					{
						Type type3 = assembly2.GetType(text);
						if (!(type3 != null))
						{
							continue;
						}
						ConstructorInfo constructor = type3.GetConstructor(new Type[1] { value3.GetType() });
						if (constructor != null)
						{
							object obj = constructor.Invoke(new object[1] { value3 });
							if (obj != null)
							{
								type2.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(_revitAdapter, obj);
								Logger.Info("[AIChatPanel] ✅ " + text + " 已初始化");
							}
						}
					}
					catch (Exception ex2)
					{
						Logger.Warning("[AIChatPanel] ⚠\ufe0f 初始化 " + text + " 失败: " + ex2.Message);
					}
				}
			}
		}
		catch (Exception ex3)
		{
			Logger.Error("[AIChatPanel] ❌ 确保 Revit 服务初始化失败: " + ex3.Message, ex3);
		}
	}

	private void UpdateAttachFileButtonVisibility()
	{
		bool flag = SelectedAIConfig?.SupportsVision ?? false;
		AttachFileButtonVisible = ((!flag) ? Visibility.Collapsed : Visibility.Visible);
		Logger.Info($"[AIChatPanel] 附件按钮可见性更新: {AttachFileButtonVisible} (模型: {SelectedAIConfig?.ModelName}, IsUserConfig: {SelectedAIConfig?.IsUserConfig}, SupportsVision: {SelectedAIConfig?.SupportsVision})");
	}

	public AIChatPanelViewModel(IWindowManager windowManager, IDialogService dialogService, IAIService? aiService = null)
	{
		_windowManager = windowManager;
		_dialogService = dialogService;
		_aiService = aiService ?? TryCreateAIService();
		_aiServiceEx = _aiService as IAIServiceEx;
		try
		{
			_tokenUsageService = TryCreateTokenUsageService();
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] ❌ TokenUsageService 初始化异常: " + ex.Message, ex);
		}
		try
		{
			FileStorageService storage = new FileStorageService(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "AIChat"));
			_sessionManager = new AIChatSessionManager(storage);
			_dataCache = UIBootstrapper.TryGetService<IAIToolDataCache>();
			_currentSession = CreateNewSession();
		}
		catch (Exception ex2)
		{
			Logger.Error("[AIChatPanel] ❌ 初始化会话管理器失败: " + ex2.Message, ex2);
		}
		try
		{
			AIToolRegistry instance = AIToolRegistry.Instance;
			_toolInvocationHandler = new AIToolInvocationHandler(instance, _dataCache);
		}
		catch (Exception ex3)
		{
			Logger.Error("[AIChatPanel] ❌ 初始化工具调用处理器失败: " + ex3.Message, ex3);
		}
		Messages.Add(new ChatMessage
		{
			Content = "您好！我是 AS.AI 助手，有什么可以帮助您的吗？",
			IsUser = false,
			Timestamp = DateTime.Now
		});
		Task.Run(() => LoadAIConfigsAsync());
		Task.Run(() => RegisterFamilyLibraryLoadToolAsync());
		Task.Run(() => RegisterFeedbackAIToolsAsync());
		Task.Run(() => RegisterWebSearchAIToolsAsync());
		Task.Run(() => RegisterMemoryAIToolAsync());
		try
		{
			CodeSnippetLibraryViewModel = new CodeSnippetLibraryViewModel();
			CodeSnippetLibraryViewModel.ExecuteSnippetRequested += OnExecuteSnippetRequested;
		}
		catch (Exception ex4)
		{
			Logger.Error("[AIChatPanel] ❌ 初始化代码片段库失败: " + ex4.Message, ex4);
		}
		try
		{
			CodeMarketViewModel = new CodeMarketViewModel(UIBootstrapper.TryGetService<ICodeMarketService>(), UIBootstrapper.TryGetService<IAuthManager>(), UIBootstrapper.TryGetService<IDialogService>(), UIBootstrapper.TryGetService<IWindowManager>());
			if (CodeMarketViewModel != null)
			{
				CodeMarketViewModel.SnippetDownloaded += async delegate
				{
					await RefreshCodeSnippetLibraryAsync();
				};
			}
			LoadCodeMarketAsync();
		}
		catch (Exception ex5)
		{
			Logger.Error("[AIChatPanel] ❌ 初始化代码市场失败: " + ex5.Message, ex5);
		}
	}

	private async Task LoadCodeMarketAsync()
	{
		_ = 1;
		try
		{
			await Task.Delay(100).ConfigureAwait(continueOnCapturedContext: false);
			if (CodeMarketViewModel != null)
			{
				await CodeMarketViewModel.LoadSnippetsAsync();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] 加载代码市场失败: " + ex.Message, ex);
		}
	}

	private async Task RefreshCodeSnippetLibraryAsync()
	{
		try
		{
			if (CodeSnippetLibraryViewModel != null)
			{
				await CodeSnippetLibraryViewModel.RefreshAsync();
				Logger.Info("[AIChatPanel] 代码片段库已刷新（下载后）");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] 刷新代码片段库失败: " + ex.Message, ex);
		}
	}

	private async void OnExecuteSnippetRequested(object? sender, ExecuteSnippetEventArgs e)
	{
		if (_toolInvocationHandler == null || _revitAdapter == null || _externalEvent == null)
		{
			Logger.Warning("[AIChatPanel] 无法执行代码片段：Revit 适配器或工具调用处理器未初始化");
			return;
		}
		try
		{
			EnsureRevitServicesInitialized();
			AIToolRegistry.Instance.EnsureToolsDiscovered();
			object document = _revitAdapter?.GetActiveDocument();
			RevitAi.Core.AI.ToolCallInfo toolCall = new RevitAi.Core.AI.ToolCallInfo
			{
				ToolName = "execute_code",
				Parameters = new Dictionary<string, object> { { "code", e.Code } }
			};
			AIToolContext context = new AIToolContext
			{
				Document = document,
				RevitAdapter = _revitAdapter,
				ExternalEvent = _externalEvent,
				Parameters = new Dictionary<string, object> { { "code", e.Code } }
			};
			AIToolResult aIToolResult = await _toolInvocationHandler.HandleToolCallAsync(toolCall, context);
			if (aIToolResult.Success)
			{
				Logger.Info("[CodeSnippetLibrary] 代码片段执行成功");
			}
			else
			{
				Logger.Error("[CodeSnippetLibrary] 代码片段执行失败: " + aIToolResult.Error);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] 执行代码片段时发生异常: " + ex.Message, ex);
		}
	}

	private static IAIService? TryCreateAIService()
	{
		try
		{
			ISupabaseClient supabaseClient = UIBootstrapper.TryGetService<ISupabaseClient>();
			if (supabaseClient == null)
			{
				Logger.Warning("[AIChatPanel] ⚠\ufe0f 无法获取 ISupabaseClient，AI 服务创建失败");
				return null;
			}
			INativeCryptoService nativeCryptoService = UIBootstrapper.TryGetService<INativeCryptoService>();
			if (nativeCryptoService == null)
			{
				Logger.Warning("[AIChatPanel] ⚠\ufe0f 无法获取 INativeCryptoService，AI 服务创建失败");
				return null;
			}
			return CoreServicesFactory.CreateAIService(CoreServicesFactory.CreateApiKeyManager(nativeCryptoService, supabaseClient), supabaseClient);
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] ❌ 创建 AI 服务异常: " + ex.Message, ex);
			return null;
		}
	}

	private static TokenUsageService? TryCreateTokenUsageService()
	{
		try
		{
			TokenUsageService tokenUsageService = UIBootstrapper.TryGetService<TokenUsageService>();
			if (tokenUsageService != null)
			{
				return tokenUsageService;
			}
			Logger.Warning("[AIChatPanel] ⚠\ufe0f DI 容器中没有 TokenUsageService，尝试手动创建");
			ISupabaseClient supabaseClient = UIBootstrapper.TryGetService<ISupabaseClient>();
			if (supabaseClient == null)
			{
				Logger.Warning("[AIChatPanel] ⚠\ufe0f 无法获取 ISupabaseClient，TokenUsageService 创建失败");
				return null;
			}
			IAuthManager authManager = UIBootstrapper.TryGetService<IAuthManager>();
			if (authManager == null)
			{
				Logger.Warning("[AIChatPanel] ⚠\ufe0f 无法获取 IAuthManager，TokenUsageService 创建失败");
				return null;
			}
			Assembly assembly = Assembly.Load("RevitAi.Core");
			if (assembly == null)
			{
				Logger.Error("[AIChatPanel] ❌ 无法加载 RevitAi.Core 程序集");
				return null;
			}
			Type type = assembly.GetType("RevitAi.Core.AI.TokenUsageService");
			if (type == null)
			{
				Logger.Error("[AIChatPanel] ❌ 无法找到 TokenUsageService 类型");
				return null;
			}
			if (!(Activator.CreateInstance(type, supabaseClient, authManager) is TokenUsageService result))
			{
				Logger.Error("[AIChatPanel] ❌ TokenUsageService 创建失败");
				return null;
			}
			Logger.Info("[AIChatPanel] ✅ 手动创建 TokenUsageService 成功");
			return result;
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] ❌ 创建 TokenUsageService 异常: " + ex.Message, ex);
			return null;
		}
	}

	private async Task LoadAIConfigsAsync()
	{
		try
		{
			IsLoadingAIConfigs = true;
			List<AIConfig> allConfigs = new List<AIConfig>();
			try
			{
				foreach (AIProviderConfig item2 in new LocalAIConfigService().LoadConfigs())
				{
					AIConfig item = new AIConfig
					{
						Provider = item2.Provider,
						ModelName = item2.ModelName,
						DisplayName = item2.DisplayName,
						ProviderEndpoint = item2.ProviderEndpoint,
						MaxTokens = item2.MaxTokens,
						Temperature = item2.Temperature,
						Priority = item2.Priority,
						IsDefault = item2.IsDefault,
						IsUserConfig = true,
						CachedApiKey = item2.EncryptedApiKey,
						SupportsVision = item2.SupportsVision,
						SupportsStreaming = true
					};
					allConfigs.Add(item);
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[AIChatPanel] ⚠\ufe0f 加载本地用户配置失败: " + ex.Message);
			}
			try
			{
				ISupabaseClient supabaseClient = UIBootstrapper.TryGetService<ISupabaseClient>();
				if (supabaseClient == null)
				{
					Logger.Warning("[AIChatPanel] ⚠\ufe0f 无法从 UIBootstrapper 获取 ISupabaseClient");
					Logger.Warning("[AIChatPanel] 提示：请确认 MainApplication 已正确传递 SupabaseClient 到 UIBootstrapper");
				}
				else
				{
					Result<List<AIConfig>> result = await supabaseClient.GetAllActiveAIConfigsAsync();
					if (result.IsSuccess && result.Value != null)
					{
						List<AIConfig> value = result.Value;
						foreach (AIConfig config in value)
						{
							config.IsUserConfig = false;
							if (await VerifyApiKeyForConfig(supabaseClient, config))
							{
								allConfigs.Add(config);
							}
							else
							{
								Logger.Warning("[AIChatPanel] ⚠\ufe0f 系统配置 " + config.DisplayName + " 验证失败（未找到 API 密钥）");
							}
						}
					}
					else
					{
						Logger.Error("[AIChatPanel] 获取系统 AI 配置失败: " + result.Error);
					}
				}
			}
			catch (Exception ex2)
			{
				Logger.Warning("[AIChatPanel] ⚠\ufe0f 加载系统配置失败: " + ex2.Message);
			}
			List<AIConfig> sortedConfigs = (from c in allConfigs
				orderby c.Priority, (!c.IsUserConfig) ? 1 : 0
				select c).ToList();
			await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
			{
				AvailableAIConfigs.Clear();
				foreach (AIConfig item3 in sortedConfigs)
				{
					AvailableAIConfigs.Add(item3);
				}
				AIConfig aIConfig = null;
				try
				{
					LocalAIConfigService localAIConfigService = new LocalAIConfigService();
					string defaultModelId = localAIConfigService.GetDefaultModelId();
					if (!string.IsNullOrEmpty(defaultModelId))
					{
						aIConfig = sortedConfigs.FirstOrDefault((AIConfig c) => c.ModelName == defaultModelId);
					}
				}
				catch (Exception ex4)
				{
					Logger.Warning("[AIChatPanel] 获取管理界面默认模型失败: " + ex4.Message);
				}
				if (aIConfig == null)
				{
					aIConfig = sortedConfigs.FirstOrDefault((AIConfig c) => c.IsDefault && c.IsUserConfig);
				}
				if (aIConfig == null)
				{
					aIConfig = sortedConfigs.FirstOrDefault((AIConfig c) => c.IsDefault && !c.IsUserConfig);
				}
				if (aIConfig == null && sortedConfigs.Count > 0)
				{
					aIConfig = sortedConfigs[0];
				}
				if (aIConfig != null)
				{
					SelectedAIConfig = aIConfig;
				}
			});
		}
		catch (Exception ex3)
		{
			Logger.Error("[AIChatPanel] 加载 AI 配置异常: " + ex3.Message, ex3);
		}
		finally
		{
			IsLoadingAIConfigs = false;
		}
	}

	private async Task<bool> VerifyApiKeyForConfig(ISupabaseClient supabaseClient, AIConfig config)
	{
		try
		{
			Result<EncryptedApiKeyRecord> result = await supabaseClient.GetEncryptedApiKeyByNotesAsync(config.Provider);
			if (result.IsSuccess && result.Value != null)
			{
				try
				{
					ApiKeyManager apiKeyManager = UIBootstrapper.TryGetService<ApiKeyManager>();
					if (apiKeyManager == null)
					{
						Logger.Warning("[AIChatPanel]   ⚠\ufe0f 无法获取 ApiKeyManager 服务");
						return true;
					}
					string masterKey = MasterKeyProvider.GetMasterKey();
					if (string.IsNullOrEmpty(masterKey))
					{
						Logger.Warning("[AIChatPanel]   ⚠\ufe0f 获取主密钥失败");
						return true;
					}
					string text = apiKeyManager.DecryptApiKey(result.Value.EncryptedKey, masterKey);
					if (!string.IsNullOrEmpty(text))
					{
						config.CachedApiKey = text;
						return true;
					}
					Logger.Warning("[AIChatPanel]   ⚠\ufe0f 解密失败，返回空字符串");
				}
				catch (Exception ex)
				{
					Logger.Warning("[AIChatPanel]   ⚠\ufe0f 解密 API Key 失败: " + ex.Message);
				}
				return true;
			}
			Logger.Warning($"[AIChatPanel]   ✗ 未找到配置 {config.DisplayName} 的 API Key（Provider: {config.Provider}）");
			return false;
		}
		catch (Exception ex2)
		{
			Logger.Error("[AIChatPanel] 验证配置 " + config.DisplayName + " 时发生异常: " + ex2.Message, ex2);
			return false;
		}
	}

	private void EnsureTypingMessageAtEnd()
	{
		if (Messages.Count != 0)
		{
			ChatMessage chatMessage = Messages.FirstOrDefault((ChatMessage m) => m.IsTyping);
			int index = Messages.Count - 1;
			if (chatMessage != null && Messages[index] != chatMessage)
			{
				Messages.Remove(chatMessage);
				Messages.Add(chatMessage);
			}
		}
	}

	[RelayCommand]
	private void CancelGeneration()
	{
		try
		{
			if (_cts != null && !_cts.IsCancellationRequested)
			{
				Logger.Info("[AIChatPanel] \ud83d\uded1 用户请求取消 AI 生成");
				_cts.Cancel();
				IsProcessing = false;
				Messages.Add(new ChatMessage
				{
					Content = "⏸\ufe0f 正在取消生成...",
					IsUser = false,
					Timestamp = DateTime.Now
				});
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] 取消生成时发生异常: " + ex.Message, ex);
			IsProcessing = false;
		}
	}

	private bool CanCancelGeneration()
	{
		if (IsProcessing && _cts != null)
		{
			return !_cts.IsCancellationRequested;
		}
		return false;
	}

	[RelayCommand]
	private async Task AttachFileAsync()
	{
		_ = 1;
		try
		{
			AIConfig? selectedAIConfig = SelectedAIConfig;
			if (selectedAIConfig == null || !selectedAIConfig.SupportsVision)
			{
				Logger.Warning("[AIChatPanel] 当前模型 " + SelectedAIConfig?.ModelName + " 不支持视觉能力");
				await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
				{
					Messages.Add(new ChatMessage
					{
						Content = "⚠\ufe0f 当前模型 " + SelectedAIConfig?.ModelName + " 不支持视觉能力。\n\n请切换到支持图片的模型（如 DeepSeek-Vision、GPT-4V 等）后再添加附件。",
						IsUser = false,
						Timestamp = DateTime.Now
					});
				});
				return;
			}
			if (_revitAdapter == null)
			{
				Logger.Warning("[AIChatPanel] RevitAdapter 未设置，无法添加附件");
				return;
			}
			IFileAttachmentService fileAttachmentService = _revitAdapter.GetFileAttachmentService();
			if (fileAttachmentService == null)
			{
				Logger.Warning("[AIChatPanel] 无法获取文件附件服务");
				return;
			}
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "选择要上传的文件（可多选）",
				Filter = "支持的文件|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.webp;*.pdf;*.txt;*.md|所有文件|*.*",
				Multiselect = true
			};
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}
			string[] fileNames = openFileDialog.FileNames;
			int successCount = 0;
			int failCount = 0;
			string[] array = fileNames;
			foreach (string fileName in array)
			{
				try
				{
					string fileName2 = Path.GetFileName(fileName);
					string extension = Path.GetExtension(fileName);
					FileInfo fileInfo = new FileInfo(fileName);
					long maxFileSize = fileAttachmentService.GetMaxFileSize(extension, SelectedAIConfig?.Provider);
					if (fileInfo.Length > maxFileSize)
					{
						Logger.Warning($"[AIChatPanel] 文件过大: {fileName2} ({fileInfo.Length} 字节)，最大支持: {maxFileSize} 字节");
						failCount++;
						continue;
					}
					if (!fileAttachmentService.IsSupportedFileType(extension, SelectedAIConfig?.Provider))
					{
						Logger.Warning("[AIChatPanel] 不支持的文件类型: " + extension + " - " + fileName2);
						failCount++;
						continue;
					}
					AIProviderFileCapability capability = PredefinedFileCapabilities.GetCapability(SelectedAIConfig?.Provider ?? "deepseek");
					if (Attachments.Count >= capability.MaxFileCount)
					{
						Logger.Warning($"[AIChatPanel] 已达到最大文件数量限制: {capability.MaxFileCount}");
						break;
					}
					FileAttachment fileAttachment = await fileAttachmentService.SaveFileAsync(fileName, fileName2);
					Attachments.Add(fileAttachment);
					successCount++;
					Logger.Info($"[AIChatPanel] 成功添加附件: {fileAttachment.FileName} ({fileAttachment.FileSizeDisplay})");
				}
				catch (Exception ex)
				{
					Logger.Error("[AIChatPanel] 添加文件 " + fileName + " 失败: " + ex.Message);
					failCount++;
				}
			}
			Logger.Info($"[AIChatPanel] 文件上传完成: 成功 {successCount} 个，失败 {failCount} 个");
		}
		catch (Exception ex2)
		{
			Logger.Error("[AIChatPanel] 添加附件时发生异常: " + ex2.Message, ex2);
		}
	}

	[RelayCommand]
	private async Task RemoveAttachmentAsync(string attachmentId)
	{
		try
		{
			FileAttachment attachment = Attachments.FirstOrDefault((FileAttachment a) => a.AttachmentId == attachmentId);
			if (attachment == null)
			{
				return;
			}
			Attachments.Remove(attachment);
			if (_revitAdapter != null)
			{
				IFileAttachmentService fileAttachmentService = _revitAdapter.GetFileAttachmentService();
				if (fileAttachmentService != null)
				{
					await fileAttachmentService.DeleteAttachmentAsync(attachmentId);
				}
			}
			Logger.Info("[AIChatPanel] 移除附件: " + attachment.FileName);
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] 移除附件时发生异常: " + ex.Message, ex);
		}
	}

	private (bool IsAuthorized, string Message) CheckAuthorization()
	{
		try
		{
			IAuthManager authManager = ServiceProvider.AuthManager;
			if (authManager == null || !authManager.IsInitialized)
			{
				return (IsAuthorized: true, Message: "");
			}
			AuthorizationState authorizationState = authManager.GetAuthorizationState();
			if (authorizationState.HasValidLicense)
			{
				return (IsAuthorized: true, Message: "");
			}
			if (authorizationState.TrialUsage.TryGetValue("AI_Send", out var value))
			{
				int num = 10;
				MethodInfo method = authManager.GetType().GetMethod("GetMaxTrialCountFromConfig", BindingFlags.Instance | BindingFlags.NonPublic);
				if (method != null && method.Invoke(authManager, new object[1] { "AI_Send" }) is int num2 && num2 > 0)
				{
					num = num2;
				}
				if (num - value > 0)
				{
					return (IsAuthorized: true, Message: "");
				}
				return (IsAuthorized: false, Message: $"⚠\ufe0f AI 试用次数已用完（已使用 {value}/{num} 次）\n\n您可以：\n• 登录已购买许可证的账户\n• 在关于窗口购买授权");
			}
			return (IsAuthorized: true, Message: "");
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIChatPanel] AI 授权检查失败: " + ex.Message, ex);
			return (IsAuthorized: true, Message: "");
		}
	}

	private async Task RecordAIUsageAsync()
	{
		try
		{
			IAuthManager authManager = ServiceProvider.AuthManager;
			if (authManager != null && authManager.IsInitialized && !authManager.GetAuthorizationState().HasValidLicense)
			{
				await authManager.RecordFeatureUsageAsync("AI_Send");
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIChatPanel] 记录 AI 使用失败: " + ex.Message, ex);
		}
	}

	[RelayCommand]
	private async Task SendMessageAsync()
	{
		if (string.IsNullOrWhiteSpace(UserInput) && Attachments.Count == 0)
		{
			return;
		}
		int myTaskId = ++_currentTaskId;
		(bool, string) tuple = CheckAuthorization();
		if (!tuple.Item1)
		{
			Messages.Add(new ChatMessage
			{
				Content = tuple.Item2,
				IsUser = false,
				Timestamp = DateTime.Now
			});
			return;
		}
		if (_tokenUsageService != null && SelectedAIConfig != null && !SelectedAIConfig.IsUserConfig)
		{
			try
			{
				CombinedCreditsInfo combinedCreditsInfo = await _tokenUsageService.GetUserCreditsAsync(null, null, forceRefresh: true);
				if (combinedCreditsInfo != null && combinedCreditsInfo.TotalBalance <= 0m)
				{
					Messages.Add(new ChatMessage
					{
						Content = "⚠\ufe0f 电量不足，请先充值电量。\n\n您可以：\n• 在关于窗口充值电量\n• 设置自有 API Key 使用本地配置（选择模型右侧的齿轮按钮）",
						IsUser = false,
						Timestamp = DateTime.Now
					});
					return;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[AIChatPanel] 电量检查失败: " + ex.Message, ex);
			}
		}
		string userMessage = UserInput.Trim();
		UserInput = string.Empty;
		List<FileAttachment> list = Attachments.ToList();
		Attachments.Clear();
		ChatMessage item = new ChatMessage
		{
			Content = userMessage,
			IsUser = true,
			Timestamp = DateTime.Now,
			Attachments = ((list.Count > 0) ? list : null)
		};
		Messages.Add(item);
		Task.Run(async delegate
		{
			_ = 1;
			try
			{
				ISupabaseClient supabaseClient = UIBootstrapper.TryGetService<ISupabaseClient>();
				if (supabaseClient != null)
				{
					string sessionId = _currentSession?.SessionId;
					Guid? deviceId = null;
					Guid? userId = null;
					IAuthManager authManager = UIBootstrapper.TryGetService<IAuthManager>();
					if (authManager != null)
					{
						Result<IDeviceInfo> result = await authManager.EnsureDeviceRegisteredAsync();
						if (result.IsSuccess && result.Value != null)
						{
							if (result.Value.Id != Guid.Empty)
							{
								deviceId = result.Value.Id;
							}
							if (authManager.CurrentDevice is DeviceInfo { IsBound: not false, UserId: not null, UserId: var userId2 })
							{
								userId = userId2.Value;
							}
						}
					}
					await supabaseClient.RecordAIMessageAsync(userMessage, "AI_Send", sessionId, SelectedAIConfig?.Provider, SelectedAIConfig?.ModelName, deviceId, userId);
				}
			}
			catch (Exception ex10)
			{
				Logger.Debug("[AIChatPanel] 记录消息失败: " + ex10.Message);
			}
		});
		_cts?.Dispose();
		_cts = new CancellationTokenSource();
		if (SelectedAIConfig == null && _aiServiceEx == null)
		{
			Messages.Add(new ChatMessage
			{
				Content = "请先选择 AI 模型。",
				IsUser = false,
				Timestamp = DateTime.Now
			});
			return;
		}
		bool flag = _tokenUsageService != null && SelectedAIConfig != null;
		if (flag && SelectedAIConfig.IsUserConfig)
		{
			flag = false;
			Logger.Info("[AIChatPanel] \ud83d\udcdd 使用用户配置模型，不消耗电量");
		}
		if (flag)
		{
			_sessionAccumulator = _tokenUsageService.StartSession(SelectedAIConfig.Provider, SelectedAIConfig.ModelName);
			_requestStartTime = DateTime.Now;
		}
		IsProcessing = true;
		try
		{
			int num = default;
			_ = num - 1;
			_ = 6;
			try
			{
				new List<object>();
				ChatMessage typingMsg = new ChatMessage
				{
					Content = string.Empty,
					IsUser = false,
					IsTyping = true,
					Timestamp = DateTime.Now
				};
				Messages.Add(typingMsg);
				try
				{
					if (_aiServiceEx != null && SelectedAIConfig != null)
					{
						string aiResponse;
						try
						{
							if (EnableToolCalling)
							{
								ChatMessage aiMsg = new ChatMessage
								{
									Content = string.Empty,
									IsUser = false,
									Timestamp = DateTime.Now
								};
								if (list.Count > 0)
								{
									Logger.Info($"[AIChatPanel] \ud83d\udcce 检测到 {list.Count} 个附件，随消息一起发送");
								}
								await ProcessMessageWithToolCallingAsync(userMessage, SelectedAIConfig, aiMsg, _cts?.Token ?? default(CancellationToken), list);
								aiResponse = aiMsg.Content;
							}
							else
							{
								AIResponseWithThinking aIResponseWithThinking = await _aiServiceEx.SendMessageWithUsageAsync(userMessage, SelectedAIConfig, null, (list.Count > 0) ? list : null, _cts?.Token ?? default(CancellationToken));
								aiResponse = aIResponseWithThinking.Content ?? string.Empty;
								if (_sessionAccumulator != null && aIResponseWithThinking.InputTokens > 0)
								{
									int durationMs = (int)(DateTime.Now - _requestStartTime).TotalMilliseconds;
									_tokenUsageService.AccumulateTokenUsage(aIResponseWithThinking.InputTokens, aIResponseWithThinking.OutputTokens, durationMs, isToolCall: false, aIResponseWithThinking.CachedTokens);
									string value = ((aIResponseWithThinking.CachedTokens > 0) ? $", 缓存={aIResponseWithThinking.CachedTokens}" : "");
									Logger.Info($"[AIChatPanel] \ud83d\udd0b 已累积 Token: 输入={aIResponseWithThinking.InputTokens}, 输出={aIResponseWithThinking.OutputTokens}{value}");
								}
								if (_currentSession != null)
								{
									_currentSession.Messages.Add(new SessionMessage
									{
										Role = "user",
										Content = userMessage,
										Timestamp = DateTime.Now
									});
									string content = ExtractContentOnly(aiResponse ?? string.Empty, SelectedAIConfig);
									_currentSession.Messages.Add(new SessionMessage
									{
										Role = "assistant",
										Content = content,
										Timestamp = DateTime.Now
									});
									Logger.Info($"[AIChatPanel] \ud83d\udcbe 已保存对话到会话历史（总计 {_currentSession.Messages.Count} 条消息）");
								}
							}
						}
						catch (Exception ex2)
						{
							Logger.Error("[AIChatPanel] AI 服务调用失败: " + ex2.Message, ex2);
							aiResponse = "抱歉，发生了错误：" + ex2.Message;
						}
						Messages.Remove(typingMsg);
						string displayContent = ExtractContentOnly(aiResponse ?? string.Empty, SelectedAIConfig);
						await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
						{
							Messages.Add(new ChatMessage
							{
								Content = (string.IsNullOrEmpty(displayContent) ? (aiResponse ?? "未收到回复") : displayContent),
								IsUser = false,
								Timestamp = DateTime.Now
							});
						});
					}
					else if (_aiService != null)
					{
						string aiResponse2;
						try
						{
							aiResponse2 = await _aiService.SendMessageAsync(userMessage, _cts?.Token ?? default(CancellationToken));
							if (ContainsTextBasedToolCall(aiResponse2))
							{
								Logger.Error("[AIChatPanel] ❌ AI 使用了文本形式的工具调用描述，而不是标准的 Function Calling 格式");
								Logger.Error("[AIChatPanel] 响应内容: " + aiResponse2?.Substring(0, Math.Min(200, aiResponse2?.Length ?? 0)) + "...");
								Messages.Remove(typingMsg);
								await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
								{
									Messages.Add(new ChatMessage
									{
										Content = "⚠\ufe0f AI 返回了错误的格式。请重新表述您的问题或尝试新建对话。",
										IsUser = false,
										Timestamp = DateTime.Now
									});
								});
								if (_currentSession != null)
								{
									_currentSession.Messages.Add(new SessionMessage
									{
										Role = "user",
										Content = userMessage,
										Timestamp = DateTime.Now
									});
									string content2 = ExtractContentOnly(aiResponse2 ?? string.Empty, SelectedAIConfig);
									_currentSession.Messages.Add(new SessionMessage
									{
										Role = "assistant",
										Content = content2,
										Timestamp = DateTime.Now
									});
								}
								IsProcessing = false;
								return;
							}
							if (_currentSession != null)
							{
								_currentSession.Messages.Add(new SessionMessage
								{
									Role = "user",
									Content = userMessage,
									Timestamp = DateTime.Now
								});
								string content3 = ExtractContentOnly(aiResponse2 ?? string.Empty, SelectedAIConfig);
								_currentSession.Messages.Add(new SessionMessage
								{
									Role = "assistant",
									Content = content3,
									Timestamp = DateTime.Now
								});
								Logger.Info($"[AIChatPanel] \ud83d\udcbe 已保存对话到会话历史（总计 {_currentSession.Messages.Count} 条消息）");
							}
						}
						catch (Exception ex3)
						{
							Logger.Error("[AIChatPanel] AI 服务调用失败: " + ex3.Message, ex3);
							aiResponse2 = "抱歉，发生了错误：" + ex3.Message;
						}
						Messages.Remove(typingMsg);
						string displayContent2 = ExtractContentOnly(aiResponse2 ?? string.Empty, SelectedAIConfig);
						await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
						{
							Messages.Add(new ChatMessage
							{
								Content = (string.IsNullOrEmpty(displayContent2) ? (aiResponse2 ?? "未收到回复") : displayContent2),
								IsUser = false,
								Timestamp = DateTime.Now
							});
						});
					}
					else
					{
						Messages.Remove(typingMsg);
						await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
						{
							Messages.Add(new ChatMessage
							{
								Content = "AI 服务不可用。",
								IsUser = false,
								Timestamp = DateTime.Now
							});
						});
					}
				}
				catch (Exception ex4)
				{
					if (typingMsg != null && Messages.Contains(typingMsg))
					{
						Messages.Remove(typingMsg);
					}
					Messages.Add(new ChatMessage
					{
						Content = "抱歉，发生错误：" + ex4.Message,
						IsUser = false,
						Timestamp = DateTime.Now
					});
				}
				goto end_IL_0353;
			}
			catch (OperationCanceledException)
			{
				Logger.Warning("[AIChatPanel] ⚠\ufe0f 用户中途停止响应");
				if (_tokenUsageService == null || _sessionAccumulator == null)
				{
					return;
				}
				try
				{
					string generatedText = string.Join("\n", (from m in Messages
						where !m.IsUser && m.Timestamp >= _requestStartTime
						select m.Content).ToArray());
					int num2 = _sessionAccumulator.AddEstimatedUsageFromText(generatedText);
					int value2 = (int)(DateTime.Now - _requestStartTime).TotalMilliseconds;
					Logger.Warning($"[AIChatPanel] ⏱\ufe0f 响应已停止，已耗时: {value2}ms，估算 Token: {num2}");
					decimal value3 = 0m;
					if (_sessionAccumulator.TotalInputTokens > 0)
					{
						value3 = _tokenUsageService.CalculateCostSync(_sessionAccumulator.Provider, _sessionAccumulator.ModelName, _sessionAccumulator.TotalInputTokens, num2);
					}
					Messages.Add(new ChatMessage
					{
						Content = $"⚠\ufe0f 响应已停止。\n\ud83d\udcca 已消耗 Token 约 {num2} 个（基于已生成文本估算）\n\ud83d\udcb0 预计消耗电量：{value3:F4}",
						Kind = MessageKind.AIResponse,
						Timestamp = DateTime.Now
					});
				}
				catch (Exception ex6)
				{
					Logger.Error("[AIChatPanel] 估算 Token 失败: " + ex6.Message, ex6);
					Messages.Add(new ChatMessage
					{
						Content = "⚠\ufe0f 响应已停止。部分 Token 可能已被消耗。",
						Kind = MessageKind.AIResponse,
						Timestamp = DateTime.Now
					});
				}
				goto end_IL_0353;
			}
			catch (Exception ex7)
			{
				Messages.Add(new ChatMessage
				{
					Content = "抱歉，发生错误：" + ex7.Message,
					IsUser = false,
					Timestamp = DateTime.Now
				});
				goto end_IL_0353;
			}
			end_IL_0353:;
		}
		finally
		{
			if (_tokenUsageService != null && _sessionAccumulator != null)
			{
				try
				{
					await _tokenUsageService.EndSessionAndSyncAsync();
				}
				catch (Exception ex8)
				{
					Logger.Error("[AIChatPanel] ❌ Token 统计同步异常: " + ex8.Message, ex8);
				}
				finally
				{
					_sessionAccumulator = null;
				}
			}
			try
			{
				await RecordAIUsageAsync();
			}
			catch (Exception ex9)
			{
				Logger.Warning("[AIChatPanel] 记录 AI 使用失败: " + ex9.Message, ex9);
			}
			if (myTaskId == _currentTaskId)
			{
				IsProcessing = false;
			}
			if (_cts != null)
			{
				_cts.Dispose();
				_cts = null;
			}
		}
	}

	private async Task ProcessMessageWithToolCallingAsync(string userMessage, AIConfig config, ChatMessage aiMsg, CancellationToken cancellationToken, List<FileAttachment>? attachments = null)
	{
		if (_aiServiceEx == null)
		{
			return;
		}
		AISystemPromptManager systemPromptManager = new AISystemPromptManager(AIToolRegistry.Instance);
		string systemPrompt = systemPromptManager.GenerateSystemPrompt();
		string toolsDefinition = null;
		bool toolsProvided = true;
		try
		{
			AIToolRegistry instance = AIToolRegistry.Instance;
			EnsureRevitServicesInitialized();
			instance.EnsureToolsDiscovered();
			Logger.Info($"[AIChatPanel] \ud83d\udce6 自动包含工具定义，共 {instance.GetAllTools().Count()} 个工具");
			toolsDefinition = instance.GetToolsDefinitionForAI();
			if (_currentSession != null)
			{
				_currentSession.HasRequestedTools = true;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] 生成工具定义失败: " + ex.Message, ex);
		}
		List<object> conversationHistory = new List<object>
		{
			new
			{
				role = "system",
				content = systemPrompt
			}
		};
		if (_currentSession?.Messages != null && _currentSession.Messages.Count > 0)
		{
			int totalMessages = _currentSession.Messages.Count;
			List<SessionMessage> list;
			if (totalMessages > 100)
			{
				int num = 50;
				int num2 = totalMessages - 50;
				int middleCount = num2 - num;
				Logger.Info($"[AIChatPanel] \ud83d\udcca 中间部分范围: 第 {num + 1}-{num2} 条（共 {middleCount} 条）");
				if (_currentSession.SummaryMessageCount < middleCount || string.IsNullOrEmpty(_currentSession.AccumulatedSummary))
				{
					string text = await GenerateSessionSummaryForRangeAsync(_currentSession, num, middleCount);
					if (!string.IsNullOrEmpty(text))
					{
						_currentSession.AccumulatedSummary = text;
						_currentSession.SummaryMessageCount = middleCount;
						Logger.Info($"[AIChatPanel] \ud83d\udcdd 已更新累积摘要（覆盖 {middleCount} 条消息）");
					}
				}
				else
				{
					Logger.Info($"[AIChatPanel] ♻\ufe0f 复用现有累积摘要（{_currentSession.SummaryMessageCount} 条消息）");
				}
				List<SessionMessage> first = _currentSession.Messages.Take(50).ToList();
				List<SessionMessage> second = _currentSession.Messages.Skip(totalMessages - 50).ToList();
				list = first.Concat(second).ToList();
				if (!string.IsNullOrEmpty(_currentSession.AccumulatedSummary))
				{
					conversationHistory.Add(new
					{
						role = "system",
						content = $"## \ud83d\udcdd 会话摘要（{_currentSession.SummaryMessageCount} 条历史对话的压缩内容）\n\n{_currentSession.AccumulatedSummary}\n\n---\n以上是会话中间内容的累积摘要，以下是完整的对话。"
					});
				}
			}
			else
			{
				list = _currentSession.Messages.ToList();
				Logger.Info($"[AIChatPanel] \ud83d\udcdc 已加载全部 {list.Count} 条历史对话（未压缩）");
			}
			HashSet<string> hashSet = new HashSet<string>();
			foreach (SessionMessage item5 in list)
			{
				if (!(item5.Role == "assistant") || item5.ToolCalls == null)
				{
					continue;
				}
				foreach (ToolCallData toolCall3 in item5.ToolCalls)
				{
					if (toolCall3.ToolName == "save_memory" && !string.IsNullOrEmpty(toolCall3.CallId))
					{
						hashSet.Add(toolCall3.CallId);
					}
				}
			}
			Logger.Info($"[AIChatPanel] \ud83e\udde0 检测到 {hashSet.Count} 个 save_memory 调用，将在历史中保留其结果");
			for (int i = 0; i < list.Count; i++)
			{
				SessionMessage sessionMessage = list[i];
				if (sessionMessage.Role == "system" || sessionMessage.Role == "tool")
				{
					continue;
				}
				string text2 = sessionMessage.Content ?? "";
				if (sessionMessage.Role == "assistant")
				{
					text2 = ExtractContentOnly(text2, SelectedAIConfig);
				}
				if (sessionMessage.Role == "assistant" && sessionMessage.ToolCalls != null && sessionMessage.ToolCalls.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (!string.IsNullOrEmpty(text2))
					{
						stringBuilder.AppendLine(text2);
					}
					int j;
					for (j = i + 1; j < list.Count && list[j].Role == "tool"; j++)
					{
						SessionMessage sessionMessage2 = list[j];
						if (!string.IsNullOrEmpty(sessionMessage2.ToolCallId) && hashSet.Contains(sessionMessage2.ToolCallId))
						{
							stringBuilder.AppendLine(sessionMessage2.Content ?? string.Empty);
						}
					}
					i = j - 1;
					text2 = stringBuilder.ToString().TrimEnd();
					if (string.IsNullOrWhiteSpace(text2))
					{
						continue;
					}
				}
				conversationHistory.Add(new
				{
					role = sessionMessage.Role,
					content = text2
				});
			}
		}
		conversationHistory.Add(new
		{
			role = "user",
			content = userMessage
		});
		int newTurnStartIndex = conversationHistory.Count - 1;
		try
		{
			StringBuilder allReasoningContent = new StringBuilder();
			List<string> toolCallsInCurrentRound = new List<string>();
			int totalMessages = 0;
			string aiResponse;
			while (true)
			{
				totalMessages++;
				Logger.Info($"[AIChatPanel] ========== 第 {totalMessages} 轮对话 ==========");
				List<object> list2 = new List<object>();
				foreach (object item6 in conversationHistory)
				{
					if (item6 is Dictionary<string, object> dictionary)
					{
						Dictionary<string, object> dictionary2 = new Dictionary<string, object>(dictionary);
						if (dictionary2.ContainsKey("content") && dictionary2["content"] is string response)
						{
							string value = ExtractContentOnly(response, config);
							dictionary2["content"] = value;
						}
						if (!dictionary2.ContainsKey("tool_calls") && dictionary2.ContainsKey("reasoning_content"))
						{
							dictionary2.Remove("reasoning_content");
						}
						if (dictionary2.ContainsKey("thinking"))
						{
							dictionary2.Remove("thinking");
						}
						list2.Add(dictionary2);
						continue;
					}
					JObject jObject = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(item6));
					if (jObject != null)
					{
						JToken jToken = jObject["content"];
						if (jToken != null && jToken.Type == JTokenType.String)
						{
							string response2 = jToken.ToString() ?? "";
							string text3 = ExtractContentOnly(response2, config);
							jObject["content"] = text3;
						}
						if (jObject["tool_calls"] == null && jObject["reasoning_content"] != null)
						{
							jObject.Remove("reasoning_content");
						}
						if (jObject["thinking"] != null)
						{
							jObject.Remove("thinking");
						}
						list2.Add(jObject);
					}
					else
					{
						list2.Add(item6);
					}
				}
				string message = JsonConvert.SerializeObject(list2);
				AIResponseWithThinking aIResponseWithThinking = await _aiServiceEx.SendMessageWithUsageAsync(message, config, toolsDefinition, (totalMessages == 1) ? attachments : null, cancellationToken);
				aiResponse = aIResponseWithThinking.Content;
				if (_sessionAccumulator != null && aIResponseWithThinking.InputTokens > 0)
				{
					int durationMs = (int)(DateTime.Now - _requestStartTime).TotalMilliseconds;
					_tokenUsageService.AccumulateTokenUsage(aIResponseWithThinking.InputTokens, aIResponseWithThinking.OutputTokens, durationMs, aIResponseWithThinking.ToolCalls != null && aIResponseWithThinking.ToolCalls.Count > 0, aIResponseWithThinking.CachedTokens);
				}
				if (string.IsNullOrEmpty(aiResponse) && aIResponseWithThinking.ToolCalls == null)
				{
					Logger.Error("[AIChatPanel] AI 返回了空响应");
					aiMsg.Content = "抱歉，AI 服务返回了空响应。请稍后再试。";
					return;
				}
				List<object> toolResponseMessages;
				if (aIResponseWithThinking.ToolCalls != null && aIResponseWithThinking.ToolCalls.Count > 0)
				{
					if (!string.IsNullOrEmpty(aIResponseWithThinking.ReasoningContent))
					{
						allReasoningContent.Append(aIResponseWithThinking.ReasoningContent);
					}
					List<RevitAi.Core.AI.ToolCallInfo> callsFromService = aIResponseWithThinking.ToolCalls.Select((RevitAi.Core.AI.ToolCallInfo tc) => new RevitAi.Core.AI.ToolCallInfo
					{
						ToolName = tc.ToolName,
						Parameters = tc.Parameters,
						CallId = tc.CallId
					}).ToList();
					toolResponseMessages = new List<object>();
					foreach (RevitAi.Core.AI.ToolCallInfo toolCall in callsFromService)
					{
						bool isMemoryTool = toolCall.ToolName == "save_memory";
						ChatMessage toolCallMessage = null;
						if (!isMemoryTool)
						{
							await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
							{
								toolCallMessage = new ChatMessage
								{
									Content = "\ud83d\udd27 正在调用工具: " + toolCall.ToolName + "...",
									Kind = MessageKind.AIToolResult,
									ToolCall = new RevitAi.UI.Models.ToolCallInfo
									{
										ToolName = toolCall.ToolName,
										Parameters = toolCall.Parameters,
										CallId = toolCall.CallId
									},
									Timestamp = DateTime.Now,
									SavableCode = null
								};
								Messages.Add(toolCallMessage);
								EnsureTypingMessageAtEnd();
							});
						}
						string toolResult = await ExecuteToolCallAsync(new RevitAi.UI.Models.ToolCallInfo
						{
							ToolName = toolCall.ToolName,
							Parameters = toolCall.Parameters,
							CallId = toolCall.CallId
						}, cancellationToken);
						if (isMemoryTool)
						{
							Logger.Info("[AIChatPanel] \ud83e\udde0 save_memory 已执行（静默模式，结果仅返回给 AI）");
						}
						else
						{
							string displayResult = RemoveReturnDataFromResult(toolResult);
							await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
							{
								if (toolCallMessage != null)
								{
									bool flag = displayResult.Contains("执行成功");
									toolCallMessage.Kind = (flag ? MessageKind.AIToolResult : MessageKind.AIToolError);
									toolCallMessage.Content = displayResult;
									toolCallMessage.SavableCode = null;
									if (flag && toolCall.ToolName == "execute_code" && toolCall.Parameters != null)
									{
										toolCall.Parameters.TryGetValue("code", out object value2);
										string text8 = value2?.ToString();
										if (!string.IsNullOrWhiteSpace(text8))
										{
											toolCallMessage.SavableCode = text8;
										}
									}
									int num3 = Messages.IndexOf(toolCallMessage);
									if (num3 >= 0)
									{
										Messages.RemoveAt(num3);
										Messages.Insert(num3, toolCallMessage);
									}
								}
							});
						}
						toolCallsInCurrentRound.Add(toolCall.ToolName);
						Task.Run(async delegate
						{
							try
							{
								if (toolCall.Parameters == null)
								{
									_ = string.Empty;
								}
								else
								{
									JsonConvert.SerializeObject(toolCall.Parameters);
								}
							}
							catch (Exception ex3)
							{
								Logger.Warning("[AIChatPanel] ⚠\ufe0f 工具调用处理失败: " + ex3.Message);
							}
						});
						toolResponseMessages.Add(new
						{
							tool_call_id = toolCall.CallId,
							content = toolResult
						});
					}
					List<object> list3 = new List<object>();
					foreach (RevitAi.Core.AI.ToolCallInfo item7 in callsFromService)
					{
						list3.Add(new
						{
							id = item7.CallId,
							type = "function",
							function = new
							{
								name = item7.ToolName,
								arguments = JsonConvert.SerializeObject(item7.Parameters)
							}
						});
					}
					string text4 = (string.IsNullOrEmpty(allReasoningContent.ToString()) ? null : allReasoningContent.ToString());
					Dictionary<string, object> dictionary3 = new Dictionary<string, object>
					{
						["role"] = "assistant",
						["tool_calls"] = list3
					};
					if (config.Provider.Equals("deepseek", StringComparison.OrdinalIgnoreCase) || config.Provider.Equals("vllm", StringComparison.OrdinalIgnoreCase))
					{
						dictionary3["reasoning_content"] = text4 ?? string.Empty;
					}
					else
					{
						dictionary3["content"] = string.Empty;
						if (!string.IsNullOrEmpty(text4))
						{
							dictionary3["reasoning_content"] = text4;
						}
					}
					conversationHistory.Add(dictionary3);
					foreach (object item8 in toolResponseMessages)
					{
						Type type = item8.GetType();
						PropertyInfo property = type.GetProperty("tool_call_id");
						PropertyInfo? property2 = type.GetProperty("content");
						object tool_call_id = property?.GetValue(item8);
						object content = property2?.GetValue(item8);
						conversationHistory.Add(new
						{
							role = "tool",
							tool_call_id = tool_call_id,
							content = content
						});
					}
					if (_currentSession != null)
					{
						_currentSession.RoundCount = totalMessages + 1;
						if (_sessionManager != null && _sessionManager.ShouldAutoSave(_currentSession))
						{
							await SaveCurrentSessionAsync($"对话进行到第{_currentSession.RoundCount}轮，已自动保存");
						}
					}
					continue;
				}
				bool item = DetectResponseType(aiResponse, config).hasThinking;
				if (!string.IsNullOrWhiteSpace(aiResponse))
				{
					var (text5, text6) = SpecialCommandHandler.ParseSpecialCommand(aiResponse ?? string.Empty);
					if (!string.IsNullOrEmpty(text5))
					{
						Logger.Info("[AIChatPanel] \ud83d\udd27 检测到特殊指令: " + text5 + " " + text6);
						(string, string, bool) obj = await HandleSpecialCommandAsync(text5, text6, conversationHistory, systemPromptManager, systemPrompt, toolsDefinition, toolsProvided);
						string item2 = obj.Item1;
						string item3 = obj.Item2;
						bool item4 = obj.Item3;
						systemPrompt = item2;
						toolsDefinition = item3;
						toolsProvided = item4;
						continue;
					}
				}
				if (item)
				{
					string thinkingContent = ExtractThinkingContent(aiResponse, config);
					allReasoningContent.Append(thinkingContent);
					await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
					{
						Messages.Add(new ChatMessage
						{
							Content = thinkingContent,
							Kind = MessageKind.AIThinking,
							Timestamp = DateTime.Now
						});
						EnsureTypingMessageAtEnd();
					});
				}
				if (!IsMultiToolCallResponse(aiResponse, out List<RevitAi.Core.AI.ToolCallInfo> toolCalls) || toolCalls == null || toolCalls.Count <= 0)
				{
					break;
				}
				toolResponseMessages = new List<object>();
				foreach (RevitAi.Core.AI.ToolCallInfo toolCall2 in toolCalls)
				{
					bool isMemoryTool = toolCall2.ToolName == "save_memory";
					ChatMessage toolCallMessage2 = null;
					if (!isMemoryTool)
					{
						await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
						{
							toolCallMessage2 = new ChatMessage
							{
								Content = "\ud83d\udd27 正在调用工具: " + toolCall2.ToolName + "...",
								Kind = MessageKind.AIToolResult,
								ToolCall = new RevitAi.UI.Models.ToolCallInfo
								{
									ToolName = toolCall2.ToolName,
									Parameters = toolCall2.Parameters,
									CallId = toolCall2.CallId
								},
								Timestamp = DateTime.Now,
								SavableCode = null
							};
							Messages.Add(toolCallMessage2);
							EnsureTypingMessageAtEnd();
						});
					}
					string toolResult = await ExecuteToolCallAsync(new RevitAi.UI.Models.ToolCallInfo
					{
						ToolName = toolCall2.ToolName,
						Parameters = toolCall2.Parameters,
						CallId = toolCall2.CallId
					}, cancellationToken);
					if (isMemoryTool)
					{
						Logger.Info("[AIChatPanel] \ud83e\udde0 save_memory 已执行（静默模式，结果仅返回给 AI）");
					}
					else
					{
						string displayResult2 = RemoveReturnDataFromResult(toolResult);
						await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
						{
							if (toolCallMessage2 != null)
							{
								bool flag = displayResult2.Contains("执行成功");
								toolCallMessage2.Kind = (flag ? MessageKind.AIToolResult : MessageKind.AIToolError);
								toolCallMessage2.Content = displayResult2;
								if (flag && toolCall2.ToolName == "execute_code" && toolCall2.Parameters != null)
								{
									toolCall2.Parameters.TryGetValue("code", out object value2);
									string text8 = value2?.ToString();
									if (!string.IsNullOrWhiteSpace(text8))
									{
										toolCallMessage2.SavableCode = text8;
									}
								}
								int num3 = Messages.IndexOf(toolCallMessage2);
								if (num3 >= 0)
								{
									Messages.RemoveAt(num3);
									Messages.Insert(num3, toolCallMessage2);
								}
							}
						});
					}
					Task.Run(async delegate
					{
						try
						{
							if (toolCall2.Parameters == null)
							{
								_ = string.Empty;
							}
							else
							{
								JsonConvert.SerializeObject(toolCall2.Parameters);
							}
						}
						catch (Exception ex3)
						{
							Logger.Warning("[AIChatPanel] ⚠\ufe0f 工具调用处理失败: " + ex3.Message);
						}
					});
					toolResponseMessages.Add(new
					{
						tool_call_id = toolCall2.CallId,
						content = toolResult
					});
				}
				List<object> list4 = new List<object>();
				foreach (RevitAi.Core.AI.ToolCallInfo item9 in toolCalls)
				{
					list4.Add(new
					{
						id = item9.CallId,
						type = "function",
						function = new
						{
							name = item9.ToolName,
							arguments = JsonConvert.SerializeObject(item9.Parameters)
						}
					});
				}
				string text7 = (string.IsNullOrEmpty(allReasoningContent.ToString()) ? null : allReasoningContent.ToString());
				Dictionary<string, object> dictionary4 = new Dictionary<string, object>
				{
					["role"] = "assistant",
					["content"] = ExtractContentOnly(aiResponse ?? string.Empty, config) ?? null,
					["tool_calls"] = list4
				};
				if (config.Provider.Equals("deepseek", StringComparison.OrdinalIgnoreCase) || config.Provider.Equals("vllm", StringComparison.OrdinalIgnoreCase))
				{
					dictionary4["reasoning_content"] = text7 ?? string.Empty;
				}
				else if (!string.IsNullOrEmpty(text7))
				{
					dictionary4["reasoning_content"] = text7;
				}
				conversationHistory.Add(dictionary4);
				foreach (object item10 in toolResponseMessages)
				{
					Type type2 = item10.GetType();
					PropertyInfo property3 = type2.GetProperty("tool_call_id");
					PropertyInfo? property4 = type2.GetProperty("content");
					object tool_call_id2 = property3?.GetValue(item10);
					object content2 = property4?.GetValue(item10);
					conversationHistory.Add(new
					{
						role = "tool",
						tool_call_id = tool_call_id2,
						content = content2
					});
				}
				if (_currentSession != null)
				{
					_currentSession.RoundCount = totalMessages + 1;
					if (_sessionManager != null && _sessionManager.ShouldAutoSave(_currentSession))
					{
						await SaveCurrentSessionAsync($"对话进行到第{_currentSession.RoundCount}轮，已自动保存");
					}
				}
			}
			string content3 = ExtractContentOnly(aiResponse ?? string.Empty, config);
			aiMsg.Content = content3;
			PersistFullTurnToSession(conversationHistory, newTurnStartIndex, aiMsg.Content ?? string.Empty);
		}
		catch (OperationCanceledException)
		{
			Logger.Warning("[AIChatPanel] ⚠\ufe0f 用户中途停止响应，保存已生成的部分对话到历史");
			PersistFullTurnToSession(conversationHistory, newTurnStartIndex, null);
			throw;
		}
	}

	private void PersistFullTurnToSession(List<object> conversationHistory, int newTurnStartIndex, string? finalAssistantContent)
	{
		if (_currentSession == null || conversationHistory == null || newTurnStartIndex < 0)
		{
			return;
		}
		int num = 0;
		for (int i = newTurnStartIndex; i < conversationHistory.Count; i++)
		{
			SessionMessage sessionMessage = ConvertHistoryItemToSessionMessage(conversationHistory[i]);
			if (sessionMessage != null)
			{
				_currentSession.Messages.Add(sessionMessage);
				num++;
			}
		}
		if (!string.IsNullOrEmpty(finalAssistantContent))
		{
			_currentSession.Messages.Add(new SessionMessage
			{
				Role = "assistant",
				Content = finalAssistantContent,
				Timestamp = DateTime.Now
			});
			num++;
		}
		if (num > 0)
		{
			Logger.Info($"[AIChatPanel] \ud83d\udcbe 已保存本轮对话到会话历史（新增 {num} 条，总计 {_currentSession.Messages.Count} 条）");
		}
	}

	private SessionMessage? ConvertHistoryItemToSessionMessage(object item)
	{
		if (item == null)
		{
			return null;
		}
		if (item is Dictionary<string, object> dictionary)
		{
			object value;
			switch ((!dictionary.TryGetValue("role", out value)) ? null : value?.ToString())
			{
			case "user":
			{
				object value7;
				return new SessionMessage
				{
					Role = "user",
					Content = ((!dictionary.TryGetValue("content", out value7)) ? string.Empty : (value7?.ToString() ?? string.Empty)),
					Timestamp = DateTime.Now
				};
			}
			case "tool":
			{
				object value8;
				object value9;
				return new SessionMessage
				{
					Role = "tool",
					Content = ((!dictionary.TryGetValue("content", out value8)) ? string.Empty : (value8?.ToString() ?? string.Empty)),
					ToolCallId = ((!dictionary.TryGetValue("tool_call_id", out value9)) ? null : value9?.ToString()),
					Timestamp = DateTime.Now
				};
			}
			case "assistant":
			{
				SessionMessage sessionMessage = new SessionMessage
				{
					Role = "assistant",
					Timestamp = DateTime.Now
				};
				if (dictionary.TryGetValue("content", out var value2) && value2 is string content)
				{
					sessionMessage.Content = content;
				}
				if (dictionary.TryGetValue("reasoning_content", out var value3) && value3 is string reasoningContent)
				{
					sessionMessage.ReasoningContent = reasoningContent;
				}
				if (dictionary.TryGetValue("tool_calls", out var value4) && value4 is IEnumerable enumerable)
				{
					List<ToolCallData> list = new List<ToolCallData>();
					foreach (object item2 in enumerable)
					{
						if (item2 == null)
						{
							continue;
						}
						Type type = item2.GetType();
						PropertyInfo property = type.GetProperty("id");
						PropertyInfo property2 = type.GetProperty("function");
						string callId = property?.GetValue(item2)?.ToString();
						string text = null;
						Dictionary<string, object> dictionary2 = null;
						if (property2 != null)
						{
							object value5 = property2.GetValue(item2);
							if (value5 != null)
							{
								Type type2 = value5.GetType();
								text = type2.GetProperty("name")?.GetValue(value5)?.ToString();
								string value6 = type2.GetProperty("arguments")?.GetValue(value5)?.ToString();
								if (!string.IsNullOrEmpty(value6))
								{
									try
									{
										dictionary2 = JsonConvert.DeserializeObject<Dictionary<string, object>>(value6);
									}
									catch
									{
										dictionary2 = null;
									}
								}
							}
						}
						list.Add(new ToolCallData
						{
							ToolName = (text ?? string.Empty),
							CallId = callId,
							Parameters = (dictionary2 ?? new Dictionary<string, object>())
						});
					}
					if (list.Count > 0)
					{
						sessionMessage.ToolCalls = list;
					}
				}
				return sessionMessage;
			}
			default:
				return null;
			}
		}
		Type type3 = item.GetType();
		PropertyInfo property3 = type3.GetProperty("role");
		if (property3 == null)
		{
			return null;
		}
		switch (property3.GetValue(item)?.ToString())
		{
		case "user":
			return new SessionMessage
			{
				Role = "user",
				Content = (type3.GetProperty("content")?.GetValue(item)?.ToString() ?? string.Empty),
				Timestamp = DateTime.Now
			};
		case "tool":
			return new SessionMessage
			{
				Role = "tool",
				Content = (type3.GetProperty("content")?.GetValue(item)?.ToString() ?? string.Empty),
				ToolCallId = type3.GetProperty("tool_call_id")?.GetValue(item)?.ToString(),
				Timestamp = DateTime.Now
			};
		case "assistant":
		{
			SessionMessage sessionMessage2 = new SessionMessage
			{
				Role = "assistant",
				Timestamp = DateTime.Now
			};
			if (type3.GetProperty("content")?.GetValue(item) is string content2)
			{
				sessionMessage2.Content = content2;
			}
			return sessionMessage2;
		}
		default:
			return null;
		}
	}

	private async Task<string> ExecuteToolCallAsync(RevitAi.UI.Models.ToolCallInfo toolCall, CancellationToken cancellationToken)
	{
		if (_toolInvocationHandler == null)
		{
			Logger.Warning("[AIChatPanel] ⚠\ufe0f 工具调用处理器未初始化");
			return "错误: 工具调用处理器未初始化";
		}
		IRevitAdapter revitAdapter = _revitAdapter;
		if (revitAdapter == null)
		{
			try
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					try
					{
						if (assembly.FullName == null || !assembly.FullName.StartsWith("RevitAi.Revit"))
						{
							continue;
						}
						Type type = assembly.GetType("RevitAi.Revit.UI.RevitAdapterManager");
						if (!(type != null))
						{
							continue;
						}
						PropertyInfo property = type.GetProperty("CurrentAdapter");
						if (property != null)
						{
							revitAdapter = property.GetValue(null) as IRevitAdapter;
							if (revitAdapter != null)
							{
								Logger.Info("[AIChatPanel] ✅ 从 " + assembly.GetName().Name + " 获取到 RevitAdapter");
								_revitAdapter = revitAdapter;
								break;
							}
						}
					}
					catch
					{
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[AIChatPanel] 从 RevitAdapterManager 获取 RevitAdapter 失败: " + ex.Message);
			}
		}
		if (revitAdapter == null)
		{
			Logger.Warning("[AIChatPanel] ⚠\ufe0f RevitAdapter 未设置");
			return "错误: Revit 适配器未初始化，请确保在 Revit 中运行此插件";
		}
		try
		{
			object activeDocument = revitAdapter.GetActiveDocument();
			if (activeDocument == null)
			{
				Logger.Warning("[AIChatPanel] ⚠\ufe0f 无法获取活动文档");
				return "错误: 无法获取活动文档，请确保有打开的 Revit 文档";
			}
			AIToolContext aIToolContext = new AIToolContext
			{
				Document = activeDocument,
				RevitAdapter = revitAdapter,
				ExternalEvent = _externalEvent,
				SessionId = _currentSession?.SessionId
			};
			if (toolCall.Parameters != null)
			{
				foreach (KeyValuePair<string, object> parameter in toolCall.Parameters)
				{
					try
					{
						if (parameter.Value is JObject jObject)
						{
							Dictionary<string, object> dictionary = jObject.ToObject<Dictionary<string, object>>();
							aIToolContext.Parameters[parameter.Key] = dictionary ?? new Dictionary<string, object>();
						}
						else if (parameter.Value is JArray jArray)
						{
							List<object> list = jArray.ToObject<List<object>>();
							aIToolContext.Parameters[parameter.Key] = list ?? new List<object>();
						}
						else if (parameter.Value is JValue jValue)
						{
							aIToolContext.Parameters[parameter.Key] = jValue.Value ?? string.Empty;
						}
						else
						{
							aIToolContext.Parameters[parameter.Key] = parameter.Value ?? string.Empty;
						}
					}
					catch (Exception ex2)
					{
						Logger.Warning($"[AIChatPanel] 参数 {parameter.Key} 转换失败: {ex2.Message}，使用空字符串");
						aIToolContext.Parameters[parameter.Key] = string.Empty;
					}
				}
			}
			RevitAi.Core.AI.ToolCallInfo toolCall2 = new RevitAi.Core.AI.ToolCallInfo
			{
				ToolName = toolCall.ToolName,
				Parameters = aIToolContext.Parameters,
				CallId = toolCall.CallId
			};
			AIToolResult aIToolResult = await _toolInvocationHandler.HandleToolCallAsync(toolCall2, aIToolContext, cancellationToken);
			if (aIToolResult.Success)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
				handler.AppendLiteral("✅ ");
				handler.AppendFormatted(toolCall.ToolName);
				handler.AppendLiteral(" 执行成功");
				stringBuilder2.AppendLine(ref handler);
				if (!string.IsNullOrEmpty(aIToolResult.Message))
				{
					stringBuilder.AppendLine(aIToolResult.Message);
				}
				if (aIToolResult.Data != null)
				{
					stringBuilder.AppendLine("返回数据:");
					JsonSerializerSettings settings = new JsonSerializerSettings
					{
						Formatting = Formatting.Indented,
						Error = delegate(object? sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
						{
							args.ErrorContext.Handled = true;
						},
						NullValueHandling = NullValueHandling.Ignore,
						ReferenceLoopHandling = ReferenceLoopHandling.Ignore
					};
					stringBuilder.AppendLine(JsonConvert.SerializeObject(aIToolResult.Data, settings));
				}
				return stringBuilder.ToString();
			}
			return "❌ " + toolCall.ToolName + " 执行失败: " + (aIToolResult.Error ?? "未知错误");
		}
		catch (Exception ex3)
		{
			Logger.Error("[AIChatPanel] ❌ 执行工具 " + toolCall.ToolName + " 时发生异常", ex3);
			return "❌ 执行工具时发生异常: " + ex3.Message;
		}
	}

	private static string RemoveReturnDataFromResult(string result)
	{
		if (string.IsNullOrEmpty(result))
		{
			return result;
		}
		int num = result.IndexOf("返回数据:");
		if (num > 0)
		{
			return result.Substring(0, num).Trim();
		}
		return result;
	}

	private (bool hasThinking, bool hasToolCalls) DetectResponseType(string? response, AIConfig config)
	{
		if (string.IsNullOrEmpty(response))
		{
			return (hasThinking: false, hasToolCalls: false);
		}
		bool item = IsMultiToolCallResponse(response, out List<RevitAi.Core.AI.ToolCallInfo> toolCalls) && toolCalls != null && toolCalls.Count > 0;
		return (hasThinking: response.Contains("\"reasoning_content\"") || response.Contains("\"thinking\""), hasToolCalls: item);
	}

	private string ExtractThinkingContent(string? response, AIConfig config)
	{
		if (string.IsNullOrEmpty(response))
		{
			return string.Empty;
		}
		try
		{
			JObject jObject = JsonConvert.DeserializeObject<JObject>(response);
			if (jObject == null)
			{
				return string.Empty;
			}
			string text = jObject["reasoning_content"]?.ToString() ?? string.Empty;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			if (jObject["content"] is JArray jArray)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (JToken item in jArray)
				{
					if (item != null && item["type"]?.ToString() == "thinking")
					{
						string value = item["thinking"]?.ToString();
						if (!string.IsNullOrEmpty(value))
						{
							stringBuilder.AppendLine(value);
						}
					}
				}
				return stringBuilder.ToString();
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIChatPanel] 提取思考内容失败: " + ex.Message);
		}
		return string.Empty;
	}

	private string ExtractContentOnly(string response, AIConfig config)
	{
		if (string.IsNullOrWhiteSpace(response))
		{
			return response;
		}
		string text = response.TrimStart();
		bool flag = text.StartsWith("{") || text.Contains("\"reasoning_content\":");
		if (!flag)
		{
			return response;
		}
		try
		{
			JObject jObject = JsonConvert.DeserializeObject<JObject>(response);
			if (jObject == null)
			{
				return response;
			}
			if (config.Provider.Equals("claude", StringComparison.OrdinalIgnoreCase))
			{
				if (jObject["content"] is JArray jArray)
				{
					foreach (JToken item in jArray)
					{
						if (item != null && item["type"]?.ToString() == "text")
						{
							string text2 = item["text"]?.ToString() ?? string.Empty;
							if (!string.IsNullOrEmpty(text2))
							{
								return text2;
							}
						}
					}
				}
			}
			else if (jObject["choices"] is JArray { Count: >0 } jArray2 && jArray2[0] is JObject jObject2 && jObject2["message"] is JObject jObject3)
			{
				string text3 = jObject3["content"]?.ToString() ?? string.Empty;
				if (!string.IsNullOrEmpty(text3))
				{
					return text3;
				}
			}
		}
		catch (Exception ex)
		{
			if (flag)
			{
				Logger.Warning("[AIChatPanel] ExtractContentOnly 解析失败: " + ex.Message);
			}
			if (response.Contains("\"reasoning_content\":") || response.Contains("\"content\":"))
			{
				try
				{
					JObject jObject4 = JsonConvert.DeserializeObject<JObject>(response);
					if (jObject4 != null)
					{
						if (jObject4["choices"] is JArray { Count: >0 } jArray3 && jArray3[0] is JObject jObject5 && jObject5["message"] is JObject jObject6)
						{
							string text4 = jObject6["content"]?.ToString();
							if (!string.IsNullOrEmpty(text4))
							{
								Logger.Info("[AIChatPanel] ExtractContentOnly 从异常处理中提取 content 成功");
								return text4;
							}
						}
						string text5 = jObject4["content"]?.ToString();
						if (!string.IsNullOrEmpty(text5))
						{
							Logger.Info("[AIChatPanel] ExtractContentOnly 从顶层 content 提取成功");
							return text5;
						}
					}
				}
				catch (Exception ex2)
				{
					Logger.Warning("[AIChatPanel] ExtractContentOnly 异常处理也失败: " + ex2.Message);
				}
			}
		}
		return response;
	}

	private AIChatSession CreateNewSession()
	{
		AIChatSession aIChatSession = new AIChatSession
		{
			Title = $"对话 {DateTime.Now:yyyy-MM-dd HH:mm}",
			Messages = new List<SessionMessage>(),
			AIConfig = SelectedAIConfig,
			HasRequestedTools = false,
			AccumulatedSummary = null,
			SummaryMessageCount = 0
		};
		Logger.Info("[AIChatPanel] ✨ 创建新会话: " + aIChatSession.Title);
		Logger.Info("[AIChatPanel] \ud83d\udce6 工具定义请求状态: 未请求（等待AI首次请求）");
		return aIChatSession;
	}

	private bool IsMultiToolCallResponse(string? response, out List<RevitAi.Core.AI.ToolCallInfo>? toolCalls)
	{
		toolCalls = null;
		if (string.IsNullOrEmpty(response))
		{
			return false;
		}
		try
		{
			if (response.StartsWith("TOOL_CALLS:"))
			{
				JArray jArray = JArray.Parse(response.Substring("TOOL_CALLS:".Length));
				toolCalls = new List<RevitAi.Core.AI.ToolCallInfo>();
				foreach (JToken item in jArray)
				{
					if (item is JObject jObject)
					{
						string toolName = jObject["ToolName"]?.ToString() ?? jObject["name"]?.ToString() ?? jObject["tool_name"]?.ToString() ?? jObject["function"]?.ToString() ?? string.Empty;
						RevitAi.Core.AI.ToolCallInfo toolCallInfo = new RevitAi.Core.AI.ToolCallInfo
						{
							ToolName = toolName,
							CallId = (jObject["CallId"]?.ToString() ?? jObject["call_id"]?.ToString() ?? jObject["id"]?.ToString() ?? string.Empty)
						};
						if ((jObject["Parameters"] ?? jObject["parameters"]) is JObject token)
						{
							toolCallInfo.Parameters = ConvertJObjectToDictionary(token);
						}
						toolCalls.Add(toolCallInfo);
					}
				}
				return toolCalls.Count > 0;
			}
			if (response.StartsWith("TOOL_CALL:"))
			{
				JObject jObject2 = JObject.Parse(response.Substring("TOOL_CALL:".Length));
				string toolName2 = jObject2["ToolName"]?.ToString() ?? jObject2["name"]?.ToString() ?? jObject2["tool_name"]?.ToString() ?? jObject2["function"]?.ToString() ?? string.Empty;
				RevitAi.Core.AI.ToolCallInfo toolCallInfo2 = new RevitAi.Core.AI.ToolCallInfo
				{
					ToolName = toolName2,
					CallId = (jObject2["CallId"]?.ToString() ?? jObject2["call_id"]?.ToString() ?? jObject2["id"]?.ToString() ?? string.Empty)
				};
				if ((jObject2["Parameters"] ?? jObject2["parameters"]) is JObject token2)
				{
					toolCallInfo2.Parameters = ConvertJObjectToDictionary(token2);
				}
				toolCalls = new List<RevitAi.Core.AI.ToolCallInfo> { toolCallInfo2 };
				return true;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AI] 解析工具调用失败: " + ex.Message, ex);
		}
		return false;
	}

	private IDictionary<string, object>? ConvertJObjectToDictionary(JToken? token)
	{
		if (token == null)
		{
			return null;
		}
		try
		{
			if (token is JObject jObject)
			{
				return jObject.ToObject<Dictionary<string, object>>();
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIChatPanel] 参数 Dictionary 转换失败: " + ex.Message);
		}
		return null;
	}

	private object? ConvertJTokenToObject(JToken token)
	{
		if (token == null || token.Type == JTokenType.Null)
		{
			return null;
		}
		try
		{
			return token.Type switch
			{
				JTokenType.Object => token.ToObject<Dictionary<string, object>>(), 
				JTokenType.Array => token.ToObject<List<object>>(), 
				JTokenType.String => token.ToString(), 
				JTokenType.Integer => token.ToObject<int>(), 
				JTokenType.Float => token.ToObject<double>(), 
				JTokenType.Boolean => token.ToObject<bool>(), 
				JTokenType.Guid => token.ToObject<Guid>(), 
				JTokenType.Uri => token.ToObject<Uri>(), 
				_ => token.ToString(), 
			};
		}
		catch (Exception ex)
		{
			Logger.Warning($"[AIChatPanel] JToken 转换失败 (类型: {token.Type}): {ex.Message}，降级为字符串");
			return token.ToString();
		}
	}

	[Obsolete("请使用 IsMultiToolCallResponse 代替")]
	private bool IsToolCallResponse(string response, out RevitAi.UI.Models.ToolCallInfo? toolCall)
	{
		toolCall = null;
		try
		{
			if (response.StartsWith("TOOL_CALL:"))
			{
				string value = response.Substring("TOOL_CALL:".Length);
				toolCall = JsonConvert.DeserializeObject<RevitAi.UI.Models.ToolCallInfo>(value);
				return toolCall != null;
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	private bool ContainsTextBasedToolCall(string? response)
	{
		if (string.IsNullOrEmpty(response))
		{
			return false;
		}
		string[] array = new string[2] { "[工具调用:", "[Tool Call:" };
		foreach (string text in array)
		{
			if (Regex.IsMatch(response, Regex.Escape(text).Replace("工具调用", "\\s*工具\\s*调用").Replace("调用.*工具", "调用\\w+工具")
				.Replace("查询结果：", "查询结果\\s*[:：]")))
			{
				Logger.Warning("[AIChatPanel] ⚠\ufe0f 检测到文本形式的工具调用描述（模式: " + text + "）");
				return true;
			}
		}
		return false;
	}

	[RelayCommand]
	private async Task ClearChatAsync()
	{
		if (!_dialogService.ShowConfirm("确定要清空所有对话记录吗？", "确认清空"))
		{
			return;
		}
		await SaveCurrentSessionAsync("用户清空对话");
		if (_currentSession != null && _dataCache != null)
		{
			try
			{
				_dataCache.ClearSession(_currentSession.SessionId);
				Logger.Info("[AIChatPanel] ✅ 已清空会话缓存: " + _currentSession.SessionId);
			}
			catch (Exception ex)
			{
				Logger.Warning("[AIChatPanel] ⚠\ufe0f 清空会话缓存失败: " + ex.Message);
			}
		}
		Messages.Clear();
		_currentSession = CreateNewSession();
		Messages.Add(new ChatMessage
		{
			Content = "对话已清空。有什么新的问题吗？",
			IsUser = false,
			Timestamp = DateTime.Now
		});
	}

	[RelayCommand]
	private async Task NewSessionAsync()
	{
		if (Messages.Count > 1)
		{
			await SaveCurrentSessionAsync("新建会话前自动保存");
		}
		if (_currentSession != null && _dataCache != null)
		{
			try
			{
				_dataCache.ClearSession(_currentSession.SessionId);
				Logger.Info("[AIChatPanel] ✅ 已清空会话缓存: " + _currentSession.SessionId);
			}
			catch (Exception ex)
			{
				Logger.Warning("[AIChatPanel] ⚠\ufe0f 清空会话缓存失败: " + ex.Message);
			}
		}
		Messages.Clear();
		_currentSession = CreateNewSession();
		Messages.Add(new ChatMessage
		{
			Content = "\ud83c\udf89 新会话已创建！有什么可以帮您的吗？",
			IsUser = false,
			Timestamp = DateTime.Now
		});
		Logger.Info("[AIChatPanel] ✅ 新会话已创建（自动保存）");
	}

	[RelayCommand]
	private void ManageSessions()
	{
		if (_sessionManager == null)
		{
			_dialogService.ShowInfo("会话管理未初始化");
			return;
		}
		try
		{
			SessionManagerWindow sessionManagerWindow = new SessionManagerWindow();
			SessionManagerViewModel viewModel = new SessionManagerViewModel(_sessionManager);
			sessionManagerWindow.SetViewModel(viewModel);
			sessionManagerWindow.Show();
			Logger.Info("[AIChatPanel] ✅ 打开会话管理窗口");
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] 打开会话管理窗口失败", ex);
			_dialogService.ShowInfo("打开会话管理失败: " + ex.Message);
		}
	}

	[RelayCommand]
	private void ClosePanel()
	{
		_windowManager.CloseAIChatPanelWindow();
	}

	[RelayCommand]
	private void ExportChat()
	{
		_dialogService.ShowInfo("导出对话功能即将推出", "功能开发中");
	}

	[RelayCommand]
	private void ToggleTopMost()
	{
		IsTopMost = !IsTopMost;
		_windowManager.SetAIChatTopMost(IsTopMost);
	}

	[RelayCommand]
	public void SwitchToDockable()
	{
		Logger.Info("[AIChatPanelViewModel] \ud83d\udcce 切换到停靠窗口命令被调用");
		_windowManager.SwitchToDockableMode();
	}

	[RelayCommand]
	public void SwitchToWindow()
	{
		Logger.Info("[AIChatPanelViewModel] \ud83e\ude9f 切换到独立窗口命令被调用");
		_windowManager.SwitchToWindowMode();
	}

	[RelayCommand]
	private async Task ReloadAIConfigsAsync()
	{
		await LoadAIConfigsAsync();
	}

	[RelayCommand]
	private void ManageAIProviders()
	{
		try
		{
			AIProviderManagementWindow aIProviderManagementWindow = new AIProviderManagementWindow();
			aIProviderManagementWindow.Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault((Window w) => w.IsActive);
			aIProviderManagementWindow.ShowDialog();
			Task.Run(() => LoadAIConfigsAsync());
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] 打开 AI 提供商管理窗口失败: " + ex.Message);
		}
	}

	private async Task SaveCurrentSessionAsync(string statusMessage)
	{
		if (_sessionManager == null || _currentSession == null)
		{
			return;
		}
		try
		{
			_currentSession.Messages = Messages.Select((ChatMessage m) => new SessionMessage
			{
				Role = ((m.Kind == MessageKind.User) ? "user" : "assistant"),
				Content = ((m.Kind == MessageKind.User) ? m.Content : ExtractContentOnly(m.Content, SelectedAIConfig)),
				Kind = m.Kind.ToString(),
				Timestamp = m.Timestamp,
				ToolCallId = m.ToolCall?.CallId
			}).ToList();
			_currentSession.AIConfig = SelectedAIConfig;
			await _sessionManager.SaveSessionAsync(_currentSession);
			Logger.Info("[AIChatPanel] \ud83d\udcbe 会话已保存: " + _currentSession.Title);
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] 保存会话失败: " + ex.Message);
		}
	}

	[RelayCommand]
	private async Task SaveSession()
	{
		if (_sessionManager == null || _currentSession == null)
		{
			_dialogService.ShowInfo("会话管理未初始化");
			return;
		}
		await SaveCurrentSessionAsync("用户手动保存");
		_dialogService.ShowInfo("会话已保存");
	}

	[RelayCommand]
	private async Task RestoreSession()
	{
		if (_sessionManager == null)
		{
			_dialogService.ShowInfo("会话管理未初始化");
			return;
		}
		List<SessionIndexItem> list = await _sessionManager.GetSessionListAsync();
		if (list.Count == 0)
		{
			_dialogService.ShowInfo("没有可恢复的会话");
			return;
		}
		AIChatSession session = await _sessionManager.LoadSessionAsync(list[0].SessionId);
		if (session == null)
		{
			return;
		}
		_currentSession = session;
		await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
		{
			Messages.Clear();
			foreach (SessionMessage message in session.Messages)
			{
				object obj = Enum.Parse(typeof(MessageKind), message.Kind);
				Messages.Add(new ChatMessage
				{
					Content = message.Content,
					Kind = (MessageKind)obj,
					Timestamp = message.Timestamp
				});
			}
		});
		Logger.Info("[AIChatPanel] \ud83d\udcbe 已恢复会话: " + session.Title);
		Logger.Info("[AIChatPanel] \ud83d\udce6 工具定义请求状态: " + (session.HasRequestedTools ? "已请求（自动包含）" : "未请求（等待AI请求）"));
		_dialogService.ShowInfo("已恢复会话: " + session.Title);
	}

	private async Task<(string systemPrompt, string? toolsDefinition, bool toolsProvided)> HandleSpecialCommandAsync(string command, string? argument, List<object> conversationHistory, AISystemPromptManager systemPromptManager, string systemPrompt, string? toolsDefinition, bool toolsProvided)
	{
		switch (command.ToUpper())
		{
		case "REQUEST_TOOLS":
			Logger.Warning("[AIChatPanel] 收到 REQUEST_TOOLS 指令，但工具定义已自动提供，忽略此指令");
			return (systemPrompt: systemPrompt, toolsDefinition: toolsDefinition, toolsProvided: toolsProvided);
		case "REQUEST_SKILL_LIST":
			await HandleRequestSkillListAsync(conversationHistory);
			return (systemPrompt: systemPrompt, toolsDefinition: toolsDefinition, toolsProvided: toolsProvided);
		case "REQUEST_SKILL_DETAIL":
			await HandleRequestSkillDetailAsync(argument, conversationHistory);
			return (systemPrompt: systemPrompt, toolsDefinition: toolsDefinition, toolsProvided: toolsProvided);
		default:
			Logger.Warning("[AIChatPanel] 未知特殊指令: " + command);
			return (systemPrompt: systemPrompt, toolsDefinition: toolsDefinition, toolsProvided: toolsProvided);
		}
	}

	private Task<(string systemPrompt, string? toolsDefinition, bool toolsProvided)> HandleRequestToolsAsync(List<object> conversationHistory, AISystemPromptManager systemPromptManager, string systemPrompt, string? toolsDefinition, bool toolsProvided)
	{
		try
		{
			string toolsDefinitionForAI = AIToolRegistry.Instance.GetToolsDefinitionForAI();
			object? obj = JsonConvert.DeserializeObject<object>(toolsDefinitionForAI);
			int value = 0;
			if (obj is JArray jArray)
			{
				value = jArray.Count;
			}
			toolsDefinition = toolsDefinitionForAI;
			toolsProvided = true;
			if (_currentSession != null)
			{
				_currentSession.HasRequestedTools = true;
				Logger.Info("[AIChatPanel] ✅ 已标记会话 " + _currentSession.SessionId + " 为已请求工具定义状态");
			}
			systemPrompt = systemPromptManager.GenerateSystemPromptWithTools();
			conversationHistory[0] = new
			{
				role = "system",
				content = systemPrompt
			};
			conversationHistory.Add(new
			{
				role = "assistant",
				content = "REQUEST_TOOLS"
			});
			conversationHistory.Add(new
			{
				role = "system",
				content = $"已提供所有 {value} 个工具的完整定义（包含名称、描述和参数）。您可以直接调用这些工具。"
			});
			Logger.Info($"[AIChatPanel] \ud83d\udce6 已提供 {value} 个工具的完整定义，后续对话将自动包含工具定义");
			return Task.FromResult((systemPrompt, toolsDefinition, toolsProvided));
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] 处理 REQUEST_TOOLS 失败: " + ex.Message, ex);
			conversationHistory.Add(new
			{
				role = "system",
				content = "获取工具列表失败：" + ex.Message
			});
			return Task.FromResult((systemPrompt, toolsDefinition, toolsProvided));
		}
	}

	private async Task HandleRequestSkillListAsync(List<object> conversationHistory)
	{
		try
		{
			ISkillManager requiredService = UIBootstrapper.Services.GetRequiredService<ISkillManager>();
			if (requiredService == null)
			{
				Logger.Warning("[AIChatPanel] SkillManager 未注册");
				conversationHistory.Add(new
				{
					role = "system",
					content = "技能系统不可用。"
				});
				return;
			}
			Result<List<SkillInfo>> skillList = requiredService.GetSkillList();
			if (skillList.IsSuccess && skillList.Value != null)
			{
				List<SkillInfo> value = skillList.Value;
				string value2 = string.Join("\n", value.Select((SkillInfo s) => $"- {s.SkillName}: {s.DisplayName} - {s.Description}（分类：{s.Category}）"));
				conversationHistory.Add(new
				{
					role = "assistant",
					content = "REQUEST_SKILL_LIST"
				});
				conversationHistory.Add(new
				{
					role = "system",
					content = $"可用技能列表（共 {value.Count} 个）：\n{value2}\n\n使用 execute_skill 工具执行技能。"
				});
			}
			else
			{
				conversationHistory.Add(new
				{
					role = "system",
					content = "获取技能列表失败：" + skillList.Error
				});
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] 获取技能列表失败: " + ex.Message, ex);
			conversationHistory.Add(new
			{
				role = "system",
				content = "获取技能列表失败：" + ex.Message
			});
		}
		await Task.CompletedTask;
	}

	private async Task HandleRequestSkillDetailAsync(string? skillName, List<object> conversationHistory)
	{
		if (string.IsNullOrEmpty(skillName))
		{
			conversationHistory.Add(new
			{
				role = "system",
				content = "请提供技能名称。"
			});
			await Task.CompletedTask;
			return;
		}
		try
		{
			ISkillManager requiredService = UIBootstrapper.Services.GetRequiredService<ISkillManager>();
			if (requiredService == null)
			{
				conversationHistory.Add(new
				{
					role = "system",
					content = "技能系统不可用。"
				});
				await Task.CompletedTask;
				return;
			}
			Result<SkillInfo> skillDetail = requiredService.GetSkillDetail(skillName);
			if (skillDetail.IsSuccess && skillDetail.Value != null)
			{
				SkillInfo value = skillDetail.Value;
				string value2 = string.Join("\n", value.Steps.Select((SkillStep s) => $"  {s.Order}. {s.Tool}: {s.Description}"));
				conversationHistory.Add(new
				{
					role = "assistant",
					content = "REQUEST_SKILL_DETAIL " + skillName
				});
				conversationHistory.Add(new
				{
					role = "system",
					content = $"技能详情：{value.DisplayName}\n描述：{value.Description}\n步骤：\n{value2}\n\n使用 execute_skill 工具执行此技能。"
				});
			}
			else
			{
				conversationHistory.Add(new
				{
					role = "system",
					content = "技能不存在：" + skillName
				});
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] 获取技能详情失败: " + ex.Message, ex);
			conversationHistory.Add(new
			{
				role = "system",
				content = "获取技能详情失败：" + ex.Message
			});
		}
		await Task.CompletedTask;
	}

	[RelayCommand]
	private async Task AttachFileByPathAsync(string filePath)
	{
		_ = 2;
		try
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				return;
			}
			if (!File.Exists(filePath))
			{
				Logger.Warning("[AIChatPanel] 附件文件不存在: " + filePath);
				return;
			}
			AIConfig? selectedAIConfig = SelectedAIConfig;
			if (selectedAIConfig == null || !selectedAIConfig.SupportsVision)
			{
				Logger.Warning("[AIChatPanel] 当前模型 " + SelectedAIConfig?.ModelName + " 不支持视觉能力");
				await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
				{
					Messages.Add(new ChatMessage
					{
						Content = "⚠\ufe0f 当前模型 " + SelectedAIConfig?.ModelName + " 不支持视觉能力。\n\n请切换到支持图片的模型（如 DeepSeek-Vision、GPT-4V 等）后再添加附件。",
						IsUser = false,
						Timestamp = DateTime.Now
					});
				});
				return;
			}
			string extension = Path.GetExtension(filePath);
			FileAttachmentType fileTypeFromExtension = FileAttachment.GetFileTypeFromExtension(extension);
			string mimeType = FileAttachment.GetMimeType(extension);
			if (fileTypeFromExtension == FileAttachmentType.Unknown)
			{
				Logger.Warning("[AIChatPanel] 不支持的文件类型: " + extension);
				await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
				{
					Messages.Add(new ChatMessage
					{
						Content = "⚠\ufe0f 不支持的文件类型: " + extension + "\n\n支持的格式：图片（JPEG、PNG、GIF、WebP）、PDF、文本文件等",
						IsUser = false,
						Timestamp = DateTime.Now
					});
				});
				return;
			}
			FileInfo fileInfo = new FileInfo(filePath);
			FileAttachment attachment = new FileAttachment
			{
				FileName = Path.GetFileName(filePath),
				FileExtension = extension,
				FileType = fileTypeFromExtension,
				FileSize = fileInfo.Length,
				FilePath = filePath,
				MimeType = mimeType,
				UploadedAt = DateTime.Now
			};
			await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
			{
				Attachments.Add(attachment);
				Logger.Info($"[AIChatPanel] 已添加附件: {attachment.FileName} ({attachment.FileSizeDisplay})");
			});
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatPanel] 添加附件失败: " + ex.Message, ex);
		}
	}

	private async Task RegisterFamilyLibraryLoadToolAsync()
	{
		try
		{
			await Task.Delay(200);
			IServiceProvider services = UIBootstrapper.Services;
			if (services == null)
			{
				Logger.Warning("[FamilyLibraryAI] ServiceProvider 未初始化，无法注册在线族库 AI 工具");
				return;
			}
			AIToolRegistry instance = AIToolRegistry.Instance;
			IFamilyLibraryService service = services.GetService<IFamilyLibraryService>();
			if (service == null)
			{
				Logger.Warning("[FamilyLibraryAI] 族库服务未初始化，无法注册在线族库 AI 工具");
				return;
			}
			if (!instance.ContainsTool("search_family_library"))
			{
				FamilyLibraryAITools.RegisterSearchTool(instance, service);
			}
			if (instance.ContainsTool("load_family_from_library"))
			{
				return;
			}
			IFamilyLoadService familyLoadService = null;
			try
			{
				Type type = Type.GetType("RevitAi.Revit.UI.RevitAdapterManager, RevitAi.Revit");
				if (type != null)
				{
					object obj = type.GetProperty("CurrentAdapter")?.GetValue(null);
					if (obj != null)
					{
						familyLoadService = obj.GetType().GetMethod("GetFamilyLoadService")?.Invoke(obj, null) as IFamilyLoadService;
					}
				}
			}
			catch
			{
			}
			if (familyLoadService != null)
			{
				FamilyLibraryAITools.RegisterLoadTool(instance, service, familyLoadService);
			}
			else
			{
				Logger.Warning("[FamilyLibraryAI] 族加载服务未初始化，跳过在线族库加载工具注册（需要 Revit 上下文）");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[FamilyLibraryAI] 注册在线族库 AI 工具失败: " + ex.Message, ex);
		}
	}

	private async Task RegisterFeedbackAIToolsAsync()
	{
		try
		{
			await Task.Delay(300);
			IServiceProvider services = UIBootstrapper.Services;
			if (services == null)
			{
				Logger.Warning("[FeedbackAI] ServiceProvider 未初始化，无法注册反馈 AI 工具");
				return;
			}
			FeedbackService service = services.GetService<FeedbackService>();
			if (service == null)
			{
				Logger.Warning("[FeedbackAI] 反馈服务未初始化，无法注册反馈 AI 工具");
				return;
			}
			AIToolRegistry instance = AIToolRegistry.Instance;
			if (!instance.ContainsTool("submit_developer_issue"))
			{
				FeedbackAITools.RegisterTools(instance, service);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[FeedbackAI] 注册反馈 AI 工具失败: " + ex.Message, ex);
		}
	}

	private async Task RegisterWebSearchAIToolsAsync()
	{
		try
		{
			await Task.Delay(500);
			IServiceProvider services = UIBootstrapper.Services;
			if (services == null)
			{
				Logger.Warning("[WebSearchAI] ServiceProvider 未初始化，无法注册互联网搜索 AI 工具");
				return;
			}
			IApiKeyService service = services.GetService<IApiKeyService>();
			if (service == null)
			{
				Logger.Warning("[WebSearchAI] ApiKeyService 未初始化，无法注册互联网搜索 AI 工具");
				return;
			}
			IHttpClientFactory httpClientFactory = CoreServicesFactory.CreateHttpClientFactory();
			AIToolRegistry instance = AIToolRegistry.Instance;
			if (!instance.ContainsTool("web_search"))
			{
				WebSearchTool tool = new WebSearchTool(service, httpClientFactory);
				instance.RegisterTool(tool);
			}
			if (!instance.ContainsTool("web_fetch"))
			{
				WebFetchTool tool2 = new WebFetchTool(httpClientFactory);
				instance.RegisterTool(tool2);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[WebSearchAI] 注册互联网搜索 AI 工具失败: " + ex.Message, ex);
		}
	}

	private async Task RegisterMemoryAIToolAsync()
	{
		try
		{
			await Task.Delay(100);
			AIToolRegistry instance = AIToolRegistry.Instance;
			if (!instance.ContainsTool("save_memory"))
			{
				instance.RegisterTool(new SaveMemoryAITool());
				Logger.Info("[MemoryAI] ✅ 会话记忆工具 save_memory 已注册");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[MemoryAI] 注册会话记忆 AI 工具失败: " + ex.Message, ex);
		}
	}

	private async Task<string> GenerateSessionSummaryAsync(AIChatSession session, int messageCount)
	{
		if (session?.Messages == null || session.Messages.Count <= messageCount)
		{
			return string.Empty;
		}
		try
		{
			List<SessionMessage> list = (from m in session.Messages.Take(Math.Max(0, session.Messages.Count - messageCount))
				where m.Role == "user" || m.Role == "assistant"
				select m).ToList();
			if (!list.Any())
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(14, 1, stringBuilder2);
			handler.AppendLiteral("本次会话共进行了 ");
			handler.AppendFormatted(list.Count);
			handler.AppendLiteral(" 条对话。");
			stringBuilder3.AppendLine(ref handler);
			List<string> list2 = (from m in list
				where m.Role == "user"
				select m.Content into c
				where !string.IsNullOrEmpty(c)
				select c).ToList();
			List<string> list3 = list.Where((SessionMessage m) => m.ToolCalls != null && m.ToolCalls.Any()).SelectMany((SessionMessage m) => m.ToolCalls.Select((ToolCallData tc) => tc.ToolName)).Distinct()
				.ToList();
			if (list2.Any())
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("**用户查询**：");
				foreach (string item in list2.Take(5))
				{
					string value = ((item.Length > 60) ? (item.Substring(0, 60) + "...") : item);
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder4 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
					handler.AppendLiteral("- ");
					handler.AppendFormatted(value);
					stringBuilder4.AppendLine(ref handler);
				}
				if (list2.Count > 5)
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder5 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(13, 1, stringBuilder2);
					handler.AppendLiteral("- ... 还有 ");
					handler.AppendFormatted(list2.Count - 5);
					handler.AppendLiteral(" 条查询");
					stringBuilder5.AppendLine(ref handler);
				}
			}
			if (list3.Any())
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("**使用的工具**：");
				foreach (string item2 in list3)
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder6 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
					handler.AppendLiteral("- ");
					handler.AppendFormatted(item2);
					stringBuilder6.AppendLine(ref handler);
				}
			}
			DateTime timestamp = list.First().Timestamp;
			DateTime timestamp2 = list.Last().Timestamp;
			stringBuilder.AppendLine();
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder7 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(12, 2, stringBuilder2);
			handler.AppendLiteral("**时间范围**：");
			handler.AppendFormatted(timestamp, "HH:mm");
			handler.AppendLiteral(" - ");
			handler.AppendFormatted(timestamp2, "HH:mm");
			stringBuilder7.AppendLine(ref handler);
			string text = stringBuilder.ToString();
			Logger.Info($"[AIChatPanel] \ud83d\udcdd 已生成会话摘要 ({text.Length} 字符)");
			return text;
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIChatPanel] ⚠\ufe0f 生成会话摘要失败: " + ex.Message);
			return "（会话摘要生成失败：" + ex.Message + "）";
		}
	}

	private async Task<string> GenerateSessionSummaryForRangeAsync(AIChatSession session, int startIndex, int count)
	{
		if (session?.Messages == null || session.Messages.Count <= startIndex)
		{
			return string.Empty;
		}
		try
		{
			List<SessionMessage> list = (from m in session.Messages.Skip(startIndex).Take(count)
				where m.Role == "user" || m.Role == "assistant"
				select m).ToList();
			if (!list.Any())
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder2);
			handler.AppendLiteral("中间部分共有 ");
			handler.AppendFormatted(list.Count);
			handler.AppendLiteral(" 条对话。");
			stringBuilder3.AppendLine(ref handler);
			List<string> list2 = (from m in list
				where m.Role == "user"
				select m.Content into c
				where !string.IsNullOrEmpty(c)
				select c).ToList();
			List<string> list3 = list.Where((SessionMessage m) => m.ToolCalls != null && m.ToolCalls.Any()).SelectMany((SessionMessage m) => m.ToolCalls.Select((ToolCallData tc) => tc.ToolName)).Distinct()
				.ToList();
			if (list2.Any())
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("**用户查询**：");
				foreach (string item in list2.Take(5))
				{
					string value = ((item.Length > 60) ? (item.Substring(0, 60) + "...") : item);
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder4 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
					handler.AppendLiteral("- ");
					handler.AppendFormatted(value);
					stringBuilder4.AppendLine(ref handler);
				}
				if (list2.Count > 5)
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder5 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(13, 1, stringBuilder2);
					handler.AppendLiteral("- ... 还有 ");
					handler.AppendFormatted(list2.Count - 5);
					handler.AppendLiteral(" 条查询");
					stringBuilder5.AppendLine(ref handler);
				}
			}
			if (list3.Any())
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("**使用的工具**：");
				foreach (string item2 in list3)
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder6 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
					handler.AppendLiteral("- ");
					handler.AppendFormatted(item2);
					stringBuilder6.AppendLine(ref handler);
				}
			}
			DateTime timestamp = list.First().Timestamp;
			DateTime timestamp2 = list.Last().Timestamp;
			stringBuilder.AppendLine();
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder7 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(12, 2, stringBuilder2);
			handler.AppendLiteral("**时间范围**：");
			handler.AppendFormatted(timestamp, "HH:mm");
			handler.AppendLiteral(" - ");
			handler.AppendFormatted(timestamp2, "HH:mm");
			stringBuilder7.AppendLine(ref handler);
			string text = stringBuilder.ToString();
			Logger.Info($"[AIChatPanel] \ud83d\udcdd 已生成范围摘要 [{startIndex}-{startIndex + count}] ({text.Length} 字符)");
			return text;
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIChatPanel] ⚠\ufe0f 生成范围摘要失败: " + ex.Message);
			return "（范围摘要生成失败：" + ex.Message + "）";
		}
	}

	private void RemoveReasoningContentFromJson(JToken token)
	{
		if (token is JObject jObject)
		{
			if (jObject["reasoning_content"] != null)
			{
				jObject.Remove("reasoning_content");
			}
			if (jObject["thinking"] != null)
			{
				jObject.Remove("thinking");
			}
			{
				foreach (JProperty item in jObject.Properties().ToList())
				{
					RemoveReasoningContentFromJson(item.Value);
				}
				return;
			}
		}
		if (!(token is JArray jArray))
		{
			return;
		}
		foreach (JToken item2 in jArray)
		{
			RemoveReasoningContentFromJson(item2);
		}
	}

	[RelayCommand]
	private async Task SaveCodeSnippetAsync(string? code)
	{
		if (string.IsNullOrWhiteSpace(code))
		{
			_dialogService.ShowWarning("没有可保存的代码");
			return;
		}
		try
		{
			string message = "请分析以下代码，为其生成一个合适的名称（包含 Emoji）、简短描述和标签。请以 JSON 格式返回：\n\n{\"name\": \"\ud83c\udfaf 名称\", \"description\": \"描述\", \"tags\": [\"标签1\", \"标签2\"]}\n\n代码内容：\n" + code;
			string text = ((_aiService == null) ? null : (await _aiService.SendMessageAsync(message)));
			string text2 = text;
			string name = "\ud83d\udcdd 未命名代码";
			string description = "用户保存的代码片段";
			List<string> tags = new List<string> { "自定义" };
			try
			{
				if (!string.IsNullOrEmpty(text2))
				{
					int num = text2.IndexOf('{');
					int num2 = text2.LastIndexOf('}');
					if (num >= 0 && num2 > num)
					{
						string value = text2.Substring(num, num2 - num + 1);
						if (!string.IsNullOrEmpty(value))
						{
							dynamic val = JsonConvert.DeserializeObject<object>(value);
							if (val != null)
							{
								name = val.name ?? name;
								description = val.description ?? description;
								tags = val.tags?.ToObject<List<string>>() ?? tags;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[AIChatPanel] 解析 AI 分析结果失败: " + ex.Message);
			}
			CodeSnippet snippet = new CodeSnippet
			{
				Name = name,
				Description = description,
				Code = (code ?? string.Empty),
				Tags = tags
			};
			new CodeSnippetStorageService().AddSnippet(snippet);
			if (CodeSnippetLibraryViewModel != null)
			{
				await CodeSnippetLibraryViewModel.LoadSnippetsAsync();
			}
			_dialogService.ShowInfo($"代码片段已保存：\n\n名称：{name}\n描述：{description}\n标签：{string.Join(", ", tags)}");
		}
		catch (Exception ex2)
		{
			Logger.Error("[AIChatPanel] 保存代码片段失败: " + ex2.Message, ex2);
			_dialogService.ShowError("保存代码片段失败: " + ex2.Message);
		}
	}

	private List<string> ExtractRelatedFilesFromToolCall(string toolName, Dictionary<string, object?>? parameters)
	{
		List<string> list = new List<string>();
		try
		{
			if (parameters == null)
			{
				return list;
			}
			if (toolName != null)
			{
				int length = toolName.Length;
				if (length <= 8)
				{
					if (length != 4)
					{
						if (length == 8)
						{
							char c = toolName[0];
							if (c != 'E')
							{
								if (c == 'R' && toolName == "ReadFile")
								{
									goto IL_010f;
								}
							}
							else if (toolName == "EditFile")
							{
								goto IL_010f;
							}
						}
					}
					else
					{
						char c = toolName[1];
						if (c != 'l')
						{
							if (c == 'r' && toolName == "Grep")
							{
								goto IL_0141;
							}
						}
						else if (toolName == "Glob")
						{
							goto IL_0141;
						}
					}
				}
				else if (length != 9)
				{
					if (length == 13)
					{
						switch (toolName[0])
						{
						case 'C':
							if (!(toolName == "CreateElement"))
							{
								break;
							}
							goto end_IL_0006;
						case 'M':
							if (!(toolName == "ModifyElement"))
							{
								break;
							}
							goto end_IL_0006;
						}
					}
				}
				else if (toolName == "WriteFile")
				{
					goto IL_010f;
				}
			}
			foreach (KeyValuePair<string, object> parameter in parameters)
			{
				if (parameter.Key.IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0 || parameter.Key.IndexOf("file", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					string text = parameter.Value?.ToString();
					if (!string.IsNullOrEmpty(text) && (text.Contains("/") || text.Contains("\\")))
					{
						list.Add(text);
					}
				}
			}
			goto end_IL_0006;
			IL_0141:
			if (parameters.TryGetValue("path", out object value))
			{
				list.Add(value?.ToString() ?? string.Empty);
			}
			goto end_IL_0006;
			IL_010f:
			if (parameters.TryGetValue("filePath", out object value2))
			{
				list.Add(value2?.ToString() ?? string.Empty);
			}
			end_IL_0006:;
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIChatPanel] ⚠\ufe0f 提取相关文件失败: " + ex.Message);
		}
		return list.Where((string f) => !string.IsNullOrEmpty(f)).Distinct().ToList();
	}

	private List<string> ExtractRelatedFilesFromResult(string toolResult)
	{
		List<string> list = new List<string>();
		try
		{
			foreach (Match item in Regex.Matches(toolResult, "AS\\.Tools[\\\\/][\\w\\\\\\/.-]+\\.(?:cs|xaml|csproj|json)"))
			{
				list.Add(item.Value);
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIChatPanel] ⚠\ufe0f 从结果提取文件失败: " + ex.Message);
		}
		return list.Distinct().ToList();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnUserInputChanged(string value)
	{
		OnPropertyChanged("CanSendMessage");
		if (!IsProcessing)
		{
			OnPropertyChanged("PrimaryButtonCommand");
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnIsProcessingChanged(bool value)
	{
		OnPropertyChanged("CanSendMessage");
		OnPropertyChanged("StatusColor");
		OnPropertyChanged("StatusText");
		OnPropertyChanged("PrimaryButtonCommand");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedAIConfigChanged(AIConfig? value)
	{
		UpdateAttachFileButtonVisibility();
	}
}
