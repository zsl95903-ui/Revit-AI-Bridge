using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RevitAi.Core.AI;

public sealed class ToolCallData
{
	[CompilerGenerated]
	private string string_0 = string.Empty;

	[CompilerGenerated]
	private Dictionary<string, object> dictionary_0 = new Dictionary<string, object>();

	[CompilerGenerated]
	private string? string_1;

	[CompilerGenerated]
	private string? string_2;

	[CompilerGenerated]
	private bool bool_0;

	[CompilerGenerated]
	private string? string_3;

	public string ToolName
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

	public Dictionary<string, object> Parameters
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

	public string? CallId
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

	public string? Result
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

	public bool IsSuccess
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

	public string? Error
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
}
