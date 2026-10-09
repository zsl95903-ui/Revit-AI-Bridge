using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RevitAi.Revit.Revit;

public sealed class FamilyLoadResult
{
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool bool_0;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string? string_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private string? string_1;

	public bool Success
	{
		[CompilerGenerated]
		get
		{
			return bool_0;
		}
		[CompilerGenerated]
		set
		{
			bool_0 = value;
		}
	}

	public string? Error
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

	public string? FamilyId
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

	public static FamilyLoadResult Succeeded(string? familyId = null)
	{
		return new FamilyLoadResult
		{
			Success = true,
			FamilyId = familyId
		};
	}

	public static FamilyLoadResult Failed(string error)
	{
		return new FamilyLoadResult
		{
			Success = false,
			Error = error
		};
	}
}
