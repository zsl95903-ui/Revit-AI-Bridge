using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using ns7;

namespace RevitAi.Core.Authentication.Models;

public sealed class TrialRecordDetail
{
	[CompilerGenerated]
	private Guid guid_0;

	[CompilerGenerated]
	private string string_0 = string.Empty;

	[CompilerGenerated]
	private Guid? nullable_0;

	[CompilerGenerated]
	private string string_1 = string.Empty;

	[CompilerGenerated]
	private string string_2 = string.Empty;

	[CompilerGenerated]
	private string string_3 = "v1.0";

	[CompilerGenerated]
	private int int_0;

	[CompilerGenerated]
	private int int_1 = 10;

	[CompilerGenerated]
	private DateTime dateTime_0;

	[CompilerGenerated]
	private DateTime dateTime_1;

	[JsonProperty("id")]
	public Guid Id
	{
		[CompilerGenerated]
		get
		{
			return guid_0;
		}
		[CompilerGenerated]
		set
		{
			guid_0 = value;
		}
	}

	[JsonProperty("device_id")]
	public string DeviceId
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

	[JsonProperty("user_id")]
	public Guid? UserId
	{
		[CompilerGenerated]
		get
		{
			return nullable_0;
		}
		[CompilerGenerated]
		set
		{
			nullable_0 = value;
		}
	}

	[JsonProperty("feature_group")]
	public string FeatureGroup
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

	[JsonProperty("feature_id")]
	public string FeatureId
	{
		[CompilerGenerated]
		get
		{
			return string_2;
		}
		[CompilerGenerated]
		set
		{
			string_2 = value;
		}
	}

	[JsonProperty("trial_version")]
	public string TrialVersion
	{
		[CompilerGenerated]
		get
		{
			return string_3;
		}
		[CompilerGenerated]
		set
		{
			string_3 = value;
		}
	}

	[JsonProperty("usage_count")]
	public int UsageCount
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

	[JsonProperty("max_usage_count")]
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

	[JsonProperty("remaining_count")]
	public int RemainingCount => MaxUsageCount - UsageCount;

	[JsonProperty("first_used_at")]
	public DateTime FirstUsedAt
	{
		[CompilerGenerated]
		get
		{
			return dateTime_0;
		}
		[CompilerGenerated]
		set
		{
			dateTime_0 = value;
		}
	}

	[JsonProperty("last_used_at")]
	public DateTime LastUsedAt
	{
		[CompilerGenerated]
		get
		{
			return dateTime_1;
		}
		[CompilerGenerated]
		set
		{
			dateTime_1 = value;
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
			return (double)UsageCount / (double)MaxUsageCount * 100.0;
		}
	}

	public TrialStatus Status
	{
		get
		{
			if (RemainingCount <= 0)
			{
				return TrialStatus.Exhausted;
			}
			if ((double)RemainingCount <= (double)MaxUsageCount * 0.2)
			{
				return TrialStatus.Low;
			}
			return TrialStatus.Available;
		}
	}

	public bool CanUse()
	{
		return RemainingCount > 0;
	}

	public string GetStatusMessage()
	{
		switch (Status)
		{
		default:
			return "未知状态";
		case TrialStatus.Available:
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(13, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("可以继续使用（剩余 ");
			defaultInterpolatedStringHandler3.AppendFormatted(RemainingCount);
			defaultInterpolatedStringHandler3.AppendLiteral(" 次）");
			return defaultInterpolatedStringHandler3.ToStringAndClear();
		}
		case TrialStatus.Low:
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(15, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("试用次数即将用尽（剩余 ");
			defaultInterpolatedStringHandler2.AppendFormatted(RemainingCount);
			defaultInterpolatedStringHandler2.AppendLiteral(" 次）");
			return defaultInterpolatedStringHandler2.ToStringAndClear();
		}
		case TrialStatus.Exhausted:
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 2);
			defaultInterpolatedStringHandler.AppendLiteral("试用次数已用尽（");
			defaultInterpolatedStringHandler.AppendFormatted(UsageCount);
			defaultInterpolatedStringHandler.AppendLiteral("/");
			defaultInterpolatedStringHandler.AppendFormatted(MaxUsageCount);
			defaultInterpolatedStringHandler.AppendLiteral("），请购买授权或登录");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
		}
	}
}
