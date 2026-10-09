using System.Runtime.CompilerServices;

namespace RevitAi.Core.Authentication.Models;

public class PromotionCodeValidationResult
{
	[CompilerGenerated]
	private bool bool_0;

	[CompilerGenerated]
	private PromotionCode? promotionCode_0;

	[CompilerGenerated]
	private string? string_0;

	[CompilerGenerated]
	private decimal? nullable_0;

	[CompilerGenerated]
	private decimal? nullable_1;

	public bool IsValid
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

	public PromotionCode? PromotionCode
	{
		[CompilerGenerated]
		get
		{
			return promotionCode_0;
		}
		[CompilerGenerated]
		set
		{
			promotionCode_0 = value;
		}
	}

	public string? ErrorMessage
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

	public decimal? DiscountedPrice
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

	public decimal? SavedAmount
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
}
