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
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using ns0;
using ns6;

namespace RevitAi.Revit.AI.Tools;

[AITool("create_topography_surface", Category = "地形建模", Description = "创建新的地形表面。", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateTopographySurfaceTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class345 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CreateTopographySurfaceTool createTopographySurfaceTool_0;

		private object object_0;

		private List<(double X, double Y, double Z)> list_0;

		private List<XYZ> list_1;

		private List<object> list_2;

		private object object_1;

		private int int_1;

		private Exception exception_0;

		void IAsyncStateMachine.MoveNext()
		{
			AIToolResult result;
			try
			{
				object_0 = aitoolContext_0.GetParameter<object>("points", (object)null);
				list_0 = createTopographySurfaceTool_0.method_1(object_0);
				if (list_0 == null || list_0.Count < 3)
				{
					result = AIToolResult.Fail("创建地形至少需要3个有效的高程点");
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[CreateTopographySurfaceTool] 准备创建地形，点数: ");
					defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					list_1 = ((IEnumerable<(double, double, double)>)list_0).Select((Func<(double, double, double), XYZ>)delegate((double X, double Y, double Z) valueTuple_0)
					{
						//IL_0030: Unknown result type (might be due to invalid IL or missing references)
						//IL_0036: Expected O, but got Unknown
						return new XYZ(valueTuple_0.X / 304.8, valueTuple_0.Y / 304.8, valueTuple_0.Z / 304.8);
					}).ToList();
					list_2 = list_1.Cast<object>().ToList();
					object_1 = createTopographySurfaceTool_0.itopographyService_0.CreateTopographySurface(aitoolContext_0.Document, (IList<object>)list_2);
					if (object_1 == null)
					{
						result = AIToolResult.Fail("创建地形表面失败");
					}
					else
					{
						int_1 = createTopographySurfaceTool_0.method_4(object_1);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(41, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("[CreateTopographySurfaceTool] 成功创建地形，ID: ");
						defaultInterpolatedStringHandler2.AppendFormatted(int_1);
						Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(17, 1);
						defaultInterpolatedStringHandler3.AppendLiteral("成功创建地形表面，包含 ");
						defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler3.AppendLiteral(" 个高程点");
						result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class95<int, int, string>(int_1, list_0.Count, createTopographySurfaceTool_0.method_5()));
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[CreateTopographySurfaceTool] 创建地形失败", exception_0);
				result = AIToolResult.Fail("创建地形失败：" + exception_0.Message);
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
	public sealed class Class346 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateTopographySurfaceTool createTopographySurfaceTool_0;

		private AIToolResult aitoolResult_0;

		private Exception exception_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			if (num == 0)
			{
			}
			AIToolResult result;
			try
			{
				TaskAwaiter<AIToolResult> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_00a4;
				}
				if (aitoolContext_0.Document != null)
				{
					Logger.Info("[CreateTopographySurfaceTool] 开始创建地形表面");
					awaiter = createTopographySurfaceTool_0.method_0(aitoolContext_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						Class346 stateMachine = this;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
						return;
					}
					goto IL_00a4;
				}
				result = AIToolResult.Fail("文档对象为空");
				goto end_IL_000b;
				IL_00a4:
				aitoolResult_0 = awaiter.GetResult();
				result = aitoolResult_0;
				end_IL_000b:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[CreateTopographySurfaceTool] 工具执行失败", exception_0);
				result = AIToolResult.Fail("执行失败：" + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	private readonly ITopographyService itopographyService_0;

	public string Name => "create_topography_surface";

	public string Category => "地形建模";

	public string Description => "创建地形表面";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"points\": {\n                \"type\": \"array\",\n                \"description\": \"高程点数组，每个点包含 x, y, z 坐标（单位：毫米）。至少需要3个点\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"x\": { \"type\": \"number\", \"description\": \"X坐标（毫米）\" },\n                        \"y\": { \"type\": \"number\", \"description\": \"Y坐标（毫米）\" },\n                        \"z\": { \"type\": \"number\", \"description\": \"Z高程（毫米）\" }\n                    },\n                    \"required\": [\"x\", \"y\", \"z\"]\n                }\n            }\n        },\n        \"required\": [\"points\"]\n    }";

	public CreateTopographySurfaceTool(ITopographyService topographyService)
	{
		itopographyService_0 = topographyService ?? throw new ArgumentNullException("topographyService");
	}

	[AsyncStateMachine(typeof(Class346))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class346 stateMachine = new Class346();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createTopographySurfaceTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class345))]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0)
	{
		Class345 stateMachine = new Class345();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createTopographySurfaceTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private List<(double X, double Y, double Z)>? method_1(object object_0)
	{
		try
		{
			List<(double, double, double)> list = new List<(double, double, double)>();
			if (object_0 == null)
			{
				Logger.Warning("[ParsePoints] pointsParam 为 null");
				return null;
			}
			if (object_0 is IEnumerable enumerable)
			{
				foreach (object item in enumerable)
				{
					if (item != null)
					{
						if (method_2(item, out (double, double, double) valueTuple_))
						{
							list.Add(valueTuple_);
						}
						else
						{
							Logger.Warning("[ParsePoints] 无法解析点对象，类型: " + item.GetType().Name);
						}
					}
				}
			}
			else
			{
				Logger.Warning("[ParsePoints] pointsParam 不是可枚举类型，类型: " + object_0.GetType().Name);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[ParsePoints] 成功解析 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个点");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return (list.Count > 0) ? list : null;
		}
		catch (Exception ex)
		{
			Logger.Error("[ParsePoints] 解析点数组失败: " + ex.Message);
			return null;
		}
	}

	private bool method_2(object object_0, out (double X, double Y, double Z) valueTuple_0)
	{
		valueTuple_0 = (X: 0.0, Y: 0.0, Z: 0.0);
		try
		{
			double? num = null;
			double? num2 = null;
			double? num3 = null;
			if (object_0 is IDictionary dictionary)
			{
				foreach (object key in dictionary.Keys)
				{
					if (key == null)
					{
						continue;
					}
					string text = key.ToString()?.ToLower();
					object obj = dictionary[key];
					if (obj != null && text != null)
					{
						if (text == "x")
						{
							num = Convert.ToDouble(obj);
						}
						else if (text == "y")
						{
							num2 = Convert.ToDouble(obj);
						}
						else if (text == "z")
						{
							num3 = Convert.ToDouble(obj);
						}
					}
				}
			}
			else
			{
				PropertyInfo propertyInfo = method_3(object_0.GetType(), "x");
				PropertyInfo propertyInfo2 = method_3(object_0.GetType(), "y");
				PropertyInfo propertyInfo3 = method_3(object_0.GetType(), "z");
				if (propertyInfo != null)
				{
					object value = propertyInfo.GetValue(object_0);
					if (value != null)
					{
						num = Convert.ToDouble(value);
					}
				}
				if (propertyInfo2 != null)
				{
					object value2 = propertyInfo2.GetValue(object_0);
					if (value2 != null)
					{
						num2 = Convert.ToDouble(value2);
					}
				}
				if (propertyInfo3 != null)
				{
					object value3 = propertyInfo3.GetValue(object_0);
					if (value3 != null)
					{
						num3 = Convert.ToDouble(value3);
					}
				}
			}
			if (num.HasValue && num2.HasValue && num3.HasValue)
			{
				valueTuple_0 = (X: num.Value, Y: num2.Value, Z: num3.Value);
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logger.Warning("[TryParsePoint] 解析点失败: " + ex.Message);
			return false;
		}
	}

	private PropertyInfo? method_3(Type type_0, string string_0)
	{
		PropertyInfo property = type_0.GetProperty(string_0);
		if (property != null)
		{
			return property;
		}
		return type_0.GetProperty(string_0, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
	}

	private int method_4(object object_0)
	{
		try
		{
			Element val = (Element)((object_0 is Element) ? object_0 : null);
			if (val != null)
			{
				return (int)val.Id.Value;
			}
			return 0;
		}
		catch
		{
			return 0;
		}
	}

	private string method_5()
	{
		return "Toposolid (Revit 2025+)";
	}
}
