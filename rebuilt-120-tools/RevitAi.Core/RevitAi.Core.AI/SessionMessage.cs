using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ns7;

namespace RevitAi.Core.AI;

public sealed class SessionMessage
{
	[CompilerGenerated]
	private string string_0 = "user";

	[CompilerGenerated]
	private string string_1 = string.Empty;

	[CompilerGenerated]
	private string? string_2;

	[CompilerGenerated]
	private string? string_3;

	[CompilerGenerated]
	private List<ToolCallData>? list_0;

	[CompilerGenerated]
	private DateTime dateTime_0 = DateTime.Now;

	[CompilerGenerated]
	private string string_4 = "User";

	[CompilerGenerated]
	private Dictionary<string, object>? dictionary_0;

	public string Role
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

	public string Content
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

	public string? ToolCallId
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

	public string? ReasoningContent
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

	public List<ToolCallData>? ToolCalls
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

	public DateTime Timestamp
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

	public string Kind
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
}
