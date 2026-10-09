using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RevitAi.Revit.Revit;

public sealed class FamilyLoadRequest
{
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string string_0 = string.Empty;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private string string_1 = string.Empty;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly TaskCompletionSource<FamilyLoadResult> taskCompletionSource_0 = new TaskCompletionSource<FamilyLoadResult>();

	public string FilePath
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

	public string FamilyName
	{
		[CompilerGenerated]
		get
		{
			return string_1;
		}
		[CompilerGenerated]
		set
		{
			string_1 = value;
		}
	}

	public TaskCompletionSource<FamilyLoadResult> TaskSource
	{
		[CompilerGenerated]
		get
		{
			return taskCompletionSource_0;
		}
	}
}
