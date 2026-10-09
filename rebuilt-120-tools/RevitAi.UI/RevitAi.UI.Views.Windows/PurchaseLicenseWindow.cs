using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class PurchaseLicenseWindow : Window, IComponentConnector
{
	public PurchaseLicenseWindow()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		InitializeComponent();
		base.DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
	}

	private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		if (!(base.DataContext is PurchaseLicenseViewModel purchaseLicenseViewModel))
		{
			return;
		}
		purchaseLicenseViewModel.PropertyChanged += delegate(object? s, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "IsPaymentView")
			{
				UpdateViewVisibility();
			}
		};
	}

	private void UpdateViewVisibility()
	{
		if (base.DataContext is PurchaseLicenseViewModel purchaseLicenseViewModel)
		{
			if (purchaseLicenseViewModel.IsPaymentView)
			{
				SelectionView.Visibility = Visibility.Collapsed;
				PaymentView.Visibility = Visibility.Visible;
			}
			else
			{
				SelectionView.Visibility = Visibility.Visible;
				PaymentView.Visibility = Visibility.Collapsed;
			}
		}
	}

	private void LicenseDeveloperContact_MouseEnter(object sender, MouseEventArgs e)
	{
		if (base.DataContext is PurchaseLicenseViewModel purchaseLicenseViewModel)
		{
			purchaseLicenseViewModel.ShowDeveloperQrCode = true;
		}
	}

	private void LicenseDeveloperContact_MouseLeave(object sender, MouseEventArgs e)
	{
		if (base.DataContext is PurchaseLicenseViewModel purchaseLicenseViewModel)
		{
			purchaseLicenseViewModel.ShowDeveloperQrCode = false;
		}
	}
}
