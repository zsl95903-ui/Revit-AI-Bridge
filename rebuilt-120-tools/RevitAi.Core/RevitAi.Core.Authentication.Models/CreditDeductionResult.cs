using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace RevitAi.Core.Authentication.Models;

public sealed class CreditDeductionResult
{
	[CompilerGenerated]
	private bool bool_0;

	[CompilerGenerated]
	private string string_0 = string.Empty;

	[CompilerGenerated]
	private decimal decimal_0;

	[CompilerGenerated]
	private decimal decimal_1;

	[CompilerGenerated]
	private decimal decimal_2;

	[CompilerGenerated]
	private decimal decimal_3;

	[CompilerGenerated]
	private string string_1 = string.Empty;

	[JsonProperty("success")]
	public bool Success
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

	[JsonProperty("deducted_from")]
	public string DeductedFrom
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

	[JsonProperty("user_actual_deducted")]
	public decimal UserActualDeducted
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

	[JsonProperty("device_actual_deducted")]
	public decimal DeviceActualDeducted
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

	[JsonProperty("actual_deducted")]
	public decimal ActualDeducted
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

	[JsonProperty("remaining_balance")]
	public decimal RemainingBalance
	{
		[CompilerGenerated]
		get
		{
			return decimal_3;
		}
		[CompilerGenerated]
		set
		{
			decimal_3 = value;
		}
	}

	[JsonProperty("message")]
	public string Message
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
