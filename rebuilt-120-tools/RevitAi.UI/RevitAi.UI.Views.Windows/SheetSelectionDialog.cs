using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace RevitAi.UI.Views.Windows;

public partial class SheetSelectionDialog : Window, IComponentConnector
{
	public string? SelectedSheet { get; private set; }

	public SheetSelectionDialog(List<string> sheetNames, string fileName)
	{
		InitializeComponent();
		FileNameTextBlock.Text = "文件: " + fileName;
		foreach (string sheetName in sheetNames)
		{
			SheetsListBox.Items.Add(sheetName);
		}
		if (SheetsListBox.Items.Count > 0)
		{
			SheetsListBox.SelectedIndex = 0;
		}
		SheetsListBox.MouseDoubleClick += delegate
		{
			OKButton_Click(this, new RoutedEventArgs());
		};
	}

	private void OKButton_Click(object sender, RoutedEventArgs e)
	{
		if (SheetsListBox.SelectedItem != null)
		{
			SelectedSheet = SheetsListBox.SelectedItem.ToString();
			base.DialogResult = true;
			Close();
		}
	}

	private void CancelButton_Click(object sender, RoutedEventArgs e)
	{
		base.DialogResult = false;
		Close();
	}
}
