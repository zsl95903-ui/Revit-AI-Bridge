using System;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.Update.Models;

namespace RevitAi.Abstractions.Update;

public interface IUpdateService
{
	Task<UpdateInfo?> CheckForUpdatesAsync(CancellationToken cancellationToken = default(CancellationToken));

	Task<string> DownloadUpdateAsync(UpdateInfo updateInfo, Action<int>? progressCallback = null, CancellationToken cancellationToken = default(CancellationToken));

	bool VerifyUpdatePackage(string filePath, string expectedChecksum);

	Task PrepareUpdateAsync(string packagePath, UpdateInfo updateInfo, CancellationToken cancellationToken = default(CancellationToken));

	bool HasPendingUpdate();

	UpdateInfo? GetPendingUpdateInfo();

	string GetCurrentVersion();

	Task<string?> DownloadTextAsync(string url, CancellationToken cancellationToken = default(CancellationToken));

	Task<LatestVersionInfo?> GetVersionInfoAsync(CancellationToken cancellationToken = default(CancellationToken));
}
