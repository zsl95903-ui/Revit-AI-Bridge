using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Core.AI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class SessionManagerViewModel : ObservableObject
{
	private readonly AIChatSessionManager _sessionManager;

	private SessionIndexItem? _selectedSession;

	private string? _previewContent;

	private int _selectedDaysIndex = 2;

	private bool _isLoading;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? initializeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? refreshCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? deleteSelectedCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? cleanupOldSessionsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closeCommand;

	public ObservableCollection<SessionIndexItem> Sessions { get; } = new ObservableCollection<SessionIndexItem>();

	public SessionIndexItem? SelectedSession
	{
		get
		{
			return _selectedSession;
		}
		set
		{
			if (SetProperty(ref _selectedSession, value, "SelectedSession"))
			{
				LoadPreviewAsync(value);
			}
		}
	}

	public string? PreviewContent
	{
		get
		{
			return _previewContent;
		}
		set
		{
			SetProperty(ref _previewContent, value, "PreviewContent");
		}
	}

	public ObservableCollection<string> CleanupDayOptions { get; } = new ObservableCollection<string>(new string[5] { "1 天", "3 天", "7 天", "15 天", "30 天" });

	public int SelectedDaysIndex
	{
		get
		{
			return _selectedDaysIndex;
		}
		set
		{
			SetProperty(ref _selectedDaysIndex, value, "SelectedDaysIndex");
		}
	}

	public bool IsLoading
	{
		get
		{
			return _isLoading;
		}
		set
		{
			SetProperty(ref _isLoading, value, "IsLoading");
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand InitializeCommand => initializeCommand ?? (initializeCommand = new AsyncRelayCommand(InitializeAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand RefreshCommand => refreshCommand ?? (refreshCommand = new AsyncRelayCommand(RefreshAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand DeleteSelectedCommand => deleteSelectedCommand ?? (deleteSelectedCommand = new AsyncRelayCommand(DeleteSelectedAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand CleanupOldSessionsCommand => cleanupOldSessionsCommand ?? (cleanupOldSessionsCommand = new AsyncRelayCommand(CleanupOldSessionsAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(Close));

	public SessionManagerViewModel(AIChatSessionManager sessionManager)
	{
		_sessionManager = sessionManager ?? throw new ArgumentNullException("sessionManager");
	}

	[RelayCommand]
	public async Task InitializeAsync()
	{
		await LoadSessionsAsync();
	}

	[RelayCommand]
	public async Task RefreshAsync()
	{
		await LoadSessionsAsync();
	}

	[RelayCommand]
	private async Task DeleteSelectedAsync()
	{
		List<SessionIndexItem> selectedSessions = Sessions.Where((SessionIndexItem s) => s.IsSelected).ToList();
		if (selectedSessions.Count == 0)
		{
			Logger.Warning("[SessionManager] 未选择任何会话");
			return;
		}
		try
		{
			IsLoading = true;
			foreach (SessionIndexItem session in selectedSessions)
			{
				await _sessionManager.DeleteSessionAsync(session.SessionId);
				Sessions.Remove(session);
			}
			Logger.Info($"[SessionManager] 已删除 {selectedSessions.Count} 个会话");
			if (PreviewContent != null)
			{
				PreviewContent = null;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[SessionManager] 删除会话失败: " + ex.Message, ex);
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	private async Task CleanupOldSessionsAsync()
	{
		int[] array = new int[5] { 1, 3, 7, 15, 30 };
		int days = array[SelectedDaysIndex];
		DateTime cutoffDate = DateTime.Now.AddDays(-days);
		try
		{
			IsLoading = true;
			List<SessionIndexItem> toDelete = Sessions.Where((SessionIndexItem s) => s.UpdatedAt < cutoffDate).ToList();
			if (toDelete.Count == 0)
			{
				Logger.Info($"[SessionManager] 没有超过 {days} 天的会话");
				return;
			}
			foreach (SessionIndexItem session in toDelete)
			{
				await _sessionManager.DeleteSessionAsync(session.SessionId);
				Sessions.Remove(session);
			}
			Logger.Info($"[SessionManager] 已清理 {toDelete.Count} 个超过 {days} 天的会话");
		}
		catch (Exception ex)
		{
			Logger.Error("[SessionManager] 清理会话失败: " + ex.Message, ex);
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	public void Close()
	{
	}

	private async Task LoadSessionsAsync()
	{
		try
		{
			IsLoading = true;
			Sessions.Clear();
			foreach (SessionIndexItem item in await _sessionManager.GetSessionListAsync())
			{
				Sessions.Add(item);
			}
			Logger.Info($"[SessionManager] 加载了 {Sessions.Count} 个会话");
		}
		catch (Exception ex)
		{
			Logger.Error("[SessionManager] 加载会话列表失败: " + ex.Message, ex);
		}
		finally
		{
			IsLoading = false;
		}
	}

	private async Task LoadPreviewAsync(SessionIndexItem? session)
	{
		if (session == null)
		{
			PreviewContent = null;
			return;
		}
		try
		{
			AIChatSession aIChatSession = await _sessionManager.LoadSessionAsync(session.SessionId);
			if (aIChatSession == null)
			{
				PreviewContent = "无法加载会话内容";
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(4, 1, stringBuilder2);
			handler.AppendLiteral("标题: ");
			handler.AppendFormatted(aIChatSession.Title);
			stringBuilder3.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("创建时间: ");
			handler.AppendFormatted(aIChatSession.CreatedAt, "yyyy-MM-dd HH:mm:ss");
			stringBuilder4.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder5 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("更新时间: ");
			handler.AppendFormatted(aIChatSession.UpdatedAt, "yyyy-MM-dd HH:mm:ss");
			stringBuilder5.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder6 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("对话轮次: ");
			handler.AppendFormatted(aIChatSession.RoundCount);
			stringBuilder6.AppendLine(ref handler);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("对话内容:");
			stringBuilder.AppendLine(new string('═', 60));
			stringBuilder.AppendLine();
			foreach (SessionMessage message in aIChatSession.Messages)
			{
				if (message.Role == "user")
				{
					stringBuilder.AppendLine("\ud83d\udd35 【用户】");
					stringBuilder.AppendLine(message.Content ?? string.Empty);
					stringBuilder.AppendLine();
				}
				else if (message.Role == "assistant")
				{
					if (!string.IsNullOrEmpty(message.ReasoningContent))
					{
						stringBuilder.AppendLine("\ud83e\udd14 【思考】");
						stringBuilder.AppendLine(message.ReasoningContent);
						stringBuilder.AppendLine();
					}
					if (message.ToolCalls != null && message.ToolCalls.Count > 0)
					{
						foreach (ToolCallData toolCall in message.ToolCalls)
						{
							stringBuilder.AppendLine("\ud83d\udd27 【工具调用】");
							stringBuilder2 = stringBuilder;
							StringBuilder stringBuilder7 = stringBuilder2;
							handler = new StringBuilder.AppendInterpolatedStringHandler(4, 1, stringBuilder2);
							handler.AppendLiteral("函数: ");
							handler.AppendFormatted(toolCall.ToolName);
							stringBuilder7.AppendLine(ref handler);
							string value = JsonSerializer.Serialize(toolCall.Parameters);
							stringBuilder2 = stringBuilder;
							StringBuilder stringBuilder8 = stringBuilder2;
							handler = new StringBuilder.AppendInterpolatedStringHandler(4, 1, stringBuilder2);
							handler.AppendLiteral("参数: ");
							handler.AppendFormatted(value);
							stringBuilder8.AppendLine(ref handler);
							stringBuilder.AppendLine();
						}
					}
					if (!string.IsNullOrEmpty(message.Content))
					{
						stringBuilder.AppendLine("⚪ 【助手】");
						stringBuilder.AppendLine(message.Content);
						stringBuilder.AppendLine();
					}
				}
				else if (message.Role == "tool")
				{
					if ((message.Content?.Contains("错误") ?? false) || (message.Content?.Contains("失败") ?? false) || (message.Content?.Contains("Error") ?? false))
					{
						stringBuilder.AppendLine("❌ 【工具失败】");
					}
					else
					{
						stringBuilder.AppendLine("✅ 【工具结果】");
					}
					stringBuilder.AppendLine(message.Content ?? string.Empty);
					stringBuilder.AppendLine();
				}
			}
			PreviewContent = stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			Logger.Error("[SessionManager] 加载预览失败: " + ex.Message, ex);
			PreviewContent = "加载预览失败: " + ex.Message;
		}
	}
}
