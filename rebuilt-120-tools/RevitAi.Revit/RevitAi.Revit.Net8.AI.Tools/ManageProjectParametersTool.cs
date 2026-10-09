using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("manage_project_parameters", Category = "参数管理", Description = "管理项目参数，包括列出和删除。项目参数只对当前项目有效，可以出现在明细表中，但不能出现在标记中。", RequiresTransaction = true, RequiresModification = true)]
public sealed class ManageProjectParametersTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class551 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public ManageProjectParametersTool manageProjectParametersTool_0;

		private string string_0;

		private string[] string_1;

		private string string_2;

		private string string_3;

		private object object_0;

		private PropertyInfo propertyInfo_0;

		private bool bool_0;

		private string string_4;

		private string string_5;

		private object object_1;

		private string[] string_6;

		private string string_7;

		private string string_8;

		private string string_9;

		private PropertyInfo propertyInfo_1;

		private string string_10;

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
					Class551 stateMachine = this;
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
			string_0 = aitoolContext_0.GetParameter<string>("sharedParameterName", (string)null);
			string_1 = aitoolContext_0.GetParameter<string[]>("categoryNames", (string[])null);
			string_2 = aitoolContext_0.GetParameter<string>("bindingType", (string)null);
			string_3 = aitoolContext_0.GetParameter<string>("parameterGroup", (string)null);
			AIToolResult result;
			object obj;
			if (string.IsNullOrEmpty(string_0))
			{
				result = AIToolResult.Fail("sharedParameterName 参数不能为空");
			}
			else if (string_1 == null || string_1.Length == 0)
			{
				result = AIToolResult.Fail("categoryNames 参数不能为空");
			}
			else if (string.IsNullOrEmpty(string_2))
			{
				result = AIToolResult.Fail("bindingType 参数不能为空");
			}
			else
			{
				object_0 = ielementService_0.CreateProjectParameterBinding(aitoolContext_0.Document, string_0, string_1, string_2, string_3);
				if (object_0 == null)
				{
					result = AIToolResult.Fail("创建项目参数绑定失败");
				}
				else
				{
					propertyInfo_0 = object_0.GetType().GetProperty("success");
					bool_0 = propertyInfo_0 != null && (bool?)propertyInfo_0.GetValue(object_0) == true;
					if (bool_0)
					{
						PropertyInfo? property = object_0.GetType().GetProperty("parameterName");
						if ((object)property == null)
						{
							obj = null;
						}
						else
						{
							object? value = property.GetValue(object_0);
							if (value == null)
							{
								obj = null;
							}
							else
							{
								obj = value.ToString();
								if (obj != null)
								{
									goto IL_02a9;
								}
							}
						}
						obj = string_0;
						goto IL_02a9;
					}
					propertyInfo_1 = object_0.GetType().GetProperty("error");
					string_10 = propertyInfo_1?.GetValue(object_0)?.ToString();
					result = AIToolResult.Fail("创建项目参数绑定失败: " + string_10);
				}
			}
			goto IL_0522;
			IL_0522:
			int_0 = -2;
			string_0 = null;
			string_1 = null;
			string_2 = null;
			string_3 = null;
			object_0 = null;
			propertyInfo_0 = null;
			string_4 = null;
			string_5 = null;
			object_1 = null;
			string_6 = null;
			string_7 = null;
			string_8 = null;
			string_9 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
			return;
			IL_02a9:
			string_4 = (string)obj;
			string_5 = object_0.GetType().GetProperty("bindingType")?.GetValue(object_0)?.ToString();
			object_1 = object_0.GetType().GetProperty("categoryCount")?.GetValue(object_0);
			string_6 = object_0.GetType().GetProperty("boundCategories")?.GetValue(object_0) as string[];
			string_7 = object_0.GetType().GetProperty("parameterGroup")?.GetValue(object_0)?.ToString();
			string_8 = ((string_5 == "type") ? "类型参数" : "实例参数");
			string_9 = "✅ 成功创建项目参数绑定 '" + string_4 + "'\n\n";
			string_9 = string_9 + "📋 绑定类型: " + string_8 + "\n";
			string text = string_9;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
			defaultInterpolatedStringHandler.AppendLiteral("📊 绑定类别数: ");
			defaultInterpolatedStringHandler.AppendFormatted<object>(object_1);
			defaultInterpolatedStringHandler.AppendLiteral("\n");
			string_9 = text + defaultInterpolatedStringHandler.ToStringAndClear();
			if (string_6 != null && string_6.Length != 0)
			{
				string_9 = string_9 + "📂 绑定类别: " + string.Join("、", string_6) + "\n";
			}
			if (!string.IsNullOrEmpty(string_7))
			{
				string_9 = string_9 + "📁 参数分组: " + string_7 + "\n";
			}
			string_9 += "\n💡 提示：项目参数仅对当前项目有效，可以出现在明细表中";
			result = AIToolResult.Ok(string_9, object_0);
			goto IL_0522;
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class552 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public ManageProjectParametersTool manageProjectParametersTool_0;

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
					Class552 stateMachine = this;
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
				bool_0 = ielementService_0.DeleteProjectParameter(aitoolContext_0.Document, string_0);
				result = ((!bool_0) ? AIToolResult.Fail("删除项目参数 '" + string_0 + "' 失败") : AIToolResult.Ok("✅ 成功删除项目参数 '" + string_0 + "'", (object)null));
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
	public sealed class Class553 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ManageProjectParametersTool manageProjectParametersTool_0;

		private IElementService ielementService_0;

		private string string_0;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private AIToolResult aitoolResult_2;

		private AIToolResult aitoolResult_3;

		private AIToolResult aitoolResult_4;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if ((uint)(num - 1) <= 4u)
				{
					goto IL_006e;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class553 stateMachine = this;
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
				TaskAwaiter<AIToolResult> awaiter6;
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
							if (text2 == "list")
							{
								awaiter6 = manageProjectParametersTool_0.method_0(aitoolContext_0, ielementService_0).GetAwaiter();
								if (!awaiter6.IsCompleted)
								{
									num = 1;
									int_0 = 1;
									taskAwaiter_1 = awaiter6;
									Class553 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter6, ref stateMachine);
									return;
								}
								goto IL_0382;
							}
							if (text2 == "delete")
							{
								awaiter5 = manageProjectParametersTool_0.method_1(aitoolContext_0, ielementService_0).GetAwaiter();
								if (!awaiter5.IsCompleted)
								{
									num = 2;
									int_0 = 2;
									taskAwaiter_1 = awaiter5;
									Class553 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
									return;
								}
								goto IL_03b9;
							}
							if (text2 == "create_binding")
							{
								awaiter4 = manageProjectParametersTool_0.method_2(aitoolContext_0, ielementService_0).GetAwaiter();
								if (!awaiter4.IsCompleted)
								{
									num = 3;
									int_0 = 3;
									taskAwaiter_1 = awaiter4;
									Class553 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
									return;
								}
								goto IL_03f0;
							}
							if (text2 == "update_binding")
							{
								awaiter3 = manageProjectParametersTool_0.method_3(aitoolContext_0, ielementService_0).GetAwaiter();
								if (!awaiter3.IsCompleted)
								{
									num = 4;
									int_0 = 4;
									taskAwaiter_1 = awaiter3;
									Class553 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
									return;
								}
								goto IL_0427;
							}
							if (text2 == "list_parameter_groups")
							{
								awaiter2 = manageProjectParametersTool_0.method_4(aitoolContext_0, ielementService_0).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = 5;
									int_0 = 5;
									taskAwaiter_1 = awaiter2;
									Class553 stateMachine = this;
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
					awaiter6 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0382;
				case 2:
					awaiter5 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_03b9;
				case 3:
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_03f0;
				case 4:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0427;
				case 5:
					{
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_03f0:
					aitoolResult_2 = awaiter4.GetResult();
					result = aitoolResult_2;
					goto end_IL_006e;
					IL_0382:
					aitoolResult_0 = awaiter6.GetResult();
					result = aitoolResult_0;
					goto end_IL_006e;
					IL_0427:
					aitoolResult_3 = awaiter3.GetResult();
					result = aitoolResult_3;
					goto end_IL_006e;
					IL_03b9:
					aitoolResult_1 = awaiter5.GetResult();
					result = aitoolResult_1;
					goto end_IL_006e;
				}
				aitoolResult_4 = awaiter2.GetResult();
				result = aitoolResult_4;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("管理项目参数失败: " + exception_0.Message);
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
	public sealed class Class554 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public ManageProjectParametersTool manageProjectParametersTool_0;

		private Type type_0;

		private PropertyInfo propertyInfo_0;

		private object object_0;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

		private string string_0;

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
					Class554 stateMachine = this;
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
			if (aitoolContext_0.Document == null)
			{
				result = AIToolResult.Fail("文档对象为空");
			}
			else
			{
				type_0 = aitoolContext_0.Document.GetType();
				propertyInfo_0 = type_0.GetProperty("Application");
				if (propertyInfo_0 == null)
				{
					result = AIToolResult.Fail("无法获取 Application 对象");
				}
				else
				{
					object_0 = propertyInfo_0.GetValue(aitoolContext_0.Document);
					if (object_0 == null)
					{
						result = AIToolResult.Fail("Application 对象为空");
					}
					else
					{
						ienumerable_0 = ielementService_0.GetAllParameterGroups(object_0);
						list_0 = ienumerable_0.ToList();
						if (list_0.Count == 0)
						{
							result = AIToolResult.Ok("未找到可用的参数组", (object)null);
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
							defaultInterpolatedStringHandler.AppendLiteral("找到 ");
							defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
							defaultInterpolatedStringHandler.AppendLiteral(" 个可用的参数组\n\n");
							string_0 = defaultInterpolatedStringHandler.ToStringAndClear();
							string_0 += "💡 提示：创建项目参数时，parameterGroup 参数支持：\n";
							string_0 += "   • 整数 ID（如：1）\n";
							string_0 += "   • 名称（如：数据）\n";
							string text = string_0;
							int count = list_0.Count;
							List<object> gparam_ = list_0;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(23, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("系统中有 ");
							defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
							defaultInterpolatedStringHandler2.AppendLiteral(" 个可用的参数组可供项目参数绑定使用");
							result = AIToolResult.Ok(text, (object)new Class242<int, List<object>, string>(count, gparam_, defaultInterpolatedStringHandler2.ToStringAndClear()));
						}
					}
				}
			}
			int_0 = -2;
			type_0 = null;
			propertyInfo_0 = null;
			object_0 = null;
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
	public sealed class Class555 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public ManageProjectParametersTool manageProjectParametersTool_0;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

		private string string_0;

		private StringBuilder stringBuilder_0;

		private List<object>.Enumerator enumerator_0;

		private object object_0;

		private string string_1;

		private string string_2;

		private string[] string_3;

		private object object_1;

		private string string_4;

		private string string_5;

		private string string_6;

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
					Class555 stateMachine = this;
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
			ienumerable_0 = ielementService_0.GetAllProjectParameters(aitoolContext_0.Document);
			list_0 = ienumerable_0.ToList();
			AIToolResult result;
			if (list_0.Count == 0)
			{
				result = AIToolResult.Ok("当前文档中没有项目参数。", (object)null);
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
				defaultInterpolatedStringHandler.AppendLiteral("找到 ");
				defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个项目参数");
				string_0 = defaultInterpolatedStringHandler.ToStringAndClear();
				stringBuilder_0 = new StringBuilder();
				StringBuilder stringBuilder = stringBuilder_0;
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(10, 1, stringBuilder);
				handler.AppendLiteral("找到 ");
				handler.AppendFormatted(list_0.Count);
				handler.AppendLiteral(" 个项目参数：");
				stringBuilder2.AppendLine(ref handler);
				enumerator_0 = list_0.GetEnumerator();
				try
				{
					while (enumerator_0.MoveNext())
					{
						object_0 = enumerator_0.Current;
						string_1 = object_0.GetType().GetProperty("parameterName")?.GetValue(object_0)?.ToString();
						string_2 = object_0.GetType().GetProperty("parameterType")?.GetValue(object_0)?.ToString();
						string_3 = object_0.GetType().GetProperty("boundCategories")?.GetValue(object_0) as string[];
						object_1 = object_0.GetType().GetProperty("categoryCount")?.GetValue(object_0);
						string_4 = object_0.GetType().GetProperty("bindingType")?.GetValue(object_0)?.ToString();
						string_5 = object_0.GetType().GetProperty("parameterGroup")?.GetValue(object_0)?.ToString();
						string text = string_4;
						string text2 = ((text == "instance") ? "实例参数" : ((text == "type") ? "类型参数" : "未知"));
						string_6 = text2;
						stringBuilder = stringBuilder_0;
						StringBuilder stringBuilder3 = stringBuilder;
						handler = new StringBuilder.AppendInterpolatedStringHandler(22, 5, stringBuilder);
						handler.AppendLiteral("- ");
						handler.AppendFormatted(string_1);
						handler.AppendLiteral(" (");
						handler.AppendFormatted(string_2);
						handler.AppendLiteral(") | ");
						handler.AppendFormatted(string_6);
						handler.AppendLiteral(" | ");
						handler.AppendFormatted<object>(object_1);
						handler.AppendLiteral(" 个类别 | 分组: ");
						handler.AppendFormatted(string_5);
						stringBuilder3.AppendLine(ref handler);
						string_1 = null;
						string_2 = null;
						string_3 = null;
						object_1 = null;
						string_4 = null;
						string_5 = null;
						string_6 = null;
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
				result = AIToolResult.Ok(string_0, (object)new Class241<int, List<object>, string>(list_0.Count, list_0, stringBuilder_0.ToString()));
			}
			int_0 = -2;
			ienumerable_0 = null;
			list_0 = null;
			string_0 = null;
			stringBuilder_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class556 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public ManageProjectParametersTool manageProjectParametersTool_0;

		private string string_0;

		private string[] string_1;

		private string[] string_2;

		private object object_0;

		private PropertyInfo propertyInfo_0;

		private bool bool_0;

		private string string_3;

		private object object_1;

		private string[] string_4;

		private PropertyInfo propertyInfo_1;

		private string string_5;

		private string string_6;

		private PropertyInfo propertyInfo_2;

		private string string_7;

		private string[] string_8;

		private int int_1;

		private string string_9;

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
					Class556 stateMachine = this;
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
			string_1 = aitoolContext_0.GetParameter<string[]>("categoriesToAdd", (string[])null);
			string_2 = aitoolContext_0.GetParameter<string[]>("categoriesToRemove", (string[])null);
			AIToolResult result;
			object obj;
			if (string.IsNullOrEmpty(string_0))
			{
				result = AIToolResult.Fail("parameterName 参数不能为空");
			}
			else if ((string_1 == null || string_1.Length == 0) && (string_2 == null || string_2.Length == 0))
			{
				result = AIToolResult.Fail("categoriesToAdd 或 categoriesToRemove 至少需要提供一个");
			}
			else
			{
				object_0 = ielementService_0.UpdateProjectParameterBinding(aitoolContext_0.Document, string_0, string_1 ?? Array.Empty<string>(), string_2 ?? Array.Empty<string>());
				if (object_0 == null)
				{
					result = AIToolResult.Fail("更新项目参数绑定失败");
				}
				else
				{
					propertyInfo_0 = object_0.GetType().GetProperty("success");
					bool_0 = propertyInfo_0 != null && (bool?)propertyInfo_0.GetValue(object_0) == true;
					if (bool_0)
					{
						PropertyInfo? property = object_0.GetType().GetProperty("parameterName");
						if ((object)property == null)
						{
							obj = null;
						}
						else
						{
							object? value = property.GetValue(object_0);
							if (value == null)
							{
								obj = null;
							}
							else
							{
								obj = value.ToString();
								if (obj != null)
								{
									goto IL_028b;
								}
							}
						}
						obj = string_0;
						goto IL_028b;
					}
					propertyInfo_2 = object_0.GetType().GetProperty("error");
					string_7 = propertyInfo_2?.GetValue(object_0)?.ToString();
					result = AIToolResult.Fail("更新项目参数绑定失败: " + string_7);
				}
			}
			goto IL_04b2;
			IL_028b:
			string_3 = (string)obj;
			object_1 = object_0.GetType().GetProperty("categoryCount")?.GetValue(object_0);
			string_4 = object_0.GetType().GetProperty("affectedCategories")?.GetValue(object_0) as string[];
			propertyInfo_1 = object_0.GetType().GetProperty("message");
			string_5 = propertyInfo_1?.GetValue(object_0)?.ToString();
			string_6 = "✅ 成功更新项目参数 '" + string_3 + "' 的绑定\n\n";
			string text = string_6;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
			defaultInterpolatedStringHandler.AppendLiteral("📊 当前绑定类别数: ");
			defaultInterpolatedStringHandler.AppendFormatted<object>(object_1);
			defaultInterpolatedStringHandler.AppendLiteral("\n");
			string_6 = text + defaultInterpolatedStringHandler.ToStringAndClear();
			if (string_4 != null && string_4.Length != 0)
			{
				string_6 += "\n🔄 变更详情:\n";
				string_8 = string_4;
				for (int_1 = 0; int_1 < string_8.Length; int_1++)
				{
					string_9 = string_8[int_1];
					string_6 = string_6 + "   • " + string_9 + "\n";
					string_9 = null;
				}
				string_8 = null;
			}
			if (!string.IsNullOrEmpty(string_5))
			{
				string_6 = string_6 + "\n" + string_5;
			}
			result = AIToolResult.Ok(string_6, object_0);
			goto IL_04b2;
			IL_04b2:
			int_0 = -2;
			string_0 = null;
			string_1 = null;
			string_2 = null;
			object_0 = null;
			propertyInfo_0 = null;
			string_3 = null;
			object_1 = null;
			string_4 = null;
			propertyInfo_1 = null;
			string_5 = null;
			string_6 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "manage_project_parameters";

	public string Category => "参数管理";

	public string Description => "管理项目参数，包括列出和删除";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operation\": {\n                \"type\": \"string\",\n                \"description\": \"操作类型：'list'（列出）、'delete'（删除）、'create_binding'（创建项目参数绑定）、'update_binding'（更新绑定类别）、'list_parameter_groups'（列出所有参数组）\",\n                \"enum\": [\"list\", \"delete\", \"create_binding\", \"update_binding\", \"list_parameter_groups\"]\n            },\n            \"parameterName\": {\n                \"type\": \"string\",\n                \"description\": \"参数名称（delete、update_binding 操作时必需）\"\n            },\n            \"sharedParameterName\": {\n                \"type\": \"string\",\n                \"description\": \"共享参数名称（create_binding 操作时必需）\"\n            },\n            \"categoryNames\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"string\" },\n                \"description\": \"要绑定的类别名称数组，使用中文类别名，例如：墙、房间。create_binding 操作时必需\"\n            },\n            \"bindingType\": {\n                \"type\": \"string\",\n                \"description\": \"绑定类型（create_binding 操作时必需）：'instance'（实例参数）或 'type'（类型参数）\",\n                \"enum\": [\"instance\", \"type\"]\n            },\n            \"parameterGroup\": {\n                \"type\": \"string\",\n                \"description\": \"参数分组名称（create_binding 操作时可选，例如：数据、文字。默认值为：数据）\"\n            },\n            \"categoriesToAdd\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"string\" },\n                \"description\": \"要添加的类别名称数组（update_binding 操作时可选）\"\n            },\n            \"categoriesToRemove\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"string\" },\n                \"description\": \"要删除的类别名称数组（update_binding 操作时可选）\"\n            }\n        },\n        \"required\": [\"operation\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class553))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class553 stateMachine = new Class553();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageProjectParametersTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class555))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class555 stateMachine = new Class555();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageProjectParametersTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class552))]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class552 stateMachine = new Class552();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageProjectParametersTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class551))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_2(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class551 stateMachine = new Class551();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageProjectParametersTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class556))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_3(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class556 stateMachine = new Class556();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageProjectParametersTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class554))]
	private Task<AIToolResult> method_4(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class554 stateMachine = new Class554();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageProjectParametersTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
