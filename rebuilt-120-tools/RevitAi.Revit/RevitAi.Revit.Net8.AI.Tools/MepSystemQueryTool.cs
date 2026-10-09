using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Microsoft.CSharp.RuntimeBinder;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("mep_system_query", Category = "机电查询", Description = "查询 MEP 系统类型信息，包括系统列表、统计、类别分布、尺寸分布、保温状态、使用情况等", RequiresTransaction = false, RequiresModification = false)]
public sealed class MepSystemQueryTool : IAITool
{
	[CompilerGenerated]
	private sealed class Class582
	{
		public MepSystemQueryTool mepSystemQueryTool_0;

		public Document document_0;

		public long long_0;

		internal bool method_0(Pipe pipe_0)
		{
			return mepSystemQueryTool_0.method_22(document_0, pipe_0) == long_0;
		}

		internal bool method_1(FamilyInstance familyInstance_0)
		{
			try
			{
				Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140334L));
				return val != null && val.HasValue && val.AsElementId().Value == long_0;
			}
			catch
			{
				return false;
			}
		}

		internal bool method_2(Duct duct_0)
		{
			return mepSystemQueryTool_0.method_23(duct_0) == long_0;
		}

		internal bool method_3(FamilyInstance familyInstance_0)
		{
			try
			{
				Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140333L));
				return val != null && val.HasValue && val.AsElementId().Value == long_0;
			}
			catch
			{
				return false;
			}
		}
	}

	[CompilerGenerated]
	private sealed class Class583
	{
		public MepSystemQueryTool mepSystemQueryTool_0;

		public Document document_0;

		public long? nullable_0;

		public Dictionary<long, string> dictionary_0;

		internal Class272<long, string, string, int, int, int> method_0(Class271<long, int, int> class271_0)
		{
			string value;
			return new Class272<long, string, string, int, int, int>(class271_0.system_type_id, dictionary_0.TryGetValue(class271_0.system_type_id, out value) ? value : "未知系统", "水管", class271_0.element_count, class271_0.insulated_count, class271_0.element_count - class271_0.insulated_count);
		}
	}

	[CompilerGenerated]
	private sealed class Class584
	{
		public MepSystemQueryTool mepSystemQueryTool_0;

		public long? nullable_0;

		public Document document_0;

		public Dictionary<long, string> dictionary_0;

		internal Class272<long, string, string, int, int, int> method_0(Class271<long, int, int> class271_0)
		{
			string value;
			return new Class272<long, string, string, int, int, int>(class271_0.system_type_id, dictionary_0.TryGetValue(class271_0.system_type_id, out value) ? value : "未知系统", "风管", class271_0.element_count, class271_0.insulated_count, class271_0.element_count - class271_0.insulated_count);
		}
	}

	[CompilerGenerated]
	private sealed class Class585
	{
		public MepSystemQueryTool mepSystemQueryTool_0;

		public Document document_0;

		public long long_0;

		internal bool method_0(Pipe pipe_0)
		{
			return mepSystemQueryTool_0.method_22(document_0, pipe_0) == long_0;
		}

		internal bool method_1(FamilyInstance familyInstance_0)
		{
			try
			{
				Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140334L));
				return val != null && val.HasValue && val.AsElementId().Value == long_0;
			}
			catch
			{
				return false;
			}
		}
	}

	[CompilerGenerated]
	private sealed class Class586
	{
		public MepSystemQueryTool mepSystemQueryTool_0;

		public long long_0;

		internal bool method_0(Duct duct_0)
		{
			return mepSystemQueryTool_0.method_23(duct_0) == long_0;
		}

		internal bool method_1(FamilyInstance familyInstance_0)
		{
			try
			{
				Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140333L));
				return val != null && val.HasValue && val.AsElementId().Value == long_0;
			}
			catch
			{
				return false;
			}
		}
	}

	[CompilerGenerated]
	private sealed class Class587
	{
		public MepSystemQueryTool mepSystemQueryTool_0;

		public Document document_0;

		public long long_0;

		public List<Pipe> list_0;

		internal bool method_0(Pipe pipe_0)
		{
			return mepSystemQueryTool_0.method_22(document_0, pipe_0) == long_0;
		}

		internal Class275<string, int, int> method_1(Class274<string, double, double> class274_0)
		{
			return new Class275<string, int, int>(class274_0.range, list_0.Count((Pipe pipe_0) => ((MEPCurve)pipe_0).Diameter >= class274_0.min && ((MEPCurve)pipe_0).Diameter < class274_0.max), list_0.Count((Pipe pipe_0) => ((MEPCurve)pipe_0).Diameter >= class274_0.min && ((MEPCurve)pipe_0).Diameter < class274_0.max && mepSystemQueryTool_0.method_24(document_0, ((Element)pipe_0).Id)));
		}
	}

	[CompilerGenerated]
	private sealed class Class588
	{
		public Class274<string, double, double> class274_0;

		public Class587 class587_0;

		internal bool method_0(Pipe pipe_0)
		{
			return ((MEPCurve)pipe_0).Diameter >= class274_0.min && ((MEPCurve)pipe_0).Diameter < class274_0.max;
		}

		internal bool method_1(Pipe pipe_0)
		{
			return ((MEPCurve)pipe_0).Diameter >= class274_0.min && ((MEPCurve)pipe_0).Diameter < class274_0.max && class587_0.mepSystemQueryTool_0.method_24(class587_0.document_0, ((Element)pipe_0).Id);
		}
	}

	[CompilerGenerated]
	private sealed class Class589
	{
		public MepSystemQueryTool mepSystemQueryTool_0;

		public long long_0;

		public List<Duct> list_0;

		public Document document_0;

		internal bool method_0(Duct duct_0)
		{
			return mepSystemQueryTool_0.method_23(duct_0) == long_0;
		}

		internal Class275<string, int, int> method_1(Class274<string, double, double> class274_0)
		{
			return new Class275<string, int, int>(class274_0.range, list_0.Count(delegate(Duct duct_0)
			{
				double width = ((MEPCurve)duct_0).Width;
				double height = ((MEPCurve)duct_0).Height;
				double num = Math.Max(width, height);
				return num >= class274_0.min && num < class274_0.max;
			}), list_0.Count(delegate(Duct duct_0)
			{
				double width = ((MEPCurve)duct_0).Width;
				double height = ((MEPCurve)duct_0).Height;
				double num = Math.Max(width, height);
				return num >= class274_0.min && num < class274_0.max && mepSystemQueryTool_0.method_25(document_0, ((Element)duct_0).Id);
			}));
		}
	}

	[CompilerGenerated]
	private sealed class Class590
	{
		public Class274<string, double, double> class274_0;

		public Class589 class589_0;

		internal bool method_0(Duct duct_0)
		{
			double width = ((MEPCurve)duct_0).Width;
			double height = ((MEPCurve)duct_0).Height;
			double num = Math.Max(width, height);
			return num >= class274_0.min && num < class274_0.max;
		}

		internal bool method_1(Duct duct_0)
		{
			double width = ((MEPCurve)duct_0).Width;
			double height = ((MEPCurve)duct_0).Height;
			double num = Math.Max(width, height);
			return num >= class274_0.min && num < class274_0.max && class589_0.mepSystemQueryTool_0.method_25(class589_0.document_0, ((Element)duct_0).Id);
		}
	}

	[CompilerGenerated]
	private sealed class Class591
	{
		public MepSystemQueryTool mepSystemQueryTool_0;

		public Document document_0;

		public Dictionary<long, int> dictionary_0;

		internal long method_0(Pipe pipe_0)
		{
			return mepSystemQueryTool_0.method_22(document_0, pipe_0);
		}

		internal Class276<long, string, bool, int, bool, bool, bool, bool> method_1(PipingSystemType pipingSystemType_0)
		{
			int value;
			return new Class276<long, string, bool, int, bool, bool, bool, bool>(((Element)pipingSystemType_0).Id.Value, ((Element)pipingSystemType_0).Name, dictionary_0.ContainsKey(((Element)pipingSystemType_0).Id.Value) && dictionary_0[((Element)pipingSystemType_0).Id.Value] > 0, dictionary_0.TryGetValue(((Element)pipingSystemType_0).Id.Value, out value) ? value : 0, ((ElementType)pipingSystemType_0).CanBeDeleted, ((ElementType)pipingSystemType_0).CanBeRenamed, ((ElementType)pipingSystemType_0).CanBeCopied, gparam_15: false);
		}
	}

	[CompilerGenerated]
	private sealed class Class592
	{
		public MepSystemQueryTool mepSystemQueryTool_0;

		public Dictionary<long, int> dictionary_0;

		internal long method_0(Duct duct_0)
		{
			return mepSystemQueryTool_0.method_23(duct_0);
		}

		internal Class276<long, string, bool, int, bool, bool, bool, bool> method_1(MEPSystemType mepsystemType_0)
		{
			int value;
			return new Class276<long, string, bool, int, bool, bool, bool, bool>(((Element)mepsystemType_0).Id.Value, ((Element)mepsystemType_0).Name, dictionary_0.ContainsKey(((Element)mepsystemType_0).Id.Value) && dictionary_0[((Element)mepsystemType_0).Id.Value] > 0, dictionary_0.TryGetValue(((Element)mepsystemType_0).Id.Value, out value) ? value : 0, ((ElementType)mepsystemType_0).CanBeDeleted, ((ElementType)mepsystemType_0).CanBeRenamed, ((ElementType)mepsystemType_0).CanBeCopied, gparam_15: false);
		}
	}

	[CompilerGenerated]
	private sealed class Class593
	{
		public MepSystemQueryTool mepSystemQueryTool_0;

		public ElementId elementId_0;

		internal bool method_0(PipeInsulation pipeInsulation_0)
		{
			try
			{
				ElementId val = mepSystemQueryTool_0.method_26(pipeInsulation_0);
				return val == elementId_0;
			}
			catch
			{
				return false;
			}
		}
	}

	[CompilerGenerated]
	private sealed class Class594
	{
		public MepSystemQueryTool mepSystemQueryTool_0;

		public ElementId elementId_0;

		internal bool method_0(DuctInsulation ductInsulation_0)
		{
			try
			{
				ElementId val = mepSystemQueryTool_0.method_27(ductInsulation_0);
				return val == elementId_0;
			}
			catch
			{
				return false;
			}
		}
	}

	[CompilerGenerated]
	private static class Class595
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, int, object>> callSite_1;

		public static CallSite<Func<CallSite, object, bool>> callSite_2;
	}

	[CompilerGenerated]
	private static class Class596
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, bool, object>> callSite_1;

		public static CallSite<Func<CallSite, object, bool>> callSite_2;
	}

	[CompilerGenerated]
	private static class Class597
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, int, object>> callSite_1;

		public static CallSite<Func<CallSite, object, bool>> callSite_2;

		public static CallSite<Func<CallSite, object, object>> callSite_3;

		public static CallSite<Func<CallSite, object, object>> callSite_4;

		public static CallSite<Func<CallSite, object, object>> callSite_5;

		public static CallSite<Func<CallSite, object, object>> callSite_6;

		public static CallSite<Func<CallSite, object, object>> callSite_7;

		public static CallSite<Func<CallSite, object, object>> callSite_8;

		public static CallSite<Func<CallSite, object, int, object>> callSite_9;

		public static CallSite<Func<CallSite, object, bool>> callSite_10;

		public static CallSite<Func<CallSite, object, object>> callSite_11;

		public static CallSite<Func<CallSite, object, double>> callSite_12;

		public static CallSite<Func<CallSite, object, object>> callSite_13;

		public static CallSite<Func<CallSite, double, object, object>> callSite_14;

		public static CallSite<Func<CallSite, object, int, object>> callSite_15;

		public static CallSite<Func<CallSite, Type, object, int, object>> callSite_16;
	}

	[CompilerGenerated]
	private static class Class598
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, int, object>> callSite_1;

		public static CallSite<Func<CallSite, object, bool>> callSite_2;

		public static CallSite<Func<CallSite, object, object>> callSite_3;

		public static CallSite<Func<CallSite, object, object>> callSite_4;

		public static CallSite<Func<CallSite, object, object>> callSite_5;

		public static CallSite<Func<CallSite, object, object>> callSite_6;

		public static CallSite<Func<CallSite, object, object>> callSite_7;

		public static CallSite<Func<CallSite, object, object>> callSite_8;

		public static CallSite<Func<CallSite, object, int, object>> callSite_9;

		public static CallSite<Func<CallSite, object, bool>> callSite_10;

		public static CallSite<Func<CallSite, object, object>> callSite_11;

		public static CallSite<Func<CallSite, object, double>> callSite_12;

		public static CallSite<Func<CallSite, object, object>> callSite_13;

		public static CallSite<Func<CallSite, double, object, object>> callSite_14;

		public static CallSite<Func<CallSite, object, int, object>> callSite_15;

		public static CallSite<Func<CallSite, Type, object, int, object>> callSite_16;
	}

	[CompilerGenerated]
	private sealed class Class599 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public string string_0;

		public MepSystemQueryTool mepSystemQueryTool_0;

		private List<object> list_0;

		private int int_1;

		private IEnumerable<object> ienumerable_0;

		private IEnumerable<object> ienumerable_1;

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
					Class599 stateMachine = this;
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
			list_0 = new List<object>();
			if (string_0 == "pipe" || string_0 == "all")
			{
				ienumerable_0 = mepSystemQueryTool_0.method_20(document_0);
				list_0.AddRange(ienumerable_0);
				ienumerable_0 = null;
			}
			if (string_0 == "duct" || string_0 == "all")
			{
				ienumerable_1 = mepSystemQueryTool_0.method_21(document_0);
				list_0.AddRange(ienumerable_1);
				ienumerable_1 = null;
			}
			int_1 = list_0.Count(delegate(object object_0)
			{
				if (Class596.callSite_0 == null)
				{
					Class596.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "is_in_use", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				return (dynamic)Class596.callSite_0.Target(Class596.callSite_0, object_0) == false;
			});
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[MepSystemQueryTool] CheckUsage: 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个系统，其中 ");
			defaultInterpolatedStringHandler.AppendFormatted(int_1);
			defaultInterpolatedStringHandler.AppendLiteral(" 个未使用");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(23, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("成功检查 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个系统的使用情况，其中 ");
			defaultInterpolatedStringHandler2.AppendFormatted(int_1);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个未使用");
			AIToolResult result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class267<int, int, List<object>>(list_0.Count, int_1, list_0));
			int_0 = -2;
			list_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	private sealed class Class600 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public MepSystemQueryTool mepSystemQueryTool_0;

		private Document document_0;

		private string string_0;

		private string string_1;

		private string string_2;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private AIToolResult aitoolResult_2;

		private AIToolResult aitoolResult_3;

		private AIToolResult aitoolResult_4;

		private AIToolResult aitoolResult_5;

		private AIToolResult aitoolResult_6;

		private AIToolResult aitoolResult_7;

		private AIToolResult aitoolResult_8;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if ((uint)(num - 1) <= 8u)
				{
					goto IL_006e;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class600 stateMachine = this;
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
				TaskAwaiter<AIToolResult> awaiter6;
				TaskAwaiter<AIToolResult> awaiter8;
				TaskAwaiter<AIToolResult> awaiter2;
				TaskAwaiter<AIToolResult> awaiter3;
				TaskAwaiter<AIToolResult> awaiter5;
				TaskAwaiter<AIToolResult> awaiter7;
				TaskAwaiter<AIToolResult> awaiter10;
				TaskAwaiter<AIToolResult> awaiter9;
				AIToolResult val;
				switch (num)
				{
				default:
					if (aitoolContext_0.Document == null)
					{
						result = AIToolResult.Fail("文档对象为空");
					}
					else
					{
						object document = aitoolContext_0.Document;
						document_0 = (Document)((document is Document) ? document : null);
						if (document_0 == null)
						{
							result = AIToolResult.Fail("文档对象类型不正确");
						}
						else
						{
							string_0 = aitoolContext_0.GetParameter<string>("operation", (string)null);
							if (!string.IsNullOrEmpty(string_0))
							{
								string_1 = aitoolContext_0.GetParameter<string>("target", (string)null) ?? "all";
								Logger.Info("[MepSystemQueryTool] 执行操作: " + string_0 + ", target: " + string_1);
								string_2 = string_0.ToLower();
								string text = string_2;
								uint num2 = Class653.smethod_0(text);
								if (num2 <= 2152549505u)
								{
									if (num2 <= 1757870687)
									{
										if (num2 != 546856658)
										{
											if (num2 == 1757870687 && text == "check_usage")
											{
												awaiter4 = mepSystemQueryTool_0.method_6(document_0, string_1).GetAwaiter();
												if (!awaiter4.IsCompleted)
												{
													num = 7;
													int_0 = 7;
													taskAwaiter_1 = awaiter4;
													Class600 stateMachine = this;
													asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
													return;
												}
												goto IL_07b3;
											}
										}
										else if (text == "insulation_status")
										{
											awaiter6 = mepSystemQueryTool_0.method_4(document_0, string_1, aitoolContext_0).GetAwaiter();
											if (!awaiter6.IsCompleted)
											{
												num = 5;
												int_0 = 5;
												taskAwaiter_1 = awaiter6;
												Class600 stateMachine = this;
												asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter6, ref stateMachine);
												return;
											}
											goto IL_0737;
										}
									}
									else if (num2 != 2027355209)
									{
										if (num2 == 2152549505u && text == "category_breakdown")
										{
											awaiter8 = mepSystemQueryTool_0.method_2(document_0, string_1, aitoolContext_0).GetAwaiter();
											if (!awaiter8.IsCompleted)
											{
												num = 3;
												int_0 = 3;
												taskAwaiter_1 = awaiter8;
												Class600 stateMachine = this;
												asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter8, ref stateMachine);
												return;
											}
											goto IL_06bb;
										}
									}
									else if (text == "get_elements")
									{
										awaiter2 = mepSystemQueryTool_0.method_8(document_0, string_1, aitoolContext_0).GetAwaiter();
										if (!awaiter2.IsCompleted)
										{
											num = 9;
											int_0 = 9;
											taskAwaiter_1 = awaiter2;
											Class600 stateMachine = this;
											asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
											return;
										}
										goto IL_0829;
									}
								}
								else if (num2 <= 2360751069u)
								{
									if (num2 != 2182176033u)
									{
										if (num2 == 2360751069u && text == "get_properties")
										{
											awaiter3 = mepSystemQueryTool_0.method_7(document_0, string_1, aitoolContext_0).GetAwaiter();
											if (!awaiter3.IsCompleted)
											{
												num = 8;
												int_0 = 8;
												taskAwaiter_1 = awaiter3;
												Class600 stateMachine = this;
												asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
												return;
											}
											goto IL_07ee;
										}
									}
									else if (text == "system_details")
									{
										awaiter5 = mepSystemQueryTool_0.method_5(document_0, string_1, aitoolContext_0).GetAwaiter();
										if (!awaiter5.IsCompleted)
										{
											num = 6;
											int_0 = 6;
											taskAwaiter_1 = awaiter5;
											Class600 stateMachine = this;
											asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
											return;
										}
										goto IL_0775;
									}
								}
								else if (num2 != 2854105798u)
								{
									if (num2 != 3220147184u)
									{
										if (num2 == 3778501695u && text == "size_distribution")
										{
											awaiter7 = mepSystemQueryTool_0.method_3(document_0, string_1, aitoolContext_0).GetAwaiter();
											if (!awaiter7.IsCompleted)
											{
												num = 4;
												int_0 = 4;
												taskAwaiter_1 = awaiter7;
												Class600 stateMachine = this;
												asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter7, ref stateMachine);
												return;
											}
											goto IL_06f9;
										}
									}
									else if (text == "list_systems")
									{
										awaiter10 = mepSystemQueryTool_0.method_0(document_0, string_1).GetAwaiter();
										if (!awaiter10.IsCompleted)
										{
											num = 1;
											int_0 = 1;
											taskAwaiter_1 = awaiter10;
											Class600 stateMachine = this;
											asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter10, ref stateMachine);
											return;
										}
										goto IL_063f;
									}
								}
								else if (text == "system_statistics")
								{
									awaiter9 = mepSystemQueryTool_0.method_1(document_0, string_1, aitoolContext_0).GetAwaiter();
									if (!awaiter9.IsCompleted)
									{
										num = 2;
										int_0 = 2;
										taskAwaiter_1 = awaiter9;
										Class600 stateMachine = this;
										asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter9, ref stateMachine);
										return;
									}
									goto IL_067d;
								}
								val = AIToolResult.Fail("不支持的操作类型: " + string_0);
								break;
							}
							result = AIToolResult.Fail("必须指定 operation 参数");
						}
					}
					goto end_IL_006e;
				case 1:
					awaiter10 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_063f;
				case 2:
					awaiter9 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_067d;
				case 3:
					awaiter8 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_06bb;
				case 4:
					awaiter7 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_06f9;
				case 5:
					awaiter6 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0737;
				case 6:
					awaiter5 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0775;
				case 7:
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_07b3;
				case 8:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_07ee;
				case 9:
					{
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						goto IL_0829;
					}
					IL_063f:
					aitoolResult_0 = awaiter10.GetResult();
					val = aitoolResult_0;
					aitoolResult_0 = null;
					break;
					IL_067d:
					aitoolResult_1 = awaiter9.GetResult();
					val = aitoolResult_1;
					aitoolResult_1 = null;
					break;
					IL_06bb:
					aitoolResult_2 = awaiter8.GetResult();
					val = aitoolResult_2;
					aitoolResult_2 = null;
					break;
					IL_0775:
					aitoolResult_5 = awaiter5.GetResult();
					val = aitoolResult_5;
					aitoolResult_5 = null;
					break;
					IL_0829:
					aitoolResult_8 = awaiter2.GetResult();
					val = aitoolResult_8;
					aitoolResult_8 = null;
					break;
					IL_07b3:
					aitoolResult_6 = awaiter4.GetResult();
					val = aitoolResult_6;
					aitoolResult_6 = null;
					break;
					IL_07ee:
					aitoolResult_7 = awaiter3.GetResult();
					val = aitoolResult_7;
					aitoolResult_7 = null;
					break;
					IL_06f9:
					aitoolResult_3 = awaiter7.GetResult();
					val = aitoolResult_3;
					aitoolResult_3 = null;
					break;
					IL_0737:
					aitoolResult_4 = awaiter6.GetResult();
					val = aitoolResult_4;
					aitoolResult_4 = null;
					break;
				}
				result = val;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[MepSystemQueryTool] 执行失败: " + exception_0.Message);
				result = AIToolResult.Fail("查询失败: " + exception_0.Message);
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
	private sealed class Class601 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public string string_0;

		public AIToolContext aitoolContext_0;

		public MepSystemQueryTool mepSystemQueryTool_0;

		private long long_0;

		private object object_0;

		private object object_1;

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
					Class601 stateMachine = this;
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
			if (string_0 != "pipe" && string_0 != "duct")
			{
				result = AIToolResult.Fail("category_breakdown 操作必须指定 target 为 pipe 或 duct（不支持 all）");
			}
			else if (!aitoolContext_0.HasParameter("system_type_id"))
			{
				result = AIToolResult.Fail("category_breakdown 操作需要指定 system_type_id 参数");
			}
			else
			{
				long_0 = aitoolContext_0.GetParameter<long>("system_type_id", 0L);
				try
				{
					if (string_0 == "pipe")
					{
						object_0 = mepSystemQueryTool_0.method_14(document_0, long_0);
						result = AIToolResult.Ok("成功获取水管系统的类别分布", (object)new Class263<object>(object_0));
					}
					else if (string_0 == "duct")
					{
						object_1 = mepSystemQueryTool_0.method_15(document_0, long_0);
						result = AIToolResult.Ok("成功获取风管系统的类别分布", (object)new Class263<object>(object_1));
					}
					else
					{
						result = AIToolResult.Fail("category_breakdown 操作必须指定 target 为 pipe 或 duct");
					}
				}
				catch (Exception ex)
				{
					exception_0 = ex;
					Logger.Error("[MepSystemQueryTool] CategoryBreakdown: " + exception_0.Message);
					result = AIToolResult.Fail("获取类别分布失败: " + exception_0.Message);
				}
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
	private sealed class Class602 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public string string_0;

		public AIToolContext aitoolContext_0;

		public MepSystemQueryTool mepSystemQueryTool_0;

		private Class582 class582_0;

		private bool bool_0;

		private int int_1;

		private MEPSystemType mepsystemType_0;

		private List<int> list_0;

		private int int_2;

		private int int_3;

		private List<object> list_1;

		private List<int> list_2;

		private List<int> list_3;

		private List<int> list_4;

		private List<int> list_5;

		private List<int>.Enumerator enumerator_0;

		private int int_4;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private PropertyInfo propertyInfo_0;

		private object object_0;

		private PropertyInfo propertyInfo_1;

		private string string_2;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Expected O, but got Unknown
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				class582_0 = new Class582();
				class582_0.mepSystemQueryTool_0 = mepSystemQueryTool_0;
				class582_0.document_0 = document_0;
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class602 stateMachine = this;
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
			if (!aitoolContext_0.HasParameter("system_type_id"))
			{
				result = AIToolResult.Fail("get_elements 操作需要指定 system_type_id 参数");
			}
			else if (string_0 != "pipe" && string_0 != "duct")
			{
				result = AIToolResult.Fail("get_elements 操作必须指定 target 为 pipe 或 duct（不支持 all）");
			}
			else
			{
				class582_0.long_0 = aitoolContext_0.GetParameter<long>("system_type_id", 0L);
				bool_0 = aitoolContext_0.GetParameter<bool>("include_fittings", true);
				int_1 = aitoolContext_0.GetParameter<int>("limit", 100);
				try
				{
					Element element = class582_0.document_0.GetElement(new ElementId(class582_0.long_0));
					mepsystemType_0 = (MEPSystemType)(object)((element is MEPSystemType) ? element : null);
					if (mepsystemType_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到系统类型 ID: ");
						defaultInterpolatedStringHandler.AppendFormatted(class582_0.long_0);
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						list_0 = new List<int>();
						int_2 = 0;
						if (string_0 == "pipe")
						{
							list_2 = (from Pipe pipe_0 in (IEnumerable)new FilteredElementCollector(class582_0.document_0).OfClass(typeof(Pipe))
								where class582_0.mepSystemQueryTool_0.method_22(class582_0.document_0, pipe_0) == class582_0.long_0
								select (int)((Element)pipe_0).Id.Value).ToList();
							list_0.AddRange(list_2);
							int_2 = list_2.Count;
							if (bool_0)
							{
								list_3 = (from familyInstance_0 in ((IEnumerable)new FilteredElementCollector(class582_0.document_0).OfClass(typeof(FamilyInstance))).Cast<FamilyInstance>().Where(delegate(FamilyInstance familyInstance_0)
									{
										try
										{
											Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140334L));
											return val != null && val.HasValue && val.AsElementId().Value == class582_0.long_0;
										}
										catch
										{
											return false;
										}
									})
									select (int)((Element)familyInstance_0).Id.Value).ToList();
								list_0.AddRange(list_3);
								int_2 += list_3.Count;
								list_3 = null;
							}
							list_2 = null;
						}
						else
						{
							list_4 = (from Duct duct_0 in (IEnumerable)new FilteredElementCollector(class582_0.document_0).OfClass(typeof(Duct))
								where class582_0.mepSystemQueryTool_0.method_23(duct_0) == class582_0.long_0
								select (int)((Element)duct_0).Id.Value).ToList();
							list_0.AddRange(list_4);
							int_2 = list_4.Count;
							if (bool_0)
							{
								list_5 = (from familyInstance_0 in ((IEnumerable)new FilteredElementCollector(class582_0.document_0).OfClass(typeof(FamilyInstance))).Cast<FamilyInstance>().Where(delegate(FamilyInstance familyInstance_0)
									{
										try
										{
											Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140333L));
											return val != null && val.HasValue && val.AsElementId().Value == class582_0.long_0;
										}
										catch
										{
											return false;
										}
									})
									select (int)((Element)familyInstance_0).Id.Value).ToList();
								list_0.AddRange(list_5);
								int_2 += list_5.Count;
								list_5 = null;
							}
							list_4 = null;
						}
						int_3 = list_0.Count;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(51, 3);
						defaultInterpolatedStringHandler2.AppendLiteral("[MepSystemQueryTool] GetElements: 系统 ");
						defaultInterpolatedStringHandler2.AppendFormatted(((Element)mepsystemType_0).Name);
						defaultInterpolatedStringHandler2.AppendLiteral(" 总计 ");
						defaultInterpolatedStringHandler2.AppendFormatted(int_2);
						defaultInterpolatedStringHandler2.AppendLiteral(" 个元素，返回 ");
						defaultInterpolatedStringHandler2.AppendFormatted(int_3);
						defaultInterpolatedStringHandler2.AppendLiteral(" 个");
						Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
						list_1 = new List<object>();
						enumerator_0 = list_0.GetEnumerator();
						try
						{
							while (enumerator_0.MoveNext())
							{
								int_4 = enumerator_0.Current;
								List<object> list = list_1;
								int gparam_ = int_4;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(3, 1);
								defaultInterpolatedStringHandler3.AppendLiteral("元素_");
								defaultInterpolatedStringHandler3.AppendFormatted(int_4);
								list.Add(new Class1<int, string, string>(gparam_, defaultInterpolatedStringHandler3.ToStringAndClear(), (string_0 == "pipe") ? "管道" : "风管"));
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
							}
						}
						enumerator_0 = default(List<int>.Enumerator);
						if (!string.IsNullOrEmpty(aitoolContext_0.SessionId) && aitoolContext_0.DataCache != null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(13, 3);
							defaultInterpolatedStringHandler4.AppendLiteral("mep_system_");
							defaultInterpolatedStringHandler4.AppendFormatted(((Element)mepsystemType_0).Name);
							defaultInterpolatedStringHandler4.AppendLiteral("_");
							defaultInterpolatedStringHandler4.AppendFormatted(string_0);
							defaultInterpolatedStringHandler4.AppendLiteral("_");
							defaultInterpolatedStringHandler4.AppendFormatted(DateTime.Now.Ticks);
							string_1 = defaultInterpolatedStringHandler4.ToStringAndClear();
							aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)list_1, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_1, ((Element)mepsystemType_0).Name, 200);
							if (aitoolResult_0.Data != null)
							{
								try
								{
									propertyInfo_0 = aitoolResult_0.Data.GetType().GetProperty("cache_info");
									if (propertyInfo_0 != null)
									{
										object_0 = propertyInfo_0.GetValue(aitoolResult_0.Data);
										if (object_0 != null)
										{
											propertyInfo_1 = object_0.GetType().GetProperty("cache_id");
											if (propertyInfo_1 != null)
											{
												string_2 = propertyInfo_1.GetValue(object_0)?.ToString();
												if (!string.IsNullOrEmpty(string_2))
												{
													AIToolResult obj = aitoolResult_0;
													DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(54, 3);
													defaultInterpolatedStringHandler5.AppendLiteral("✅ 成功获取系统 '");
													defaultInterpolatedStringHandler5.AppendFormatted(((Element)mepsystemType_0).Name);
													defaultInterpolatedStringHandler5.AppendLiteral("' 的 ");
													defaultInterpolatedStringHandler5.AppendFormatted(int_2);
													defaultInterpolatedStringHandler5.AppendLiteral(" 个元素\n\n💡 在后续工具调用中使用 cacheId=\"");
													defaultInterpolatedStringHandler5.AppendFormatted(string_2);
													defaultInterpolatedStringHandler5.AppendLiteral("\" 参数来操作这些元素");
													obj.Message = defaultInterpolatedStringHandler5.ToStringAndClear();
												}
												string_2 = null;
											}
											propertyInfo_1 = null;
										}
										object_0 = null;
									}
									propertyInfo_0 = null;
								}
								catch
								{
								}
							}
							result = aitoolResult_0;
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(28, 3);
							defaultInterpolatedStringHandler6.AppendLiteral("成功获取系统 '");
							defaultInterpolatedStringHandler6.AppendFormatted(((Element)mepsystemType_0).Name);
							defaultInterpolatedStringHandler6.AppendLiteral("' 的 ");
							defaultInterpolatedStringHandler6.AppendFormatted(int_2);
							defaultInterpolatedStringHandler6.AppendLiteral(" 个元素，已返回 ");
							defaultInterpolatedStringHandler6.AppendFormatted(int_3);
							defaultInterpolatedStringHandler6.AppendLiteral(" 个元素 ID");
							result = AIToolResult.Ok(defaultInterpolatedStringHandler6.ToStringAndClear(), (object)new Class270<long, string, string, int, int, List<int>, List<object>>(class582_0.long_0, ((Element)mepsystemType_0).Name, string_0, int_2, int_3, list_0, list_1));
						}
					}
				}
				catch (Exception ex)
				{
					exception_0 = ex;
					Logger.Error("[MepSystemQueryTool] GetElements: " + exception_0.Message);
					result = AIToolResult.Fail("获取系统元素失败: " + exception_0.Message);
				}
			}
			int_0 = -2;
			class582_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	private sealed class Class603 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public string string_0;

		public AIToolContext aitoolContext_0;

		public MepSystemQueryTool mepSystemQueryTool_0;

		private bool bool_0;

		private List<object> list_0;

		private int int_1;

		private int int_2;

		private IEnumerable<object> ienumerable_0;

		private IEnumerable<object> ienumerable_1;

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
					Class603 stateMachine = this;
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
			bool_0 = aitoolContext_0.GetParameter<bool>("only_uninsulated", false);
			list_0 = new List<object>();
			if (string_0 == "pipe" || string_0 == "all")
			{
				ienumerable_0 = mepSystemQueryTool_0.method_18(document_0, bool_0);
				list_0.AddRange(ienumerable_0);
				ienumerable_0 = null;
			}
			if (string_0 == "duct" || string_0 == "all")
			{
				ienumerable_1 = mepSystemQueryTool_0.method_19(document_0, bool_0);
				list_0.AddRange(ienumerable_1);
				ienumerable_1 = null;
			}
			int_1 = list_0.Count;
			int_2 = list_0.Count(delegate(object object_0)
			{
				if (Class595.callSite_0 == null)
				{
					Class595.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "insulated_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				return (dynamic)Class595.callSite_0.Target(Class595.callSite_0, object_0) == 0;
			});
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[MepSystemQueryTool] InsulationStatus: 返回 ");
			defaultInterpolatedStringHandler.AppendFormatted(int_1);
			defaultInterpolatedStringHandler.AppendLiteral(" 个系统，其中 ");
			defaultInterpolatedStringHandler.AppendFormatted(int_2);
			defaultInterpolatedStringHandler.AppendLiteral(" 个未保温");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(25, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("成功获取 ");
			defaultInterpolatedStringHandler2.AppendFormatted(int_1);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个系统的保温状态，其中 ");
			defaultInterpolatedStringHandler2.AppendFormatted(int_2);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个系统未保温");
			AIToolResult result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class265<int, int, List<object>>(int_1, int_2, list_0));
			int_0 = -2;
			list_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	private sealed class Class604 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public string string_0;

		public AIToolContext aitoolContext_0;

		public MepSystemQueryTool mepSystemQueryTool_0;

		private long long_0;

		private MEPSystemType mepsystemType_0;

		private Class268<long, string, string, string, SystemCalculationLevel, bool, bool, bool, Class269<object, int, long?, object, long?, bool, long?>> class268_0;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Expected O, but got Unknown
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class604 stateMachine = this;
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
			if (!aitoolContext_0.HasParameter("system_type_id"))
			{
				result = AIToolResult.Fail("get_properties 操作需要指定 system_type_id 参数");
			}
			else
			{
				long_0 = aitoolContext_0.GetParameter<long>("system_type_id", 0L);
				try
				{
					Element element = document_0.GetElement(new ElementId(long_0));
					mepsystemType_0 = (MEPSystemType)(object)((element is MEPSystemType) ? element : null);
					if (mepsystemType_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到系统类型 ID: ");
						defaultInterpolatedStringHandler.AppendFormatted(long_0);
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						long gparam_ = long_0;
						string name = ((Element)mepsystemType_0).Name;
						string gparam_2 = mepSystemQueryTool_0.method_28(mepsystemType_0);
						string? gparam_3 = ((object)mepsystemType_0.SystemClassification/*cast due to constrained. prefix*/).ToString();
						SystemCalculationLevel calculationLevel = mepsystemType_0.CalculationLevel;
						bool canBeCopied = ((ElementType)mepsystemType_0).CanBeCopied;
						bool canBeDeleted = ((ElementType)mepsystemType_0).CanBeDeleted;
						bool canBeRenamed = ((ElementType)mepsystemType_0).CanBeRenamed;
						object? gparam_4 = mepSystemQueryTool_0.method_11(mepsystemType_0.LineColor);
						int lineWeight = mepsystemType_0.LineWeight;
						ElementId linePatternId = mepsystemType_0.LinePatternId;
						long? gparam_5 = ((linePatternId != null) ? new long?(linePatternId.Value) : ((long?)null));
						object? gparam_6 = mepSystemQueryTool_0.method_11(mepsystemType_0.FillColor);
						ElementId fillPatternId = mepsystemType_0.FillPatternId;
						long? gparam_7 = ((fillPatternId != null) ? new long?(fillPatternId.Value) : ((long?)null));
						bool fillVisible = mepsystemType_0.FillVisible;
						ElementId materialId = mepsystemType_0.MaterialId;
						class268_0 = new Class268<long, string, string, string, SystemCalculationLevel, bool, bool, bool, Class269<object, int, long?, object, long?, bool, long?>>(gparam_, name, gparam_2, gparam_3, calculationLevel, canBeCopied, canBeDeleted, canBeRenamed, new Class269<object, int, long?, object, long?, bool, long?>(gparam_4, lineWeight, gparam_5, gparam_6, gparam_7, fillVisible, (materialId != null) ? new long?(materialId.Value) : ((long?)null)));
						Logger.Info("[MepSystemQueryTool] GetProperties: 成功获取系统 " + ((Element)mepsystemType_0).Name + " 的属性");
						result = AIToolResult.Ok("成功获取系统 '" + ((Element)mepsystemType_0).Name + "' 的属性", (object)class268_0);
					}
				}
				catch (Exception ex)
				{
					exception_0 = ex;
					Logger.Error("[MepSystemQueryTool] GetProperties: " + exception_0.Message);
					result = AIToolResult.Fail("获取系统属性失败: " + exception_0.Message);
				}
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
	private sealed class Class605 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public string string_0;

		public AIToolContext aitoolContext_0;

		public MepSystemQueryTool mepSystemQueryTool_0;

		private long long_0;

		private object object_0;

		private object object_1;

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
					Class605 stateMachine = this;
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
			if (!aitoolContext_0.HasParameter("system_type_id"))
			{
				result = AIToolResult.Fail("size_distribution 操作需要指定 system_type_id 参数");
			}
			else
			{
				long_0 = aitoolContext_0.GetParameter<long>("system_type_id", 0L);
				try
				{
					if (string_0 == "pipe")
					{
						object_0 = mepSystemQueryTool_0.method_16(document_0, long_0);
						result = AIToolResult.Ok("成功获取水管系统的尺寸分布", (object)new Class264<object>(object_0));
					}
					else if (string_0 == "duct")
					{
						object_1 = mepSystemQueryTool_0.method_17(document_0, long_0);
						result = AIToolResult.Ok("成功获取风管系统的尺寸分布", (object)new Class264<object>(object_1));
					}
					else
					{
						result = AIToolResult.Fail("size_distribution 操作必须指定 target 为 pipe 或 duct");
					}
				}
				catch (Exception ex)
				{
					exception_0 = ex;
					Logger.Error("[MepSystemQueryTool] SizeDistribution: " + exception_0.Message);
					result = AIToolResult.Fail("获取尺寸分布失败: " + exception_0.Message);
				}
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
	private sealed class Class606 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public string string_0;

		public AIToolContext aitoolContext_0;

		public MepSystemQueryTool mepSystemQueryTool_0;

		private long long_0;

		private bool bool_0;

		private bool bool_1;

		private MEPSystemType mepsystemType_0;

		private bool bool_2;

		private string string_1;

		private Class266<long, string, string, string, string> class266_0;

		private Dictionary<string, object> dictionary_0;

		private Dictionary<string, object> dictionary_1;

		private object object_0;

		private object object_1;

		private Dictionary<string, object>.Enumerator enumerator_0;

		private KeyValuePair<string, object> keyValuePair_0;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Expected O, but got Unknown
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
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
					Class606 stateMachine = this;
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
			if (!aitoolContext_0.HasParameter("system_type_id"))
			{
				result = AIToolResult.Fail("system_details 操作需要指定 system_type_id 参数");
			}
			else
			{
				long_0 = aitoolContext_0.GetParameter<long>("system_type_id", 0L);
				bool_0 = aitoolContext_0.GetParameter<bool>("include_size_distribution", false);
				bool_1 = aitoolContext_0.GetParameter<bool>("include_category_breakdown", false);
				try
				{
					Element element = document_0.GetElement(new ElementId(long_0));
					mepsystemType_0 = (MEPSystemType)(object)((element is MEPSystemType) ? element : null);
					if (mepsystemType_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到系统类型 ID: ");
						defaultInterpolatedStringHandler.AppendFormatted(long_0);
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						bool_2 = mepsystemType_0 is PipingSystemType || mepSystemQueryTool_0.method_9(mepsystemType_0);
						string_1 = mepSystemQueryTool_0.method_28(mepsystemType_0);
						class266_0 = new Class266<long, string, string, string, string>(long_0, ((Element)mepsystemType_0).Name, ((object)mepsystemType_0.SystemClassification/*cast due to constrained. prefix*/).ToString(), bool_2 ? "水管" : "风管", string_1);
						dictionary_0 = new Dictionary<string, object>();
						if (bool_2)
						{
							object_0 = mepSystemQueryTool_0.method_12(document_0, long_0).FirstOrDefault();
							if (object_0 != null)
							{
								dictionary_0["statistics"] = object_0;
							}
							if (bool_1)
							{
								dictionary_0["category_breakdown"] = mepSystemQueryTool_0.method_14(document_0, long_0);
							}
							if (bool_0)
							{
								dictionary_0["size_distribution"] = mepSystemQueryTool_0.method_16(document_0, long_0);
							}
							object_0 = null;
						}
						else
						{
							object_1 = mepSystemQueryTool_0.method_13(document_0, long_0).FirstOrDefault();
							if (object_1 != null)
							{
								dictionary_0["statistics"] = object_1;
							}
							if (bool_1)
							{
								dictionary_0["category_breakdown"] = mepSystemQueryTool_0.method_15(document_0, long_0);
							}
							if (bool_0)
							{
								dictionary_0["size_distribution"] = mepSystemQueryTool_0.method_17(document_0, long_0);
							}
							object_1 = null;
						}
						dictionary_1 = new Dictionary<string, object> { ["details"] = class266_0 };
						enumerator_0 = dictionary_0.GetEnumerator();
						try
						{
							while (enumerator_0.MoveNext())
							{
								keyValuePair_0 = enumerator_0.Current;
								dictionary_1[keyValuePair_0.Key] = keyValuePair_0.Value;
								keyValuePair_0 = default(KeyValuePair<string, object>);
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
							}
						}
						enumerator_0 = default(Dictionary<string, object>.Enumerator);
						Logger.Info("[MepSystemQueryTool] SystemDetails: 成功获取系统 " + ((Element)mepsystemType_0).Name + " 的详细信息");
						result = AIToolResult.Ok("成功获取系统 '" + ((Element)mepsystemType_0).Name + "' 的详细信息", (object)dictionary_1);
					}
				}
				catch (Exception ex)
				{
					exception_0 = ex;
					Logger.Error("[MepSystemQueryTool] SystemDetails: " + exception_0.Message);
					result = AIToolResult.Fail("获取系统详情失败: " + exception_0.Message);
				}
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
	private sealed class Class607 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public string string_0;

		public AIToolContext aitoolContext_0;

		public MepSystemQueryTool mepSystemQueryTool_0;

		private long? nullable_0;

		private List<object> list_0;

		private IEnumerable<object> ienumerable_0;

		private IEnumerable<object> ienumerable_1;

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
					Class607 stateMachine = this;
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
			nullable_0 = (aitoolContext_0.HasParameter("system_type_id") ? new long?(aitoolContext_0.GetParameter<long>("system_type_id", 0L)) : ((long?)null));
			list_0 = new List<object>();
			if (string_0 == "pipe" || string_0 == "all")
			{
				ienumerable_0 = mepSystemQueryTool_0.method_12(document_0, nullable_0);
				list_0.AddRange(ienumerable_0);
				ienumerable_0 = null;
			}
			if (string_0 == "duct" || string_0 == "all")
			{
				ienumerable_1 = mepSystemQueryTool_0.method_13(document_0, nullable_0);
				list_0.AddRange(ienumerable_1);
				ienumerable_1 = null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[MepSystemQueryTool] SystemStatistics: 返回 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个系统的统计信息");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(14, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("成功获取 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个系统的统计信息");
			AIToolResult result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class262<List<object>>(list_0));
			int_0 = -2;
			list_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	private sealed class Class608 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public string string_0;

		public MepSystemQueryTool mepSystemQueryTool_0;

		private List<object> list_0;

		private int int_1;

		private int int_2;

		private IOrderedEnumerable<_003C_003Ef__AnonymousType275<long, string, string, string>> iorderedEnumerable_0;

		private IOrderedEnumerable<_003C_003Ef__AnonymousType275<long, string, string, string>> iorderedEnumerable_1;

		private int int_3;

		private int int_4;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class608 stateMachine = this;
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
			list_0 = new List<object>();
			if (string_0 == "pipe" || string_0 == "all")
			{
				iorderedEnumerable_0 = from _003C_003Ef__AnonymousType275_0 in ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(PipingSystemType))).Cast<PipingSystemType>().Select(delegate(PipingSystemType pipingSystemType_0)
					{
						//IL_0012: Unknown result type (might be due to invalid IL or missing references)
						//IL_0017: Unknown result type (might be due to invalid IL or missing references)
						return new _003C_003Ef__AnonymousType275<long, string, string, string>(((Element)pipingSystemType_0).Id.Value, ((Element)pipingSystemType_0).Name, ((object)((MEPSystemType)pipingSystemType_0).SystemClassification/*cast due to constrained. prefix*/).ToString(), mepSystemQueryTool_0.method_28((MEPSystemType)(object)pipingSystemType_0));
					})
					orderby _003C_003Ef__AnonymousType275_0.system_type_name
					select _003C_003Ef__AnonymousType275_0;
				list_0.AddRange(iorderedEnumerable_0);
				iorderedEnumerable_0 = null;
			}
			if (string_0 == "duct" || string_0 == "all")
			{
				iorderedEnumerable_1 = from _003C_003Ef__AnonymousType275_0 in (from MEPSystemType mepsystemType_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(MEPSystemType))
						where mepSystemQueryTool_0.method_10(mepsystemType_0)
						select mepsystemType_0).Select(delegate(MEPSystemType mepsystemType_0)
					{
						//IL_0012: Unknown result type (might be due to invalid IL or missing references)
						//IL_0017: Unknown result type (might be due to invalid IL or missing references)
						return new _003C_003Ef__AnonymousType275<long, string, string, string>(((Element)mepsystemType_0).Id.Value, ((Element)mepsystemType_0).Name, ((object)mepsystemType_0.SystemClassification/*cast due to constrained. prefix*/).ToString(), mepSystemQueryTool_0.method_28(mepsystemType_0));
					})
					orderby _003C_003Ef__AnonymousType275_0.system_type_name
					select _003C_003Ef__AnonymousType275_0;
				list_0.AddRange(iorderedEnumerable_1);
				iorderedEnumerable_1 = null;
			}
			int_1 = 0;
			int_2 = 0;
			if (string_0 == "pipe")
			{
				int_1 = list_0.Count;
				int_2 = 0;
			}
			else if (string_0 == "duct")
			{
				int_1 = 0;
				int_2 = list_0.Count;
			}
			else
			{
				int_3 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(PipingSystemType))).Cast<PipingSystemType>().Count();
				int_4 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(MEPSystemType))).Cast<MEPSystemType>().Count(mepSystemQueryTool_0.method_10);
				int_1 = int_3;
				int_2 = int_4;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 3);
			defaultInterpolatedStringHandler.AppendLiteral("[MepSystemQueryTool] ListSystems: 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个系统类型（水管: ");
			defaultInterpolatedStringHandler.AppendFormatted(int_1);
			defaultInterpolatedStringHandler.AppendLiteral(", 风管: ");
			defaultInterpolatedStringHandler.AppendFormatted(int_2);
			defaultInterpolatedStringHandler.AppendLiteral("）");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(25, 3);
			defaultInterpolatedStringHandler2.AppendLiteral("找到 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个系统类型（水管系统: ");
			defaultInterpolatedStringHandler2.AppendFormatted(int_1);
			defaultInterpolatedStringHandler2.AppendLiteral(", 风管系统: ");
			defaultInterpolatedStringHandler2.AppendFormatted(int_2);
			defaultInterpolatedStringHandler2.AppendLiteral("）");
			AIToolResult result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class261<int, int, int, List<object>>(list_0.Count, int_1, int_2, list_0));
			int_0 = -2;
			list_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "mep_system_query";

	public string Category => "机电查询";

	public string Description => "查询 MEP 系统类型信息，包括系统列表、统计、类别分布、尺寸分布、保温状态、使用情况等";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operation\": {\n                \"type\": \"string\",\n                \"enum\": [\n                    \"list_systems\",\n                    \"system_statistics\",\n                    \"category_breakdown\",\n                    \"size_distribution\",\n                    \"insulation_status\",\n                    \"system_details\",\n                    \"check_usage\",\n                    \"get_properties\",\n                    \"get_elements\"\n                ],\n                \"description\": \"查询操作类型：list_systems(列系统), system_statistics(统计), category_breakdown(类别分布,仅支持pipe/duct), size_distribution(尺寸分布,仅支持pipe/duct), insulation_status(保温状态), system_details(系统详情), check_usage(使用检查), get_properties(属性查询), get_elements(获取使用该系统的元素)\"\n            },\n            \"target\": {\n                \"type\": \"string\",\n                \"enum\": [\"pipe\", \"duct\", \"all\"],\n                \"description\": \"目标类型：pipe(水管)、duct(风管)、all(全部)。注意：category_breakdown、size_distribution 和 get_elements 操作不支持 all\"\n            },\n            \"system_type_id\": {\n                \"type\": \"integer\",\n                \"description\": \"系统类型 ID（部分操作需要）\"\n            },\n            \"only_uninsulated\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否只显示未保温的系统（仅 insulation_status 操作）\"\n            },\n            \"include_size_distribution\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否包含尺寸分布（仅 system_details 操作）\"\n            },\n            \"include_category_breakdown\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否包含类别分布（仅 system_details 操作）\"\n            },\n            \"include_fittings\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否包含管件（仅 get_elements 操作，默认 true）\"\n            },\n            \"limit\": {\n                \"type\": \"integer\",\n                \"description\": \"限制返回数量（仅 get_elements 操作，默认 100）\"\n            }\n        },\n        \"required\": [\"operation\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class600))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class600 stateMachine = new Class600();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mepSystemQueryTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class608))]
	private Task<AIToolResult> method_0(Document document_0, string string_0)
	{
		Class608 stateMachine = new Class608();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mepSystemQueryTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.string_0 = string_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class607))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(Document document_0, string string_0, AIToolContext aitoolContext_0)
	{
		Class607 stateMachine = new Class607();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mepSystemQueryTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.string_0 = string_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class601))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_2(Document document_0, string string_0, AIToolContext aitoolContext_0)
	{
		Class601 stateMachine = new Class601();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mepSystemQueryTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.string_0 = string_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class605))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_3(Document document_0, string string_0, AIToolContext aitoolContext_0)
	{
		Class605 stateMachine = new Class605();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mepSystemQueryTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.string_0 = string_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class603))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_4(Document document_0, string string_0, AIToolContext aitoolContext_0)
	{
		Class603 stateMachine = new Class603();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mepSystemQueryTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.string_0 = string_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class606))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_5(Document document_0, string string_0, AIToolContext aitoolContext_0)
	{
		Class606 stateMachine = new Class606();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mepSystemQueryTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.string_0 = string_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class599))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_6(Document document_0, string string_0)
	{
		Class599 stateMachine = new Class599();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mepSystemQueryTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.string_0 = string_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class604))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_7(Document document_0, string string_0, AIToolContext aitoolContext_0)
	{
		Class604 stateMachine = new Class604();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mepSystemQueryTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.string_0 = string_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class602))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_8(Document document_0, string string_0, AIToolContext aitoolContext_0)
	{
		Class602 stateMachine = new Class602();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mepSystemQueryTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.string_0 = string_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private bool method_9(MEPSystemType mepsystemType_0)
	{
		return mepsystemType_0 is PipingSystemType;
	}

	private bool method_10(MEPSystemType mepsystemType_0)
	{
		return mepsystemType_0 is MechanicalSystemType;
	}

	private object? method_11(Color? color_0)
	{
		if (color_0 == null)
		{
			return null;
		}
		try
		{
			if (color_0.Red == 0 && color_0.Green == 0 && color_0.Blue == 0)
			{
				return null;
			}
			return new Class249<byte, byte, byte>(color_0.Red, color_0.Green, color_0.Blue);
		}
		catch
		{
			return null;
		}
	}

	private IEnumerable<object> method_12(Document document_0, long? nullable_0)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		Class583 CS_0024_003C_003E8__locals12 = new Class583();
		CS_0024_003C_003E8__locals12.mepSystemQueryTool_0 = this;
		CS_0024_003C_003E8__locals12.document_0 = document_0;
		CS_0024_003C_003E8__locals12.nullable_0 = nullable_0;
		List<Pipe> source = ((IEnumerable)new FilteredElementCollector(CS_0024_003C_003E8__locals12.document_0).OfClass(typeof(Pipe))).Cast<Pipe>().ToList();
		IQueryable<Pipe> queryable = source.AsQueryable();
		ParameterExpression parameterExpression;
		if (CS_0024_003C_003E8__locals12.nullable_0.HasValue)
		{
			IQueryable<Pipe> source2 = queryable;
			parameterExpression = Expression.Parameter(typeof(Pipe), "p");
			queryable = source2.Where(Expression.Lambda<Func<Pipe, bool>>(Expression.Equal(Expression.Call(Expression.Constant(this, typeof(MepSystemQueryTool)), (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(MepSystemQueryTool), "method_22", 2)), new Expression[2]
			{
				Expression.Field(Expression.Constant(CS_0024_003C_003E8__locals12, typeof(Class583)), FieldInfo.GetFieldFromHandle(RuntimeHandleHelper.FieldOf(typeof(Class583), "document_0"))),
				parameterExpression
			}), Expression.Property(Expression.Field(Expression.Constant(CS_0024_003C_003E8__locals12, typeof(Class583)), FieldInfo.GetFieldFromHandle(RuntimeHandleHelper.FieldOf(typeof(Class583), "nullable_0"))), (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(long?), "get_Value", 0), typeof(long?).TypeHandle))), new ParameterExpression[1] { parameterExpression }));
		}
		IQueryable<Pipe> source3 = queryable;
		parameterExpression = Expression.Parameter(typeof(Pipe), "p");
		IQueryable<IGrouping<long, Pipe>> source4 = source3.GroupBy(Expression.Lambda<Func<Pipe, long>>(Expression.Call(Expression.Constant(this, typeof(MepSystemQueryTool)), (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(MepSystemQueryTool), "method_22", 2)), new Expression[2]
		{
			Expression.Field(Expression.Constant(CS_0024_003C_003E8__locals12, typeof(Class583)), FieldInfo.GetFieldFromHandle(RuntimeHandleHelper.FieldOf(typeof(Class583), "document_0"))),
			parameterExpression
		}), new ParameterExpression[1] { parameterExpression }));
		parameterExpression = Expression.Parameter(typeof(IGrouping<long, Pipe>), "g");
		ConstructorInfo constructor = (ConstructorInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.CtorOf(typeof(ns0.Class271<long, int, int>), 3), typeof(Class271<long, int, int>).TypeHandle);
		Expression[] obj = new Expression[3]
		{
			Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(System.Linq.IGrouping<long, Autodesk.Revit.DB.Plumbing.Pipe>), "get_Key", 0), typeof(IGrouping<long, Pipe>).TypeHandle)),
			Expression.Call(null, (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(System.Linq.Enumerable), "Count", 1)), parameterExpression),
			null
		};
		MethodInfo method = (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(System.Linq.Enumerable), "Count", 2));
		Expression[] obj2 = new Expression[2] { parameterExpression, null };
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(Pipe), "p");
		obj2[1] = Expression.Lambda<Func<Pipe, bool>>(Expression.Call(Expression.Constant(this, typeof(MepSystemQueryTool)), (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(MepSystemQueryTool), "method_24", 2)), new Expression[2]
		{
			Expression.Field(Expression.Constant(CS_0024_003C_003E8__locals12, typeof(Class583)), FieldInfo.GetFieldFromHandle(RuntimeHandleHelper.FieldOf(typeof(Class583), "document_0"))),
			Expression.Property(parameterExpression2, (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(Autodesk.Revit.DB.Element), "get_Id", 0)))
		}), new ParameterExpression[1] { parameterExpression2 });
		obj[2] = Expression.Call(null, method, obj2);
		List<Class271<long, int, int>> source5 = source4.Select(Expression.Lambda<Func<IGrouping<long, Pipe>, Class271<long, int, int>>>(Expression.New(constructor, obj, (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(ns0.Class271<long, int, int>), "get_system_type_id", 0), typeof(Class271<long, int, int>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(ns0.Class271<long, int, int>), "get_element_count", 0), typeof(Class271<long, int, int>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(ns0.Class271<long, int, int>), "get_insulated_count", 0), typeof(Class271<long, int, int>).TypeHandle)), new ParameterExpression[1] { parameterExpression })).ToList();
		CS_0024_003C_003E8__locals12.dictionary_0 = ((IEnumerable)new FilteredElementCollector(CS_0024_003C_003E8__locals12.document_0).OfClass(typeof(PipingSystemType))).Cast<PipingSystemType>().ToDictionary((PipingSystemType pipingSystemType_0) => ((Element)pipingSystemType_0).Id.Value, (PipingSystemType pipingSystemType_0) => ((Element)pipingSystemType_0).Name);
		return source5.Select((Class271<long, int, int> class271_0) => new Class272<long, string, string, int, int, int>(class271_0.system_type_id, CS_0024_003C_003E8__locals12.dictionary_0.TryGetValue(class271_0.system_type_id, out var value) ? value : "未知系统", "水管", class271_0.element_count, class271_0.insulated_count, class271_0.element_count - class271_0.insulated_count));
	}

	private IEnumerable<object> method_13(Document document_0, long? nullable_0)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Expected O, but got Unknown
		Class584 CS_0024_003C_003E8__locals11 = new Class584();
		CS_0024_003C_003E8__locals11.mepSystemQueryTool_0 = this;
		CS_0024_003C_003E8__locals11.nullable_0 = nullable_0;
		CS_0024_003C_003E8__locals11.document_0 = document_0;
		List<Duct> source = ((IEnumerable)new FilteredElementCollector(CS_0024_003C_003E8__locals11.document_0).OfClass(typeof(Duct))).Cast<Duct>().ToList();
		IQueryable<Duct> queryable = source.AsQueryable();
		ParameterExpression parameterExpression;
		if (CS_0024_003C_003E8__locals11.nullable_0.HasValue)
		{
			IQueryable<Duct> source2 = queryable;
			parameterExpression = Expression.Parameter(typeof(Duct), "d");
			queryable = source2.Where(Expression.Lambda<Func<Duct, bool>>(Expression.Equal(Expression.Call(Expression.Constant(this, typeof(MepSystemQueryTool)), (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(MepSystemQueryTool), "method_23", 1)), parameterExpression), Expression.Property(Expression.Field(Expression.Constant(CS_0024_003C_003E8__locals11, typeof(Class584)), FieldInfo.GetFieldFromHandle(RuntimeHandleHelper.FieldOf(typeof(Class584), "nullable_0"))), (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(long?), "get_Value", 0), typeof(long?).TypeHandle))), new ParameterExpression[1] { parameterExpression }));
		}
		IQueryable<Duct> source3 = queryable;
		parameterExpression = Expression.Parameter(typeof(Duct), "d");
		IQueryable<IGrouping<long, Duct>> source4 = source3.GroupBy(Expression.Lambda<Func<Duct, long>>(Expression.Call(Expression.Constant(this, typeof(MepSystemQueryTool)), (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(MepSystemQueryTool), "method_23", 1)), parameterExpression), new ParameterExpression[1] { parameterExpression }));
		parameterExpression = Expression.Parameter(typeof(IGrouping<long, Duct>), "g");
		ConstructorInfo constructor = (ConstructorInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.CtorOf(typeof(ns0.Class271<long, int, int>), 3), typeof(Class271<long, int, int>).TypeHandle);
		Expression[] obj = new Expression[3]
		{
			Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(System.Linq.IGrouping<long, Autodesk.Revit.DB.Mechanical.Duct>), "get_Key", 0), typeof(IGrouping<long, Duct>).TypeHandle)),
			Expression.Call(null, (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(System.Linq.Enumerable), "Count", 1)), parameterExpression),
			null
		};
		MethodInfo method = (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(System.Linq.Enumerable), "Count", 2));
		Expression[] obj2 = new Expression[2] { parameterExpression, null };
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(Duct), "d");
		obj2[1] = Expression.Lambda<Func<Duct, bool>>(Expression.Call(Expression.Constant(this, typeof(MepSystemQueryTool)), (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(MepSystemQueryTool), "method_25", 2)), new Expression[2]
		{
			Expression.Field(Expression.Constant(CS_0024_003C_003E8__locals11, typeof(Class584)), FieldInfo.GetFieldFromHandle(RuntimeHandleHelper.FieldOf(typeof(Class584), "document_0"))),
			Expression.Property(parameterExpression2, (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(Autodesk.Revit.DB.Element), "get_Id", 0)))
		}), new ParameterExpression[1] { parameterExpression2 });
		obj[2] = Expression.Call(null, method, obj2);
		List<Class271<long, int, int>> source5 = source4.Select(Expression.Lambda<Func<IGrouping<long, Duct>, Class271<long, int, int>>>(Expression.New(constructor, obj, (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(ns0.Class271<long, int, int>), "get_system_type_id", 0), typeof(Class271<long, int, int>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(ns0.Class271<long, int, int>), "get_element_count", 0), typeof(Class271<long, int, int>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(RuntimeHandleHelper.MethodOf(typeof(ns0.Class271<long, int, int>), "get_insulated_count", 0), typeof(Class271<long, int, int>).TypeHandle)), new ParameterExpression[1] { parameterExpression })).ToList();
		CS_0024_003C_003E8__locals11.dictionary_0 = new Dictionary<long, string>();
		foreach (MEPSystemType item in new FilteredElementCollector(CS_0024_003C_003E8__locals11.document_0).OfClass(typeof(MEPSystemType)))
		{
			MEPSystemType val = item;
			if (method_10(val))
			{
				CS_0024_003C_003E8__locals11.dictionary_0[((Element)val).Id.Value] = ((Element)val).Name;
			}
		}
		return source5.Select((Class271<long, int, int> class271_0) => new Class272<long, string, string, int, int, int>(class271_0.system_type_id, CS_0024_003C_003E8__locals11.dictionary_0.TryGetValue(class271_0.system_type_id, out string value) ? value : "未知系统", "风管", class271_0.element_count, class271_0.insulated_count, class271_0.element_count - class271_0.insulated_count));
	}

	private object method_14(Document document_0, long long_0)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		List<Pipe> list = (from Pipe pipe_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Pipe))
			where method_22(document_0, pipe_0) == long_0
			select pipe_0).ToList();
		List<FamilyInstance> list2 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(FamilyInstance))).Cast<FamilyInstance>().Where(delegate(FamilyInstance familyInstance_0)
		{
			try
			{
				Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140334L));
				return val != null && val.HasValue && val.AsElementId().Value == long_0;
			}
			catch
			{
				return false;
			}
		}).ToList();
		return new Class273<long, object[]>(long_0, new object[2]
		{
			new
			{
				category = "管道",
				count = list.Count
			},
			new
			{
				category = "管件",
				count = list2.Count
			}
		});
	}

	private object method_15(Document document_0, long long_0)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		List<Duct> list = (from Duct duct_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Duct))
			where method_23(duct_0) == long_0
			select duct_0).ToList();
		List<FamilyInstance> list2 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(FamilyInstance))).Cast<FamilyInstance>().Where(delegate(FamilyInstance familyInstance_0)
		{
			try
			{
				Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140333L));
				return val != null && val.HasValue && val.AsElementId().Value == long_0;
			}
			catch
			{
				return false;
			}
		}).ToList();
		return new Class273<long, object[]>(long_0, new object[2]
		{
			new
			{
				category = "风管",
				count = list.Count
			},
			new
			{
				category = "管件",
				count = list2.Count
			}
		});
	}

	private object method_16(Document document_0, long long_0)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		List<Pipe> list_0 = (from Pipe pipe_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Pipe))
			where method_22(document_0, pipe_0) == long_0
			select pipe_0).ToList();
		Class274<string, double, double>[] source = new Class274<string, double, double>[4]
		{
			new Class274<string, double, double>("≤DN50", 0.0, 15.24),
			new Class274<string, double, double>("DN50-DN100", 15.24, 30.48),
			new Class274<string, double, double>("DN100-DN150", 30.48, 45.72),
			new Class274<string, double, double>(">DN150", 45.72, double.MaxValue)
		};
		List<Class275<string, int, int>> gparam_ = source.Select((Class274<string, double, double> class274_0) => new Class275<string, int, int>(class274_0.range, list_0.Count((Pipe pipe_0) => ((MEPCurve)pipe_0).Diameter >= class274_0.min && ((MEPCurve)pipe_0).Diameter < class274_0.max), list_0.Count((Pipe pipe_0) => ((MEPCurve)pipe_0).Diameter >= class274_0.min && ((MEPCurve)pipe_0).Diameter < class274_0.max && method_24(document_0, ((Element)pipe_0).Id)))).ToList();
		return new Class226<long, List<Class275<string, int, int>>>(long_0, gparam_);
	}

	private object method_17(Document document_0, long long_0)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		List<Duct> list_0 = (from Duct duct_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Duct))
			where method_23(duct_0) == long_0
			select duct_0).ToList();
		Class274<string, double, double>[] source = new Class274<string, double, double>[3]
		{
			new Class274<string, double, double>("小尺寸(≤300x300)", 0.0, 300.0),
			new Class274<string, double, double>("中尺寸(300-600)", 300.0, 600.0),
			new Class274<string, double, double>("大尺寸(>600)", 600.0, double.MaxValue)
		};
		List<Class275<string, int, int>> gparam_ = source.Select((Class274<string, double, double> class274_0) => new Class275<string, int, int>(class274_0.range, list_0.Count(delegate(Duct duct_0)
		{
			double width = ((MEPCurve)duct_0).Width;
			double height = ((MEPCurve)duct_0).Height;
			double num = Math.Max(width, height);
			return num >= class274_0.min && num < class274_0.max;
		}), list_0.Count(delegate(Duct duct_0)
		{
			double width = ((MEPCurve)duct_0).Width;
			double height = ((MEPCurve)duct_0).Height;
			double num = Math.Max(width, height);
			return num >= class274_0.min && num < class274_0.max && method_25(document_0, ((Element)duct_0).Id);
		}))).ToList();
		return new Class226<long, List<Class275<string, int, int>>>(long_0, gparam_);
	}

	private IEnumerable<object> method_18(Document document_0, bool bool_0)
	{
		IEnumerable<object> source = method_12(document_0, null);
		if (bool_0)
		{
			source = source.Where(delegate(object object_0)
			{
				if (Class597.callSite_0 == null)
				{
					Class597.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "insulated_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				return (dynamic)Class597.callSite_0.Target(Class597.callSite_0, object_0) == 0;
			});
		}
		return source.Select(delegate(object object_0)
		{
			if (Class597.callSite_3 == null)
			{
				Class597.callSite_3 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "system_type_id", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object system_type_id = Class597.callSite_3.Target(Class597.callSite_3, object_0);
			if (Class597.callSite_4 == null)
			{
				Class597.callSite_4 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "system_type_name", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object system_type_name = Class597.callSite_4.Target(Class597.callSite_4, object_0);
			string element_type = "水管";
			if (Class597.callSite_5 == null)
			{
				Class597.callSite_5 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "element_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object total_elements = Class597.callSite_5.Target(Class597.callSite_5, object_0);
			if (Class597.callSite_6 == null)
			{
				Class597.callSite_6 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "insulated_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object insulated_count = Class597.callSite_6.Target(Class597.callSite_6, object_0);
			if (Class597.callSite_7 == null)
			{
				Class597.callSite_7 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "uninsulated_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object uninsulated_count = Class597.callSite_7.Target(Class597.callSite_7, object_0);
			if (Class597.callSite_8 == null)
			{
				Class597.callSite_8 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "element_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object coverage_rate;
			if (!(((dynamic)Class597.callSite_8.Target(Class597.callSite_8, object_0) > 0) ? true : false))
			{
				coverage_rate = 0;
			}
			else
			{
				if (Class597.callSite_16 == null)
				{
					Class597.callSite_16 = CallSite<Func<CallSite, Type, object, int, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "Round", null, typeof(MepSystemQueryTool), new CSharpArgumentInfo[3]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				Func<CallSite, Type, object, int, object> target = Class597.callSite_16.Target;
				CallSite<Func<CallSite, Type, object, int, object>> callSite_ = Class597.callSite_16;
				Type? typeFromHandle = typeof(Math);
				if (Class597.callSite_11 == null)
				{
					Class597.callSite_11 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "insulated_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				double num = (double)(dynamic)Class597.callSite_11.Target(Class597.callSite_11, object_0);
				if (Class597.callSite_13 == null)
				{
					Class597.callSite_13 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "element_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				coverage_rate = target(callSite_, typeFromHandle, (object)(num / (dynamic)Class597.callSite_13.Target(Class597.callSite_13, object_0) * 100), 1);
			}
			return new { system_type_id, system_type_name, element_type, total_elements, insulated_count, uninsulated_count, coverage_rate };
		});
	}

	private IEnumerable<object> method_19(Document document_0, bool bool_0)
	{
		IEnumerable<object> source = method_13(document_0, null);
		if (bool_0)
		{
			source = source.Where(delegate(object object_0)
			{
				if (Class598.callSite_0 == null)
				{
					Class598.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "insulated_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				return (dynamic)Class598.callSite_0.Target(Class598.callSite_0, object_0) == 0;
			});
		}
		return source.Select(delegate(object object_0)
		{
			if (Class598.callSite_3 == null)
			{
				Class598.callSite_3 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "system_type_id", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object system_type_id = Class598.callSite_3.Target(Class598.callSite_3, object_0);
			if (Class598.callSite_4 == null)
			{
				Class598.callSite_4 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "system_type_name", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object system_type_name = Class598.callSite_4.Target(Class598.callSite_4, object_0);
			string element_type = "风管";
			if (Class598.callSite_5 == null)
			{
				Class598.callSite_5 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "element_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object total_elements = Class598.callSite_5.Target(Class598.callSite_5, object_0);
			if (Class598.callSite_6 == null)
			{
				Class598.callSite_6 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "insulated_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object insulated_count = Class598.callSite_6.Target(Class598.callSite_6, object_0);
			if (Class598.callSite_7 == null)
			{
				Class598.callSite_7 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "uninsulated_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object uninsulated_count = Class598.callSite_7.Target(Class598.callSite_7, object_0);
			if (Class598.callSite_8 == null)
			{
				Class598.callSite_8 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "element_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object coverage_rate;
			if (!(((dynamic)Class598.callSite_8.Target(Class598.callSite_8, object_0) > 0) ? true : false))
			{
				coverage_rate = 0;
			}
			else
			{
				if (Class598.callSite_16 == null)
				{
					Class598.callSite_16 = CallSite<Func<CallSite, Type, object, int, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "Round", null, typeof(MepSystemQueryTool), new CSharpArgumentInfo[3]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				Func<CallSite, Type, object, int, object> target = Class598.callSite_16.Target;
				CallSite<Func<CallSite, Type, object, int, object>> callSite_ = Class598.callSite_16;
				Type? typeFromHandle = typeof(Math);
				if (Class598.callSite_11 == null)
				{
					Class598.callSite_11 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "insulated_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				double num = (double)(dynamic)Class598.callSite_11.Target(Class598.callSite_11, object_0);
				if (Class598.callSite_13 == null)
				{
					Class598.callSite_13 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "element_count", typeof(MepSystemQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				coverage_rate = target(callSite_, typeFromHandle, (object)(num / (dynamic)Class598.callSite_13.Target(Class598.callSite_13, object_0) * 100), 1);
			}
			return new { system_type_id, system_type_name, element_type, total_elements, insulated_count, uninsulated_count, coverage_rate };
		});
	}

	private IEnumerable<object> method_20(Document document_0)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		List<PipingSystemType> source = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(PipingSystemType))).Cast<PipingSystemType>().ToList();
		Dictionary<long, int> dictionary_0 = (from Pipe pipe_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Pipe))
			group pipe_0 by method_22(document_0, pipe_0)).ToDictionary((IGrouping<long, Pipe> igrouping_0) => igrouping_0.Key, (IGrouping<long, Pipe> igrouping_0) => igrouping_0.Count());
		return source.Select((PipingSystemType pipingSystemType_0) => new Class276<long, string, bool, int, bool, bool, bool, bool>(((Element)pipingSystemType_0).Id.Value, ((Element)pipingSystemType_0).Name, dictionary_0.ContainsKey(((Element)pipingSystemType_0).Id.Value) && dictionary_0[((Element)pipingSystemType_0).Id.Value] > 0, dictionary_0.TryGetValue(((Element)pipingSystemType_0).Id.Value, out var value) ? value : 0, ((ElementType)pipingSystemType_0).CanBeDeleted, ((ElementType)pipingSystemType_0).CanBeRenamed, ((ElementType)pipingSystemType_0).CanBeCopied, gparam_15: false));
	}

	private IEnumerable<object> method_21(Document document_0)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		List<MEPSystemType> source = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(MEPSystemType))).Cast<MEPSystemType>().Where(method_10).ToList();
		Dictionary<long, int> dictionary_0 = (from Duct duct_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Duct))
			group duct_0 by method_23(duct_0)).ToDictionary((IGrouping<long, Duct> igrouping_0) => igrouping_0.Key, (IGrouping<long, Duct> igrouping_0) => igrouping_0.Count());
		return source.Select((MEPSystemType mepsystemType_0) => new Class276<long, string, bool, int, bool, bool, bool, bool>(((Element)mepsystemType_0).Id.Value, ((Element)mepsystemType_0).Name, dictionary_0.ContainsKey(((Element)mepsystemType_0).Id.Value) && dictionary_0[((Element)mepsystemType_0).Id.Value] > 0, dictionary_0.TryGetValue(((Element)mepsystemType_0).Id.Value, out var value) ? value : 0, ((ElementType)mepsystemType_0).CanBeDeleted, ((ElementType)mepsystemType_0).CanBeRenamed, ((ElementType)mepsystemType_0).CanBeCopied, gparam_15: false));
	}

	private long method_22(Document document_0, Pipe pipe_0)
	{
		try
		{
			if (((MEPCurve)pipe_0).MEPSystem != null)
			{
				return ((Element)((MEPCurve)pipe_0).MEPSystem).GetTypeId().Value;
			}
			Parameter val = ((Element)pipe_0).get_Parameter((BuiltInParameter)(-1140334L));
			if (val != null && val.HasValue)
			{
				return val.AsElementId().Value;
			}
		}
		catch
		{
		}
		return -1L;
	}

	private long method_23(Duct duct_0)
	{
		try
		{
			if (((MEPCurve)duct_0).MEPSystem != null)
			{
				return ((Element)((MEPCurve)duct_0).MEPSystem).GetTypeId().Value;
			}
			Parameter val = ((Element)duct_0).get_Parameter((BuiltInParameter)(-1140333L));
			if (val != null && val.HasValue)
			{
				return val.AsElementId().Value;
			}
		}
		catch
		{
		}
		return -1L;
	}

	private bool method_24(Document document_0, ElementId elementId_0)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			return ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(PipeInsulation))).Cast<PipeInsulation>().Any(delegate(PipeInsulation pipeInsulation_0)
			{
				try
				{
					ElementId val = method_26(pipeInsulation_0);
					return val == elementId_0;
				}
				catch
				{
					return false;
				}
			});
		}
		catch
		{
			return false;
		}
	}

	private bool method_25(Document document_0, ElementId elementId_0)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			return ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(DuctInsulation))).Cast<DuctInsulation>().Any(delegate(DuctInsulation ductInsulation_0)
			{
				try
				{
					ElementId val = method_27(ductInsulation_0);
					return val == elementId_0;
				}
				catch
				{
					return false;
				}
			});
		}
		catch
		{
			return false;
		}
	}

	private ElementId method_26(PipeInsulation pipeInsulation_0)
	{
		try
		{
			return ((InsulationLiningBase)pipeInsulation_0).HostElementId;
		}
		catch
		{
			return ElementId.InvalidElementId;
		}
	}

	private ElementId method_27(DuctInsulation ductInsulation_0)
	{
		try
		{
			return ((InsulationLiningBase)ductInsulation_0).HostElementId;
		}
		catch
		{
			return ElementId.InvalidElementId;
		}
	}

	private string method_28(MEPSystemType mepsystemType_0)
	{
		try
		{
			Parameter val = ((IEnumerable)((Element)mepsystemType_0).Parameters).Cast<Parameter>().FirstOrDefault((Parameter parameter_0) => parameter_0.Definition.Name.Equals("Abbreviation", StringComparison.OrdinalIgnoreCase));
			object obj;
			if (val == null)
			{
				obj = null;
			}
			else
			{
				obj = val.AsString();
				if (obj != null)
				{
					goto IL_004b;
				}
			}
			obj = "";
			goto IL_004b;
			IL_004b:
			return (string)obj;
		}
		catch
		{
			return "";
		}
	}

	[CompilerGenerated]
	private _003C_003Ef__AnonymousType275<long, string, string, string> method_29(PipingSystemType pipingSystemType_0)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return new _003C_003Ef__AnonymousType275<long, string, string, string>(((Element)pipingSystemType_0).Id.Value, ((Element)pipingSystemType_0).Name, ((object)((MEPSystemType)pipingSystemType_0).SystemClassification/*cast due to constrained. prefix*/).ToString(), method_28((MEPSystemType)(object)pipingSystemType_0));
	}

	[CompilerGenerated]
	private bool method_30(MEPSystemType mepsystemType_0)
	{
		return method_10(mepsystemType_0);
	}

	[CompilerGenerated]
	private _003C_003Ef__AnonymousType275<long, string, string, string> method_31(MEPSystemType mepsystemType_0)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return new _003C_003Ef__AnonymousType275<long, string, string, string>(((Element)mepsystemType_0).Id.Value, ((Element)mepsystemType_0).Name, ((object)mepsystemType_0.SystemClassification/*cast due to constrained. prefix*/).ToString(), method_28(mepsystemType_0));
	}
}
