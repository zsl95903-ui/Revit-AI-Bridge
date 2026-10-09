using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;

namespace RevitAi.Abstractions.Authentication;

public interface IAuthManager
{
	IUserIdentity? CurrentUser { get; }

	IDeviceInfo? CurrentDevice { get; }

	ILicenseInfo? CurrentLicense { get; }

	bool IsInitialized { get; }

	bool IsOfflineMode { get; }

	bool IsNetworkAvailable { get; }

	event EventHandler<AuthorizationStateChangedEventArgs>? AuthorizationStateChanged;

	Task<Result> InitializeAsync();

	Task<Result<IUserIdentity>> SignUpAsync(string email, string password);

	Task<Result<IUserIdentity>> SignInAsync(string email, string password);

	Task<Result<IUserIdentity>> AutoSignInAsync();

	Task<Result> SignOutAsync();

	Task<Result> RequestPasswordResetAsync(string email);

	Task<Result<bool>> CanUseFeatureAsync(string featureId);

	Task<Result<bool>> HasValidLicenseAsync();

	Task<Result<string>> GetLicenseTypeAsync();

	Task<Result<DateTime?>> GetLicenseExpiryAsync();

	Task<Result<IUserIdentity?>> GetCurrentUserAsync();

	Task<Result> RecordFeatureUsageAsync(string featureId);

	Task<Result<int>> GetRemainingTrialUsageAsync(string featureId);

	decimal GetCachedCreditsBalance();

	CombinedCreditsInfo GetCachedCreditsInfo();

	Task RefreshCreditsCacheAsync();

	Dictionary<string, int> GetCachedTrialUsage();

	Task<Result> RefreshTrialLimitConfigCacheAsync();

	Task<Result<int>> GetRemainingTrialUsageByGroupAsync(string groupIdentifier);

	Task<Result> EnableOfflineModeAsync();

	Task<Result> DisableOfflineModeAsync();

	Task<Result<ILicenseInfo>> GetLicenseInfoAsync();

	Task<Result<bool>> IsLicenseExpiringSoonAsync(int daysThreshold = 7);

	Task<Result<IDeviceInfo>> GetDeviceInfoAsync();

	Task<Result<IDeviceInfo>> RegisterDeviceAsync();

	Task<Result> RegisterOrFindDeviceAsync();

	Task<Result<IDeviceInfo>> EnsureDeviceRegisteredAsync();

	Task<bool> NeedsHardwareFingerprintMigrationAsync();

	Task<Result> MigrateHardwareFingerprintAsync();

	Task<Result> RefreshAuthorizationDataAsync();

	Task<Result<List<IDeviceInfo>>> GetUserDevicesAsync();

	Task<Result> BindCurrentDeviceAsync();

	Task<(bool canBind, string message)> CanBindCurrentDeviceAsync();

	Task<Result> RefreshCurrentDeviceAsync();

	Task<Result> UnbindDeviceAsync(string deviceId);

	Task<Result> InitializeUserCreditsAsync();

	void SetCurrentRevitVersion(string versionYear);

	AuthorizationState GetAuthorizationState();

	List<IDeviceInfo>? GetCachedUserDevices();
}
