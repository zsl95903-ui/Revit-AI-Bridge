using System.Threading.Tasks;

namespace RevitAi.UI.Services;

public interface IPaymentService
{
	Task<PaymentResult> CreatePaymentAsync(string productName, decimal amount, string orderId);

	Task<PaymentStatusResult> QueryPaymentStatusAsync(string orderId);
}
