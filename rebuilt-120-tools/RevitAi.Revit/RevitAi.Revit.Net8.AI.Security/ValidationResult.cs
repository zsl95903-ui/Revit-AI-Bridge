using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RevitAi.Revit.Net8.AI.Security;

public sealed class ValidationResult
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private readonly bool bool_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private readonly string? string_0;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly string? string_1;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly int? nullable_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private readonly string? string_2;

	public bool IsValid
	{
		[CompilerGenerated]
		get
		{
			return bool_0;
		}
	}

	public string? ErrorType
	{
		[CompilerGenerated]
		get
		{
			return string_0;
		}
	}

	public string? Error
	{
		[CompilerGenerated]
		get
		{
			return string_1;
		}
	}

	public int? LineNumber
	{
		[CompilerGenerated]
		get
		{
			return nullable_0;
		}
	}

	public string? CodeSnippet
	{
		[CompilerGenerated]
		get
		{
			return string_2;
		}
	}

	private ValidationResult(bool isValid, string? errorType = null, string? error = null, int? lineNumber = null, string? codeSnippet = null)
	{
		bool_0 = isValid;
		string_0 = errorType;
		string_1 = error;
		nullable_0 = lineNumber;
		string_2 = codeSnippet;
	}

	public static ValidationResult Valid()
	{
		return new ValidationResult(isValid: true);
	}

	public static ValidationResult Invalid(string error, string? errorType = "SecurityViolation", int? lineNumber = null, string? codeSnippet = null)
	{
		return new ValidationResult(isValid: false, errorType, error, lineNumber, codeSnippet);
	}
}
