using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

[AITool("search_family_library", Category = "在线族库", Description = "搜索在线族库中的族文件，支持按分类、名称、标签筛选", RequiresTransaction = false, RequiresModification = false, RequiresActiveDocument = false)]
public sealed class SearchFamilyLibraryTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class138
	{
		public string string_0;

		internal bool method_0(FamilyCategory familyCategory_0)
		{
			if (!familyCategory_0.Name.Equals(string_0, StringComparison.OrdinalIgnoreCase))
			{
				return familyCategory_0.Name.Contains(string_0);
			}
			return true;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct221 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public SearchFamilyLibraryTool searchFamilyLibraryTool_0;

		private Class138 class138_0;

		private string string_0;

		private string string_1;

		private bool bool_0;

		private int int_1;

		private Guid? nullable_0;

		private TaskAwaiter<Result<List<FamilyCategory>>> taskAwaiter_0;

		private TaskAwaiter<Result<List<FamilyLibraryItem>>> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SearchFamilyLibraryTool searchFamilyLibraryTool = searchFamilyLibraryTool_0;
			AIToolResult result2;
			try
			{
				TaskAwaiter<Result<List<FamilyLibraryItem>>> awaiter;
				TaskAwaiter<Result<List<FamilyCategory>>> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<Result<List<FamilyLibraryItem>>>);
						num = -1;
						int_0 = -1;
						goto IL_0388;
					}
					this.class138_0 = new Class138();
					string_0 = aitoolContext_0.GetParameter<string>("searchText", (string)null);
					this.class138_0.string_0 = aitoolContext_0.GetParameter<string>("categoryName", (string)null);
					string_1 = aitoolContext_0.GetParameter<string>("revitCategoryName", (string)null);
					bool_0 = aitoolContext_0.GetParameter<bool>("isFree", false);
					int_1 = aitoolContext_0.GetParameter<int>("pageSize", 20);
					if (int_1 < 1)
					{
						int_1 = 20;
					}
					if (int_1 > 100)
					{
						int_1 = 100;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 4);
					defaultInterpolatedStringHandler.AppendLiteral("[AI工具] 搜索在线族库: 关键词=");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					defaultInterpolatedStringHandler.AppendLiteral(", 族库分类=");
					defaultInterpolatedStringHandler.AppendFormatted(this.class138_0.string_0);
					defaultInterpolatedStringHandler.AppendLiteral(", Revit类别=");
					defaultInterpolatedStringHandler.AppendFormatted(string_1);
					defaultInterpolatedStringHandler.AppendLiteral(", 免费=");
					defaultInterpolatedStringHandler.AppendFormatted(bool_0);
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					nullable_0 = null;
					if (string.IsNullOrEmpty(this.class138_0.string_0))
					{
						goto IL_0313;
					}
					awaiter2 = searchFamilyLibraryTool.ifamilyLibraryService_0.GetCategoriesAsync(false).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<List<FamilyCategory>>>);
					num = -1;
					int_0 = -1;
				}
				Result<List<FamilyCategory>> result = awaiter2.GetResult();
				if (!result.IsSuccess || result.Value == null)
				{
					goto IL_0313;
				}
				var class138_0 = this.class138_0;
				FamilyCategory val = result.Value.FirstOrDefault((FamilyCategory familyCategory_0) => familyCategory_0.Name.Equals(class138_0.string_0, StringComparison.OrdinalIgnoreCase) || familyCategory_0.Name.Contains(class138_0.string_0));
				if (val != null)
				{
					nullable_0 = val.Id;
					goto IL_0313;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(18, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("未找到族库分类：");
				defaultInterpolatedStringHandler2.AppendFormatted(this.class138_0.string_0);
				defaultInterpolatedStringHandler2.AppendLiteral("，可用的分类包括：");
				defaultInterpolatedStringHandler2.AppendFormatted(string.Join("、", result.Value.Select((FamilyCategory familyCategory_0) => familyCategory_0.Name).Take(10)));
				defaultInterpolatedStringHandler2.AppendLiteral("等");
				result2 = AIToolResult.Fail(defaultInterpolatedStringHandler2.ToStringAndClear());
				goto end_IL_000f;
				IL_0313:
				awaiter = searchFamilyLibraryTool.ifamilyLibraryService_0.GetFamiliesAsync(nullable_0, string_0, (string[])null, (string)null, string_1, bool_0 ? new bool?(true) : ((bool?)null), 1, int_1, false).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
				goto IL_0388;
				IL_0388:
				Result<List<FamilyLibraryItem>> result3 = awaiter.GetResult();
				if (result3.IsSuccess && result3.Value != null)
				{
					List<FamilyLibraryItem> value = result3.Value;
					List<object> list = new List<object>();
					List<FamilyLibraryItem>.Enumerator enumerator = value.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							FamilyLibraryItem current = enumerator.Current;
							list.Add(new Class19<string, string, string, string, string, bool, int, string[], Class20<int, int, FamilyParamSummary[]>>(current.Id.ToString(), current.DisplayName, current.CategoryName, current.Description, current.RevitVersion, current.IsFree, current.DownloadCount, current.Tags, (current.KeyParams != null) ? new Class20<int, int, FamilyParamSummary[]>(current.KeyParams.TypeParamCount, current.KeyParams.InstanceParamCount, current.KeyParams.CommonTypeParams) : null));
						}
					}
					finally
					{
						if (num < 0)
						{
							((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
						}
					}
					List<string> list2 = new List<string>();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(6, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("找到 ");
					defaultInterpolatedStringHandler3.AppendFormatted(value.Count);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个族");
					list2.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
					if (!string.IsNullOrEmpty(this.class138_0.string_0))
					{
						list2.Add("族库分类：" + this.class138_0.string_0);
					}
					if (!string.IsNullOrEmpty(string_1))
					{
						list2.Add("Revit类别：" + string_1);
					}
					if (bool_0)
					{
						list2.Add("仅免费族");
					}
					result2 = AIToolResult.Ok(string.Join("，", list2), (object)new Class21<int, List<object>, Class22<string, string, string, bool>>(value.Count, list, new Class22<string, string, string, bool>(string_0 ?? "", this.class138_0.string_0 ?? "", string_1 ?? "", bool_0)));
				}
				else
				{
					result2 = AIToolResult.Fail("搜索族库失败：" + result3.Error);
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[AI工具] 搜索在线族库失败", ex);
				result2 = AIToolResult.Fail("搜索失败：" + ex.Message);
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

	private readonly IFamilyLibraryService ifamilyLibraryService_0;

	public string Name => "search_family_library";

	public string Category => "在线族库";

	public string Description => "搜索在线族库中的族文件";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"searchText\": {\n                \"type\": \"string\",\n                \"description\": \"搜索关键词（可选），用于搜索族名称或描述\"\n            },\n            \"categoryName\": {\n                \"type\": \"string\",\n                \"description\": \"族库分类名称（可选），如：门窗家具、结构构件、机电设备等。这是族库的业务分类，与 Revit 类别不同。留空表示搜索所有分类。\"\n            },\n            \"revitCategoryName\": {\n                \"type\": \"string\",\n                \"description\": \"Revit 原生类别名称（可选），如：门、窗、结构柱、墙、楼板、家具、卫浴装置等。这是 Revit 中的类别，与族库分类不同。留空表示搜索所有 Revit 类别。\"\n            },\n            \"isFree\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否只搜索免费族（可选），true 表示只搜索免费族，false 或不传表示搜索所有族。非免费的族需要具有有效授权并且下载次数在24小时下载限额内，才能成功载入。\"\n            },\n            \"pageSize\": {\n                \"type\": \"number\",\n                \"description\": \"每页返回数量（可选），默认 20，最大 100。\"\n            }\n        }\n    }";

	public SearchFamilyLibraryTool(IFamilyLibraryService familyLibraryService)
	{
		ifamilyLibraryService_0 = familyLibraryService ?? throw new ArgumentNullException("familyLibraryService");
	}

	[AsyncStateMachine(typeof(Struct221))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct221 stateMachine = default(Struct221);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.searchFamilyLibraryTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
