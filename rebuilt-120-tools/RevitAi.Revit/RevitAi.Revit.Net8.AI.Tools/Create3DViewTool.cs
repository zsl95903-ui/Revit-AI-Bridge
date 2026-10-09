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

[AITool("create_3d_view", Category = "视图高级操作", Description = "创建新的 3D 视图，可选择启用剖面框。剖面框范围可通过元素的 BoundingBox 自动设置。", RequiresTransaction = true, RequiresModification = true)]
public sealed class Create3DViewTool : IAITool
{
	public struct Struct2
	{
		public double double_0;

		public double double_1;

		public double double_2;

		public double double_3;

		public double double_4;

		public double double_5;

		public int int_0;
	}

	[CompilerGenerated]
	public sealed class Class381 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public Create3DViewTool create3DViewTool_0;

		private string string_0;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private IGeometryService igeometryService_0;

		private object object_0;

		private int? nullable_0;

		private string string_1;

		private Struct2? nullable_1;

		private List<int> list_0;

		private bool bool_0;

		private string string_2;

		private Struct2 struct2_0;

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
					Class381 stateMachine = this;
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
				string_0 = aitoolContext_0.GetParameter<string>("viewName", (string)null);
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				iviewService_0 = ((revitAdapter != null) ? revitAdapter.ViewService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				IRevitAdapter revitAdapter3 = aitoolContext_0.RevitAdapter;
				igeometryService_0 = ((revitAdapter3 != null) ? revitAdapter3.GeometryService : null);
				if (iviewService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ViewService");
				}
				else if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (igeometryService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 GeometryService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					object_0 = iviewService_0.Create3DView(aitoolContext_0.Document, string_0);
					if (object_0 == null)
					{
						result = AIToolResult.Fail("创建 3D 视图失败");
					}
					else
					{
						nullable_0 = ielementService_0.GetElementId(object_0);
						string_1 = ielementService_0.GetElementName(object_0);
						iviewService_0.SetViewDisplayStyle(object_0, 3);
						iviewService_0.SetViewDetailLevel(object_0, 3);
						nullable_1 = null;
						list_0 = create3DViewTool_0.method_0(aitoolContext_0);
						if (list_0.Count <= 0)
						{
							goto IL_0309;
						}
						nullable_1 = create3DViewTool_0.method_2(aitoolContext_0, list_0, ielementService_0, igeometryService_0);
						if (!nullable_1.HasValue)
						{
							Logger.Warning("[Create3DViewTool] 无法计算元素的 BoundingBox，跳过剖面框设置");
							goto IL_0309;
						}
						if (iviewService_0.SetSectionBox(aitoolContext_0.Document, object_0, nullable_1.Value.double_0, nullable_1.Value.double_1, nullable_1.Value.double_2, nullable_1.Value.double_3, nullable_1.Value.double_4, nullable_1.Value.double_5))
						{
							goto IL_0309;
						}
						result = AIToolResult.Fail("设置剖面框范围失败");
					}
				}
				goto end_IL_0067;
				IL_0309:
				bool_0 = nullable_1.HasValue;
				if (bool_0)
				{
					struct2_0 = nullable_1.Value;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 2);
					defaultInterpolatedStringHandler.AppendLiteral("成功创建带剖面框的 3D 视图: ");
					defaultInterpolatedStringHandler.AppendFormatted(string_1 ?? "未命名");
					defaultInterpolatedStringHandler.AppendLiteral("（基于 ");
					defaultInterpolatedStringHandler.AppendFormatted(struct2_0.int_0);
					defaultInterpolatedStringHandler.AppendLiteral(" 个元素的包围框）");
					string_2 = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				else
				{
					string_2 = "成功创建 3D 视图: " + (string_1 ?? "未命名");
				}
				result = AIToolResult.Ok(string_2, (object)new Class36<int?, string, bool, int>(nullable_0, string_1, bool_0, nullable_1?.int_0 ?? 0));
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[Create3DViewTool] 创建 3D 视图失败: " + exception_0.Message, exception_0);
				result = AIToolResult.Fail("创建 3D 视图失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_3d_view";

	public string Category => "视图高级操作";

	public string Description => "创建新的 3D 视图，支持通过元素设置剖面框";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"viewName\": {\n                \"type\": \"string\",\n                \"description\": \"视图名称（可选）\"\n            },\n            \"elementId\": {\n                \"type\": \"integer\",\n                \"description\": \"单个元素 ID（可选）。剖面框将自动包含该元素的 BoundingBox。\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"元素 ID 数组（可选）。剖面框将自动包含所有这些元素的合并 BoundingBox。\"\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（可选，支持 @1 格式）。剖面框将自动包含缓存中所有元素的合并 BoundingBox。\"\n            }\n        },\n        \"required\": []\n    }";

	[AsyncStateMachine(typeof(Class381))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class381 stateMachine = new Class381();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.create3DViewTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private List<int> method_0(AIToolContext aitoolContext_0)
	{
		List<int> list = new List<int>();
		if (aitoolContext_0.HasParameter("cacheId"))
		{
			string parameter = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
			if (!string.IsNullOrEmpty(parameter))
			{
				string text = (parameter.StartsWith("@") ? parameter.Substring(1) : parameter);
				object cachedData = aitoolContext_0.GetCachedData<object>(text);
				if (cachedData != null)
				{
					method_1(cachedData, list);
				}
			}
		}
		if (aitoolContext_0.HasParameter("elementIds"))
		{
			try
			{
				object parameter2 = aitoolContext_0.GetParameter<object>("elementIds", (object)null);
				if (parameter2 != null)
				{
					if (parameter2 is List<int> collection)
					{
						list.AddRange(collection);
					}
					else if (parameter2 is IEnumerable enumerable)
					{
						foreach (object item2 in enumerable)
						{
							if (item2 == null)
							{
								continue;
							}
							try
							{
								if (item2 is int item)
								{
									list.Add(item);
								}
								else if (item2 is IConvertible value)
								{
									int num = Convert.ToInt32(value);
									if (num > 0)
									{
										list.Add(num);
									}
								}
							}
							catch
							{
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[Create3DViewTool] 解析 elementIds 参数失败: " + ex.Message);
			}
		}
		if (aitoolContext_0.HasParameter("elementId"))
		{
			int parameter3 = aitoolContext_0.GetParameter<int>("elementId", 0);
			if (parameter3 > 0)
			{
				list.Add(parameter3);
			}
		}
		List<int> list2 = list.Distinct().ToList();
		if (list2.Count != list.Count)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[Create3DViewTool] 元素 ID 去重: ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" -> ");
			defaultInterpolatedStringHandler.AppendFormatted(list2.Count);
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		return list2;
	}

	private void method_1(object object_0, List<int> list_0)
	{
		if (!(object_0 is IEnumerable enumerable))
		{
			return;
		}
		foreach (object item2 in enumerable)
		{
			if (item2 != null)
			{
				PropertyInfo propertyInfo = item2.GetType().GetProperty("id") ?? item2.GetType().GetProperty("Id");
				if (propertyInfo != null && propertyInfo.GetValue(item2) is int item)
				{
					list_0.Add(item);
				}
			}
		}
	}

	private Struct2? method_2(AIToolContext aitoolContext_0, List<int> list_0, IElementService ielementService_0, IGeometryService igeometryService_0)
	{
		try
		{
			object document = aitoolContext_0.Document;
			if (document == null)
			{
				Logger.Warning("[Create3DViewTool] 文档对象为空，无法计算包围框");
				return null;
			}
			if (list_0.Count == 0)
			{
				Logger.Warning("[Create3DViewTool] 元素 ID 列表为空，无法计算包围框");
				return null;
			}
			double num = double.MaxValue;
			double num2 = double.MaxValue;
			double num3 = double.MaxValue;
			double num4 = double.MinValue;
			double num5 = double.MinValue;
			double num6 = double.MinValue;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			foreach (int item in list_0)
			{
				try
				{
					object elementById = ielementService_0.GetElementById(document, item);
					if (elementById == null)
					{
						num8++;
						continue;
					}
					((double, double, double)?, (double, double, double)?)? boundingBox = igeometryService_0.GetBoundingBox(elementById);
					if (!boundingBox.HasValue)
					{
						num9++;
						continue;
					}
					var (tuple2, tuple3) = boundingBox.Value;
					if (tuple2.HasValue && tuple3.HasValue)
					{
						num = Math.Min(num, tuple2.Value.Item1);
						num2 = Math.Min(num2, tuple2.Value.Item2);
						num3 = Math.Min(num3, tuple2.Value.Item3);
						num4 = Math.Max(num4, tuple3.Value.Item1);
						num5 = Math.Max(num5, tuple3.Value.Item2);
						num6 = Math.Max(num6, tuple3.Value.Item3);
						num7++;
					}
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[Create3DViewTool] 处理元素 ");
					defaultInterpolatedStringHandler.AppendFormatted(item);
					defaultInterpolatedStringHandler.AppendLiteral(" 的 BoundingBox 时出错: ");
					defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			if (num7 == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(59, 3);
				defaultInterpolatedStringHandler2.AppendLiteral("[Create3DViewTool] 没有找到有效的 BoundingBox（共 ");
				defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个元素，");
				defaultInterpolatedStringHandler2.AppendFormatted(num8);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个不存在，");
				defaultInterpolatedStringHandler2.AppendFormatted(num9);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个无边界框）");
				Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			double num10 = (num4 - num) * 0.1;
			double num11 = (num5 - num2) * 0.1;
			double num12 = (num6 - num3) * 0.1;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(60, 8);
			defaultInterpolatedStringHandler3.AppendLiteral("[Create3DViewTool] 计算包围框成功: 有效元素 ");
			defaultInterpolatedStringHandler3.AppendFormatted(num7);
			defaultInterpolatedStringHandler3.AppendLiteral("/");
			defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler3.AppendLiteral("，");
			defaultInterpolatedStringHandler3.AppendLiteral("Min=(");
			defaultInterpolatedStringHandler3.AppendFormatted(num, "F2");
			defaultInterpolatedStringHandler3.AppendLiteral(", ");
			defaultInterpolatedStringHandler3.AppendFormatted(num2, "F2");
			defaultInterpolatedStringHandler3.AppendLiteral(", ");
			defaultInterpolatedStringHandler3.AppendFormatted(num3, "F2");
			defaultInterpolatedStringHandler3.AppendLiteral(")，Max=(");
			defaultInterpolatedStringHandler3.AppendFormatted(num4, "F2");
			defaultInterpolatedStringHandler3.AppendLiteral(", ");
			defaultInterpolatedStringHandler3.AppendFormatted(num5, "F2");
			defaultInterpolatedStringHandler3.AppendLiteral(", ");
			defaultInterpolatedStringHandler3.AppendFormatted(num6, "F2");
			defaultInterpolatedStringHandler3.AppendLiteral(")（英尺）");
			Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
			return new Struct2
			{
				double_0 = num - num10,
				double_1 = num2 - num11,
				double_2 = num3 - num12,
				double_3 = num4 + num10,
				double_4 = num5 + num11,
				double_5 = num6 + num12,
				int_0 = num7
			};
		}
		catch (Exception ex2)
		{
			Logger.Error("[Create3DViewTool] 计算包围框失败: " + ex2.Message, ex2);
			return null;
		}
	}
}
