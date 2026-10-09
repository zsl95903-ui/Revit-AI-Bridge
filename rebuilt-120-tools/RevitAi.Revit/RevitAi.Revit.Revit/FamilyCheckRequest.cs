using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RevitAi.Revit.Revit;

public sealed class FamilyCheckRequest
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private string string_0 = string.Empty;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private readonly TaskCompletionSource<bool> taskCompletionSource_0 = new TaskCompletionSource<bool>();

	public string FamilyName
	{
		[CompilerGenerated]
		get
		{
			return string_0;
		}
		[CompilerGenerated]
		set
		{
			string_0 = value;
		}
	}

	public TaskCompletionSource<bool> TaskSource
	{
		[CompilerGenerated]
		get
		{
			return taskCompletionSource_0;
		}
	}
}
