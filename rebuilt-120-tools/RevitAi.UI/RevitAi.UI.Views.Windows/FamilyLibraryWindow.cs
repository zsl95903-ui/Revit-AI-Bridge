using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using RevitAi.Abstractions.FamilyLibrary;
using RevitAi.Abstractions.Logging;
using RevitAi.UI.Models;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class FamilyLibraryWindow : Window, IComponentConnector, IStyleConnector
{
	public FamilyLibraryWindow(FamilyLibraryViewModel viewModel)
	{
		InitializeComponent();
		base.DataContext = viewModel ?? throw new ArgumentNullException("viewModel");
		base.Loaded += OnWindowLoaded;
		base.Closing += OnWindowClosing;
	}

	private void OnWindowClosing(object? sender, CancelEventArgs e)
	{
		if (base.DataContext is FamilyLibraryViewModel familyLibraryViewModel)
		{
			familyLibraryViewModel.Cleanup();
		}
	}

	private void OnWindowLoaded(object sender, RoutedEventArgs e)
	{
		object dataContext = base.DataContext;
		FamilyLibraryViewModel viewModel = dataContext as FamilyLibraryViewModel;
		if (viewModel == null)
		{
			return;
		}
		((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Func<Task>)async delegate
		{
			try
			{
				await viewModel.InitializeAsync();
			}
			catch (Exception ex)
			{
				Logger.Error("[FamilyLibraryWindow] 初始化失败", ex);
			}
		}, (DispatcherPriority)6, Array.Empty<object>());
	}

	private void OnCategoryTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
		if (e.NewValue != e.OldValue && base.DataContext is FamilyLibraryViewModel familyLibraryViewModel && e.NewValue is FamilyCategoryNode selectedCategoryNode)
		{
			RevitCategoryListBox.SelectedIndex = -1;
			familyLibraryViewModel.SelectedCategoryNode = selectedCategoryNode;
		}
	}

	private void OnRevitCategoryTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
	}

	private void OnRevitCategoryListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (base.DataContext is FamilyLibraryViewModel familyLibraryViewModel && sender is ListBox { SelectedItem: string selectedItem })
		{
			familyLibraryViewModel.SearchText = string.Empty;
			familyLibraryViewModel.SelectedRevitCategory = selectedItem;
			familyLibraryViewModel.SearchCommand?.Execute(null);
		}
	}

	private void OnCloseDetailPanel(object sender, RoutedEventArgs e)
	{
		if (base.DataContext is FamilyLibraryViewModel familyLibraryViewModel)
		{
			familyLibraryViewModel.SelectedFamily = null;
		}
	}

	private void OnFamilyItemClick(object sender, MouseButtonEventArgs e)
	{
		if (sender is Border { DataContext: FamilyLibraryItem dataContext } && base.DataContext is FamilyLibraryViewModel familyLibraryViewModel)
		{
			if (familyLibraryViewModel.SelectedFamily?.Id == dataContext.Id)
			{
				familyLibraryViewModel.SelectedFamily = null;
			}
			else
			{
				familyLibraryViewModel.SelectedFamily = dataContext;
			}
		}
	}

	private void OnFamilyScrollChanged(object sender, ScrollChangedEventArgs e)
	{
		if (base.DataContext is FamilyLibraryViewModel familyLibraryViewModel && sender is ScrollViewer { ScrollableHeight: >0.0 } scrollViewer && scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight - 100.0 && !familyLibraryViewModel.IsLoading)
		{
			familyLibraryViewModel.LoadMoreCommand?.Execute(null);
		}
	}
}
