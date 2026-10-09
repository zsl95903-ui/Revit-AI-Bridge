using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("edit_schedule", Category = "视图高级操作", Description = "编辑已存在明细表的字段、排序、过滤和分组。\n\n参数说明：\n- addFieldParameterIds: 要添加的字段 parameterId 列表（从 get_schedule_fields 获取的 parameterId 字段）\n- removeFieldParameterIds: 要删除的字段 parameterId 列表\n- replaceFields: 如果为 true，将用 addFieldParameterIds 替换所有现有字段\n- sortByParameterId/sortOrder: 设置排序（会清除现有排序）\n- filterFieldParameterId/filterType/filterValue: 设置过滤（会清除现有过滤）\n- groupByParameterId/showGroupHeader: 设置分组（会清除现有分组）\n\n注意：排序、过滤、分组设置后不能直接清除，需要重新创建明细表才能完全移除。", RequiresTransaction = true, RequiresModification = true)]
public sealed class EditScheduleTool : IAITool
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct5
	{
		public Dictionary<string, ScheduleField> dictionary_0;

		public IList<SchedulableField> ilist_0;

		public ScheduleDefinition scheduleDefinition_0;
	}

	[CompilerGenerated]
	public sealed class Class442
	{
		public long long_0;

		internal bool method_0(SchedulableField schedulableField_0)
		{
			return schedulableField_0.ParameterId.Value == long_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class443 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public EditScheduleTool editScheduleTool_0;

		private string string_0;

		private string string_1;

		private IList<string> ilist_0;

		private IList<string> ilist_1;

		private bool bool_0;

		private string string_2;

		private string string_3;

		private string string_4;

		private string string_5;

		private string string_6;

		private string string_7;

		private bool bool_1;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private ViewSchedule viewSchedule_0;

		private Document document_0;

		private int int_1;

		private object object_0;

		private ViewSchedule viewSchedule_1;

		private FilteredElementCollector filteredElementCollector_0;

		private IEnumerator<Element> ienumerator_0;

		private ViewSchedule viewSchedule_2;

		private Struct5 struct5_0;

		private List<string> list_0;

		private string string_8;

		private int int_2;

		private ScheduleField scheduleField_0;

		private string string_9;

		private IEnumerator<string> ienumerator_1;

		private string string_10;

		private int int_3;

		private List<int> list_1;

		private int int_4;

		private ScheduleField scheduleField_1;

		private string string_11;

		private int int_5;

		private ScheduleField scheduleField_2;

		private string string_12;

		private int int_6;

		private IEnumerator<string> ienumerator_2;

		private string string_13;

		private ScheduleField scheduleField_3;

		private ScheduleSortGroupField scheduleSortGroupField_0;

		private ScheduleField scheduleField_4;

		private ScheduleFilterType? nullable_0;

		private ScheduleFilter scheduleFilter_0;

		private ScheduleField scheduleField_5;

		private ScheduleSortGroupField scheduleSortGroupField_1;

		private Exception exception_0;

		private Exception exception_1;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b8: Expected O, but got Unknown
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Expected O, but got Unknown
			//IL_0b44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4e: Expected O, but got Unknown
			//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aba: Expected O, but got Unknown
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Expected O, but got Unknown
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
					Class443 stateMachine = this;
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
				ilist_0 = aitoolContext_0.GetParameter<IList<string>>("addFieldParameterIds", (IList<string>)null);
				ilist_1 = aitoolContext_0.GetParameter<IList<string>>("removeFieldParameterIds", (IList<string>)null);
				bool_0 = aitoolContext_0.GetParameter<bool>("replaceFields", false);
				string_2 = aitoolContext_0.GetParameter<string>("sortByParameterId", (string)null);
				string_3 = aitoolContext_0.GetParameter<string>("sortOrder", "ascending");
				string_4 = aitoolContext_0.GetParameter<string>("filterFieldParameterId", (string)null);
				string_5 = aitoolContext_0.GetParameter<string>("filterType", (string)null);
				string_6 = aitoolContext_0.GetParameter<string>("filterValue", (string)null);
				string_7 = aitoolContext_0.GetParameter<string>("groupByParameterId", (string)null);
				bool_1 = aitoolContext_0.GetParameter<bool>("showGroupHeader", true);
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				iviewService_0 = ((revitAdapter != null) ? revitAdapter.ViewService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				if (iviewService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ViewService");
				}
				else if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					viewSchedule_0 = null;
					if (!string.IsNullOrEmpty(string_0))
					{
						if (int.TryParse(string_0, out int_1))
						{
							object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
							object obj = object_0;
							viewSchedule_1 = (ViewSchedule)((obj is ViewSchedule) ? obj : null);
							if (viewSchedule_1 != null)
							{
								viewSchedule_0 = viewSchedule_1;
							}
							object_0 = null;
							viewSchedule_1 = null;
						}
					}
					else if (!string.IsNullOrEmpty(string_1))
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
					}
					if (viewSchedule_0 == null)
					{
						result = AIToolResult.Fail("找不到明细表: " + (string_0 ?? string_1));
					}
					else
					{
						object document2 = aitoolContext_0.Document;
						document_0 = (Document)((document2 is Document) ? document2 : null);
						if (document_0 != null)
						{
							try
							{
								struct5_0.scheduleDefinition_0 = viewSchedule_0.Definition;
								list_0 = new List<string>();
								struct5_0.ilist_0 = struct5_0.scheduleDefinition_0.GetSchedulableFields();
								struct5_0.dictionary_0 = new Dictionary<string, ScheduleField>();
								int_2 = 0;
								while (int_2 < struct5_0.scheduleDefinition_0.GetFieldCount())
								{
									scheduleField_0 = struct5_0.scheduleDefinition_0.GetField(int_2);
									string_9 = scheduleField_0.ParameterId.Value.ToString();
									if (!struct5_0.dictionary_0.ContainsKey(string_9))
									{
										struct5_0.dictionary_0[string_9] = scheduleField_0;
									}
									scheduleField_0 = null;
									string_9 = null;
									int_2++;
								}
								if (bool_0 && ilist_0 != null && ilist_0.Count > 0)
								{
									struct5_0.scheduleDefinition_0.ClearFields();
									struct5_0.dictionary_0.Clear();
									list_0.Add("清除了所有字段");
									ienumerator_1 = ilist_0.GetEnumerator();
									try
									{
										while (ienumerator_1.MoveNext())
										{
											string_10 = ienumerator_1.Current;
											smethod_0(string_10, ref struct5_0);
											string_10 = null;
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
									List<string> list = list_0;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
									defaultInterpolatedStringHandler.AppendLiteral("添加了 ");
									defaultInterpolatedStringHandler.AppendFormatted(ilist_0.Count);
									defaultInterpolatedStringHandler.AppendLiteral(" 个字段");
									list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
								}
								else
								{
									if (ilist_1 != null && ilist_1.Count > 0)
									{
										int_3 = 0;
										list_1 = new List<int>();
										int_4 = 0;
										while (int_4 < struct5_0.scheduleDefinition_0.GetFieldCount())
										{
											scheduleField_1 = struct5_0.scheduleDefinition_0.GetField(int_4);
											string_11 = scheduleField_1.ParameterId.Value.ToString();
											if (ilist_1.Contains(string_11))
											{
												list_1.Add(int_4);
											}
											scheduleField_1 = null;
											string_11 = null;
											int_4++;
										}
										int_5 = list_1.Count - 1;
										while (int_5 >= 0)
										{
											try
											{
												scheduleField_2 = struct5_0.scheduleDefinition_0.GetField(list_1[int_5]);
												string_12 = scheduleField_2.ParameterId.Value.ToString();
												struct5_0.dictionary_0.Remove(string_12);
												struct5_0.scheduleDefinition_0.RemoveField(list_1[int_5]);
												int_3++;
												scheduleField_2 = null;
												string_12 = null;
											}
											catch
											{
											}
											int_5--;
										}
										if (int_3 > 0)
										{
											List<string> list2 = list_0;
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(8, 1);
											defaultInterpolatedStringHandler2.AppendLiteral("删除了 ");
											defaultInterpolatedStringHandler2.AppendFormatted(int_3);
											defaultInterpolatedStringHandler2.AppendLiteral(" 个字段");
											list2.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
										}
										list_1 = null;
									}
									if (ilist_0 != null && ilist_0.Count > 0)
									{
										int_6 = 0;
										ienumerator_2 = ilist_0.GetEnumerator();
										try
										{
											while (ienumerator_2.MoveNext())
											{
												string_13 = ienumerator_2.Current;
												if (!struct5_0.dictionary_0.ContainsKey(string_13) && smethod_0(string_13, ref struct5_0) != null)
												{
													int_6++;
												}
												string_13 = null;
											}
										}
										finally
										{
											if (num < 0 && ienumerator_2 != null)
											{
												ienumerator_2.Dispose();
											}
										}
										ienumerator_2 = null;
										if (int_6 > 0)
										{
											List<string> list3 = list_0;
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(8, 1);
											defaultInterpolatedStringHandler3.AppendLiteral("添加了 ");
											defaultInterpolatedStringHandler3.AppendFormatted(int_6);
											defaultInterpolatedStringHandler3.AppendLiteral(" 个字段");
											list3.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
										}
									}
								}
								if (!string.IsNullOrEmpty(string_2))
								{
									scheduleField_3 = smethod_0(string_2, ref struct5_0);
									if (scheduleField_3 != null)
									{
										struct5_0.scheduleDefinition_0.ClearSortGroupFields();
										scheduleSortGroupField_0 = new ScheduleSortGroupField(scheduleField_3.FieldId);
										scheduleSortGroupField_0.SortOrder = (ScheduleSortOrder)((string_3 == "descending") ? 1 : 0);
										struct5_0.scheduleDefinition_0.AddSortGroupField(scheduleSortGroupField_0);
										list_0.Add("设置了排序");
										scheduleSortGroupField_0 = null;
									}
									scheduleField_3 = null;
								}
								if (!string.IsNullOrEmpty(string_4) && !string.IsNullOrEmpty(string_6))
								{
									scheduleField_4 = smethod_0(string_4, ref struct5_0);
									if (scheduleField_4 != null)
									{
										struct5_0.scheduleDefinition_0.ClearFilters();
										nullable_0 = editScheduleTool_0.method_0(string_5);
										if (nullable_0.HasValue)
										{
											scheduleFilter_0 = new ScheduleFilter(scheduleField_4.FieldId, nullable_0.Value, string_6);
											struct5_0.scheduleDefinition_0.AddFilter(scheduleFilter_0);
											list_0.Add("设置了过滤");
											scheduleFilter_0 = null;
										}
									}
									scheduleField_4 = null;
								}
								if (!string.IsNullOrEmpty(string_7))
								{
									scheduleField_5 = smethod_0(string_7, ref struct5_0);
									if (scheduleField_5 != null)
									{
										struct5_0.scheduleDefinition_0.ClearSortGroupFields();
										scheduleSortGroupField_1 = new ScheduleSortGroupField(scheduleField_5.FieldId);
										scheduleSortGroupField_1.ShowHeader = bool_1;
										struct5_0.scheduleDefinition_0.AddSortGroupField(scheduleSortGroupField_1);
										list_0.Add("设置了分组");
										scheduleSortGroupField_1 = null;
									}
									scheduleField_5 = null;
								}
								string_8 = ielementService_0.GetElementName((object)viewSchedule_0);
								result = AIToolResult.Ok("成功编辑明细表: " + string_8 + ((list_0.Count > 0) ? (" (" + string.Join(", ", list_0) + ")") : ""), (object)new Class113<int?, string, List<string>>(ielementService_0.GetElementId((object)viewSchedule_0), string_8, list_0));
							}
							catch (Exception ex)
							{
								exception_0 = ex;
								result = AIToolResult.Fail("编辑明细表操作失败: " + exception_0.Message);
							}
						}
						else
						{
							result = AIToolResult.Fail("文档对象无效");
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_1 = ex;
				result = AIToolResult.Fail("编辑明细表失败: " + exception_1.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "edit_schedule";

	public string Category => "视图高级操作";

	public string Description => "编辑已存在明细表的字段、排序、过滤和分组";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"scheduleId\": {\n                \"type\": \"string\",\n                \"description\": \"要编辑的明细表元素 ID（可选）\"\n            },\n            \"scheduleName\": {\n                \"type\": \"string\",\n                \"description\": \"要编辑的明细表名称（可选，如果不提供 scheduleId 则使用此参数）\"\n            },\n            \"addFieldParameterIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"string\" },\n                \"description\": \"要添加的字段 parameterId 列表（可选）。从 get_schedule_fields 获取的 parameterId 字段（如 '-1002052'）\"\n            },\n            \"removeFieldParameterIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"string\" },\n                \"description\": \"要删除的字段 parameterId 列表（可选）\"\n            },\n            \"replaceFields\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否替换所有字段（默认为 false）。true = 用 addFieldParameterIds 替换所有字段，false = 只添加不删除\"\n            },\n            \"sortByParameterId\": {\n                \"type\": \"string\",\n                \"description\": \"排序字段的 parameterId（可选）。从 get_schedule_fields 获取。设置后会覆盖现有排序\"\n            },\n            \"sortOrder\": {\n                \"type\": \"string\",\n                \"description\": \"排序方向：'ascending'（升序）或 'descending'（降序），默认升序\",\n                \"enum\": [\"ascending\", \"descending\"],\n                \"default\": \"ascending\"\n            },\n            \"filterFieldParameterId\": {\n                \"type\": \"string\",\n                \"description\": \"过滤字段的 parameterId（可选）。从 get_schedule_fields 获取。设置后会覆盖现有过滤\"\n            },\n            \"filterType\": {\n                \"type\": \"string\",\n                \"description\": \"过滤类型：'equal', 'greater_than', 'less_than', 'greater_or_equal', 'less_or_equal', 'contains', 'not_equal'\",\n                \"enum\": [\"equal\", \"greater_than\", \"less_than\", \"greater_or_equal\", \"less_or_equal\", \"contains\", \"not_equal\"]\n            },\n            \"filterValue\": {\n                \"type\": \"string\",\n                \"description\": \"过滤值（可选，与 filterFieldParameterId 配合使用）\"\n            },\n            \"groupByParameterId\": {\n                \"type\": \"string\",\n                \"description\": \"分组字段的 parameterId（可选）。从 get_schedule_fields 获取。设置后会覆盖现有分组\"\n            },\n            \"showGroupHeader\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否显示分组标题（默认为 true）\",\n                \"default\": true\n            }\n        }\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class443))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class443 stateMachine = new Class443();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.editScheduleTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private ScheduleFilterType? method_0(string string_0)
	{
		string text = string_0?.ToLower();
		switch (Class653.smethod_0(text))
		{
		case 796199151u:
			if (text == "equal")
			{
				return (ScheduleFilterType)2;
			}
			goto default;
		case 343236382u:
			if (text == "not_begins_with")
			{
				return (ScheduleFilterType)11;
			}
			goto default;
		case 138129653u:
			if (text == "not_equal")
			{
				return (ScheduleFilterType)3;
			}
			goto default;
		case 2621950312u:
			if (text == "greater_or_equal")
			{
				return (ScheduleFilterType)5;
			}
			goto default;
		case 2017020315u:
			if (text == "less_or_equal")
			{
				return (ScheduleFilterType)7;
			}
			goto default;
		case 1825239352u:
			if (text == "contains")
			{
				return (ScheduleFilterType)8;
			}
			goto default;
		case 3220978160u:
			if (text == "begins_with")
			{
				return (ScheduleFilterType)10;
			}
			goto default;
		case 2845923401u:
			if (text == "greater_than")
			{
				return (ScheduleFilterType)4;
			}
			goto default;
		case 2783210406u:
			if (text == "not_contains")
			{
				return (ScheduleFilterType)9;
			}
			goto default;
		case 4210062626u:
			if (text == "not_ends_with")
			{
				return (ScheduleFilterType)13;
			}
			goto default;
		case 3724801604u:
			if (text == "ends_with")
			{
				return (ScheduleFilterType)12;
			}
			goto default;
		case 3627653626u:
			if (text == "less_than")
			{
				return (ScheduleFilterType)6;
			}
			goto default;
		default:
			return null;
		}
	}

	[CompilerGenerated]
	internal static ScheduleField? smethod_0(string string_0, ref Struct5 struct5_0)
	{
		if (struct5_0.dictionary_0.TryGetValue(string_0, out ScheduleField value))
		{
			return value;
		}
		if (long.TryParse(string_0, out var long_0))
		{
			SchedulableField val = struct5_0.ilist_0.FirstOrDefault((SchedulableField schedulableField_0) => schedulableField_0.ParameterId.Value == long_0);
			if (val != (SchedulableField)null)
			{
				ScheduleField val2 = struct5_0.scheduleDefinition_0.AddField(val);
				struct5_0.dictionary_0[string_0] = val2;
				return val2;
			}
		}
		return null;
	}
}
