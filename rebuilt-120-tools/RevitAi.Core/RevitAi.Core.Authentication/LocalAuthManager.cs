using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;

namespace RevitAi.Core.Authentication;

/// <summary>
/// Local authorization stub (authorization layer is intentionally stubbed).
///
/// The original AS.Tools validated accounts/licenses/device binding against Supabase and
/// distributed updates plus credit settlement through Tencent COS. Per the agreed plan this
/// rebuild replaces that with the class below:
///   * no network requests at all;
///   * always considered licensed, every feature gate opens;
///   * device identity is still local (see DeviceService via CoreServicesFactory).
///
/// To re-attach real authorization, switch the construction in CoreServicesFactory back to AuthManager.
/// </summary>
public sealed class LocalAuthManager : IAuthManager
{
	private readonly LocalDevice _device = new LocalDevice();
	private readonly LocalLicense _license = new LocalLicense();
	private readonly LocalUser _user = new LocalUser();

	private readonly Dictionary<string, int> _trialUsage = new Dictionary<string, int>();
	private readonly List<IDeviceInfo> _devices;

	public LocalAuthManager()
	{
		_devices = new List<IDeviceInfo> { _device };
	}

	public IUserIdentity? CurrentUser => _user;

	public IDeviceInfo? CurrentDevice => _device;

	public ILicenseInfo? CurrentLicense => _license;

	public bool IsInitialized => true;

	public bool IsOfflineMode => false;

	public bool IsNetworkAvailable => false;

	public string? RevitVersionYear { get; private set; }

	public event EventHandler<AuthorizationStateChangedEventArgs>? AuthorizationStateChanged;

	public Task<Result> InitializeAsync()
	{
		RaiseChanged();
		return Task.FromResult(Result.Success());
	}

	public Task<Result<IUserIdentity>> SignUpAsync(string email, string password)
		=> Task.FromResult(Result<IUserIdentity>.Success(_user));

	public Task<Result<IUserIdentity>> SignInAsync(string email, string password)
		=> Task.FromResult(Result<IUserIdentity>.Success(_user));

	public Task<Result<IUserIdentity>> AutoSignInAsync()
		=> Task.FromResult(Result<IUserIdentity>.Success(_user));

	public Task<Result> SignOutAsync()
		=> Task.FromResult(Result.Success());

	public Task<Result> RequestPasswordResetAsync(string email)
		=> Task.FromResult(Result.Failure("Local build: password reset is unavailable."));

	public Task<Result<bool>> CanUseFeatureAsync(string featureId)
		=> Task.FromResult(Result<bool>.Success(true));

	public Task<Result<bool>> HasValidLicenseAsync()
		=> Task.FromResult(Result<bool>.Success(true));

	public Task<Result<string>> GetLicenseTypeAsync()
		=> Task.FromResult(Result<string>.Success(_license.LicenseType));

	public Task<Result<DateTime?>> GetLicenseExpiryAsync()
		=> Task.FromResult(Result<DateTime?>.Success(_license.ValidTo));

	public Task<Result<IUserIdentity?>> GetCurrentUserAsync()
		=> Task.FromResult(Result<IUserIdentity?>.Success(_user));

	public Task<Result> RecordFeatureUsageAsync(string featureId)
		=> Task.FromResult(Result.Success());

	public Task<Result<int>> GetRemainingTrialUsageAsync(string featureId)
		=> Task.FromResult(Result<int>.Success(int.MaxValue));

	public decimal GetCachedCreditsBalance() => 0m;

	public CombinedCreditsInfo GetCachedCreditsInfo() => new CombinedCreditsInfo();

	public Task RefreshCreditsCacheAsync() => Task.CompletedTask;

	public Dictionary<string, int> GetCachedTrialUsage() => new Dictionary<string, int>(_trialUsage);

	public Task<Result> RefreshTrialLimitConfigCacheAsync()
		=> Task.FromResult(Result.Success());

	public Task<Result<int>> GetRemainingTrialUsageByGroupAsync(string groupIdentifier)
		=> Task.FromResult(Result<int>.Success(int.MaxValue));

	public Task<Result> EnableOfflineModeAsync()
		=> Task.FromResult(Result.Success());

	public Task<Result> DisableOfflineModeAsync()
		=> Task.FromResult(Result.Success());

	public Task<Result<ILicenseInfo>> GetLicenseInfoAsync()
		=> Task.FromResult(Result<ILicenseInfo>.Success(_license));

	public Task<Result<bool>> IsLicenseExpiringSoonAsync(int daysThreshold = 7)
		=> Task.FromResult(Result<bool>.Success(false));

	public Task<Result<IDeviceInfo>> GetDeviceInfoAsync()
		=> Task.FromResult(Result<IDeviceInfo>.Success(_device));

	public Task<Result<IDeviceInfo>> RegisterDeviceAsync()
		=> Task.FromResult(Result<IDeviceInfo>.Success(_device));

	public Task<Result> RegisterOrFindDeviceAsync()
		=> Task.FromResult(Result.Success());

	public Task<Result<IDeviceInfo>> EnsureDeviceRegisteredAsync()
		=> Task.FromResult(Result<IDeviceInfo>.Success(_device));

	public Task<bool> NeedsHardwareFingerprintMigrationAsync()
		=> Task.FromResult(false);

	public Task<Result> MigrateHardwareFingerprintAsync()
		=> Task.FromResult(Result.Success());

	public Task<Result> RefreshAuthorizationDataAsync()
	{
		RaiseChanged();
		return Task.FromResult(Result.Success());
	}

	public Task<Result<List<IDeviceInfo>>> GetUserDevicesAsync()
		=> Task.FromResult(Result<List<IDeviceInfo>>.Success(new List<IDeviceInfo>(_devices)));

	public Task<Result> BindCurrentDeviceAsync()
		=> Task.FromResult(Result.Success());

	public Task<(bool canBind, string message)> CanBindCurrentDeviceAsync()
		=> Task.FromResult((true, string.Empty));

	public Task<Result> RefreshCurrentDeviceAsync()
		=> Task.FromResult(Result.Success());

	public Task<Result> UnbindDeviceAsync(string deviceId)
		=> Task.FromResult(Result.Success());

	public Task<Result> InitializeUserCreditsAsync()
		=> Task.FromResult(Result.Success());

	public void SetCurrentRevitVersion(string versionYear)
	{
		RevitVersionYear = versionYear;
	}

	public AuthorizationState GetAuthorizationState()
	{
		return new AuthorizationState
		{
			HasValidLicense = true,
			LicenseType = _license.LicenseType,
			ExpiryDate = _license.ValidTo,
			CreditsBalance = 0m,
			TrialUsage = new Dictionary<string, int>(_trialUsage)
		};
	}

	public List<IDeviceInfo>? GetCachedUserDevices() => new List<IDeviceInfo>(_devices);

	private void RaiseChanged()
	{
		AuthorizationStateChanged?.Invoke(this, new AuthorizationStateChangedEventArgs
		{
			HasValidLicense = true,
			LicenseType = _license.LicenseType,
			ExpiryDate = _license.ValidTo,
			CreditsBalance = 0m,
			TrialUsage = new Dictionary<string, int>(_trialUsage),
			UserDevices = new List<IDeviceInfo>(_devices)
		});
	}

	// ------------------------------------------------------------------

	/// <summary>鏈満鎸囩汗锛氫粎鐢ㄦ満鍣ㄥ悕 + 鎿嶄綔绯荤粺鐗堟湰鍋氱ǔ瀹氬搱甯岋紝涓嶈仈缃戙€佷笉璇荤‖浠跺簭鍒楀彿銆?/summary>
	private static class LocalFingerprint
	{
		private static readonly string _value = Compute();

		public static string Value => _value;

		private static string Compute()
		{
			string raw = Environment.MachineName + "|" + Environment.OSVersion.VersionString;
			using (System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create())
			{
				byte[] hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(raw));
				System.Text.StringBuilder builder = new System.Text.StringBuilder(hash.Length * 2);
				foreach (byte b in hash)
				{
					builder.Append(b.ToString("x2"));
				}
				return builder.ToString();
			}
		}
	}

	private sealed class LocalUser : IUserIdentity
	{
		public Guid UserId { get; } = Guid.Empty;

		public string Email => "local@revitai";

		public string? FullName => "鏈湴鐢ㄦ埛";
	}

	private sealed class LocalDevice : IDeviceInfo
	{
		public Guid Id { get; } = Guid.Empty;

		public string DeviceId => LocalFingerprint.Value;

		public string DeviceName => Environment.MachineName;

		public string OsVersion => Environment.OSVersion.VersionString;

		public bool IsActive => true;

		public bool IsBound => true;
	}

	private sealed class LocalLicense : ILicenseInfo
	{
		public Guid LicenseId { get; } = Guid.Empty;

		public string LicenseType => "Local";

		public DateTime ValidFrom { get; } = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public DateTime? ValidTo => null;

		public string DeviceFingerprint => LocalFingerprint.Value;

		public string[] Features { get; } = new string[0];

		public bool IsActive => true;

		public bool IsValid() => true;

		public bool HasFeature(string featureId) => true;
	}
}
