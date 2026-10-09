using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("delete_link", Category = "链接管理", Description = "删除 Revit 链接模型（从项目中完全移除）", RequiresTransaction = true, RequiresModification = true)]
public sealed class DeleteLinkTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class437
	{
		public int int_0;

		internal bool method_0(object object_0)
		{
			return smethod_1(object_0) == int_0;
		}

		internal bool method_1(object object_0)
		{
			return smethod_0(object_0) == int_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class438 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public DeleteLinkTool deleteLinkTool_0;

		private Class437 class437_0;

		private ILinkService ilinkService_0;

		private IEnumerable<object> ienumerable_0;

		private object object_0;

		private IEnumerable<object> ienumerable_1;

		private object object_1;

		private string string_0;

		private bool bool_0;

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
					Class438 stateMachine = this;
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
				class437_0 = new Class437();
				class437_0.int_0 = aitoolContext_0.GetParameter<int>("linkId", 0);
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ilinkService_0 = ((revitAdapter != null) ? revitAdapter.LinkService : null);
				if (ilinkService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 LinkService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					ienumerable_0 = ilinkService_0.GetAllLinkInstances(aitoolContext_0.Document);
					object_0 = ienumerable_0.FirstOrDefault((object object_0) => smethod_1(object_0) == class437_0.int_0);
					if (object_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
						defaultInterpolatedStringHandler.AppendFormatted(class437_0.int_0);
						defaultInterpolatedStringHandler.AppendLiteral(" 的链接实例");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						ienumerable_1 = ilinkService_0.GetAllLinks(aitoolContext_0.Document);
						object_1 = ienumerable_1.FirstOrDefault((object object_0) => smethod_0(object_0) == class437_0.int_0);
						string text;
						if (object_1 == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("链接 ");
							defaultInterpolatedStringHandler2.AppendFormatted(class437_0.int_0);
							text = defaultInterpolatedStringHandler2.ToStringAndClear();
						}
						else
						{
							text = ilinkService_0.GetLinkName(object_1);
						}
						string_0 = text;
						bool_0 = ilinkService_0.DeleteLink(aitoolContext_0.Document, object_0);
						result = (bool_0 ? AIToolResult.Ok("成功删除链接: " + string_0, (object)new Class110<int, string, string>(class437_0.int_0, string_0, "已删除")) : AIToolResult.Fail("删除链接失败"));
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("删除链接失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "delete_link";

	public string Category => "链接管理";

	public string Description => "删除链接模型";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"linkId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"要删除的链接 ID\"\r\n            }\r\n        },\r\n        \"required\": [\"linkId\"]\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class438))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class438 stateMachine = new Class438();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.deleteLinkTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private static int? smethod_0(object object_0)
	{
		try
		{
			PropertyInfo property = object_0.GetType().GetProperty("Id", BindingFlags.Instance | BindingFlags.Public);
			if (property != null)
			{
				object value = property.GetValue(object_0);
				if (value != null)
				{
					PropertyInfo property2 = value.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
					if (property2 != null && property2.GetValue(value) is long num)
					{
						return (int)num;
					}
				}
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	private static int? smethod_1(object object_0)
	{
		try
		{
			PropertyInfo property = object_0.GetType().GetProperty("TypeId");
			if (property != null)
			{
				object value = property.GetValue(object_0);
				if (value != null)
				{
					PropertyInfo property2 = value.GetType().GetProperty("Value");
					if (property2 != null && property2.GetValue(value) is long num)
					{
						return (int)num;
					}
				}
			}
			return null;
		}
		catch
		{
			return null;
		}
	}
}
