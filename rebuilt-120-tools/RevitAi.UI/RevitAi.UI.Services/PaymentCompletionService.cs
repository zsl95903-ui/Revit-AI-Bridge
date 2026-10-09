using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using Newtonsoft.Json;

namespace RevitAi.UI.Services;

public class PaymentCompletionService : IPaymentCompletionService
{
	private class LicenseCreationResult
	{
		public bool success { get; set; }

		public Guid? license_id { get; set; }

		public string? license_key { get; set; }

		public DateTime? valid_from { get; set; }

		public DateTime? valid_to { get; set; }

		public bool? is_permanent { get; set; }

		public string? message { get; set; }
	}

	private readonly ISupabaseClientProxy _supabaseProxy;

	private readonly IAuthManager _authManager;

	private const int HOURS_PER_MONTH = 744;

	private const int PERMANENT_MONTHS_THRESHOLD = 999;

	public PaymentCompletionService(ISupabaseClientProxy supabaseProxy, IAuthManager authManager)
	{
		_supabaseProxy = supabaseProxy ?? throw new ArgumentNullException("supabaseProxy");
		_authManager = authManager ?? throw new ArgumentNullException("authManager");
	}

	public async Task<Result> ProcessLicensePurchaseAsync(string orderId, int months, string paymentMethod, decimal? amount = null, string? transactionId = null)
	{
		Guid? userId = null;
		Guid? deviceId = null;
		object requestBody = null;
		try
		{
			Result<IDeviceInfo> result = await _authManager.EnsureDeviceRegisteredAsync();
			if (!result.IsSuccess || result.Value == null)
			{
				return Result.Failure(result.Error ?? "获取设备信息失败");
			}
			deviceId = result.Value.Id;
			Result<IUserIdentity> result2 = await _authManager.GetCurrentUserAsync();
			if (result2.IsSuccess && result2.Value != null)
			{
				userId = result2.Value.UserId;
			}
			int durationHours = ConvertMonthsToHours(months);
			requestBody = new
			{
				p_user_id = userId,
				p_device_id = ((!userId.HasValue) ? deviceId : ((Guid?)null)),
				p_order_id = orderId,
				p_months = months,
				p_duration_hours = durationHours,
				p_payment_method = paymentMethod
			};
			StringContent content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
			HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/create_purchase_license");
			httpRequestMessage.Content = content;
			if (!string.IsNullOrEmpty(_supabaseProxy.ApiKey))
			{
				httpRequestMessage.Headers.Add("Authorization", "Bearer " + _supabaseProxy.ApiKey);
				httpRequestMessage.Headers.Add("apikey", _supabaseProxy.ApiKey);
			}
			HttpResponseMessage response = await _supabaseProxy.HttpClient.SendAsync(httpRequestMessage);
			string value = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				return Result.Failure($"创建授权失败（HTTP {(int)response.StatusCode}）：{value}");
			}
			LicenseCreationResult[] array = JsonConvert.DeserializeObject<LicenseCreationResult[]>(value);
			if (array != null && array.Length != 0)
			{
				LicenseCreationResult licenseResult = array[0];
				if (licenseResult.success)
				{
					await LogLicensePurchaseAsync(orderId, userId, deviceId ?? Guid.Empty, paymentMethod, amount, months, durationHours, transactionId, JsonConvert.SerializeObject(requestBody, Formatting.Indented), JsonConvert.SerializeObject(array, Formatting.Indented), rpcSuccess: true, null);
					return Result.Success();
				}
				await LogLicensePurchaseAsync(orderId, userId, deviceId ?? Guid.Empty, paymentMethod, amount, months, durationHours, transactionId, JsonConvert.SerializeObject(requestBody, Formatting.Indented), JsonConvert.SerializeObject(array, Formatting.Indented), rpcSuccess: false, licenseResult.message);
				return Result.Failure(licenseResult.message ?? "创建授权失败");
			}
			return Result.Failure("无效的响应");
		}
		catch (Exception ex)
		{
			try
			{
				int durationHours2 = ConvertMonthsToHours(months);
				await LogLicensePurchaseAsync(orderId, userId, deviceId ?? Guid.Empty, paymentMethod, amount, months, durationHours2, transactionId, (requestBody != null) ? JsonConvert.SerializeObject(requestBody, Formatting.Indented) : null, null, rpcSuccess: false, ex.Message);
			}
			catch
			{
			}
			return Result.Failure("处理授权购买异常：" + ex.Message);
		}
	}

	public async Task<Result> ProcessCreditRechargeAsync(string orderId, decimal amount, int credits, string paymentMethod, string? transactionId, decimal? originalPrice = null, int? originalCredits = null, decimal discountRate = 1.0m)
	{
		Guid? userId = null;
		Guid? deviceId = null;
		object requestBody = null;
		try
		{
			Result<IDeviceInfo> result = await _authManager.EnsureDeviceRegisteredAsync();
			if (!result.IsSuccess || result.Value == null)
			{
				return Result.Failure(result.Error ?? "获取设备信息失败");
			}
			deviceId = result.Value.Id;
			Result<IUserIdentity> result2 = await _authManager.GetCurrentUserAsync();
			if (result2.IsSuccess && result2.Value != null)
			{
				userId = result2.Value.UserId;
			}
			requestBody = new
			{
				p_user_id = userId,
				p_device_id = ((!userId.HasValue) ? deviceId : ((Guid?)null)),
				p_order_id = orderId,
				p_amount = amount,
				p_credits = credits,
				p_payment_method = paymentMethod,
				p_transaction_id = transactionId,
				p_original_price = originalPrice,
				p_original_credits = originalCredits,
				p_discount_rate = discountRate
			};
			StringContent content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
			HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/create_credit_recharge");
			httpRequestMessage.Content = content;
			if (!string.IsNullOrEmpty(_supabaseProxy.ApiKey))
			{
				httpRequestMessage.Headers.Add("Authorization", "Bearer " + _supabaseProxy.ApiKey);
				httpRequestMessage.Headers.Add("apikey", _supabaseProxy.ApiKey);
			}
			HttpResponseMessage response = await _supabaseProxy.HttpClient.SendAsync(httpRequestMessage);
			string text = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				return Result.Failure($"创建充值记录失败（HTTP {(int)response.StatusCode}）：{text}");
			}
			if (Guid.TryParse(text.Trim('"'), out var result3))
			{
				Logger.Debug($"[PaymentCompletion] 充值记录创建成功，ID: {result3}");
				await LogCreditRechargeAsync(orderId, userId, deviceId ?? Guid.Empty, paymentMethod, amount, credits, transactionId, originalPrice, originalCredits, discountRate, JsonConvert.SerializeObject(requestBody, Formatting.Indented), JsonConvert.SerializeObject(new
				{
					recharge_id = result3
				}, Formatting.Indented), rpcSuccess: true, null);
				return Result.Success();
			}
			string errorMsg = text;
			if (!string.IsNullOrEmpty(errorMsg) && errorMsg.Length > 100)
			{
				errorMsg = errorMsg.Substring(0, 100) + "...";
			}
			await LogCreditRechargeAsync(orderId, userId, deviceId ?? Guid.Empty, paymentMethod, amount, credits, transactionId, originalPrice, originalCredits, discountRate, JsonConvert.SerializeObject(requestBody, Formatting.Indented), text, rpcSuccess: false, errorMsg);
			return Result.Failure("创建充值记录失败：" + errorMsg);
		}
		catch (Exception ex)
		{
			try
			{
				await LogCreditRechargeAsync(orderId, userId, deviceId ?? Guid.Empty, paymentMethod, amount, credits, transactionId, originalPrice, originalCredits, discountRate, (requestBody != null) ? JsonConvert.SerializeObject(requestBody, Formatting.Indented) : null, null, rpcSuccess: false, ex.Message);
			}
			catch
			{
			}
			return Result.Failure("处理电量充值异常：" + ex.Message);
		}
	}

	private int ConvertMonthsToHours(int months)
	{
		if (months >= 999)
		{
			return -1;
		}
		return months * 744;
	}

	private async Task LogLicensePurchaseAsync(string orderId, Guid? userId, Guid deviceId, string paymentMethod, decimal? amount, int months, int durationHours, string? transactionId, object? rpcRequest, string? rpcResponse, bool rpcSuccess, string? rpcErrorMessage)
	{
		_ = 1;
		try
		{
			StringContent content = new StringContent(JsonConvert.SerializeObject(new
			{
				p_order_id = orderId,
				p_user_id = userId,
				p_device_id = deviceId,
				p_payment_method = paymentMethod,
				p_amount = amount,
				p_months = months,
				p_duration_hours = durationHours,
				p_transaction_id = transactionId,
				p_original_price = (decimal?)null,
				p_original_credits = (int?)null,
				p_discount_rate = 1.0m,
				p_rpc_function = "create_purchase_license",
				p_rpc_request = ((rpcRequest != null) ? JsonConvert.SerializeObject(rpcRequest) : null),
				p_rpc_response = rpcResponse,
				p_rpc_success = rpcSuccess,
				p_rpc_error_message = rpcErrorMessage
			}), Encoding.UTF8, "application/json");
			HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/log_license_purchase");
			httpRequestMessage.Content = content;
			if (!string.IsNullOrEmpty(_supabaseProxy.ApiKey))
			{
				httpRequestMessage.Headers.Add("Authorization", "Bearer " + _supabaseProxy.ApiKey);
				httpRequestMessage.Headers.Add("apikey", _supabaseProxy.ApiKey);
			}
			HttpResponseMessage httpResponseMessage = await _supabaseProxy.HttpClient.SendAsync(httpRequestMessage);
			if (!httpResponseMessage.IsSuccessStatusCode)
			{
				Logger.Warning("[PaymentLog] 记录授权购买日志失败: " + await httpResponseMessage.Content.ReadAsStringAsync());
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[PaymentLog] 记录授权购买日志异常", ex);
		}
	}

	private async Task LogCreditRechargeAsync(string orderId, Guid? userId, Guid deviceId, string paymentMethod, decimal amount, int credits, string? transactionId, decimal? originalPrice, int? originalCredits, decimal discountRate, string? rpcRequest, string? rpcResponse, bool rpcSuccess, string? rpcErrorMessage)
	{
		_ = 1;
		try
		{
			StringContent content = new StringContent(JsonConvert.SerializeObject(new
			{
				p_order_id = orderId,
				p_user_id = userId,
				p_device_id = deviceId,
				p_payment_method = paymentMethod,
				p_amount = amount,
				p_credits = credits,
				p_transaction_id = transactionId,
				p_original_price = originalPrice,
				p_original_credits = originalCredits,
				p_discount_rate = discountRate,
				p_rpc_function = "create_credit_recharge",
				p_rpc_request = rpcRequest,
				p_rpc_response = rpcResponse,
				p_rpc_success = rpcSuccess,
				p_rpc_error_message = rpcErrorMessage
			}), Encoding.UTF8, "application/json");
			HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/log_credit_recharge");
			httpRequestMessage.Content = content;
			if (!string.IsNullOrEmpty(_supabaseProxy.ApiKey))
			{
				httpRequestMessage.Headers.Add("Authorization", "Bearer " + _supabaseProxy.ApiKey);
				httpRequestMessage.Headers.Add("apikey", _supabaseProxy.ApiKey);
			}
			HttpResponseMessage httpResponseMessage = await _supabaseProxy.HttpClient.SendAsync(httpRequestMessage);
			if (!httpResponseMessage.IsSuccessStatusCode)
			{
				Logger.Warning("[PaymentLog] 记录电量充值日志失败: " + await httpResponseMessage.Content.ReadAsStringAsync());
			}
			else
			{
				Logger.Debug("[PaymentLog] 电量充值日志已记录: order_id=" + orderId);
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[PaymentLog] 记录电量充值日志异常", ex);
		}
	}
}
