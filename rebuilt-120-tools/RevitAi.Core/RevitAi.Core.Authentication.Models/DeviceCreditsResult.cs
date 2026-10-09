using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace RevitAi.Core.Authentication.Models;

public sealed class DeviceCreditsResult
{
	[CompilerGenerated]
	private Guid guid_0;

	[CompilerGenerated]
	private decimal decimal_0;

	[CompilerGenerated]
	private decimal decimal_1;

	[JsonProperty("device_db_id")]
	public Guid DeviceId
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

	[JsonProperty("initial_balance")]
	public decimal InitialBalance
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

	[JsonProperty("current_balance")]
	public decimal CurrentBalance
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
}
