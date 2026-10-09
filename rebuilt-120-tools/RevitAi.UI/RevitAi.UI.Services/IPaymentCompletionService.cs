using System.Threading.Tasks;
using RevitAi.Abstractions.Common;

namespace RevitAi.UI.Services;

public interface IPaymentCompletionService
{
	Task<Result> ProcessLicensePurchaseAsync(string orderId, int months, string paymentMethod, decimal? amount = null, string? transactionId = null);

	Task<Result> ProcessCreditRechargeAsync(string orderId, decimal amount, int credits, string paymentMethod, string? transactionId, decimal? originalPrice = null, int? originalCredits = null, decimal discountRate = 1.0m);
}
