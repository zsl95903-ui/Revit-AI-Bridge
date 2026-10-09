using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

[AITool("set_parameter_values", Category = "参数操作", Description = "设置元素参数值，支持简单批量模式和高级批量模式。此工具修改文档，需要事务。", RequiresTransaction = true, RequiresModification = true)]
public sealed class SetParameterValuesTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class632 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public SetParameterValuesTool setParameterValuesTool_0;

		private IElementService ielementService_0;

		private IParameterService iparameterService_0;

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
					Class632 stateMachine = this;
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
					goto IL_0218;
				}
				TaskAwaiter<AIToolResult> awaiter3;
				if (num == 2)
				{
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_01e5;
				}
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				iparameterService_0 = ((revitAdapter2 != null) ? revitAdapter2.ParameterService : null);
				if (ielementService_0 == null || iparameterService_0 == null)
				{
					result = AIToolResult.Fail("无法获取服务");
				}
				else
				{
					if (aitoolContext_0.Document != null)
					{
						if (aitoolContext_0.HasParameter("elementParameterValues"))
						{
							awaiter2 = setParameterValuesTool_0.method_1(aitoolContext_0, ielementService_0, iparameterService_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter2;
								Class632 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
								return;
							}
							goto IL_0218;
						}
						awaiter3 = setParameterValuesTool_0.method_0(aitoolContext_0, ielementService_0, iparameterService_0).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_1 = awaiter3;
							Class632 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
							return;
						}
						goto IL_01e5;
					}
					result = AIToolResult.Fail("文档对象为空");
				}
				goto end_IL_006e;
				IL_0218:
				aitoolResult_0 = awaiter2.GetResult();
				result = aitoolResult_0;
				goto end_IL_006e;
				IL_01e5:
				aitoolResult_1 = awaiter3.GetResult();
				result = aitoolResult_1;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("设置参数值失败: " + exception_0.Message);
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
	public sealed class Class633 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public IParameterService iparameterService_0;

		public SetParameterValuesTool setParameterValuesTool_0;

		private object object_0;

		private object object_1;

		private object object_2;

		private List<(int elementId, string parameterName, object value, string unit)> list_0;

		private int int_1;

		private List<object> list_1;

		private int int_2;

		private int int_3;

		private List<string> list_2;

		private string string_0;

		private List<object>.Enumerator enumerator_0;

		private object object_3;

		private IDictionary<string, object> idictionary_0;

		private object object_4;

		private object object_5;

		private string string_1;

		private object object_6;

		private object object_7;

		private object object_8;

		private string string_2;

		private object object_9;

		private int int_4;

		private PropertyInfo[] propertyInfo_0;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo propertyInfo_2;

		private PropertyInfo propertyInfo_3;

		private PropertyInfo propertyInfo_4;

		private object object_10;

		private string string_3;

		private object object_11;

		private string string_4;

		private int int_5;

		private List<(int elementId, string parameterName, object value, string unit)>.Enumerator enumerator_1;

		private int int_6;

		private string string_5;

		private object object_12;

		private string string_6;

		private object object_13;

		private bool bool_0;

		private Exception exception_0;

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
					Class633 stateMachine = this;
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
			object_0 = (aitoolContext_0.Parameters.TryGetValue("elementParameterValues", out object_1) ? object_1 : null);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(69, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[SetParameterValuesAdvanced] 🔍 AI 调用参数：elementParameterValues 类型=");
			defaultInterpolatedStringHandler.AppendFormatted(object_0?.GetType().Name);
			defaultInterpolatedStringHandler.AppendLiteral("，值=");
			defaultInterpolatedStringHandler.AppendFormatted<object>(object_0);
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			if (aitoolContext_0.Parameters.ContainsKey("elementId"))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(49, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("[SetParameterValuesAdvanced] ⚠️ 同时存在参数：elementId=");
				defaultInterpolatedStringHandler2.AppendFormatted<object>(aitoolContext_0.Parameters["elementId"]);
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			if (aitoolContext_0.Parameters.ContainsKey("parameterName"))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(53, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("[SetParameterValuesAdvanced] ⚠️ 同时存在参数：parameterName=");
				defaultInterpolatedStringHandler3.AppendFormatted<object>(aitoolContext_0.Parameters["parameterName"]);
				Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
			}
			if (aitoolContext_0.Parameters.ContainsKey("value"))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(45, 1);
				defaultInterpolatedStringHandler4.AppendLiteral("[SetParameterValuesAdvanced] ⚠️ 同时存在参数：value=");
				defaultInterpolatedStringHandler4.AppendFormatted<object>(aitoolContext_0.Parameters["value"]);
				Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
			}
			object_2 = aitoolContext_0.GetParameter<object>("elementParameterValues", (object)null);
			AIToolResult result;
			if (object_2 == null)
			{
				result = AIToolResult.Fail("elementParameterValues 参数不能为空");
			}
			else
			{
				list_0 = new List<(int, string, object, string)>();
				int_1 = 0;
				list_1 = object_2 as List<object>;
				if (list_1 != null)
				{
					enumerator_0 = list_1.GetEnumerator();
					try
					{
						while (enumerator_0.MoveNext())
						{
							object_3 = enumerator_0.Current;
							if (object_3 != null)
							{
								try
								{
									idictionary_0 = object_3 as IDictionary<string, object>;
									if (idictionary_0 != null)
									{
										object_4 = (idictionary_0.TryGetValue("elementId", out object_5) ? object_5 : null);
										string_1 = ((!idictionary_0.TryGetValue("parameterName", out object_6)) ? null : object_6?.ToString());
										object_7 = (idictionary_0.TryGetValue("value", out object_8) ? object_8 : null);
										string_2 = ((!idictionary_0.TryGetValue("unit", out object_9)) ? "mm" : ((object_9 != null) ? object_9.ToString() : "mm"));
										if (object_4 != null && int.TryParse(object_4.ToString(), out int_4) && !string.IsNullOrEmpty(string_1) && object_7 != null)
										{
											list_0.Add((int_4, string_1 ?? "", object_7, string_2 ?? "mm"));
										}
										else
										{
											int_1++;
										}
										object_4 = null;
										object_5 = null;
										string_1 = null;
										object_6 = null;
										object_7 = null;
										object_8 = null;
										string_2 = null;
										object_9 = null;
										goto IL_0718;
									}
									propertyInfo_0 = object_3.GetType().GetProperties();
									propertyInfo_1 = propertyInfo_0.FirstOrDefault((PropertyInfo propertyInfo_0) => "elementId".Equals(propertyInfo_0.Name, StringComparison.OrdinalIgnoreCase));
									propertyInfo_2 = propertyInfo_0.FirstOrDefault((PropertyInfo propertyInfo_0) => "parameterName".Equals(propertyInfo_0.Name, StringComparison.OrdinalIgnoreCase));
									propertyInfo_3 = propertyInfo_0.FirstOrDefault((PropertyInfo propertyInfo_0) => "value".Equals(propertyInfo_0.Name, StringComparison.OrdinalIgnoreCase));
									propertyInfo_4 = propertyInfo_0.FirstOrDefault((PropertyInfo propertyInfo_0) => "unit".Equals(propertyInfo_0.Name, StringComparison.OrdinalIgnoreCase));
									object obj;
									if (propertyInfo_1 != null && propertyInfo_2 != null && propertyInfo_3 != null)
									{
										object_10 = propertyInfo_1.GetValue(object_3);
										string_3 = propertyInfo_2.GetValue(object_3)?.ToString();
										object_11 = propertyInfo_3.GetValue(object_3);
										PropertyInfo propertyInfo = propertyInfo_4;
										if ((object)propertyInfo == null)
										{
											obj = null;
										}
										else
										{
											object? value = propertyInfo.GetValue(object_3);
											if (value == null)
											{
												obj = null;
											}
											else
											{
												obj = value.ToString();
												if (obj != null)
												{
													goto IL_062d;
												}
											}
										}
										obj = "mm";
										goto IL_062d;
									}
									int_1++;
									goto IL_06f5;
									IL_0718:
									idictionary_0 = null;
									goto end_IL_02d6;
									IL_06f5:
									propertyInfo_0 = null;
									propertyInfo_1 = null;
									propertyInfo_2 = null;
									propertyInfo_3 = null;
									propertyInfo_4 = null;
									goto IL_0718;
									IL_062d:
									string_4 = (string)obj;
									if (object_10 != null && int.TryParse(object_10.ToString(), out int_5) && !string.IsNullOrEmpty(string_3) && object_11 != null)
									{
										list_0.Add((int_5, string_3 ?? "", object_11, string_4 ?? "mm"));
									}
									else
									{
										int_1++;
									}
									object_10 = null;
									string_3 = null;
									object_11 = null;
									string_4 = null;
									goto IL_06f5;
									end_IL_02d6:;
								}
								catch (Exception)
								{
									int_1++;
								}
							}
							else
							{
								int_1++;
							}
							object_3 = null;
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
				if (list_0.Count == 0)
				{
					result = AIToolResult.Fail("elementParameterValues 参数格式不正确或为空");
				}
				else
				{
					int_2 = 0;
					int_3 = 0;
					list_2 = new List<string>();
					enumerator_1 = list_0.GetEnumerator();
					try
					{
						while (enumerator_1.MoveNext())
						{
							(int, string, object, string) current = enumerator_1.Current;
							int_6 = current.Item1;
							string_5 = current.Item2;
							object_12 = current.Item3;
							string_6 = current.Item4;
							object_13 = ielementService_0.GetElementById(aitoolContext_0.Document, int_6);
							if (object_13 == null)
							{
								int_3++;
								List<string> list = list_2;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(7, 1);
								defaultInterpolatedStringHandler5.AppendLiteral("元素 ");
								defaultInterpolatedStringHandler5.AppendFormatted(int_6);
								defaultInterpolatedStringHandler5.AppendLiteral(" 不存在");
								list.Add(defaultInterpolatedStringHandler5.ToStringAndClear());
								continue;
							}
							try
							{
								bool_0 = setParameterValuesTool_0.method_2(aitoolContext_0, object_13, ielementService_0, iparameterService_0, string_5, object_12, string_6);
							}
							catch (Exception ex2)
							{
								exception_0 = ex2;
								bool_0 = false;
								List<string> list2 = list_2;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(5, 2);
								defaultInterpolatedStringHandler6.AppendLiteral("元素 ");
								defaultInterpolatedStringHandler6.AppendFormatted(int_6);
								defaultInterpolatedStringHandler6.AppendLiteral(": ");
								defaultInterpolatedStringHandler6.AppendFormatted(exception_0.Message);
								list2.Add(defaultInterpolatedStringHandler6.ToStringAndClear());
							}
							if (bool_0)
							{
								int_2++;
							}
							else
							{
								int_3++;
								if (list_2.Count < 5)
								{
									List<string> list3 = list_2;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(15, 2);
									defaultInterpolatedStringHandler7.AppendLiteral("元素 ");
									defaultInterpolatedStringHandler7.AppendFormatted(int_6);
									defaultInterpolatedStringHandler7.AppendLiteral(": 设置参数 '");
									defaultInterpolatedStringHandler7.AppendFormatted(string_5);
									defaultInterpolatedStringHandler7.AppendLiteral("' 失败");
									list3.Add(defaultInterpolatedStringHandler7.ToStringAndClear());
								}
							}
							object_13 = null;
							string_5 = null;
							object_12 = null;
							string_6 = null;
						}
					}
					finally
					{
						if (num < 0)
						{
							((IDisposable)enumerator_1/*cast due to constrained. prefix*/).Dispose();
						}
					}
					enumerator_1 = default(List<(int, string, object, string)>.Enumerator);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(21, 1);
					defaultInterpolatedStringHandler8.AppendLiteral("📝 高级批量模式：尝试设置 ");
					defaultInterpolatedStringHandler8.AppendFormatted(list_0.Count);
					defaultInterpolatedStringHandler8.AppendLiteral(" 个参数值\n");
					string_0 = defaultInterpolatedStringHandler8.ToStringAndClear();
					string text = string_0;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(9, 1);
					defaultInterpolatedStringHandler9.AppendLiteral("✅ 成功设置 ");
					defaultInterpolatedStringHandler9.AppendFormatted(int_2);
					defaultInterpolatedStringHandler9.AppendLiteral(" 个");
					string_0 = text + defaultInterpolatedStringHandler9.ToStringAndClear();
					if (int_3 > 0)
					{
						string text2 = string_0;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(9, 1);
						defaultInterpolatedStringHandler10.AppendLiteral("，⚠️ 失败 ");
						defaultInterpolatedStringHandler10.AppendFormatted(int_3);
						defaultInterpolatedStringHandler10.AppendLiteral(" 个");
						string_0 = text2 + defaultInterpolatedStringHandler10.ToStringAndClear();
						if (list_2.Count > 0)
						{
							string_0 = string_0 + "\n\n❌ 部分错误：\n" + string.Join("\n", list_2.Take(3));
							if (list_2.Count > 3)
							{
								string text3 = string_0;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler11 = new DefaultInterpolatedStringHandler(12, 1);
								defaultInterpolatedStringHandler11.AppendLiteral("\n... 还有 ");
								defaultInterpolatedStringHandler11.AppendFormatted(list_2.Count - 3);
								defaultInterpolatedStringHandler11.AppendLiteral(" 个错误");
								string_0 = text3 + defaultInterpolatedStringHandler11.ToStringAndClear();
							}
						}
					}
					result = AIToolResult.Ok(string_0, (object)new Class297<string, int, int, int, List<string>>("advanced_batch", list_0.Count, int_2, int_3, (list_2.Count > 0) ? list_2 : null));
				}
			}
			int_0 = -2;
			object_0 = null;
			object_1 = null;
			object_2 = null;
			list_0 = null;
			list_1 = null;
			list_2 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class634 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public IParameterService iparameterService_0;

		public SetParameterValuesTool setParameterValuesTool_0;

		private string string_0;

		private object object_0;

		private object object_1;

		private string string_1;

		private List<int> list_0;

		private int int_1;

		private int int_2;

		private List<string> list_1;

		private string string_2;

		private int int_3;

		private object object_2;

		private int[] int_4;

		private List<int> list_2;

		private List<object> list_3;

		private List<object>.Enumerator enumerator_0;

		private object object_3;

		private int int_5;

		private string string_3;

		private object object_4;

		private List<int> list_4;

		private List<int>.Enumerator enumerator_1;

		private int int_6;

		private object object_5;

		private bool bool_0;

		private Exception exception_0;

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
					Class634 stateMachine = this;
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
			object_0 = (aitoolContext_0.Parameters.TryGetValue("value", out object_1) ? object_1 : null);
			string_1 = aitoolContext_0.GetParameter<string>("unit", (string)null);
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
				if (string.IsNullOrEmpty(string_1))
				{
					string_1 = "mm";
					Logger.Info("[SetParameterValues] 未指定单位，使用默认值: mm");
				}
				list_0 = new List<int>();
				if (aitoolContext_0.HasParameter("elementId"))
				{
					int_3 = aitoolContext_0.GetParameter<int>("elementId", 0);
					if (int_3 > 0)
					{
						list_0.Add(int_3);
					}
				}
				if (aitoolContext_0.HasParameter("elementIds"))
				{
					object_2 = aitoolContext_0.GetParameter<object>("elementIds", (object)null);
					int_4 = object_2 as int[];
					if (int_4 != null)
					{
						list_0.AddRange(int_4);
					}
					else
					{
						list_2 = object_2 as List<int>;
						if (list_2 != null)
						{
							list_0.AddRange(list_2);
						}
						else
						{
							list_3 = object_2 as List<object>;
							if (list_3 != null)
							{
								enumerator_0 = list_3.GetEnumerator();
								try
								{
									while (enumerator_0.MoveNext())
									{
										object_3 = enumerator_0.Current;
										if (object_3 != null && int.TryParse(object_3.ToString(), out int_5))
										{
											list_0.Add(int_5);
										}
										object_3 = null;
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
							list_3 = null;
						}
						list_2 = null;
					}
					object_2 = null;
					int_4 = null;
				}
				if (aitoolContext_0.HasParameter("cacheId"))
				{
					string_3 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
					if (string_3 != null && string_3.Length > 0)
					{
						object_4 = aitoolContext_0.DataCache.Retrieve<object>(string_3);
						if (object_4 != null)
						{
							list_4 = setParameterValuesTool_0.method_3(object_4);
							if (list_4 != null && list_4.Count > 0)
							{
								list_0.AddRange(list_4);
							}
							list_4 = null;
						}
						object_4 = null;
					}
					string_3 = null;
				}
				if (list_0.Count == 0)
				{
					result = AIToolResult.Fail("未提供有效的元素 ID（请使用 elementId、elementIds 或 cacheId 参数，或使用高级批量模式 elementParameterValues）");
				}
				else
				{
					int_1 = 0;
					int_2 = 0;
					list_1 = new List<string>();
					enumerator_1 = list_0.GetEnumerator();
					try
					{
						while (enumerator_1.MoveNext())
						{
							int_6 = enumerator_1.Current;
							object_5 = ielementService_0.GetElementById(aitoolContext_0.Document, int_6);
							if (object_5 == null)
							{
								int_2++;
								List<string> list = list_1;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
								defaultInterpolatedStringHandler.AppendLiteral("元素 ");
								defaultInterpolatedStringHandler.AppendFormatted(int_6);
								defaultInterpolatedStringHandler.AppendLiteral(" 不存在");
								list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
								continue;
							}
							try
							{
								bool_0 = setParameterValuesTool_0.method_2(aitoolContext_0, object_5, ielementService_0, iparameterService_0, string_0, object_0, string_1);
							}
							catch (Exception ex)
							{
								exception_0 = ex;
								bool_0 = false;
								List<string> list2 = list_1;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(5, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("元素 ");
								defaultInterpolatedStringHandler2.AppendFormatted(int_6);
								defaultInterpolatedStringHandler2.AppendLiteral(": ");
								defaultInterpolatedStringHandler2.AppendFormatted(exception_0.Message);
								list2.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
							}
							if (bool_0)
							{
								int_1++;
							}
							else
							{
								int_2++;
								if (list_1.Count < 5)
								{
									List<string> list3 = list_1;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(9, 1);
									defaultInterpolatedStringHandler3.AppendLiteral("元素 ");
									defaultInterpolatedStringHandler3.AppendFormatted(int_6);
									defaultInterpolatedStringHandler3.AppendLiteral(": 设置失败");
									list3.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
								}
							}
							object_5 = null;
						}
					}
					finally
					{
						if (num < 0)
						{
							((IDisposable)enumerator_1/*cast due to constrained. prefix*/).Dispose();
						}
					}
					enumerator_1 = default(List<int>.Enumerator);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(19, 2);
					defaultInterpolatedStringHandler4.AppendLiteral("📝 尝试设置 ");
					defaultInterpolatedStringHandler4.AppendFormatted(list_0.Count);
					defaultInterpolatedStringHandler4.AppendLiteral(" 个元素的参数 '");
					defaultInterpolatedStringHandler4.AppendFormatted(string_0);
					defaultInterpolatedStringHandler4.AppendLiteral("'\n");
					string_2 = defaultInterpolatedStringHandler4.ToStringAndClear();
					string text = string_2;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(11, 1);
					defaultInterpolatedStringHandler5.AppendLiteral("✅ 成功设置 ");
					defaultInterpolatedStringHandler5.AppendFormatted(int_1);
					defaultInterpolatedStringHandler5.AppendLiteral(" 个元素");
					string_2 = text + defaultInterpolatedStringHandler5.ToStringAndClear();
					if (int_2 > 0)
					{
						string text2 = string_2;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(9, 1);
						defaultInterpolatedStringHandler6.AppendLiteral("，⚠️ 失败 ");
						defaultInterpolatedStringHandler6.AppendFormatted(int_2);
						defaultInterpolatedStringHandler6.AppendLiteral(" 个");
						string_2 = text2 + defaultInterpolatedStringHandler6.ToStringAndClear();
						if (list_1.Count > 0)
						{
							string_2 = string_2 + "\n\n❌ 部分错误：\n" + string.Join("\n", list_1.Take(3));
							if (list_1.Count > 3)
							{
								string text3 = string_2;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(12, 1);
								defaultInterpolatedStringHandler7.AppendLiteral("\n... 还有 ");
								defaultInterpolatedStringHandler7.AppendFormatted(list_1.Count - 3);
								defaultInterpolatedStringHandler7.AppendLiteral(" 个错误");
								string_2 = text3 + defaultInterpolatedStringHandler7.ToStringAndClear();
							}
						}
					}
					result = AIToolResult.Ok(string_2, (object)new Class296<int, int, int, List<string>>(list_0.Count, int_1, int_2, (list_1.Count > 0) ? list_1 : null));
				}
			}
			int_0 = -2;
			string_0 = null;
			object_0 = null;
			object_1 = null;
			string_1 = null;
			list_0 = null;
			list_1 = null;
			string_2 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "set_parameter_values";

	public string Category => "参数操作";

	public string Description => "设置元素参数值（需要事务）";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"elementId\": {\n                \"type\": \"integer\",\n                \"description\": \"单个元素 ID（可选）。如果要操作单个元素，使用此参数。\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"元素 ID 数组（可选）。如果要批量操作多个元素，使用此参数，例如：[12345, 12346, 12347]\"\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（可选）。从上一个查询工具（如 element_query）的返回结果中获取 cache_id 字段。使用缓存可以批量操作之前查询到的所有元素。注意：只需提供单个 cache_id 字符串，不需要数组。\"\n            },\n            \"parameterName\": {\n                \"type\": \"string\",\n                \"description\": \"参数名称（简单批量模式时必需）。例如：材质、宽度、偏移量等\"\n            },\n            \"value\": {\n                \"description\": \"参数数值（简单批量模式时必需）。支持：\\n- 整数或浮点数（用于长度、数值等参数）：200、150.5、1000\\n- 整数 ElementId（用于材质、楼层等参数）：提供材质的 ElementId\\n- 布尔值：true/false\\n- 字符串：文本值\"\n            },\n            \"unit\": {\n                \"type\": \"string\",\n                \"description\": \"参数单位（简单批量模式时可选，默认为 mm）。支持：mm（毫米）、m（米）、ft（英尺）、in（英寸）、deg（度）、rad（弧度）。对于长度参数需要明确指定单位，对于其他类型参数（如材质、布尔值）可以使用默认值\",\n                \"enum\": [\"mm\", \"m\", \"ft\", \"in\", \"deg\", \"rad\"]\n            },\n            \"elementParameterValues\": {\n                \"type\": \"array\",\n                \"description\": \"元素参数值列表（高级批量模式）。每个对象指定一个元素的参数设置。使用此参数时，不要同时提供 elementIds/parameterName/value/unit。每个对象必须包含 elementId、parameterName、value 字段，可选 unit 字段。示例：为元素12345设置偏移量200mm，为元素12346设置宽度300mm\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"elementId\": { \"type\": \"integer\", \"description\": \"元素 ID\" },\n                        \"parameterName\": { \"type\": \"string\", \"description\": \"参数名称\" },\n                        \"value\": { \"description\": \"参数值\" },\n                        \"unit\": { \"type\": \"string\", \"description\": \"单位（可选，默认 mm）\", \"enum\": [\"mm\", \"m\", \"ft\", \"in\", \"deg\", \"rad\"] }\n                    },\n                    \"required\": [\"elementId\", \"parameterName\", \"value\"]\n                }\n            }\n        }\n    }";

	[AsyncStateMachine(typeof(Class632))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class632 stateMachine = new Class632();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setParameterValuesTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class634))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, IElementService ielementService_0, IParameterService iparameterService_0)
	{
		Class634 stateMachine = new Class634();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setParameterValuesTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.iparameterService_0 = iparameterService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class633))]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, IElementService ielementService_0, IParameterService iparameterService_0)
	{
		Class633 stateMachine = new Class633();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setParameterValuesTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.iparameterService_0 = iparameterService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private bool method_2(AIToolContext aitoolContext_0, object object_0, IElementService ielementService_0, IParameterService iparameterService_0, string string_0, object? object_1, string string_1)
	{
		if (object_1 == null)
		{
			Logger.Warning("[SetParameterValues] 参数值为 null，无法设置");
			return false;
		}
		object parameter = iparameterService_0.GetParameter(object_0, string_0);
		object obj = object_0;
		if (parameter == null)
		{
			int? elementTypeId = ielementService_0.GetElementTypeId(object_0);
			if (elementTypeId.HasValue)
			{
				object elementById = ielementService_0.GetElementById(aitoolContext_0.Document, elementTypeId.Value);
				if (elementById != null)
				{
					parameter = iparameterService_0.GetParameter(elementById, string_0);
					if (parameter != null)
					{
						obj = elementById;
					}
				}
			}
		}
		if (parameter != null)
		{
			string parameterType = iparameterService_0.GetParameterType(parameter);
			bool flag = iparameterService_0.IsParameterReadOnly(parameter);
			string parameterDefinitionType = iparameterService_0.GetParameterDefinitionType(parameter);
			if (flag)
			{
				Logger.Warning("[SetParameterValues] 参数 '" + string_0 + "' 是只读的");
				return false;
			}
			object obj2 = null;
			try
			{
				object obj3;
				if (parameterType == null)
				{
					obj3 = null;
				}
				else
				{
					obj3 = parameterType.ToLowerInvariant();
					if (obj3 != null)
					{
						goto IL_00e3;
					}
				}
				obj3 = "string";
				goto IL_00e3;
				IL_0101:
				object obj4;
				string text = (string)obj4;
				string text2;
				if (!(text2 == "double"))
				{
					obj2 = ((text2 == "integer") ? ((object_1 is int num) ? ((object)num) : ((!(object_1 is double a) || 1 == 0) ? ((object)Convert.ToInt32(object_1)) : ((object)(int)Math.Round(a)))) : ((text2 == "string") ? object_1.ToString() : ((!(text2 == "elementid")) ? object_1 : ((!(object_1 is int num2) || 1 == 0) ? ((object)Convert.ToInt32(object_1)) : ((object)num2)))));
				}
				else
				{
					double num3 = ((!(object_1 is double num4)) ? Convert.ToDouble(object_1) : num4);
					if (text == "length" || text.Contains("length"))
					{
						string text3 = string_1.ToLowerInvariant();
						double num5 = ((text3 == "mm") ? (num3 / 304.8) : ((text3 == "m") ? (num3 / 0.3048) : ((text3 == "ft") ? num3 : ((text3 == "in") ? (num3 / 12.0) : (num3 / 304.8)))));
						obj2 = num5;
					}
					else
					{
						obj2 = num3;
					}
				}
				if (obj2 != null)
				{
					object parameter2 = iparameterService_0.GetParameter(obj, string_0);
					if (parameter2 != null)
					{
						try
						{
							bool flag2 = false;
							if (obj2 is double num6)
							{
								MethodInfo method = parameter2.GetType().GetMethod("Set", new Type[1] { typeof(double) });
								if (method != null)
								{
									method.Invoke(parameter2, new object[1] { num6 });
									flag2 = true;
								}
							}
							else if (obj2 is int num7)
							{
								MethodInfo method2 = parameter2.GetType().GetMethod("Set", new Type[1] { typeof(int) });
								if (method2 != null)
								{
									method2.Invoke(parameter2, new object[1] { num7 });
									flag2 = true;
								}
							}
							else if (obj2 is string text4)
							{
								MethodInfo method3 = parameter2.GetType().GetMethod("Set", new Type[1] { typeof(string) });
								if (method3 != null)
								{
									method3.Invoke(parameter2, new object[1] { text4 });
									flag2 = true;
								}
							}
							else if (obj2 is bool flag3)
							{
								MethodInfo method4 = parameter2.GetType().GetMethod("Set", new Type[1] { typeof(int) });
								if (method4 != null)
								{
									method4.Invoke(parameter2, new object[1] { flag3 ? 1 : 0 });
									flag2 = true;
								}
							}
							else
							{
								Logger.Warning("[SetParameterValues] 不支持的值类型: " + obj2.GetType().Name);
							}
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
							defaultInterpolatedStringHandler.AppendLiteral("[SetParameterValues] 设置结果: ");
							defaultInterpolatedStringHandler.AppendFormatted(flag2);
							Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
							return flag2;
						}
						catch (TargetInvocationException ex)
						{
							string text5 = "[SetParameterValues] 参数设置异常: ";
							Exception? innerException = ex.InnerException;
							object obj5;
							if (innerException == null)
							{
								obj5 = null;
							}
							else
							{
								obj5 = innerException.Message;
								if (obj5 != null)
								{
									goto IL_0554;
								}
							}
							obj5 = ex.Message;
							goto IL_0554;
							IL_0586:
							object text6;
							object obj6;
							Logger.Error((string?)text6 + (string?)obj6);
							return false;
							IL_0554:
							Logger.Error(text5 + (string?)obj5);
							text6 = "[SetParameterValues] 堆栈: ";
							Exception? innerException2 = ex.InnerException;
							if (innerException2 == null)
							{
								obj6 = null;
							}
							else
							{
								obj6 = innerException2.StackTrace;
								if (obj6 != null)
								{
									goto IL_0586;
								}
							}
							obj6 = ex.StackTrace;
							goto IL_0586;
						}
						catch (Exception ex2)
						{
							Logger.Error("[SetParameterValues] 设置参数值失败: " + ex2.Message + "\n堆栈: " + ex2.StackTrace);
							return false;
						}
					}
					Logger.Warning("[SetParameterValues] 无法获取参数对象");
				}
				else
				{
					Logger.Warning("[SetParameterValues] convertedValue 为 null");
				}
				goto end_IL_00c7;
				IL_00e3:
				text2 = (string)obj3;
				if (parameterDefinitionType == null)
				{
					obj4 = null;
				}
				else
				{
					obj4 = parameterDefinitionType.ToLowerInvariant();
					if (obj4 != null)
					{
						goto IL_0101;
					}
				}
				obj4 = "";
				goto IL_0101;
				end_IL_00c7:;
			}
			catch (Exception ex3)
			{
				Logger.Error("[SetParameterValues] 转换或设置参数值失败: " + ex3.Message);
				return false;
			}
		}
		else
		{
			Logger.Warning("[SetParameterValues] 找不到参数 '" + string_0 + "'");
		}
		return false;
	}

	private List<int> method_3(object object_0)
	{
		List<int> list = new List<int>();
		try
		{
			Type type = object_0.GetType();
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
			{
				if (type.GetProperty("Items")?.GetValue(object_0) is IEnumerable enumerable)
				{
					foreach (object item2 in enumerable)
					{
						if (item2 != null)
						{
							PropertyInfo property = item2.GetType().GetProperty("Id");
							if (property != null && property.GetValue(item2) is int item)
							{
								list.Add(item);
							}
						}
					}
				}
			}
			else
			{
				PropertyInfo property2 = type.GetProperty("elements");
				if (property2 != null)
				{
					object value = property2.GetValue(object_0);
					if (value != null)
					{
						return method_3(value);
					}
				}
				PropertyInfo property3 = type.GetProperty("elementIds");
				if (property3 != null)
				{
					object value2 = property3.GetValue(object_0);
					if (value2 is int[] collection)
					{
						list.AddRange(collection);
					}
					else if (value2 is List<int> collection2)
					{
						list.AddRange(collection2);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[ExtractElementIdsFromCachedData] 提取元素ID失败: " + ex.Message);
		}
		return list;
	}
}
