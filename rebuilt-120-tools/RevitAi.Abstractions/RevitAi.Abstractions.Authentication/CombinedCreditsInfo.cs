using System;
using Newtonsoft.Json;

namespace RevitAi.Abstractions.Authentication;

public sealed class CombinedCreditsInfo
{
	[JsonProperty("total_balance")]
	public decimal TotalBalance { get; set; }

	[JsonProperty("user_balance")]
	public decimal UserBalance { get; set; }

	[JsonProperty("device_balance")]
	public decimal DeviceBalance { get; set; }

	[JsonProperty("total_purchased")]
	public decimal TotalPurchased { get; set; }

	[JsonProperty("total_consumed")]
	public decimal TotalConsumed { get; set; }

	[JsonProperty("user_purchased")]
	public decimal UserPurchased { get; set; }

	[JsonProperty("user_consumed")]
	public decimal UserConsumed { get; set; }

	[JsonProperty("device_purchased")]
	public decimal DevicePurchased { get; set; }

	[JsonProperty("device_consumed")]
	public decimal DeviceConsumed { get; set; }

	[JsonProperty("user_id")]
	public Guid? UserId { get; set; }

	[JsonProperty("device_id")]
	public Guid? DeviceId { get; set; }

	[JsonProperty("user_last_recharge_at")]
	public string? UserLastRechargeAt { get; set; }

	[JsonProperty("device_last_recharge_at")]
	public string? DeviceLastRechargeAt { get; set; }

	[JsonProperty("user_last_usage_at")]
	public string? UserLastUsageAt { get; set; }

	[JsonProperty("device_last_usage_at")]
	public string? DeviceLastUsageAt { get; set; }
}
