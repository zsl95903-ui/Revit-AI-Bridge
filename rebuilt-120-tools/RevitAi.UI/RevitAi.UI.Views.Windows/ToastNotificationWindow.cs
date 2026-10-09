using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace RevitAi.UI.Views.Windows;

public partial class ToastNotificationWindow : Window, IComponentConnector
{
	private readonly int _displayDuration;

	private double? _targetRight;

	private double? _targetBottom;

	private double _offsetX = 20.0;

	private double _offsetY = 20.0;

	public ToastNotificationWindow(string title, string message, int displayDuration = 5)
	{
		InitializeComponent();
		TitleTextBlock.Text = title;
		MessageTextBlock.Text = message;
		_displayDuration = Math.Max(1, displayDuration);
		base.Loaded += OnLoaded;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		if (_targetRight.HasValue && _targetBottom.HasValue)
		{
			ApplyPosition();
		}
		DispatcherTimer timer = new DispatcherTimer
		{
			Interval = TimeSpan.FromSeconds(_displayDuration)
		};
		timer.Tick += delegate
		{
			timer.Stop();
			BeginFadeOut();
		};
		timer.Start();
	}

	private void ApplyPosition()
	{
		if (_targetRight.HasValue && _targetBottom.HasValue)
		{
			base.Left = _targetRight.Value - base.ActualWidth - _offsetX;
			base.Top = _targetBottom.Value - base.ActualHeight - _offsetY;
		}
	}

	private void BeginFadeOut()
	{
		DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, TimeSpan.FromSeconds(0.5));
		doubleAnimation.Completed += delegate
		{
			Close();
		};
		BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
	}

	public void PositionAtBottomRight(double right, double bottom, double offsetX = 20.0, double offsetY = 20.0)
	{
		_targetRight = right;
		_targetBottom = bottom;
		_offsetX = offsetX;
		_offsetY = offsetY;
		if (base.IsLoaded)
		{
			ApplyPosition();
		}
	}
}
