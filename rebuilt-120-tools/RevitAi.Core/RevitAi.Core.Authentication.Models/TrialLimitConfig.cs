using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using ns7;

namespace RevitAi.Core.Authentication.Models;

public class TrialLimitConfig
{
	[CompilerGenerated]
	private Guid guid_0;

	[CompilerGenerated]
	private string? string_0;

	[CompilerGenerated]
	private string? string_1;

	[CompilerGenerated]
	private int int_0;

	[CompilerGenerated]
	private bool bool_0;

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

	[JsonProperty("feature_group")]
	public string? FeatureGroup
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

	[JsonProperty("feature_id")]
	public string? FeatureId
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

	[JsonProperty("max_usage_count")]
	public int MaxUsageCount
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

	[JsonProperty("is_enabled")]
	public bool IsEnabled
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

	public int Priority
	{
		get
		{
			if (string.IsNullOrEmpty(FeatureId))
			{
				return (!string.IsNullOrEmpty(FeatureGroup)) ? 1 : 0;
			}
			return 2;
		}
	}

	public bool IsGroupConfig
	{
		get
		{
			if (!string.IsNullOrEmpty(FeatureGroup))
			{
				return string.IsNullOrEmpty(FeatureId);
			}
			return false;
		}
	}

	public bool IsFeatureConfig => !string.IsNullOrEmpty(FeatureId);

	public override string ToString()
	{
		if (IsFeatureConfig)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 2);
			defaultInterpolatedStringHandler.AppendLiteral("功能: ");
			defaultInterpolatedStringHandler.AppendFormatted(FeatureId);
			defaultInterpolatedStringHandler.AppendLiteral(", 最大试用: ");
			defaultInterpolatedStringHandler.AppendFormatted(MaxUsageCount);
			defaultInterpolatedStringHandler.AppendLiteral(" 次");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
		if (IsGroupConfig)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(15, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("功能组: ");
			defaultInterpolatedStringHandler2.AppendFormatted(FeatureGroup);
			defaultInterpolatedStringHandler2.AppendLiteral(", 最大试用: ");
			defaultInterpolatedStringHandler2.AppendFormatted(MaxUsageCount);
			defaultInterpolatedStringHandler2.AppendLiteral(" 次");
			return defaultInterpolatedStringHandler2.ToStringAndClear();
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(8, 1);
		defaultInterpolatedStringHandler3.AppendLiteral("默认配置: ");
		defaultInterpolatedStringHandler3.AppendFormatted(MaxUsageCount);
		defaultInterpolatedStringHandler3.AppendLiteral(" 次");
		return defaultInterpolatedStringHandler3.ToStringAndClear();
	}
}
