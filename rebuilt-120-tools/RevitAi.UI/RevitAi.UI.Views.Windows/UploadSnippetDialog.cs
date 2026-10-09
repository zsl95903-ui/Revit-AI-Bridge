using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using RevitAi.Core.AI;
using RevitAi.Core.AI.Models;
using RevitAi.UI.Services;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class UploadSnippetDialog : Window, IComponentConnector
{
	private readonly CodeSnippetStorageService _storage;

	private readonly ICodeMarketService? _codeMarketService;

	private CodeSnippet? _selectedSnippet;

	private CodeSnippet? _presetSnippet;

	public UploadSnippetDialog(CodeSnippet? presetSnippet = null)
	{
		InitializeComponent();
		_storage = new CodeSnippetStorageService();
		_codeMarketService = UIBootstrapper.TryGetService<ICodeMarketService>();
		_presetSnippet = presetSnippet;
		CategoryComboBox.SelectedIndex = 0;
		if (_presetSnippet != null)
		{
			_selectedSnippet = _presetSnippet;
			LocalSnippetsPanel.Visibility = Visibility.Collapsed;
			PreviewBorder.DataContext = _selectedSnippet;
			PreviewBorder.Visibility = Visibility.Visible;
		}
		else
		{
			LoadLocalSnippets();
		}
	}

	private void LoadLocalSnippets()
	{
		try
		{
			_storage.Reload();
			List<CodeSnippetItemViewModel> list = (from s in _storage.GetAllSnippets(sortByOrder: true)
				select new CodeSnippetItemViewModel(s)).ToList();
			LocalSnippetsComboBox.ItemsSource = list;
			if (list.Count > 0)
			{
				LocalSnippetsComboBox.SelectedIndex = 0;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[UploadSnippetDialog] 加载本地片段失败: " + ex.Message, ex);
		}
	}

	private void LocalSnippetsComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (LocalSnippetsComboBox.SelectedItem is CodeSnippetItemViewModel codeSnippetItemViewModel)
		{
			_selectedSnippet = _storage.GetSnippet(codeSnippetItemViewModel.Id);
			PreviewBorder.DataContext = _selectedSnippet;
			PreviewBorder.Visibility = Visibility.Visible;
		}
		else
		{
			_selectedSnippet = null;
			PreviewBorder.Visibility = Visibility.Collapsed;
		}
	}

	private void CancelButton_Click(object? sender, RoutedEventArgs e)
	{
		base.DialogResult = false;
		Close();
	}

	private async void UploadButton_Click(object? sender, RoutedEventArgs e)
	{
		if (_selectedSnippet == null)
		{
			MessageBox.Show("请先选择要上传的代码片段", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}
		else
		{
			if (!(CategoryComboBox.SelectedItem is ComboBoxItem { Tag: var tag }))
			{
				return;
			}
			string text = tag?.ToString();
			if (string.IsNullOrEmpty(text))
			{
				MessageBox.Show("请选择分类", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			bool valueOrDefault = FreeRadioButton.IsChecked == true;
			decimal result = 0m;
			if (valueOrDefault || (decimal.TryParse(PriceTextBox.Text, out result) && !(result <= 0m)))
			{
				UploadButton.IsEnabled = false;
				CancelButton.IsEnabled = false;
				try
				{
					if (_codeMarketService == null)
					{
						MessageBox.Show("无法连接到代码市场服务", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
						return;
					}
					CodeMarketResult<CodeSnippetUploadResult> codeMarketResult = await _codeMarketService.UploadSnippetAsync(_selectedSnippet.Name, _selectedSnippet.Description, _selectedSnippet.Code, _selectedSnippet.Tags, text, result);
					if (codeMarketResult.Success)
					{
						MessageBox.Show("上传成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
						base.DialogResult = true;
						Close();
					}
					else
					{
						MessageBox.Show("上传失败：" + codeMarketResult.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
						UploadButton.IsEnabled = true;
						CancelButton.IsEnabled = true;
					}
					return;
				}
				catch (Exception ex)
				{
					Logger.Error("[UploadSnippetDialog] 上传失败: " + ex.Message, ex);
					MessageBox.Show("上传失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
					UploadButton.IsEnabled = true;
					CancelButton.IsEnabled = true;
					return;
				}
			}
			MessageBox.Show("请输入有效的价格（必须大于 0）", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}
	}
}
