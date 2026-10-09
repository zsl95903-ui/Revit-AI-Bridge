using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Network;
using ns7;

namespace RevitAi.Core.Network;

public sealed class NetworkStatusChecker : IDisposable
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct39 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncVoidMethodBuilder asyncVoidMethodBuilder_0;

		public NetworkStatusChecker networkStatusChecker_0;

		private TaskAwaiter<bool> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			NetworkStatusChecker networkStatusChecker = networkStatusChecker_0;
			TaskAwaiter<bool> awaiter;
			if (num != 0)
			{
				awaiter = networkStatusChecker.CheckNetworkAsync().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					asyncVoidMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
			}
			else
			{
				awaiter = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter<bool>);
				num = -1;
				int_0 = -1;
			}
			awaiter.GetResult();
			int_0 = -2;
			asyncVoidMethodBuilder_0.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncVoidMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct40 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<bool> asyncTaskMethodBuilder_0;

		public NetworkStatusChecker networkStatusChecker_0;

		private string string_0;

		private string string_1;

		private DateTime dateTime_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			NetworkStatusChecker networkStatusChecker = networkStatusChecker_0;
			string text = default(string);
			if (num != 0)
			{
				if (!networkStatusChecker.bool_1)
				{
					text = networkStatusChecker.string_0;
					string_0 = "Supabase 服务器";
					networkStatusChecker.bool_1 = true;
				}
				else
				{
					text = "https://www.baidu.com";
					string_0 = "备用检查点";
				}
				string_1 = ((text == networkStatusChecker.string_0) ? networkStatusChecker.string_1 : text);
				networkStatusChecker.LastCheckedUrl = text;
				networkStatusChecker.LastError = null;
				dateTime_0 = DateTime.UtcNow;
			}
			bool result2;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num != 0)
				{
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, text)
					{
						Headers = 
						{
							{
								"User-Agent",
								"RevitAi-HealthCheck/1.0"
							},
							{
								"Cache-Control",
								"no-cache"
							}
						}
					};
					awaiter = networkStatusChecker.httpClient_0.SendAsync(request).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
				}
				HttpResponseMessage result = awaiter.GetResult();
				try
				{
					_ = (DateTime.UtcNow - dateTime_0).TotalMilliseconds;
					networkStatusChecker.method_0(bool_3: true);
					result2 = true;
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)result)?.Dispose();
					}
				}
			}
			catch (HttpRequestException ex)
			{
				int value = (int)(DateTime.UtcNow - dateTime_0).TotalMilliseconds;
				string value2 = (networkStatusChecker.LastError = "HTTP 请求失败: " + ex.Message);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[NetworkStatusChecker] ✗ ");
				defaultInterpolatedStringHandler.AppendFormatted(value2);
				defaultInterpolatedStringHandler.AppendLiteral(" (耗时 ");
				defaultInterpolatedStringHandler.AppendFormatted(value);
				defaultInterpolatedStringHandler.AppendLiteral("ms)");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(32, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("[NetworkStatusChecker] 检查目标: ");
				defaultInterpolatedStringHandler2.AppendFormatted(string_0);
				defaultInterpolatedStringHandler2.AppendLiteral(" (");
				defaultInterpolatedStringHandler2.AppendFormatted(string_1);
				defaultInterpolatedStringHandler2.AppendLiteral(")");
				Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
				Logger.Warning("[NetworkStatusChecker] 可能原因:");
				Logger.Warning("  - 防火墙或杀毒软件拦截了 Revit 进程");
				Logger.Warning("  - 企业网络需要配置代理服务器");
				Logger.Warning("  - DNS 解析失败");
				Logger.Warning("  - SSL/TLS 证书验证问题");
				Logger.Warning("[NetworkStatusChecker] 建议: 如果浏览器可以访问该地址，请检查防火墙设置或联系 IT 管理员");
				networkStatusChecker.method_0(bool_3: false);
				result2 = false;
			}
			catch (TaskCanceledException)
			{
				int value3 = (int)(DateTime.UtcNow - dateTime_0).TotalMilliseconds;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(8, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("连接超时 (");
				defaultInterpolatedStringHandler3.AppendFormatted(10);
				defaultInterpolatedStringHandler3.AppendLiteral("秒)");
				string value4 = (networkStatusChecker.LastError = defaultInterpolatedStringHandler3.ToStringAndClear());
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(33, 2);
				defaultInterpolatedStringHandler4.AppendLiteral("[NetworkStatusChecker] ✗ ");
				defaultInterpolatedStringHandler4.AppendFormatted(value4);
				defaultInterpolatedStringHandler4.AppendLiteral(" (耗时 ");
				defaultInterpolatedStringHandler4.AppendFormatted(value3);
				defaultInterpolatedStringHandler4.AppendLiteral("ms)");
				Logger.Warning(defaultInterpolatedStringHandler4.ToStringAndClear());
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(32, 2);
				defaultInterpolatedStringHandler5.AppendLiteral("[NetworkStatusChecker] 检查目标: ");
				defaultInterpolatedStringHandler5.AppendFormatted(string_0);
				defaultInterpolatedStringHandler5.AppendLiteral(" (");
				defaultInterpolatedStringHandler5.AppendFormatted(string_1);
				defaultInterpolatedStringHandler5.AppendLiteral(")");
				Logger.Warning(defaultInterpolatedStringHandler5.ToStringAndClear());
				Logger.Warning("[NetworkStatusChecker] 可能原因:");
				Logger.Warning("  - 网络连接缓慢");
				Logger.Warning("  - 防火墙阻止了连接");
				Logger.Warning("  - 代理服务器配置问题");
				Logger.Warning("[NetworkStatusChecker] 提示: 浏览器可以访问不表示应用程序也能访问，请检查系统代理设置");
				networkStatusChecker.method_0(bool_3: false);
				result2 = false;
			}
			catch (Exception ex3)
			{
				int value5 = (int)(DateTime.UtcNow - dateTime_0).TotalMilliseconds;
				string value6 = (networkStatusChecker.LastError = ex3.GetType().Name + ": " + ex3.Message);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(33, 2);
				defaultInterpolatedStringHandler6.AppendLiteral("[NetworkStatusChecker] ✗ ");
				defaultInterpolatedStringHandler6.AppendFormatted(value6);
				defaultInterpolatedStringHandler6.AppendLiteral(" (耗时 ");
				defaultInterpolatedStringHandler6.AppendFormatted(value5);
				defaultInterpolatedStringHandler6.AppendLiteral("ms)");
				Logger.Warning(defaultInterpolatedStringHandler6.ToStringAndClear());
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(32, 2);
				defaultInterpolatedStringHandler7.AppendLiteral("[NetworkStatusChecker] 检查目标: ");
				defaultInterpolatedStringHandler7.AppendFormatted(string_0);
				defaultInterpolatedStringHandler7.AppendLiteral(" (");
				defaultInterpolatedStringHandler7.AppendFormatted(string_1);
				defaultInterpolatedStringHandler7.AppendLiteral(")");
				Logger.Warning(defaultInterpolatedStringHandler7.ToStringAndClear());
				Logger.Warning("[NetworkStatusChecker] 堆栈: " + ex3.StackTrace);
				networkStatusChecker.method_0(bool_3: false);
				result2 = false;
			}
			int_0 = -2;
			string_0 = null;
			string_1 = null;
			asyncTaskMethodBuilder_0.SetResult(result2);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly HttpClient httpClient_0;

	private readonly IHttpClientFactory? ihttpClientFactory_0;

	private readonly HttpClientHandler httpClientHandler_0;

	private readonly string string_0;

	private readonly string string_1;

	private Timer? timer_0;

	private bool bool_0 = true;

	private bool bool_1;

	private const int int_0 = 300;

	private const int int_1 = 10;

	private const string string_2 = "https://www.baidu.com";

	[CompilerGenerated]
	private EventHandler<bool>? eventHandler_0;

	[CompilerGenerated]
	private bool bool_2 = true;

	[CompilerGenerated]
	private string? string_3;

	[CompilerGenerated]
	private string? string_4;

	public bool IsNetworkAvailable
	{
		[CompilerGenerated]
		get
		{
			return bool_2;
		}
		[CompilerGenerated]
		private set
		{
			bool_2 = value;
		}
	}

	public string? LastError
	{
		[CompilerGenerated]
		get
		{
			return string_3;
		}
		[CompilerGenerated]
		private set
		{
			string_3 = value;
		}
	}

	public string? LastCheckedUrl
	{
		[CompilerGenerated]
		get
		{
			return string_4;
		}
		[CompilerGenerated]
		private set
		{
			string_4 = value;
		}
	}

	public event EventHandler<bool>? NetworkStatusChanged
	{
		[CompilerGenerated]
		add
		{
			EventHandler<bool> eventHandler = eventHandler_0;
			EventHandler<bool> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler<bool> value2 = (EventHandler<bool>)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref eventHandler_0, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			EventHandler<bool> eventHandler = eventHandler_0;
			EventHandler<bool> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler<bool> value2 = (EventHandler<bool>)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref eventHandler_0, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	public NetworkStatusChecker(string supabaseUrl)
		: this(supabaseUrl, null)
	{
	}

	public NetworkStatusChecker(string supabaseUrl, IHttpClientFactory? httpClientFactory)
	{
		string_0 = supabaseUrl ?? throw new ArgumentNullException("supabaseUrl");
		string_1 = smethod_0(supabaseUrl);
		ihttpClientFactory_0 = httpClientFactory;
		if (ihttpClientFactory_0 != null)
		{
			httpClient_0 = ihttpClientFactory_0.CreateClient((string)null, 10);
			httpClientHandler_0 = (httpClient_0.GetType().GetField("_handler", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(httpClient_0) as HttpClientHandler) ?? new HttpClientHandler();
		}
		else
		{
			Logger.Info("[NetworkStatusChecker] 使用默认方式创建 HttpClient（建议注入 HttpClientFactory）");
			HttpClientHandler httpClientHandler = new HttpClientHandler
			{
				UseProxy = true,
				SslProtocols = (SslProtocols.Tls12 | SslProtocols.Tls13),
				AllowAutoRedirect = true,
				MaxAutomaticRedirections = 5
			};
			try
			{
				IWebProxy systemWebProxy = WebRequest.GetSystemWebProxy();
				if (systemWebProxy != null)
				{
					httpClientHandler.Proxy = systemWebProxy;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[NetworkStatusChecker] 获取系统代理失败: " + ex.Message);
			}
			httpClientHandler_0 = httpClientHandler;
			httpClient_0 = new HttpClient(httpClientHandler)
			{
				Timeout = TimeSpan.FromSeconds(10L)
			};
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[NetworkStatusChecker] 初始化完成，超时设置: ");
		defaultInterpolatedStringHandler.AppendFormatted(10);
		defaultInterpolatedStringHandler.AppendLiteral(" 秒");
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
	}

	[AsyncStateMachine(typeof(Struct40))]
	public Task<bool> CheckNetworkAsync()
	{
		Struct40 stateMachine = default(Struct40);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<bool>.Create();
		stateMachine.networkStatusChecker_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public void StartPeriodicCheck(int intervalSeconds = 300)
	{
		if (timer_0 != null)
		{
			Logger.Warning("[NetworkStatusChecker] 定时检查已在运行");
			return;
		}
		timer_0 = new Timer([AsyncStateMachine(typeof(Struct39))] (object? object_0) =>
		{
			Struct39 stateMachine = default(Struct39);
			stateMachine.asyncVoidMethodBuilder_0 = AsyncVoidMethodBuilder.Create();
			stateMachine.networkStatusChecker_0 = this;
			stateMachine.int_0 = -1;
			stateMachine.asyncVoidMethodBuilder_0.Start(ref stateMachine);
		}, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervalSeconds));
	}

	public void StopPeriodicCheck()
	{
		if (timer_0 != null)
		{
			Logger.Info("[NetworkStatusChecker] 停止定期网络检查");
			timer_0.Dispose();
			timer_0 = null;
		}
	}

	private void method_0(bool bool_3)
	{
		if (bool_3 != bool_0)
		{
			bool_0 = bool_3;
			IsNetworkAvailable = bool_3;
			if (bool_3)
			{
				Logger.Info("[NetworkStatusChecker] ✓ 网络已恢复");
			}
			else
			{
				Logger.Warning("[NetworkStatusChecker] ✗ 网络已断开");
				if (!string.IsNullOrEmpty(LastError))
				{
					Logger.Warning("[NetworkStatusChecker] 错误信息: " + LastError);
				}
			}
			eventHandler_0?.Invoke(this, bool_3);
		}
		else
		{
			IsNetworkAvailable = bool_3;
		}
	}

	public void ResetSupabaseCheck()
	{
		bool_1 = false;
	}

	private static string smethod_0(string string_5)
	{
		if (string.IsNullOrEmpty(string_5))
		{
			return "<null>";
		}
		try
		{
			Uri uri = new Uri(string_5);
			string host = uri.Host;
			if (host.Length > 8)
			{
				string text = host.Substring(0, 4) + "***" + host.Substring(host.Length - 4);
				return uri.Scheme + "://" + text + "/";
			}
			return uri.Scheme + "://<masked>/";
		}
		catch
		{
			return "<invalid_url>";
		}
	}

	public string GetDiagnosticInfo()
	{
		string value = "未配置";
		try
		{
			IWebProxy proxy = httpClientHandler_0.Proxy;
			object obj;
			if (proxy != null)
			{
				Uri? proxy2 = proxy.GetProxy(new Uri(string_0));
				if ((object)proxy2 == null)
				{
					obj = null;
				}
				else
				{
					obj = proxy2.ToString();
					if (obj != null)
					{
						goto IL_0045;
					}
				}
				obj = "unknown";
				goto IL_0045;
			}
			goto end_IL_000b;
			IL_0045:
			string text = smethod_0((string)obj);
			value = "已配置 (" + text + ")";
			end_IL_000b:;
		}
		catch
		{
			value = "已配置（地址未知）";
		}
		string value2 = ((LastCheckedUrl == null) ? "未检查" : ((LastCheckedUrl == string_0) ? string_1 : LastCheckedUrl));
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(261, 5);
		defaultInterpolatedStringHandler.AppendLiteral("\n网络诊断信息:\n===========================================\n当前状态: ");
		defaultInterpolatedStringHandler.AppendFormatted(IsNetworkAvailable ? "✓ 已连接" : "✗ 已断开");
		defaultInterpolatedStringHandler.AppendLiteral("\n最后检查 URL: ");
		defaultInterpolatedStringHandler.AppendFormatted(value2);
		defaultInterpolatedStringHandler.AppendLiteral("\n最后错误: ");
		defaultInterpolatedStringHandler.AppendFormatted(LastError ?? "无");
		defaultInterpolatedStringHandler.AppendLiteral("\n检查超时: ");
		defaultInterpolatedStringHandler.AppendFormatted(10);
		defaultInterpolatedStringHandler.AppendLiteral(" 秒\n代理状态: ");
		defaultInterpolatedStringHandler.AppendFormatted(value);
		defaultInterpolatedStringHandler.AppendLiteral("\n===========================================\n\n故障排查建议:\n1. 检查 Windows 防火墙是否限制了 Revit.exe\n2. 检查杀毒软件是否拦截了网络请求\n3. 如果是企业网络，联系 IT 管理员配置代理\n4. 尝试临时关闭防火墙/杀毒软件测试\n5. 确认系统日期和时间设置正确\n");
		return defaultInterpolatedStringHandler.ToStringAndClear();
	}

	public void Dispose()
	{
		StopPeriodicCheck();
		httpClient_0?.Dispose();
	}

	[CompilerGenerated]
	[AsyncStateMachine(typeof(Struct39))]
	private void method_1(object? object_0)
	{
		Struct39 stateMachine = default(Struct39);
		stateMachine.asyncVoidMethodBuilder_0 = AsyncVoidMethodBuilder.Create();
		stateMachine.networkStatusChecker_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncVoidMethodBuilder_0.Start(ref stateMachine);
	}
}
