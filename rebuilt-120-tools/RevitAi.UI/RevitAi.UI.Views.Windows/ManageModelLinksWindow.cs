using System;
using System.Windows;
using System.Windows.Markup;
using RevitAi.Abstractions.Logging;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class ManageModelLinksWindow : Window, IComponentConnector
{
	public ManageModelLinksWindow()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		try
		{
			InitializeComponent();
			base.DataContextChanged += (DependencyPropertyChangedEventHandler)delegate(object s, DependencyPropertyChangedEventArgs e)
			{
				try
				{
					if (((DependencyPropertyChangedEventArgs)e).NewValue is ManageModelLinksViewModel manageModelLinksViewModel)
					{
						manageModelLinksViewModel.OnRequestClose = (Action)Delegate.Combine(manageModelLinksViewModel.OnRequestClose, (Action)delegate
						{
							Close();
						});
					}
					else if (((DependencyPropertyChangedEventArgs)e).NewValue != null)
					{
						Logger.Warning("[ManageModelLinksWindow] DataContext 类型不匹配: " + ((DependencyPropertyChangedEventArgs)e).NewValue.GetType().Name);
					}
				}
				catch (Exception ex2)
				{
					Logger.Error("[ManageModelLinksWindow] DataContextChanged 处理失败", ex2);
				}
			};
		}
		catch (Exception ex)
		{
			Logger.Error("[ManageModelLinksWindow] 构造函数执行失败", ex);
			throw;
		}
	}
}
