using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using RevitAi.Abstractions.Logging;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class PurchaseCreditsWindow : Window, IComponentConnector
{
	private readonly PurchaseCreditsViewModel _viewModel;

	public PurchaseCreditsWindow()
	{
		InitializeComponent();
		_viewModel = new PurchaseCreditsViewModel(null, this);
		base.DataContext = _viewModel;
		base.Topmost = true;
	}

	public new bool? ShowDialog()
	{
		return base.ShowDialog();
	}

	public void CustomAmountTextBox_GotFocus(object sender, RoutedEventArgs e)
	{
		try
		{
			if (CustomAmountRadioButton != null)
			{
				Logger.Info("[PurchaseCreditsWindow] CustomAmountTextBox_GotFocus triggered");
				CustomAmountRadioButton.IsChecked = true;
				Logger.Info($"[PurchaseCreditsWindow] CustomAmountRadioButton.IsChecked = {CustomAmountRadioButton.IsChecked}");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[PurchaseCreditsWindow] CustomAmountTextBox_GotFocus Error: " + ex.Message);
		}
	}

	private void CreditsDeveloperContact_MouseEnter(object sender, MouseEventArgs e)
	{
		if (base.DataContext is PurchaseCreditsViewModel purchaseCreditsViewModel)
		{
			purchaseCreditsViewModel.ShowDeveloperQrCode = true;
		}
	}

	private void CreditsDeveloperContact_MouseLeave(object sender, MouseEventArgs e)
	{
		if (base.DataContext is PurchaseCreditsViewModel purchaseCreditsViewModel)
		{
			purchaseCreditsViewModel.ShowDeveloperQrCode = false;
		}
	}
}
