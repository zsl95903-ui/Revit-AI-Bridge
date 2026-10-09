using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.DB;
using Microsoft.Win32;
using ns0;
using ns1;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("export_schedule_to_csv", Category = "数据导出", Description = "将明细表数据导出为 CSV 文件（UTF-8 with BOM 编码，Excel 可正确打开并显示中文，自动处理包含逗号、引号、换行符的单元格）", RequiresTransaction = false, RequiresModification = false)]
public sealed class ExportScheduleToCsvTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class465
	{
		public char[] char_0;

		internal bool method_0(char char_1)
		{
			return !char_0.Contains(char_1);
		}
	}

	[CompilerGenerated]
	public sealed class Class466 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ExportScheduleToCsvTool exportScheduleToCsvTool_0;

		private string string_0;

		private string string_1;

		private string string_2;

		private bool bool_0;

		private ViewSchedule viewSchedule_0;

		private string[] string_3;

		private List<string[]> list_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private object object_0;

		private ViewSchedule viewSchedule_1;

		private FilteredElementCollector filteredElementCollector_0;

		private IEnumerator<Element> ienumerator_0;

		private ViewSchedule viewSchedule_2;

		private SaveFileDialog saveFileDialog_0;

		private bool? nullable_0;

		private string string_4;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Expected O, but got Unknown
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Expected O, but got Unknown
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
					Class466 stateMachine = this;
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
				string_2 = aitoolContext_0.GetParameter<string>("filePath", (string)null);
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
							if (int.TryParse(string_0, out int_3))
							{
								object_0 = aitoolContext_0.RevitAdapter.ElementService.GetElementById(aitoolContext_0.Document, int_3);
								object obj = object_0;
								viewSchedule_1 = (ViewSchedule)((obj is ViewSchedule) ? obj : null);
								if (viewSchedule_1 != null)
								{
									viewSchedule_0 = viewSchedule_1;
								}
								object_0 = null;
								viewSchedule_1 = null;
								goto IL_02b2;
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
								goto IL_02b2;
							}
							result = AIToolResult.Fail("必须提供 scheduleId 或 scheduleName 参数");
						}
					}
				}
				goto end_IL_0067;
				IL_047d:
				exportScheduleToCsvTool_0.method_1(string_2, string_3, list_0);
				Logger.Info("[ExportScheduleToCsvTool] 成功导出明细表 '" + ((Element)viewSchedule_0).Name + "' 到: " + string_2);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 3);
				defaultInterpolatedStringHandler.AppendLiteral("成功导出明细表 '");
				defaultInterpolatedStringHandler.AppendFormatted(((Element)viewSchedule_0).Name);
				defaultInterpolatedStringHandler.AppendLiteral("' 到 CSV 文件，共 ");
				defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 行 ");
				defaultInterpolatedStringHandler.AppendFormatted(int_2);
				defaultInterpolatedStringHandler.AppendLiteral(" 列");
				result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class127<string, string, string, int, int, int, bool>(((Element)viewSchedule_0).Id.smethod_0().ToString(), ((Element)viewSchedule_0).Name, string_2, list_0.Count, int_2, int_1, File.Exists(string_2)));
				goto end_IL_0067;
				IL_02b2:
				if (viewSchedule_0 == null)
				{
					result = AIToolResult.Fail("找不到明细表: " + (string_0 ?? string_1));
				}
				else
				{
					(string_3, list_0, int_1, int_2) = exportScheduleToCsvTool_0.method_0(viewSchedule_0, bool_0);
					if (!string.IsNullOrEmpty(string_2))
					{
						string_4 = Path.GetDirectoryName(string_2);
						if (!string.IsNullOrEmpty(string_4) && !Directory.Exists(string_4))
						{
							Directory.CreateDirectory(string_4);
						}
						string_4 = null;
						goto IL_047d;
					}
					SaveFileDialog obj2 = new SaveFileDialog
					{
						Filter = "CSV 文件|*.csv|所有文件|*.*",
						DefaultExt = "csv"
					};
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(5, 2);
					defaultInterpolatedStringHandler2.AppendFormatted(exportScheduleToCsvTool_0.method_3(((Element)viewSchedule_0).Name));
					defaultInterpolatedStringHandler2.AppendLiteral("_");
					defaultInterpolatedStringHandler2.AppendFormatted(DateTime.Now, "yyyyMMdd_HHmmss");
					defaultInterpolatedStringHandler2.AppendLiteral(".csv");
					obj2.FileName = defaultInterpolatedStringHandler2.ToStringAndClear();
					saveFileDialog_0 = obj2;
					nullable_0 = saveFileDialog_0.ShowDialog();
					if (nullable_0 == true)
					{
						string_2 = saveFileDialog_0.FileName;
						saveFileDialog_0 = null;
						goto IL_047d;
					}
					Logger.Info("[ExportScheduleToCsvTool] 用户取消了保存操作");
					result = AIToolResult.Fail("用户取消了保存操作");
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[ExportScheduleToCsvTool] 导出明细表到 CSV 失败: " + exception_0.Message, exception_0);
				result = AIToolResult.Fail("导出明细表到 CSV 失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "export_schedule_to_csv";

	public string Category => "数据导出";

	public string Description => "将明细表数据导出为 CSV 文件";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"scheduleId\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"明细表的元素 ID（可选，优先使用）\"\r\n            },\r\n            \"scheduleName\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"明细表的名称（可选，如果不提供 scheduleId 则使用此参数）\"\r\n            },\r\n            \"filePath\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"保存文件路径（可选，如果不提供将弹出保存对话框让用户选择位置）\"\r\n            },\r\n            \"includeHeader\": {\r\n                \"type\": \"boolean\",\r\n                \"description\": \"是否包含表头（可选，默认 true）\",\r\n                \"default\": true\r\n            }\r\n        }\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class466))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class466 stateMachine = new Class466();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.exportScheduleToCsvTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private (string[] headers, List<string[]> rows, int totalRows, int totalColumns) method_0(ViewSchedule viewSchedule_0, bool bool_0)
	{
		string[] array = Array.Empty<string>();
		List<string[]> list = new List<string[]>();
		int item = 0;
		int num = 0;
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
						int num2 = Math.Max(0, numberOfRows - 1);
						for (int i = 0; i < numberOfColumns; i++)
						{
							string cellText = ((TableView)viewSchedule_0).GetCellText((SectionType)0, num2, i);
							array[i] = cellText;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[ExportScheduleToCsvTool] 读取表头: ");
						defaultInterpolatedStringHandler.AppendFormatted(numberOfColumns);
						defaultInterpolatedStringHandler.AppendLiteral(" 列");
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("[ExportScheduleToCsvTool] 读取表头失败: " + ex.Message);
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
					for (int j = 0; j < numberOfRows2; j++)
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
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 3);
								defaultInterpolatedStringHandler2.AppendLiteral("[ExportScheduleToCsvTool] 读取单元格 [");
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
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(47, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("[ExportScheduleToCsvTool] 读取数据体: ");
					defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
					defaultInterpolatedStringHandler3.AppendLiteral(" 行，");
					defaultInterpolatedStringHandler3.AppendFormatted(numberOfColumns2);
					defaultInterpolatedStringHandler3.AppendLiteral(" 列（已读取全部数据）");
					Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
				}
			}
			catch (Exception ex3)
			{
				Logger.Warning("[ExportScheduleToCsvTool] 读取数据体失败: " + ex3.Message);
			}
		}
		catch (Exception ex4)
		{
			Logger.Error("[ExportScheduleToCsvTool] 读取明细表数据失败: " + ex4.Message, ex4);
		}
		return (headers: array, rows: list, totalRows: item, totalColumns: num);
	}

	private void method_1(string string_0, string[] string_1, List<string[]> list_0)
	{
		try
		{
			using StreamWriter streamWriter = new StreamWriter(string_0, append: false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
			if (string_1.Length != 0)
			{
				string value = string.Join(",", string_1.Select(method_2));
				streamWriter.WriteLine(value);
			}
			foreach (string[] item in list_0)
			{
				string value2 = string.Join(",", item.Select(method_2));
				streamWriter.WriteLine(value2);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[ExportScheduleToCsvTool] CSV 文件已写入: ");
			defaultInterpolatedStringHandler.AppendFormatted(string_0);
			defaultInterpolatedStringHandler.AppendLiteral("，共 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 行数据");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		catch (Exception ex)
		{
			Logger.Error("[ExportScheduleToCsvTool] 写入 CSV 文件失败: " + ex.Message, ex);
			throw;
		}
	}

	private string method_2(string string_0)
	{
		if (string.IsNullOrEmpty(string_0))
		{
			return "";
		}
		if (string_0.Contains(",") || string_0.Contains("\"") || string_0.Contains("\n") || string_0.Contains("\r"))
		{
			string text = string_0.Replace("\"", "\"\"");
			return "\"" + text + "\"";
		}
		return string_0;
	}

	private string method_3(string string_0)
	{
		char[] char_0 = Path.GetInvalidFileNameChars();
		string text = new string(string_0.Where((char value) => !char_0.Contains(value)).ToArray());
		if (string.IsNullOrWhiteSpace(text))
		{
			text = "Schedule";
		}
		return text;
	}
}
