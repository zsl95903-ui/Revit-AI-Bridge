using System.Runtime.CompilerServices;

namespace RevitAi.Core.Authentication;

public class AuthorizationResult
{
	[CompilerGenerated]
	private readonly bool bool_0;

	[CompilerGenerated]
	private readonly string string_0;

	[CompilerGenerated]
	private readonly int int_0;

	public bool IsGranted
	{
		[CompilerGenerated]
		get
		{
			return bool_0;
		}
	}

	public string Reason
	{
		[CompilerGenerated]
		get
		{
			return string_0;
		}
	}

	public int RemainingCount
	{
		[CompilerGenerated]
		get
		{
			return int_0;
		}
	}

	private AuthorizationResult(bool isGranted, string reason, int remainingCount = -1)
	{
		bool_0 = isGranted;
		string_0 = reason;
		int_0 = remainingCount;
	}

	public static AuthorizationResult Granted(string reason, int remainingCount = -1)
	{
		return new AuthorizationResult(isGranted: true, reason, remainingCount);
	}

	public static AuthorizationResult Denied(string reason)
	{
		return new AuthorizationResult(isGranted: false, reason);
	}
}
