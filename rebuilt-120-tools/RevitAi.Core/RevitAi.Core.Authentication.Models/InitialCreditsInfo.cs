using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace RevitAi.Core.Authentication.Models;

public sealed class InitialCreditsInfo
{
	[CompilerGenerated]
	private Guid guid_0;

	[CompilerGenerated]
	private decimal decimal_0;

	[CompilerGenerated]
	private decimal decimal_1;

	[CompilerGenerated]
	private string? string_0;

	[CompilerGenerated]
	private bool bool_0;

	[JsonProperty("id")]
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

	[JsonProperty("user_credit")]
	public decimal UserCredit
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

	[JsonProperty("device_credit")]
	public decimal DeviceCredit
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

	[JsonProperty("description")]
	public string? Description
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

	[JsonProperty("is_enabled")]
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
}
