using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.DB;
using ns0;
using ns1;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("read_schedule_data", Category = "视图查询", Description = "读取明细表的数据内容，包括表头和数据行，返回结构化表格数据并自动缓存供后续导出使用", RequiresTransaction = false, RequiresModification = false)]
public sealed class ReadScheduleDataTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class622 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ReadScheduleDataTool readScheduleDataTool_0;

		private string string_0;

		private string string_1;

		private int int_1;

		private int int_2;

		private bool bool_0;

		private ViewSchedule viewSchedule_0;

		private string[] string_2;

		private List<string[]> list_0;

		private int int_3;

		private int int_4;

		private int int_5;

		private bool bool_1;

		private Class289<string, string, string[], List<string[]>, int, int, int, bool, int> class289_0;

		private string string_3;

		private string string_4;

		private int int_6;

		private object object_0;

		private ViewSchedule viewSchedule_1;

		private FilteredElementCollector filteredElementCollector_0;

		private IEnumerator<Element> ienumerator_0;

		private ViewSchedule viewSchedule_2;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Expected O, but got Unknown
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Expected O, but got Unknown
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
					Class622 stateMachine = this;
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
			AIToolResult result;
			try
			{
				string_0 = aitoolContext_0.GetParameter<string>("scheduleId", (string)null);
				string_1 = aitoolContext_0.GetParameter<string>("scheduleName", (string)null);
				int_1 = aitoolContext_0.GetParameter<int>("startRow", 0);
				int_2 = aitoolContext_0.GetParameter<int>("maxRows", 1000);
				bool_0 = aitoolContext_0.GetParameter<bool>("includeHeader", true);
				if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					if (((revitAdapter != null) ? revitAdapter.ElementService : null) == null)
					{
						result = AIToolResult.Fail("无法获取 ElementService");
					}
					else
					{
						viewSchedule_0 = null;
						if (!string.IsNullOrEmpty(string_0))
						{
							if (int.TryParse(string_0, out int_6))
							{
								object_0 = aitoolContext_0.RevitAdapter.ElementService.GetElementById(aitoolContext_0.Document, int_6);
								object obj = object_0;
								viewSchedule_1 = (ViewSchedule)((obj is ViewSchedule) ? obj : null);
								if (viewSchedule_1 != null)
								{
									viewSchedule_0 = viewSchedule_1;
								}
								object_0 = null;
								viewSchedule_1 = null;
								goto IL_02d2;
							}
							result = AIToolResult.Fail("无效的元素 ID: " + string_0);
						}
						else
						{
							if (!string.IsNullOrEmpty(string_1))
							{
								object document = aitoolContext_0.Document;
								filteredElementCollector_0 = new FilteredElementCollector((Document)((document is Document) ? document : null));
								filteredElementCollector_0.OfClass(typeof(ViewSchedule));
								ienumerator_0 = filteredElementCollector_0.GetEnumerator();
								try
								{
									while (ienumerator_0.MoveNext())
									{
										viewSchedule_2 = (ViewSchedule)ienumerator_0.Current;
										if (!string.Equals(((Element)viewSchedule_2).Name, string_1, StringComparison.OrdinalIgnoreCase))
										{
											viewSchedule_2 = null;
											continue;
										}
										viewSchedule_0 = viewSchedule_2;
										break;
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
								filteredElementCollector_0 = null;
								goto IL_02d2;
							}
							result = AIToolResult.Fail("必须提供 scheduleId 或 scheduleName 参数");
						}
					}
				}
				goto end_IL_0067;
				IL_02d2:
				if (viewSchedule_0 == null)
				{
					result = AIToolResult.Fail("找不到明细表: " + (string_0 ?? string_1));
				}
				else
				{
					(string[], List<string[]>, int, int, int) tuple = readScheduleDataTool_0.method_0(viewSchedule_0, int_1, int_2, bool_0);
					string_2 = tuple.Item1;
					list_0 = tuple.Item2;
					int_3 = tuple.Item3;
					int_4 = tuple.Item4;
					int_5 = tuple.Item5;
					bool_1 = int_1 + int_5 < int_3;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 4);
					defaultInterpolatedStringHandler.AppendLiteral("[ReadScheduleDataTool] 成功读取明细表 '");
					defaultInterpolatedStringHandler.AppendFormatted(((Element)viewSchedule_0).Name);
					defaultInterpolatedStringHandler.AppendLiteral("'，返回 ");
					defaultInterpolatedStringHandler.AppendFormatted(int_5);
					defaultInterpolatedStringHandler.AppendLiteral(" 行（总 ");
					defaultInterpolatedStringHandler.AppendFormatted(int_3);
					defaultInterpolatedStringHandler.AppendLiteral(" 行，从第 ");
					defaultInterpolatedStringHandler.AppendFormatted(int_1);
					defaultInterpolatedStringHandler.AppendLiteral(" 行开始）");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					class289_0 = new Class289<string, string, string[], List<string[]>, int, int, int, bool, int>(((Element)viewSchedule_0).Id.smethod_0().ToString(), ((Element)viewSchedule_0).Name, string_2, list_0, int_3, int_5, int_1, bool_1, int_4);
					string_3 = null;
					if (aitoolContext_0.DataCache != null && aitoolContext_0.SessionId != null && !string.IsNullOrEmpty(aitoolContext_0.SessionId))
					{
						IAIToolDataCache dataCache = aitoolContext_0.DataCache;
						string sessionId = aitoolContext_0.SessionId;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(14, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("schedule_data_");
						defaultInterpolatedStringHandler2.AppendFormatted(((Element)viewSchedule_0).Id.smethod_0());
						string_3 = dataCache.Store<Class290<string, string[], List<string[]>, int, int, int, bool, int>>(sessionId, defaultInterpolatedStringHandler2.ToStringAndClear(), new Class290<string, string[], List<string[]>, int, int, int, bool, int>(((Element)viewSchedule_0).Name, string_2, list_0, int_3, int_5, int_1, bool_1, int_4));
						Logger.Info("[ReadScheduleDataTool] 数据已保存到缓存: " + string_3);
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(18, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("成功读取明细表 '");
					defaultInterpolatedStringHandler3.AppendFormatted(((Element)viewSchedule_0).Name);
					defaultInterpolatedStringHandler3.AppendLiteral("'，返回 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_5);
					defaultInterpolatedStringHandler3.AppendLiteral(" 行数据");
					string text = defaultInterpolatedStringHandler3.ToStringAndClear();
					string text2;
					if (!bool_1)
					{
						text2 = "（已读取全部数据）";
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(29, 2);
						defaultInterpolatedStringHandler4.AppendLiteral("（还有 ");
						defaultInterpolatedStringHandler4.AppendFormatted(int_3 - int_1 - int_5);
						defaultInterpolatedStringHandler4.AppendLiteral(" 行未读取，可使用 startRow=");
						defaultInterpolatedStringHandler4.AppendFormatted(int_1 + int_5);
						defaultInterpolatedStringHandler4.AppendLiteral(" 继续读取）");
						text2 = defaultInterpolatedStringHandler4.ToStringAndClear();
					}
					string_4 = text + text2;
					if (string_3 != null)
					{
						string_4 = string_4 + "\n\n💾 数据已缓存，缓存 ID: " + string_3;
						string_4 += "\n   此缓存 ID 可用于后续操作，避免重复读取明细表。";
					}
					result = AIToolResult.Ok(string_4, (object)new Class291<string, string, string[], List<string[]>, int, int, int, bool, int, string>(((Element)viewSchedule_0).Id.smethod_0().ToString(), ((Element)viewSchedule_0).Name, string_2, list_0, int_3, int_5, int_1, bool_1, int_4, string_3));
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[ReadScheduleDataTool] 读取明细表数据失败: " + exception_0.Message, exception_0);
				result = AIToolResult.Fail("读取明细表数据失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "read_schedule_data";

	public string Category => "视图查询";

	public string Description => "读取明细表的数据内容，包括表头和数据行";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"scheduleId\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"明细表的元素 ID（可选，优先使用）\"\r\n            },\r\n            \"scheduleName\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"明细表的名称（可选，如果不提供 scheduleId 则使用此参数）\"\r\n            },\r\n            \"startRow\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"从第几行开始读取（可选，默认 0，从第一行开始）\",\r\n                \"default\": 0,\r\n                \"minimum\": 0\r\n            },\r\n            \"maxRows\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"最多读取多少行（可选，默认 1000，最大 10000，避免返回过多数据）\",\r\n                \"default\": 1000,\r\n                \"minimum\": 1,\r\n                \"maximum\": 10000\r\n            },\r\n            \"includeHeader\": {\r\n                \"type\": \"boolean\",\r\n                \"description\": \"是否包含表头（可选，默认 true）\",\r\n                \"default\": true\r\n            }\r\n        }\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class622))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class622 stateMachine = new Class622();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.readScheduleDataTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private (string[] headers, List<string[]> rows, int totalRows, int totalColumns, int returnedRows) method_0(ViewSchedule viewSchedule_0, int int_0, int int_1, bool bool_0)
	{
		string[] array = Array.Empty<string>();
		List<string[]> list = new List<string[]>();
		int item = 0;
		int num = 0;
		int num2 = 0;
		try
		{
			TableData tableData = viewSchedule_0.GetTableData();
			if (bool_0)
			{
				try
				{
					TableSectionData sectionData = tableData.GetSectionData((SectionType)0);
					if (sectionData != null)
					{
						int numberOfColumns = sectionData.NumberOfColumns;
						num = numberOfColumns;
						array = new string[numberOfColumns];
						int numberOfRows = sectionData.NumberOfRows;
						int num3 = Math.Max(0, numberOfRows - 1);
						for (int i = 0; i < numberOfColumns; i++)
						{
							string cellText = ((TableView)viewSchedule_0).GetCellText((SectionType)0, num3, i);
							array[i] = cellText;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[ReadScheduleDataTool] 读取表头: ");
						defaultInterpolatedStringHandler.AppendFormatted(numberOfColumns);
						defaultInterpolatedStringHandler.AppendLiteral(" 列");
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("[ReadScheduleDataTool] 读取表头失败: " + ex.Message);
				}
			}
			try
			{
				TableSectionData sectionData2 = tableData.GetSectionData((SectionType)1);
				if (sectionData2 != null)
				{
					int numberOfRows2 = sectionData2.NumberOfRows;
					int numberOfColumns2 = sectionData2.NumberOfColumns;
					item = numberOfRows2;
					if (numberOfColumns2 > num)
					{
						num = numberOfColumns2;
					}
					int num4 = Math.Min(numberOfRows2 - int_0, int_1);
					for (int j = int_0; j < int_0 + num4; j++)
					{
						string[] array2 = new string[numberOfColumns2];
						for (int k = 0; k < numberOfColumns2; k++)
						{
							try
							{
								string cellText2 = ((TableView)viewSchedule_0).GetCellText((SectionType)1, j, k);
								array2[k] = cellText2;
							}
							catch (Exception ex2)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(37, 3);
								defaultInterpolatedStringHandler2.AppendLiteral("[ReadScheduleDataTool] 读取单元格 [");
								defaultInterpolatedStringHandler2.AppendFormatted(j);
								defaultInterpolatedStringHandler2.AppendLiteral(",");
								defaultInterpolatedStringHandler2.AppendFormatted(k);
								defaultInterpolatedStringHandler2.AppendLiteral("] 失败: ");
								defaultInterpolatedStringHandler2.AppendFormatted(ex2.Message);
								Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
								array2[k] = "";
							}
						}
						list.Add(array2);
						num2++;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(51, 4);
					defaultInterpolatedStringHandler3.AppendLiteral("[ReadScheduleDataTool] 读取数据体: 从第 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_0);
					defaultInterpolatedStringHandler3.AppendLiteral(" 行开始，读取 ");
					defaultInterpolatedStringHandler3.AppendFormatted(num2);
					defaultInterpolatedStringHandler3.AppendLiteral(" 行，共 ");
					defaultInterpolatedStringHandler3.AppendFormatted(numberOfRows2);
					defaultInterpolatedStringHandler3.AppendLiteral(" 行，");
					defaultInterpolatedStringHandler3.AppendFormatted(numberOfColumns2);
					defaultInterpolatedStringHandler3.AppendLiteral(" 列");
					Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
				}
			}
			catch (Exception ex3)
			{
				Logger.Warning("[ReadScheduleDataTool] 读取数据体失败: " + ex3.Message);
			}
		}
		catch (Exception ex4)
		{
			Logger.Error("[ReadScheduleDataTool] 读取明细表数据失败: " + ex4.Message, ex4);
		}
		return (headers: array, rows: list, totalRows: item, totalColumns: num, returnedRows: num2);
	}
}
