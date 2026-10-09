using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using RevitAi.UI.Models;

namespace RevitAi.UI.Views.Windows;

public partial class AIProviderEditWindow : Window, IComponentConnector
{
	public AIProviderConfig? ResultConfig { get; private set; }

	public AIProviderEditWindow()
	{
		InitializeComponent();
		base.Topmost = true;
		base.DataContext = new AIProviderConfig
		{
			Provider = "deepseek",
			ModelName = "deepseek-v4-pro",
			ProviderEndpoint = "https://api.deepseek.com",
			MaxTokens = 8000,
			Temperature = 1.0
		};
		base.Title = "添加AI提供商";
		ProviderComboBox.SelectedIndex = 0;
	}

	public AIProviderEditWindow(AIProviderConfig existingConfig)
	{
		InitializeComponent();
		base.Topmost = true;
		base.DataContext = existingConfig;
		base.Title = "编辑AI提供商";
		bool flag = existingConfig.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase) || existingConfig.Provider.Equals("vllm", StringComparison.OrdinalIgnoreCase);
		ApiKeyLabel.Text = (flag ? "API Key (可选)" : "API Key *");
		SelectProvider(existingConfig.Provider, triggerEvent: false);
		if (!string.IsNullOrEmpty(existingConfig.EncryptedApiKey) && existingConfig.EncryptedApiKey != "local-placeholder")
		{
			ApiKeyPasswordBox.Password = existingConfig.EncryptedApiKey;
		}
	}

	private void OnProviderChanged(object sender, SelectionChangedEventArgs e)
	{
		if (ProviderComboBox.SelectedItem is ComboBoxItem { Tag: string tag } && base.DataContext is AIProviderConfig aIProviderConfig)
		{
			string providerEndpoint = tag switch
			{
				"deepseek" => "https://api.deepseek.com", 
				"openai" => "https://api.openai.com/v1", 
				"ollama" => "http://localhost:11434", 
				"vllm" => "http://localhost:8000/v1", 
				"custom" => string.Empty, 
				_ => string.Empty, 
			};
			if (string.IsNullOrWhiteSpace(aIProviderConfig.ProviderEndpoint) || IsDefaultEndpoint(aIProviderConfig.ProviderEndpoint))
			{
				aIProviderConfig.ProviderEndpoint = providerEndpoint;
			}
			aIProviderConfig.Provider = tag;
			bool flag = tag.Equals("ollama", StringComparison.OrdinalIgnoreCase) || tag.Equals("vllm", StringComparison.OrdinalIgnoreCase);
			ApiKeyLabel.Text = (flag ? "API Key (可选)" : "API Key *");
		}
	}

	private static bool IsDefaultEndpoint(string endpoint)
	{
		return endpoint switch
		{
			"https://api.deepseek.com" => true, 
			"https://api.openai.com/v1" => true, 
			"http://localhost:11434" => true, 
			"http://localhost:8000/v1" => true, 
			_ => false, 
		};
	}

	private void OnSaveClick(object sender, RoutedEventArgs e)
	{
		if (!(base.DataContext is AIProviderConfig aIProviderConfig))
		{
			return;
		}
		if (string.IsNullOrWhiteSpace(aIProviderConfig.DisplayName))
		{
			ShowError("请输入显示名称");
			DisplayNameTextBox.Focus();
			return;
		}
		if (string.IsNullOrWhiteSpace(aIProviderConfig.ModelName))
		{
			ShowError("请输入模型名称");
			ModelNameTextBox.Focus();
			return;
		}
		string password = ApiKeyPasswordBox.Password;
		bool flag = aIProviderConfig.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase) || aIProviderConfig.Provider.Equals("vllm", StringComparison.OrdinalIgnoreCase);
		if (string.IsNullOrWhiteSpace(password) && !flag)
		{
			ShowError("请输入API Key");
			ApiKeyPasswordBox.Focus();
			return;
		}
		aIProviderConfig.EncryptedApiKey = (flag ? "local-placeholder" : password);
		ResultConfig = aIProviderConfig;
		base.DialogResult = true;
		Close();
	}

	private void OnCancelClick(object sender, RoutedEventArgs e)
	{
		base.DialogResult = false;
		Close();
	}

	private void SelectProvider(string provider, bool triggerEvent = true)
	{
		foreach (ComboBoxItem item in (IEnumerable)ProviderComboBox.Items)
		{
			if (item.Tag is string text && text == provider)
			{
				if (triggerEvent)
				{
					ProviderComboBox.SelectedItem = item;
				}
				else
				{
					ProviderComboBox.SelectedIndex = ProviderComboBox.Items.IndexOf(item);
				}
				break;
			}
		}
	}

	private void ShowError(string message)
	{
		MessageBox.Show(this, message, "验证错误", MessageBoxButton.OK, MessageBoxImage.Exclamation);
	}
}
