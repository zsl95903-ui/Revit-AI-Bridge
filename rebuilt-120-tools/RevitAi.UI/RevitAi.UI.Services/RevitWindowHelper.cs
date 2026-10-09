using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace RevitAi.UI.Services;

internal static class RevitWindowHelper
{
	private struct RECT
	{
		public int Left;

		public int Top;

		public int Right;

		public int Bottom;
	}

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

	public static (double Right, double Bottom)? GetRevitWindowBottomRight()
	{
		try
		{
			nint revitWindowHandle = GetRevitWindowHandle();
			if (revitWindowHandle == IntPtr.Zero)
			{
				return null;
			}
			if (!GetWindowRect(revitWindowHandle, out var lpRect))
			{
				return null;
			}
			double dPIScale = GetDPIScale();
			double item = (double)lpRect.Right / dPIScale;
			double item2 = (double)lpRect.Bottom / dPIScale;
			return (item, item2);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private static nint GetRevitWindowHandle()
	{
		try
		{
			Assembly assembly = TryGetRevitAdapterAssembly();
			if (assembly != null)
			{
				Type type = assembly.GetType("RevitAi.Revit.RevitAdapter") ?? assembly.GetType("RevitAi.Revit.Net8.RevitAdapter");
				if (type != null)
				{
					PropertyInfo property = type.GetProperty("CurrentUIApplication", BindingFlags.Static | BindingFlags.Public);
					if (property != null)
					{
						object value = property.GetValue(null);
						if (value != null)
						{
							PropertyInfo property2 = value.GetType().GetProperty("MainWindowHandle");
							if (property2 != null && property2.GetValue(value) is nint num && num != IntPtr.Zero)
							{
								return num;
							}
						}
					}
				}
			}
			return GetRevitWindowHandleFromProcess();
		}
		catch (Exception)
		{
			return IntPtr.Zero;
		}
	}

	private static Assembly? TryGetRevitAdapterAssembly()
	{
		try
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				string? name = assembly.GetName().Name;
				if (name != null && name.Contains("RevitAi.Revit"))
				{
					return assembly;
				}
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	private static nint GetRevitWindowHandleFromProcess()
	{
		try
		{
			return Process.GetCurrentProcess().MainWindowHandle;
		}
		catch
		{
			return IntPtr.Zero;
		}
	}

	private static double GetDPIScale()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Application current = Application.Current;
			if (current != null && current.Windows.Count > 0)
			{
				PresentationSource presentationSource = PresentationSource.FromVisual(Application.Current.Windows[0]);
				if (presentationSource?.CompositionTarget != null)
				{
					Matrix transformToDevice = presentationSource.CompositionTarget.TransformToDevice;
					return ((Matrix)transformToDevice).M11;
				}
			}
			nint dC = GetDC(IntPtr.Zero);
			if (dC != IntPtr.Zero)
			{
				int deviceCaps = GetDeviceCaps(dC, 88);
				ReleaseDC(IntPtr.Zero, dC);
				return (double)deviceCaps / 96.0;
			}
			return 1.0;
		}
		catch
		{
			return 1.0;
		}
	}

	[DllImport("user32.dll")]
	private static extern nint GetDC(nint hWnd);

	[DllImport("user32.dll")]
	private static extern int ReleaseDC(nint hWnd, nint hDC);

	[DllImport("gdi32.dll")]
	private static extern int GetDeviceCaps(nint hdc, int nIndex);
}
