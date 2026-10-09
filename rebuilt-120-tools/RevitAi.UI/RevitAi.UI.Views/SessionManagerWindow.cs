using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using RevitAi.Abstractions.Loader;
using RevitAi.Core.AI;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views;

public partial class SessionManagerWindow : Window, IComponentConnector
{
	private CheckBox? _selectAllCheckBox;

	public SessionManagerWindow()
	{
		InitializeComponent();
	}

	public void SetViewModel(SessionManagerViewModel viewModel)
	{
		base.DataContext = viewModel ?? throw new ArgumentNullException("viewModel");
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		ApplyRevit2018CompatibilityFix();
		_selectAllCheckBox = FindName("SelectAllCheckBox") as CheckBox;
		if (base.DataContext is SessionManagerViewModel sessionManagerViewModel)
		{
			await sessionManagerViewModel.InitializeAsync();
		}
	}

	private void ApplyRevit2018CompatibilityFix()
	{
		try
		{
			if (RevitVersionDetector.GetCurrentVersion().VersionYear <= 2020)
			{
				if (FindName("MainGridSplitter") is GridSplitter { Parent: Grid parent } gridSplitter)
				{
					parent.Children.Remove(gridSplitter);
				}
				if (FindName("SessionsListBox") is ListBox listBox)
				{
					listBox.ItemContainerStyle = null;
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
	{
		if (!(base.DataContext is SessionManagerViewModel sessionManagerViewModel))
		{
			return;
		}
		foreach (SessionIndexItem session in sessionManagerViewModel.Sessions)
		{
			session.IsSelected = true;
		}
	}

	private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
	{
		if (!(base.DataContext is SessionManagerViewModel sessionManagerViewModel))
		{
			return;
		}
		foreach (SessionIndexItem session in sessionManagerViewModel.Sessions)
		{
			session.IsSelected = false;
		}
	}
}
