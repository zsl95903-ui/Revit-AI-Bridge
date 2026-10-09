using System.Windows;
using System.Windows.Markup;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class DefineBeamWindow : Window, IComponentConnector
{
	private DefineBeamViewModel? _viewModel;

	public DefineBeamWindow()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		InitializeComponent();
		base.DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
	}

	private void OnDataContextChanged(object? sender, DependencyPropertyChangedEventArgs e)
	{
		if (base.DataContext is DefineBeamViewModel viewModel)
		{
			_viewModel = viewModel;
			_viewModel.RequestClose = delegate
			{
				Close();
			};
		}
	}
}
