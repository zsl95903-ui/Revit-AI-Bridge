using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Product;
using RevitAi.Abstractions.Update;
using RevitAi.Abstractions.Update.Models;
using ns7;

namespace RevitAi.Core.Update;

public sealed class AutoUpdateManager : IDisposable
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct0 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public AutoUpdateManager autoUpdateManager_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AutoUpdateManager autoUpdateManager = autoUpdateManager_0;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_0;
						taskAwaiter_0 = default(TaskAwaiter);
						num = -1;
						int_0 = -1;
						goto IL_00f6;
					}
					awaiter = Task.Delay(TimeSpan.FromSeconds(30L), autoUpdateManager.cancellationTokenSource_0.Token).GetAwaiter();
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
				Logger.Info("[AutoUpdate] 开始后台检查更新...");
				awaiter = autoUpdateManager.method_0(autoUpdateManager.cancellationTokenSource_0.Token).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_0 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
				goto IL_00f6;
				IL_00f6:
				awaiter.GetResult();
			}
			catch (OperationCanceledException)
			{
				Logger.Info("[AutoUpdate] 后台更新检查已取消");
			}
			catch (Exception ex2)
			{
				Logger.Error("[AutoUpdate] 后台更新检查失败: " + ex2.Message, ex2);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct1 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public AutoUpdateManager autoUpdateManager_0;

		public CancellationToken cancellationToken_0;

		private UpdateInfo updateInfo_0;

		private bool bool_0;

		private TaskAwaiter<UpdateInfo?> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		private TaskAwaiter taskAwaiter_2;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AutoUpdateManager autoUpdateManager = autoUpdateManager_0;
			try
			{
				TaskAwaiter<UpdateInfo> awaiter3;
				TaskAwaiter<string> awaiter2;
				TaskAwaiter awaiter;
				string text;
				UpdateInfo result;
				switch (num)
				{
				default:
					awaiter3 = autoUpdateManager.iupdateService_0.CheckForUpdatesAsync(cancellationToken_0).GetAwaiter();
					if (!awaiter3.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter3;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
						return;
					}
					goto IL_0087;
				case 0:
					awaiter3 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<UpdateInfo>);
					num = -1;
					int_0 = -1;
					goto IL_0087;
				case 1:
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0299;
				case 2:
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_02c7;
				case 3:
					{
						awaiter = taskAwaiter_2;
						taskAwaiter_2 = default(TaskAwaiter);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0299:
					text = awaiter2.GetResult();
					bool_0 = true;
					goto IL_02d6;
					IL_0087:
					result = awaiter3.GetResult();
					updateInfo_0 = result;
					if (updateInfo_0 != null)
					{
						string text2 = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi\\Updates"), "RevitAi_v" + updateInfo_0.Version + ".zip");
						bool_0 = false;
						if (File.Exists(text2))
						{
							Logger.Info("[AutoUpdate] 发现本地已有更新包: " + text2);
							if (autoUpdateManager.iupdateService_0.VerifyUpdatePackage(text2, updateInfo_0.Checksum))
							{
								Logger.Info("[AutoUpdate] ✓ 本地更新包校验通过，跳过下载");
								text = text2;
								bool_0 = false;
								goto IL_02d6;
							}
							Logger.Warning("[AutoUpdate] 本地更新包校验失败，将重新下载");
							try
							{
								File.Delete(text2);
							}
							catch (Exception ex)
							{
								Logger.Warning("[AutoUpdate] 删除损坏的更新包失败: " + ex.Message);
							}
							Logger.Info("[AutoUpdate] 开始下载更新包...");
							awaiter2 = autoUpdateManager.iupdateService_0.DownloadUpdateAsync(updateInfo_0, (Action<int>)delegate
							{
							}, cancellationToken_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter2;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
							goto IL_0299;
						}
						Logger.Info("[AutoUpdate] 开始下载更新包...");
						awaiter2 = autoUpdateManager.iupdateService_0.DownloadUpdateAsync(updateInfo_0, (Action<int>)delegate(int int_0)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
							defaultInterpolatedStringHandler.AppendLiteral("[AutoUpdate] 下载进度: ");
							defaultInterpolatedStringHandler.AppendFormatted(int_0);
							defaultInterpolatedStringHandler.AppendLiteral("%");
							Logger.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
						}, cancellationToken_0).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_1 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_02c7;
					}
					Logger.Info("[AutoUpdate] 已是最新版本");
					goto end_IL_000f;
					IL_02c7:
					text = awaiter2.GetResult();
					bool_0 = true;
					goto IL_02d6;
					IL_02d6:
					if (autoUpdateManager.iupdateService_0.VerifyUpdatePackage(text, updateInfo_0.Checksum))
					{
						Logger.Info("[AutoUpdate] ✓ 更新包校验通过");
						awaiter = autoUpdateManager.iupdateService_0.PrepareUpdateAsync(text, updateInfo_0, cancellationToken_0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 3;
							int_0 = 3;
							taskAwaiter_2 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						break;
					}
					Logger.Error("[AutoUpdate] 更新包校验失败，取消更新");
					goto end_IL_000f;
				}
				awaiter.GetResult();
				Logger.Info("[AutoUpdate] ✓ 更新已准备就绪: " + updateInfo_0.Version);
				Logger.Info("[AutoUpdate] 将在关闭 Revit 后自动安装");
				if (bool_0)
				{
					autoUpdateManager.method_3(updateInfo_0.Version);
				}
				updateInfo_0 = null;
				end_IL_000f:;
			}
			catch (OperationCanceledException)
			{
				Logger.Info("[AutoUpdate] 更新检查已取消");
			}
			catch (Exception ex3)
			{
				Logger.Error("[AutoUpdate] 静默更新失败: " + ex3.Message, ex3);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private const int int_0 = 30;

	private readonly IUpdateService iupdateService_0;

	private bool bool_0;

	private CancellationTokenSource? cancellationTokenSource_0;

	private Task? task_0;

	public AutoUpdateManager()
	{
		iupdateService_0 = (IUpdateService)(object)new UpdateService(ProductInfo.Version);
		Logger.Info("[AutoUpdate] 初始化，当前版本: " + ProductInfo.Version);
	}

	public void StartBackgroundUpdateCheck()
	{
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[AutoUpdate] 将在 ");
		defaultInterpolatedStringHandler.AppendFormatted(30);
		defaultInterpolatedStringHandler.AppendLiteral(" 秒后开始检查更新...");
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		cancellationTokenSource_0 = new CancellationTokenSource();
		task_0 = Task.Run([AsyncStateMachine(typeof(Struct0))] () =>
		{
			Struct0 stateMachine = default(Struct0);
			stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
			stateMachine.autoUpdateManager_0 = this;
			stateMachine.int_0 = -1;
			stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
			return stateMachine.asyncTaskMethodBuilder_0.Task;
		}, cancellationTokenSource_0.Token);
	}

	[AsyncStateMachine(typeof(Struct1))]
	private Task method_0(CancellationToken cancellationToken_0)
	{
		Struct1 stateMachine = default(Struct1);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.autoUpdateManager_0 = this;
		stateMachine.cancellationToken_0 = cancellationToken_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public bool HasPendingUpdate()
	{
		return iupdateService_0.HasPendingUpdate();
	}

	public UpdateInfo? GetPendingUpdateInfo()
	{
		return iupdateService_0.GetPendingUpdateInfo();
	}

	public void StartSilentInstaller()
	{
		try
		{
			string text = method_2();
			if (string.IsNullOrEmpty(text))
			{
				Logger.Warning("[AutoUpdate] 无法获取主程序目录");
				return;
			}
			string text2 = Path.Combine(text, "UpdateInstaller.exe");
			if (!File.Exists(text2))
			{
				Logger.Warning("[AutoUpdate] 未找到安装程序: " + text2);
				Logger.Warning("[AutoUpdate] 更新将在下次手动启动时安装");
				return;
			}
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = text2,
				Arguments = "--silent",
				UseShellExecute = true,
				WindowStyle = ProcessWindowStyle.Minimized
			};
			try
			{
				Process process = Process.Start(startInfo);
				if (process != null)
				{
					process.EnableRaisingEvents = false;
				}
				Logger.Info("[AutoUpdate] ✓ 已启动静默安装程序: " + text2);
				Logger.Info("[AutoUpdate] 安装程序将独立运行，不影响 Revit 关闭");
			}
			catch (Exception ex)
			{
				Logger.Error("[AutoUpdate] 启动安装程序进程失败: " + ex.Message);
				throw;
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("[AutoUpdate] 启动安装程序失败: " + ex2.Message, ex2);
		}
	}

	private string? method_1()
	{
		try
		{
			return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		}
		catch (Exception ex)
		{
			Logger.Error("[AutoUpdate] 获取插件目录失败: " + ex.Message, ex);
			return null;
		}
	}

	private string? method_2()
	{
		try
		{
			string text = method_1();
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			DirectoryInfo parent = Directory.GetParent(text);
			if (parent != null)
			{
				string fullName = parent.FullName;
				if (File.Exists(Path.Combine(fullName, "UpdateInstaller.exe")))
				{
					Logger.Debug("[AutoUpdate] 找到主程序目录: " + fullName);
					return fullName;
				}
				Logger.Debug("[AutoUpdate] 父目录中未找到 UpdateInstaller.exe，使用插件目录: " + text);
				return text;
			}
			Logger.Warning("[AutoUpdate] 无法获取父目录，使用插件目录: " + text);
			return text;
		}
		catch (Exception ex)
		{
			Logger.Error("[AutoUpdate] 获取主程序目录失败: " + ex.Message, ex);
			return method_1();
		}
	}

	private void method_3(string string_0)
	{
		try
		{
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly assembly_0) => assembly_0.GetName().Name == "RevitAi.UI");
			if (assembly == null)
			{
				Logger.Warning("[AutoUpdate] 未找到 RevitAi.UI 程序集，跳过更新下载提示");
				return;
			}
			Type type = assembly.GetType("RevitAi.UI.Services.UserNotificationService");
			if (type == null)
			{
				Logger.Warning("[AutoUpdate] 未找到 UserNotificationService 类型，跳过更新下载提示");
				return;
			}
			PropertyInfo property = type.GetProperty("Toast", BindingFlags.Static | BindingFlags.Public);
			if (property == null)
			{
				Logger.Warning("[AutoUpdate] 未找到 UserNotificationService.Toast 属性，跳过更新下载提示");
				return;
			}
			object value = property.GetValue(null);
			if (value == null)
			{
				Logger.Warning("[AutoUpdate] Toast 服务实例为 null，跳过更新下载提示");
				return;
			}
			MethodInfo method = value.GetType().GetMethod("ShowToast", new Type[3]
			{
				typeof(string),
				typeof(string),
				typeof(int)
			});
			if (method == null)
			{
				Logger.Warning("[AutoUpdate] 未找到 Toast.ShowToast 方法，跳过更新下载提示");
				return;
			}
			string text = "RevitAi";
			string text2 = "✓ 更新已下载 (v" + string_0 + ")\n\n请关闭并重新启动 Revit 以完成安装。";
			method.Invoke(value, new object[3] { text, text2, 10 });
			Logger.Info("[AutoUpdate] ✓ 已显示更新下载提示 (v" + string_0 + ")");
		}
		catch (Exception ex)
		{
			Logger.Warning("[AutoUpdate] 显示更新下载提示失败（已忽略）: " + ex.Message);
		}
	}

	public void Dispose()
	{
		if (bool_0)
		{
			return;
		}
		if (cancellationTokenSource_0 != null)
		{
			try
			{
				cancellationTokenSource_0.Cancel();
				cancellationTokenSource_0.Dispose();
				if (task_0 != null)
				{
					task_0.Wait(TimeSpan.FromSeconds(1L));
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[AutoUpdate] 取消后台任务时出错: " + ex.Message);
			}
		}
		if (iupdateService_0 is IDisposable disposable)
		{
			disposable.Dispose();
		}
		bool_0 = true;
	}

	[CompilerGenerated]
	[AsyncStateMachine(typeof(Struct0))]
	private Task? method_4()
	{
		Struct0 stateMachine = default(Struct0);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.autoUpdateManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
