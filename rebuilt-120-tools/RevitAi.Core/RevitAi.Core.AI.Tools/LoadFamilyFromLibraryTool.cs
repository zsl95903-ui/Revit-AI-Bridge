using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.FamilyLibrary;
using RevitAi.Abstractions.Logging;
using ns0;
using ns7;

namespace RevitAi.Core.AI.Tools;

[AITool("load_family_from_library", Category = "在线族库", Description = "从在线族库下载指定的族文件并载入到 Revit 项目中", RequiresTransaction = false, RequiresModification = false)]
public sealed class LoadFamilyFromLibraryTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class137
	{
		[StructLayout(LayoutKind.Auto)]
		public struct Struct219 : IAsyncStateMachine
		{
			public int int_0;

			public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

			public Class137 class137_0;

			private TaskAwaiter<bool> taskAwaiter_0;

			void IAsyncStateMachine.MoveNext()
			{
				int num = int_0;
				Class137 @class = this.class137_0;
				TaskAwaiter<bool> awaiter;
				if (num != 0)
				{
					awaiter = @class.loadFamilyFromLibraryTool_0.ifamilyLoadService_0.LoadFamilyAsync(@class.string_0, @class.string_1 ?? Path.GetFileNameWithoutExtension(@class.string_0)).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<bool>);
					num = -1;
					int_0 = -1;
				}
				AIToolResult result = AIToolResult.Ok(awaiter.GetResult() ? "加载成功" : "加载失败", (object)null);
				int_0 = -2;
				asyncTaskMethodBuilder_0.SetResult(result);
			}

			[DebuggerHidden]
			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
			{
				asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
			}
		}

		public LoadFamilyFromLibraryTool loadFamilyFromLibraryTool_0;

		public string string_0;

		public string string_1;

		[AsyncStateMachine(typeof(Struct219))]
		internal Task<AIToolResult> method_0(AIToolContext aitoolContext_0)
		{
			Struct219 stateMachine = default(Struct219);
			stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
			stateMachine.class137_0 = this;
			stateMachine.int_0 = -1;
			stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
			return stateMachine.asyncTaskMethodBuilder_0.Task;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct220 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public LoadFamilyFromLibraryTool loadFamilyFromLibraryTool_0;

		public AIToolContext aitoolContext_0;

		private Class137 class137_0;

		public CancellationToken cancellationToken_0;

		private string string_0;

		private Guid guid_0;

		private string string_1;

		private FamilyDownloadInfo familyDownloadInfo_0;

		private TaskAwaiter<Result<FamilyDownloadInfo>> taskAwaiter_0;

		private HttpClient httpClient_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_1;

		private TaskAwaiter<byte[]> taskAwaiter_2;

		private TaskAwaiter<AIToolResult> taskAwaiter_3;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			LoadFamilyFromLibraryTool loadFamilyFromLibraryTool = loadFamilyFromLibraryTool_0;
			if ((uint)num > 3u)
			{
				this.class137_0 = new Class137();
				this.class137_0.loadFamilyFromLibraryTool_0 = loadFamilyFromLibraryTool_0;
			}
			AIToolResult result;
			try
			{
				TaskAwaiter<Result<FamilyDownloadInfo>> awaiter2;
				TaskAwaiter<AIToolResult> awaiter;
				Result<FamilyDownloadInfo> result4;
				switch (num)
				{
				default:
				{
					string parameter = aitoolContext_0.GetParameter<string>("familyId", (string)null);
					string_0 = aitoolContext_0.GetParameter<string>("familyName", (string)null);
					if (string.IsNullOrEmpty(parameter))
					{
						result = AIToolResult.Fail("族 ID 不能为空");
					}
					else
					{
						if (Guid.TryParse(parameter, out guid_0))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 2);
							defaultInterpolatedStringHandler.AppendLiteral("[AI工具] 从在线族库下载并载入族: ID=");
							defaultInterpolatedStringHandler.AppendFormatted(guid_0);
							defaultInterpolatedStringHandler.AppendLiteral(", 自定义名称=");
							defaultInterpolatedStringHandler.AppendFormatted(string_0);
							Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
							string text = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "FamilyLibrary"), "FamilyFiles");
							Directory.CreateDirectory(text);
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(4, 1);
							defaultInterpolatedStringHandler2.AppendFormatted(guid_0);
							defaultInterpolatedStringHandler2.AppendLiteral(".rfa");
							string_1 = Path.Combine(text, defaultInterpolatedStringHandler2.ToStringAndClear());
							if (File.Exists(string_1))
							{
								Logger.Info("[AI工具] 使用本地缓存: " + string_1);
								this.class137_0.string_0 = string_1;
								this.class137_0.string_1 = ((!string.IsNullOrEmpty(string_0)) ? string_0 : Path.GetFileNameWithoutExtension(string_1));
								goto IL_062f;
							}
							Logger.Info("[AI工具] 请求从服务器下载族文件");
							awaiter2 = loadFamilyFromLibraryTool.ifamilyLibraryService_0.RequestDownloadAsync(guid_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 0;
								int_0 = 0;
								taskAwaiter_0 = awaiter2;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
							goto IL_0267;
						}
						result = AIToolResult.Fail("无效的族 ID 格式：" + parameter + "，必须是 UUID 格式");
					}
					goto end_IL_002f;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<FamilyDownloadInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_0267;
				case 1:
				case 2:
					try
					{
						TaskAwaiter<byte[]> awaiter3;
						TaskAwaiter<HttpResponseMessage> awaiter4;
						if (num != 1)
						{
							if (num == 2)
							{
								awaiter3 = taskAwaiter_2;
								taskAwaiter_2 = default(TaskAwaiter<byte[]>);
								num = -1;
								int_0 = -1;
								goto IL_0550;
							}
							httpClient_0.Timeout = TimeSpan.FromMinutes(5L);
							awaiter4 = httpClient_0.GetAsync(familyDownloadInfo_0.FileUrl, cancellationToken_0).GetAwaiter();
							if (!awaiter4.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter4;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref this);
								return;
							}
						}
						else
						{
							awaiter4 = taskAwaiter_1;
							taskAwaiter_1 = default(TaskAwaiter<HttpResponseMessage>);
							num = -1;
							int_0 = -1;
						}
						HttpResponseMessage result2 = awaiter4.GetResult();
						if (result2.IsSuccessStatusCode)
						{
							awaiter3 = result2.Content.ReadAsByteArrayAsync().GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = 2;
								int_0 = 2;
								taskAwaiter_2 = awaiter3;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
								return;
							}
							goto IL_0550;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(12, 1);
						defaultInterpolatedStringHandler3.AppendLiteral("下载文件失败：HTTP ");
						defaultInterpolatedStringHandler3.AppendFormatted(result2.StatusCode);
						result = AIToolResult.Fail(defaultInterpolatedStringHandler3.ToStringAndClear());
						goto end_IL_002f;
						IL_0550:
						byte[] result3 = awaiter3.GetResult();
						File.WriteAllBytes(this.class137_0.string_0, result3);
					}
					finally
					{
						if (num < 0 && httpClient_0 != null)
						{
							((IDisposable)httpClient_0).Dispose();
						}
					}
					httpClient_0 = null;
					Logger.Info("[AI工具] 文件已下载到: " + this.class137_0.string_0);
					this.class137_0.string_1 = ((!string.IsNullOrEmpty(string_0)) ? string_0 : familyDownloadInfo_0.DisplayName);
					try
					{
						File.Copy(this.class137_0.string_0, string_1, overwrite: true);
						Logger.Info("[AI工具] 文件已缓存到: " + string_1);
					}
					catch (Exception ex)
					{
						Logger.Warning("[AI工具] 缓存文件失败: " + ex.Message);
					}
					familyDownloadInfo_0 = null;
					goto IL_062f;
				case 3:
					{
						awaiter = taskAwaiter_3;
						taskAwaiter_3 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_062f:
					if (aitoolContext_0.Document == null)
					{
						result = AIToolResult.Fail("Revit 文档对象为空");
					}
					else
					{
						if (aitoolContext_0.ExternalEvent != null)
						{
							Logger.Info("[AI工具] 正在载入族到 Revit: " + this.class137_0.string_1);
				var class137_0 = this.class137_0;
							awaiter = new RevitExternalEventHandler(aitoolContext_0.ExternalEvent, null, "Load Family", aitoolContext_0.DataCache).ExecuteAsync(aitoolContext_0, requiresTransaction: true, [AsyncStateMachine(typeof(Class137.Struct219))] (AIToolContext aitoolContext_0) =>
							{
								Class137.Struct219 stateMachine = default(Class137.Struct219);
								stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
								stateMachine.class137_0 = class137_0;
								stateMachine.int_0 = -1;
								stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
								return stateMachine.asyncTaskMethodBuilder_0.Task;
							}).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = 3;
								int_0 = 3;
								taskAwaiter_3 = awaiter;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
							break;
						}
						result = AIToolResult.Fail("无法访问 Revit API，ExternalEvent 未提供");
					}
					goto end_IL_002f;
					IL_0267:
					result4 = awaiter2.GetResult();
					if (result4.IsSuccess && result4.Value != null)
					{
						familyDownloadInfo_0 = result4.Value;
						if (familyDownloadInfo_0.QuotaInfo == null || familyDownloadInfo_0.QuotaInfo.Remaining > 0 || familyDownloadInfo_0.IsFree)
						{
							string text2 = Path.Combine(Path.GetTempPath(), "RevitAi", "FamilyLibrary");
							Directory.CreateDirectory(text2);
							Class137 @class = class137_0;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(4, 1);
							defaultInterpolatedStringHandler4.AppendFormatted(guid_0);
							defaultInterpolatedStringHandler4.AppendLiteral(".rfa");
							@class.string_0 = Path.Combine(text2, defaultInterpolatedStringHandler4.ToStringAndClear());
							httpClient_0 = new HttpClient();
							goto case 1;
						}
						string text3;
						if (familyDownloadInfo_0.QuotaInfo.DailyLimit != 0)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(25, 2);
							defaultInterpolatedStringHandler5.AppendLiteral("今日下载次数已用完（");
							defaultInterpolatedStringHandler5.AppendFormatted(familyDownloadInfo_0.QuotaInfo.Remaining);
							defaultInterpolatedStringHandler5.AppendLiteral("/");
							defaultInterpolatedStringHandler5.AppendFormatted(familyDownloadInfo_0.QuotaInfo.DailyLimit);
							defaultInterpolatedStringHandler5.AppendLiteral("），请明天再试或升级许可证。");
							text3 = defaultInterpolatedStringHandler5.ToStringAndClear();
						}
						else
						{
							text3 = "试用用户无法下载族文件，请购买许可证后使用。";
						}
						string text4 = text3;
						result = AIToolResult.Fail("下载配额不足：" + text4);
					}
					else
					{
						result = AIToolResult.Fail("下载请求失败：" + result4.Error);
					}
					goto end_IL_002f;
				}
				AIToolResult result5 = awaiter.GetResult();
				if (!result5.Success)
				{
					result = AIToolResult.Fail("载入族失败：" + class137_0.string_1 + " - " + result5.Error);
				}
				else
				{
					try
					{
						loadFamilyFromLibraryTool.ifamilyLibraryService_0.RecordDownloadLocally(guid_0);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(14, 1);
						defaultInterpolatedStringHandler6.AppendLiteral("[AI工具] 已记录下载: ");
						defaultInterpolatedStringHandler6.AppendFormatted(guid_0);
						Logger.Info(defaultInterpolatedStringHandler6.ToStringAndClear());
					}
					catch (Exception ex2)
					{
						Logger.Warning("[AI工具] 记录下载失败: " + ex2.Message);
					}
					result = AIToolResult.Ok("✅ 族已成功载入到项目：" + class137_0.string_1, (object)new Class18<string, string, string>(guid_0.ToString(), class137_0.string_1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
				}
				end_IL_002f:;
			}
			catch (OperationCanceledException)
			{
				Logger.Warning("[AI工具] 载入族操作被取消");
				result = AIToolResult.Fail("操作被取消");
			}
			catch (Exception ex4)
			{
				Logger.Error("[AI工具] 载入族失败", ex4);
				result = AIToolResult.Fail("载入失败：" + ex4.Message);
			}
			int_0 = -2;
			class137_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly IFamilyLibraryService ifamilyLibraryService_0;

	private readonly IFamilyLoadService ifamilyLoadService_0;

	public string Name => "load_family_from_library";

	public string Category => "在线族库";

	public string Description => "从在线族库下载并载入族";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"familyId\": {\n                \"type\": \"string\",\n                \"description\": \"族的唯一标识符（UUID 格式），必须使用从 search_family_library 工具获取到的 id。\"\n            },\n            \"familyName\": {\n                \"type\": \"string\",\n                \"description\": \"族名称（必须提供）。必须使用 search_family_library 返回结果中对应族的 name 字段值。这个名称用于在 Revit 中正确显示族，不提供会导致族名显示为无意义的 GUID。\"\n            }\n        },\n        \"required\": [\"familyId\", \"familyName\"]\n    }";

	public LoadFamilyFromLibraryTool(IFamilyLibraryService familyLibraryService, IFamilyLoadService familyLoadService)
	{
		ifamilyLibraryService_0 = familyLibraryService ?? throw new ArgumentNullException("familyLibraryService");
		ifamilyLoadService_0 = familyLoadService ?? throw new ArgumentNullException("familyLoadService");
	}

	[AsyncStateMachine(typeof(Struct220))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct220 stateMachine = default(Struct220);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.loadFamilyFromLibraryTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
