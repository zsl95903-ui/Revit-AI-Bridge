using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Newtonsoft.Json.Linq;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("create_section_view", Category = "视图管理", Description = "创建剖面视图。支持四个标准方向：left_to_right（从左往右）、right_to_left（从右往左）、bottom_to_top（从下往上）、top_to_bottom（从上往下）", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateSectionViewTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class416 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateSectionViewTool createSectionViewTool_0;

		private string string_0;

		private string string_1;

		private object object_0;

		private object object_1;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private XYZ xyz_0;

		private XYZ xyz_1;

		private XYZ xyz_2;

		private XYZ xyz_3;

		private XYZ xyz_4;

		private object object_2;

		private int? nullable_0;

		private string string_2;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Expected O, but got Unknown
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Expected O, but got Unknown
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class416 stateMachine = this;
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
				string_0 = aitoolContext_0.GetParameter<string>("view_name", (string)null);
				string_1 = aitoolContext_0.GetParameter<string>("direction", "left_to_right");
				object_0 = aitoolContext_0.GetParameter<object>("min_point", (object)null);
				object_1 = aitoolContext_0.GetParameter<object>("max_point", (object)null);
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
				else if (!createSectionViewTool_0.method_0(object_0, out xyz_0) || xyz_0 == null)
				{
					result = AIToolResult.Fail("无法解析最小点坐标");
				}
				else if (!createSectionViewTool_0.method_0(object_1, out xyz_1) || xyz_1 == null)
				{
					result = AIToolResult.Fail("无法解析最大点坐标");
				}
				else
				{
					xyz_2 = new XYZ(xyz_0.X / 304.8, xyz_0.Y / 304.8, xyz_0.Z / 304.8);
					xyz_3 = new XYZ(xyz_1.X / 304.8, xyz_1.Y / 304.8, xyz_1.Z / 304.8);
					Logger.Info("[CreateSectionViewTool] 创建剖面视图 - 名称: " + string_0 + ", 方向: " + string_1);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 6);
					defaultInterpolatedStringHandler.AppendLiteral("[CreateSectionViewTool] 最小点(mm): (");
					defaultInterpolatedStringHandler.AppendFormatted(xyz_0.X, "F2");
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(xyz_0.Y, "F2");
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(xyz_0.Z, "F2");
					defaultInterpolatedStringHandler.AppendLiteral(") -> (ft): (");
					defaultInterpolatedStringHandler.AppendFormatted(xyz_2.X, "F2");
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(xyz_2.Y, "F2");
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(xyz_2.Z, "F2");
					defaultInterpolatedStringHandler.AppendLiteral(")");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(55, 6);
					defaultInterpolatedStringHandler2.AppendLiteral("[CreateSectionViewTool] 最大点(mm): (");
					defaultInterpolatedStringHandler2.AppendFormatted(xyz_1.X, "F2");
					defaultInterpolatedStringHandler2.AppendLiteral(", ");
					defaultInterpolatedStringHandler2.AppendFormatted(xyz_1.Y, "F2");
					defaultInterpolatedStringHandler2.AppendLiteral(", ");
					defaultInterpolatedStringHandler2.AppendFormatted(xyz_1.Z, "F2");
					defaultInterpolatedStringHandler2.AppendLiteral(") -> (ft): (");
					defaultInterpolatedStringHandler2.AppendFormatted(xyz_3.X, "F2");
					defaultInterpolatedStringHandler2.AppendLiteral(", ");
					defaultInterpolatedStringHandler2.AppendFormatted(xyz_3.Y, "F2");
					defaultInterpolatedStringHandler2.AppendLiteral(", ");
					defaultInterpolatedStringHandler2.AppendFormatted(xyz_3.Z, "F2");
					defaultInterpolatedStringHandler2.AppendLiteral(")");
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					xyz_4 = xyz_3 - xyz_2;
					if (xyz_4.X < 0.01 && xyz_4.Y < 0.01 && xyz_4.Z < 0.01)
					{
						result = AIToolResult.Fail("剖面框尺寸过小，最小边长必须大于 3mm");
					}
					else
					{
						object_2 = iviewService_0.CreateSectionView(aitoolContext_0.Document, string_0, string_1, (object)xyz_2, (object)xyz_3);
						if (object_2 == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(86, 7);
							defaultInterpolatedStringHandler3.AppendLiteral("创建剖面视图失败。可能原因：1) 剖面框尺寸无效；2) 方向参数错误；3) 无法获取剖面视图类型。参数：direction=");
							defaultInterpolatedStringHandler3.AppendFormatted(string_1);
							defaultInterpolatedStringHandler3.AppendLiteral(", min=(");
							defaultInterpolatedStringHandler3.AppendFormatted(xyz_2.X, "F2");
							defaultInterpolatedStringHandler3.AppendLiteral(", ");
							defaultInterpolatedStringHandler3.AppendFormatted(xyz_2.Y, "F2");
							defaultInterpolatedStringHandler3.AppendLiteral(", ");
							defaultInterpolatedStringHandler3.AppendFormatted(xyz_2.Z, "F2");
							defaultInterpolatedStringHandler3.AppendLiteral("), max=(");
							defaultInterpolatedStringHandler3.AppendFormatted(xyz_3.X, "F2");
							defaultInterpolatedStringHandler3.AppendLiteral(", ");
							defaultInterpolatedStringHandler3.AppendFormatted(xyz_3.Y, "F2");
							defaultInterpolatedStringHandler3.AppendLiteral(", ");
							defaultInterpolatedStringHandler3.AppendFormatted(xyz_3.Z, "F2");
							defaultInterpolatedStringHandler3.AppendLiteral(")");
							result = AIToolResult.Fail(defaultInterpolatedStringHandler3.ToStringAndClear());
						}
						else
						{
							nullable_0 = ielementService_0.GetElementId(object_2);
							string_2 = ielementService_0.GetElementName(object_2);
							result = AIToolResult.Ok("成功创建剖面视图: " + (string_2 ?? string_0), (object)new Class78<int?, string, string>(nullable_0, string_2, string_1));
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("创建剖面视图失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_section_view";

	public string Category => "视图管理";

	public string Description => "创建剖面视图";

	public string ParametersSchema => "\r\n{\r\n    \"type\": \"object\",\r\n    \"properties\": {\r\n        \"view_name\": {\r\n            \"type\": \"string\",\r\n            \"description\": \"剖面视图名称\"\r\n        },\r\n        \"direction\": {\r\n            \"type\": \"string\",\r\n            \"description\": \"剖面方向：left_to_right（从左往右，类似西立面）、right_to_left（从右往左，类似东立面）、bottom_to_top（从下往上，类似南立面）、top_to_bottom（从上往下，类似北立面）\",\r\n            \"enum\": [\"left_to_right\", \"right_to_left\", \"bottom_to_top\", \"top_to_bottom\"],\r\n            \"default\": \"left_to_right\"\r\n        },\r\n        \"min_point\": {\r\n            \"type\": \"object\",\r\n            \"description\": \"剖面框的最小角点（世界坐标系，单位：毫米）\",\r\n            \"properties\": {\r\n                \"x\": { \"type\": \"number\" },\r\n                \"y\": { \"type\": \"number\" },\r\n                \"z\": { \"type\": \"number\" }\r\n            },\r\n            \"required\": [\"x\", \"y\", \"z\"]\r\n        },\r\n        \"max_point\": {\r\n            \"type\": \"object\",\r\n            \"description\": \"剖面框的最大角点（世界坐标系，单位：毫米）\",\r\n            \"properties\": {\r\n                \"x\": { \"type\": \"number\" },\r\n                \"y\": { \"type\": \"number\" },\r\n                \"z\": { \"type\": \"number\" }\r\n            },\r\n            \"required\": [\"x\", \"y\", \"z\"]\r\n        }\r\n    },\r\n    \"required\": [\"view_name\", \"direction\", \"min_point\", \"max_point\"]\r\n}";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class416))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class416 stateMachine = new Class416();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createSectionViewTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private bool method_0(object object_0, out XYZ? xyz_0)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		try
		{
			if (object_0 == null)
			{
				xyz_0 = null;
				return false;
			}
			JObject val = (JObject)((object_0 is JObject) ? object_0 : null);
			if (val != null)
			{
				JToken val2 = val["x"];
				JToken val3 = val["y"];
				JToken val4 = val["z"];
				if (val2 != null && val3 != null && val4 != null)
				{
					double num = Convert.ToDouble(val2);
					double num2 = Convert.ToDouble(val3);
					double num3 = Convert.ToDouble(val4);
					xyz_0 = new XYZ(num, num2, num3);
					return true;
				}
				xyz_0 = null;
				return false;
			}
			if (object_0 is Dictionary<string, object> dictionary)
			{
				if (dictionary.TryGetValue("x", out var value) && dictionary.TryGetValue("y", out var value2) && dictionary.TryGetValue("z", out var value3))
				{
					double num4 = Convert.ToDouble(value);
					double num5 = Convert.ToDouble(value2);
					double num6 = Convert.ToDouble(value3);
					xyz_0 = new XYZ(num4, num5, num6);
					return true;
				}
				xyz_0 = null;
				return false;
			}
			Type type = object_0.GetType();
			PropertyInfo property = type.GetProperty("x");
			PropertyInfo property2 = type.GetProperty("y");
			PropertyInfo property3 = type.GetProperty("z");
			if (property != null && property2 != null && property3 != null)
			{
				double num7 = Convert.ToDouble(property.GetValue(object_0));
				double num8 = Convert.ToDouble(property2.GetValue(object_0));
				double num9 = Convert.ToDouble(property3.GetValue(object_0));
				xyz_0 = new XYZ(num7, num8, num9);
				return true;
			}
			xyz_0 = null;
			return false;
		}
		catch
		{
			xyz_0 = null;
			return false;
		}
	}
}
