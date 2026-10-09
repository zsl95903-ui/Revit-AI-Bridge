using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class WallToRoadWindow : Window, IComponentConnector
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

	public WallToRoadWindow()
	{
		InitializeComponent();
		base.Loaded += WallToRoadWindow_Loaded;
	}

	private void WallToRoadWindow_Loaded(object sender, RoutedEventArgs e)
	{
		if (base.DataContext is WallToRoadViewModel wallToRoadViewModel)
		{
			wallToRoadViewModel.CloseWindow = delegate
			{
				Close();
			};
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
