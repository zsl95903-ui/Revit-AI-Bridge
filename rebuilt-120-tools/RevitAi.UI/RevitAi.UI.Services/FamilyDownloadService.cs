using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.FamilyLibrary;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;

namespace RevitAi.UI.Services;

internal sealed class FamilyDownloadService : IFamilyDownloadService
{
	private readonly ILogger _logger;

	public FamilyDownloadService()
	{
		_logger = ServiceProvider.GetLogger();
	}

	public async Task<FamilyDownloadResult> DownloadFamilyFileAsync(string familyName)
	{
		try
		{
			IFamilyLibraryService familyLibraryService = ServiceProvider.GetService<IFamilyLibraryService>();
			if (familyLibraryService == null)
			{
				return FamilyDownloadResult.Fail("族库服务未初始化");
			}
			string searchText = familyName;
			Result<List<FamilyLibraryItem>> result = await familyLibraryService.GetFamiliesAsync(null, searchText, null, null, null, null, 1, 50, includeNonPublic: true);
			if (!result.IsSuccess || result.Value == null)
			{
				return FamilyDownloadResult.Fail("无法连接族库，请检查网络后重试");
			}
			FamilyLibraryItem matchingFamily = result.Value.FirstOrDefault((FamilyLibraryItem f) => f.DisplayName.Equals(familyName, StringComparison.OrdinalIgnoreCase));
			if (matchingFamily == null)
			{
				return FamilyDownloadResult.Fail("族库中暂未提供族 [" + familyName + "]，请联系开发者上传该族文件");
			}
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "FamilyLibrary", "FamilyFiles");
			Directory.CreateDirectory(text);
			string cachedFile = Path.Combine(text, $"{matchingFamily.Id}.rfa");
			if (File.Exists(cachedFile))
			{
				return FamilyDownloadResult.Ok(cachedFile);
			}
			Result<FamilyDownloadInfo> result2 = await familyLibraryService.RequestDownloadAsync(matchingFamily.Id);
			if (!result2.IsSuccess || result2.Value == null)
			{
				return FamilyDownloadResult.Fail("无法连接族库，请检查网络后重试");
			}
			if (!result2.Value.CanDownload)
			{
				return FamilyDownloadResult.Fail("族下载次数已用完，请购买授权以增加下载额度后再试。\n（族: " + familyName + "）");
			}
			_logger.Info("[FamilyDownloadService] 开始下载族文件: " + familyName);
			using (HttpClient client = new HttpClient())
			{
				client.Timeout = TimeSpan.FromMinutes(5L);
				HttpResponseMessage httpResponseMessage = await client.GetAsync(result2.Value.FileUrl);
				if (!httpResponseMessage.IsSuccessStatusCode)
				{
					return FamilyDownloadResult.Fail($"下载族文件失败 (HTTP {httpResponseMessage.StatusCode})，请稍后重试");
				}
				File.WriteAllBytes(cachedFile, await httpResponseMessage.Content.ReadAsByteArrayAsync());
			}
			familyLibraryService.RecordDownloadLocally(matchingFamily.Id);
			_logger.Info("[FamilyDownloadService] 族文件下载成功: " + familyName + " -> " + cachedFile);
			return FamilyDownloadResult.Ok(cachedFile);
		}
		catch (Exception ex)
		{
			_logger.Error("[FamilyDownloadService] 下载族文件失败: " + ex.Message, ex);
			return FamilyDownloadResult.Fail("下载族文件时发生错误: " + ex.Message);
		}
	}
}
