using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using Newtonsoft.Json;
using ns7;

namespace RevitAi.Core.AI;

public sealed class FileStorageService : IFileStorageService
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct204<T> : IAsyncStateMachine where T : notnull
	{
		public int int_0;

		public AsyncTaskMethodBuilder<T> asyncTaskMethodBuilder_0;

		public string string_0;

		public FileStorageService fileStorageService_0;

		private StreamReader streamReader_0;

		private ConfiguredTaskAwaitable<string>.ConfiguredTaskAwaiter configuredTaskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FileStorageService fileStorageService = fileStorageService_0;
			T result;
			try
			{
				if (num == 0)
				{
					goto IL_005d;
				}
				if (File.Exists(string_0))
				{
					streamReader_0 = new StreamReader(string_0, Encoding.UTF8);
					goto IL_005d;
				}
				Logger.Warning("[FileStorage] 文件不存在: " + string_0);
				result = default(T);
				goto end_IL_000f;
				IL_005d:
				string result2;
				try
				{
					ConfiguredTaskAwaitable<string>.ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						awaiter = streamReader_0.ReadToEndAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							configuredTaskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = configuredTaskAwaiter_0;
						configuredTaskAwaiter_0 = default(ConfiguredTaskAwaitable<string>.ConfiguredTaskAwaiter);
						num = -1;
						int_0 = -1;
					}
					result2 = awaiter.GetResult();
				}
				finally
				{
					if (num < 0 && streamReader_0 != null)
					{
						((IDisposable)streamReader_0).Dispose();
					}
				}
				streamReader_0 = null;
				result = JsonConvert.DeserializeObject<T>(result2, fileStorageService.jsonSerializerSettings_0);
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[FileStorage] 加载文件失败: " + string_0, ex);
				result = default(T);
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
	public struct Struct205<T> : IAsyncStateMachine where T : notnull
	{
		public int int_0;

		public AsyncTaskMethodBuilder<bool> asyncTaskMethodBuilder_0;

		public string string_0;

		public FileStorageService fileStorageService_0;

		public T gparam_0;

		private StreamWriter streamWriter_0;

		private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter configuredTaskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FileStorageService fileStorageService = fileStorageService_0;
			bool result;
			try
			{
				string value = default(string);
				if (num != 0)
				{
					string directoryName = Path.GetDirectoryName(string_0);
					if (!string.IsNullOrEmpty(directoryName))
					{
						fileStorageService.EnsureDirectoryExists(directoryName);
					}
					value = JsonConvert.SerializeObject((object)gparam_0, fileStorageService.jsonSerializerSettings_0);
					streamWriter_0 = new StreamWriter(string_0, append: false, Encoding.UTF8);
				}
				try
				{
					ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						awaiter = streamWriter_0.WriteAsync(value).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							configuredTaskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = configuredTaskAwaiter_0;
						configuredTaskAwaiter_0 = default(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter);
						num = -1;
						int_0 = -1;
					}
					awaiter.GetResult();
				}
				finally
				{
					if (num < 0 && streamWriter_0 != null)
					{
						((IDisposable)streamWriter_0).Dispose();
					}
				}
				streamWriter_0 = null;
				result = true;
			}
			catch (Exception ex)
			{
				Logger.Error("[FileStorage] 保存文件失败: " + string_0, ex);
				result = false;
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

	private readonly string string_0;

	private readonly JsonSerializerSettings jsonSerializerSettings_0;

	public FileStorageService(string baseDirectory)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		if (string.IsNullOrWhiteSpace(baseDirectory))
		{
			throw new ArgumentException("基础目录路径不能为空", "baseDirectory");
		}
		string_0 = baseDirectory;
		EnsureDirectoryExists(string_0);
		jsonSerializerSettings_0 = new JsonSerializerSettings
		{
			Formatting = (Formatting)1,
			NullValueHandling = (NullValueHandling)1,
			ReferenceLoopHandling = (ReferenceLoopHandling)1,
			DateFormatString = "yyyy-MM-dd HH:mm:ss",
			ConstructorHandling = (ConstructorHandling)1
		};
	}

	[AsyncStateMachine(typeof(Struct205<>))]
	public Task<bool> SaveAsync<T>(string path, T data, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct205<T> stateMachine = default(Struct205<T>);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<bool>.Create();
		stateMachine.fileStorageService_0 = this;
		stateMachine.string_0 = path;
		stateMachine.gparam_0 = data;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct204<>))]
	public Task<T?> LoadAsync<T>(string path, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct204<T> stateMachine = default(Struct204<T>);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<T>.Create();
		stateMachine.fileStorageService_0 = this;
		stateMachine.string_0 = path;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public Task<bool> DeleteAsync(string path, CancellationToken cancellationToken = default(CancellationToken))
	{
		try
		{
			if (File.Exists(path))
			{
				File.Delete(path);
				Logger.Info("[FileStorage] 文件删除成功: " + path);
				return Task.FromResult(result: true);
			}
			Logger.Warning("[FileStorage] 删除失败，文件不存在: " + path);
			return Task.FromResult(result: false);
		}
		catch (Exception ex)
		{
			Logger.Error("[FileStorage] 删除文件失败: " + path, ex);
			return Task.FromResult(result: false);
		}
	}

	public Task<List<string>> ListAsync(string directoryPath, string searchPattern = "*.json", CancellationToken cancellationToken = default(CancellationToken))
	{
		try
		{
			if (!Directory.Exists(directoryPath))
			{
				Logger.Warning("[FileStorage] 目录不存在: " + directoryPath);
				return Task.FromResult(new List<string>());
			}
			return Task.FromResult(new List<string>(Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly)));
		}
		catch (Exception ex)
		{
			Logger.Error("[FileStorage] 列出文件失败: " + directoryPath, ex);
			return Task.FromResult(new List<string>());
		}
	}

	public void EnsureDirectoryExists(string path)
	{
		try
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[FileStorage] 创建目录失败: " + path, ex);
			throw;
		}
	}

	public bool FileExists(string path)
	{
		return File.Exists(path);
	}
}
