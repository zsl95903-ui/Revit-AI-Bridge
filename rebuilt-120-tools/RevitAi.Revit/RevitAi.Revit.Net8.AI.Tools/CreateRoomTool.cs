using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("create_room", Category = "元素创建", Description = "在指定位置和标高创建房间（单个模式）或自动在标高上创建所有房间（自动模式）。距离单位：毫米", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateRoomTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class414 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateRoomTool createRoomTool_0;

		private ICreationService icreationService_0;

		private IElementService ielementService_0;

		private bool bool_0;

		private bool bool_1;

		private int int_1;

		private object object_0;

		private string string_0;

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
					Class414 stateMachine = this;
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
				icreationService_0 = ((revitAdapter != null) ? revitAdapter.CreationService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				if (icreationService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 CreationService");
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
					bool_0 = aitoolContext_0.GetParameter<bool>("autoMode", false);
					bool_1 = aitoolContext_0.HasParameter("x") && aitoolContext_0.HasParameter("y");
					if (bool_0 & bool_1)
					{
						Logger.Warning("[CreateRoomTool] 同时指定了 autoMode 和坐标，优先使用自动模式");
					}
					if (!bool_0 && !bool_1)
					{
						result = AIToolResult.Fail("非自动模式必须提供 x 和 y 坐标参数");
					}
					else
					{
						int_1 = aitoolContext_0.GetParameter<int>("levelId", 0);
						object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
						if (object_0 == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
							defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
							defaultInterpolatedStringHandler.AppendFormatted(int_1);
							defaultInterpolatedStringHandler.AppendLiteral(" 的标高");
							result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						else
						{
							string_0 = ielementService_0.GetElementName(object_0);
							result = ((!bool_0) ? createRoomTool_0.method_0(aitoolContext_0, icreationService_0, ielementService_0, object_0, int_1, string_0) : createRoomTool_0.method_1(aitoolContext_0, icreationService_0, ielementService_0, object_0, int_1, string_0));
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[CreateRoomTool] 执行失败: " + exception_0.Message);
				result = AIToolResult.Fail("创建房间失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_room";

	public string Category => "元素创建";

	public string Description => "在指定位置和标高创建房间（单个模式）或自动在标高上创建所有房间（自动模式）。距离单位：毫米";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"levelId\": {\n                \"type\": \"integer\",\n                \"description\": \"房间所在的标高 ID（单个模式必选，自动模式下必选）\"\n            },\n            \"x\": {\n                \"type\": \"number\",\n                \"description\": \"房间中心的 X 坐标（毫米，单个模式必选）\"\n            },\n            \"y\": {\n                \"type\": \"number\",\n                \"description\": \"房间中心的 Y 坐标（毫米，单个模式必选）\"\n            },\n            \"roomName\": {\n                \"type\": \"string\",\n                \"description\": \"房间名称（可选，仅单个模式有效）\"\n            },\n            \"roomNumber\": {\n                \"type\": \"string\",\n                \"description\": \"房间编号（可选，仅单个模式有效）\"\n            },\n            \"autoMode\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否启用自动模式：在指定标高的所有闭合区域自动创建房间。默认 false\",\n                \"default\": false\n            }\n        },\n        \"required\": [\"levelId\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class414))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class414 stateMachine = new Class414();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createRoomTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private AIToolResult method_0(AIToolContext aitoolContext_0, ICreationService icreationService_0, IElementService ielementService_0, object object_0, int int_0, string? string_0)
	{
		object document = aitoolContext_0.Document;
		double num = aitoolContext_0.GetParameter<double>("x", 0.0) / 304.8;
		double num2 = aitoolContext_0.GetParameter<double>("y", 0.0) / 304.8;
		string parameter = aitoolContext_0.GetParameter<string>("roomName", (string)null);
		string parameter2 = aitoolContext_0.GetParameter<string>("roomNumber", (string)null);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 3);
		defaultInterpolatedStringHandler.AppendLiteral("[CreateRoomTool] 单个模式：在标高 ");
		defaultInterpolatedStringHandler.AppendFormatted(string_0);
		defaultInterpolatedStringHandler.AppendLiteral(" 的位置 (");
		defaultInterpolatedStringHandler.AppendFormatted(num * 304.8, "F0");
		defaultInterpolatedStringHandler.AppendLiteral(", ");
		defaultInterpolatedStringHandler.AppendFormatted(num2 * 304.8, "F0");
		defaultInterpolatedStringHandler.AppendLiteral(") mm 创建房间");
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		object obj = icreationService_0.CreateRoomAtPoint(document, object_0, (object)new
		{
			x = num,
			y = num2,
			z = 0.0
		}, (object)null);
		if (obj == null)
		{
			return AIToolResult.Fail("创建房间失败（服务返回 null）");
		}
		if (!string.IsNullOrEmpty(parameter))
		{
			ielementService_0.SetParameterValue(obj, "Name", (object)parameter, document);
		}
		if (!string.IsNullOrEmpty(parameter2))
		{
			ielementService_0.SetParameterValue(obj, "Number", (object)parameter2, document);
		}
		int? elementId = ielementService_0.GetElementId(obj);
		string elementName = ielementService_0.GetElementName(obj);
		return AIToolResult.Ok("成功创建房间: " + (elementName ?? "未命名"), (object)new Class72<string, int?, string, int, string, Class63<double, double>, string>("single", elementId, elementName, int_0, string_0, new Class63<double, double>(Math.Round(num * 304.8, 0), Math.Round(num2 * 304.8, 0)), "millimeters"));
	}

	private AIToolResult method_1(AIToolContext aitoolContext_0, ICreationService icreationService_0, IElementService ielementService_0, object object_0, int int_0, string? string_0)
	{
		object document = aitoolContext_0.Document;
		Logger.Info("[CreateRoomTool] 自动模式：在标高 " + string_0 + " 上自动创建所有房间");
		IEnumerable<object> source = icreationService_0.CreateAllRoomsInLevel(document, object_0, (object)null);
		List<object> list = source.ToList();
		if (list.Count == 0)
		{
			return AIToolResult.Ok("标高 " + string_0 + " 上没有可创建房间的闭合区域", (object)new Class73<string, int, string, int>("auto", int_0, string_0, 0));
		}
		List<object> list2 = new List<object>();
		foreach (object item in list)
		{
			int? elementId = ielementService_0.GetElementId(item);
			string elementName = ielementService_0.GetElementName(item);
			list2.Add(new Class74<int?, string>(elementId, elementName ?? "未命名"));
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 2);
		defaultInterpolatedStringHandler.AppendLiteral("[CreateRoomTool] 自动模式完成：在标高 ");
		defaultInterpolatedStringHandler.AppendFormatted(string_0);
		defaultInterpolatedStringHandler.AppendLiteral(" 上成功创建 ");
		defaultInterpolatedStringHandler.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 个房间");
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(15, 2);
		defaultInterpolatedStringHandler2.AppendLiteral("成功在标高 ");
		defaultInterpolatedStringHandler2.AppendFormatted(string_0);
		defaultInterpolatedStringHandler2.AppendLiteral(" 上创建 ");
		defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler2.AppendLiteral(" 个房间");
		return AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class75<string, int, string, int, List<object>>("auto", int_0, string_0, list.Count, list2));
	}
}
