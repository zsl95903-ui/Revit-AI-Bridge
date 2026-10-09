using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace RevitAi.UI.Views.Windows;

public partial class RoadModelPlacementWindow : Window, IComponentConnector
{
	public RoadModelPlacementWindow()
	{
		InitializeComponent();
	}

	private void ScenarioComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is ComboBox { SelectedIndex: >=0 } comboBox)
		{
			SinglePlacementPanel.Visibility = Visibility.Collapsed;
			IntervalPlacementPanel.Visibility = Visibility.Collapsed;
			CustomListPanel.Visibility = Visibility.Collapsed;
			switch (comboBox.SelectedIndex)
			{
			case 0:
				SinglePlacementPanel.Visibility = Visibility.Visible;
				break;
			case 1:
				IntervalPlacementPanel.Visibility = Visibility.Visible;
				break;
			case 2:
				CustomListPanel.Visibility = Visibility.Visible;
				break;
			}
		}
	}
}
