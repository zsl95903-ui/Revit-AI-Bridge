using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RevitAi.Revit.UI;

public static class RevitAdapterManager
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private static object? object_0;

	public static object? CurrentAdapter
	{
		[CompilerGenerated]
		get
		{
			return object_0;
		}
		[CompilerGenerated]
		private set
		{
			object_0 = value;
		}
	}

	public static void SetAdapter(object adapter)
	{
		CurrentAdapter = adapter;
	}

	public static void ClearAdapter()
	{
		CurrentAdapter = null;
	}
}
