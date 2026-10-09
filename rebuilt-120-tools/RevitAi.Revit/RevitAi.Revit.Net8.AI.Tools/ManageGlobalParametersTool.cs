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

[AITool("manage_global_parameters", Category = "参数管理", Description = "管理全局参数，包括创建、列出、设置值和删除。全局参数是 Revit 2021+ 引入的功能，可以在其他参数的公式中引用。", RequiresTransaction = true, RequiresModification = true)]
public sealed class ManageGlobalParametersTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class546 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public ManageGlobalParametersTool manageGlobalParametersTool_0;

		private string string_0;

		private string string_1;

		private string string_2;

		private object object_0;

		private object object_1;

		private object object_2;

		private PropertyInfo propertyInfo_0;

		private bool bool_0;

		private string string_3;

		private string string_4;

		private object object_3;

		private string string_5;

		private PropertyInfo propertyInfo_1;

		private string string_6;

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
					Class546 stateMachine = this;
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
			string_0 = aitoolContext_0.GetParameter<string>("parameterName", (string)null);
			string_1 = aitoolContext_0.GetParameter<string>("parameterType", (string)null);
			string_2 = aitoolContext_0.GetParameter<string>("formula", (string)null);
			object_0 = (aitoolContext_0.Parameters.TryGetValue("value", out object_1) ? object_1 : null);
			AIToolResult result;
			object obj;
			if (string.IsNullOrEmpty(string_0))
			{
				result = AIToolResult.Fail("parameterName 参数不能为空");
			}
			else if (string.IsNullOrEmpty(string_1))
			{
				result = AIToolResult.Fail("parameterType 参数不能为空");
			}
			else
			{
				object_2 = ielementService_0.CreateGlobalParameter(aitoolContext_0.Document, string_0, string_1, string_2, object_0);
				if (object_2 == null)
				{
					result = AIToolResult.Fail("创建全局参数失败");
				}
				else
				{
					propertyInfo_0 = object_2.GetType().GetProperty("success");
					bool_0 = propertyInfo_0 != null && (bool?)propertyInfo_0.GetValue(object_2) == true;
					if (bool_0)
					{
						PropertyInfo? property = object_2.GetType().GetProperty("parameterName");
						if ((object)property == null)
						{
							obj = null;
						}
						else
						{
							object? value = property.GetValue(object_2);
							if (value == null)
							{
								obj = null;
							}
							else
							{
								obj = value.ToString();
								if (obj != null)
								{
									goto IL_0292;
								}
							}
						}
						obj = string_0;
						goto IL_0292;
					}
					propertyInfo_1 = object_2.GetType().GetProperty("error");
					string_6 = propertyInfo_1?.GetValue(object_2)?.ToString();
					result = AIToolResult.Fail("创建全局参数失败: " + string_6);
				}
			}
			goto IL_0402;
			IL_0292:
			string_3 = (string)obj;
			string_4 = object_2.GetType().GetProperty("formula")?.GetValue(object_2)?.ToString();
			object_3 = object_2.GetType().GetProperty("value")?.GetValue(object_2);
			string_5 = "✅ 成功创建全局参数 '" + string_3 + "'\n\n";
			string_5 = string_5 + "📝 类型: " + string_1 + "\n";
			if (!string.IsNullOrEmpty(string_4))
			{
				string_5 = string_5 + "📐 公式: " + string_4 + "\n";
			}
			if (object_3 != null)
			{
				string text = string_5;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
				defaultInterpolatedStringHandler.AppendLiteral("📊 初始值: ");
				defaultInterpolatedStringHandler.AppendFormatted<object>(object_3);
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				string_5 = text + defaultInterpolatedStringHandler.ToStringAndClear();
			}
			result = AIToolResult.Ok(string_5, object_2);
			goto IL_0402;
			IL_0402:
			int_0 = -2;
			string_0 = null;
			string_1 = null;
			string_2 = null;
			object_0 = null;
			object_1 = null;
			object_2 = null;
			propertyInfo_0 = null;
			string_3 = null;
			string_4 = null;
			object_3 = null;
			string_5 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class547 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public ManageGlobalParametersTool manageGlobalParametersTool_0;

		private string string_0;

		private bool bool_0;

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
					Class547 stateMachine = this;
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
			string_0 = aitoolContext_0.GetParameter<string>("parameterName", (string)null);
			AIToolResult result;
			if (string.IsNullOrEmpty(string_0))
			{
				result = AIToolResult.Fail("parameterName 参数不能为空");
			}
			else
			{
				bool_0 = ielementService_0.DeleteGlobalParameter(aitoolContext_0.Document, string_0);
				result = ((!bool_0) ? AIToolResult.Fail("删除全局参数 '" + string_0 + "' 失败") : AIToolResult.Ok("✅ 成功删除全局参数 '" + string_0 + "'", (object)null));
			}
			int_0 = -2;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class548 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ManageGlobalParametersTool manageGlobalParametersTool_0;

		private IElementService ielementService_0;

		private string string_0;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private AIToolResult aitoolResult_2;

		private AIToolResult aitoolResult_3;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if ((uint)(num - 1) <= 3u)
				{
					goto IL_006e;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class548 stateMachine = this;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
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
			goto IL_006e;
			IL_006e:
			AIToolResult result;
			try
			{
				TaskAwaiter<AIToolResult> awaiter5;
				TaskAwaiter<AIToolResult> awaiter4;
				TaskAwaiter<AIToolResult> awaiter3;
				TaskAwaiter<AIToolResult> awaiter2;
				switch (num)
				{
				default:
				{
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
					if (ielementService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 ElementService");
					}
					else if (aitoolContext_0.Document == null)
					{
						result = AIToolResult.Fail("文档对象为空");
					}
					else
					{
						string_0 = aitoolContext_0.GetParameter<string>("operation", (string)null);
						if (string.IsNullOrEmpty(string_0))
						{
							result = AIToolResult.Fail("operation 参数不能为空");
						}
						else
						{
							string text = string_0.ToLowerInvariant();
							string_1 = text;
							string text2 = string_1;
							if (text2 == "create")
							{
								awaiter5 = manageGlobalParametersTool_0.method_0(aitoolContext_0, ielementService_0).GetAwaiter();
								if (!awaiter5.IsCompleted)
								{
									num = 1;
									int_0 = 1;
									taskAwaiter_1 = awaiter5;
									Class548 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
									return;
								}
								goto IL_0317;
							}
							if (text2 == "list")
							{
								awaiter4 = manageGlobalParametersTool_0.method_1(aitoolContext_0, ielementService_0).GetAwaiter();
								if (!awaiter4.IsCompleted)
								{
									num = 2;
									int_0 = 2;
									taskAwaiter_1 = awaiter4;
									Class548 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
									return;
								}
								goto IL_034e;
							}
							if (text2 == "set_value")
							{
								awaiter3 = manageGlobalParametersTool_0.method_2(aitoolContext_0, ielementService_0).GetAwaiter();
								if (!awaiter3.IsCompleted)
								{
									num = 3;
									int_0 = 3;
									taskAwaiter_1 = awaiter3;
									Class548 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
									return;
								}
								goto IL_0385;
							}
							if (text2 == "delete")
							{
								awaiter2 = manageGlobalParametersTool_0.method_3(aitoolContext_0, ielementService_0).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = 4;
									int_0 = 4;
									taskAwaiter_1 = awaiter2;
									Class548 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
									return;
								}
								break;
							}
							result = AIToolResult.Fail("不支持的操作类型: " + string_0);
						}
					}
					goto end_IL_006e;
				}
				case 1:
					awaiter5 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0317;
				case 2:
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_034e;
				case 3:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0385;
				case 4:
					{
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_034e:
					aitoolResult_1 = awaiter4.GetResult();
					result = aitoolResult_1;
					goto end_IL_006e;
					IL_0317:
					aitoolResult_0 = awaiter5.GetResult();
					result = aitoolResult_0;
					goto end_IL_006e;
					IL_0385:
					aitoolResult_2 = awaiter3.GetResult();
					result = aitoolResult_2;
					goto end_IL_006e;
				}
				aitoolResult_3 = awaiter2.GetResult();
				result = aitoolResult_3;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("管理全局参数失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class549 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public ManageGlobalParametersTool manageGlobalParametersTool_0;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

		private string string_0;

		private List<object>.Enumerator enumerator_0;

		private object object_0;

		private string string_1;

		private string string_2;

		private string string_3;

		private object object_1;

		private string string_4;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class549 stateMachine = this;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
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
			ienumerable_0 = ielementService_0.GetAllGlobalParameters(aitoolContext_0.Document);
			list_0 = ienumerable_0.ToList();
			AIToolResult result;
			if (list_0.Count == 0)
			{
				result = AIToolResult.Ok("当前文档中没有全局参数。", (object)null);
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
				defaultInterpolatedStringHandler.AppendLiteral("找到 ");
				defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个全局参数\n\n");
				string_0 = defaultInterpolatedStringHandler.ToStringAndClear();
				enumerator_0 = list_0.GetEnumerator();
				try
				{
					while (enumerator_0.MoveNext())
					{
						object_0 = enumerator_0.Current;
						string_1 = object_0.GetType().GetProperty("parameterName")?.GetValue(object_0)?.ToString();
						string_2 = object_0.GetType().GetProperty("parameterType")?.GetValue(object_0)?.ToString();
						string_3 = object_0.GetType().GetProperty("formula")?.GetValue(object_0)?.ToString();
						object_1 = object_0.GetType().GetProperty("value")?.GetValue(object_0);
						string_4 = object_0.GetType().GetProperty("description")?.GetValue(object_0)?.ToString();
						string text = string_0;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(6, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("📋 ");
						defaultInterpolatedStringHandler2.AppendFormatted(string_1);
						defaultInterpolatedStringHandler2.AppendLiteral(" (");
						defaultInterpolatedStringHandler2.AppendFormatted(string_2);
						defaultInterpolatedStringHandler2.AppendLiteral(")");
						string_0 = text + defaultInterpolatedStringHandler2.ToStringAndClear();
						if (!string.IsNullOrEmpty(string_3))
						{
							string_0 = string_0 + "\n   📐 公式: " + string_3;
						}
						if (object_1 != null)
						{
							string text2 = string_0;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(10, 1);
							defaultInterpolatedStringHandler3.AppendLiteral("\n   📊 值: ");
							defaultInterpolatedStringHandler3.AppendFormatted<object>(object_1);
							string_0 = text2 + defaultInterpolatedStringHandler3.ToStringAndClear();
						}
						if (!string.IsNullOrEmpty(string_4))
						{
							string_0 = string_0 + "\n   📝 说明: " + string_4;
						}
						string_0 += "\n\n";
						string_1 = null;
						string_2 = null;
						string_3 = null;
						object_1 = null;
						string_4 = null;
						object_0 = null;
					}
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
					}
				}
				enumerator_0 = default(List<object>.Enumerator);
				result = AIToolResult.Ok(string_0, (object)new Class239<int, List<object>>(list_0.Count, list_0));
			}
			int_0 = -2;
			ienumerable_0 = null;
			list_0 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class550 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public ManageGlobalParametersTool manageGlobalParametersTool_0;

		private string string_0;

		private object object_0;

		private object object_1;

		private object object_2;

		private bool bool_0;

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
					Class550 stateMachine = this;
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
			string_0 = aitoolContext_0.GetParameter<string>("parameterName", (string)null);
			object_0 = (aitoolContext_0.Parameters.TryGetValue("value", out object_1) ? object_1 : null);
			AIToolResult result;
			if (string.IsNullOrEmpty(string_0))
			{
				result = AIToolResult.Fail("parameterName 参数不能为空");
			}
			else if (object_0 == null)
			{
				result = AIToolResult.Fail("value 参数不能为空");
			}
			else
			{
				object_2 = ielementService_0.GetGlobalParameter(aitoolContext_0.Document, string_0);
				if (object_2 == null)
				{
					result = AIToolResult.Fail("全局参数 '" + string_0 + "' 不存在。请先使用 operation='create' 创建该参数。");
				}
				else
				{
					bool_0 = ielementService_0.SetGlobalParameterValue(aitoolContext_0.Document, string_0, object_0);
					if (bool_0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 2);
						defaultInterpolatedStringHandler.AppendLiteral("✅ 成功设置全局参数 '");
						defaultInterpolatedStringHandler.AppendFormatted(string_0);
						defaultInterpolatedStringHandler.AppendLiteral("' 的值为: ");
						defaultInterpolatedStringHandler.AppendFormatted<object>(object_0);
						result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class240<string, object>(string_0, object_0));
					}
					else
					{
						result = AIToolResult.Fail("设置全局参数 '" + string_0 + "' 的值失败");
					}
				}
			}
			int_0 = -2;
			string_0 = null;
			object_0 = null;
			object_1 = null;
			object_2 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "manage_global_parameters";

	public string Category => "参数管理";

	public string Description => "管理全局参数，包括创建、列出、设置值和删除";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operation\": {\n                \"type\": \"string\",\n                \"description\": \"操作类型：'create'（创建）、'list'（列出）、'set_value'（设置值）、'delete'（删除）\",\n                \"enum\": [\"create\", \"list\", \"set_value\", \"delete\"]\n            },\n            \"parameterName\": {\n                \"type\": \"string\",\n                \"description\": \"参数名称（create、set_value、delete 操作时必需）\"\n            },\n            \"parameterType\": {\n                \"type\": \"string\",\n                \"description\": \"参数类型（create 操作时必需）。支持：Text（文本）、Integer（整数）、Number（数值）、Length（长度）、Area（面积）、Volume（体积）、Angle（角度）、YesNo（是/否）\"\n            },\n            \"formula\": {\n                \"type\": \"string\",\n                \"description\": \"公式（create 操作时可选）。可以在公式中引用其他全局参数，例如：'Width * Height' 或 '层高 * 2'\"\n            },\n            \"value\": {\n                \"description\": \"参数值（create 或 set_value 操作时可选）。类型要与 parameterType 匹配：\\n- Text: 字符串\\n- Integer: 整数\\n- Number: 浮点数\\n- YesNo: 布尔值（true/false）\"\n            }\n        },\n        \"required\": [\"operation\"]\n    }";

	[AsyncStateMachine(typeof(Class548))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class548 stateMachine = new Class548();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageGlobalParametersTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class546))]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class546 stateMachine = new Class546();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageGlobalParametersTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class549))]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class549 stateMachine = new Class549();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageGlobalParametersTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class550))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_2(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class550 stateMachine = new Class550();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageGlobalParametersTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class547))]
	private Task<AIToolResult> method_3(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class547 stateMachine = new Class547();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageGlobalParametersTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
