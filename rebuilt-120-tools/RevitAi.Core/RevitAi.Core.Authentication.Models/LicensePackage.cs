using System;
using System.Runtime.CompilerServices;

namespace RevitAi.Core.Authentication.Models;

public class LicensePackage
{
	[CompilerGenerated]
	private Guid guid_0;

	[CompilerGenerated]
	private int int_0;

	[CompilerGenerated]
	private decimal decimal_0;

	[CompilerGenerated]
	private string string_0 = string.Empty;

	[CompilerGenerated]
	private string? string_1;

	[CompilerGenerated]
	private bool bool_0;

	[CompilerGenerated]
	private int int_1;

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

	public int Months
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

	public decimal Price
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

	public string DisplayName
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

	public string? Description
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

	public bool IsEnabled
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

	public int SortOrder
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
