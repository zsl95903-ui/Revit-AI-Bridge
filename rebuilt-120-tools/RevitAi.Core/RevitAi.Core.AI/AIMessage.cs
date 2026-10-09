using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.AI;
using ns7;

namespace RevitAi.Core.AI;

public sealed class AIMessage
{
	[CompilerGenerated]
	private string string_0 = "user";

	[CompilerGenerated]
	private string string_1 = string.Empty;

	[CompilerGenerated]
	private List<FileAttachment>? list_0;

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

	public List<FileAttachment>? Attachments
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
}
