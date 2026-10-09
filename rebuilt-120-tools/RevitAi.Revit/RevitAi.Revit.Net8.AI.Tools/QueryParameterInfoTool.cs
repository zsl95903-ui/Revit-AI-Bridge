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

[AITool("query_parameter_info", Category = "参数操作", Description = "查询元素参数信息，包括获取参数值、参数定义、参数类型和只读状态。此工具不修改文档，无需事务。", RequiresTransaction = false, RequiresModification = false)]
public sealed class QueryParameterInfoTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class617
	{
		public string string_0;

		internal bool method_0(object object_0)
		{
			return (object_0.GetType().GetProperty("name")?.GetValue(object_0)?.ToString())?.Equals(string_0, StringComparison.OrdinalIgnoreCase) ?? false;
		}
	}

	[CompilerGenerated]
	public sealed class Class618 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public QueryParameterInfoTool queryParameterInfoTool_0;

		private IElementService ielementService_0;

		private IParameterService iparameterService_0;

		private string string_0;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private AIToolResult aitoolResult_2;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if ((uint)(num - 1) <= 2u)
				{
					goto IL_006e;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class618 stateMachine = this;
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
				TaskAwaiter<AIToolResult> awaiter4;
				TaskAwaiter<AIToolResult> awaiter3;
				TaskAwaiter<AIToolResult> awaiter2;
				switch (num)
				{
				default:
				{
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
					IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
					iparameterService_0 = ((revitAdapter2 != null) ? revitAdapter2.ParameterService : null);
					if (ielementService_0 == null || iparameterService_0 == null)
					{
						result = AIToolResult.Fail("无法获取服务");
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
							if (text2 == "get")
							{
								awaiter4 = queryParameterInfoTool_0.method_0(aitoolContext_0, ielementService_0, iparameterService_0).GetAwaiter();
								if (!awaiter4.IsCompleted)
								{
									num = 1;
									int_0 = 1;
									taskAwaiter_1 = awaiter4;
									Class618 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
									return;
								}
								goto IL_02e6;
							}
							if (text2 == "schema")
							{
								awaiter3 = queryParameterInfoTool_0.method_1(aitoolContext_0, ielementService_0, iparameterService_0).GetAwaiter();
								if (!awaiter3.IsCompleted)
								{
									num = 2;
									int_0 = 2;
									taskAwaiter_1 = awaiter3;
									Class618 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
									return;
								}
								goto IL_031d;
							}
							if (text2 == "get_type")
							{
								awaiter2 = queryParameterInfoTool_0.method_2(aitoolContext_0, ielementService_0, iparameterService_0).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = 3;
									int_0 = 3;
									taskAwaiter_1 = awaiter2;
									Class618 stateMachine = this;
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
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_02e6;
				case 2:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_031d;
				case 3:
					{
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_031d:
					aitoolResult_1 = awaiter3.GetResult();
					result = aitoolResult_1;
					goto end_IL_006e;
					IL_02e6:
					aitoolResult_0 = awaiter4.GetResult();
					result = aitoolResult_0;
					goto end_IL_006e;
				}
				aitoolResult_2 = awaiter2.GetResult();
				result = aitoolResult_2;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("查询参数信息失败: " + exception_0.Message);
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
	public sealed class Class619 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public IParameterService iparameterService_0;

		public QueryParameterInfoTool queryParameterInfoTool_0;

		private int int_1;

		private object object_0;

		private string string_0;

		private string string_1;

		private List<object> list_0;

		private bool bool_0;

		private IEnumerable<object> ienumerable_0;

		private object object_1;

		private int? nullable_0;

		private string string_2;

		private int int_2;

		private int int_3;

		private IEnumerator<object> ienumerator_0;

		private object object_2;

		private string string_3;

		private string string_4;

		private bool bool_1;

		private IEnumerable<object> ienumerable_1;

		private IEnumerator<object> ienumerator_1;

		private object object_3;

		private Class617 class617_0;

		private string string_5;

		private bool bool_2;

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
					Class619 stateMachine = this;
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
			int_1 = aitoolContext_0.GetParameter<int>("elementId", 0);
			object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
			AIToolResult result;
			if (object_0 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendLiteral("元素 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(int_1);
				defaultInterpolatedStringHandler.AppendLiteral(" 不存在");
				result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				string_0 = ielementService_0.GetElementTypeName(object_0) ?? "未知";
				string_1 = ielementService_0.GetElementCategory(object_0) ?? "未分类";
				list_0 = new List<object>();
				bool_0 = string_1 == "墙" || string_1 == "楼板";
				ienumerable_0 = iparameterService_0.GetParameters(object_0);
				ienumerator_0 = ienumerable_0.GetEnumerator();
				try
				{
					while (ienumerator_0.MoveNext())
					{
						object_2 = ienumerator_0.Current;
						string_3 = iparameterService_0.GetParameterName(object_2);
						if (!string.IsNullOrEmpty(string_3))
						{
							string_4 = iparameterService_0.GetParameterType(object_2);
							bool_1 = iparameterService_0.IsParameterReadOnly(object_2);
							list_0.Add(new Class286<string, string, string, bool>(string_3, string_4, "instance", bool_1));
							string_3 = null;
							string_4 = null;
							object_2 = null;
						}
					}
				}
				finally
				{
					if (num < 0 && ienumerator_0 != null)
					{
						ienumerator_0.Dispose();
					}
				}
				ienumerator_0 = null;
				object_1 = null;
				nullable_0 = ielementService_0.GetElementTypeId(object_0);
				if (nullable_0.HasValue)
				{
					object_1 = ielementService_0.GetElementById(aitoolContext_0.Document, nullable_0.Value);
				}
				if (object_1 != null)
				{
					ienumerable_1 = iparameterService_0.GetParameters(object_1);
					ienumerator_1 = ienumerable_1.GetEnumerator();
					try
					{
						while (ienumerator_1.MoveNext())
						{
							object_3 = ienumerator_1.Current;
							class617_0 = new Class617();
							class617_0.string_0 = iparameterService_0.GetParameterName(object_3);
							if (!string.IsNullOrEmpty(class617_0.string_0) && !list_0.Any((object object_0) => (object_0.GetType().GetProperty("name")?.GetValue(object_0)?.ToString())?.Equals(class617_0.string_0, StringComparison.OrdinalIgnoreCase) ?? false))
							{
								string_5 = iparameterService_0.GetParameterType(object_3);
								bool_2 = iparameterService_0.IsParameterReadOnly(object_3);
								list_0.Add(new Class286<string, string, string, bool>(class617_0.string_0, string_5, "type", bool_2));
								class617_0 = null;
								string_5 = null;
								object_3 = null;
							}
						}
					}
					finally
					{
						if (num < 0 && ienumerator_1 != null)
						{
							ienumerator_1.Dispose();
						}
					}
					ienumerator_1 = null;
					ienumerable_1 = null;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(16, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("📋 获取了 ");
				defaultInterpolatedStringHandler2.AppendFormatted(string_0);
				defaultInterpolatedStringHandler2.AppendLiteral("（");
				defaultInterpolatedStringHandler2.AppendFormatted(string_1);
				defaultInterpolatedStringHandler2.AppendLiteral("）的参数定义\n\n");
				string_2 = defaultInterpolatedStringHandler2.ToStringAndClear();
				string text = string_2;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(8, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("总参数数量: ");
				defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler3.AppendLiteral("\n");
				string_2 = text + defaultInterpolatedStringHandler3.ToStringAndClear();
				int_2 = list_0.Count((object object_0) => "instance".Equals(object_0.GetType().GetProperty("scope")?.GetValue(object_0)?.ToString()));
				int_3 = list_0.Count((object object_0) => "type".Equals(object_0.GetType().GetProperty("scope")?.GetValue(object_0)?.ToString()));
				string text2 = string_2;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(9, 1);
				defaultInterpolatedStringHandler4.AppendLiteral("- 实例参数: ");
				defaultInterpolatedStringHandler4.AppendFormatted(int_2);
				defaultInterpolatedStringHandler4.AppendLiteral("\n");
				string_2 = text2 + defaultInterpolatedStringHandler4.ToStringAndClear();
				string text3 = string_2;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler5.AppendLiteral("- 类型参数: ");
				defaultInterpolatedStringHandler5.AppendFormatted(int_3);
				defaultInterpolatedStringHandler5.AppendLiteral("\n\n");
				string_2 = text3 + defaultInterpolatedStringHandler5.ToStringAndClear();
				if (bool_0)
				{
					string_2 += "💡 提示：墙和楼板实例的参数通常很少，大部分参数（如材质、厚度、功能等）都在类型参数中。\n\n";
				}
				string_2 += "✅ 使用这些参数名称调用 query_parameter_info 或 set_parameter_values";
				result = AIToolResult.Ok(string_2, (object)new Class287<string, string, int, int, int, List<object>>(string_1, string_0, list_0.Count, int_2, int_3, list_0));
			}
			int_0 = -2;
			object_0 = null;
			string_0 = null;
			string_1 = null;
			list_0 = null;
			ienumerable_0 = null;
			object_1 = null;
			string_2 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class620 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public IParameterService iparameterService_0;

		public QueryParameterInfoTool queryParameterInfoTool_0;

		private int int_1;

		private string string_0;

		private object object_0;

		private string string_1;

		private string string_2;

		private string string_3;

		private object object_1;

		private string string_4;

		private bool bool_0;

		private string string_5;

		private int? nullable_0;

		private object object_2;

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
					Class620 stateMachine = this;
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
			int_1 = aitoolContext_0.GetParameter<int>("elementId", 0);
			string_0 = aitoolContext_0.GetParameter<string>("parameterName", (string)null);
			AIToolResult result;
			if (string.IsNullOrEmpty(string_0))
			{
				result = AIToolResult.Fail("parameterName 参数不能为空");
			}
			else
			{
				object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
				if (object_0 == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
					defaultInterpolatedStringHandler.AppendLiteral("元素 ID ");
					defaultInterpolatedStringHandler.AppendFormatted(int_1);
					defaultInterpolatedStringHandler.AppendLiteral(" 不存在");
					result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					string_1 = ielementService_0.GetElementCategory(object_0) ?? "未分类";
					string_2 = ielementService_0.GetElementTypeName(object_0) ?? "未知";
					string text = ielementService_0.GetElementName(object_0);
					if (text == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(8, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("Element_");
						defaultInterpolatedStringHandler2.AppendFormatted(int_1);
						text = defaultInterpolatedStringHandler2.ToStringAndClear();
					}
					string_3 = text;
					object_1 = iparameterService_0.GetParameter(object_0, string_0);
					if (object_1 == null)
					{
						nullable_0 = ielementService_0.GetElementTypeId(object_0);
						if (nullable_0.HasValue)
						{
							object_2 = ielementService_0.GetElementById(aitoolContext_0.Document, nullable_0.Value);
							if (object_2 != null)
							{
								object_1 = iparameterService_0.GetParameter(object_2, string_0);
							}
							object_2 = null;
						}
					}
					if (object_1 == null)
					{
						result = AIToolResult.Fail("参数 '" + string_0 + "' 不存在");
					}
					else
					{
						string_4 = iparameterService_0.GetParameterType(object_1);
						bool_0 = iparameterService_0.IsParameterReadOnly(object_1);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(10, 2);
						defaultInterpolatedStringHandler3.AppendLiteral("📋 元素: ");
						defaultInterpolatedStringHandler3.AppendFormatted(string_3);
						defaultInterpolatedStringHandler3.AppendLiteral("（");
						defaultInterpolatedStringHandler3.AppendFormatted(string_2);
						defaultInterpolatedStringHandler3.AppendLiteral("）\n");
						string_5 = defaultInterpolatedStringHandler3.ToStringAndClear();
						string_5 = string_5 + "📝 参数: " + string_0 + "\n\n";
						string_5 = string_5 + "🔧 类型: " + string_4 + "\n";
						string_5 = string_5 + "🔒 只读: " + (bool_0 ? "是" : "否") + "\n\n";
						if (bool_0)
						{
							string_5 += "⚠️ 该参数为只读，无法修改\n\n";
						}
						string_5 += "💡 提示：可以使用 query_parameter_info 的 get 操作查看参数值";
						result = AIToolResult.Ok(string_5, (object)new Class288<int, string, string, string, string, string, bool>(int_1, string_3, string_1, string_2, string_0, string_4, bool_0));
					}
				}
			}
			int_0 = -2;
			string_0 = null;
			object_0 = null;
			string_1 = null;
			string_2 = null;
			string_3 = null;
			object_1 = null;
			string_4 = null;
			string_5 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class621 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public IParameterService iparameterService_0;

		public QueryParameterInfoTool queryParameterInfoTool_0;

		private List<int> list_0;

		private List<string> list_1;

		private List<object> list_2;

		private int int_1;

		private int int_2;

		private string string_0;

		private int int_3;

		private object object_0;

		private int[] int_4;

		private List<int> list_3;

		private List<object> list_4;

		private List<object>.Enumerator enumerator_0;

		private object object_1;

		private int int_5;

		private string string_1;

		private object object_2;

		private List<int> list_5;

		private object object_3;

		private string[] string_2;

		private List<string> list_6;

		private List<object> list_7;

		private List<int>.Enumerator enumerator_1;

		private int int_6;

		private object object_4;

		private string string_3;

		private string string_4;

		private string string_5;

		private List<object> list_8;

		private int int_7;

		private IEnumerable<object> ienumerable_0;

		private IEnumerator<object> ienumerator_0;

		private object object_5;

		private string string_6;

		private string string_7;

		private string string_8;

		private bool bool_0;

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
					Class621 stateMachine = this;
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
				object_0 = aitoolContext_0.GetParameter<object>("elementIds", (object)null);
				int_4 = object_0 as int[];
				if (int_4 != null)
				{
					list_0.AddRange(int_4);
				}
				else
				{
					list_3 = object_0 as List<int>;
					if (list_3 != null)
					{
						list_0.AddRange(list_3);
					}
					else
					{
						list_4 = object_0 as List<object>;
						if (list_4 != null)
						{
							enumerator_0 = list_4.GetEnumerator();
							try
							{
								while (enumerator_0.MoveNext())
								{
									object_1 = enumerator_0.Current;
									if (object_1 != null && int.TryParse(object_1.ToString(), out int_5))
									{
										list_0.Add(int_5);
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
						list_4 = null;
					}
					list_3 = null;
				}
				object_0 = null;
				int_4 = null;
			}
			if (aitoolContext_0.HasParameter("cacheId"))
			{
				string_1 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
				if (string_1 != null && string_1.Length > 0)
				{
					object_2 = aitoolContext_0.DataCache.Retrieve<object>(string_1);
					if (object_2 != null)
					{
						list_5 = queryParameterInfoTool_0.method_3(object_2);
						if (list_5 != null && list_5.Count > 0)
						{
							list_0.AddRange(list_5);
						}
						list_5 = null;
					}
					object_2 = null;
				}
				string_1 = null;
			}
			AIToolResult result;
			if (list_0.Count == 0)
			{
				result = AIToolResult.Fail("未提供有效的元素 ID（请使用 elementId、elementIds 或 cacheId 参数）");
			}
			else
			{
				list_1 = null;
				if (aitoolContext_0.HasParameter("parameterNames"))
				{
					object_3 = aitoolContext_0.GetParameter<object>("parameterNames", (object)null);
					string_2 = object_3 as string[];
					if (string_2 != null)
					{
						list_1 = string_2.ToList();
					}
					else
					{
						list_6 = object_3 as List<string>;
						if (list_6 != null)
						{
							list_1 = list_6;
						}
						else
						{
							list_7 = object_3 as List<object>;
							if (list_7 != null)
							{
								list_1 = (from object_0 in list_7
									where object_0 != null
									select object_0.ToString()).ToList();
							}
							list_7 = null;
						}
						list_6 = null;
					}
					object_3 = null;
					string_2 = null;
				}
				list_2 = new List<object>();
				int_1 = 0;
				int_2 = 0;
				enumerator_1 = list_0.GetEnumerator();
				try
				{
					while (enumerator_1.MoveNext())
					{
						int_6 = enumerator_1.Current;
						object_4 = ielementService_0.GetElementById(aitoolContext_0.Document, int_6);
						if (object_4 == null)
						{
							int_1++;
							continue;
						}
						int_2++;
						string_3 = ielementService_0.GetElementCategory(object_4) ?? "未分类";
						string_4 = ielementService_0.GetElementTypeName(object_4) ?? "未知";
						string text = ielementService_0.GetElementName(object_4);
						if (text == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
							defaultInterpolatedStringHandler.AppendLiteral("Element_");
							defaultInterpolatedStringHandler.AppendFormatted(int_6);
							text = defaultInterpolatedStringHandler.ToStringAndClear();
						}
						string_5 = text;
						list_8 = new List<object>();
						int_7 = 0;
						ienumerable_0 = iparameterService_0.GetParameters(object_4);
						ienumerator_0 = ienumerable_0.GetEnumerator();
						try
						{
							while (ienumerator_0.MoveNext())
							{
								object_5 = ienumerator_0.Current;
								string_6 = iparameterService_0.GetParameterName(object_5);
								if (!string.IsNullOrEmpty(string_6) && (list_1 == null || list_1.Contains<string>(string_6, StringComparer.OrdinalIgnoreCase)))
								{
									string_7 = iparameterService_0.GetParameterValueAsString(object_5);
									string_8 = iparameterService_0.GetParameterType(object_5);
									bool_0 = iparameterService_0.IsParameterReadOnly(object_5);
									list_8.Add(new Class283<string, string, string, bool>(string_6, string_7, string_8, bool_0));
									int_7++;
									string_6 = null;
									string_7 = null;
									string_8 = null;
									object_5 = null;
								}
							}
						}
						finally
						{
							if (num < 0 && ienumerator_0 != null)
							{
								ienumerator_0.Dispose();
							}
						}
						ienumerator_0 = null;
						list_2.Add(new Class284<int, string, string, string, int, List<object>>(int_6, string_5, string_3, string_4, int_7, list_8));
						object_4 = null;
						string_3 = null;
						string_4 = null;
						string_5 = null;
						list_8 = null;
						ienumerable_0 = null;
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
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(16, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("📊 查询了 ");
				defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个元素的参数值\n");
				string_0 = defaultInterpolatedStringHandler2.ToStringAndClear();
				string text2 = string_0;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(14, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("✅ 成功获取 ");
				defaultInterpolatedStringHandler3.AppendFormatted(int_2);
				defaultInterpolatedStringHandler3.AppendLiteral(" 个元素的参数");
				string_0 = text2 + defaultInterpolatedStringHandler3.ToStringAndClear();
				if (int_1 > 0)
				{
					string text3 = string_0;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(11, 1);
					defaultInterpolatedStringHandler4.AppendLiteral("，⚠️ ");
					defaultInterpolatedStringHandler4.AppendFormatted(int_1);
					defaultInterpolatedStringHandler4.AppendLiteral(" 个元素不存在");
					string_0 = text3 + defaultInterpolatedStringHandler4.ToStringAndClear();
				}
				result = AIToolResult.Ok(string_0, (object)new Class285<int, int, int, List<object>>(list_0.Count, int_2, int_1, list_2));
			}
			int_0 = -2;
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

	public string Name => "query_parameter_info";

	public string Category => "参数操作";

	public string Description => "查询元素参数信息（只读操作）";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operation\": {\n                \"type\": \"string\",\n                \"description\": \"操作类型：get（获取参数值）、schema（获取参数定义）、get_type（获取参数类型和只读状态）\",\n                \"enum\": [\"get\", \"schema\", \"get_type\"]\n            },\n            \"elementId\": {\n                \"type\": \"integer\",\n                \"description\": \"单个元素 ID（可选）。如果要操作单个元素，使用此参数。\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"元素 ID 数组（可选）。如果要批量操作多个元素，使用此参数，例如：[12345, 12346, 12347]\"\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（可选）。从上一个查询工具（如 element_query）的返回结果中获取 cache_id 字段。使用缓存可以批量操作之前查询到的所有元素。注意：只需提供单个 cache_id 字符串，不需要数组。\"\n            },\n            \"parameterName\": {\n                \"type\": \"string\",\n                \"description\": \"参数名称（get_type 操作时必需）。例如：材质、宽度、偏移量等\"\n            },\n            \"parameterNames\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"string\" },\n                \"description\": \"参数名称列表（get 操作时可选）。指定要获取的参数名称，例如：材质、偏移量。如果不提供，则获取所有可读取的参数值\"\n            }\n        },\n        \"required\": [\"operation\"]\n    }";

	[AsyncStateMachine(typeof(Class618))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class618 stateMachine = new Class618();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.queryParameterInfoTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class621))]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, IElementService ielementService_0, IParameterService iparameterService_0)
	{
		Class621 stateMachine = new Class621();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.queryParameterInfoTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.iparameterService_0 = iparameterService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class619))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, IElementService ielementService_0, IParameterService iparameterService_0)
	{
		Class619 stateMachine = new Class619();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.queryParameterInfoTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.iparameterService_0 = iparameterService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class620))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_2(AIToolContext aitoolContext_0, IElementService ielementService_0, IParameterService iparameterService_0)
	{
		Class620 stateMachine = new Class620();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.queryParameterInfoTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.iparameterService_0 = iparameterService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
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
