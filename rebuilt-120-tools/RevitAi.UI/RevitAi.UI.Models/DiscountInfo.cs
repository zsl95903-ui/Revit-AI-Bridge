namespace RevitAi.UI.Models;

public class DiscountInfo
{
	public decimal DiscountRate { get; set; }

	public string DiscountPercent { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public decimal OriginalPrice { get; set; }

	public decimal DiscountedPrice { get; set; }

	public decimal SavedAmount { get; set; }

	public int OriginalCredits { get; set; }

	public int ActualCredits { get; set; }

	public int BonusCredits { get; set; }
}
