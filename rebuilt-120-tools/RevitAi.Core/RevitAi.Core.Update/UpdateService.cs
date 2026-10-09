using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Product;
using RevitAi.Abstractions.Update;
using RevitAi.Abstractions.Update.Models;
using COSXML;
using COSXML.Auth;
using COSXML.CosException;
using COSXML.Model;
using COSXML.Model.Object;
using Newtonsoft.Json;
using ns0;
using ns7;

namespace RevitAi.Core.Update;

public sealed class UpdateService : IDisposable, IUpdateService
{
	[CompilerGenerated]
	public sealed class Class66
	{
		public CancellationToken cancellationToken_0;

		public UpdateService updateService_0;

		public GetObjectRequest getObjectRequest_0;

		internal void method_0()
		{
			cancellationToken_0.ThrowIfCancellationRequested();
			updateService_0.cosXmlServer_0.GetObject(getObjectRequest_0);
		}
	}

	[CompilerGenerated]
	public sealed class Class67
	{
		public CancellationToken cancellationToken_0;

		public UpdateService updateService_0;

		public GetObjectRequest getObjectRequest_0;

		internal void method_0()
		{
			cancellationToken_0.ThrowIfCancellationRequested();
			updateService_0.cosXmlServer_0.GetObject(getObjectRequest_0);
		}
	}

	[CompilerGenerated]
	public sealed class Class68
	{
		public CancellationToken cancellationToken_0;

		public UpdateService updateService_0;

		public GetObjectRequest getObjectRequest_0;

		internal void method_0()
		{
			cancellationToken_0.ThrowIfCancellationRequested();
			updateService_0.cosXmlServer_0.GetObject(getObjectRequest_0);
		}
	}

	[CompilerGenerated]
	public sealed class Class69
	{
		public CancellationToken cancellationToken_0;

		public UpdateService updateService_0;

		public GetObjectRequest getObjectRequest_0;

		internal void method_0()
		{
			cancellationToken_0.ThrowIfCancellationRequested();
			updateService_0.cosXmlServer_0.GetObject(getObjectRequest_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct4 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<UpdateInfo> asyncTaskMethodBuilder_0;

		public CancellationToken cancellationToken_0;

		public UpdateService updateService_0;

		private string string_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_02c9: Expected O, but got Unknown
			//IL_02ec: Expected O, but got Unknown
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Expected O, but got Unknown
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Invalid comparison between Unknown and I4
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Expected O, but got Unknown
			int num = int_0;
			UpdateService updateService = updateService_0;
			Class68 @class = default(Class68);
			if (num != 0)
			{
				@class = new Class68();
				@class.cancellationToken_0 = cancellationToken_0;
				@class.updateService_0 = updateService_0;
			}
			UpdateInfo result;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi\\Updates");
					Directory.CreateDirectory(text);
					string_0 = Path.Combine(text, "version_remote.json");
					@class.getObjectRequest_0 = new GetObjectRequest("astools-1314165830", "version.json", text, "version_remote.json");
					long num2 = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
					((CosRequest)@class.getObjectRequest_0).SetSign(num2, 600L);
					awaiter = Task.Run((Action)@class.method_0, @class.cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
				}
				awaiter.GetResult();
				if (!File.Exists(string_0))
				{
					Logger.Error("[UpdateService] 下载的文件不存在: " + string_0);
					result = null;
				}
				else
				{
					string text2 = File.ReadAllText(string_0);
					if (text2.TrimStart().StartsWith("<"))
					{
						Logger.Error("[UpdateService] 下载的内容是 HTML 而非 JSON");
						Logger.Error("[UpdateService] 文件内容前500字符: " + ((text2.Length > 500) ? text2.Substring(0, 500) : text2));
						result = null;
					}
					else
					{
						RemoteVersionManifest val = JsonConvert.DeserializeObject<RemoteVersionManifest>(text2);
						try
						{
							File.Delete(string_0);
						}
						catch
						{
						}
						if (val != null && val.Latest != null)
						{
							result = (((int)updateService.method_0(updateService.string_1, val.Latest.Version) != 1) ? ((UpdateInfo)null) : new UpdateInfo
							{
								Version = val.Latest.Version,
								CurrentVersion = updateService.string_1,
								DownloadUrl = val.Latest.DownloadUrl,
								Checksum = val.Latest.Checksum,
								PackageSize = val.Latest.PackageSize,
								IsForceUpdate = val.Latest.IsForceUpdate,
								ReleaseNotes = val.Latest.ReleaseNotes,
								ReleaseDate = val.Latest.ReleaseDate
							});
						}
						else
						{
							Logger.Warning("[UpdateService] 版本清单解析失败");
							result = null;
						}
					}
				}
			}
			catch (OperationCanceledException)
			{
				result = null;
			}
			catch (CosClientException ex2)
			{
				CosClientException ex3 = ex2;
				Logger.Error("[UpdateService] COS 客户端错误: " + ((Exception)(object)ex3).Message, (Exception)(object)ex3);
				result = null;
			}
			catch (CosServerException ex4)
			{
				CosServerException ex5 = ex4;
				Logger.Error("[UpdateService] COS 服务器错误: " + ex5.GetInfo(), (Exception)(object)ex5);
				result = null;
			}
			catch (Exception ex6)
			{
				Logger.Error("[UpdateService] 检查更新失败: " + ex6.Message, ex6);
				result = null;
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct5 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public CancellationToken cancellationToken_0;

		public UpdateService updateService_0;

		public string string_0;

		private string string_1;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Expected O, but got Unknown
			int num = int_0;
			UpdateService updateService = updateService_0;
			Class66 @class = default(Class66);
			if (num != 0)
			{
				@class = new Class66();
				@class.cancellationToken_0 = cancellationToken_0;
				@class.updateService_0 = updateService_0;
			}
			string result;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					string text = updateService.method_1(string_0);
					string text2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi\\Updates");
					Directory.CreateDirectory(text2);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
					defaultInterpolatedStringHandler.AppendLiteral("temp_");
					defaultInterpolatedStringHandler.AppendFormatted(Guid.NewGuid());
					defaultInterpolatedStringHandler.AppendLiteral(".json");
					string text3 = defaultInterpolatedStringHandler.ToStringAndClear();
					string_1 = Path.Combine(text2, text3);
					@class.getObjectRequest_0 = new GetObjectRequest("astools-1314165830", text, text2, text3);
					long num2 = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
					((CosRequest)@class.getObjectRequest_0).SetSign(num2, 600L);
					awaiter = Task.Run((Action)@class.method_0, @class.cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
				}
				awaiter.GetResult();
				string text4 = File.ReadAllText(string_1);
				try
				{
					File.Delete(string_1);
				}
				catch
				{
				}
				result = text4;
			}
			catch (OperationCanceledException)
			{
				result = null;
			}
			catch (Exception ex2)
			{
				Logger.Error("[UpdateService] 下载文本失败: " + string_0, ex2);
				result = null;
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct6 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public CancellationToken cancellationToken_0;

		public UpdateService updateService_0;

		public UpdateInfo updateInfo_0;

		public Action<int> action_0;

		private string string_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected O, but got Unknown
			int num = int_0;
			UpdateService updateService = updateService_0;
			Class69 @class = default(Class69);
			if (num != 0)
			{
				@class = new Class69();
				@class.cancellationToken_0 = cancellationToken_0;
				@class.updateService_0 = updateService_0;
			}
			string result;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi\\Updates");
					Directory.CreateDirectory(text);
					updateService.method_2(text, updateInfo_0.Version);
					string text2 = "RevitAi_v" + updateInfo_0.Version + ".zip";
					string_0 = Path.Combine(text, text2);
					string text3 = updateService.method_1(updateInfo_0.DownloadUrl);
					@class.getObjectRequest_0 = new GetObjectRequest("astools-1314165830", text3, text, text2);
					long num2 = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
					((CosRequest)@class.getObjectRequest_0).SetSign(num2, 600L);
					awaiter = Task.Run((Action)@class.method_0, @class.cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
				}
				awaiter.GetResult();
				action_0?.Invoke(100);
				result = string_0;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				Logger.Error("[UpdateService] 下载更新包失败: " + ex2.Message, ex2);
				throw;
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct7 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<LatestVersionInfo> asyncTaskMethodBuilder_0;

		public CancellationToken cancellationToken_0;

		public UpdateService updateService_0;

		private string string_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Expected O, but got Unknown
			int num = int_0;
			Class67 @class = default(Class67);
			if (num != 0)
			{
				@class = new Class67();
				@class.cancellationToken_0 = cancellationToken_0;
				@class.updateService_0 = updateService_0;
			}
			LatestVersionInfo result;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi\\Updates");
					Directory.CreateDirectory(text);
					string_0 = Path.Combine(text, "version_info.json");
					@class.getObjectRequest_0 = new GetObjectRequest("astools-1314165830", "version.json", text, "version_info.json");
					Logger.Debug("[UpdateService] 从 COS 下载版本文件: version.json");
					long num2 = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
					((CosRequest)@class.getObjectRequest_0).SetSign(num2, 600L);
					awaiter = Task.Run((Action)@class.method_0, @class.cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
				}
				awaiter.GetResult();
				if (!File.Exists(string_0))
				{
					Logger.Error("[UpdateService] 下载的文件不存在: " + string_0);
					result = null;
				}
				else
				{
					string text2 = File.ReadAllText(string_0);
					try
					{
						File.Delete(string_0);
					}
					catch
					{
					}
					if (text2.TrimStart().StartsWith("<"))
					{
						Logger.Error("[UpdateService] 下载的内容是 HTML 而非 JSON");
						result = null;
					}
					else
					{
						RemoteVersionManifest val = JsonConvert.DeserializeObject<RemoteVersionManifest>(text2);
						if (val != null && val.Latest != null)
						{
							Logger.Debug("[UpdateService] 版本信息获取成功: " + val.Latest.Version);
							result = val.Latest;
						}
						else
						{
							Logger.Warning("[UpdateService] 版本清单解析失败");
							result = null;
						}
					}
				}
			}
			catch (OperationCanceledException)
			{
				result = null;
			}
			catch (Exception ex2)
			{
				Logger.Error("[UpdateService] 获取版本信息失败: " + ex2.Message, ex2);
				result = null;
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private const string string_0 = "version.json";

	private readonly CosXmlServer cosXmlServer_0;

	private readonly string string_1;

	private bool bool_0;

	public UpdateService(string currentVersion)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		if (string.IsNullOrEmpty(currentVersion))
		{
			throw new ArgumentNullException("currentVersion");
		}
		string_1 = currentVersion;
		CosXmlConfig val = new CosXmlConfig.Builder().IsHttps(true).SetRegion("ap-shanghai").SetDebugLog(false)
			.Build();
		DefaultQCloudCredentialProvider val2 = new DefaultQCloudCredentialProvider(ProductInfo.CosSecretId, ProductInfo.CosSecretKey, 600L);
		cosXmlServer_0 = new CosXmlServer(val, (QCloudCredentialProvider)(object)val2);
	}

	[AsyncStateMachine(typeof(Struct4))]
	public Task<UpdateInfo?> CheckForUpdatesAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct4 stateMachine = default(Struct4);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<UpdateInfo>.Create();
		stateMachine.updateService_0 = this;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct6))]
	public Task<string> DownloadUpdateAsync(UpdateInfo updateInfo, Action<int>? progressCallback = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct6 stateMachine = default(Struct6);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.updateService_0 = this;
		stateMachine.updateInfo_0 = updateInfo;
		stateMachine.action_0 = progressCallback;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public bool VerifyUpdatePackage(string filePath, string expectedChecksum)
	{
		try
		{
			if (File.Exists(filePath))
			{
				using (SHA256 sHA = SHA256.Create())
				{
					using FileStream inputStream = File.OpenRead(filePath);
					if (BitConverter.ToString(sHA.ComputeHash(inputStream)).Replace("-", "").ToLowerInvariant()
						.Equals(expectedChecksum, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
					Logger.Error("[UpdateService] ✗ 校验失败：SHA256 不匹配");
					return false;
				}
			}
			Logger.Error("[UpdateService] 更新包文件不存在: " + filePath);
			return false;
		}
		catch (Exception ex)
		{
			Logger.Error("[UpdateService] 验证更新包失败: " + ex.Message, ex);
			return false;
		}
	}

	public Task PrepareUpdateAsync(string packagePath, UpdateInfo updateInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi\\Updates\\pending_update.json");
			string directoryName = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			string contents = JsonConvert.SerializeObject((object)new Class64<string, string, string, string, string, string, DateTime>(packagePath, updateInfo.Version, updateInfo.DownloadUrl, updateInfo.Checksum, updateInfo.ReleaseNotes, updateInfo.CurrentVersion, DateTime.UtcNow), (Formatting)1);
			File.WriteAllText(path, contents);
			return Task.CompletedTask;
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (Exception ex2)
		{
			Logger.Error("[UpdateService] 准备更新失败: " + ex2.Message, ex2);
			throw;
		}
	}

	public bool HasPendingUpdate()
	{
		try
		{
			return File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi\\Updates\\pending_update.json"));
		}
		catch (Exception ex)
		{
			Logger.Error("[UpdateService] 检查待安装更新失败: " + ex.Message, ex);
			return false;
		}
	}

	public UpdateInfo? GetPendingUpdateInfo()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		try
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi\\Updates\\pending_update.json");
			if (!File.Exists(path))
			{
				return null;
			}
			Class65<string, string, string, string, string, string> @class = JsonConvert.DeserializeAnonymousType<Class65<string, string, string, string, string, string>>(File.ReadAllText(path), new Class65<string, string, string, string, string, string>(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
			if (@class == null)
			{
				return null;
			}
			return new UpdateInfo
			{
				Version = @class.TargetVersion,
				DownloadUrl = @class.DownloadUrl,
				Checksum = @class.Checksum,
				ReleaseNotes = @class.ReleaseNotes,
				CurrentVersion = @class.CurrentVersion
			};
		}
		catch (Exception ex)
		{
			Logger.Error("[UpdateService] 读取待安装更新失败: " + ex.Message, ex);
			return null;
		}
	}

	public string GetCurrentVersion()
	{
		return string_1;
	}

	[AsyncStateMachine(typeof(Struct5))]
	public Task<string?> DownloadTextAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct5 stateMachine = default(Struct5);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.updateService_0 = this;
		stateMachine.string_0 = url;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct7))]
	public Task<LatestVersionInfo?> GetVersionInfoAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct7 stateMachine = default(Struct7);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<LatestVersionInfo>.Create();
		stateMachine.updateService_0 = this;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private VersionComparisonResult method_0(string string_2, string string_3)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (Version.TryParse(string_2, out Version result) && Version.TryParse(string_3, out Version result2))
		{
			int num = result.CompareTo(result2);
			if (num >= 0)
			{
				if (num <= 0)
				{
					return (VersionComparisonResult)2;
				}
				return (VersionComparisonResult)0;
			}
			return (VersionComparisonResult)1;
		}
		Logger.Warning("[UpdateService] 版本号格式无效: current=" + string_2 + ", target=" + string_3);
		return (VersionComparisonResult)3;
	}

	private string method_1(string string_2)
	{
		try
		{
			return new Uri(string_2).AbsolutePath.TrimStart('/');
		}
		catch (Exception ex)
		{
			Logger.Error("[UpdateService] 提取 COS Key 失败: " + ex.Message, ex);
			throw;
		}
	}

	private void method_2(string string_2, string string_3)
	{
		try
		{
			if (!Directory.Exists(string_2))
			{
				return;
			}
			string[] files = Directory.GetFiles(string_2, "RevitAi_v*.zip");
			if (files.Length == 0)
			{
				return;
			}
			int num = 0;
			string[] array = files;
			foreach (string path in array)
			{
				string fileName = Path.GetFileName(path);
				if (!fileName.Equals("RevitAi_v" + string_3 + ".zip", StringComparison.OrdinalIgnoreCase))
				{
					try
					{
						File.Delete(path);
						num++;
					}
					catch (Exception ex)
					{
						Logger.Warning("[UpdateService] 删除旧版本压缩包失败 " + fileName + ": " + ex.Message);
					}
				}
			}
		}
		catch (Exception ex2)
		{
			Logger.Warning("[UpdateService] 清理旧版本压缩包失败: " + ex2.Message);
		}
	}

	public void Dispose()
	{
		if (!bool_0)
		{
			bool_0 = true;
		}
	}
}
