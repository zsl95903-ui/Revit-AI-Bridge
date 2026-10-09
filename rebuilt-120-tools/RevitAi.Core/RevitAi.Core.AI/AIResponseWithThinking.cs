using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace RevitAi.Core.AI;

public sealed class AIResponseWithThinking
{
	[CompilerGenerated]
	private string? string_0;

	[CompilerGenerated]
	private string? string_1;

	[CompilerGenerated]
	private List<ToolCallInfo>? list_0;

	[CompilerGenerated]
	private int int_0;

	[CompilerGenerated]
	private int int_1;

	[CompilerGenerated]
	private int int_2;

	[JsonProperty("reasoning_content")]
	public string? ReasoningContent
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

	[JsonProperty("content")]
	public string? Content
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

	[JsonIgnore]
	public bool HasThinking => !string.IsNullOrEmpty(ReasoningContent);

	[JsonProperty("tool_calls")]
	public List<ToolCallInfo>? ToolCalls
	{
		[CompilerGenerated]
		get
		{
			return list_0;
		}
		[CompilerGenerated]
		set
		{
			list_0 = value;
		}
	}

	[JsonIgnore]
	public int InputTokens
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

	[JsonIgnore]
	public int OutputTokens
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

	[JsonIgnore]
	public int TotalTokens => InputTokens + OutputTokens;

	[JsonIgnore]
	public int CachedTokens
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

	[JsonIgnore]
	public double CacheHitRatio
	{
		get
		{
			if (InputTokens <= 0)
			{
				return 0.0;
			}
			return (double)CachedTokens / (double)InputTokens;
		}
	}
}
