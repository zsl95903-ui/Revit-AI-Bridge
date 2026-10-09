using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using RevitAi.Abstractions.Infrastructure;

namespace RevitAi.UI.Views.Windows;

public partial class CopySubgradeLayersDialog : Window, IComponentConnector
{
	private readonly List<RoadProjectSelectItem> _items;

	public List<RoadProject> SelectedProjects => (from i in _items
		where i.IsSelected
		select i.Project).ToList();

	public CopySubgradeLayersDialog(List<RoadProject> roadProjects)
	{
		InitializeComponent();
		_items = roadProjects.Select((RoadProject p) => new RoadProjectSelectItem
		{
			Project = p,
			IsSelected = true
		}).ToList();
		RoadListBox.ItemsSource = _items;
	}

	private void SelectAll_Click(object sender, RoutedEventArgs e)
	{
		foreach (RoadProjectSelectItem item in _items)
		{
			item.IsSelected = true;
		}
	}

	private void DeselectAll_Click(object sender, RoutedEventArgs e)
	{
		foreach (RoadProjectSelectItem item in _items)
		{
			item.IsSelected = false;
		}
	}

	private void Ok_Click(object sender, RoutedEventArgs e)
	{
		base.DialogResult = true;
		Close();
	}

	private void Cancel_Click(object sender, RoutedEventArgs e)
	{
		base.DialogResult = false;
		Close();
	}
}
