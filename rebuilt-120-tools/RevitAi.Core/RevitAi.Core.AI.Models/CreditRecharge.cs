using System;
using System.Runtime.CompilerServices;
using ns7;

namespace RevitAi.Core.AI.Models;

public sealed class CreditRecharge
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
	private string? string_0;

	[CompilerGenerated]
	private string? string_1;

	[CompilerGenerated]
	private string? string_2;

	[CompilerGenerated]
	private string string_3 = "pending";

	[CompilerGenerated]
	private string? string_4;

	[CompilerGenerated]
	private decimal? nullable_2;

	[CompilerGenerated]
	private decimal decimal_1;

	[CompilerGenerated]
	private decimal decimal_2;

	[CompilerGenerated]
	private DateTime? nullable_3;

	[CompilerGenerated]
	private DateTime dateTime_0;

	[CompilerGenerated]
	private DateTime dateTime_1;

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

	public decimal Amount
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

	public string? PaymentMethod
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

	public string? OrderId
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

	public string? TransactionId
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

	public string Status
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

	public string? FailureReason
	{
		[CompilerGenerated]
		get
		{
			return string_4;
		}
		[CompilerGenerated]
		set
		{
			string_4 = value;
		}
	}

	public decimal? OriginalAmount
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

	public decimal DiscountAmount
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

	public decimal BonusAmount
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

	public DateTime? PaidAt
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
}
