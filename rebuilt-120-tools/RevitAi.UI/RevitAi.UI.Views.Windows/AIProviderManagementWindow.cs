using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using RevitAi.Abstractions.Common;
using RevitAi.Core.AI;
using RevitAi.Core.Authentication;
using RevitAi.UI.Models;
using RevitAi.UI.Services;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class AIProviderManagementWindow : Window, IComponentConnector, IStyleConnector
{
	private readonly LocalAIConfigService _localConfigService;

	public AIProviderManagementWindow()
	{
		InitializeComponent();
		base.Topmost = true;
		_localConfigService = new LocalAIConfigService();
		base.Loaded += OnLoaded;
	}

	private async void OnLoaded(object sender, RoutedEventArgs e)
	{
		await LoadConfigsAsync();
	}

	private async Task LoadConfigsAsync()
	{
		try
		{
			ObservableCollection<AIProviderConfig> localConfigs = _localConfigService.LoadConfigs();
			List<AIConfig> systemConfigs = new List<AIConfig>();
			ISupabaseClient supabaseClient = UIBootstrapper.TryGetService<ISupabaseClient>();
			if (supabaseClient != null)
			{
				Result<List<AIConfig>> result = await supabaseClient.GetAllActiveAIConfigsAsync();
				if (result.IsSuccess && result.Value != null)
				{
					systemConfigs = result.Value;
				}
			}
			List<AIProviderViewModel> list = new List<AIProviderViewModel>();
			foreach (AIProviderConfig item in localConfigs)
			{
				list.Add(new AIProviderViewModel(item));
			}
			foreach (AIConfig item2 in systemConfigs)
			{
				AIProviderConfig config = new AIProviderConfig
				{
					Id = item2.ModelName,
					Provider = item2.Provider,
					ModelName = item2.ModelName,
					DisplayName = item2.DisplayName,
					ProviderEndpoint = (item2.ProviderEndpoint ?? string.Empty),
					EncryptedApiKey = string.Empty,
					MaxTokens = item2.MaxTokens,
					Temperature = item2.Temperature,
					Priority = item2.Priority,
					IsDefault = item2.IsDefault,
					SupportsVision = item2.SupportsVision,
					IsUserConfig = false
				};
				list.Add(new AIProviderViewModel(config));
			}
			string defaultModelId = _localConfigService.GetDefaultModelId();
			if (!string.IsNullOrEmpty(defaultModelId))
			{
				foreach (AIProviderViewModel item3 in list)
				{
					item3.Config.IsDefault = item3.ModelName == defaultModelId;
				}
			}
			ConfigsList.ItemsSource = null;
			ConfigsList.ItemsSource = list;
		}
		catch (Exception ex)
		{
			MessageBox.Show(this, "加载配置失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private async void OnAddClick(object sender, RoutedEventArgs e)
	{
		AIProviderEditWindow editWindow = new AIProviderEditWindow();
		if (editWindow.ShowDialog() != true || editWindow.ResultConfig == null)
		{
			return;
		}
		try
		{
			await Task.Run(delegate
			{
				ObservableCollection<AIProviderConfig> observableCollection = _localConfigService.LoadConfigs();
				observableCollection.Add(editWindow.ResultConfig);
				_localConfigService.SaveConfigs(observableCollection);
			});
			await LoadConfigsAsync();
		}
		catch (Exception ex)
		{
			MessageBox.Show(this, "保存配置失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private async void OnEditClick(object sender, RoutedEventArgs e)
	{
		if (!(sender is Button { Tag: var tag }))
		{
			return;
		}
		AIProviderViewModel viewModel = tag as AIProviderViewModel;
		if (viewModel == null)
		{
			return;
		}
		if (!viewModel.IsUserConfig)
		{
			MessageBox.Show(this, "系统配置不允许编辑", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			return;
		}
		AIProviderEditWindow editWindow = new AIProviderEditWindow(viewModel.Config);
		if (editWindow.ShowDialog() != true || editWindow.ResultConfig == null)
		{
			return;
		}
		try
		{
			await Task.Run(delegate
			{
				ObservableCollection<AIProviderConfig> observableCollection = _localConfigService.LoadConfigs();
				AIProviderConfig aIProviderConfig = observableCollection.FirstOrDefault((AIProviderConfig c) => c.Id == viewModel.Id);
				if (aIProviderConfig != null)
				{
					int index = observableCollection.IndexOf(aIProviderConfig);
					observableCollection[index] = editWindow.ResultConfig;
					_localConfigService.SaveConfigs(observableCollection);
				}
			});
			await LoadConfigsAsync();
		}
		catch (Exception ex)
		{
			MessageBox.Show(this, "更新配置失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private async void OnDeleteClick(object sender, RoutedEventArgs e)
	{
		if (!(sender is Button { Tag: var tag }))
		{
			return;
		}
		AIProviderViewModel viewModel = tag as AIProviderViewModel;
		if (viewModel == null)
		{
			return;
		}
		if (!viewModel.IsUserConfig)
		{
			MessageBox.Show(this, "系统配置不允许删除", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
		else
		{
			if (MessageBox.Show(this, "确定要删除配置 \"" + viewModel.DisplayName + "\" 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
			{
				return;
			}
			try
			{
				await Task.Run(delegate
				{
					ObservableCollection<AIProviderConfig> observableCollection = _localConfigService.LoadConfigs();
					AIProviderConfig aIProviderConfig = observableCollection.FirstOrDefault((AIProviderConfig c) => c.Id == viewModel.Id);
					if (aIProviderConfig != null)
					{
						observableCollection.Remove(aIProviderConfig);
						_localConfigService.SaveConfigs(observableCollection);
					}
				});
				await LoadConfigsAsync();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "删除配置失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}
	}

	private async void OnSetDefaultClick(object sender, RoutedEventArgs e)
	{
		if (!(sender is Button { Tag: var tag }))
		{
			return;
		}
		AIProviderViewModel viewModel = tag as AIProviderViewModel;
		if (viewModel == null)
		{
			return;
		}
		try
		{
			await Task.Run(delegate
			{
				if (viewModel.IsUserConfig)
				{
					ObservableCollection<AIProviderConfig> observableCollection = _localConfigService.LoadConfigs();
					foreach (AIProviderConfig item in observableCollection)
					{
						item.IsDefault = false;
					}
					AIProviderConfig aIProviderConfig = observableCollection.FirstOrDefault((AIProviderConfig c) => c.Id == viewModel.Id);
					if (aIProviderConfig != null)
					{
						aIProviderConfig.IsDefault = true;
						_localConfigService.SaveConfigs(observableCollection);
					}
				}
				_localConfigService.SaveDefaultModelId(viewModel.ModelName);
			});
			await LoadConfigsAsync();
		}
		catch (Exception ex)
		{
			MessageBox.Show(this, "设置默认配置失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void OnCloseClick(object sender, RoutedEventArgs e)
	{
		base.DialogResult = true;
		Close();
	}}
