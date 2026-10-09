using System;
using System.Runtime.CompilerServices;

namespace RevitAi.Core.Authentication.Models;

public sealed class UserCreditsInfo
{
	[CompilerGenerated]
	private Guid guid_0;

	[CompilerGenerated]
	private Guid? nullable_0;

	[CompilerGenerated]
	private Guid? nullable_1;

	[CompilerGenerated]
	private decimal decimal_0;

	[CompilerGenerated]
	private decimal decimal_1;

	[CompilerGenerated]
	private decimal decimal_2;

	[CompilerGenerated]
	private DateTime? nullable_2;

	[CompilerGenerated]
	private DateTime? nullable_3;

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

	public Guid? UserId
	{
		[CompilerGenerated]
		get
		{
			return nullable_0;
		}
		[CompilerGenerated]
		set
		{
			nullable_0 = value;
		}
	}

	public Guid? DeviceId
	{
		[CompilerGenerated]
		get
		{
			return nullable_1;
		}
		[CompilerGenerated]
		set
		{
			nullable_1 = value;
		}
	}

	public decimal Balance
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

	public decimal TotalPurchased
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

	public decimal TotalConsumed
	{
		[CompilerGenerated]
		get
		{
			return decimal_2;
		}
		[CompilerGenerated]
		set
		{
			decimal_2 = value;
		}
	}

	public DateTime? LastRechargeAt
	{
		[CompilerGenerated]
		get
		{
			return nullable_2;
		}
		[CompilerGenerated]
		set
		{
			nullable_2 = value;
		}
	}

	public DateTime? LastUsageAt
	{
		[CompilerGenerated]
		get
		{
			return nullable_3;
		}
		[CompilerGenerated]
		set
		{
			nullable_3 = value;
		}
	}
}
