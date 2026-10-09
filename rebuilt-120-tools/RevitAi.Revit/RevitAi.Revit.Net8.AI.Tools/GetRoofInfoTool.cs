using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_roof_info", Category = "元素查询", Description = "获取屋面信息，包括类型、标高、边坡度等。可查询所有屋面或指定屋面", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetRoofInfoTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class504 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetRoofInfoTool getRoofInfoTool_0;

		private IElementService ielementService_0;

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
					Class504 stateMachine = this;
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
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
				result = (AIToolResult)((ielementService_0 == null) ? AIToolResult.Fail("无法获取 ElementService") : ((aitoolContext_0.Document == null) ? ((object)AIToolResult.Fail("文档对象为空")) : ((object)((!aitoolContext_0.HasParameter("roof_id")) ? getRoofInfoTool_0.method_1(aitoolContext_0, ielementService_0) : getRoofInfoTool_0.method_0(aitoolContext_0, ielementService_0)))));
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取屋面信息失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_roof_info";

	public string Category => "元素查询";

	public string Description => "获取屋面信息";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"roof_id\": {\n                \"type\": \"integer\",\n                \"description\": \"屋面元素 ID。不指定时返回所有屋面摘要\"\n            },\n            \"include_edges\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否包含迹线屋面的边信息。默认 false\"\n            }\n        }\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class504))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class504 stateMachine = new Class504();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getRoofInfoTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private AIToolResult method_0(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		int parameter = aitoolContext_0.GetParameter<int>("roof_id", 0);
		object elementById = ielementService_0.GetElementById(aitoolContext_0.Document, parameter);
		if (elementById == null)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler.AppendLiteral("找不到屋面 ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(parameter);
			return AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		string elementName = ielementService_0.GetElementName(elementById);
		string elementTypeName = ielementService_0.GetElementTypeName(elementById);
		int? elementTypeId = ielementService_0.GetElementTypeId(elementById);
		string elementCategory = ielementService_0.GetElementCategory(elementById);
		Class191<int, string, string, int?, string> @class = new Class191<int, string, string, int?, string>(parameter, elementName ?? "未命名", elementTypeName ?? "未知", elementTypeId, elementCategory ?? "未知");
		if (aitoolContext_0.HasParameter("include_edges") && aitoolContext_0.GetParameter<bool>("include_edges", false))
		{
			IEnumerable<(int, bool, double)> footPrintRoofEdges = ielementService_0.GetFootPrintRoofEdges(elementById);
			if (footPrintRoofEdges != null)
			{
				var list = footPrintRoofEdges.Select(((int EdgeIndex, bool DefinesSlope, double SlopeAngle) valueTuple_0) => new _003C_003Ef__AnonymousType199<int, bool, double>(valueTuple_0.EdgeIndex, valueTuple_0.DefinesSlope, Math.Round(valueTuple_0.SlopeAngle, 2))).ToList();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(9, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("屋面 ");
				defaultInterpolatedStringHandler2.AppendFormatted(parameter);
				defaultInterpolatedStringHandler2.AppendLiteral(" 共 ");
				defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 条边");
				return AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class192<int, string, string, int?, string, string, List<_003C_003Ef__AnonymousType199<int, bool, double>>>(@class.roof_id, @class.name, @class.type_name, @class.type_id, @class.category, "FootPrintRoof", list));
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(18, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("屋面 ");
			defaultInterpolatedStringHandler3.AppendFormatted(parameter);
			defaultInterpolatedStringHandler3.AppendLiteral(" 不是迹线屋面，无法获取边信息");
			return AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class193<int, string, string, int?, string, string>(@class.roof_id, @class.name, @class.type_name, @class.type_id, @class.category, "ExtrusionRoof 或其他"));
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(5, 2);
		defaultInterpolatedStringHandler4.AppendLiteral("屋面 ");
		defaultInterpolatedStringHandler4.AppendFormatted(parameter);
		defaultInterpolatedStringHandler4.AppendLiteral(": ");
		defaultInterpolatedStringHandler4.AppendFormatted(elementName);
		return AIToolResult.Ok(defaultInterpolatedStringHandler4.ToStringAndClear(), (object)@class);
	}

	private AIToolResult method_1(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		List<object> list = ielementService_0.GetElementsByCategory(aitoolContext_0.Document, "Roofs").ToList();
		if (list.Count == 0)
		{
			return AIToolResult.Ok("当前文档中没有屋面", (object)new Class194<int, object[]>(0, Array.Empty<object>()));
		}
		List<object> list2 = new List<object>();
		foreach (object item in list)
		{
			int? elementId = ielementService_0.GetElementId(item);
			string elementName = ielementService_0.GetElementName(item);
			string elementTypeName = ielementService_0.GetElementTypeName(item);
			list2.Add(new Class195<int?, string, string>(elementId, elementName ?? "未命名", elementTypeName ?? "未知"));
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
		defaultInterpolatedStringHandler.AppendLiteral("共找到 ");
		defaultInterpolatedStringHandler.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 个屋面");
		return AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class194<int, List<object>>(list.Count, list2));
	}
}
