using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace RevitAi.UI.Behaviors;

public static class ItemVisibilityBehavior
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static DependencyPropertyChangedEventHandler _003C0_003E__OnElementIsVisibleChanged;
	}

	public static readonly DependencyProperty IsVisibleCommandProperty;

	public static readonly DependencyProperty ItemIdProperty;

	public static ICommand GetIsVisibleCommand(DependencyObject obj)
	{
		return (ICommand)obj.GetValue(IsVisibleCommandProperty);
	}

	public static void SetIsVisibleCommand(DependencyObject obj, ICommand value)
	{
		obj.SetValue(IsVisibleCommandProperty, (object)value);
	}

	public static Guid GetItemId(DependencyObject obj)
	{
		return (Guid)obj.GetValue(ItemIdProperty);
	}

	public static void SetItemId(DependencyObject obj, Guid value)
	{
		obj.SetValue(ItemIdProperty, (object)value);
	}

	private static void OnIsVisibleCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		if (d is FrameworkElement frameworkElement)
		{
			object obj = _003C_003EO._003C0_003E__OnElementIsVisibleChanged;
			if (obj == null)
			{
				DependencyPropertyChangedEventHandler val = OnElementIsVisibleChanged;
				_003C_003EO._003C0_003E__OnElementIsVisibleChanged = val;
				obj = (object)val;
			}
			frameworkElement.IsVisibleChanged -= (DependencyPropertyChangedEventHandler)obj;
			object obj2 = _003C_003EO._003C0_003E__OnElementIsVisibleChanged;
			if (obj2 == null)
			{
				DependencyPropertyChangedEventHandler val2 = OnElementIsVisibleChanged;
				_003C_003EO._003C0_003E__OnElementIsVisibleChanged = val2;
				obj2 = (object)val2;
			}
			frameworkElement.IsVisibleChanged += (DependencyPropertyChangedEventHandler)obj2;
		}
	}

	private static void OnElementIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		if (sender is FrameworkElement { IsVisible: not false } frameworkElement)
		{
			ICommand isVisibleCommand = GetIsVisibleCommand((DependencyObject)(object)frameworkElement);
			Guid itemId = GetItemId((DependencyObject)(object)frameworkElement);
			if (isVisibleCommand != null && isVisibleCommand.CanExecute(itemId))
			{
				isVisibleCommand.Execute(itemId);
			}
		}
	}

	static ItemVisibilityBehavior()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		IsVisibleCommandProperty = DependencyProperty.RegisterAttached("IsVisibleCommand", typeof(ICommand), typeof(ItemVisibilityBehavior), new PropertyMetadata((object)null, new PropertyChangedCallback(OnIsVisibleCommandChanged)));
		ItemIdProperty = DependencyProperty.RegisterAttached("ItemId", typeof(Guid), typeof(ItemVisibilityBehavior), new PropertyMetadata((object)Guid.Empty));
	}
}
