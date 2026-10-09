using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

[AITool("create_tag", Category = "注释与标记", Description = "为元素创建标记（单个）或在视图中批量自动标记未标记的对象。距离单位：毫米", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateTagTool : IAITool
{
	[CompilerGenerated]
	private sealed class Class422
	{
		public IElementService ielementService_0;

		internal int method_0(object object_0)
		{
			return ielementService_0.GetElementId(object_0).GetValueOrDefault();
		}
	}

	[CompilerGenerated]
	private sealed class Class423 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateTagTool createTagTool_0;

		private IAnnotationService iannotationService_0;

		private IElementService ielementService_0;

		private IViewService iviewService_0;

		private bool bool_0;

		private bool bool_1;

		private bool bool_2;

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
					Class423 stateMachine = this;
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
				iannotationService_0 = ((revitAdapter != null) ? revitAdapter.AnnotationService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				IRevitAdapter revitAdapter3 = aitoolContext_0.RevitAdapter;
				iviewService_0 = ((revitAdapter3 != null) ? revitAdapter3.ViewService : null);
				if (iannotationService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 AnnotationService");
				}
				else if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (iviewService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ViewService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					bool_0 = aitoolContext_0.GetParameter<bool>("batchMode", false);
					bool_1 = aitoolContext_0.HasParameter("categoryNames");
					bool_2 = aitoolContext_0.GetParameter<bool>("includeTagged", false);
					if (bool_1 | bool_2)
					{
						bool_0 = true;
					}
					result = ((bool_0 || !aitoolContext_0.HasParameter("elementId")) ? createTagTool_0.method_1(aitoolContext_0, iannotationService_0, ielementService_0, iviewService_0) : createTagTool_0.method_0(aitoolContext_0, iannotationService_0, ielementService_0, iviewService_0));
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[CreateTagTool] 执行失败: " + exception_0.Message);
				result = AIToolResult.Fail("创建标记失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	private static readonly HashSet<string> hashSet_0 = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		"ThreeD",
		"DrawingSheet",
		"Legend",
		"Schedule",
		"ProjectBrowser",
		"SystemBrowser",
		"Report"
	};

	public string Name => "create_tag";

	public string Category => "注释与标记";

	public string Description => "为元素创建标记（单个）或在视图中批量自动标记未标记的对象。距离单位：毫米";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"elementId\": {\n                \"type\": \"integer\",\n                \"description\": \"要标记的元素 ID（单个模式必选）。批量模式下忽略\"\n            },\n            \"position\": {\n                \"type\": \"object\",\n                \"description\": \"标记位置（单个模式必选）。包含 x/y/z（毫米）。批量模式忽略\",\n                \"properties\": {\n                    \"x\": { \"type\": \"number\", \"description\": \"X 坐标（毫米）\" },\n                    \"y\": { \"type\": \"number\", \"description\": \"Y 坐标（毫米）\" },\n                    \"z\": { \"type\": \"number\", \"description\": \"Z 坐标（毫米，可选，默认 0）\", \"default\": 0 }\n                }\n            },\n            \"tagTypeId\": {\n                \"type\": \"integer\",\n                \"description\": \"标记类型 ID（可选，0 或不传表示自动选择类别默认标记类型）\",\n                \"default\": 0\n            },\n            \"addLeader\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否添加引线（可选，默认 false）\",\n                \"default\": false\n            },\n            \"batchMode\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否启用批量模式：自动为视图中未标记的对象创建标记。默认 false（除非指定 categoryNames 或 includeTagged=true 时自动启用）\",\n                \"default\": false\n            },\n            \"categoryNames\": {\n                \"type\": \"array\",\n                \"description\": \"批量模式：要标记的元素类别（中文，如 门、窗、墙）。不指定则标记所有可标记类别\",\n                \"items\": { \"type\": \"string\" }\n            },\n            \"viewId\": {\n                \"type\": \"integer\",\n                \"description\": \"批量模式：目标视图 ID。不指定则使用当前激活视图\"\n            },\n            \"viewName\": {\n                \"type\": \"string\",\n                \"description\": \"批量模式：目标视图名称（与 viewId 二选一）\"\n            },\n            \"includeTagged\": {\n                \"type\": \"boolean\",\n                \"description\": \"批量模式：是否包含已标记的元素（默认 false，跳过已标记）\",\n                \"default\": false\n            }\n        }\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class423))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class423 stateMachine = new Class423();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createTagTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private AIToolResult method_0(AIToolContext aitoolContext_0, IAnnotationService iannotationService_0, IElementService ielementService_0, IViewService iviewService_0)
	{
		object document = aitoolContext_0.Document;
		if (!aitoolContext_0.HasParameter("elementId"))
		{
			return AIToolResult.Fail("单个模式必须提供 elementId 参数");
		}
		int parameter = aitoolContext_0.GetParameter<int>("elementId", 0);
		int parameter2 = aitoolContext_0.GetParameter<int>("tagTypeId", 0);
		bool parameter3 = aitoolContext_0.GetParameter<bool>("addLeader", false);
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 0.0;
		bool flag = false;
		if (aitoolContext_0.HasParameter("position"))
		{
			try
			{
				object parameter4 = aitoolContext_0.GetParameter<object>("position", (object)null);
				if (parameter4 is IDictionary<string, object> dictionary)
				{
					if (dictionary.TryGetValue("x", out var value) && value != null)
					{
						num = Convert.ToDouble(value);
						flag = true;
					}
					if (dictionary.TryGetValue("y", out var value2) && value2 != null)
					{
						num2 = Convert.ToDouble(value2);
					}
					if (dictionary.TryGetValue("z", out var value3) && value3 != null)
					{
						num3 = Convert.ToDouble(value3);
					}
				}
			}
			catch (Exception ex)
			{
				return AIToolResult.Fail("解析 position 参数失败: " + ex.Message);
			}
		}
		if (!flag)
		{
			return AIToolResult.Fail("单个模式必须提供 position 参数（包含 x/y 坐标）");
		}
		object elementById = ielementService_0.GetElementById(document, parameter);
		if (elementById == null)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
			defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
			defaultInterpolatedStringHandler.AppendFormatted(parameter);
			defaultInterpolatedStringHandler.AppendLiteral(" 的元素");
			return AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		object activeView = iviewService_0.GetActiveView(document);
		if (activeView == null)
		{
			return AIToolResult.Fail("无法获取当前激活视图");
		}
		string viewType = iviewService_0.GetViewType(activeView);
		if (viewType != null && hashSet_0.Contains(viewType))
		{
			return AIToolResult.Fail("当前视图类型 '" + viewType + "' 不支持创建标记，请切换到平面/立面/剖面视图");
		}
		var anon = new
		{
			x = num / 304.8,
			y = num2 / 304.8,
			z = num3 / 304.8
		};
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(43, 4);
		defaultInterpolatedStringHandler2.AppendLiteral("[CreateTagTool] 单个模式：为元素 ");
		defaultInterpolatedStringHandler2.AppendFormatted(parameter);
		defaultInterpolatedStringHandler2.AppendLiteral(" 创建标记，位置 (");
		defaultInterpolatedStringHandler2.AppendFormatted(num, "F0");
		defaultInterpolatedStringHandler2.AppendLiteral(", ");
		defaultInterpolatedStringHandler2.AppendFormatted(num2, "F0");
		defaultInterpolatedStringHandler2.AppendLiteral(", ");
		defaultInterpolatedStringHandler2.AppendFormatted(num3, "F0");
		defaultInterpolatedStringHandler2.AppendLiteral(") mm");
		Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
		object obj = iannotationService_0.CreateTag(document, activeView, elementById, (object)anon, parameter2, parameter3);
		if (obj == null)
		{
			return AIToolResult.Fail("创建标记失败（服务返回 null）");
		}
		int? elementId = ielementService_0.GetElementId(obj);
		int? viewId = iviewService_0.GetViewId(activeView);
		string viewName = iviewService_0.GetViewName(activeView);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(20, 2);
		defaultInterpolatedStringHandler3.AppendLiteral("成功创建标记: 标记 ID ");
		defaultInterpolatedStringHandler3.AppendFormatted(elementId);
		defaultInterpolatedStringHandler3.AppendLiteral("，目标元素 ");
		defaultInterpolatedStringHandler3.AppendFormatted(parameter);
		return AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class88<string, int?, int, int?, bool, int?, string, _003C_003Ef__AnonymousType27<double, double, double>, string>("single", elementId, parameter, (parameter2 > 0) ? new int?(parameter2) : ((int?)null), parameter3, viewId, viewName, new _003C_003Ef__AnonymousType27<double, double, double>(Math.Round(num, 0), Math.Round(num2, 0), Math.Round(num3, 0)), "millimeters"));
	}

	private AIToolResult method_1(AIToolContext aitoolContext_0, IAnnotationService iannotationService_0, IElementService ielementService_0, IViewService iviewService_0)
	{
		object document = aitoolContext_0.Document;
		int parameter = aitoolContext_0.GetParameter<int>("tagTypeId", 0);
		bool parameter2 = aitoolContext_0.GetParameter<bool>("addLeader", false);
		bool parameter3 = aitoolContext_0.GetParameter<bool>("includeTagged", false);
		List<string> list = new List<string>();
		if (aitoolContext_0.HasParameter("categoryNames"))
		{
			try
			{
				object parameter4 = aitoolContext_0.GetParameter<object>("categoryNames", (object)null);
				if (parameter4 is List<object> source)
				{
					list = (from string_0 in source.Select(delegate(object object_0)
						{
							object obj5;
							if (object_0 == null)
							{
								obj5 = null;
							}
							else
							{
								obj5 = object_0.ToString();
								if (obj5 != null)
								{
									goto IL_0015;
								}
							}
							obj5 = string.Empty;
							goto IL_0015;
							IL_0015:
							return (string)obj5;
						})
						where !string.IsNullOrEmpty(string_0)
						select string_0).ToList();
				}
				else if (parameter4 is IEnumerable<object> source2)
				{
					list = (from string_0 in source2.Select(delegate(object object_0)
						{
							object obj5;
							if (object_0 == null)
							{
								obj5 = null;
							}
							else
							{
								obj5 = object_0.ToString();
								if (obj5 != null)
								{
									goto IL_0015;
								}
							}
							obj5 = string.Empty;
							goto IL_0015;
							IL_0015:
							return (string)obj5;
						})
						where !string.IsNullOrEmpty(string_0)
						select string_0).ToList();
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[CreateTagTool] 解析 categoryNames 失败: " + ex.Message);
			}
		}
		object obj = null;
		string text = null;
		if (aitoolContext_0.HasParameter("viewId"))
		{
			int parameter5 = aitoolContext_0.GetParameter<int>("viewId", 0);
			foreach (object allView in iviewService_0.GetAllViews(document))
			{
				if (iviewService_0.GetViewId(allView) == parameter5)
				{
					obj = allView;
					text = iviewService_0.GetViewName(allView);
					break;
				}
			}
			if (obj == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
				defaultInterpolatedStringHandler.AppendFormatted(parameter5);
				defaultInterpolatedStringHandler.AppendLiteral(" 的视图");
				return AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
		else if (aitoolContext_0.HasParameter("viewName"))
		{
			string parameter6 = aitoolContext_0.GetParameter<string>("viewName", (string)null);
			foreach (object allView2 in iviewService_0.GetAllViews(document))
			{
				if (string.Equals(iviewService_0.GetViewName(allView2), parameter6, StringComparison.OrdinalIgnoreCase))
				{
					obj = allView2;
					text = iviewService_0.GetViewName(allView2);
					break;
				}
			}
			if (obj == null)
			{
				return AIToolResult.Fail("找不到名称为 '" + parameter6 + "' 的视图");
			}
		}
		else
		{
			obj = iviewService_0.GetActiveView(document);
			if (obj == null)
			{
				return AIToolResult.Fail("无法获取当前激活视图");
			}
			text = iviewService_0.GetViewName(obj);
		}
		string viewType = iviewService_0.GetViewType(obj);
		if (viewType != null && hashSet_0.Contains(viewType))
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(20, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("视图 '");
			defaultInterpolatedStringHandler2.AppendFormatted(text);
			defaultInterpolatedStringHandler2.AppendLiteral("' 的类型 '");
			defaultInterpolatedStringHandler2.AppendFormatted(viewType);
			defaultInterpolatedStringHandler2.AppendLiteral("' 不支持创建标记");
			return AIToolResult.Fail(defaultInterpolatedStringHandler2.ToStringAndClear());
		}
		int? viewId = iviewService_0.GetViewId(obj);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(54, 4);
		defaultInterpolatedStringHandler3.AppendLiteral("[CreateTagTool] 批量模式：视图 '");
		defaultInterpolatedStringHandler3.AppendFormatted(text);
		defaultInterpolatedStringHandler3.AppendLiteral("' (ID: ");
		defaultInterpolatedStringHandler3.AppendFormatted(viewId);
		defaultInterpolatedStringHandler3.AppendLiteral(")，类别 [");
		defaultInterpolatedStringHandler3.AppendFormatted(string.Join(",", list));
		defaultInterpolatedStringHandler3.AppendLiteral("]，includeTagged=");
		defaultInterpolatedStringHandler3.AppendFormatted(parameter3);
		Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
		HashSet<int> hashSet = new HashSet<int>();
		if (!parameter3)
		{
			IEnumerable<object> tagsInView = iannotationService_0.GetTagsInView(document, obj);
			foreach (object item in tagsInView)
			{
				foreach (int taggedElementId in iannotationService_0.GetTaggedElementIds(item))
				{
					hashSet.Add(taggedElementId);
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(27, 1);
			defaultInterpolatedStringHandler4.AppendLiteral("[CreateTagTool] 视图中已标记 ");
			defaultInterpolatedStringHandler4.AppendFormatted(hashSet.Count);
			defaultInterpolatedStringHandler4.AppendLiteral(" 个元素");
			Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
		}
		List<object> list2 = new List<object>();
		if (list.Count > 0)
		{
			foreach (string item2 in list)
			{
				IEnumerable<object> elementsByCategory = ielementService_0.GetElementsByCategory(document, item2);
				foreach (object item3 in elementsByCategory)
				{
					list2.Add(item3);
				}
			}
		}
		else
		{
			string[] array = new string[15]
			{
				"门",
				"窗",
				"墙",
				"柱",
				"梁",
				"楼板",
				"屋顶",
				"天花板",
				"家具",
				"设备",
				"管道",
				"风管",
				"电缆桥架",
				"线管",
				"水管装置"
			};
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				try
				{
					IEnumerable<object> elementsByCategory2 = ielementService_0.GetElementsByCategory(document, text2);
					foreach (object item4 in elementsByCategory2)
					{
						list2.Add(item4);
					}
				}
				catch
				{
				}
			}
		}
		List<object> list3 = (from object_0 in list2
			group object_0 by ielementService_0.GetElementId(object_0).GetValueOrDefault() into igrouping_0
			where igrouping_0.Key != 0
			select igrouping_0.First()).ToList();
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(26, 1);
		defaultInterpolatedStringHandler5.AppendLiteral("[CreateTagTool] 共找到 ");
		defaultInterpolatedStringHandler5.AppendFormatted(list3.Count);
		defaultInterpolatedStringHandler5.AppendLiteral(" 个候选元素");
		Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
		List<object> list4 = new List<object>();
		int num2 = 0;
		foreach (object item5 in list3)
		{
			int valueOrDefault = ielementService_0.GetElementId(item5).GetValueOrDefault();
			if (valueOrDefault != 0)
			{
				if (!parameter3 && hashSet.Contains(valueOrDefault))
				{
					num2++;
				}
				else
				{
					list4.Add(item5);
				}
			}
		}
		if (list4.Count == 0)
		{
			Logger.Info("[CreateTagTool] 没有需要标记的元素");
			string text3 = "没有需要标记的元素";
			string text4;
			if (num2 <= 0)
			{
				text4 = "";
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler6.AppendLiteral("（已跳过 ");
				defaultInterpolatedStringHandler6.AppendFormatted(num2);
				defaultInterpolatedStringHandler6.AppendLiteral(" 个已标记元素）");
				text4 = defaultInterpolatedStringHandler6.ToStringAndClear();
			}
			return AIToolResult.Ok(text3 + text4, (object)new Class89<string, int?, string, int, int, int, int, List<string>>("batch", viewId, text, list3.Count, 0, num2, 0, (list.Count > 0) ? list : null));
		}
		List<object> list5 = new List<object>();
		List<object> list6 = new List<object>();
		int num3 = 0;
		foreach (object item6 in list4)
		{
			int valueOrDefault2 = ielementService_0.GetElementId(item6).GetValueOrDefault();
			try
			{
				(double, double, double)? elementLocation = ielementService_0.GetElementLocation(item6);
				if (elementLocation.HasValue)
				{
					object obj3 = new
					{
						x = elementLocation.Value.Item1,
						y = elementLocation.Value.Item2,
						z = elementLocation.Value.Item3
					};
					object obj4 = iannotationService_0.CreateTag(document, obj, item6, obj3, parameter, parameter2);
					if (obj4 == null)
					{
						list6.Add(new Class90<int, string>(valueOrDefault2, "CreateTag 返回 null"));
						continue;
					}
					int? elementId = ielementService_0.GetElementId(obj4);
					list5.Add(new Class91<int?, int, string>(elementId, valueOrDefault2, ielementService_0.GetElementCategory(item6)));
					num3++;
				}
				else
				{
					list6.Add(new Class90<int, string>(valueOrDefault2, "无法获取元素位置"));
				}
			}
			catch (Exception ex2)
			{
				list6.Add(new Class90<int, string>(valueOrDefault2, ex2.Message));
			}
		}
		int count = list6.Count;
		int count2 = list4.Count;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(33, 4);
		defaultInterpolatedStringHandler7.AppendLiteral("[CreateTagTool] 批量完成：成功 ");
		defaultInterpolatedStringHandler7.AppendFormatted(num3);
		defaultInterpolatedStringHandler7.AppendLiteral("/");
		defaultInterpolatedStringHandler7.AppendFormatted(count2);
		defaultInterpolatedStringHandler7.AppendLiteral("，跳过 ");
		defaultInterpolatedStringHandler7.AppendFormatted(num2);
		defaultInterpolatedStringHandler7.AppendLiteral("，失败 ");
		defaultInterpolatedStringHandler7.AppendFormatted(count);
		Logger.Info(defaultInterpolatedStringHandler7.ToStringAndClear());
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(11, 2);
		defaultInterpolatedStringHandler8.AppendLiteral("批量标记完成：成功 ");
		defaultInterpolatedStringHandler8.AppendFormatted(num3);
		defaultInterpolatedStringHandler8.AppendLiteral("/");
		defaultInterpolatedStringHandler8.AppendFormatted(count2);
		string text5 = defaultInterpolatedStringHandler8.ToStringAndClear();
		if (num2 > 0)
		{
			string text6 = text5;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(7, 1);
			defaultInterpolatedStringHandler9.AppendLiteral("，跳过已标记 ");
			defaultInterpolatedStringHandler9.AppendFormatted(num2);
			text5 = text6 + defaultInterpolatedStringHandler9.ToStringAndClear();
		}
		if (count > 0)
		{
			string text7 = text5;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(4, 1);
			defaultInterpolatedStringHandler10.AppendLiteral("，失败 ");
			defaultInterpolatedStringHandler10.AppendFormatted(count);
			text5 = text7 + defaultInterpolatedStringHandler10.ToStringAndClear();
		}
		return AIToolResult.Ok(text5, (object)new Class92<string, int?, string, int, int, int, int, int, List<string>, List<object>, List<object>>("batch", viewId, text, list3.Count, count2, num3, num2, count, (list.Count > 0) ? list : null, (list5.Count <= 100) ? list5 : null, (count > 0) ? list6 : null));
	}
}
