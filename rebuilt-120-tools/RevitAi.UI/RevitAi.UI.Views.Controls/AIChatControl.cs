using System;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using RevitAi.Abstractions.Logging;
using RevitAi.UI.Models;
using RevitAi.UI.Services;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Controls;

public partial class AIChatControl : UserControl, IComponentConnector
{
	public AIChatControl()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		InitializeComponent();
		base.AllowDrop = true;
		base.Drop += OnChatDrop;
		base.PreviewDragEnter += OnChatDragEnter;
		base.PreviewDragOver += OnChatDragOver;
		base.PreviewDragLeave += OnChatDragLeave;
		DataObject.AddPastingHandler((DependencyObject)(object)this, OnPaste);
		Logger.Info("[AIChatControl] AIChatControl 初始化完成，拖拽和粘贴功能已启用");
		base.DataContextChanged += (DependencyPropertyChangedEventHandler)delegate(object s, DependencyPropertyChangedEventArgs e)
		{
			if (((DependencyPropertyChangedEventArgs)e).NewValue != null && ((DependencyPropertyChangedEventArgs)e).NewValue is AIChatPanelViewModel aIChatPanelViewModel)
			{
				INotifyCollectionChanged messages = aIChatPanelViewModel.Messages;
				if (messages != null)
				{
					messages.CollectionChanged += delegate
					{
						((DispatcherObject)this).Dispatcher.InvokeAsync((Action)delegate
						{
							ChatScrollViewer.ScrollToBottom();
						}, (DispatcherPriority)4);
					};
				}
				ApplyFontSettings();
			}
		};
		ApplyFontSettings();
		base.Unloaded += async delegate
		{
			try
			{
				if (base.DataContext is AIChatPanelViewModel aIChatPanelViewModel)
				{
					MethodInfo method = aIChatPanelViewModel.GetType().GetMethod("EndMemorySessionAsync", BindingFlags.Instance | BindingFlags.NonPublic);
					if (method != null && method.Invoke(aIChatPanelViewModel, null) is Task task)
					{
						await task;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[AIChatControl] Unloaded 事件中保存记忆会话失败: " + ex.Message);
			}
		};
	}

	private void ApplyFontSettings()
	{
		try
		{
			IUserPreferencesService userPreferencesService = TryGetService<IUserPreferencesService>();
			if (userPreferencesService != null)
			{
				UserPreferences currentPreferences = userPreferencesService.CurrentPreferences;
				ApplyFontSettingsToVisualTree((DependencyObject)(object)this, currentPreferences);
			}
		}
		catch (Exception)
		{
		}
	}

	private void ApplyFontSettingsToVisualTree(DependencyObject parent, UserPreferences preferences)
	{
		if (parent == null)
		{
			return;
		}
		if (parent is MarkdownTextBlock markdownTextBlock)
		{
			markdownTextBlock.FontFamily = new FontFamily(preferences.ChatFontFamily);
			markdownTextBlock.FontSize = preferences.ChatFontSize;
		}
		int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
		for (int i = 0; i < childrenCount; i++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(parent, i);
			if (child != null)
			{
				ApplyFontSettingsToVisualTree(child, preferences);
			}
		}
	}

	private T? TryGetService<T>() where T : class
	{
		try
		{
			if (Application.Current != null)
			{
				PropertyInfo property = ((object)Application.Current).GetType().GetProperty("Services");
				if (property != null)
				{
					object value = property.GetValue(Application.Current);
					if (value != null)
					{
						MethodInfo method = value.GetType().GetMethod("GetService", new Type[1] { typeof(Type) });
						if (method != null)
						{
							return method.Invoke(value, new object[1] { typeof(T) }) as T;
						}
					}
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private void OnKeyDown(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		if ((int)e.Key != 6)
		{
			return;
		}
		bool num = (e.KeyboardDevice.Modifiers & (ModifierKeys)2) > 0;
		bool flag = (e.KeyboardDevice.Modifiers & (ModifierKeys)4) > 0;
		if (num | flag)
		{
			if (sender is TextBox { CaretIndex: var caretIndex, Text: var text } textBox)
			{
				textBox.Text = text.Insert(caretIndex, "\n");
				textBox.CaretIndex = caretIndex + 1;
				e.Handled = true;
			}
		}
		else
		{
			e.Handled = true;
			if (base.DataContext is AIChatPanelViewModel aIChatPanelViewModel)
			{
				aIChatPanelViewModel.SendMessageCommand.Execute(null);
			}
		}
	}

	private void OnChatDragEnter(object sender, DragEventArgs e)
	{
		try
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Copy;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatControl] 拖拽进入事件处理失败: " + ex.Message);
		}
	}

	private void OnChatDragOver(object sender, DragEventArgs e)
	{
		try
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Copy;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatControl] 拖拽悬停事件处理失败: " + ex.Message);
		}
	}

	private void OnChatDragLeave(object sender, DragEventArgs e)
	{
	}

	private async void OnChatDrop(object sender, DragEventArgs e)
	{
		try
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop) || !(e.Data.GetData(DataFormats.FileDrop) is string[] array) || array.Length == 0)
			{
				return;
			}
			object dataContext = base.DataContext;
			if (!(dataContext is AIChatPanelViewModel viewModel))
			{
				return;
			}
			string[] array2 = array.Where(delegate(string f)
			{
				switch (Path.GetExtension(f).ToLower())
				{
				case ".png":
				case ".jpg":
				case ".jpeg":
				case ".gif":
				case ".bmp":
				case ".webp":
					return true;
				case ".pdf":
					return true;
				case ".txt":
				case ".md":
				case ".json":
				case ".xml":
					return true;
				default:
					return false;
				}
			}).ToArray();
			if (array2.Length == 0)
			{
				Logger.Warning("[AIChatControl] 拖拽的文件类型不支持，已忽略");
				return;
			}
			string[] array3 = array2;
			foreach (string text in array3)
			{
				if (File.Exists(text))
				{
					MethodInfo method = viewModel.GetType().GetMethod("AttachFileByPathAsync", BindingFlags.Instance | BindingFlags.NonPublic);
					if (!(method == null) && method.Invoke(viewModel, new object[1] { text }) is Task task)
					{
						await task;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatControl] 拖拽文件处理失败: " + ex.Message, ex);
		}
	}

	private void OnPaste(object sender, DataObjectPastingEventArgs e)
	{
	}

	private void ScrollToBottom()
	{
		if (ChatScrollViewer != null && ChatScrollViewer.ScrollableHeight != 0.0)
		{
			ChatScrollViewer.ScrollToVerticalOffset(ChatScrollViewer.ScrollableHeight);
		}
	}

	private void SwitchToWindowButton_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			MethodInfo methodInfo = Type.GetType("RevitAi.UI.Services.UIBootstrapper, RevitAi.UI")?.GetMethod("GetService");
			if (methodInfo == null)
			{
				Logger.Error("[AIChatControl] ❌ 无法找到 GetService 方法");
				return;
			}
			Type type = Type.GetType("RevitAi.UI.Services.IWindowManager, RevitAi.UI");
			if (type == null)
			{
				Logger.Error("[AIChatControl] ❌ 无法找到 IWindowManager 类型");
				return;
			}
			object obj = methodInfo.MakeGenericMethod(type).Invoke(null, null);
			if (obj == null)
			{
				Logger.Error("[AIChatControl] ❌ WindowManager 为 null");
			}
			else
			{
				obj.GetType().GetMethod("SwitchToWindowMode")?.Invoke(obj, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AIChatControl] ❌ 调用 WindowManager 失败", ex);
		}
	}}
