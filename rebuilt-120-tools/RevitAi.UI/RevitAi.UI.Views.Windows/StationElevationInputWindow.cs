using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class StationElevationInputWindow : Window, IComponentConnector
{
	public StationElevationInputWindow()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		InitializeComponent();
		base.DataContextChanged += (DependencyPropertyChangedEventHandler)delegate
		{
			if (base.DataContext is StationElevationInputViewModel stationElevationInputViewModel)
			{
				stationElevationInputViewModel.SetWindow(this);
			}
		};
	}

	private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (base.DataContext is StationElevationInputViewModel stationElevationInputViewModel && sender is DataGrid dataGrid)
		{
			stationElevationInputViewModel.SelectedItems = dataGrid.SelectedItems;
		}
	}
}
