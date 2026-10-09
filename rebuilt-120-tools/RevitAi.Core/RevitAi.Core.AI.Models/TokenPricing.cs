using System;
using System.Runtime.CompilerServices;
using ns7;

namespace RevitAi.Core.AI.Models;

public sealed class TokenPricing
{
	[CompilerGenerated]
	private Guid guid_0;

	[CompilerGenerated]
	private string string_0 = string.Empty;

	[CompilerGenerated]
	private string string_1 = string.Empty;

	[CompilerGenerated]
	private decimal decimal_0;

	[CompilerGenerated]
	private decimal decimal_1;

	[CompilerGenerated]
	private string string_2 = "CREDIT";

	[CompilerGenerated]
	private int int_0 = 1000;

	[CompilerGenerated]
	private bool bool_0;

	[CompilerGenerated]
	private DateTime dateTime_0;

	[CompilerGenerated]
	private string? string_3;

	[CompilerGenerated]
	private DateTime dateTime_1;

	[CompilerGenerated]
	private DateTime dateTime_2;

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

	public string ModelName
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

	public decimal InputPrice
	{
		[CompilerGenerated]
		get
		{
			return decimal_0;
		}
		[CompilerGenerated]
		set
		{
			decimal_0 = value;
		}
	}

	public decimal OutputPrice
	{
		[CompilerGenerated]
		get
		{
			return decimal_1;
		}
		[CompilerGenerated]
		set
		{
			decimal_1 = value;
		}
	}

	public string Currency
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

	public int UnitMultiplier
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

	public bool IsActive
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

	public DateTime EffectiveDate
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

	public string? Notes
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

	public DateTime CreatedAt
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

	public DateTime UpdatedAt
	{
		[CompilerGenerated]
		get
		{
			return dateTime_2;
		}
		[CompilerGenerated]
		set
		{
			dateTime_2 = value;
		}
	}
}
