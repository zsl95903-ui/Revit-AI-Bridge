using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ns7;

namespace RevitAi.Core.AI;

public sealed class AIChatSession
{
	[CompilerGenerated]
	private string string_0 = Guid.NewGuid().ToString();

	[CompilerGenerated]
	private string string_1 = "新对话";

	[CompilerGenerated]
	private List<SessionMessage> list_0 = new List<SessionMessage>();

	[CompilerGenerated]
	private DateTime dateTime_0 = DateTime.Now;

	[CompilerGenerated]
	private DateTime dateTime_1 = DateTime.Now;

	[CompilerGenerated]
	private int int_0;

	[CompilerGenerated]
	private AIConfig? aiconfig_0;

	[CompilerGenerated]
	private bool bool_0;

	[CompilerGenerated]
	private List<string> list_1 = new List<string>();

	[CompilerGenerated]
	private bool bool_1;

	[CompilerGenerated]
	private string? string_2;

	[CompilerGenerated]
	private int int_1;

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

	public string Title
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

	public List<SessionMessage> Messages
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

	public DateTime CreatedAt
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

	public DateTime UpdatedAt
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

	public int RoundCount
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

	public AIConfig? AIConfig
	{
		[CompilerGenerated]
		get
		{
			return aiconfig_0;
		}
		[CompilerGenerated]
		set
		{
			aiconfig_0 = value;
		}
	}

	public bool IsArchived
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

	public List<string> Tags
	{
		[CompilerGenerated]
		get
		{
			return list_1;
		}
		[CompilerGenerated]
		set
		{
			list_1 = value;
		}
	}

	public bool HasRequestedTools
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

	public string? AccumulatedSummary
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

	public int SummaryMessageCount
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
}
