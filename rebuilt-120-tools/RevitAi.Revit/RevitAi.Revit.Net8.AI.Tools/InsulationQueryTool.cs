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
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("insulation_query", Category = "保温查询", Description = "查询 MEP 元素的保温状态，包括系统保温概况、元素保温状态、保温类型、尺寸分档统计等", RequiresTransaction = false, RequiresModification = false)]
public sealed class InsulationQueryTool : IAITool
{
	[CompilerGenerated]
	private sealed class Class520
	{
		public InsulationQueryTool insulationQueryTool_0;

		public Document document_0;

		public Dictionary<ElementId, PipeInsulation> dictionary_0;

		public Func<Pipe, bool> func_0;

		public Func<FamilyInstance, bool> func_1;

		internal bool method_0(Pipe pipe_0)
		{
			return insulationQueryTool_0.method_18(document_0, pipe_0) > 0L;
		}

		internal long method_1(Pipe pipe_0)
		{
			return insulationQueryTool_0.method_18(document_0, pipe_0);
		}

		internal bool method_2(Pipe pipe_0)
		{
			return dictionary_0.ContainsKey(((Element)pipe_0).Id);
		}

		internal bool method_3(FamilyInstance familyInstance_0)
		{
			return dictionary_0.ContainsKey(((Element)familyInstance_0).Id);
		}
	}

	[CompilerGenerated]
	private sealed class Class521
	{
		public IGrouping<long, Pipe> igrouping_0;

		public Class520 class520_0;

		internal bool method_0(FamilyInstance familyInstance_0)
		{
			return class520_0.insulationQueryTool_0.method_19(class520_0.document_0, familyInstance_0) == igrouping_0.Key;
		}
	}

	[CompilerGenerated]
	private sealed class Class522
	{
		public InsulationQueryTool insulationQueryTool_0;

		public Dictionary<ElementId, DuctInsulation> dictionary_0;

		public Document document_0;

		public Func<Duct, bool> func_0;

		public Func<FamilyInstance, bool> func_1;

		internal bool method_0(Duct duct_0)
		{
			return insulationQueryTool_0.method_20(duct_0) > 0L;
		}

		internal long method_1(Duct duct_0)
		{
			return insulationQueryTool_0.method_20(duct_0);
		}

		internal bool method_2(Duct duct_0)
		{
			return dictionary_0.ContainsKey(((Element)duct_0).Id);
		}

		internal bool method_3(FamilyInstance familyInstance_0)
		{
			return dictionary_0.ContainsKey(((Element)familyInstance_0).Id);
		}
	}

	[CompilerGenerated]
	private sealed class Class523
	{
		public IGrouping<long, Duct> igrouping_0;

		public Class522 class522_0;

		internal bool method_0(FamilyInstance familyInstance_0)
		{
			return class522_0.insulationQueryTool_0.method_21(class522_0.document_0, familyInstance_0) == igrouping_0.Key;
		}
	}

	[CompilerGenerated]
	private sealed class Class524
	{
		public Element element_0;

		internal bool method_0(PipeInsulation pipeInsulation_0)
		{
			return ((InsulationLiningBase)pipeInsulation_0).HostElementId == element_0.Id;
		}

		internal bool method_1(DuctInsulation ductInsulation_0)
		{
			return ((InsulationLiningBase)ductInsulation_0).HostElementId == element_0.Id;
		}

		internal bool method_2(PipeInsulation pipeInsulation_0)
		{
			return ((InsulationLiningBase)pipeInsulation_0).HostElementId == element_0.Id;
		}

		internal bool method_3(DuctInsulation ductInsulation_0)
		{
			return ((InsulationLiningBase)ductInsulation_0).HostElementId == element_0.Id;
		}
	}

	[CompilerGenerated]
	private sealed class Class525
	{
		public InsulationQueryTool insulationQueryTool_0;

		public Document document_0;

		public long long_0;

		public Dictionary<ElementId, PipeInsulation> dictionary_0;

		public Func<Pipe, bool> func_0;

		internal bool method_0(Pipe pipe_0)
		{
			return insulationQueryTool_0.method_18(document_0, pipe_0) == long_0;
		}

		internal bool method_1(Pipe pipe_0)
		{
			return dictionary_0.ContainsKey(((Element)pipe_0).Id);
		}
	}

	[CompilerGenerated]
	private sealed class Class526
	{
		public InsulationQueryTool insulationQueryTool_0;

		public long long_0;

		public Dictionary<ElementId, DuctInsulation> dictionary_0;

		public Func<Duct, bool> func_0;

		internal bool method_0(Duct duct_0)
		{
			return insulationQueryTool_0.method_20(duct_0) == long_0;
		}

		internal _003C_003Ef__AnonymousType237<string, double, double, Duct> method_1(Duct duct_0)
		{
			var (num, num2, num3) = insulationQueryTool_0.method_16(duct_0);
			if (num3 > 0.0)
			{
				double num4 = Math.Round(num3 * 304.8);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Φ");
				defaultInterpolatedStringHandler.AppendFormatted(num4, "0");
				return new _003C_003Ef__AnonymousType237<string, double, double, Autodesk.Revit.DB.Mechanical.Duct>(defaultInterpolatedStringHandler.ToStringAndClear(), num4, num4, duct_0);
			}
			double val = Math.Round(num * 304.8);
			double val2 = Math.Round(num2 * 304.8);
			double num5 = Math.Max(val, val2);
			double value = Math.Min(val, val2);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler2.AppendFormatted(num5, "0");
			defaultInterpolatedStringHandler2.AppendLiteral("x");
			defaultInterpolatedStringHandler2.AppendFormatted(value, "0");
			return new _003C_003Ef__AnonymousType237<string, double, double, Autodesk.Revit.DB.Mechanical.Duct>(defaultInterpolatedStringHandler2.ToStringAndClear(), num5, num5, duct_0);
		}

		internal bool method_2(Duct duct_0)
		{
			return dictionary_0.ContainsKey(((Element)duct_0).Id);
		}
	}

	[CompilerGenerated]
	private sealed class Class527
	{
		public Dictionary<ElementId, PipeInsulation> dictionary_0;

		public InsulationQueryTool insulationQueryTool_0;

		public Document document_0;

		internal bool method_0(Pipe pipe_0)
		{
			return !dictionary_0.ContainsKey(((Element)pipe_0).Id);
		}

		internal _003C_003Ef__AnonymousType239<long, string, double, long, double, double> method_1(Pipe pipe_0)
		{
			return new _003C_003Ef__AnonymousType239<long, string, double, long, double, double>(((Element)pipe_0).Id.Value, "管道", Math.Round(((MEPCurve)pipe_0).Diameter * 304.8, 1), insulationQueryTool_0.method_18(document_0, pipe_0), insulationQueryTool_0.method_14(pipe_0), Math.Round(insulationQueryTool_0.method_14(pipe_0) * 304.8, 0));
		}

		internal bool method_2(FamilyInstance familyInstance_0)
		{
			Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140334L));
			return val != null && val.HasValue && !dictionary_0.ContainsKey(((Element)familyInstance_0).Id);
		}
	}

	[CompilerGenerated]
	private sealed class Class528
	{
		public Dictionary<ElementId, DuctInsulation> dictionary_0;

		public InsulationQueryTool insulationQueryTool_0;

		internal bool method_0(Duct duct_0)
		{
			return !dictionary_0.ContainsKey(((Element)duct_0).Id);
		}

		internal Class228<long, string, double, double, double, long, double, double> method_1(Duct duct_0)
		{
			var (num, num2, num3) = insulationQueryTool_0.method_16(duct_0);
			return new Class228<long, string, double, double, double, long, double, double>(((Element)duct_0).Id.Value, "风管", (num3 > 0.0) ? 0.0 : Math.Round(num * 304.8, 0), (num3 > 0.0) ? 0.0 : Math.Round(num2 * 304.8, 0), (num3 > 0.0) ? Math.Round(num3 * 304.8, 0) : 0.0, insulationQueryTool_0.method_20(duct_0), insulationQueryTool_0.method_15(duct_0), Math.Round(insulationQueryTool_0.method_15(duct_0) * 304.8, 0));
		}

		internal bool method_2(FamilyInstance familyInstance_0)
		{
			Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140333L));
			return val != null && val.HasValue && !dictionary_0.ContainsKey(((Element)familyInstance_0).Id);
		}
	}

	[CompilerGenerated]
	private sealed class Class529
	{
		public Dictionary<ElementId, PipeInsulation> dictionary_0;

		public InsulationQueryTool insulationQueryTool_0;

		internal bool method_0(Pipe pipe_0)
		{
			return dictionary_0.ContainsKey(((Element)pipe_0).Id);
		}

		internal _003C_003Ef__AnonymousType242<string, string> method_1(Pipe pipe_0)
		{
			PipeInsulation val = dictionary_0[((Element)pipe_0).Id];
			Parameter? obj = insulationQueryTool_0.method_17((Element)(object)val, "Insulation Thickness");
			double? num = ((obj != null) ? new double?(obj.AsDouble()) : ((double?)null));
			string thickness = (num.HasValue ? Math.Round(num.Value * 304.8, 1).ToString() : "0");
			string name = ((Element)val).Name;
			return new _003C_003Ef__AnonymousType242<string, string>(thickness, name);
		}
	}

	[CompilerGenerated]
	private sealed class Class530
	{
		public Dictionary<ElementId, DuctInsulation> dictionary_0;

		public InsulationQueryTool insulationQueryTool_0;

		internal bool method_0(Duct duct_0)
		{
			return dictionary_0.ContainsKey(((Element)duct_0).Id);
		}

		internal _003C_003Ef__AnonymousType242<string, string> method_1(Duct duct_0)
		{
			DuctInsulation val = dictionary_0[((Element)duct_0).Id];
			Parameter? obj = insulationQueryTool_0.method_17((Element)(object)val, "Insulation Thickness");
			double? num = ((obj != null) ? new double?(obj.AsDouble()) : ((double?)null));
			string thickness = (num.HasValue ? Math.Round(num.Value * 304.8, 1).ToString() : "0");
			string name = ((Element)val).Name;
			return new _003C_003Ef__AnonymousType242<string, string>(thickness, name);
		}
	}

	[CompilerGenerated]
	private static class Class531
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, bool>> callSite_1;
	}

	[CompilerGenerated]
	private static class Class532
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, string, object>> callSite_1;

		public static CallSite<Func<CallSite, object, bool>> callSite_2;
	}

	[CompilerGenerated]
	private static class Class533
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, int>> callSite_1;

		public static CallSite<Func<CallSite, object, object>> callSite_2;

		public static CallSite<Func<CallSite, object, int>> callSite_3;
	}

	[CompilerGenerated]
	private sealed class Class534 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public InsulationQueryTool insulationQueryTool_0;

		private Document document_0;

		private string string_0;

		private string string_1;

		private string string_2;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private AIToolResult aitoolResult_2;

		private AIToolResult aitoolResult_3;

		private AIToolResult aitoolResult_4;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if ((uint)(num - 1) <= 4u)
				{
					goto IL_006e;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class534 stateMachine = this;
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
				AIToolResult val;
				TaskAwaiter<AIToolResult> awaiter2;
				TaskAwaiter<AIToolResult> awaiter3;
				TaskAwaiter<AIToolResult> awaiter4;
				TaskAwaiter<AIToolResult> awaiter5;
				TaskAwaiter<AIToolResult> awaiter6;
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
								Logger.Info("[InsulationQueryTool] 执行操作: " + string_0 + ", target: " + string_1);
								string_2 = string_0.ToLower();
								string text = string_2;
								if (!(text == "list_systems"))
								{
									if (!(text == "query_elements"))
									{
										if (!(text == "query_types"))
										{
											if (!(text == "query_size_ranges"))
											{
												if (!(text == "query_uninsulated"))
												{
													val = AIToolResult.Fail("不支持的操作类型: " + string_0);
													break;
												}
												awaiter2 = insulationQueryTool_0.method_4(document_0, string_1, aitoolContext_0).GetAwaiter();
												if (!awaiter2.IsCompleted)
												{
													num = 5;
													int_0 = 5;
													taskAwaiter_1 = awaiter2;
													Class534 stateMachine = this;
													asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
													return;
												}
												goto IL_04d6;
											}
											awaiter3 = insulationQueryTool_0.method_3(document_0, string_1, aitoolContext_0).GetAwaiter();
											if (!awaiter3.IsCompleted)
											{
												num = 4;
												int_0 = 4;
												taskAwaiter_1 = awaiter3;
												Class534 stateMachine = this;
												asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
												return;
											}
											goto IL_049b;
										}
										awaiter4 = insulationQueryTool_0.method_2(document_0, string_1).GetAwaiter();
										if (!awaiter4.IsCompleted)
										{
											num = 3;
											int_0 = 3;
											taskAwaiter_1 = awaiter4;
											Class534 stateMachine = this;
											asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
											return;
										}
										goto IL_0460;
									}
									awaiter5 = insulationQueryTool_0.method_1(document_0, string_1, aitoolContext_0).GetAwaiter();
									if (!awaiter5.IsCompleted)
									{
										num = 2;
										int_0 = 2;
										taskAwaiter_1 = awaiter5;
										Class534 stateMachine = this;
										asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
										return;
									}
									goto IL_0422;
								}
								awaiter6 = insulationQueryTool_0.method_0(document_0, string_1).GetAwaiter();
								if (!awaiter6.IsCompleted)
								{
									num = 1;
									int_0 = 1;
									taskAwaiter_1 = awaiter6;
									Class534 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter6, ref stateMachine);
									return;
								}
								goto IL_03e4;
							}
							result = AIToolResult.Fail("必须指定 operation 参数");
						}
					}
					goto end_IL_006e;
				case 1:
					awaiter6 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_03e4;
				case 2:
					awaiter5 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0422;
				case 3:
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0460;
				case 4:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_049b;
				case 5:
					{
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						goto IL_04d6;
					}
					IL_0422:
					aitoolResult_1 = awaiter5.GetResult();
					val = aitoolResult_1;
					aitoolResult_1 = null;
					break;
					IL_0460:
					aitoolResult_2 = awaiter4.GetResult();
					val = aitoolResult_2;
					aitoolResult_2 = null;
					break;
					IL_049b:
					aitoolResult_3 = awaiter3.GetResult();
					val = aitoolResult_3;
					aitoolResult_3 = null;
					break;
					IL_03e4:
					aitoolResult_0 = awaiter6.GetResult();
					val = aitoolResult_0;
					aitoolResult_0 = null;
					break;
					IL_04d6:
					aitoolResult_4 = awaiter2.GetResult();
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
				Logger.Error("[InsulationQueryTool] 执行失败: " + exception_0.Message);
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
	private sealed class Class537 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public string string_0;

		public AIToolContext aitoolContext_0;

		public InsulationQueryTool insulationQueryTool_0;

		private List<int> list_0;

		private List<object> list_1;

		private int int_1;

		private int int_2;

		private List<int>.Enumerator enumerator_0;

		private int int_3;

		private Element element_0;

		private object object_0;

		private bool bool_0;

		private Exception exception_0;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private PropertyInfo propertyInfo_0;

		private object object_1;

		private PropertyInfo propertyInfo_1;

		private string string_2;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Expected O, but got Unknown
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
					Class537 stateMachine = this;
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
			list_0 = smethod_2(aitoolContext_0);
			if ((list_0 == null || list_0.Count == 0) && aitoolContext_0.HasParameter("cacheId"))
			{
				list_0 = smethod_0(aitoolContext_0, aitoolContext_0.GetParameter<string>("cacheId", (string)null));
			}
			AIToolResult result;
			if (list_0 == null || list_0.Count == 0)
			{
				result = AIToolResult.Fail("必须指定要查询的元素 ID 列表 (element_ids)，或提供 cacheId 引用已缓存的元素");
			}
			else
			{
				list_1 = new List<object>();
				int_1 = 0;
				int_2 = 0;
				enumerator_0 = list_0.GetEnumerator();
				try
				{
					while (enumerator_0.MoveNext())
					{
						int_3 = enumerator_0.Current;
						try
						{
							element_0 = document_0.GetElement(new ElementId((long)int_3));
							if (element_0 == null)
							{
								continue;
							}
							object_0 = insulationQueryTool_0.method_7(document_0, element_0);
							if (object_0 != null)
							{
								list_1.Add(object_0);
								if (Class531.callSite_0 == null)
								{
									Class531.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "has_insulation", typeof(InsulationQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								bool_0 = (bool)(dynamic)Class531.callSite_0.Target(Class531.callSite_0, object_0);
								if (bool_0)
								{
									int_1++;
								}
								else
								{
									int_2++;
								}
							}
							element_0 = null;
							object_0 = null;
						}
						catch (Exception ex)
						{
							exception_0 = ex;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 2);
							defaultInterpolatedStringHandler.AppendLiteral("[InsulationQueryTool] QueryElements: 查询元素 ");
							defaultInterpolatedStringHandler.AppendFormatted(int_3);
							defaultInterpolatedStringHandler.AppendLiteral(" 失败 - ");
							defaultInterpolatedStringHandler.AppendFormatted(exception_0.Message);
							Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
						}
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
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(50, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("[InsulationQueryTool] QueryElements: 查询了 ");
				defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个元素的保温状态");
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
				if (list_1.Count > 100)
				{
					if (aitoolContext_0.DataCache == null || string.IsNullOrEmpty(aitoolContext_0.SessionId))
					{
						Logger.Warning("[InsulationQueryTool] 数据缓存或会话ID不可用，无法缓存元素保温状态结果");
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(28, 3);
						defaultInterpolatedStringHandler3.AppendLiteral("成功查询 ");
						defaultInterpolatedStringHandler3.AppendFormatted(list_1.Count);
						defaultInterpolatedStringHandler3.AppendLiteral(" 个元素的保温状态（已保温: ");
						defaultInterpolatedStringHandler3.AppendFormatted(int_1);
						defaultInterpolatedStringHandler3.AppendLiteral(", 未保温: ");
						defaultInterpolatedStringHandler3.AppendFormatted(int_2);
						defaultInterpolatedStringHandler3.AppendLiteral("）");
						result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class218<int, int, int, List<object>>(list_1.Count, int_1, int_2, list_1.Take(100).ToList()));
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(26, 1);
						defaultInterpolatedStringHandler4.AppendLiteral("insulation_element_status_");
						defaultInterpolatedStringHandler4.AppendFormatted(DateTime.Now, "yyyyMMddHHmmssfff");
						string_1 = defaultInterpolatedStringHandler4.ToStringAndClear();
						aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)list_1, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_1, "个元素保温状态", 100);
						if (aitoolResult_0.Data != null)
						{
							try
							{
								propertyInfo_0 = aitoolResult_0.Data.GetType().GetProperty("cache_info");
								if (propertyInfo_0 != null)
								{
									object_1 = propertyInfo_0.GetValue(aitoolResult_0.Data);
									if (object_1 != null)
									{
										propertyInfo_1 = object_1.GetType().GetProperty("cache_id");
										if (propertyInfo_1 != null)
										{
											string_2 = propertyInfo_1.GetValue(object_1)?.ToString();
											if (!string.IsNullOrEmpty(string_2))
											{
												AIToolResult obj = aitoolResult_0;
												DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(68, 4);
												defaultInterpolatedStringHandler5.AppendLiteral("成功查询 ");
												defaultInterpolatedStringHandler5.AppendFormatted(list_1.Count);
												defaultInterpolatedStringHandler5.AppendLiteral(" 个元素的保温状态（已保温: ");
												defaultInterpolatedStringHandler5.AppendFormatted(int_1);
												defaultInterpolatedStringHandler5.AppendLiteral(", 未保温: ");
												defaultInterpolatedStringHandler5.AppendFormatted(int_2);
												defaultInterpolatedStringHandler5.AppendLiteral("）\n\n💡 完整结果已缓存，后续工具调用中使用 cacheId=\"");
												defaultInterpolatedStringHandler5.AppendFormatted(string_2);
												defaultInterpolatedStringHandler5.AppendLiteral("\" 引用这些元素");
												obj.Message = defaultInterpolatedStringHandler5.ToStringAndClear();
											}
											string_2 = null;
										}
										propertyInfo_1 = null;
									}
									object_1 = null;
								}
								propertyInfo_0 = null;
							}
							catch
							{
							}
						}
						result = aitoolResult_0;
					}
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(28, 3);
					defaultInterpolatedStringHandler6.AppendLiteral("成功查询 ");
					defaultInterpolatedStringHandler6.AppendFormatted(list_1.Count);
					defaultInterpolatedStringHandler6.AppendLiteral(" 个元素的保温状态（已保温: ");
					defaultInterpolatedStringHandler6.AppendFormatted(int_1);
					defaultInterpolatedStringHandler6.AppendLiteral(", 未保温: ");
					defaultInterpolatedStringHandler6.AppendFormatted(int_2);
					defaultInterpolatedStringHandler6.AppendLiteral("）");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler6.ToStringAndClear(), (object)new Class218<int, int, int, List<object>>(list_1.Count, int_1, int_2, list_1));
				}
			}
			int_0 = -2;
			list_0 = null;
			list_1 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	private sealed class Class538 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public string string_0;

		public AIToolContext aitoolContext_0;

		public InsulationQueryTool insulationQueryTool_0;

		private long long_0;

		private MEPSystemType mepsystemType_0;

		private bool bool_0;

		private object object_0;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Expected O, but got Unknown
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class538 stateMachine = this;
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
			long_0 = aitoolContext_0.GetParameter<long>("system_type_id", 0L);
			AIToolResult result;
			if (long_0 <= 0L)
			{
				result = AIToolResult.Fail("query_size_ranges 操作需要指定 system_type_id 参数");
			}
			else
			{
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
						bool_0 = insulationQueryTool_0.method_12(mepsystemType_0);
						if (bool_0)
						{
							object_0 = insulationQueryTool_0.method_8(document_0, long_0);
						}
						else
						{
							object_0 = insulationQueryTool_0.method_9(document_0, long_0);
						}
						Logger.Info("[InsulationQueryTool] QuerySizeRanges: 成功查询系统 " + ((Element)mepsystemType_0).Name + " 的尺寸分档");
						result = AIToolResult.Ok("成功查询系统 '" + ((Element)mepsystemType_0).Name + "' 的尺寸分档统计", (object)new Class220<long, string, string, object>(long_0, ((Element)mepsystemType_0).Name, bool_0 ? "水管" : "风管", object_0));
					}
				}
				catch (Exception ex)
				{
					exception_0 = ex;
					Logger.Error("[InsulationQueryTool] QuerySizeRanges: " + exception_0.Message);
					result = AIToolResult.Fail("查询尺寸分档失败: " + exception_0.Message);
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
	private sealed class Class539 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public string string_0;

		public InsulationQueryTool insulationQueryTool_0;

		private List<object> list_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private double double_0;

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
					Class539 stateMachine = this;
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
				ienumerable_0 = insulationQueryTool_0.method_5(document_0);
				list_0.AddRange(ienumerable_0);
				ienumerable_0 = null;
			}
			if (string_0 == "duct" || string_0 == "all")
			{
				ienumerable_1 = insulationQueryTool_0.method_6(document_0);
				list_0.AddRange(ienumerable_1);
				ienumerable_1 = null;
			}
			int_1 = list_0.Sum(delegate(object object_0)
			{
				if (Class533.callSite_0 == null)
				{
					Class533.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "element_count", typeof(InsulationQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				return (dynamic)Class533.callSite_0.Target(Class533.callSite_0, object_0);
			});
			int_2 = list_0.Sum(delegate(object object_0)
			{
				if (Class533.callSite_2 == null)
				{
					Class533.callSite_2 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "insulated_count", typeof(InsulationQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				return (dynamic)Class533.callSite_2.Target(Class533.callSite_2, object_0);
			});
			int_3 = int_1 - int_2;
			double_0 = ((int_1 > 0) ? Math.Round((double)int_2 / (double)int_1 * 100.0, 1) : 0.0);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[InsulationQueryTool] QuerySystems: 返回 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个系统的保温概况");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(25, 4);
			defaultInterpolatedStringHandler2.AppendLiteral("成功查询 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个系统的保温概况，总体保温率 ");
			defaultInterpolatedStringHandler2.AppendFormatted(double_0);
			defaultInterpolatedStringHandler2.AppendLiteral("%（");
			defaultInterpolatedStringHandler2.AppendFormatted(int_2);
			defaultInterpolatedStringHandler2.AppendLiteral("/");
			defaultInterpolatedStringHandler2.AppendFormatted(int_1);
			defaultInterpolatedStringHandler2.AppendLiteral("）");
			AIToolResult result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class217<int, int, int, int, double, List<object>>(list_0.Count, int_1, int_2, int_3, double_0, list_0));
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
	private sealed class Class540 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public string string_0;

		public InsulationQueryTool insulationQueryTool_0;

		private List<object> list_0;

		private int int_1;

		private int int_2;

		private IOrderedEnumerable<_003C_003Ef__AnonymousType227<long, string, string>> iorderedEnumerable_0;

		private IOrderedEnumerable<_003C_003Ef__AnonymousType227<long, string, string>> iorderedEnumerable_1;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class540 stateMachine = this;
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
				iorderedEnumerable_0 = from PipeInsulationType pipeInsulationType_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(PipeInsulationType))
					select new _003C_003Ef__AnonymousType227<long, string, string>(((Element)pipeInsulationType_0).Id.Value, ((Element)pipeInsulationType_0).Name, "水管保温") into _003C_003Ef__AnonymousType227_0
					orderby _003C_003Ef__AnonymousType227_0.type_name
					select _003C_003Ef__AnonymousType227_0;
				list_0.AddRange(iorderedEnumerable_0);
				iorderedEnumerable_0 = null;
			}
			if (string_0 == "duct" || string_0 == "all")
			{
				iorderedEnumerable_1 = from DuctInsulationType ductInsulationType_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(DuctInsulationType))
					select new _003C_003Ef__AnonymousType227<long, string, string>(((Element)ductInsulationType_0).Id.Value, ((Element)ductInsulationType_0).Name, "风管保温") into _003C_003Ef__AnonymousType227_0
					orderby _003C_003Ef__AnonymousType227_0.type_name
					select _003C_003Ef__AnonymousType227_0;
				list_0.AddRange(iorderedEnumerable_1);
				iorderedEnumerable_1 = null;
			}
			int_1 = ((!(string_0 == "duct")) ? ((string_0 == "pipe") ? list_0.Count : list_0.Count(delegate(object object_0)
			{
				if (Class532.callSite_0 == null)
				{
					Class532.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "element_type", typeof(InsulationQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				return (dynamic)Class532.callSite_0.Target(Class532.callSite_0, object_0) == "水管保温";
			})) : 0);
			int_2 = list_0.Count - int_1;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[InsulationQueryTool] QueryTypes: 返回 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个保温类型");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(23, 3);
			defaultInterpolatedStringHandler2.AppendLiteral("成功查询 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个保温类型（水管: ");
			defaultInterpolatedStringHandler2.AppendFormatted(int_1);
			defaultInterpolatedStringHandler2.AppendLiteral(", 风管: ");
			defaultInterpolatedStringHandler2.AppendFormatted(int_2);
			defaultInterpolatedStringHandler2.AppendLiteral("）");
			AIToolResult result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class219<int, int, int, List<object>>(list_0.Count, int_1, int_2, list_0));
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
	private sealed class Class541 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public string string_0;

		public AIToolContext aitoolContext_0;

		public InsulationQueryTool insulationQueryTool_0;

		private List<object> list_0;

		private int int_1;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private IEnumerable<object> ienumerable_0;

		private IEnumerable<object> ienumerable_1;

		private PropertyInfo propertyInfo_0;

		private object object_0;

		private PropertyInfo propertyInfo_1;

		private string string_2;

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
					Class541 stateMachine = this;
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
				ienumerable_0 = insulationQueryTool_0.method_10(document_0);
				list_0.AddRange(ienumerable_0);
				ienumerable_0 = null;
			}
			if (string_0 == "duct" || string_0 == "all")
			{
				ienumerable_1 = insulationQueryTool_0.method_11(document_0);
				list_0.AddRange(ienumerable_1);
				ienumerable_1 = null;
			}
			int_1 = list_0.Count;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[InsulationQueryTool] QueryUninsulated: 返回 ");
			defaultInterpolatedStringHandler.AppendFormatted(int_1);
			defaultInterpolatedStringHandler.AppendLiteral(" 个未保温元素");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			AIToolResult result;
			if (aitoolContext_0.DataCache == null || string.IsNullOrEmpty(aitoolContext_0.SessionId))
			{
				Logger.Warning("[InsulationQueryTool] 数据缓存或会话ID不可用，无法缓存未保温元素结果");
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(12, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("成功查询 ");
				defaultInterpolatedStringHandler2.AppendFormatted(int_1);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个未保温元素");
				result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class221<int, List<object>>(int_1, list_0.Take(100).ToList()));
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("insulation_uninsulated_");
				defaultInterpolatedStringHandler3.AppendFormatted(DateTime.Now, "yyyyMMddHHmmssfff");
				string_1 = defaultInterpolatedStringHandler3.ToStringAndClear();
				aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)list_0, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_1, "未保温元素", 100);
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
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(90, 2);
										defaultInterpolatedStringHandler4.AppendLiteral("成功查询 ");
										defaultInterpolatedStringHandler4.AppendFormatted(int_1);
										defaultInterpolatedStringHandler4.AppendLiteral(" 个未保温元素\n\n💡 完整结果已缓存，在后续工具调用中使用 cacheId=\"");
										defaultInterpolatedStringHandler4.AppendFormatted(string_2);
										defaultInterpolatedStringHandler4.AppendLiteral("\" 来引用这些元素（如 insulation_manager 的 element_ids）");
										obj.Message = defaultInterpolatedStringHandler4.ToStringAndClear();
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
			int_0 = -2;
			list_0 = null;
			string_1 = null;
			aitoolResult_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	private static readonly double[] double_0 = new double[20]
	{
		15.0, 20.0, 25.0, 32.0, 40.0, 50.0, 65.0, 75.0, 80.0, 100.0,
		125.0, 150.0, 200.0, 250.0, 300.0, 350.0, 400.0, 450.0, 500.0, 600.0
	};

	public string Name => "insulation_query";

	public string Category => "保温查询";

	public string Description => "查询 MEP 元素的保温状态，包括系统保温概况、元素保温状态、保温类型、尺寸分档统计等";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operation\": {\n                \"type\": \"string\",\n                \"enum\": [\n                    \"list_systems\",\n                    \"query_elements\",\n                    \"query_types\",\n                    \"query_size_ranges\",\n                    \"query_uninsulated\"\n                ],\n                \"description\": \"查询操作类型：list_systems(系统保温概况), query_elements(元素保温状态，必须提供 element_ids), query_types(保温类型), query_size_ranges(尺寸分档，必须提供 system_type_id), query_uninsulated(未保温元素)\"\n            },\n            \"target\": {\n                \"type\": \"string\",\n                \"enum\": [\"pipe\", \"duct\", \"all\"],\n                \"description\": \"目标类型：pipe(水管)、duct(风管)、all(全部)\"\n            },\n            \"system_type_id\": {\n                \"type\": \"integer\",\n                \"description\": \"系统类型 ID（query_size_ranges 操作需要）。可通过 mep_system_query 的 list_systems 操作获取\"\n            },\n            \"element_ids\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"元素 ID 列表（query_elements 操作使用）。可通过 query_uninsulated 或 query_elements 结果中的 element_id 获取\"\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（query_elements 操作使用，与 element_ids 二选一）。引用之前查询缓存的完整结果\"\n            },\n            \"limit\": {\n                \"type\": \"integer\",\n                \"description\": \"已废弃：query_uninsulated 将查询全部未保温元素，不再限制数量，结果统一通过智能摘要返回前100条并缓存全部\"\n            }\n        },\n        \"required\": [\"operation\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class534))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class534 stateMachine = new Class534();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.insulationQueryTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class539))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_0(Document document_0, string string_0)
	{
		Class539 stateMachine = new Class539();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.insulationQueryTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.string_0 = string_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class537))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(Document document_0, string string_0, AIToolContext aitoolContext_0)
	{
		Class537 stateMachine = new Class537();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.insulationQueryTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.string_0 = string_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private static List<int>? smethod_0(AIToolContext aitoolContext_0, string? string_0)
	{
		if (string.IsNullOrEmpty(string_0))
		{
			return null;
		}
		object cachedData = aitoolContext_0.GetCachedData<object>(string_0);
		if (cachedData == null)
		{
			return null;
		}
		List<int> list = new List<int>();
		int int_2;
		if (cachedData is IEnumerable enumerable)
		{
			foreach (object item in enumerable)
			{
				if (item != null && (smethod_1(item, "element_id", out var int_) || smethod_1(item, "id", out int_)))
				{
					list.Add(int_);
				}
			}
		}
		else if (smethod_1(cachedData, "element_id", out int_2) || smethod_1(cachedData, "id", out int_2))
		{
			list.Add(int_2);
		}
		return list.Distinct().ToList();
	}

	private static bool smethod_1(object object_0, string string_0, out int int_0)
	{
		int_0 = 0;
		try
		{
			if (object_0 is IDictionary dictionary && dictionary.Contains(string_0))
			{
				return smethod_4(dictionary[string_0], out int_0);
			}
			PropertyInfo property = object_0.GetType().GetProperty(string_0);
			if (property != null)
			{
				return smethod_4(property.GetValue(object_0), out int_0);
			}
		}
		catch
		{
		}
		return false;
	}

	private static List<int>? smethod_2(AIToolContext aitoolContext_0)
	{
		List<int> list = new List<int>();
		string[] array = new string[3]
		{
			"element_ids",
			"elementIds",
			"ids"
		};
		foreach (string text in array)
		{
			if (aitoolContext_0.HasParameter(text))
			{
				object parameter = aitoolContext_0.GetParameter<object>(text, (object)null);
				List<int> list2 = smethod_3(parameter);
				if (list2 != null && list2.Count > 0)
				{
					list.AddRange(list2);
				}
			}
		}
		string[] array2 = new string[2]
		{
			"element_id",
			"elementId"
		};
		foreach (string text2 in array2)
		{
			if (aitoolContext_0.HasParameter(text2))
			{
				object parameter2 = aitoolContext_0.GetParameter<object>(text2, (object)null);
				if (smethod_4(parameter2, out var int_))
				{
					list.Add(int_);
				}
			}
		}
		return list.Distinct().ToList();
	}

	private static List<int>? smethod_3(object? object_0)
	{
		if (object_0 == null)
		{
			return null;
		}
		List<int> list = new List<int>();
		if (smethod_4(object_0, out var int_))
		{
			list.Add(int_);
			return list;
		}
		if (object_0 is int[] collection)
		{
			list.AddRange(collection);
			return list;
		}
		if (object_0 is long[] array)
		{
			list.AddRange(Array.ConvertAll(array, (long long_0) => (int)long_0));
			return list;
		}
		if (object_0 is List<int> collection2)
		{
			list.AddRange(collection2);
			return list;
		}
		if (object_0 is List<long> list2)
		{
			list.AddRange(list2.ConvertAll((long long_0) => (int)long_0));
			return list;
		}
		if (object_0 is List<object> list3)
		{
			foreach (object item in list3)
			{
				if (smethod_4(item, out var int_2))
				{
					list.Add(int_2);
				}
			}
			return list;
		}
		if (object_0 is IEnumerable<object> enumerable)
		{
			foreach (object item2 in enumerable)
			{
				if (smethod_4(item2, out var int_3))
				{
					list.Add(int_3);
				}
			}
			return list;
		}
		JArray val = (JArray)((object_0 is JArray) ? object_0 : null);
		if (val != null)
		{
			foreach (JToken item3 in val)
			{
				if (smethod_4(item3, out var int_4))
				{
					list.Add(int_4);
				}
			}
			return list;
		}
		if (object_0 is string text)
		{
			string text2 = text.Trim();
			if (text2.StartsWith("[") && text2.EndsWith("]"))
			{
				try
				{
					JArray val2 = JArray.Parse(text2);
					foreach (JToken item4 in val2)
					{
						if (smethod_4(item4, out var int_5))
						{
							list.Add(int_5);
						}
					}
				}
				catch
				{
				}
			}
			if (list.Count == 0)
			{
				string[] array2 = text2.Split(new char[3] { ',', '，', ' ' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text3 in array2)
				{
					if (int.TryParse(text3.Trim(), out var result))
					{
						list.Add(result);
					}
				}
			}
			return list;
		}
		return (list.Count > 0) ? list : null;
	}

	private static bool smethod_4(object? object_0, out int int_0)
	{
		int_0 = 0;
		if (object_0 == null)
		{
			return false;
		}
		if (object_0 is int num)
		{
			int_0 = num;
			return true;
		}
		if (object_0 is long num2)
		{
			int_0 = (int)num2;
			return true;
		}
		if (object_0 is short num3)
		{
			int_0 = num3;
			return true;
		}
		if (object_0 is byte b)
		{
			int_0 = b;
			return true;
		}
		JValue val = (JValue)((object_0 is JValue) ? object_0 : null);
		if (val != null && val.Value != null)
		{
			return smethod_4(val.Value, out int_0);
		}
		if (object_0 is string text && int.TryParse(text.Trim(), out var result))
		{
			int_0 = result;
			return true;
		}
		if (object_0 is double num4 && num4 == Math.Floor(num4) && num4 <= 2147483647.0 && num4 >= -2147483648.0)
		{
			int_0 = (int)num4;
			return true;
		}
		return false;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class540))]
	private Task<AIToolResult> method_2(Document document_0, string string_0)
	{
		Class540 stateMachine = new Class540();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.insulationQueryTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.string_0 = string_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class538))]
	private Task<AIToolResult> method_3(Document document_0, string string_0, AIToolContext aitoolContext_0)
	{
		Class538 stateMachine = new Class538();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.insulationQueryTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.string_0 = string_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class541))]
	private Task<AIToolResult> method_4(Document document_0, string string_0, AIToolContext aitoolContext_0)
	{
		Class541 stateMachine = new Class541();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.insulationQueryTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.string_0 = string_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private IEnumerable<object> method_5(Document document_0)
	{
		List<Pipe> source = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Pipe))).Cast<Pipe>().ToList();
		List<FamilyInstance> source2 = method_22(document_0);
		Dictionary<ElementId, PipeInsulation> dictionary_0 = new Dictionary<ElementId, PipeInsulation>();
		foreach (PipeInsulation item in ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(PipeInsulation))).Cast<PipeInsulation>())
		{
			try
			{
				ElementId hostElementId = ((InsulationLiningBase)item).HostElementId;
				if (hostElementId != (ElementId)null && hostElementId != ElementId.InvalidElementId)
				{
					dictionary_0[hostElementId] = item;
				}
			}
			catch
			{
			}
		}
		IEnumerable<IGrouping<long, Pipe>> enumerable = from pipe_0 in source
			where method_18(document_0, pipe_0) > 0L
			group pipe_0 by method_18(document_0, pipe_0);
		Dictionary<long, string> dictionary = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(PipingSystemType))).Cast<PipingSystemType>().ToDictionary((PipingSystemType pipingSystemType_0) => ((Element)pipingSystemType_0).Id.Value, (PipingSystemType pipingSystemType_0) => ((Element)pipingSystemType_0).Name);
		foreach (IGrouping<long, Pipe> igrouping_0 in enumerable)
		{
			List<Pipe> list = igrouping_0.ToList();
			int num = list.Count((Pipe pipe_0) => dictionary_0.ContainsKey(((Element)pipe_0).Id));
			List<FamilyInstance> list2 = source2.Where((FamilyInstance familyInstance_0) => method_19(document_0, familyInstance_0) == igrouping_0.Key).ToList();
			int num2 = list2.Count((FamilyInstance familyInstance_0) => dictionary_0.ContainsKey(((Element)familyInstance_0).Id));
			int num3 = list.Count + list2.Count;
			int num4 = num + num2;
			string value;
			yield return new Class222<long, string, string, int, int, int, double, int, int, int, int>(igrouping_0.Key, dictionary.TryGetValue(igrouping_0.Key, out value) ? value : "未知系统", "水管", num3, num4, num3 - num4, (num3 > 0) ? Math.Round((double)num4 / (double)num3 * 100.0, 1) : 0.0, list.Count, num, list2.Count, num2);
			value = null;
		}
	}

	private IEnumerable<object> method_6(Document document_0)
	{
		List<Duct> source = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Duct))).Cast<Duct>().ToList();
		List<FamilyInstance> source2 = method_23(document_0);
		Dictionary<ElementId, DuctInsulation> dictionary_0 = new Dictionary<ElementId, DuctInsulation>();
		foreach (DuctInsulation item in ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(DuctInsulation))).Cast<DuctInsulation>())
		{
			try
			{
				ElementId hostElementId = ((InsulationLiningBase)item).HostElementId;
				if (hostElementId != (ElementId)null && hostElementId != ElementId.InvalidElementId)
				{
					dictionary_0[hostElementId] = item;
				}
			}
			catch
			{
			}
		}
		IEnumerable<IGrouping<long, Duct>> enumerable = from duct_0 in source
			where method_20(duct_0) > 0L
			group duct_0 by method_20(duct_0);
		Dictionary<long, string> dictionary = new Dictionary<long, string>();
		foreach (MEPSystemType item2 in new FilteredElementCollector(document_0).OfClass(typeof(MEPSystemType)))
		{
			MEPSystemType val = item2;
			if (method_13(val))
			{
				dictionary[((Element)val).Id.Value] = ((Element)val).Name;
			}
		}
		foreach (IGrouping<long, Duct> igrouping_0 in enumerable)
		{
			List<Duct> list = igrouping_0.ToList();
			int num = list.Count((Duct duct_0) => dictionary_0.ContainsKey(((Element)duct_0).Id));
			List<FamilyInstance> list2 = source2.Where((FamilyInstance familyInstance_0) => method_21(document_0, familyInstance_0) == igrouping_0.Key).ToList();
			int num2 = list2.Count((FamilyInstance familyInstance_0) => dictionary_0.ContainsKey(((Element)familyInstance_0).Id));
			int num3 = list.Count + list2.Count;
			int num4 = num + num2;
			string value;
			yield return new Class223<long, string, string, int, int, int, double, int, int, int, int>(igrouping_0.Key, dictionary.TryGetValue(igrouping_0.Key, out value) ? value : "未知系统", "风管", num3, num4, num3 - num4, (num3 > 0) ? Math.Round((double)num4 / (double)num3 * 100.0, 1) : 0.0, list.Count, num, list2.Count, num2);
			value = null;
		}
	}

	private object? method_7(Document document_0, Element element_0)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Expected O, but got Unknown
		try
		{
			long? gparam_ = null;
			string text = null;
			string text2 = null;
			bool gparam_2 = false;
			string text3 = null;
			double? num = null;
			Element obj = element_0;
			Pipe val = (Pipe)(object)((obj is Pipe) ? obj : null);
			if (val != null)
			{
				text2 = "管道";
				long num2 = method_18(document_0, val);
				if (num2 > 0L)
				{
					gparam_ = num2;
				}
				PipeInsulation val2 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(PipeInsulation))).Cast<PipeInsulation>().FirstOrDefault((PipeInsulation pipeInsulation_0) => ((InsulationLiningBase)pipeInsulation_0).HostElementId == element_0.Id);
				if (val2 != null)
				{
					gparam_2 = true;
					text3 = ((Element)val2).Name;
					num = method_24(val2);
				}
				MEPSystem mEPSystem = ((MEPCurve)val).MEPSystem;
				text = ((mEPSystem != null) ? ((Element)mEPSystem).Name : null);
			}
			else
			{
				Element obj2 = element_0;
				Duct val3 = (Duct)(object)((obj2 is Duct) ? obj2 : null);
				if (val3 != null)
				{
					text2 = "风管";
					long num3 = method_20(val3);
					if (num3 > 0L)
					{
						gparam_ = num3;
					}
					DuctInsulation val4 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(DuctInsulation))).Cast<DuctInsulation>().FirstOrDefault((DuctInsulation ductInsulation_0) => ((InsulationLiningBase)ductInsulation_0).HostElementId == element_0.Id);
					if (val4 != null)
					{
						gparam_2 = true;
						text3 = ((Element)val4).Name;
						num = method_25(val4);
					}
					MEPSystem mEPSystem2 = ((MEPCurve)val3).MEPSystem;
					text = ((mEPSystem2 != null) ? ((Element)mEPSystem2).Name : null);
				}
				else
				{
					Element obj3 = element_0;
					FamilyInstance val5 = (FamilyInstance)(object)((obj3 is FamilyInstance) ? obj3 : null);
					if (val5 != null)
					{
						Parameter val6 = ((Element)val5).get_Parameter((BuiltInParameter)(-1140334L));
						if (val6 != null && val6.HasValue)
						{
							text2 = "管件";
							gparam_ = val6.AsElementId().Value;
							PipeInsulation val7 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(PipeInsulation))).Cast<PipeInsulation>().FirstOrDefault((PipeInsulation pipeInsulation_0) => ((InsulationLiningBase)pipeInsulation_0).HostElementId == element_0.Id);
							if (val7 != null)
							{
								gparam_2 = true;
								text3 = ((Element)val7).Name;
								num = method_24(val7);
							}
							Element element = document_0.GetElement(new ElementId(gparam_.Value));
							PipingSystemType val8 = (PipingSystemType)(object)((element is PipingSystemType) ? element : null);
							text = ((val8 != null) ? ((Element)val8).Name : null);
						}
						else
						{
							Parameter val9 = ((Element)val5).get_Parameter((BuiltInParameter)(-1140333L));
							if (val9 != null && val9.HasValue)
							{
								text2 = "风管管件";
								gparam_ = val9.AsElementId().Value;
								DuctInsulation val10 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(DuctInsulation))).Cast<DuctInsulation>().FirstOrDefault((DuctInsulation ductInsulation_0) => ((InsulationLiningBase)ductInsulation_0).HostElementId == element_0.Id);
								if (val10 != null)
								{
									gparam_2 = true;
									text3 = ((Element)val10).Name;
									num = method_25(val10);
								}
								Element element2 = document_0.GetElement(new ElementId(gparam_.Value));
								MEPSystemType val11 = (MEPSystemType)(object)((element2 is MEPSystemType) ? element2 : null);
								text = ((val11 != null) ? ((Element)val11).Name : null);
							}
						}
					}
				}
			}
			if (text2 == null)
			{
				return null;
			}
			return new Class224<long, string, long?, string, bool, string, double?>(element_0.Id.Value, text2, gparam_, text ?? "未知系统", gparam_2, text3 ?? "", num.HasValue ? new double?(Math.Round(num.Value * 304.8, 2)) : ((double?)null));
		}
		catch
		{
			return null;
		}
	}

	private object method_8(Document document_0, long long_0)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		List<Pipe> source = (from Pipe pipe_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Pipe))
			where method_18(document_0, pipe_0) == long_0
			select pipe_0).ToList();
		Dictionary<ElementId, PipeInsulation> dictionary_0 = new Dictionary<ElementId, PipeInsulation>();
		foreach (PipeInsulation item in ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(PipeInsulation))).Cast<PipeInsulation>())
		{
			try
			{
				dictionary_0[((InsulationLiningBase)item).HostElementId] = item;
			}
			catch
			{
			}
		}
		var orderedEnumerable = from pipe_0 in source
			select new
			{
				Pipe = pipe_0,
				DiameterMM = ((MEPCurve)pipe_0).Diameter * 304.8,
				Step = smethod_5(((MEPCurve)pipe_0).Diameter * 304.8)
			} into _003C_003Ef__AnonymousType234_0
			group _003C_003Ef__AnonymousType234_0 by _003C_003Ef__AnonymousType234_0.Step into igrouping_0
			orderby igrouping_0.Key
			select igrouping_0;
		List<object> list = new List<object>();
		foreach (var item2 in orderedEnumerable)
		{
			List<Pipe> list2 = item2.Select(_003C_003Ef__AnonymousType234_0 => _003C_003Ef__AnonymousType234_0.Pipe).ToList();
			int gparam_ = list2.Count((Pipe pipe_0) => dictionary_0.ContainsKey(((Element)pipe_0).Id));
			(string, string) tuple = method_26(dictionary_0, list2);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
			defaultInterpolatedStringHandler.AppendLiteral("DN");
			defaultInterpolatedStringHandler.AppendFormatted(item2.Key, "0");
			list.Add(new Class225<string, double, double, int, int, string, string>(defaultInterpolatedStringHandler.ToStringAndClear(), Math.Round(item2.Min(_003C_003Ef__AnonymousType234_0 => _003C_003Ef__AnonymousType234_0.DiameterMM), 1), Math.Round(item2.Max(_003C_003Ef__AnonymousType234_0 => _003C_003Ef__AnonymousType234_0.DiameterMM), 1), list2.Count, gparam_, tuple.Item1 ?? "0", tuple.Item2 ?? ""));
		}
		return new Class226<long, List<object>>(long_0, list);
	}

	private static double smethod_5(double double_1)
	{
		if (double_1 <= double_0[0])
		{
			return double_0[0];
		}
		double num = double_0[double_0.Length - 1];
		if (double_1 >= num)
		{
			return num;
		}
		int num2 = 0;
		double num3;
		double num4;
		while (true)
		{
			if (num2 < double_0.Length - 1)
			{
				num3 = double_0[num2];
				num4 = double_0[num2 + 1];
				if (double_1 >= num3 && double_1 < num4)
				{
					break;
				}
				num2++;
				continue;
			}
			return num;
		}
		return (double_1 - num3 <= num4 - double_1) ? num3 : num4;
	}

	private object method_9(Document document_0, long long_0)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		List<Duct> source = (from Duct duct_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Duct))
			where method_20(duct_0) == long_0
			select duct_0).ToList();
		Dictionary<ElementId, DuctInsulation> dictionary_0 = new Dictionary<ElementId, DuctInsulation>();
		foreach (DuctInsulation item in ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(DuctInsulation))).Cast<DuctInsulation>())
		{
			try
			{
				dictionary_0[((InsulationLiningBase)item).HostElementId] = item;
			}
			catch
			{
			}
		}
		var orderedEnumerable = from _003C_003Ef__AnonymousType237_0 in source.Select(delegate(Duct duct_0)
			{
				var (num, num2, num3) = method_16(duct_0);
				if (num3 > 0.0)
				{
					double num4 = Math.Round(num3 * 304.8);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Φ");
					defaultInterpolatedStringHandler.AppendFormatted(num4, "0");
					return new
					{
						Key = defaultInterpolatedStringHandler.ToStringAndClear(),
						SortSize = num4,
						SizeMM = num4,
						Duct = duct_0
					};
				}
				double val = Math.Round(num * 304.8);
				double val2 = Math.Round(num2 * 304.8);
				double num5 = Math.Max(val, val2);
				double value = Math.Min(val, val2);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler2.AppendFormatted(num5, "0");
				defaultInterpolatedStringHandler2.AppendLiteral("x");
				defaultInterpolatedStringHandler2.AppendFormatted(value, "0");
				return new
				{
					Key = defaultInterpolatedStringHandler2.ToStringAndClear(),
					SortSize = num5,
					SizeMM = num5,
					Duct = duct_0
				};
			})
			group _003C_003Ef__AnonymousType237_0 by _003C_003Ef__AnonymousType237_0.Key into igrouping_0
			orderby igrouping_0.First().SortSize
			select igrouping_0;
		List<object> list = new List<object>();
		foreach (var item2 in orderedEnumerable)
		{
			List<Duct> list2 = item2.Select(_003C_003Ef__AnonymousType237_0 => _003C_003Ef__AnonymousType237_0.Duct).ToList();
			int gparam_ = list2.Count((Duct duct_0) => dictionary_0.ContainsKey(((Element)duct_0).Id));
			(string, string) tuple = method_27(dictionary_0, list2);
			list.Add(new Class227<string, double, double, int, int, string, string>(item2.Key, item2.First().SizeMM, item2.First().SizeMM, list2.Count, gparam_, tuple.Item1 ?? "0", tuple.Item2 ?? ""));
		}
		return new Class226<long, List<object>>(long_0, list);
	}

	private IEnumerable<object> method_10(Document document_0)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<ElementId, PipeInsulation> dictionary_0 = new Dictionary<ElementId, PipeInsulation>();
		foreach (PipeInsulation item in ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(PipeInsulation))).Cast<PipeInsulation>())
		{
			try
			{
				dictionary_0[((InsulationLiningBase)item).HostElementId] = item;
			}
			catch
			{
			}
		}
		List<object> list = new List<object>();
		var enumerable = from Pipe pipe_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Pipe))
			where !dictionary_0.ContainsKey(((Element)pipe_0).Id)
			select new
			{
				element_id = ((Element)pipe_0).Id.Value,
				element_type = "管道",
				diameter_mm = Math.Round(((MEPCurve)pipe_0).Diameter * 304.8, 1),
				system_type_id = method_18(document_0, pipe_0),
				length_feet = method_14(pipe_0),
				length_mm = Math.Round(method_14(pipe_0) * 304.8, 0)
			};
		foreach (var item2 in enumerable)
		{
			list.Add(item2);
		}
		var enumerable2 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(FamilyInstance))).Cast<FamilyInstance>().Where(delegate(FamilyInstance familyInstance_0)
		{
			Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140334L));
			return val != null && val.HasValue && !dictionary_0.ContainsKey(((Element)familyInstance_0).Id);
		}).Select(delegate(FamilyInstance familyInstance_0)
		{
			long value = ((Element)familyInstance_0).Id.Value;
			string element_type = "管件";
			Parameter obj2 = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140334L));
			return new
			{
				element_id = value,
				element_type = element_type,
				diameter_mm = 0.0,
				system_type_id = ((obj2 != null) ? new long?(obj2.AsElementId().Value) : ((long?)null)),
				length_feet = 0.0,
				length_mm = 0.0
			};
		});
		foreach (var item3 in enumerable2)
		{
			list.Add(item3);
		}
		return list;
	}

	private IEnumerable<object> method_11(Document document_0)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<ElementId, DuctInsulation> dictionary_0 = new Dictionary<ElementId, DuctInsulation>();
		foreach (DuctInsulation item in ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(DuctInsulation))).Cast<DuctInsulation>())
		{
			try
			{
				dictionary_0[((InsulationLiningBase)item).HostElementId] = item;
			}
			catch
			{
			}
		}
		List<object> list = new List<object>();
		IEnumerable<Class228<long, string, double, double, double, long, double, double>> enumerable = (from Duct duct_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Duct))
			where !dictionary_0.ContainsKey(((Element)duct_0).Id)
			select duct_0).Select(delegate(Duct duct_0)
		{
			var (num, num2, num3) = method_16(duct_0);
			return new Class228<long, string, double, double, double, long, double, double>(((Element)duct_0).Id.Value, "风管", (num3 > 0.0) ? 0.0 : Math.Round(num * 304.8, 0), (num3 > 0.0) ? 0.0 : Math.Round(num2 * 304.8, 0), (num3 > 0.0) ? Math.Round(num3 * 304.8, 0) : 0.0, method_20(duct_0), method_15(duct_0), Math.Round(method_15(duct_0) * 304.8, 0));
		});
		foreach (Class228<long, string, double, double, double, long, double, double> item2 in enumerable)
		{
			list.Add(item2);
		}
		var enumerable2 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(FamilyInstance))).Cast<FamilyInstance>().Where(delegate(FamilyInstance familyInstance_0)
		{
			Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140333L));
			return val != null && val.HasValue && !dictionary_0.ContainsKey(((Element)familyInstance_0).Id);
		}).Select(delegate(FamilyInstance familyInstance_0)
		{
			long value = ((Element)familyInstance_0).Id.Value;
			string element_type = "风管管件";
			Parameter obj2 = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140333L));
			return new
			{
				element_id = value,
				element_type = element_type,
				width_mm = 0.0,
				height_mm = 0.0,
				system_type_id = ((obj2 != null) ? new long?(obj2.AsElementId().Value) : ((long?)null)),
				length_feet = 0.0,
				length_mm = 0.0
			};
		});
		foreach (var item3 in enumerable2)
		{
			list.Add(item3);
		}
		return list;
	}

	private bool method_12(MEPSystemType mepsystemType_0)
	{
		return mepsystemType_0 is PipingSystemType;
	}

	private bool method_13(MEPSystemType mepsystemType_0)
	{
		return mepsystemType_0 is MechanicalSystemType;
	}

	private double method_14(Pipe pipe_0)
	{
		try
		{
			Parameter val = method_17((Element)(object)pipe_0, "Length");
			if (val != null && val.HasValue)
			{
				return val.AsDouble();
			}
		}
		catch
		{
		}
		return 0.0;
	}

	private double method_15(Duct duct_0)
	{
		try
		{
			Parameter val = method_17((Element)(object)duct_0, "Length");
			if (val != null && val.HasValue)
			{
				return val.AsDouble();
			}
		}
		catch
		{
		}
		return 0.0;
	}

	private (double width, double height, double diameter) method_16(Duct duct_0)
	{
		try
		{
			double diameter = ((MEPCurve)duct_0).Diameter;
			if (diameter > 0.0)
			{
				return (width: 0.0, height: 0.0, diameter: diameter);
			}
		}
		catch
		{
		}
		try
		{
			double width = ((MEPCurve)duct_0).Width;
			double height = ((MEPCurve)duct_0).Height;
			return (width: width, height: height, diameter: 0.0);
		}
		catch
		{
		}
		return (width: 0.0, height: 0.0, diameter: 0.0);
	}

	private Parameter? method_17(Element element_0, string string_0)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		try
		{
			foreach (Parameter parameter in element_0.Parameters)
			{
				Parameter val = parameter;
				if (val.Definition.Name.Equals(string_0, StringComparison.OrdinalIgnoreCase))
				{
					return val;
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private long method_18(Document document_0, Pipe pipe_0)
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

	private long method_19(Document document_0, FamilyInstance familyInstance_0)
	{
		try
		{
			Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140334L));
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

	private long method_20(Duct duct_0)
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

	private long method_21(Document document_0, FamilyInstance familyInstance_0)
	{
		try
		{
			Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140333L));
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

	private List<FamilyInstance> method_22(Document document_0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(FamilyInstance))).Cast<FamilyInstance>().Where(delegate(FamilyInstance familyInstance_0)
		{
			Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140334L));
			return val != null && val.HasValue;
		}).ToList();
	}

	private List<FamilyInstance> method_23(Document document_0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(FamilyInstance))).Cast<FamilyInstance>().Where(delegate(FamilyInstance familyInstance_0)
		{
			Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140333L));
			return val != null && val.HasValue;
		}).ToList();
	}

	private double? method_24(PipeInsulation pipeInsulation_0)
	{
		try
		{
			Parameter? obj = method_17((Element)(object)pipeInsulation_0, "Insulation Thickness");
			return (obj != null) ? new double?(obj.AsDouble()) : ((double?)null);
		}
		catch
		{
		}
		return null;
	}

	private double? method_25(DuctInsulation ductInsulation_0)
	{
		try
		{
			Parameter? obj = method_17((Element)(object)ductInsulation_0, "Insulation Thickness");
			return (obj != null) ? new double?(obj.AsDouble()) : ((double?)null);
		}
		catch
		{
		}
		return null;
	}

	private (string? thickness, string? material) method_26(Dictionary<ElementId, PipeInsulation> dictionary_0, List<Pipe> list_0)
	{
		List<Pipe> source = list_0.Where((Pipe pipe_0) => dictionary_0.ContainsKey(((Element)pipe_0).Id)).ToList();
		if (!source.Any())
		{
			return (thickness: null, material: null);
		}
		var anon = (from _003C_003Ef__AnonymousType242_0 in source.Select(delegate(Pipe pipe_0)
			{
				PipeInsulation val = dictionary_0[((Element)pipe_0).Id];
				Parameter? obj3 = method_17((Element)(object)val, "Insulation Thickness");
				double? num = ((obj3 != null) ? new double?(obj3.AsDouble()) : ((double?)null));
				string thickness = (num.HasValue ? Math.Round(num.Value * 304.8, 1).ToString() : "0");
				string name = ((Element)val).Name;
				return new
				{
					thickness = thickness,
					material = name
				};
			})
			group _003C_003Ef__AnonymousType242_0 by new { _003C_003Ef__AnonymousType242_0.thickness, _003C_003Ef__AnonymousType242_0.material } into igrouping_0
			orderby igrouping_0.Count() descending
			select igrouping_0).FirstOrDefault()?.FirstOrDefault();
		object obj;
		if (anon == null)
		{
			obj = null;
		}
		else
		{
			obj = anon.thickness;
			if (obj != null)
			{
				goto IL_00ce;
			}
		}
		obj = "0";
		goto IL_00ce;
		IL_00ea:
		object obj2;
		return (thickness: (string)obj, material: (string)obj2);
		IL_00ce:
		if (anon == null)
		{
			obj2 = null;
		}
		else
		{
			obj2 = anon.material;
			if (obj2 != null)
			{
				goto IL_00ea;
			}
		}
		obj2 = "";
		goto IL_00ea;
	}

	private (string? thickness, string? material) method_27(Dictionary<ElementId, DuctInsulation> dictionary_0, List<Duct> list_0)
	{
		List<Duct> source = list_0.Where((Duct duct_0) => dictionary_0.ContainsKey(((Element)duct_0).Id)).ToList();
		if (!source.Any())
		{
			return (thickness: null, material: null);
		}
		var anon = (from _003C_003Ef__AnonymousType242_0 in source.Select(delegate(Duct duct_0)
			{
				DuctInsulation val = dictionary_0[((Element)duct_0).Id];
				Parameter? obj3 = method_17((Element)(object)val, "Insulation Thickness");
				double? num = ((obj3 != null) ? new double?(obj3.AsDouble()) : ((double?)null));
				string thickness = (num.HasValue ? Math.Round(num.Value * 304.8, 1).ToString() : "0");
				string name = ((Element)val).Name;
				return new
				{
					thickness = thickness,
					material = name
				};
			})
			group _003C_003Ef__AnonymousType242_0 by new { _003C_003Ef__AnonymousType242_0.thickness, _003C_003Ef__AnonymousType242_0.material } into igrouping_0
			orderby igrouping_0.Count() descending
			select igrouping_0).FirstOrDefault()?.FirstOrDefault();
		object obj;
		if (anon == null)
		{
			obj = null;
		}
		else
		{
			obj = anon.thickness;
			if (obj != null)
			{
				goto IL_00ce;
			}
		}
		obj = "0";
		goto IL_00ce;
		IL_00ea:
		object obj2;
		return (thickness: (string)obj, material: (string)obj2);
		IL_00ce:
		if (anon == null)
		{
			obj2 = null;
		}
		else
		{
			obj2 = anon.material;
			if (obj2 != null)
			{
				goto IL_00ea;
			}
		}
		obj2 = "";
		goto IL_00ea;
	}
}
