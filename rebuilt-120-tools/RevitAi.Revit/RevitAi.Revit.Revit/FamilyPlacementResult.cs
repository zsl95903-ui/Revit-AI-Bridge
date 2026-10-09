using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RevitAi.Revit.Revit;

public sealed class FamilyPlacementResult
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private bool bool_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private string? string_0;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
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

	public string? FamilySymbolId
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

	public static FamilyPlacementResult Succeeded(string? familySymbolId = null)
	{
		return new FamilyPlacementResult
		{
			Success = true,
			FamilySymbolId = familySymbolId
		};
	}

	public static FamilyPlacementResult Failed(string error)
	{
		return new FamilyPlacementResult
		{
			Success = false,
			Error = error
		};
	}
}
