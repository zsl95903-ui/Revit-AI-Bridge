using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("duplicate_family_type", Category = "族管理", Description = "复制现有族类型创建新类型。注意：新类型会继承源类型的所有参数值。如需修改参数值，请使用 set_parameter_values 工具", RequiresTransaction = true, RequiresModification = true)]
public sealed class DuplicateFamilyTypeTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class440 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public DuplicateFamilyTypeTool duplicateFamilyTypeTool_0;

		private int int_1;

		private string string_0;

		private IFamilyService ifamilyService_0;

		private IElementService ielementService_0;

		private object object_0;

		private string string_1;

		private string string_2;

		private object object_1;

		private int? nullable_0;

		private string string_3;

		private string string_4;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class440 stateMachine = this;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
					return;
				}
			}
			else
			{
				awaiter = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter);
				int num = -1;
				int_0 = -1;
			}
			awaiter.GetResult();
			AIToolResult result;
			try
			{
				int_1 = aitoolContext_0.GetParameter<int>("sourceTypeId", 0);
				string_0 = aitoolContext_0.GetParameter<string>("newTypeName", (string)null);
				if (string.IsNullOrWhiteSpace(string_0))
				{
					result = AIToolResult.Fail("新类型名称不能为空");
				}
				else
				{
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					ifamilyService_0 = ((revitAdapter != null) ? revitAdapter.FamilyService : null);
					IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
					ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
					if (ifamilyService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 FamilyService");
					}
					else if (ielementService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 ElementService");
					}
					else if (aitoolContext_0.Document == null)
					{
						result = AIToolResult.Fail("文档对象为空");
					}
					else
					{
						object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
						if (object_0 == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
							defaultInterpolatedStringHandler.AppendLiteral("找不到源类型 ID ");
							defaultInterpolatedStringHandler.AppendFormatted(int_1);
							result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						else
						{
							string_1 = ielementService_0.GetElementName(object_0) ?? "未知类型";
							string_2 = ielementService_0.GetElementCategory(object_0) ?? "未知类别";
							object_1 = ifamilyService_0.DuplicateFamilyType(aitoolContext_0.Document, int_1, string_0, (IDictionary<string, object>)null);
							if (object_1 == null)
							{
								result = AIToolResult.Fail("复制类型失败：可能类型名称 '" + string_0 + "' 已存在或源类型无效");
							}
							else
							{
								nullable_0 = ielementService_0.GetElementId(object_1);
								string_3 = ielementService_0.GetElementName(object_1) ?? string_0;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(102, 4);
								defaultInterpolatedStringHandler2.AppendLiteral("✅ 成功复制族类型\n\n");
								defaultInterpolatedStringHandler2.AppendLiteral("📋 源类型：");
								defaultInterpolatedStringHandler2.AppendFormatted(string_1);
								defaultInterpolatedStringHandler2.AppendLiteral("\n");
								defaultInterpolatedStringHandler2.AppendLiteral("✨ 新类型：");
								defaultInterpolatedStringHandler2.AppendFormatted(string_3);
								defaultInterpolatedStringHandler2.AppendLiteral("\n");
								defaultInterpolatedStringHandler2.AppendLiteral("🏷️ 类别：");
								defaultInterpolatedStringHandler2.AppendFormatted(string_2);
								defaultInterpolatedStringHandler2.AppendLiteral("\n");
								defaultInterpolatedStringHandler2.AppendLiteral("🆔 新类型 ID：");
								defaultInterpolatedStringHandler2.AppendFormatted(nullable_0);
								defaultInterpolatedStringHandler2.AppendLiteral("\n\n");
								defaultInterpolatedStringHandler2.AppendLiteral("💡 提示：新类型已继承源类型的所有参数值。");
								defaultInterpolatedStringHandler2.AppendLiteral("如需修改参数，请使用 set_parameter_values 工具");
								string_4 = defaultInterpolatedStringHandler2.ToStringAndClear();
								result = AIToolResult.Ok(string_4, (object)new Class111<int, string, int?, string, string>(int_1, string_1, nullable_0, string_3, string_2));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("复制族类型失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "duplicate_family_type";

	public string Category => "族管理";

	public string Description => "复制现有族类型创建新类型";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"sourceTypeId\": {\n                \"type\": \"integer\",\n                \"description\": \"源类型 ID（要复制的族类型 ID）\"\n            },\n            \"newTypeName\": {\n                \"type\": \"string\",\n                \"description\": \"新类型名称（必需，不能与现有类型重名）\"\n            }\n        },\n        \"required\": [\"sourceTypeId\", \"newTypeName\"]\n    }";

	[AsyncStateMachine(typeof(Class440))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class440 stateMachine = new Class440();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.duplicateFamilyTypeTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
