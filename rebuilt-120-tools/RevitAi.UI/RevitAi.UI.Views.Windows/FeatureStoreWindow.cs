using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using RevitAi.Abstractions.Logging;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class FeatureStoreWindow : Window, IComponentConnector
{
	private readonly FeatureStoreViewModel _viewModel;

	public FeatureStoreWindow()
	{
		InitializeComponent();
		_viewModel = new FeatureStoreViewModel();
		_viewModel.OnRequestClose = delegate
		{
			Close();
		};
		base.DataContext = _viewModel;
	}

	private void OnSaveAndApplyClick(object sender, RoutedEventArgs e)
	{
		try
		{
			Type type = Type.GetType("RevitAi.Main.RibbonConfigManager, RevitAi.Main");
			if (type != null)
			{
				type.GetMethod("SaveConfig")?.Invoke(null, null);
				Close();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("保存配置失败", ex);
			MessageBox.Show("保存配置失败: " + ex.Message, "应用仓库", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void OnFeatureToggleClick(object sender, RoutedEventArgs e)
	{
	}}
