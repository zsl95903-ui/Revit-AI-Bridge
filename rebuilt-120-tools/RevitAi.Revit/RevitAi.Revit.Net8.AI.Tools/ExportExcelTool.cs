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
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Services;
using Newtonsoft.Json;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("export_excel", Category = "数据导出", Description = "将数据导出为 Excel 文件。支持从缓存中导出之前查询的结果，或者直接导出提供的表格数据。", RequiresTransaction = false, RequiresModification = false)]
public sealed class ExportExcelTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class464 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ExportExcelTool exportExcelTool_0;

		private IExcelDataService iexcelDataService_0;

		private string string_0;

		private List<List<string>> list_0;

		private string string_1;

		private string string_2;

		private Result<string> result_0;

		private string string_3;

		private string string_4;

		private string string_5;

		private string string_6;

		private object object_0;

		private object object_1;

		private string string_7;

		private string string_8;

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
					Class464 stateMachine = this;
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
				iexcelDataService_0 = ((revitAdapter != null) ? revitAdapter.GetExcelDataService() : null);
				if (iexcelDataService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ExcelDataService");
				}
				else
				{
					string_0 = aitoolContext_0.GetParameter<string>("dataSource", (string)null);
					if (string.IsNullOrEmpty(string_0))
					{
						string_0 = "cache";
					}
					list_0 = new List<List<string>>();
					string_1 = "数据导出";
					string_2 = null;
					if (string_0 == "cache")
					{
						string_4 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
						if (string.IsNullOrEmpty(string_4))
						{
							result = AIToolResult.Fail("缓存 ID 不能为空（ dataSource 为 'cache' 时）");
						}
						else
						{
							string_5 = aitoolContext_0.GetParameter<string>("sheetName", (string)null);
							if (!string.IsNullOrEmpty(string_5))
							{
								string_1 = string_5;
							}
							string_6 = aitoolContext_0.GetParameter<string>("filePath", (string)null);
							if (!string.IsNullOrEmpty(string_6))
							{
								string_2 = string_6;
							}
							if (aitoolContext_0.DataCache == null)
							{
								result = AIToolResult.Fail("数据缓存服务不可用");
							}
							else
							{
								object_0 = aitoolContext_0.DataCache.Retrieve<object>(string_4);
								if (object_0 == null)
								{
									result = AIToolResult.Fail("缓存 '" + string_4 + "' 不存在或已过期");
								}
								else
								{
									list_0 = exportExcelTool_0.method_0(object_0);
									if (list_0.Count != 0)
									{
										string_4 = null;
										string_5 = null;
										string_6 = null;
										object_0 = null;
										goto IL_03b1;
									}
									result = AIToolResult.Fail("缓存中没有可导出的数据");
								}
							}
						}
					}
					else if (string_0 == "direct")
					{
						object_1 = aitoolContext_0.GetParameter<object>("data", (object)null);
						if (object_1 == null)
						{
							result = AIToolResult.Fail("数据不能为空（ dataSource 为 'direct' 时）");
						}
						else
						{
							string_7 = aitoolContext_0.GetParameter<string>("sheetName", (string)null);
							if (!string.IsNullOrEmpty(string_7))
							{
								string_1 = string_7;
							}
							string_8 = aitoolContext_0.GetParameter<string>("filePath", (string)null);
							if (!string.IsNullOrEmpty(string_8))
							{
								string_2 = string_8;
							}
							list_0 = exportExcelTool_0.method_0(object_1);
							if (list_0.Count != 0)
							{
								object_1 = null;
								string_7 = null;
								string_8 = null;
								goto IL_03b1;
							}
							result = AIToolResult.Fail("无法将提供的数据转换为 Excel 格式");
						}
					}
					else
					{
						result = AIToolResult.Fail("不支持的数据源类型: " + string_0 + "。支持的类型：'cache'、'direct'");
					}
				}
				goto end_IL_0067;
				IL_03b1:
				result_0 = iexcelDataService_0.ExportDataToExcel(string_2, string_1, list_0, true);
				if (!result_0.IsSuccess)
				{
					result = AIToolResult.Fail("导出 Excel 失败：" + result_0.Error);
				}
				else
				{
					string_3 = result_0.Value;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
					defaultInterpolatedStringHandler.AppendLiteral("成功导出 ");
					defaultInterpolatedStringHandler.AppendFormatted(list_0.Count - 1);
					defaultInterpolatedStringHandler.AppendLiteral(" 行数据到 Excel 文件");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class126<string, int, int, string>(string_3, list_0.Count - 1, list_0[0].Count, string_1));
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("导出 Excel 时发生异常：" + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "export_excel";

	public string Category => "数据导出";

	public string Description => "将数据导出为 Excel 文件";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"dataSource\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"数据源类型。可选值：'cache'（从缓存导出）或 'direct'（直接导出数据）。默认为 'cache'。\",\r\n                \"enum\": [\"cache\", \"direct\"]\r\n            },\r\n            \"cacheId\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"缓存 ID（当 dataSource 为 'cache' 时必需）。从之前查询返回的缓存 ID 中获取。\"\r\n            },\r\n            \"sheetName\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"工作表名称。默认为 '数据导出'。\",\r\n                \"default\": \"数据导出\"\r\n            },\r\n            \"filePath\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"保存文件路径（可选）。如果不提供，将弹出保存对话框让用户选择位置。\"\r\n            },\r\n            \"data\": {\r\n                \"type\": \"array\",\r\n                \"description\": \"要导出的数据（当 dataSource 为 'direct' 时必需）。二维数组格式，第一行为表头。例如：[[\\\"ID\\\", \\\"名称\\\", \\\"类别\\\"], [\\\"1\\\", \\\"元素A\\\", \\\"墙\\\"], [\\\"2\\\", \\\"元素B\\\", \\\"楼板\\\"]]。\",\r\n                \"items\": {\r\n                    \"type\": \"array\",\r\n                    \"items\": {\r\n                        \"type\": \"string\"\r\n                    }\r\n                }\r\n            }\r\n        },\r\n        \"required\": [\"dataSource\"]\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class464))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class464 stateMachine = new Class464();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.exportExcelTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private List<List<string>> method_0(object object_0)
	{
		List<List<string>> list = new List<List<string>>();
		try
		{
			if (object_0 is IEnumerable enumerable && !(object_0 is string))
			{
				List<object> list2 = new List<object>();
				foreach (object item4 in enumerable)
				{
					list2.Add(item4 ?? "");
				}
				if (list2.Count == 0)
				{
					return list;
				}
				object obj = list2[0];
				if (obj is IEnumerable && !(obj is string))
				{
					foreach (object item5 in list2)
					{
						List<string> list3 = new List<string>();
						if (item5 is IEnumerable enumerable2 && !(item5 is string))
						{
							foreach (object item6 in enumerable2)
							{
								list3.Add(method_1(item6));
							}
						}
						else
						{
							list3.Add(method_1(item5));
						}
						list.Add(list3);
					}
				}
				else if (obj != null && obj.GetType().IsClass && obj.GetType() != typeof(string))
				{
					PropertyInfo[] properties = obj.GetType().GetProperties();
					List<string> item = properties.Select((PropertyInfo propertyInfo_0) => propertyInfo_0.Name).ToList();
					list.Add(item);
					foreach (object item7 in list2)
					{
						if (item7 != null)
						{
							List<string> list4 = new List<string>();
							PropertyInfo[] array = properties;
							foreach (PropertyInfo propertyInfo in array)
							{
								object value = propertyInfo.GetValue(item7);
								list4.Add(method_1(value));
							}
							list.Add(list4);
						}
					}
				}
				else
				{
					foreach (object item8 in list2)
					{
						List<string> item2 = new List<string> { method_1(item8) };
						list.Add(item2);
					}
				}
			}
			else if (object_0 != null && object_0.GetType().IsClass && object_0.GetType() != typeof(string))
			{
				PropertyInfo[] properties2 = object_0.GetType().GetProperties();
				List<string> item3 = properties2.Select((PropertyInfo propertyInfo_0) => propertyInfo_0.Name).ToList();
				list.Add(item3);
				List<string> list5 = new List<string>();
				PropertyInfo[] array2 = properties2;
				foreach (PropertyInfo propertyInfo2 in array2)
				{
					object value2 = propertyInfo2.GetValue(object_0);
					list5.Add(method_1(value2));
				}
				list.Add(list5);
			}
		}
		catch (Exception)
		{
		}
		return list;
	}

	private string method_1(object? object_0)
	{
		if (object_0 == null)
		{
			return "";
		}
		if (object_0 is IEnumerable && !(object_0 is string))
		{
			try
			{
				return JsonConvert.SerializeObject(object_0);
			}
			catch
			{
				return "[...]";
			}
		}
		return object_0.ToString() ?? "";
	}
}
