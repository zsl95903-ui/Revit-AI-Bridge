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

[AITool("manage_shared_parameters", Category = "参数管理", Description = "管理共享参数，包括创建、列出、删除和移除绑定。支持GUID唯一性校验和参数使用依赖检查。", RequiresTransaction = true, RequiresModification = true)]
public sealed class ManageSharedParametersTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class557 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public ManageSharedParametersTool manageSharedParametersTool_0;

		private string string_0;

		private object object_0;

		private int int_1;

		private string[] string_1;

		private string string_2;

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
					Class557 stateMachine = this;
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
				object_0 = ielementService_0.GetSharedParameter(aitoolContext_0.Document, string_0);
				if (object_0 == null)
				{
					result = AIToolResult.Fail("找不到共享参数 '" + string_0 + "'");
				}
				else
				{
					int_1 = ielementService_0.GetSharedParameterUsageCount(aitoolContext_0.Document, string_0);
					string_1 = (object_0.GetType().GetProperty("boundCategories")?.GetValue(object_0) as string[]) ?? Array.Empty<string>();
					string_2 = "📋 共享参数 '" + string_0 + "' 使用情况\n\n";
					string_2 = string_2 + "🏷️  绑定类别: " + ((string_1.Length != 0) ? string.Join("、", string_1) : "无") + "\n";
					string text = string_2;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
					defaultInterpolatedStringHandler.AppendLiteral("📊 使用数量: ");
					defaultInterpolatedStringHandler.AppendFormatted(int_1);
					defaultInterpolatedStringHandler.AppendLiteral(" 个元素");
					string_2 = text + defaultInterpolatedStringHandler.ToStringAndClear();
					if (int_1 > 0)
					{
						string_2 += "\n\n⚠️  该参数正在被使用，无法删除。如需删除，请先删除或修改使用该参数的元素。";
					}
					else
					{
						string_2 += "\n\n✅ 该参数未被使用，可以安全删除。";
					}
					result = AIToolResult.Ok(string_2, (object)new Class243<string, string[], int, bool>(string_0, string_1, int_1, int_1 == 0));
				}
			}
			int_0 = -2;
			string_0 = null;
			object_0 = null;
			string_1 = null;
			string_2 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class558 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public ManageSharedParametersTool manageSharedParametersTool_0;

		private string string_0;

		private string string_1;

		private string string_2;

		private object object_0;

		private bool bool_0;

		private List<string> list_0;

		private string[] string_3;

		private object object_1;

		private PropertyInfo propertyInfo_0;

		private bool bool_1;

		private string string_4;

		private string[] string_5;

		private PropertyInfo propertyInfo_1;

		private int int_1;

		private List<string> list_1;

		private List<object> list_2;

		private List<object>.Enumerator enumerator_0;

		private object object_2;

		private PropertyInfo propertyInfo_2;

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
					Class558 stateMachine = this;
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
			string_0 = aitoolContext_0.GetParameter<string>("parameterName", (string)null);
			string_1 = aitoolContext_0.GetParameter<string>("parameterType", (string)null);
			string_2 = aitoolContext_0.GetParameter<string>("groupName", "Text");
			object_0 = aitoolContext_0.GetParameter<object>("categoryNames", (object)null);
			bool_0 = aitoolContext_0.GetParameter<bool>("instanceParameter", true);
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
				list_0 = new List<string>();
				string_3 = object_0 as string[];
				if (string_3 != null)
				{
					list_0.AddRange(string_3);
				}
				else
				{
					list_1 = object_0 as List<string>;
					if (list_1 != null)
					{
						list_0.AddRange(list_1);
					}
					else
					{
						list_2 = object_0 as List<object>;
						if (list_2 != null)
						{
							enumerator_0 = list_2.GetEnumerator();
							try
							{
								while (enumerator_0.MoveNext())
								{
									object_2 = enumerator_0.Current;
									if (object_2 != null)
									{
										list_0.Add(object_2.ToString() ?? "");
									}
									object_2 = null;
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
						}
						list_2 = null;
					}
					list_1 = null;
				}
				if (list_0.Count == 0)
				{
					result = AIToolResult.Fail("categoryNames 参数不能为空");
				}
				else
				{
					object_1 = ielementService_0.CreateSharedParameter(aitoolContext_0.Document, string_0, string_1, string_2, (IEnumerable<string>)list_0, bool_0);
					if (object_1 == null)
					{
						result = AIToolResult.Fail("创建共享参数失败");
					}
					else
					{
						propertyInfo_0 = object_1.GetType().GetProperty("success");
						bool_1 = propertyInfo_0 != null && (bool?)propertyInfo_0.GetValue(object_1) == true;
						if (bool_1)
						{
							PropertyInfo? property = object_1.GetType().GetProperty("parameterName");
							if ((object)property == null)
							{
								obj = null;
							}
							else
							{
								object? value = property.GetValue(object_1);
								if (value == null)
								{
									obj = null;
								}
								else
								{
									obj = value.ToString();
									if (obj != null)
									{
										goto IL_03f5;
									}
								}
							}
							obj = string_0;
							goto IL_03f5;
						}
						propertyInfo_2 = object_1.GetType().GetProperty("error");
						string_6 = propertyInfo_2?.GetValue(object_1)?.ToString();
						result = AIToolResult.Fail("创建共享参数失败: " + string_6);
					}
				}
			}
			goto IL_05d5;
			IL_03f5:
			string_4 = (string)obj;
			string_5 = object_1.GetType().GetProperty("boundCategories")?.GetValue(object_1) as string[];
			propertyInfo_1 = object_1.GetType().GetProperty("categoryCount");
			int_1 = ((propertyInfo_1 != null) ? Convert.ToInt32(propertyInfo_1.GetValue(object_1)) : list_0.Count);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 6);
			defaultInterpolatedStringHandler.AppendLiteral("✅ 成功创建共享参数 '");
			defaultInterpolatedStringHandler.AppendFormatted(string_4);
			defaultInterpolatedStringHandler.AppendLiteral("'\n\n");
			defaultInterpolatedStringHandler.AppendLiteral("📝 类型: ");
			defaultInterpolatedStringHandler.AppendFormatted(string_1);
			defaultInterpolatedStringHandler.AppendLiteral("\n");
			defaultInterpolatedStringHandler.AppendLiteral("📂 组: ");
			defaultInterpolatedStringHandler.AppendFormatted(string_2);
			defaultInterpolatedStringHandler.AppendLiteral("\n");
			defaultInterpolatedStringHandler.AppendLiteral("🏷️  绑定到 ");
			defaultInterpolatedStringHandler.AppendFormatted(int_1);
			defaultInterpolatedStringHandler.AppendLiteral(" 个类别: ");
			defaultInterpolatedStringHandler.AppendFormatted(string.Join("、", string_5 ?? list_0.ToArray()));
			defaultInterpolatedStringHandler.AppendLiteral("\n");
			defaultInterpolatedStringHandler.AppendLiteral("📌 类型: ");
			defaultInterpolatedStringHandler.AppendFormatted(bool_0 ? "实例参数" : "类型参数");
			result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), object_1);
			goto IL_05d5;
			IL_05d5:
			int_0 = -2;
			string_0 = null;
			string_1 = null;
			string_2 = null;
			object_0 = null;
			list_0 = null;
			string_3 = null;
			object_1 = null;
			propertyInfo_0 = null;
			string_4 = null;
			string_5 = null;
			propertyInfo_1 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class559 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public ManageSharedParametersTool manageSharedParametersTool_0;

		private string string_0;

		private int int_1;

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
					Class559 stateMachine = this;
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
				int_1 = ielementService_0.GetSharedParameterUsageCount(aitoolContext_0.Document, string_0);
				if (int_1 > 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 2);
					defaultInterpolatedStringHandler.AppendLiteral("参数 '");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					defaultInterpolatedStringHandler.AppendLiteral("' 正被 ");
					defaultInterpolatedStringHandler.AppendFormatted(int_1);
					defaultInterpolatedStringHandler.AppendLiteral(" 个元素使用，无法删除。请先删除或修改使用该参数的元素。");
					result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					bool_0 = ielementService_0.DeleteSharedParameter(aitoolContext_0.Document, string_0);
					result = ((!bool_0) ? AIToolResult.Fail("删除共享参数 '" + string_0 + "' 失败") : AIToolResult.Ok("✅ 成功删除共享参数 '" + string_0 + "'", (object)null));
				}
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
	public sealed class Class560 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ManageSharedParametersTool manageSharedParametersTool_0;

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
					Class560 stateMachine = this;
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
							if (text2 == "create")
							{
								awaiter6 = manageSharedParametersTool_0.method_0(aitoolContext_0, ielementService_0).GetAwaiter();
								if (!awaiter6.IsCompleted)
								{
									num = 1;
									int_0 = 1;
									taskAwaiter_1 = awaiter6;
									Class560 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter6, ref stateMachine);
									return;
								}
								goto IL_0382;
							}
							if (text2 == "list")
							{
								awaiter5 = manageSharedParametersTool_0.method_1(aitoolContext_0, ielementService_0).GetAwaiter();
								if (!awaiter5.IsCompleted)
								{
									num = 2;
									int_0 = 2;
									taskAwaiter_1 = awaiter5;
									Class560 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
									return;
								}
								goto IL_03b9;
							}
							if (text2 == "delete")
							{
								awaiter4 = manageSharedParametersTool_0.method_2(aitoolContext_0, ielementService_0).GetAwaiter();
								if (!awaiter4.IsCompleted)
								{
									num = 3;
									int_0 = 3;
									taskAwaiter_1 = awaiter4;
									Class560 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
									return;
								}
								goto IL_03f0;
							}
							if (text2 == "unbind")
							{
								awaiter3 = manageSharedParametersTool_0.method_3(aitoolContext_0, ielementService_0).GetAwaiter();
								if (!awaiter3.IsCompleted)
								{
									num = 4;
									int_0 = 4;
									taskAwaiter_1 = awaiter3;
									Class560 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
									return;
								}
								goto IL_0427;
							}
							if (text2 == "check")
							{
								awaiter2 = manageSharedParametersTool_0.method_4(aitoolContext_0, ielementService_0).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = 5;
									int_0 = 5;
									taskAwaiter_1 = awaiter2;
									Class560 stateMachine = this;
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
				result = AIToolResult.Fail("管理共享参数失败: " + exception_0.Message);
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
	public sealed class Class561 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public ManageSharedParametersTool manageSharedParametersTool_0;

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

		private string string_7;

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
					Class561 stateMachine = this;
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
			ienumerable_0 = ielementService_0.GetAllSharedParameters(aitoolContext_0.Document);
			list_0 = ienumerable_0.ToList();
			AIToolResult result;
			if (list_0.Count == 0)
			{
				result = AIToolResult.Ok("当前文档中没有共享参数", (object)null);
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
				defaultInterpolatedStringHandler.AppendLiteral("找到 ");
				defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个共享参数");
				string_0 = defaultInterpolatedStringHandler.ToStringAndClear();
				stringBuilder_0 = new StringBuilder();
				StringBuilder stringBuilder = stringBuilder_0;
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(10, 1, stringBuilder);
				handler.AppendLiteral("找到 ");
				handler.AppendFormatted(list_0.Count);
				handler.AppendLiteral(" 个共享参数：");
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
						string_6 = object_0.GetType().GetProperty("guid")?.GetValue(object_0)?.ToString();
						string text = string_4;
						string text2 = ((text == "instance") ? "实例参数" : ((text == "type") ? "类型参数" : "未知"));
						string_7 = text2;
						stringBuilder = stringBuilder_0;
						StringBuilder stringBuilder3 = stringBuilder;
						handler = new StringBuilder.AppendInterpolatedStringHandler(31, 6, stringBuilder);
						handler.AppendLiteral("- ");
						handler.AppendFormatted(string_1);
						handler.AppendLiteral(" (");
						handler.AppendFormatted(string_2);
						handler.AppendLiteral(") | ");
						handler.AppendFormatted(string_7);
						handler.AppendLiteral(" | ");
						handler.AppendFormatted<object>(object_1);
						handler.AppendLiteral(" 个类别 | 分组: ");
						handler.AppendFormatted(string_5);
						handler.AppendLiteral(" | GUID: ");
						handler.AppendFormatted(string_6);
						stringBuilder3.AppendLine(ref handler);
						string_1 = null;
						string_2 = null;
						string_3 = null;
						object_1 = null;
						string_4 = null;
						string_5 = null;
						string_6 = null;
						string_7 = null;
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
	public sealed class Class562 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public ManageSharedParametersTool manageSharedParametersTool_0;

		private string string_0;

		private object object_0;

		private List<string> list_0;

		private int int_1;

		private string[] string_1;

		private List<string> list_1;

		private List<object> list_2;

		private List<object>.Enumerator enumerator_0;

		private object object_1;

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
					Class562 stateMachine = this;
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
			string_0 = aitoolContext_0.GetParameter<string>("parameterName", (string)null);
			object_0 = aitoolContext_0.GetParameter<object>("categoryNames", (object)null);
			AIToolResult result;
			if (string.IsNullOrEmpty(string_0))
			{
				result = AIToolResult.Fail("parameterName 参数不能为空");
			}
			else
			{
				list_0 = new List<string>();
				if (object_0 != null)
				{
					string_1 = object_0 as string[];
					if (string_1 != null)
					{
						list_0.AddRange(string_1);
					}
					else
					{
						list_1 = object_0 as List<string>;
						if (list_1 != null)
						{
							list_0.AddRange(list_1);
						}
						else
						{
							list_2 = object_0 as List<object>;
							if (list_2 != null)
							{
								enumerator_0 = list_2.GetEnumerator();
								try
								{
									while (enumerator_0.MoveNext())
									{
										object_1 = enumerator_0.Current;
										if (object_1 != null)
										{
											list_0.Add(object_1.ToString() ?? "");
										}
										object_1 = null;
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
							}
							list_2 = null;
						}
						list_1 = null;
					}
					string_1 = null;
				}
				if (list_0.Count == 0)
				{
					result = AIToolResult.Fail("categoryNames 参数不能为空");
				}
				else
				{
					int_1 = ielementService_0.RemoveSharedParameterBindings(aitoolContext_0.Document, string_0, (IEnumerable<string>)list_0);
					if (int_1 > 0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 3);
						defaultInterpolatedStringHandler.AppendLiteral("✅ 成功从 ");
						defaultInterpolatedStringHandler.AppendFormatted(int_1);
						defaultInterpolatedStringHandler.AppendLiteral(" 个类别移除参数 '");
						defaultInterpolatedStringHandler.AppendFormatted(string_0);
						defaultInterpolatedStringHandler.AppendLiteral("' 的绑定\n\n");
						defaultInterpolatedStringHandler.AppendLiteral("移除的类别: ");
						defaultInterpolatedStringHandler.AppendFormatted(string.Join("、", list_0));
						result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)null);
					}
					else
					{
						result = AIToolResult.Fail("移除绑定失败：参数 '" + string_0 + "' 未绑定到指定类别");
					}
				}
			}
			int_0 = -2;
			string_0 = null;
			object_0 = null;
			list_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "manage_shared_parameters";

	public string Category => "参数管理";

	public string Description => "管理共享参数，包括创建、列出、删除和移除绑定";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operation\": {\n                \"type\": \"string\",\n                \"description\": \"操作类型：'create'（创建）、'list'（列出）、'delete'（删除）、'unbind'（移除绑定）、'check'（检查使用情况）\",\n                \"enum\": [\"create\", \"list\", \"delete\", \"unbind\", \"check\"]\n            },\n            \"parameterName\": {\n                \"type\": \"string\",\n                \"description\": \"参数名称（create、delete、unbind、check 操作时必需）\"\n            },\n            \"parameterType\": {\n                \"type\": \"string\",\n                \"description\": \"参数类型（create 操作时必需）。支持：Text（文本）、Integer（整数）、Number（数值）、Length（长度）、Area（面积）、Volume（体积）、Angle（角度）、YesNo（是/否）、Material（材质）\"\n            },\n            \"groupName\": {\n                \"type\": \"string\",\n                \"description\": \"参数组名称（create 操作时可选，默认为 'Text'）。常用组：数据、文字、约束、尺寸标注、结构等\"\n            },\n            \"categoryNames\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"string\" },\n                \"description\": \"要绑定的类别名称列表（create 操作时必需）。例如：['墙', '门', '窗']。使用中文名称，与 Revit 界面中的类别名称一致\"\n            },\n            \"instanceParameter\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否为实例参数（create 操作时可选，默认 true）\",\n                \"default\": true\n            }\n        },\n        \"required\": [\"operation\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class560))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class560 stateMachine = new Class560();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageSharedParametersTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class558))]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class558 stateMachine = new Class558();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageSharedParametersTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class561))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class561 stateMachine = new Class561();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageSharedParametersTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class559))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_2(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class559 stateMachine = new Class559();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageSharedParametersTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class562))]
	private Task<AIToolResult> method_3(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class562 stateMachine = new Class562();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageSharedParametersTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class557))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_4(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class557 stateMachine = new Class557();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.manageSharedParametersTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
