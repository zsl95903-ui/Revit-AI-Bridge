using System;
using System.Collections;
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

[AITool("create_level", Category = "元素创建", Description = "创建新的标高", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateLevelTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class408 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public object object_1;

		public ICreationService icreationService_0;

		public IElementService ielementService_0;

		public ILevelService ilevelService_0;

		public CreateLevelTool createLevelTool_0;

		private IList ilist_0;

		private IList ilist_1;

		private List<object> list_0;

		private List<string> list_1;

		private int int_1;

		private int int_2;

		private string string_0;

		private int int_3;

		private double double_0;

		private string string_1;

		private object object_2;

		private int? nullable_0;

		private string string_2;

		private double? nullable_1;

		private double double_1;

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
					Class408 stateMachine = this;
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
			ilist_0 = (IList)object_0;
			ilist_1 = object_1 as IList;
			AIToolResult result;
			if (ilist_1 != null && ilist_1.Count != ilist_0.Count)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 2);
				defaultInterpolatedStringHandler.AppendLiteral("参数不匹配：elevation 数组有 ");
				defaultInterpolatedStringHandler.AppendFormatted(ilist_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个元素，但 levelName 数组有 ");
				defaultInterpolatedStringHandler.AppendFormatted(ilist_1.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个元素");
				result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				list_0 = new List<object>();
				list_1 = new List<string>();
				if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					int_3 = 0;
					while (int_3 < ilist_0.Count)
					{
						try
						{
							double_0 = Convert.ToDouble(ilist_0[int_3]) * 3.28084;
							IList list = ilist_1;
							object obj;
							if (list == null)
							{
								obj = null;
							}
							else
							{
								object? obj2 = list[int_3];
								if (obj2 == null)
								{
									obj = null;
								}
								else
								{
									obj = obj2.ToString();
									if (obj != null)
									{
										goto IL_01d9;
									}
								}
							}
							obj = "";
							goto IL_01d9;
							IL_01d9:
							string_1 = (string)obj;
							object_2 = icreationService_0.CreateLevel(aitoolContext_0.Document, double_0, string_1);
							if (object_2 == null)
							{
								List<string> list2 = list_1;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(10, 1);
								defaultInterpolatedStringHandler2.AppendLiteral("第 ");
								defaultInterpolatedStringHandler2.AppendFormatted(int_3 + 1);
								defaultInterpolatedStringHandler2.AppendLiteral(" 个标高创建失败");
								list2.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
							}
							else
							{
								nullable_0 = ielementService_0.GetElementId(object_2);
								string_2 = ielementService_0.GetElementName(object_2);
								nullable_1 = ilevelService_0.GetLevelElevation(object_2);
								if (nullable_1.HasValue)
								{
									double_1 = nullable_1.Value / 3.28084;
									list_0.Add(new Class66<int, int?, string, double, string>(int_3 + 1, nullable_0, string_2, Math.Round(double_1, 3), "meters"));
								}
								else
								{
									List<string> list3 = list_1;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(16, 2);
									defaultInterpolatedStringHandler3.AppendLiteral("第 ");
									defaultInterpolatedStringHandler3.AppendFormatted(int_3 + 1);
									defaultInterpolatedStringHandler3.AppendLiteral(" 个标高 (");
									defaultInterpolatedStringHandler3.AppendFormatted(string_2 ?? "未命名");
									defaultInterpolatedStringHandler3.AppendLiteral(") 无法获取高度");
									list3.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
								}
								string_1 = null;
								object_2 = null;
								string_2 = null;
							}
						}
						catch (Exception ex)
						{
							exception_0 = ex;
							List<string> list4 = list_1;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(12, 2);
							defaultInterpolatedStringHandler4.AppendLiteral("第 ");
							defaultInterpolatedStringHandler4.AppendFormatted(int_3 + 1);
							defaultInterpolatedStringHandler4.AppendLiteral(" 个标高创建失败: ");
							defaultInterpolatedStringHandler4.AppendFormatted(exception_0.Message);
							list4.Add(defaultInterpolatedStringHandler4.ToStringAndClear());
						}
						int_3++;
					}
					int_1 = list_0.Count;
					int_2 = list_1.Count;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(14, 1);
					defaultInterpolatedStringHandler5.AppendLiteral("批量创建标高完成：成功 ");
					defaultInterpolatedStringHandler5.AppendFormatted(int_1);
					defaultInterpolatedStringHandler5.AppendLiteral(" 个");
					string_0 = defaultInterpolatedStringHandler5.ToStringAndClear();
					if (int_2 > 0)
					{
						string text = string_0;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(6, 1);
						defaultInterpolatedStringHandler6.AppendLiteral("，失败 ");
						defaultInterpolatedStringHandler6.AppendFormatted(int_2);
						defaultInterpolatedStringHandler6.AppendLiteral(" 个");
						string_0 = text + defaultInterpolatedStringHandler6.ToStringAndClear();
					}
					result = AIToolResult.Ok(string_0, (object)new Class67<int, int, List<object>, List<string>>(int_1, int_2, list_0, (int_2 > 0) ? list_1 : null));
				}
			}
			int_0 = -2;
			ilist_0 = null;
			ilist_1 = null;
			list_0 = null;
			list_1 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class409 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public object object_1;

		public ICreationService icreationService_0;

		public IElementService ielementService_0;

		public ILevelService ilevelService_0;

		public CreateLevelTool createLevelTool_0;

		private double double_0;

		private string string_0;

		private object object_2;

		private int? nullable_0;

		private string string_1;

		private double? nullable_1;

		private double double_1;

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
					Class409 stateMachine = this;
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
			double_0 = Convert.ToDouble(object_0) * 3.28084;
			string_0 = object_1?.ToString();
			AIToolResult result;
			if (aitoolContext_0.Document == null)
			{
				result = AIToolResult.Fail("文档对象为空");
			}
			else
			{
				object_2 = icreationService_0.CreateLevel(aitoolContext_0.Document, double_0, string_0 ?? "");
				if (object_2 == null)
				{
					result = AIToolResult.Fail("创建标高失败");
				}
				else
				{
					nullable_0 = ielementService_0.GetElementId(object_2);
					string_1 = ielementService_0.GetElementName(object_2);
					nullable_1 = ilevelService_0.GetLevelElevation(object_2);
					if (nullable_1.HasValue)
					{
						double_1 = nullable_1.Value / 3.28084;
						result = AIToolResult.Ok("成功创建标高: " + (string_1 ?? "未命名"), (object)new Class65<int?, string, double, string>(nullable_0, string_1, Math.Round(double_1, 3), "meters"));
					}
					else
					{
						result = AIToolResult.Fail("无法获取标高高度");
					}
				}
			}
			int_0 = -2;
			string_0 = null;
			object_2 = null;
			string_1 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class410 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateLevelTool createLevelTool_0;

		private ICreationService icreationService_0;

		private IElementService ielementService_0;

		private ILevelService ilevelService_0;

		private object object_0;

		private object object_1;

		private bool bool_0;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if ((uint)(num - 1) <= 1u)
				{
					goto IL_006e;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class410 stateMachine = this;
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
				TaskAwaiter<AIToolResult> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_02cd;
				}
				TaskAwaiter<AIToolResult> awaiter3;
				if (num == 2)
				{
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_029a;
				}
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				icreationService_0 = ((revitAdapter != null) ? revitAdapter.CreationService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				IRevitAdapter revitAdapter3 = aitoolContext_0.RevitAdapter;
				ilevelService_0 = ((revitAdapter3 != null) ? revitAdapter3.LevelService : null);
				if (icreationService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 CreationService");
				}
				else if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (ilevelService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 LevelService");
				}
				else
				{
					if (aitoolContext_0.Document != null)
					{
						object_0 = aitoolContext_0.GetParameter<object>("elevation", (object)null);
						object_1 = aitoolContext_0.GetParameter<object>("levelName", (object)null);
						bool_0 = object_0 is IList;
						if (bool_0)
						{
							awaiter2 = createLevelTool_0.method_1(aitoolContext_0, object_0, object_1, icreationService_0, ielementService_0, ilevelService_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter2;
								Class410 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
								return;
							}
							goto IL_02cd;
						}
						awaiter3 = createLevelTool_0.method_0(aitoolContext_0, object_0, object_1, icreationService_0, ielementService_0, ilevelService_0).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_1 = awaiter3;
							Class410 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
							return;
						}
						goto IL_029a;
					}
					result = AIToolResult.Fail("文档对象为空");
				}
				goto end_IL_006e;
				IL_029a:
				aitoolResult_1 = awaiter3.GetResult();
				result = aitoolResult_1;
				goto end_IL_006e;
				IL_02cd:
				aitoolResult_0 = awaiter2.GetResult();
				result = aitoolResult_0;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("创建标高失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_level";

	public string Category => "元素创建";

	public string Description => "创建新的标高";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"elevation\": {\r\n                \"anyOf\": [\r\n                    { \"type\": \"number\" },\r\n                    { \"type\": \"array\", \"items\": { \"type\": \"number\" } }\r\n                ],\r\n                \"description\": \"标高高度（米）或标高高度数组。可以是单个值（如3.5表示3.5米）或数组（如[3.0, 4.5, 6.0]表示3米、4.5米、6米）。如果是数组，levelName也必须是数组且长度一致。\",\r\n                \"examples\": [3.5, [3.0, 4.5, 6.0]]\r\n            },\r\n            \"levelName\": {\r\n                \"anyOf\": [\r\n                    { \"type\": \"string\" },\r\n                    { \"type\": \"array\", \"items\": { \"type\": \"string\" } }\r\n                ],\r\n                \"description\": \"标高名称（可选）或标高名称数组。单个名称时用于单个标高，数组时对应每个标高（长度必须与elevation数组一致）。例如：标高 1、F1、Level 1，或数组形式：[标高 1, 标高 2, 标高 3]\"\r\n            }\r\n        },\r\n        \"required\": [\"elevation\"]\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class410))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class410 stateMachine = new Class410();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createLevelTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class409))]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, object object_0, object object_1, ICreationService icreationService_0, IElementService ielementService_0, ILevelService ilevelService_0)
	{
		Class409 stateMachine = new Class409();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createLevelTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.object_1 = object_1;
		stateMachine.icreationService_0 = icreationService_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.ilevelService_0 = ilevelService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class408))]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, object object_0, object object_1, ICreationService icreationService_0, IElementService ielementService_0, ILevelService ilevelService_0)
	{
		Class408 stateMachine = new Class408();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createLevelTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.object_1 = object_1;
		stateMachine.icreationService_0 = icreationService_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.ilevelService_0 = ilevelService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
