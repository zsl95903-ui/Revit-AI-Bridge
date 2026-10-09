using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RevitAi.Core.AI;

public sealed class ToolCallInfo
{
	[CompilerGenerated]
	private string string_0 = string.Empty;

	[CompilerGenerated]
	private IDictionary<string, object>? idictionary_0;

	[CompilerGenerated]
	private string? string_1;

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

	public IDictionary<string, object>? Parameters
	{
		[CompilerGenerated]
		get
		{
			return idictionary_0;
		}
		[CompilerGenerated]
		set
		{
			idictionary_0 = value;
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
}
