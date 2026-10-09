using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.UI.Models;

namespace RevitAi.UI.Services;

public class ZPayService : IPaymentService
{
	private readonly HttpClient _httpClient;

	private readonly ZPayConfig _config;

	public ZPayService(ZPayConfig config)
	{
		_httpClient = new HttpClient();
		_config = config;
	}

	public async Task<PaymentResult> CreatePaymentAsync(string productName, decimal amount, string orderId)
	{
		_ = 2;
		try
		{
			Task<(bool IsSuccess, string? QrCodeUrl, string? Error)> alipayTask = CreateAlipayOrderAsync(productName, amount, orderId);
			Task<(bool IsSuccess, string? QrCodeUrl, string? Error)> wechatTask = CreateWechatOrderAsync(productName, amount, orderId);
			InlineArray2<Task<(bool, string, string)>> buffer = default(InlineArray2<Task<(bool, string, string)>>);
			buffer[0] = alipayTask;
			buffer[1] = wechatTask;
			await Task.WhenAll<(bool, string, string)>(buffer);
			(bool IsSuccess, string? QrCodeUrl, string? Error) alipayResult = await alipayTask;
			(bool, string, string) tuple = await wechatTask;
			if (!alipayResult.IsSuccess && !tuple.Item1)
			{
				Logger.Debug("[ZPay] 两个支付方式都失败");
				return new PaymentResult
				{
					IsSuccess = false,
					Error = "创建支付订单失败。支付宝：" + alipayResult.Error + "，微信：" + tuple.Item3
				};
			}
			string alipayQrCodeUrl = (alipayResult.IsSuccess ? alipayResult.QrCodeUrl : GetFallbackQrCodeUrl(orderId, "alipay"));
			string wechatQrCodeUrl = (tuple.Item1 ? tuple.Item2 : GetFallbackQrCodeUrl(orderId, "wxpay"));
			return new PaymentResult
			{
				IsSuccess = true,
				OrderId = orderId,
				AlipayQrCodeUrl = alipayQrCodeUrl,
				WechatQrCodeUrl = wechatQrCodeUrl,
				Error = (alipayResult.IsSuccess ? null : alipayResult.Error)
			};
		}
		catch (Exception ex)
		{
			Logger.Debug("[ZPay] 创建订单异常: " + ex.Message);
			Logger.Debug("[ZPay] 异常堆栈: " + ex.StackTrace);
			return new PaymentResult
			{
				IsSuccess = false,
				Error = "创建支付订单异常：" + ex.Message
			};
		}
	}

	private async Task<(bool IsSuccess, string? QrCodeUrl, string? Error)> CreateAlipayOrderAsync(string productName, decimal amount, string orderId)
	{
		_ = 1;
		try
		{
			NameValueCollection nameValueCollection = new NameValueCollection
			{
				["pid"] = _config.MerchantId,
				["type"] = "alipay",
				["cid"] = _config.AlipayChannelId,
				["out_trade_no"] = orderId,
				["notify_url"] = "http://example.com/notify",
				["name"] = productName,
				["money"] = amount.ToString("F2"),
				["clientip"] = "127.0.0.1",
				["device"] = "pc",
				["param"] = ""
			};
			string value = GenerateSign(nameValueCollection);
			nameValueCollection["sign"] = value;
			nameValueCollection["sign_type"] = "MD5";
			return ParsePaymentResponse(await (await _httpClient.PostAsync(_config.ApiGateway, new FormUrlEncodedContent(ToDictionary(nameValueCollection)))).Content.ReadAsStringAsync(), "alipay", orderId);
		}
		catch (Exception ex)
		{
			return (IsSuccess: false, QrCodeUrl: null, Error: "支付宝订单创建失败：" + ex.Message);
		}
	}

	private async Task<(bool IsSuccess, string? QrCodeUrl, string? Error)> CreateWechatOrderAsync(string productName, decimal amount, string orderId)
	{
		_ = 1;
		try
		{
			string wechatOrderId = orderId + "-WX";
			NameValueCollection nameValueCollection = new NameValueCollection
			{
				["pid"] = _config.MerchantId,
				["type"] = "wxpay",
				["cid"] = _config.WechatChannelId,
				["out_trade_no"] = wechatOrderId,
				["notify_url"] = "http://example.com/notify",
				["name"] = productName,
				["money"] = amount.ToString("F2"),
				["clientip"] = "127.0.0.1",
				["device"] = "pc",
				["param"] = ""
			};
			string value = GenerateSign(nameValueCollection);
			nameValueCollection["sign"] = value;
			nameValueCollection["sign_type"] = "MD5";
			return ParsePaymentResponse(await (await _httpClient.PostAsync(_config.ApiGateway, new FormUrlEncodedContent(ToDictionary(nameValueCollection)))).Content.ReadAsStringAsync(), "wxpay", wechatOrderId);
		}
		catch (Exception ex)
		{
			return (IsSuccess: false, QrCodeUrl: null, Error: "微信订单创建失败：" + ex.Message);
		}
	}

	private (bool IsSuccess, string? QrCodeUrl, string? Error) ParsePaymentResponse(string responseContent, string payType, string orderId)
	{
		try
		{
			using JsonDocument jsonDocument = JsonDocument.Parse(responseContent);
			JsonElement rootElement = jsonDocument.RootElement;
			if (rootElement.TryGetProperty("code", out var value))
			{
				if (value.ValueKind == JsonValueKind.String && value.GetString() == "error")
				{
					string item = (rootElement.TryGetProperty("msg", out var value2) ? value2.GetString() : "创建支付订单失败");
					return (IsSuccess: false, QrCodeUrl: null, Error: item);
				}
				if (value.ValueKind == JsonValueKind.Number && value.GetInt32() != 1)
				{
					string item2 = (rootElement.TryGetProperty("msg", out var value3) ? value3.GetString() : "创建支付订单失败");
					return (IsSuccess: false, QrCodeUrl: null, Error: item2);
				}
			}
			string text = null;
			if (rootElement.TryGetProperty("img", out var value4))
			{
				text = value4.GetString();
			}
			if (string.IsNullOrEmpty(text) && rootElement.TryGetProperty("qrcode", out var value5))
			{
				text = value5.GetString();
			}
			if (string.IsNullOrEmpty(text) && rootElement.TryGetProperty("payurl", out var value6))
			{
				text = value6.GetString();
			}
			if (!string.IsNullOrEmpty(text))
			{
				return (IsSuccess: true, QrCodeUrl: text, Error: null);
			}
			Logger.Warning("[ZPay] API响应中未找到二维码URL: " + responseContent);
			return (IsSuccess: false, QrCodeUrl: null, Error: "响应中未找到二维码URL");
		}
		catch (JsonException ex)
		{
			Logger.Error("[ZPay] 解析JSON响应失败: " + ex.Message + ", 响应内容: " + responseContent);
			return (IsSuccess: false, QrCodeUrl: null, Error: "解析响应失败：" + ex.Message);
		}
	}

	private string GetFallbackQrCodeUrl(string orderId, string payType)
	{
		return "https://zpayz.cn/qrcode.php?orderid=" + orderId + "&type=" + payType;
	}

	private string GenerateSign(NameValueCollection parameters)
	{
		IOrderedEnumerable<string> source = parameters.AllKeys.Where((string k) => !string.IsNullOrEmpty(k) && k != "sign" && k != "sign_type" && !string.IsNullOrEmpty(parameters[k])).OrderBy<string, string>((string k) => k, StringComparer.Ordinal);
		string s = string.Join("&", source.Select((string k) => k + "=" + parameters[k])) + _config.MerchantKey;
		using MD5 mD = MD5.Create();
		return BitConverter.ToString(mD.ComputeHash(Encoding.UTF8.GetBytes(s))).Replace("-", "").ToLowerInvariant();
	}

	public async Task<PaymentStatusResult> QueryPaymentStatusAsync(string orderId)
	{
		_ = 5;
		try
		{
			string queryBaseUrl = _config.ApiGateway.Replace("mapi.php", "api.php");
			string orderId2 = orderId + "-WX";
			Task<PaymentStatusResult> alipayQueryTask = QuerySingleOrderAsync(queryBaseUrl, orderId, "alipay");
			Task<PaymentStatusResult> wechatQueryTask = QuerySingleOrderAsync(queryBaseUrl, orderId2, "wxpay");
			Task<PaymentStatusResult> completedTask = await Task.WhenAny(alipayQueryTask, wechatQueryTask);
			PaymentStatusResult paymentStatusResult = await completedTask;
			if (paymentStatusResult.IsSuccess && paymentStatusResult.Status == "success")
			{
				string text = ((completedTask == alipayQueryTask) ? "支付宝" : "微信");
				Logger.Debug("[ZPay] 检测到" + text + "支付成功，立即返回");
				return paymentStatusResult;
			}
			Task<PaymentStatusResult> otherTask = ((completedTask == alipayQueryTask) ? wechatQueryTask : alipayQueryTask);
			if (otherTask.IsCompleted)
			{
				PaymentStatusResult paymentStatusResult2 = await otherTask;
				if (paymentStatusResult2.IsSuccess && paymentStatusResult2.Status == "success")
				{
					string text2 = ((otherTask == alipayQueryTask) ? "支付宝" : "微信");
					Logger.Debug("[ZPay] 检测到" + text2 + "支付成功");
					return paymentStatusResult2;
				}
			}
			else
			{
				PaymentStatusResult paymentStatusResult3 = await otherTask;
				if (paymentStatusResult3.IsSuccess && paymentStatusResult3.Status == "success")
				{
					string text3 = ((otherTask == alipayQueryTask) ? "支付宝" : "微信");
					Logger.Debug("[ZPay] 检测到" + text3 + "支付成功");
					return paymentStatusResult3;
				}
			}
			PaymentStatusResult alipayResult = await alipayQueryTask;
			PaymentStatusResult paymentStatusResult4 = await wechatQueryTask;
			if (!alipayResult.IsSuccess && !paymentStatusResult4.IsSuccess)
			{
				return new PaymentStatusResult
				{
					IsSuccess = false,
					Error = "查询失败。支付宝：" + alipayResult.Error + "，微信：" + paymentStatusResult4.Error
				};
			}
			return alipayResult.IsSuccess ? alipayResult : paymentStatusResult4;
		}
		catch (Exception ex)
		{
			return new PaymentStatusResult
			{
				IsSuccess = false,
				Error = "查询支付状态失败：" + ex.Message
			};
		}
	}

	private async Task<PaymentStatusResult> QuerySingleOrderAsync(string queryBaseUrl, string orderId, string paymentType)
	{
		_ = 1;
		try
		{
			string requestUri = $"{queryBaseUrl}?act=order&pid={_config.MerchantId}&key={_config.MerchantKey}&out_trade_no={orderId}";
			using JsonDocument jsonDocument = JsonDocument.Parse(await (await _httpClient.GetAsync(requestUri)).Content.ReadAsStringAsync());
			JsonElement rootElement = jsonDocument.RootElement;
			string text = "0";
			if (rootElement.TryGetProperty("status", out var value))
			{
				if (value.ValueKind == JsonValueKind.Number)
				{
					text = value.GetInt32().ToString();
				}
				else if (value.ValueKind == JsonValueKind.String)
				{
					text = value.GetString() ?? "0";
				}
			}
			if (text == "0" && rootElement.TryGetProperty("code", out var value2))
			{
				int num = ((value2.ValueKind == JsonValueKind.Number) ? value2.GetInt32() : (int.TryParse(value2.GetString(), out var result) ? result : 0));
				if (num != 1)
				{
					Logger.Debug($"[ZPay] {paymentType}订单未找到（code={num}）");
					return new PaymentStatusResult
					{
						IsSuccess = true,
						Status = "pending"
					};
				}
			}
			string text2 = ((text == "1") ? "success" : ((!(text == "0")) ? "unknown" : "pending"));
			if (text2 == "success")
			{
				string transactionId = (rootElement.TryGetProperty("trade_no", out var value3) ? value3.GetString() : null);
				decimal? paidAmount = null;
				if (rootElement.TryGetProperty("money", out var value4))
				{
					if (value4.ValueKind == JsonValueKind.String)
					{
						paidAmount = (decimal.TryParse(value4.GetString(), out var result2) ? new decimal?(result2) : ((decimal?)null));
					}
					else if (value4.ValueKind == JsonValueKind.Number)
					{
						paidAmount = value4.GetDecimal();
					}
				}
				DateTime? paidAt = null;
				if (rootElement.TryGetProperty("endtime", out var value5))
				{
					if (value5.ValueKind == JsonValueKind.String)
					{
						if (DateTime.TryParse(value5.GetString(), out var result3))
						{
							paidAt = result3;
						}
					}
					else if (value5.ValueKind == JsonValueKind.Number)
					{
						long @int = value5.GetInt64();
						paidAt = ((@int > 1000000000000L) ? DateTimeOffset.FromUnixTimeMilliseconds(@int).DateTime : DateTimeOffset.FromUnixTimeSeconds(@int).DateTime);
					}
				}
				return new PaymentStatusResult
				{
					IsSuccess = true,
					Status = "success",
					TransactionId = transactionId,
					PaymentMethod = paymentType,
					PaidAmount = paidAmount,
					PaidAt = paidAt
				};
			}
			return new PaymentStatusResult
			{
				IsSuccess = true,
				Status = "pending"
			};
		}
		catch (Exception ex)
		{
			Logger.Debug("[ZPay] 查询" + paymentType + "订单异常: " + ex.Message);
			return new PaymentStatusResult
			{
				IsSuccess = false,
				Error = "查询" + paymentType + "订单失败：" + ex.Message
			};
		}
	}

	private Dictionary<string, string> ToDictionary(NameValueCollection collection)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] allKeys = collection.AllKeys;
		foreach (string text in allKeys)
		{
			if (text != null)
			{
				dictionary[text] = collection[text] ?? string.Empty;
			}
		}
		return dictionary;
	}
}
