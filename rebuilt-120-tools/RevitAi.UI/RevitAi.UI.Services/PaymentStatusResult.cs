using System;

namespace RevitAi.UI.Services;

public class PaymentStatusResult
{
	public bool IsSuccess { get; set; }

	public string? Status { get; set; }

	public string? TransactionId { get; set; }

	public string? PaymentMethod { get; set; }

	public decimal? PaidAmount { get; set; }

	public DateTime? PaidAt { get; set; }

	public string? Error { get; set; }
}
