using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ns7;

namespace RevitAi.Core.AI.Models;

public sealed class SessionTokenAccumulator
{
	[CompilerGenerated]
	private string string_0 = Guid.NewGuid().ToString();

	[CompilerGenerated]
	private Guid? nullable_0;

	[CompilerGenerated]
	private Guid? nullable_1;

	[CompilerGenerated]
	private string string_1 = string.Empty;

	[CompilerGenerated]
	private string string_2 = string.Empty;

	[CompilerGenerated]
	private DateTime dateTime_0 = DateTime.Now;

	[CompilerGenerated]
	private DateTime? nullable_2;

	[CompilerGenerated]
	private int int_0;

	[CompilerGenerated]
	private int int_1;

	[CompilerGenerated]
	private int int_2;

	[CompilerGenerated]
	private int int_3;

	[CompilerGenerated]
	private int int_4;

	[CompilerGenerated]
	private bool bool_0;

	[CompilerGenerated]
	private int int_5;

	[CompilerGenerated]
	private Dictionary<string, object>? dictionary_0;

	[CompilerGenerated]
	private int int_6;

	private string? string_3;

	public string SessionId
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

	public Guid? DeviceId
	{
		[CompilerGenerated]
		get
		{
			return nullable_1;
		}
		[CompilerGenerated]
		set
		{
			nullable_1 = value;
		}
	}

	public string Provider
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

	public DateTime StartTime
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

	public DateTime? EndTime
	{
		[CompilerGenerated]
		get
		{
			return nullable_2;
		}
		[CompilerGenerated]
		set
		{
			nullable_2 = value;
		}
	}

	public int TotalInputTokens
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

	public int TotalOutputTokens
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

	public int TotalCachedTokens
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

	public int TotalTokens => TotalInputTokens + TotalOutputTokens;

	public int ToolCallCount
	{
		[CompilerGenerated]
		get
		{
			return int_3;
		}
		[CompilerGenerated]
		set
		{
			int_3 = value;
		}
	}

	public int RequestCount
	{
		[CompilerGenerated]
		get
		{
			return int_4;
		}
		[CompilerGenerated]
		set
		{
			int_4 = value;
		}
	}

	public bool IsToolCall
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

	public int TotalDurationMs
	{
		[CompilerGenerated]
		get
		{
			return int_5;
		}
		[CompilerGenerated]
		set
		{
			int_5 = value;
		}
	}

	public Dictionary<string, object>? Metadata
	{
		[CompilerGenerated]
		get
		{
			return dictionary_0;
		}
		[CompilerGenerated]
		set
		{
			dictionary_0 = value;
		}
	}

	public int EstimatedOutputTokens
	{
		[CompilerGenerated]
		get
		{
			return int_6;
		}
		[CompilerGenerated]
		set
		{
			int_6 = value;
		}
	}

	public void AddUsage(int inputTokens, int outputTokens, int durationMs, bool isToolCall = false, int cachedTokens = 0)
	{
		TotalInputTokens += inputTokens;
		TotalOutputTokens += outputTokens;
		TotalCachedTokens += cachedTokens;
		TotalDurationMs += durationMs;
		RequestCount++;
		if (isToolCall)
		{
			IsToolCall = true;
			ToolCallCount++;
		}
	}

	public int AddEstimatedUsageFromText(string generatedText)
	{
		if (string.IsNullOrEmpty(generatedText))
		{
			return 0;
		}
		string_3 = generatedText;
		return EstimatedOutputTokens = EstimateTokensFromText(generatedText);
	}

	public static int EstimateTokensFromText(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return 0;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < text.Length; i++)
		{
			if (smethod_0(text[i]))
			{
				num++;
			}
			else
			{
				num2++;
			}
		}
		int num3 = num;
		int num4 = (int)Math.Ceiling((double)num2 / 4.0);
		return num3 + num4;
	}

	private static bool smethod_0(char char_0)
	{
		if ((char_0 >= '一' && char_0 <= '鿿') || (char_0 >= '㐀' && char_0 <= '䶿'))
		{
			return true;
		}
		if (char_0 >= 131072)
		{
			return char_0 <= 173791;
		}
		return false;
	}

	public double GetTotalDurationSeconds()
	{
		if (EndTime.HasValue)
		{
			return (EndTime.Value - StartTime).TotalSeconds;
		}
		return (DateTime.Now - StartTime).TotalSeconds;
	}

	public TokenUsageRequest ToTokenUsageRequest()
	{
		int outputTokens = ((TotalOutputTokens > 0) ? TotalOutputTokens : EstimatedOutputTokens);
		return new TokenUsageRequest
		{
			UserId = UserId,
			DeviceId = DeviceId,
			SessionId = SessionId,
			Provider = Provider,
			ModelName = ModelName,
			InputTokens = TotalInputTokens,
			OutputTokens = outputTokens,
			RequestType = (IsToolCall ? "tool_call" : "chat"),
			IsToolCall = IsToolCall,
			ToolCount = ToolCallCount,
			RequestCount = RequestCount,
			DurationMs = TotalDurationMs,
			Metadata = Metadata
		};
	}
}
