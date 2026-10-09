using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class TopographyFromFloorWindow : Window, IComponentConnector
{
	private struct RECT
	{
		public int Left;

		public int Top;

		public int Right;

		public int Bottom;
	}

	private struct POINT
	{
		public int X;

		public int Y;
	}

	[DllImport("user32.dll")]
	private static extern nint GetForegroundWindow();

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

	[DllImport("user32.dll")]
	private static extern bool GetClientRect(nint hWnd, out RECT lpRect);

	[DllImport("user32.dll")]
	private static extern bool ClientToScreen(nint hWnd, ref POINT lpPoint);

	public TopographyFromFloorWindow()
	{
		InitializeComponent();
		base.Loaded += TopographyFromFloorWindow_Loaded;
	}

	private void TopographyFromFloorWindow_Loaded(object sender, RoutedEventArgs e)
	{
		if (base.DataContext is TopographyFromFloorViewModel topographyFromFloorViewModel)
		{
			topographyFromFloorViewModel.SetDispatcher(((DispatcherObject)this).Dispatcher);
		}
		try
		{
			nint foregroundWindow = GetForegroundWindow();
			if (foregroundWindow != IntPtr.Zero && GetClientRect(foregroundWindow, out var _))
			{
				POINT lpPoint = new POINT
				{
					X = 0,
					Y = 0
				};
				ClientToScreen(foregroundWindow, ref lpPoint);
				base.Left = lpPoint.X + 20;
				base.Top = lpPoint.Y + 20;
				return;
			}
		}
		catch (Exception)
		{
		}
		base.Left = 100.0;
		base.Top = 100.0;
	}
}
