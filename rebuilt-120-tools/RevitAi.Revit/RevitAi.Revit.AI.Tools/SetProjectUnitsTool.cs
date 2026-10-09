using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Units;
using Autodesk.Revit.DB;
using ns0;
using ns2;
using ns6;

namespace RevitAi.Revit.AI.Tools;

[AITool("set_project_units", Category = "项目设置", Description = "设置项目的单位类型。支持设置长度、面积、体积、角度、坡度的单位。可先用 get_project_units 查看当前设置，再使用此工具修改。支持公制（毫米、米、平方厘米、平方米等）和英制（英尺、英寸、平方英尺等）单位。", RequiresTransaction = true, RequiresModification = true)]
public sealed class SetProjectUnitsTool : IAITool
{
	[CompilerGenerated]
	private sealed class Class348 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public SetProjectUnitsTool setProjectUnitsTool_0;

		private Document document_0;

		private Dictionary<UnitType, object> dictionary_0;

		private int int_1;

		private List<string> list_0;

		private Autodesk.Revit.DB.Units units_0;

		private string string_0;

		private string string_1;

		private object object_0;

		private string string_2;

		private object object_1;

		private string string_3;

		private object object_2;

		private string string_4;

		private object object_3;

		private string string_5;

		private object object_4;

		private Dictionary<UnitType, object>.Enumerator enumerator_0;

		private KeyValuePair<UnitType, object> keyValuePair_0;

		private UnitType unitType_0;

		private object object_5;

		private ForgeTypeId forgeTypeId_0;

		private FormatOptions formatOptions_0;

		private Exception exception_0;

		private Exception exception_1;

		private Exception exception_2;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Expected I4, but got Unknown
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Expected O, but got Unknown
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Expected O, but got Unknown
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
					Class348 stateMachine = this;
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
				object document = aitoolContext_0.Document;
				document_0 = (Document)((document is Document) ? document : null);
				if (document_0 == null)
				{
					Logger.Error("[SetProjectUnitsTool] 文档为 null 或不是 Document 类型");
					result = AIToolResult.Fail("文档不可用");
				}
				else
				{
					dictionary_0 = new Dictionary<UnitType, object>();
					if (aitoolContext_0.HasParameter("length"))
					{
						string_1 = aitoolContext_0.GetParameter<string>("length", (string)null);
						if (!string.IsNullOrWhiteSpace(string_1))
						{
							object_0 = setProjectUnitsTool_0.method_0(string_1);
							if (object_0 != null)
							{
								dictionary_0[(UnitType)1] = object_0;
							}
							else
							{
								Logger.Warning("[SetProjectUnitsTool] 无法识别长度单位: " + string_1);
							}
							object_0 = null;
						}
						string_1 = null;
					}
					if (aitoolContext_0.HasParameter("area"))
					{
						string_2 = aitoolContext_0.GetParameter<string>("area", (string)null);
						if (!string.IsNullOrWhiteSpace(string_2))
						{
							object_1 = setProjectUnitsTool_0.method_0(string_2);
							if (object_1 != null)
							{
								dictionary_0[(UnitType)2] = object_1;
							}
							else
							{
								Logger.Warning("[SetProjectUnitsTool] 无法识别面积单位: " + string_2);
							}
							object_1 = null;
						}
						string_2 = null;
					}
					if (aitoolContext_0.HasParameter("volume"))
					{
						string_3 = aitoolContext_0.GetParameter<string>("volume", (string)null);
						if (!string.IsNullOrWhiteSpace(string_3))
						{
							object_2 = setProjectUnitsTool_0.method_0(string_3);
							if (object_2 != null)
							{
								dictionary_0[(UnitType)3] = object_2;
							}
							else
							{
								Logger.Warning("[SetProjectUnitsTool] 无法识别体积单位: " + string_3);
							}
							object_2 = null;
						}
						string_3 = null;
					}
					if (aitoolContext_0.HasParameter("angle"))
					{
						string_4 = aitoolContext_0.GetParameter<string>("angle", (string)null);
						if (!string.IsNullOrWhiteSpace(string_4))
						{
							object_3 = setProjectUnitsTool_0.method_0(string_4);
							if (object_3 != null)
							{
								dictionary_0[(UnitType)4] = object_3;
							}
							else
							{
								Logger.Warning("[SetProjectUnitsTool] 无法识别角度单位: " + string_4);
							}
							object_3 = null;
						}
						string_4 = null;
					}
					if (aitoolContext_0.HasParameter("slope"))
					{
						string_5 = aitoolContext_0.GetParameter<string>("slope", (string)null);
						if (!string.IsNullOrWhiteSpace(string_5))
						{
							object_4 = setProjectUnitsTool_0.method_0(string_5);
							if (object_4 != null)
							{
								dictionary_0[(UnitType)5] = object_4;
							}
							else
							{
								Logger.Warning("[SetProjectUnitsTool] 无法识别坡度单位: " + string_5);
							}
							object_4 = null;
						}
						string_5 = null;
					}
					if (dictionary_0.Count == 0)
					{
						result = AIToolResult.Fail("请提供至少一个单位类型参数（length、area、volume、angle、slope）");
					}
					else
					{
						int_1 = 0;
						list_0 = new List<string>();
						units_0 = document_0.GetUnits();
						enumerator_0 = dictionary_0.GetEnumerator();
						try
						{
							while (enumerator_0.MoveNext())
							{
								keyValuePair_0 = enumerator_0.Current;
								unitType_0 = keyValuePair_0.Key;
								object_5 = keyValuePair_0.Value;
								try
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 2);
									defaultInterpolatedStringHandler.AppendLiteral("[SetProjectUnitsTool] 设置 ");
									defaultInterpolatedStringHandler.AppendFormatted<UnitType>(unitType_0);
									defaultInterpolatedStringHandler.AppendLiteral(" 为 ");
									defaultInterpolatedStringHandler.AppendFormatted<object>(object_5);
									Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
									UnitType val = unitType_0;
									ForgeTypeId val2;
									switch ((int)(val) - 1)
									{
									default:
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(10, 1);
										defaultInterpolatedStringHandler2.AppendLiteral("不支持的单位类型: ");
										defaultInterpolatedStringHandler2.AppendFormatted<UnitType>(unitType_0);
										throw new ArgumentException(defaultInterpolatedStringHandler2.ToStringAndClear());
									}
									case 0:
										val2 = SpecTypeId.Length;
										break;
									case 1:
										val2 = SpecTypeId.Area;
										break;
									case 2:
										val2 = SpecTypeId.Volume;
										break;
									case 3:
										val2 = SpecTypeId.Angle;
										break;
									case 4:
										val2 = SpecTypeId.Slope;
										break;
									}
									forgeTypeId_0 = val2;
									if (object_5 != null)
									{
										formatOptions_0 = new FormatOptions((ForgeTypeId)object_5);
										units_0.SetFormatOptions(forgeTypeId_0, formatOptions_0);
										int_1++;
										formatOptions_0 = null;
									}
									forgeTypeId_0 = null;
								}
								catch (Exception ex)
								{
									exception_0 = ex;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(33, 2);
									defaultInterpolatedStringHandler3.AppendLiteral("[SetProjectUnitsTool] 设置 ");
									defaultInterpolatedStringHandler3.AppendFormatted<UnitType>(unitType_0);
									defaultInterpolatedStringHandler3.AppendLiteral(" 时发生异常: ");
									defaultInterpolatedStringHandler3.AppendFormatted(exception_0.Message);
									Logger.Error(defaultInterpolatedStringHandler3.ToStringAndClear(), exception_0);
									List<string> list = list_0;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(2, 2);
									defaultInterpolatedStringHandler4.AppendFormatted<UnitType>(unitType_0);
									defaultInterpolatedStringHandler4.AppendLiteral(": ");
									defaultInterpolatedStringHandler4.AppendFormatted<object>(object_5);
									list.Add(defaultInterpolatedStringHandler4.ToStringAndClear());
								}
								object_5 = null;
								keyValuePair_0 = default(KeyValuePair<UnitType, object>);
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
							}
						}
						enumerator_0 = default(Dictionary<UnitType, object>.Enumerator);
						if (int_1 > 0)
						{
							try
							{
								document_0.SetUnits(units_0);
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(35, 1);
								defaultInterpolatedStringHandler5.AppendLiteral("[SetProjectUnitsTool] 已保存 ");
								defaultInterpolatedStringHandler5.AppendFormatted(int_1);
								defaultInterpolatedStringHandler5.AppendLiteral(" 个单位设置到文档");
								Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
							}
							catch (Exception ex)
							{
								exception_1 = ex;
								Logger.Error("[SetProjectUnitsTool] 保存单位设置失败: " + exception_1.Message, exception_1);
								result = AIToolResult.Fail("保存单位设置失败: " + exception_1.Message);
								goto end_IL_0067;
							}
						}
						if (aitoolContext_0.ProjectUnitService != null)
						{
							aitoolContext_0.ProjectUnitService.ClearCache((object)document_0);
						}
						if (int_1 == 0)
						{
							result = AIToolResult.Fail("没有任何单位设置成功");
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(13, 1);
							defaultInterpolatedStringHandler6.AppendLiteral("✅ 成功设置 ");
							defaultInterpolatedStringHandler6.AppendFormatted(int_1);
							defaultInterpolatedStringHandler6.AppendLiteral(" 个单位类型");
							string_0 = defaultInterpolatedStringHandler6.ToStringAndClear();
							if (list_0.Count > 0)
							{
								string text = string_0;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(5, 1);
								defaultInterpolatedStringHandler7.AppendLiteral("，");
								defaultInterpolatedStringHandler7.AppendFormatted(list_0.Count);
								defaultInterpolatedStringHandler7.AppendLiteral(" 个失败");
								string_0 = text + defaultInterpolatedStringHandler7.ToStringAndClear();
							}
							result = AIToolResult.Ok(string_0, (object)new Class298<int, int, List<string>, _003C_003Ef__AnonymousType315<string, string>[]>(int_1, list_0.Count, list_0, Enumerable.Select(dictionary_0, delegate(KeyValuePair<UnitType, object> keyValuePair_0)
							{
								//IL_0002: Unknown result type (might be due to invalid IL or missing references)
								//IL_0007: Unknown result type (might be due to invalid IL or missing references)
								return new _003C_003Ef__AnonymousType315<string, string>(((object)keyValuePair_0.Key/*cast due to constrained. prefix*/).ToString(), keyValuePair_0.Value.ToString());
							}).ToArray()));
						}
					}
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_2 = ex;
				Logger.Error("[SetProjectUnitsTool] 执行失败", exception_2);
				result = AIToolResult.Fail("设置单位失败: " + exception_2.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "set_project_units";

	public string Category => "项目设置";

	public string Description => "设置项目的单位类型（长度、面积、体积、角度、坡度）";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"length\": {\n                \"type\": \"string\",\n                \"description\": \"长度单位（可选）。可选值：毫米/centimeters/厘米/meters/米/decimeters/分米/feet/英尺/inches/英寸等。\"\n            },\n            \"area\": {\n                \"type\": \"string\",\n                \"description\": \"面积单位（可选）。可选值：square_meters/平方米/square_millimeters/平方毫米/square_feet/平方英尺等。\"\n            },\n            \"volume\": {\n                \"type\": \"string\",\n                \"description\": \"体积单位（可选）。可选值：cubic_meters/立方米/cubic_millimeters/立方毫米/cubic_feet/立方英尺等。\"\n            },\n            \"angle\": {\n                \"type\": \"string\",\n                \"description\": \"角度单位（可选）。可选值：decimal_degrees/度/radians/弧度等。\"\n            },\n            \"slope\": {\n                \"type\": \"string\",\n                \"description\": \"坡度单位（可选）。可选值：slope_percent/百分比/slope_degrees/坡度角度/slope_ratio/比例等。\"\n            }\n        },\n        \"required\": []\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class348))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class348 stateMachine = new Class348();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setProjectUnitsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private object? method_0(string string_0)
	{
		if (string.IsNullOrWhiteSpace(string_0))
		{
			return null;
		}
		string string_1 = string_0.ToLower().Trim().Replace(" ", "")
			.Replace("　", "");
		return Class336.smethod_14(string_1);
	}
}
