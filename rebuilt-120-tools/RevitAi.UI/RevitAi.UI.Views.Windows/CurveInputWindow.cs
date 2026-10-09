using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class CurveInputWindow : Window, IComponentConnector
{
	public CurveInputWindow()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		InitializeComponent();
		base.DataContextChanged += (DependencyPropertyChangedEventHandler)delegate
		{
			if (base.DataContext is CurveInputViewModel curveInputViewModel)
			{
				curveInputViewModel.SetWindow(this);
			}
		};
	}

	private void HorizontalDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (base.DataContext is CurveInputViewModel curveInputViewModel)
		{
			curveInputViewModel.SelectedHorizontalItems = (sender as DataGrid)?.SelectedItems;
		}
	}

	private void VerticalDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (base.DataContext is CurveInputViewModel curveInputViewModel)
		{
			curveInputViewModel.SelectedVerticalItems = (sender as DataGrid)?.SelectedItems;
		}
	}
}
