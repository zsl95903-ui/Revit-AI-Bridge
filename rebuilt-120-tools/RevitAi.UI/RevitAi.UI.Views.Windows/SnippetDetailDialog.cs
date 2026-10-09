using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using RevitAi.Core.AI;
using RevitAi.Core.AI.Models;
using RevitAi.UI.Services;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class SnippetDetailDialog : Window, IComponentConnector
{
	private readonly ICodeMarketService? _codeMarketService;

	private readonly CodeSnippetStorageService _storage;

	private CodeSnippetMarketItemViewModel? _snippet;

	private string? _snippetId;

	private bool _isFavorited;

	private bool _isOwned;

	private int _userRating;

	public SnippetDetailDialog(string snippetId)
	{
		InitializeComponent();
		_snippetId = snippetId;
		_codeMarketService = UIBootstrapper.TryGetService<ICodeMarketService>();
		_storage = new CodeSnippetStorageService();
		LoadSnippetDetailAsync();
	}

	private async Task LoadSnippetDetailAsync()
	{
		_ = 1;
		try
		{
			if (_codeMarketService == null)
			{
				MessageBox.Show("无法连接到代码市场服务", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				Close();
				return;
			}
			CodeMarketResult<HashSet<string>> codeMarketResult = await _codeMarketService.CheckFavoritedStatusAsync(new List<string> { _snippetId });
			if (codeMarketResult.Success && codeMarketResult.Data != null)
			{
				_isFavorited = codeMarketResult.Data.Contains(_snippetId);
				UpdateFavoriteButton();
			}
			CodeMarketResult<CodeSnippetDownloadResult> codeMarketResult2 = await _codeMarketService.DownloadSnippetAsync(_snippetId);
			if (codeMarketResult2.Success && codeMarketResult2.Data != null)
			{
				CodeSnippetDownloadResult data = codeMarketResult2.Data;
				_isOwned = data.Owned || data.IsFree;
				_snippet = new CodeSnippetMarketItemViewModel(data.Snippet, _isFavorited);
				UpdateUI(_snippet, _isOwned);
				if (_isOwned || data.IsFree)
				{
					CodeContentText.Text = data.Snippet?.CodeContent ?? "无代码内容";
					CodeContentScrollViewer.Visibility = Visibility.Visible;
					LockedContentBorder.Visibility = Visibility.Collapsed;
				}
				else
				{
					CodeContentScrollViewer.Visibility = Visibility.Collapsed;
					LockedContentBorder.Visibility = Visibility.Visible;
				}
			}
			else
			{
				MessageBox.Show("加载片段详情失败：" + codeMarketResult2.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				Close();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[SnippetDetailDialog] 加载片段详情失败: " + ex.Message, ex);
			MessageBox.Show("加载片段详情失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			Close();
		}
	}

	private void UpdateUI(CodeSnippetMarketItemViewModel snippet, bool isOwned)
	{
		TitleText.Text = snippet.Name;
		DescriptionText.Text = snippet.Description;
		CategoryBadge.Text = snippet.CategoryDisplayName;
		AuthorText.Text = "\ud83d\udc64 " + snippet.AuthorName;
		if (snippet.IsFree)
		{
			PriceBadge.Visibility = Visibility.Collapsed;
			OwnedBadge.Visibility = Visibility.Visible;
			OwnedBadge.Text = "✅ 免费";
		}
		else if (isOwned)
		{
			PriceBadge.Visibility = Visibility.Collapsed;
			OwnedBadge.Visibility = Visibility.Visible;
			OwnedBadge.Text = "✅ 已拥有";
		}
		else
		{
			PriceText.Text = snippet.Price.ToString();
			PriceBadge.Visibility = Visibility.Visible;
			OwnedBadge.Visibility = Visibility.Collapsed;
		}
		DownloadCountText.Text = snippet.DownloadCount.ToString();
		ViewCountText.Text = snippet.ViewCount.ToString();
		FavoriteCountText.Text = snippet.FavoriteCount.ToString();
		RatingText.Text = $"{snippet.AverageRating:F1} ({snippet.RatingCount})";
		TagsPanel.Children.Clear();
		foreach (string tag in snippet.Tags)
		{
			TextBlock element = new TextBlock
			{
				Text = "#" + tag,
				FontSize = 11.0,
				Foreground = new SolidColorBrush(Color.FromRgb(0, 119, 189)),
				Margin = new Thickness(0.0, 0.0, 8.0, 8.0),
				Padding = new Thickness(6.0, 3.0, 6.0, 3.0),
				Background = new SolidColorBrush(Color.FromRgb(227, 242, 253))
			};
			TagsPanel.Children.Add(element);
		}
		if (isOwned || snippet.IsFree)
		{
			DownloadButton.Content = "已下载";
			DownloadButton.IsEnabled = false;
			DownloadButton.Background = new SolidColorBrush(Color.FromRgb(117, 117, 117));
		}
		else
		{
			DownloadButton.Content = $"购买 ({snippet.Price} ⚡)";
			DownloadButton.IsEnabled = true;
		}
		if (snippet.RatingCount > 0)
		{
			RatingStatusText.Text = $"已有 {snippet.RatingCount} 人评分";
		}
		else
		{
			RatingStatusText.Text = "暂无评分";
		}
		if (!isOwned && !snippet.IsFree)
		{
			RatingBorder.Visibility = Visibility.Collapsed;
		}
	}

	private void UpdateFavoriteButton()
	{
		if (_isFavorited)
		{
			FavoriteIconText.Text = "❤\ufe0f";
			FavoriteButtonText.Text = "已收藏";
		}
		else
		{
			FavoriteIconText.Text = "\ud83e\udd0d";
			FavoriteButtonText.Text = "收藏";
		}
	}

	private void CopyCodeButton_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			Clipboard.SetText(CodeContentText.Text);
			MessageBox.Show("代码已复制到剪贴板", "成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
		catch (Exception ex)
		{
			Logger.Error("[SnippetDetailDialog] 复制代码失败: " + ex.Message, ex);
		}
	}

	private async void FavoriteButton_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			if (_codeMarketService == null || string.IsNullOrEmpty(_snippetId))
			{
				return;
			}
			FavoriteButton.IsEnabled = false;
			CodeMarketResult<bool> codeMarketResult = await _codeMarketService.ToggleFavoriteAsync(_snippetId);
			if (codeMarketResult.Success)
			{
				_isFavorited = codeMarketResult.Data;
				UpdateFavoriteButton();
				if (_snippet != null)
				{
					if (_isFavorited)
					{
						_snippet.FavoriteCount++;
					}
					else
					{
						_snippet.FavoriteCount--;
					}
					FavoriteCountText.Text = _snippet.FavoriteCount.ToString();
				}
				Logger.Info($"[SnippetDetailDialog] 收藏操作成功: {_isFavorited}");
			}
			else
			{
				MessageBox.Show("收藏操作失败：" + codeMarketResult.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			FavoriteButton.IsEnabled = true;
		}
		catch (Exception ex)
		{
			Logger.Error("[SnippetDetailDialog] 收藏操作失败: " + ex.Message, ex);
			FavoriteButton.IsEnabled = true;
		}
	}

	private async void RatingRadioButton_Checked(object sender, RoutedEventArgs e)
	{
		try
		{
			if (sender is RadioButton { Tag: string tag } && int.TryParse(tag, out var rating) && _codeMarketService != null && !string.IsNullOrEmpty(_snippetId))
			{
				CodeMarketResult<bool> codeMarketResult = await _codeMarketService.RateSnippetAsync(_snippetId, rating);
				if (codeMarketResult.Success)
				{
					RatingStatusText.Text = $"✓ 已评分 {rating} 星";
					_userRating = rating;
					Logger.Info($"[SnippetDetailDialog] 评分成功: {rating}");
				}
				else
				{
					RatingStatusText.Text = "评分失败：" + codeMarketResult.Error;
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[SnippetDetailDialog] 评分失败: " + ex.Message, ex);
		}
	}

	private async void DownloadButton_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			if (_codeMarketService == null || string.IsNullOrEmpty(_snippetId))
			{
				return;
			}
			DownloadButton.IsEnabled = false;
			CodeMarketResult<CodeSnippetDownloadResult> codeMarketResult = await _codeMarketService.DownloadSnippetAsync(_snippetId);
			if (codeMarketResult.Success && codeMarketResult.Data != null)
			{
				CodeSnippetDownloadResult data = codeMarketResult.Data;
				if (data.Snippet != null)
				{
					object obj = new CodeSnippet
					{
						Name = data.Snippet.Name,
						Description = data.Snippet.Description,
						Code = (data.Snippet.CodeContent ?? string.Empty),
						Tags = (data.Snippet.Tags ?? new List<string>())
					};
					if (obj == null)
					{
						obj = new CodeSnippet();
					}
					CodeSnippet snippet = (CodeSnippet)obj;
					_storage.AddSnippet(snippet);
					MessageBox.Show(data.IsFree ? "下载成功！已保存到本地代码片段库" : $"购买成功！花费 {data.PricePaid} 电量\n已保存到本地代码片段库", "成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
					_isOwned = true;
					UpdateUI(_snippet, isOwned: true);
					CodeContentText.Text = data.Snippet.CodeContent ?? "无代码内容";
					CodeContentScrollViewer.Visibility = Visibility.Visible;
					LockedContentBorder.Visibility = Visibility.Collapsed;
					Logger.Info("[SnippetDetailDialog] 下载成功: " + _snippetId);
				}
			}
			else
			{
				MessageBox.Show("下载失败：" + codeMarketResult.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				DownloadButton.IsEnabled = true;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[SnippetDetailDialog] 下载失败: " + ex.Message, ex);
			MessageBox.Show("下载失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			DownloadButton.IsEnabled = true;
		}
	}

	private void CloseButton_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}
}
