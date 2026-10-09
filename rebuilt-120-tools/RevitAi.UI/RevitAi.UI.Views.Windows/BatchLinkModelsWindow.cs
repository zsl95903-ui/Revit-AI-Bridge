using System.Windows;
using System.Windows.Markup;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class BatchLinkModelsWindow : Window, IComponentConnector
{
	public BatchLinkModelsWindow()
	{
		InitializeComponent();
		base.DataContext = new BatchLinkModelsViewModel();
	}
}
