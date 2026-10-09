using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RevitAi.Core.AI.Models;

public sealed class CodeSnippet
{
	[CompilerGenerated]
	private string string_0 = Guid.NewGuid().ToString();

	[CompilerGenerated]
	private string string_1 = string.Empty;

	[CompilerGenerated]
	private string string_2 = string.Empty;

	[CompilerGenerated]
	private string string_3 = string.Empty;

	[CompilerGenerated]
	private List<string> list_0 = new List<string>();

	[CompilerGenerated]
	private DateTime dateTime_0 = DateTime.Now;

	[CompilerGenerated]
	private DateTime dateTime_1 = DateTime.Now;

	[CompilerGenerated]
	private int int_0;

	[CompilerGenerated]
	private int int_1 = int.MaxValue;

	public string Id
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

	public string Name
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

	public string Description
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

	public string Code
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

	public List<string> Tags
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

	public DateTime LastUsed
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

	public int UseCount
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

	public int Order
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

	public string DisplayName => Name;
}
