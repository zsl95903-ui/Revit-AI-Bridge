using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Abstractions.Network;
using ns7;

namespace RevitAi.Core.Services;

public class TileDownloader : IDisposable
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct32 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public TileDownloader tileDownloader_0;

		private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter configuredTaskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TileDownloader tileDownloader = tileDownloader_0;
			ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter;
			if (num != 0)
			{
				int num2 = tileDownloader.random_0.Next(tileDownloader.tileDownloadConfig_0.MinDelayMs, tileDownloader.tileDownloadConfig_0.MaxDelayMs);
				if (num2 <= 0)
				{
					goto IL_009b;
				}
				awaiter = Task.Delay(num2).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
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
			goto IL_009b;
			IL_009b:
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
	public struct Struct33 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<TileCoordinate> asyncTaskMethodBuilder_0;

		public TileDownloader tileDownloader_0;

		public TileCoordinate tileCoordinate_0;

		private int int_1;

		private int int_2;

		private ConfiguredTaskAwaitable<HttpResponseMessage>.ConfiguredTaskAwaiter configuredTaskAwaiter_0;

		private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter configuredTaskAwaiter_1;

		private ConfiguredTaskAwaitable<byte[]>.ConfiguredTaskAwaiter configuredTaskAwaiter_2;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TileDownloader tileDownloader = tileDownloader_0;
			ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter;
			TileCoordinate result;
			object obj;
			int num2;
			switch (num)
			{
			default:
				int_1 = 0;
				int_2 = tileDownloader.tileDownloadConfig_0.MaxRetries;
				goto IL_0610;
			case 5:
				awaiter = configuredTaskAwaiter_1;
				configuredTaskAwaiter_1 = default(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter);
				num = -1;
				int_0 = -1;
				goto IL_0609;
			case 6:
				awaiter = configuredTaskAwaiter_1;
				configuredTaskAwaiter_1 = default(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter);
				num = -1;
				int_0 = -1;
				goto IL_008c;
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
				{
					try
					{
						ConfiguredTaskAwaitable<HttpResponseMessage>.ConfiguredTaskAwaiter awaiter4;
						ConfiguredTaskAwaitable<byte[]>.ConfiguredTaskAwaiter awaiter3;
						TaskAwaiter awaiter2;
						byte[] result2;
						string string_;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2;
						string text;
						HttpResponseMessage result3;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3;
						switch (num)
						{
						default:
							if (tileDownloader.random_0.Next(100) < 10)
							{
								tileDownloader.method_2();
							}
							awaiter4 = tileDownloader.httpClient_0.GetAsync(tileCoordinate_0.Url).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
							if (!awaiter4.IsCompleted)
							{
								num = 0;
								int_0 = 0;
								configuredTaskAwaiter_0 = awaiter4;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref this);
								return;
							}
							goto IL_0142;
						case 0:
							awaiter4 = configuredTaskAwaiter_0;
							configuredTaskAwaiter_0 = default(ConfiguredTaskAwaitable<HttpResponseMessage>.ConfiguredTaskAwaiter);
							num = -1;
							int_0 = -1;
							goto IL_0142;
						case 1:
							awaiter = configuredTaskAwaiter_1;
							configuredTaskAwaiter_1 = default(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter);
							num = -1;
							int_0 = -1;
							goto IL_0304;
						case 2:
							awaiter = configuredTaskAwaiter_1;
							configuredTaskAwaiter_1 = default(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter);
							num = -1;
							int_0 = -1;
							goto IL_033f;
						case 3:
							awaiter3 = configuredTaskAwaiter_2;
							configuredTaskAwaiter_2 = default(ConfiguredTaskAwaitable<byte[]>.ConfiguredTaskAwaiter);
							num = -1;
							int_0 = -1;
							goto IL_037a;
						case 4:
							{
								awaiter2 = taskAwaiter_0;
								taskAwaiter_0 = default(TaskAwaiter);
								num = -1;
								int_0 = -1;
								break;
							}
							IL_0304:
							awaiter.GetResult();
							int_1++;
							goto IL_0610;
							IL_033f:
							awaiter.GetResult();
							int_1++;
							goto IL_0610;
							IL_037a:
							result2 = awaiter3.GetResult();
							if (result2.Length < 1000)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
								defaultInterpolatedStringHandler.AppendLiteral("[TileDownloader] 图片异常小 (");
								defaultInterpolatedStringHandler.AppendFormatted(result2.Length);
								defaultInterpolatedStringHandler.AppendLiteral(" bytes)");
								Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
								int_1++;
								awaiter2 = Task.Delay(1000 * int_1).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = 4;
									int_0 = 4;
									taskAwaiter_0 = awaiter2;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
									return;
								}
								break;
							}
							string_ = tileDownloader.string_0;
							defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(6, 3);
							defaultInterpolatedStringHandler2.AppendFormatted(tileCoordinate_0.X);
							defaultInterpolatedStringHandler2.AppendLiteral("_");
							defaultInterpolatedStringHandler2.AppendFormatted(tileCoordinate_0.Y);
							defaultInterpolatedStringHandler2.AppendLiteral("_");
							defaultInterpolatedStringHandler2.AppendFormatted(tileCoordinate_0.Zoom);
							defaultInterpolatedStringHandler2.AppendLiteral(".png");
							text = Path.Combine(string_, defaultInterpolatedStringHandler2.ToStringAndClear());
							File.WriteAllBytes(text, result2);
							tileCoordinate_0.LocalPath = text;
							tileCoordinate_0.IsDownloaded = true;
							tileCoordinate_0.RetryCount = int_1;
							result = tileCoordinate_0;
							goto end_IL_00a0;
							IL_0142:
							result3 = awaiter4.GetResult();
							if (result3.StatusCode != HttpStatusCode.Forbidden && result3.StatusCode != HttpStatusCode.Unauthorized)
							{
								if (result3.StatusCode == HttpStatusCode.TooManyRequests)
								{
									Logger.Warning("[TileDownloader] 请求过于频繁，触发限流");
									awaiter = Task.Delay(10000 * (int_1 + 1)).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
									if (!awaiter.IsCompleted)
									{
										num = 2;
										int_0 = 2;
										configuredTaskAwaiter_1 = awaiter;
										asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
										return;
									}
									goto IL_033f;
								}
								if (result3.StatusCode == (HttpStatusCode)418)
								{
									Logger.Error("[TileDownloader] 天地图反爬虫检测（HTTP 418），建议切换到 Google Maps");
									throw new HttpRequestException("天地图反爬虫检测（HTTP 418），建议切换到 Google Maps");
								}
								result3.EnsureSuccessStatusCode();
								awaiter3 = result3.Content.ReadAsByteArrayAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
								if (!awaiter3.IsCompleted)
								{
									num = 3;
									int_0 = 3;
									configuredTaskAwaiter_2 = awaiter3;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
									return;
								}
								goto IL_037a;
							}
							defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(24, 1);
							defaultInterpolatedStringHandler3.AppendLiteral("[TileDownloader] 访问被拒绝: ");
							defaultInterpolatedStringHandler3.AppendFormatted<TileCoordinate>(tileCoordinate_0);
							Logger.Error(defaultInterpolatedStringHandler3.ToStringAndClear());
							awaiter = Task.Delay(5000 * (int_1 + 1)).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								configuredTaskAwaiter_1 = awaiter;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
							goto IL_0304;
						}
						awaiter2.GetResult();
						goto IL_0610;
						end_IL_00a0:;
					}
					catch (HttpRequestException ex)
					{
						obj = ex;
						num2 = 1;
						goto IL_0528;
					}
					catch (Exception ex2)
					{
						obj = ex2;
						num2 = 2;
						goto IL_0528;
					}
					break;
				}
				IL_0610:
				if (int_1 < int_2)
				{
					num2 = 0;
					goto case 0;
				}
				result = tileCoordinate_0;
				break;
				IL_0609:
				awaiter.GetResult();
				goto IL_0610;
				IL_008c:
				awaiter.GetResult();
				goto IL_0610;
				IL_0528:
				if (num2 != 1)
				{
					if (num2 != 2)
					{
						goto IL_0610;
					}
					Exception ex3 = (Exception)obj;
					int_1++;
					tileCoordinate_0.RetryCount = int_1;
					if (int_1 >= int_2)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(41, 2);
						defaultInterpolatedStringHandler4.AppendLiteral("[TileDownloader] 瓦片下载失败（已达最大重试次数）: ");
						defaultInterpolatedStringHandler4.AppendFormatted<TileCoordinate>(tileCoordinate_0);
						defaultInterpolatedStringHandler4.AppendLiteral(", 错误: ");
						defaultInterpolatedStringHandler4.AppendFormatted(ex3.Message);
						Logger.Error(defaultInterpolatedStringHandler4.ToStringAndClear());
						ExceptionDispatchInfo.Capture((obj as Exception) ?? throw (Exception)obj).Throw();
					}
					awaiter = Task.Delay(1000 * int_1 + tileDownloader.random_0.Next(500, 1500)).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 6;
						int_0 = 6;
						configuredTaskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_008c;
				}
				int_1++;
				tileCoordinate_0.RetryCount = int_1;
				if (int_1 >= int_2)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(35, 1);
					defaultInterpolatedStringHandler5.AppendLiteral("[TileDownloader] 瓦片下载失败（已达最大重试次数）: ");
					defaultInterpolatedStringHandler5.AppendFormatted<TileCoordinate>(tileCoordinate_0);
					Logger.Error(defaultInterpolatedStringHandler5.ToStringAndClear());
					ExceptionDispatchInfo.Capture((obj as Exception) ?? throw (Exception)obj).Throw();
				}
				awaiter = Task.Delay((int)Math.Pow(2.0, int_1) * 1000 + tileDownloader.random_0.Next(500, 1500)).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 5;
					int_0 = 5;
					configuredTaskAwaiter_1 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
				goto IL_0609;
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
	public struct Struct34 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<List<TileCoordinate>> asyncTaskMethodBuilder_0;

		public TileDownloader tileDownloader_0;

		public int int_1;

		public List<TileCoordinate> list_0;

		private int int_2;

		private int int_3;

		private int int_4;

		private List<TileCoordinate> list_1;

		private List<TileCoordinate>.Enumerator enumerator_0;

		private TileCoordinate tileCoordinate_0;

		private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter configuredTaskAwaiter_0;

		private ConfiguredTaskAwaitable<TileCoordinate>.ConfiguredTaskAwaiter configuredTaskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Expected O, but got Unknown
			int num = int_0;
			TileDownloader tileDownloader = tileDownloader_0;
			List<TileCoordinate> result2;
			try
			{
				if ((uint)num > 2u)
				{
					tileDownloader.tileDownloadConfig_0.MaxConcurrentRequests = Math.Min(int_1, 3);
					int_2 = list_0.Count;
					int_3 = 0;
					int_4 = 0;
					list_1 = new List<TileCoordinate>();
					enumerator_0 = list_0.GetEnumerator();
				}
				try
				{
					if (num != 0)
					{
						if ((uint)(num - 1) <= 1u)
						{
							goto IL_0096;
						}
						goto IL_02de;
					}
					ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter = configuredTaskAwaiter_0;
					configuredTaskAwaiter_0 = default(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter);
					num = -1;
					int_0 = -1;
					goto IL_0320;
					IL_0320:
					awaiter.GetResult();
					goto IL_0096;
					IL_0096:
					try
					{
						ConfiguredTaskAwaitable<TileCoordinate>.ConfiguredTaskAwaiter awaiter2;
						if (num != 1)
						{
							if (num == 2)
							{
								awaiter2 = configuredTaskAwaiter_1;
								configuredTaskAwaiter_1 = default(ConfiguredTaskAwaitable<TileCoordinate>.ConfiguredTaskAwaiter);
								num = -1;
								int_0 = -1;
								goto IL_016c;
							}
							awaiter = tileDownloader.method_1().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = 1;
								int_0 = 1;
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
						awaiter2 = tileDownloader.method_0(tileCoordinate_0).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							configuredTaskAwaiter_1 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_016c;
						IL_016c:
						TileCoordinate result = awaiter2.GetResult();
						if (result == null || !result.IsDownloaded)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
							defaultInterpolatedStringHandler.AppendLiteral("瓦片 ");
							defaultInterpolatedStringHandler.AppendFormatted<TileCoordinate>(tileCoordinate_0);
							defaultInterpolatedStringHandler.AppendLiteral(" 下载失败（返回空或未标记为已下载）");
							throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						list_1.Add(result);
						int_3++;
						tileDownloader.eventHandler_0?.Invoke(tileDownloader, new TileDownloadProgressEventArgs
						{
							TotalTiles = int_2,
							CompletedTiles = int_3,
							FailedTiles = int_4,
							SuccessTiles = list_1.Count,
							CurrentTile = tileCoordinate_0
						});
					}
					catch (Exception ex)
					{
						int_4++;
						int_3++;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(31, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("[TileDownloader] 瓦片下载失败: ");
						defaultInterpolatedStringHandler2.AppendFormatted<TileCoordinate>(tileCoordinate_0);
						defaultInterpolatedStringHandler2.AppendLiteral(", 错误: ");
						defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
						Logger.Error(defaultInterpolatedStringHandler2.ToStringAndClear());
						throw;
					}
					finally
					{
						if (num < 0)
						{
							tileDownloader.semaphoreSlim_0.Release();
						}
					}
					tileCoordinate_0 = null;
					goto IL_02de;
					IL_02de:
					if (enumerator_0.MoveNext())
					{
						tileCoordinate_0 = enumerator_0.Current;
						awaiter = tileDownloader.semaphoreSlim_0.WaitAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							configuredTaskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0320;
					}
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
					}
				}
				enumerator_0 = default(List<TileCoordinate>.Enumerator);
				result2 = list_1;
			}
			catch (Exception ex2)
			{
				Logger.Error("[TileDownloader] 批量下载失败: " + ex2.Message);
				result2 = new List<TileCoordinate>();
			}
			int_0 = -2;
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

	private readonly string string_0;

	private readonly Random random_0;

	private readonly TileDownloadConfig tileDownloadConfig_0;

	private readonly SemaphoreSlim semaphoreSlim_0;

	private bool bool_0;

	private static readonly string[] string_1 = new string[6]
	{
		"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:121.0) Gecko/20100101 Firefox/121.0",
		"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:120.0) Gecko/20100101 Firefox/120.0",
		"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
		"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.1 Safari/605.1.15"
	};

	[CompilerGenerated]
	private EventHandler<TileDownloadProgressEventArgs>? eventHandler_0;

	public event EventHandler<TileDownloadProgressEventArgs>? DownloadProgress
	{
		[CompilerGenerated]
		add
		{
			EventHandler<TileDownloadProgressEventArgs> eventHandler = eventHandler_0;
			EventHandler<TileDownloadProgressEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler<TileDownloadProgressEventArgs> value2 = (EventHandler<TileDownloadProgressEventArgs>)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref eventHandler_0, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			EventHandler<TileDownloadProgressEventArgs> eventHandler = eventHandler_0;
			EventHandler<TileDownloadProgressEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler<TileDownloadProgressEventArgs> value2 = (EventHandler<TileDownloadProgressEventArgs>)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref eventHandler_0, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	public TileDownloader(TileDownloadConfig? config = null, IHttpClientFactory? httpClientFactory = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		tileDownloadConfig_0 = (TileDownloadConfig)(((object)config) ?? ((object)new TileDownloadConfig()));
		random_0 = new Random();
		ihttpClientFactory_0 = httpClientFactory;
		semaphoreSlim_0 = new SemaphoreSlim(tileDownloadConfig_0.MaxConcurrentRequests, tileDownloadConfig_0.MaxConcurrentRequests);
		if (ihttpClientFactory_0 != null)
		{
			httpClient_0 = ihttpClientFactory_0.CreateClient((string)null, tileDownloadConfig_0.RequestTimeoutSeconds);
		}
		else
		{
			httpClient_0 = new HttpClient
			{
				Timeout = TimeSpan.FromSeconds(tileDownloadConfig_0.RequestTimeoutSeconds)
			};
		}
		int num = random_0.Next(string_1.Length);
		httpClient_0.DefaultRequestHeaders.Add("User-Agent", string_1[num]);
		httpClient_0.DefaultRequestHeaders.Add("Accept", "image/webp,image/apng,image/*,*/*;q=0.8");
		httpClient_0.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
		httpClient_0.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
		httpClient_0.DefaultRequestHeaders.Add("DNT", "1");
		httpClient_0.DefaultRequestHeaders.Add("Connection", "keep-alive");
		string_0 = Path.Combine(Path.GetTempPath(), "RevitAi", "Tiles", Guid.NewGuid().ToString());
		Directory.CreateDirectory(string_0);
	}

	[AsyncStateMachine(typeof(Struct34))]
	public Task<List<TileCoordinate>> DownloadTilesAsync(List<TileCoordinate> tiles, int maxConcurrent = 2)
	{
		Struct34 stateMachine = default(Struct34);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<List<TileCoordinate>>.Create();
		stateMachine.tileDownloader_0 = this;
		stateMachine.list_0 = tiles;
		stateMachine.int_1 = maxConcurrent;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct33))]
	private Task<TileCoordinate> method_0(TileCoordinate tileCoordinate_0)
	{
		Struct33 stateMachine = default(Struct33);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<TileCoordinate>.Create();
		stateMachine.tileDownloader_0 = this;
		stateMachine.tileCoordinate_0 = tileCoordinate_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct32))]
	private Task method_1()
	{
		Struct32 stateMachine = default(Struct32);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.tileDownloader_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private void method_2()
	{
		try
		{
			httpClient_0.DefaultRequestHeaders.Remove("User-Agent");
			int num = random_0.Next(string_1.Length);
			httpClient_0.DefaultRequestHeaders.Add("User-Agent", string_1[num]);
		}
		catch (Exception ex)
		{
			Logger.Warning("[TileDownloader] 轮换 User-Agent 失败: " + ex.Message);
		}
	}

	public void Cleanup()
	{
		try
		{
			if (Directory.Exists(string_0))
			{
				Directory.Delete(string_0, recursive: true);
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[TileDownloader] 清理临时目录失败: " + ex.Message);
		}
	}

	public void Dispose()
	{
		if (!bool_0)
		{
			httpClient_0?.Dispose();
			semaphoreSlim_0?.Dispose();
			bool_0 = true;
		}
	}
}
