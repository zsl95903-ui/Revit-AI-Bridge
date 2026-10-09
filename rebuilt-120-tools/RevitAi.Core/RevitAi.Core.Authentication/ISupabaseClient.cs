using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.Core.AI;
using RevitAi.Core.Authentication.Models;
using RevitAi.Core.Feedback.Models;
using RevitAi.Core.Security;

namespace RevitAi.Core.Authentication;

public interface ISupabaseClient : IDisposable
{
	string BaseUrl { get; }

	HttpClient HttpClient { get; }

	Task WarmUpConnectionAsync();

	Task<Result<UserIdentity>> SignUpAsync(string email, string password);

	Task<Result<UserIdentity>> SignInAsync(string email, string password);

	Task<Result<UserIdentity>> SignInByDeviceAsync(string deviceId);

	Task<Result> SignOutAsync();

	Task<Result<UserIdentity>> RefreshTokenAsync(string refreshToken);

	Task<Result> RequestPasswordResetAsync(string email);

	Task<Result<DeviceInfo>> RegisterDeviceAsync(DeviceInfo deviceInfo);

	Task<Result<DeviceInfo>> GetDeviceInfoAsync(string deviceId);

	Task<Result> BindDeviceAsync(string deviceId, Guid userId);

	Task<Result> UpdateDeviceLastSeenAsync(string deviceId);

	Task<Result<List<DeviceInfo>>> GetUserDevicesAsync(Guid userId);

	Task<Result> UnbindDeviceAsync(string deviceId);

	Task<Result<DeviceInfo>> UpdateDeviceAsync(string deviceId, string revitVersion, string ipAddress);

	Task<Result<MigrateDeviceResult>> MigrateDeviceIdCopyAsync(string oldDeviceId, string newDeviceId, string? deviceName = null, string? osInfo = null, string? ipAddress = null);

	Task<Result<int>> RecordTrialUsageAsync(string deviceId, string featureId, Guid? userId = null, string? featureGroup = null);

	Task<Result<TrialRecordDetail>> RecordTrialUsageV2Async(string deviceId, string featureGroup, string featureId, Guid? userId = null, string trialVersion = "v1.0");

	Task<Result<TrialEligibility>> CheckTrialEligibilityAsync(string deviceId, string featureId, Guid? userId = null);

	Task<Result<int>> GetMaxTrialLimitAsync(string featureGroup, string? featureId = null);

	Task<Result<List<TrialRecordDetail>>> GetTrialRecordsByUserAsync(Guid userId);

	Task<Result<List<TrialRecordDetail>>> GetTrialRecordsByDeviceAsync(string deviceId);

	Task<Result> InsertTrialRecordAsync(string deviceId, string featureId, string? featureGroup, int maxUsageCount, Guid? userId = null);

	Task<Result<TrialRecordDetail?>> GetTrialRecordAsync(string deviceId, string featureId, Guid? userId = null, string? featureGroup = null);

	Task<Result> CreateTrialRecordAsync(string deviceId, string featureId, string? featureGroup, int maxUsageCount, Guid? userId = null);

	Task<Result<List<TrialLimitConfig>>> GetAllTrialLimitsAsync();

	Task<Result<LicenseInfo>> GetLicenseInfoAsync(Guid userId);

	Task<Result<LicenseInfo>> GetLicenseByDeviceAsync(string deviceId);

	Task<Result<List<LicensePackage>>> GetLicensePackagesAsync();

	Task<Result<PromotionCodeValidationResult>> ValidatePromotionCodeAsync(string code, decimal originalPrice, string featureType, Guid? userId = null, string? deviceId = null);

	Task<Result> ActivatePromotionCodeAsync(string code, string orderId, Guid? userId = null, string? deviceId = null);

	Task<Result<List<AIConfig>>> GetAllActiveAIConfigsAsync();

	Task<Result<AIConfig>> GetAIConfigByModelNameAsync(string modelName);

	Task<Result<AIConfig>> GetDefaultAIConfigAsync();

	Task<Result<AIConfig>> GetAIConfigByProviderAsync(string provider);

	Task<Result<EncryptedApiKeyRecord>> GetEncryptedApiKeyAsync(string apiUrl);

	Task<Result<EncryptedApiKeyRecord>> GetEncryptedApiKeyByNotesAsync(string notes);

	Task<Result<CombinedCreditsInfo>> GetUserCreditsAsync(Guid? userId = null, Guid? deviceId = null);

	Task<Result<InitialCreditsInfo>> GetInitialCreditsAsync();

	Task<Result<bool>> CheckCreditsSufficientAsync(decimal requiredAmount, Guid? userId = null, Guid? deviceId = null);

	Task<Result<DeviceCreditsResult>> InitializeDeviceCreditsAsync(Guid deviceId);

	Task<Result<UserCreditsResult>> InitializeUserCreditsAsync(Guid userId);

	Task<Result<LicenseTransferResult>> TransferLicenseToUserOnLoginAsync(string deviceId, Guid userId);

	Task<Result<Guid>> RecordTokenUsageOnlyAsync(Guid? userId, Guid? deviceId, string? sessionId, string provider, string modelName, int inputTokens, int outputTokens, string requestType = "chat", bool isToolCall = false, int toolCount = 0, int? durationMs = null);

	Task<Result<List<CreditDeductionResult>>> DeductCreditsSmartAsync(Guid? userId, Guid? deviceId, decimal amount);

	Task<Result<CreditDeductionResult>> DeductCreditsSafeAsync(Guid? userId, Guid? deviceId, decimal amount);

	Task<Result<List<TokenUsageInfo>>> GetRecentTokenUsageAsync(int limit = 50);

	Task<Result<Guid>> CreateCreditRechargeAsync(string orderId, decimal amount, int credits, Guid? userId = null, Guid? deviceId = null, decimal? originalPrice = null, int? originalCredits = null, decimal discountRate = 1.0m, string paymentMethod = "alipay");

	Task<Result<Guid>> RecordAIMessageAsync(string messageText, string featureId = "AI_Send", string? sessionId = null, string? aiProvider = null, string? aiModel = null, Guid? deviceId = null, Guid? userId = null);

	Task<Result<Guid>> SubmitFeedbackAsync(string deviceId, Guid? userId, string title, string content, string? email = null, string? attachments = null);

	Task<Result<List<FeedbackInfo>>> GetFeedbackListAsync(string deviceId, Guid? userId = null);

	Task<Result<int>> GetUnreadReplyCountAsync(string deviceId, Guid? userId = null);

	Task<Result> DeleteFeedbackAsync(Guid feedbackId);

	Task<Result> MarkFeedbackAsReadAsync(Guid feedbackId, Guid userId);

	Task<Result> MarkFeedbackAsReadAsync(Guid feedbackId, string deviceId);

	Task<Result<string>> UploadFileAsync(string bucketName, string path, byte[] fileBytes, string contentType);

	Task<Result> DeleteFileAsync(string bucketName, string path);

	Task<Result<CosPresignedUrlInfo>> GetCosPresignedUrlAsync(string filename, string contentType = "application/octet-stream");

	Task<Result> UploadToCosWithPresignedUrlAsync(string presignedUrl, byte[] fileBytes, string contentType);
}
