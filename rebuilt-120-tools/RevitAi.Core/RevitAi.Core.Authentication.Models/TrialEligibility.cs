using System.Runtime.CompilerServices;
using ns7;

namespace RevitAi.Core.Authentication.Models;

public sealed class TrialEligibility
{
	[CompilerGenerated]
	private bool bool_0;

	[CompilerGenerated]
	private int int_0;

	[CompilerGenerated]
	private int int_1;

	[CompilerGenerated]
	private string string_0 = string.Empty;

	[CompilerGenerated]
	private bool bool_1;

	public bool CanUse
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

	public int RemainingCount
	{
		[CompilerGenerated]
		get
		{
			return int_0;
		}
		[CompilerGenerated]
		set
		{
			int_0 = value;
		}
	}

	public int MaxUsageCount
	{
		[CompilerGenerated]
		get
		{
			return int_1;
		}
		[CompilerGenerated]
		set
		{
			int_1 = value;
		}
	}

	public string Reason
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

	public bool IsFirstUse
	{
		[CompilerGenerated]
		get
		{
			return bool_1;
		}
		[CompilerGenerated]
		set
		{
			bool_1 = value;
		}
	}

	public double UsagePercentage
	{
		get
		{
			if (MaxUsageCount == 0)
			{
				return 100.0;
			}
			return (double)(MaxUsageCount - RemainingCount) / (double)MaxUsageCount * 100.0;
		}
	}

	public string GetStatusMessage()
	{
		if (IsFirstUse)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
			defaultInterpolatedStringHandler.AppendLiteral("首次使用，默认试用次数：");
			defaultInterpolatedStringHandler.AppendFormatted(RemainingCount);
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
		if (!CanUse)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(24, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("试用次数已用尽（已用 ");
			defaultInterpolatedStringHandler2.AppendFormatted(MaxUsageCount - RemainingCount);
			defaultInterpolatedStringHandler2.AppendLiteral("/");
			defaultInterpolatedStringHandler2.AppendFormatted(MaxUsageCount);
			defaultInterpolatedStringHandler2.AppendLiteral(" 次），请购买授权或登录");
			return defaultInterpolatedStringHandler2.ToStringAndClear();
		}
		if ((double)RemainingCount <= (double)MaxUsageCount * 0.2)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(15, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("试用次数即将用尽（剩余 ");
			defaultInterpolatedStringHandler3.AppendFormatted(RemainingCount);
			defaultInterpolatedStringHandler3.AppendLiteral(" 次）");
			return defaultInterpolatedStringHandler3.ToStringAndClear();
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(13, 1);
		defaultInterpolatedStringHandler4.AppendLiteral("可以继续使用（剩余 ");
		defaultInterpolatedStringHandler4.AppendFormatted(RemainingCount);
		defaultInterpolatedStringHandler4.AppendLiteral(" 次）");
		return defaultInterpolatedStringHandler4.ToStringAndClear();
	}
}
