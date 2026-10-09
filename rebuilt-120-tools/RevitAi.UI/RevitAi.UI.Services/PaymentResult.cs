namespace RevitAi.UI.Services;

public class PaymentResult
{
	public bool IsSuccess { get; set; }

	public string? AlipayQrCodeUrl { get; set; }

	public string? WechatQrCodeUrl { get; set; }

	public string? OrderId { get; set; }

	public string? Error { get; set; }
}
