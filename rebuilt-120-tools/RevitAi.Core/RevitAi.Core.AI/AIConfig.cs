using System.Runtime.CompilerServices;
using RevitAi.Abstractions.AI;
using Newtonsoft.Json;
using ns7;

namespace RevitAi.Core.AI;

public sealed class AIConfig
{
	[CompilerGenerated]
	private string string_0 = string.Empty;

	[CompilerGenerated]
	private string? string_1;

	[CompilerGenerated]
	private string string_2 = string.Empty;

	[CompilerGenerated]
	private string string_3 = string.Empty;

	[CompilerGenerated]
	private string? string_4;

	[CompilerGenerated]
	private bool bool_0;

	[CompilerGenerated]
	private bool bool_1 = true;

	[CompilerGenerated]
	private int int_0 = 4096;

	[CompilerGenerated]
	private double double_0 = 1.0;

	[CompilerGenerated]
	private int int_1;

	[CompilerGenerated]
	private bool bool_2;

	[CompilerGenerated]
	private string? string_5 = "v1";

	[CompilerGenerated]
	private bool bool_3 = true;

	[CompilerGenerated]
	private bool bool_4;

	[CompilerGenerated]
	private int int_2 = 200000;

	[CompilerGenerated]
	private decimal? nullable_0;

	[CompilerGenerated]
	private string? string_6;

	[CompilerGenerated]
	private bool bool_5 = true;

	[CompilerGenerated]
	private AIProviderFileCapability? aiproviderFileCapability_0;

	[JsonProperty("provider")]
	public string Provider
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

	[JsonProperty("provider_endpoint")]
	public string? ProviderEndpoint
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

	[JsonProperty("model_name")]
	public string ModelName
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

	[JsonProperty("display_name")]
	public string DisplayName
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

	[JsonProperty("description")]
	public string? Description
	{
		[CompilerGenerated]
		get
		{
			return string_4;
		}
		[CompilerGenerated]
		set
		{
			string_4 = value;
		}
	}

	[JsonProperty("is_default")]
	public bool IsDefault
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

	[JsonProperty("is_active")]
	public bool IsActive
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

	[JsonProperty("max_tokens")]
	public int MaxTokens
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

	[JsonProperty("temperature")]
	public double Temperature
	{
		[CompilerGenerated]
		get
		{
			return double_0;
		}
		[CompilerGenerated]
		set
		{
			double_0 = value;
		}
	}

	[JsonProperty("priority")]
	public int Priority
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

	[JsonProperty("requires_premium")]
	public bool RequiresPremium
	{
		[CompilerGenerated]
		get
		{
			return bool_2;
		}
		[CompilerGenerated]
		set
		{
			bool_2 = value;
		}
	}

	[JsonProperty("api_version")]
	public string? ApiVersion
	{
		[CompilerGenerated]
		get
		{
			return string_5;
		}
		[CompilerGenerated]
		set
		{
			string_5 = value;
		}
	}

	[JsonProperty("supports_streaming")]
	public bool SupportsStreaming
	{
		[CompilerGenerated]
		get
		{
			return bool_3;
		}
		[CompilerGenerated]
		set
		{
			bool_3 = value;
		}
	}

	[JsonProperty("supports_vision")]
	public bool SupportsVision
	{
		[CompilerGenerated]
		get
		{
			return bool_4;
		}
		[CompilerGenerated]
		set
		{
			bool_4 = value;
		}
	}

	[JsonProperty("context_window")]
	public int ContextWindow
	{
		[CompilerGenerated]
		get
		{
			return int_2;
		}
		[CompilerGenerated]
		set
		{
			int_2 = value;
		}
	}

	[JsonProperty("cost_per_1k_tokens")]
	public decimal? CostPer1kTokens
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

	[JsonIgnore]
	public string? CachedApiKey
	{
		[CompilerGenerated]
		get
		{
			return string_6;
		}
		[CompilerGenerated]
		set
		{
			string_6 = value;
		}
	}

	[JsonIgnore]
	public bool IsUserConfig
	{
		[CompilerGenerated]
		get
		{
			return bool_5;
		}
		[CompilerGenerated]
		set
		{
			bool_5 = value;
		}
	}

	[JsonIgnore]
	public string SourceLabel
	{
		get
		{
			if (!IsUserConfig)
			{
				return "系统提供";
			}
			return "用户配置";
		}
	}

	[JsonIgnore]
	public string SourceColor
	{
		get
		{
			if (!IsUserConfig)
			{
				return "#FF2196F3";
			}
			return "#FF4CAF50";
		}
	}

	[JsonIgnore]
	public string? PriceDisplayText
	{
		get
		{
			if (CostPer1kTokens.HasValue && !(CostPer1kTokens.Value <= 0m))
			{
				decimal value = CostPer1kTokens.Value * 2m;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendFormatted(value, "F3");
				defaultInterpolatedStringHandler.AppendLiteral(" 电量/kToken");
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
			return null;
		}
	}

	[JsonIgnore]
	public string? VisionLabel
	{
		get
		{
			if (!SupportsVision)
			{
				return null;
			}
			return "视觉";
		}
	}

	[JsonIgnore]
	public string VisionColor => "#FF9C27B0";

	[JsonIgnore]
	public AIProviderFileCapability? FileCapability
	{
		[CompilerGenerated]
		get
		{
			return aiproviderFileCapability_0;
		}
		[CompilerGenerated]
		set
		{
			aiproviderFileCapability_0 = value;
		}
	}
}
