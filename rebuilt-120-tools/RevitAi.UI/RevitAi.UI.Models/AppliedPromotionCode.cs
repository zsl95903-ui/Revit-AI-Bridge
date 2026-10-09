namespace RevitAi.UI.Models;

public class AppliedPromotionCode
{
	public string Code { get; set; } = string.Empty;

	public string DiscountType { get; set; } = string.Empty;

	public decimal DiscountValue { get; set; }

	public string Description { get; set; } = string.Empty;

	public decimal OriginalPrice { get; set; }

	public decimal DiscountedPrice { get; set; }

	public decimal SavedAmount { get; set; }
}
