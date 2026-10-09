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
using Newtonsoft.Json.Linq;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("insulation_manager", Category = "保温管理", Description = "管理 MEP 元素的保温层，支持添加、删除、修改保温厚度和材质等操作", RequiresTransaction = true, RequiresModification = true)]
public sealed class InsulationManagerTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class510
	{
		public ElementId elementId_0;

		internal bool method_0(PipeInsulation pipeInsulation_0)
		{
			return ((InsulationLiningBase)pipeInsulation_0).HostElementId == elementId_0;
		}

		internal bool method_1(DuctInsulation ductInsulation_0)
		{
			return ((InsulationLiningBase)ductInsulation_0).HostElementId == elementId_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class511
	{
		public string string_0;

		internal bool method_0(ElementType elementType_0)
		{
			Parameter val = ((Element)elementType_0).get_Parameter((BuiltInParameter)(-1002001L));
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
					goto IL_0026;
				}
			}
			obj = ((Element)elementType_0).Name;
			goto IL_0026;
			IL_0026:
			string text = (string)obj;
			return text.Equals(string_0, StringComparison.OrdinalIgnoreCase) || text.Contains(string_0);
		}
	}

	[CompilerGenerated]
	public sealed class Class512
	{
		public InsulationManagerTool insulationManagerTool_0;

		public Document document_0;

		public long long_0;

		internal bool method_0(Pipe pipe_0)
		{
			return insulationManagerTool_0.method_14(document_0, pipe_0) == long_0;
		}

		internal bool method_1(FamilyInstance familyInstance_0)
		{
			if ((int)((Element)familyInstance_0).Category.Id.Value != -2008049 && (int)((Element)familyInstance_0).Category.Id.Value != -2008055)
			{
				return false;
			}
			Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140334L));
			return val != null && val.HasValue && val.AsElementId().Value == long_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class513
	{
		public InsulationManagerTool insulationManagerTool_0;

		public long long_0;

		internal bool method_0(Duct duct_0)
		{
			return insulationManagerTool_0.method_15(duct_0) == long_0;
		}

		internal bool method_1(FamilyInstance familyInstance_0)
		{
			if ((int)((Element)familyInstance_0).Category.Id.Value != -2008010 && (int)((Element)familyInstance_0).Category.Id.Value != -2008016)
			{
				return false;
			}
			Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140333L));
			return val != null && val.HasValue && val.AsElementId().Value == long_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class514 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public AIToolContext aitoolContext_0;

		public InsulationManagerTool insulationManagerTool_0;

		private List<int> list_0;

		private double double_0;

		private string string_0;

		private bool bool_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private List<int> list_1;

		private double double_1;

		private IEnumerator<int> ienumerator_0;

		private int int_4;

		private Element element_0;

		private Element element_1;

		private Exception exception_0;

		private Exception exception_1;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Expected O, but got Unknown
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
					Class514 stateMachine = this;
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
			double_0 = aitoolContext_0.GetParameter<double>("thickness", 0.0);
			string_0 = aitoolContext_0.GetParameter<string>("material", (string)null) ?? "";
			bool_0 = aitoolContext_0.GetParameter<bool>("override_existing", true);
			AIToolResult result;
			if (list_0 == null || !list_0.Any())
			{
				result = AIToolResult.Fail("必须指定要添加保温的元素 ID 列表 (element_ids)");
			}
			else if (double_0 <= 0.0)
			{
				result = AIToolResult.Fail("必须指定有效的保温厚度 (thickness)");
			}
			else
			{
				try
				{
					int_1 = 0;
					int_2 = 0;
					int_3 = 0;
					list_1 = new List<int>();
					double_1 = double_0 / 304.8;
					ienumerator_0 = list_0.Take(100).GetEnumerator();
					try
					{
						while (ienumerator_0.MoveNext())
						{
							int_4 = ienumerator_0.Current;
							try
							{
								element_0 = document_0.GetElement(new ElementId((long)int_4));
								if (element_0 == null)
								{
									list_1.Add(int_4);
									continue;
								}
								element_1 = insulationManagerTool_0.method_5(document_0, element_0.Id, element_0);
								if (element_1 != null)
								{
									if (!bool_0)
									{
										int_3++;
										continue;
									}
									insulationManagerTool_0.method_8(element_1, double_1);
									if (!string.IsNullOrEmpty(string_0))
									{
										insulationManagerTool_0.method_10(document_0, element_1, string_0, element_0);
									}
									int_2++;
									goto IL_02e3;
								}
								if (insulationManagerTool_0.method_6(document_0, element_0, double_1, string_0))
								{
									int_1++;
								}
								else
								{
									list_1.Add(int_4);
								}
								goto IL_02e3;
								IL_02e3:
								element_0 = null;
								element_1 = null;
							}
							catch (Exception ex)
							{
								exception_0 = ex;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[InsulationManagerTool] AddToElements: 处理元素 ");
								defaultInterpolatedStringHandler.AppendFormatted(int_4);
								defaultInterpolatedStringHandler.AppendLiteral(" 失败 - ");
								defaultInterpolatedStringHandler.AppendFormatted(exception_0.Message);
								Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
								list_1.Add(int_4);
							}
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
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(57, 4);
					defaultInterpolatedStringHandler2.AppendLiteral("[InsulationManagerTool] AddToElements: 成功 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_1);
					defaultInterpolatedStringHandler2.AppendLiteral(", 修改 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_2);
					defaultInterpolatedStringHandler2.AppendLiteral(", 跳过 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_3);
					defaultInterpolatedStringHandler2.AppendLiteral(", 失败 ");
					defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(34, 4);
					defaultInterpolatedStringHandler3.AppendLiteral("成功为 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_1);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个元素添加保温，修改 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_2);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个现有保温，跳过 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_3);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个，失败 ");
					defaultInterpolatedStringHandler3.AppendFormatted(list_1.Count);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class212<int, int, int, int, List<int>, double, string>(int_1, int_2, int_3, list_1.Count, list_1, double_0, string_0));
				}
				catch (Exception ex)
				{
					exception_1 = ex;
					Logger.Error("[InsulationManagerTool] AddToElements: " + exception_1.Message);
					result = AIToolResult.Fail("添加保温失败: " + exception_1.Message);
				}
			}
			int_0 = -2;
			list_0 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class515 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public AIToolContext aitoolContext_0;

		public InsulationManagerTool insulationManagerTool_0;

		private long long_0;

		private double double_0;

		private string string_0;

		private bool bool_0;

		private MEPSystemType mepsystemType_0;

		private bool bool_1;

		private double double_1;

		private int int_1;

		private int int_2;

		private int int_3;

		private (int successCount, int modifiedCount, int skippedCount) valueTuple_0;

		private (int successCount, int modifiedCount, int skippedCount) valueTuple_1;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Expected O, but got Unknown
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class515 stateMachine = this;
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
			double_0 = aitoolContext_0.GetParameter<double>("thickness", 0.0);
			string_0 = aitoolContext_0.GetParameter<string>("material", (string)null) ?? "";
			bool_0 = aitoolContext_0.GetParameter<bool>("override_existing", true);
			AIToolResult result;
			if (long_0 <= 0L)
			{
				result = AIToolResult.Fail("必须指定有效的系统类型 ID (system_type_id)");
			}
			else if (double_0 <= 0.0)
			{
				result = AIToolResult.Fail("必须指定有效的保温厚度 (thickness)");
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
						bool_1 = insulationManagerTool_0.method_13(mepsystemType_0);
						double_1 = double_0 / 304.8;
						int_1 = 0;
						int_2 = 0;
						int_3 = 0;
						if (bool_1)
						{
							valueTuple_0 = insulationManagerTool_0.method_11(document_0, long_0, double_1, string_0, bool_0);
							int_1 = valueTuple_0.successCount;
							int_2 = valueTuple_0.modifiedCount;
							int_3 = valueTuple_0.skippedCount;
						}
						else
						{
							valueTuple_1 = insulationManagerTool_0.method_12(document_0, long_0, double_1, string_0, bool_0);
							int_1 = valueTuple_1.successCount;
							int_2 = valueTuple_1.modifiedCount;
							int_3 = valueTuple_1.skippedCount;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(58, 4);
						defaultInterpolatedStringHandler2.AppendLiteral("[InsulationManagerTool] AddToSystem: 系统 '");
						defaultInterpolatedStringHandler2.AppendFormatted(((Element)mepsystemType_0).Name);
						defaultInterpolatedStringHandler2.AppendLiteral("' - 成功 ");
						defaultInterpolatedStringHandler2.AppendFormatted(int_1);
						defaultInterpolatedStringHandler2.AppendLiteral(", 修改 ");
						defaultInterpolatedStringHandler2.AppendFormatted(int_2);
						defaultInterpolatedStringHandler2.AppendLiteral(", 跳过 ");
						defaultInterpolatedStringHandler2.AppendFormatted(int_3);
						Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(31, 4);
						defaultInterpolatedStringHandler3.AppendLiteral("成功为系统 '");
						defaultInterpolatedStringHandler3.AppendFormatted(((Element)mepsystemType_0).Name);
						defaultInterpolatedStringHandler3.AppendLiteral("' 添加保温：新增 ");
						defaultInterpolatedStringHandler3.AppendFormatted(int_1);
						defaultInterpolatedStringHandler3.AppendLiteral(" 个，修改 ");
						defaultInterpolatedStringHandler3.AppendFormatted(int_2);
						defaultInterpolatedStringHandler3.AppendLiteral(" 个，跳过 ");
						defaultInterpolatedStringHandler3.AppendFormatted(int_3);
						defaultInterpolatedStringHandler3.AppendLiteral(" 个");
						result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class216<long, string, string, int, int, int, double, string>(long_0, ((Element)mepsystemType_0).Name, bool_1 ? "水管" : "风管", int_1, int_2, int_3, double_0, string_0));
					}
				}
				catch (Exception ex)
				{
					exception_0 = ex;
					Logger.Error("[InsulationManagerTool] AddToSystem: " + exception_0.Message);
					result = AIToolResult.Fail("系统批量添加失败: " + exception_0.Message);
				}
			}
			int_0 = -2;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class516 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public InsulationManagerTool insulationManagerTool_0;

		private Document document_0;

		private string string_0;

		private string string_1;

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
					Class516 stateMachine = this;
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
								Logger.Info("[InsulationManagerTool] 执行操作: " + string_0);
								string_1 = string_0.ToLower();
								string text = string_1;
								if (!(text == "add_to_elements"))
								{
									if (!(text == "remove_from_elements"))
									{
										if (!(text == "modify_thickness"))
										{
											if (!(text == "modify_material"))
											{
												if (!(text == "add_to_system"))
												{
													val = AIToolResult.Fail("不支持的操作类型: " + string_0);
													break;
												}
												awaiter2 = insulationManagerTool_0.method_4(document_0, aitoolContext_0).GetAwaiter();
												if (!awaiter2.IsCompleted)
												{
													num = 5;
													int_0 = 5;
													taskAwaiter_1 = awaiter2;
													Class516 stateMachine = this;
													asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
													return;
												}
												goto IL_048a;
											}
											awaiter3 = insulationManagerTool_0.method_3(document_0, aitoolContext_0).GetAwaiter();
											if (!awaiter3.IsCompleted)
											{
												num = 4;
												int_0 = 4;
												taskAwaiter_1 = awaiter3;
												Class516 stateMachine = this;
												asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
												return;
											}
											goto IL_044f;
										}
										awaiter4 = insulationManagerTool_0.method_2(document_0, aitoolContext_0).GetAwaiter();
										if (!awaiter4.IsCompleted)
										{
											num = 3;
											int_0 = 3;
											taskAwaiter_1 = awaiter4;
											Class516 stateMachine = this;
											asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
											return;
										}
										goto IL_0414;
									}
									awaiter5 = insulationManagerTool_0.method_1(document_0, aitoolContext_0).GetAwaiter();
									if (!awaiter5.IsCompleted)
									{
										num = 2;
										int_0 = 2;
										taskAwaiter_1 = awaiter5;
										Class516 stateMachine = this;
										asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
										return;
									}
									goto IL_03d6;
								}
								awaiter6 = insulationManagerTool_0.method_0(document_0, aitoolContext_0).GetAwaiter();
								if (!awaiter6.IsCompleted)
								{
									num = 1;
									int_0 = 1;
									taskAwaiter_1 = awaiter6;
									Class516 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter6, ref stateMachine);
									return;
								}
								goto IL_0398;
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
					goto IL_0398;
				case 2:
					awaiter5 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_03d6;
				case 3:
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0414;
				case 4:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_044f;
				case 5:
					{
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						goto IL_048a;
					}
					IL_0414:
					aitoolResult_2 = awaiter4.GetResult();
					val = aitoolResult_2;
					aitoolResult_2 = null;
					break;
					IL_0398:
					aitoolResult_0 = awaiter6.GetResult();
					val = aitoolResult_0;
					aitoolResult_0 = null;
					break;
					IL_048a:
					aitoolResult_4 = awaiter2.GetResult();
					val = aitoolResult_4;
					aitoolResult_4 = null;
					break;
					IL_03d6:
					aitoolResult_1 = awaiter5.GetResult();
					val = aitoolResult_1;
					aitoolResult_1 = null;
					break;
					IL_044f:
					aitoolResult_3 = awaiter3.GetResult();
					val = aitoolResult_3;
					aitoolResult_3 = null;
					break;
				}
				result = val;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[InsulationManagerTool] 执行失败: " + exception_0.Message);
				result = AIToolResult.Fail("操作失败: " + exception_0.Message);
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
	public sealed class Class517 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public AIToolContext aitoolContext_0;

		public InsulationManagerTool insulationManagerTool_0;

		private List<int> list_0;

		private string string_0;

		private int int_1;

		private int int_2;

		private List<int> list_1;

		private IEnumerator<int> ienumerator_0;

		private int int_3;

		private Element element_0;

		private Element element_1;

		private Exception exception_0;

		private Exception exception_1;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Expected O, but got Unknown
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
					Class517 stateMachine = this;
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
			string_0 = aitoolContext_0.GetParameter<string>("material", (string)null);
			AIToolResult result;
			if (string.IsNullOrEmpty(string_0))
			{
				result = AIToolResult.Fail("必须指定保温材质名称 (material)");
			}
			else if (list_0 == null || !list_0.Any())
			{
				result = AIToolResult.Fail("必须指定要修改材质的元素 ID 列表 (element_ids)");
			}
			else
			{
				try
				{
					int_1 = 0;
					int_2 = 0;
					list_1 = new List<int>();
					ienumerator_0 = list_0.Take(100).GetEnumerator();
					try
					{
						while (ienumerator_0.MoveNext())
						{
							int_3 = ienumerator_0.Current;
							try
							{
								element_0 = document_0.GetElement(new ElementId((long)int_3));
								if (element_0 == null)
								{
									list_1.Add(int_3);
									continue;
								}
								element_1 = insulationManagerTool_0.method_5(document_0, element_0.Id, element_0);
								if (element_1 != null)
								{
									if (insulationManagerTool_0.method_10(document_0, element_1, string_0, element_0))
									{
										int_1++;
									}
									else
									{
										list_1.Add(int_3);
									}
								}
								else
								{
									int_2++;
								}
								element_0 = null;
								element_1 = null;
							}
							catch (Exception ex)
							{
								exception_0 = ex;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[InsulationManagerTool] ModifyMaterial: 处理元素 ");
								defaultInterpolatedStringHandler.AppendFormatted(int_3);
								defaultInterpolatedStringHandler.AppendLiteral(" 失败 - ");
								defaultInterpolatedStringHandler.AppendFormatted(exception_0.Message);
								Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
								list_1.Add(int_3);
							}
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
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(55, 3);
					defaultInterpolatedStringHandler2.AppendLiteral("[InsulationManagerTool] ModifyMaterial: 成功修改 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_1);
					defaultInterpolatedStringHandler2.AppendLiteral(", 跳过 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_2);
					defaultInterpolatedStringHandler2.AppendLiteral(", 失败 ");
					defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(29, 3);
					defaultInterpolatedStringHandler3.AppendLiteral("成功修改 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_1);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个元素的保温材质为 '");
					defaultInterpolatedStringHandler3.AppendFormatted(string_0);
					defaultInterpolatedStringHandler3.AppendLiteral("'，跳过 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_2);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个无保温元素");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class215<int, int, int, List<int>, string>(int_1, int_2, list_1.Count, list_1, string_0));
				}
				catch (Exception ex)
				{
					exception_1 = ex;
					Logger.Error("[InsulationManagerTool] ModifyMaterial: " + exception_1.Message);
					result = AIToolResult.Fail("修改材质失败: " + exception_1.Message);
				}
			}
			int_0 = -2;
			list_0 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class518 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public AIToolContext aitoolContext_0;

		public InsulationManagerTool insulationManagerTool_0;

		private List<int> list_0;

		private double double_0;

		private int int_1;

		private int int_2;

		private List<int> list_1;

		private double double_1;

		private IEnumerator<int> ienumerator_0;

		private int int_3;

		private Element element_0;

		private Element element_1;

		private Exception exception_0;

		private Exception exception_1;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Expected O, but got Unknown
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
					Class518 stateMachine = this;
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
			double_0 = aitoolContext_0.GetParameter<double>("thickness", 0.0);
			AIToolResult result;
			if (list_0 == null || !list_0.Any())
			{
				result = AIToolResult.Fail("必须指定要修改厚度的元素 ID 列表 (element_ids)");
			}
			else if (double_0 <= 0.0)
			{
				result = AIToolResult.Fail("必须指定有效的保温厚度 (thickness)");
			}
			else
			{
				try
				{
					int_1 = 0;
					int_2 = 0;
					list_1 = new List<int>();
					double_1 = double_0 / 304.8;
					ienumerator_0 = list_0.Take(100).GetEnumerator();
					try
					{
						while (ienumerator_0.MoveNext())
						{
							int_3 = ienumerator_0.Current;
							try
							{
								element_0 = document_0.GetElement(new ElementId((long)int_3));
								if (element_0 == null)
								{
									list_1.Add(int_3);
									continue;
								}
								element_1 = insulationManagerTool_0.method_5(document_0, element_0.Id, element_0);
								if (element_1 != null)
								{
									if (insulationManagerTool_0.method_8(element_1, double_1))
									{
										int_1++;
									}
									else
									{
										list_1.Add(int_3);
									}
								}
								else
								{
									int_2++;
								}
								element_0 = null;
								element_1 = null;
							}
							catch (Exception ex)
							{
								exception_0 = ex;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[InsulationManagerTool] ModifyThickness: 处理元素 ");
								defaultInterpolatedStringHandler.AppendFormatted(int_3);
								defaultInterpolatedStringHandler.AppendLiteral(" 失败 - ");
								defaultInterpolatedStringHandler.AppendFormatted(exception_0.Message);
								Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
								list_1.Add(int_3);
							}
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
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(56, 3);
					defaultInterpolatedStringHandler2.AppendLiteral("[InsulationManagerTool] ModifyThickness: 成功修改 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_1);
					defaultInterpolatedStringHandler2.AppendLiteral(", 跳过 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_2);
					defaultInterpolatedStringHandler2.AppendLiteral(", 失败 ");
					defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(29, 3);
					defaultInterpolatedStringHandler3.AppendLiteral("成功修改 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_1);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个元素的保温厚度为 ");
					defaultInterpolatedStringHandler3.AppendFormatted(double_0);
					defaultInterpolatedStringHandler3.AppendLiteral("mm，跳过 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_2);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个无保温元素");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class214<int, int, int, List<int>, double>(int_1, int_2, list_1.Count, list_1, double_0));
				}
				catch (Exception ex)
				{
					exception_1 = ex;
					Logger.Error("[InsulationManagerTool] ModifyThickness: " + exception_1.Message);
					result = AIToolResult.Fail("修改厚度失败: " + exception_1.Message);
				}
			}
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
	public sealed class Class519 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public AIToolContext aitoolContext_0;

		public InsulationManagerTool insulationManagerTool_0;

		private List<int> list_0;

		private int int_1;

		private int int_2;

		private List<int> list_1;

		private IEnumerator<int> ienumerator_0;

		private int int_3;

		private Element element_0;

		private Element element_1;

		private Exception exception_0;

		private Exception exception_1;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Expected O, but got Unknown
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
					Class519 stateMachine = this;
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
			AIToolResult result;
			if (list_0 == null || !list_0.Any())
			{
				result = AIToolResult.Fail("必须指定要删除保温的元素 ID 列表 (element_ids)");
			}
			else
			{
				try
				{
					int_1 = 0;
					int_2 = 0;
					list_1 = new List<int>();
					ienumerator_0 = list_0.Take(100).GetEnumerator();
					try
					{
						while (ienumerator_0.MoveNext())
						{
							int_3 = ienumerator_0.Current;
							try
							{
								element_0 = document_0.GetElement(new ElementId((long)int_3));
								if (element_0 == null)
								{
									list_1.Add(int_3);
									continue;
								}
								element_1 = insulationManagerTool_0.method_5(document_0, element_0.Id, element_0);
								if (element_1 != null)
								{
									document_0.Delete(element_1.Id);
									int_1++;
								}
								else
								{
									int_2++;
								}
								element_0 = null;
								element_1 = null;
							}
							catch (Exception ex)
							{
								exception_0 = ex;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[InsulationManagerTool] RemoveFromElements: 处理元素 ");
								defaultInterpolatedStringHandler.AppendFormatted(int_3);
								defaultInterpolatedStringHandler.AppendLiteral(" 失败 - ");
								defaultInterpolatedStringHandler.AppendFormatted(exception_0.Message);
								Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
								list_1.Add(int_3);
							}
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
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(59, 3);
					defaultInterpolatedStringHandler2.AppendLiteral("[InsulationManagerTool] RemoveFromElements: 成功删除 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_1);
					defaultInterpolatedStringHandler2.AppendLiteral(", 跳过 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_2);
					defaultInterpolatedStringHandler2.AppendLiteral(", 失败 ");
					defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(29, 3);
					defaultInterpolatedStringHandler3.AppendLiteral("成功删除 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_1);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个元素的保温，跳过 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_2);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个无保温元素，失败 ");
					defaultInterpolatedStringHandler3.AppendFormatted(list_1.Count);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class213<int, int, int, List<int>>(int_1, int_2, list_1.Count, list_1));
				}
				catch (Exception ex)
				{
					exception_1 = ex;
					Logger.Error("[InsulationManagerTool] RemoveFromElements: " + exception_1.Message);
					result = AIToolResult.Fail("删除保温失败: " + exception_1.Message);
				}
			}
			int_0 = -2;
			list_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "insulation_manager";

	public string Category => "保温管理";

	public string Description => "管理 MEP 元素的保温层，支持添加、删除、修改保温厚度和材质等操作";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operation\": {\n                \"type\": \"string\",\n                \"enum\": [\n                    \"add_to_elements\",\n                    \"remove_from_elements\",\n                    \"modify_thickness\",\n                    \"modify_material\",\n                    \"add_to_system\"\n                ],\n                \"description\": \"管理操作类型：add_to_elements(添加保温), remove_from_elements(删除保温), modify_thickness(修改厚度), modify_material(修改材质), add_to_system(系统批量添加)\"\n            },\n            \"element_ids\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"元素 ID 列表（添加/删除/修改操作使用）。支持：单个 ID、多个 ID 数组（如 [123,456]）。也可省略，改用 cacheId 引用之前查询缓存中的元素\"\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（可选）：引用之前查询操作（如 query_uninsulated）缓存的元素列表，使用缓存时无需传 element_ids\"\n            },\n            \"thickness\": {\n                \"type\": \"number\",\n                \"description\": \"保温厚度（毫米）\"\n            },\n            \"material\": {\n                \"type\": \"string\",\n                \"description\": \"保温材质名称（留空使用默认）\"\n            },\n            \"system_type_id\": {\n                \"type\": \"integer\",\n                \"description\": \"系统类型 ID（系统批量添加使用）。系统类型自动判断水管/风管，无需指定 target\"\n            },\n            \"override_existing\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否覆盖已有保温（默认 true）\"\n            }\n        },\n        \"required\": [\"operation\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class516))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class516 stateMachine = new Class516();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.insulationManagerTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class514))]
	private Task<AIToolResult> method_0(Document document_0, AIToolContext aitoolContext_0)
	{
		Class514 stateMachine = new Class514();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.insulationManagerTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class519))]
	private Task<AIToolResult> method_1(Document document_0, AIToolContext aitoolContext_0)
	{
		Class519 stateMachine = new Class519();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.insulationManagerTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class518))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_2(Document document_0, AIToolContext aitoolContext_0)
	{
		Class518 stateMachine = new Class518();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.insulationManagerTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class517))]
	private Task<AIToolResult> method_3(Document document_0, AIToolContext aitoolContext_0)
	{
		Class517 stateMachine = new Class517();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.insulationManagerTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class515))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_4(Document document_0, AIToolContext aitoolContext_0)
	{
		Class515 stateMachine = new Class515();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.insulationManagerTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private Element? method_5(Document document_0, ElementId elementId_0, Element element_0)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (element_0 is Pipe || smethod_0(element_0))
			{
				return (Element?)(object)((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(PipeInsulation))).Cast<PipeInsulation>().FirstOrDefault((PipeInsulation pipeInsulation_0) => ((InsulationLiningBase)pipeInsulation_0).HostElementId == elementId_0);
			}
			if (element_0 is Duct || smethod_1(element_0))
			{
				return (Element?)(object)((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(DuctInsulation))).Cast<DuctInsulation>().FirstOrDefault((DuctInsulation ductInsulation_0) => ((InsulationLiningBase)ductInsulation_0).HostElementId == elementId_0);
			}
		}
		catch
		{
		}
		return null;
	}

	private static bool smethod_0(Element element_0)
	{
		FamilyInstance val = (FamilyInstance)(object)((element_0 is FamilyInstance) ? element_0 : null);
		if (val == null)
		{
			return false;
		}
		return (int)((Element)val).Category.Id.Value == -2008049 || (int)((Element)val).Category.Id.Value == -2008055;
	}

	private static bool smethod_1(Element element_0)
	{
		FamilyInstance val = (FamilyInstance)(object)((element_0 is FamilyInstance) ? element_0 : null);
		if (val == null)
		{
			return false;
		}
		return (int)((Element)val).Category.Id.Value == -2008010 || (int)((Element)val).Category.Id.Value == -2008016;
	}

	private bool method_6(Document document_0, Element element_0, double double_0, string string_0)
	{
		try
		{
			Pipe val = (Pipe)(object)((element_0 is Pipe) ? element_0 : null);
			if (val != null)
			{
				ElementId val2 = method_7(document_0, "PipeInsulationType", string_0);
				if (val2 == (ElementId)null)
				{
					return false;
				}
				PipeInsulation.Create(document_0, ((Element)val).Id, val2, double_0);
				return true;
			}
			Duct val3 = (Duct)(object)((element_0 is Duct) ? element_0 : null);
			if (val3 != null)
			{
				ElementId val4 = method_7(document_0, "DuctInsulationType", string_0);
				if (val4 == (ElementId)null)
				{
					return false;
				}
				DuctInsulation.Create(document_0, ((Element)val3).Id, val4, double_0);
				return true;
			}
			FamilyInstance val5 = (FamilyInstance)(object)((element_0 is FamilyInstance) ? element_0 : null);
			if (val5 != null)
			{
				if (smethod_0((Element)(object)val5))
				{
					ElementId val6 = method_7(document_0, "PipeInsulationType", string_0);
					if (val6 == (ElementId)null)
					{
						return false;
					}
					PipeInsulation.Create(document_0, ((Element)val5).Id, val6, double_0);
					return true;
				}
				if (smethod_1((Element)(object)val5))
				{
					ElementId val7 = method_7(document_0, "DuctInsulationType", string_0);
					if (val7 == (ElementId)null)
					{
						return false;
					}
					DuctInsulation.Create(document_0, ((Element)val5).Id, val7, double_0);
					return true;
				}
			}
			Logger.Warning("[InsulationManagerTool] CreateInsulation: 不支持的元素类型 " + ((object)element_0).GetType().Name);
			return false;
		}
		catch (Exception ex)
		{
			Logger.Warning("[InsulationManagerTool] CreateInsulation: 创建保温失败 - " + ex.Message);
			return false;
		}
	}

	private ElementId? method_7(Document document_0, string string_0, string string_1)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Type type = ((string_0 == "PipeInsulationType") ? typeof(PipeInsulationType) : typeof(DuctInsulationType));
			List<ElementType> source = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(type)).Cast<ElementType>().ToList();
			if (!source.Any())
			{
				Logger.Warning("[InsulationManagerTool] 未找到 " + type.Name + "，请先创建保温类型");
				return null;
			}
			if (!string.IsNullOrEmpty(string_1))
			{
				ElementType val = source.FirstOrDefault(delegate(ElementType elementType_0)
				{
					Parameter val4 = ((Element)elementType_0).get_Parameter((BuiltInParameter)(-1002001L));
					object obj2;
					if (val4 == null)
					{
						obj2 = null;
					}
					else
					{
						obj2 = val4.AsString();
						if (obj2 != null)
						{
							goto IL_0026;
						}
					}
					obj2 = ((Element)elementType_0).Name;
					goto IL_0026;
					IL_0026:
					string text = (string)obj2;
					return text.Equals(string_1, StringComparison.OrdinalIgnoreCase) || text.Contains(string_1);
				});
				if (val != null)
				{
					return ((Element)val).Id;
				}
				try
				{
					ElementType val2 = source.First();
					ElementType val3 = val2.Duplicate(string_1);
					return (val3 != null) ? ((Element)val3).Id : null;
				}
				catch (Exception ex)
				{
					Logger.Warning("[InsulationManagerTool] 创建保温类型 '" + string_1 + "' 失败 - " + ex.Message);
					return null;
				}
			}
			return ((Element)source.First()).Id;
		}
		catch
		{
			return null;
		}
	}

	private bool method_8(Element element_0, double double_0)
	{
		try
		{
			PipeInsulation val = (PipeInsulation)(object)((element_0 is PipeInsulation) ? element_0 : null);
			if (val != null)
			{
				((InsulationLiningBase)val).Thickness = double_0;
				return true;
			}
			DuctInsulation val2 = (DuctInsulation)(object)((element_0 is DuctInsulation) ? element_0 : null);
			if (val2 != null)
			{
				((InsulationLiningBase)val2).Thickness = double_0;
				return true;
			}
			Parameter val3 = method_9(element_0, "Insulation Thickness");
			if (val3 != null && !((APIObject)val3).IsReadOnly)
			{
				val3.Set(double_0);
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logger.Warning("[InsulationManagerTool] ModifyInsulationThickness: 修改厚度失败 - " + ex.Message);
			return false;
		}
	}

	private Parameter? method_9(Element element_0, string string_0)
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

	private bool method_10(Document document_0, Element element_0, string string_0, Element element_1)
	{
		try
		{
			if (string.IsNullOrEmpty(string_0))
			{
				return false;
			}
			string string_1 = ((element_0 is PipeInsulation) ? "PipeInsulationType" : "DuctInsulationType");
			ElementId val = method_7(document_0, string_1, string_0);
			if (val == (ElementId)null || val == ElementId.InvalidElementId)
			{
				Logger.Warning("[InsulationManagerTool] ModifyInsulationMaterial: 未找到保温类型 '" + string_0 + "'");
				return false;
			}
			ElementId typeId = element_0.GetTypeId();
			if (typeId != val)
			{
				element_0.ChangeTypeId(val);
			}
			return true;
		}
		catch (Exception ex)
		{
			Logger.Warning("[InsulationManagerTool] ModifyInsulationMaterial: 修改材质失败 - " + ex.Message);
			return false;
		}
	}

	private (int successCount, int modifiedCount, int skippedCount) method_11(Document document_0, long long_0, double double_0, string string_0, bool bool_0)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		List<Pipe> list = (from Pipe pipe_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Pipe))
			where method_14(document_0, pipe_0) == long_0
			select pipe_0).ToList();
		List<FamilyInstance> list2 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(FamilyInstance))).Cast<FamilyInstance>().Where(delegate(FamilyInstance familyInstance_0)
		{
			if ((int)((Element)familyInstance_0).Category.Id.Value != -2008049 && (int)((Element)familyInstance_0).Category.Id.Value != -2008055)
			{
				return false;
			}
			Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140334L));
			return val != null && val.HasValue && val.AsElementId().Value == long_0;
		}).ToList();
		Dictionary<ElementId, PipeInsulation> dictionary = new Dictionary<ElementId, PipeInsulation>();
		foreach (PipeInsulation item in ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(PipeInsulation))).Cast<PipeInsulation>())
		{
			try
			{
				dictionary[((InsulationLiningBase)item).HostElementId] = item;
			}
			catch
			{
			}
		}
		foreach (Pipe item2 in list)
		{
			try
			{
				if (dictionary.ContainsKey(((Element)item2).Id))
				{
					if (bool_0)
					{
						if (method_8((Element)(object)dictionary[((Element)item2).Id], double_0))
						{
							num2++;
						}
					}
					else
					{
						num3++;
					}
				}
				else if (method_6(document_0, (Element)(object)item2, double_0, string_0))
				{
					num++;
				}
			}
			catch
			{
			}
		}
		foreach (FamilyInstance item3 in list2)
		{
			try
			{
				if (dictionary.ContainsKey(((Element)item3).Id))
				{
					if (bool_0)
					{
						if (method_8((Element)(object)dictionary[((Element)item3).Id], double_0))
						{
							num2++;
						}
					}
					else
					{
						num3++;
					}
				}
				else if (method_6(document_0, (Element)(object)item3, double_0, string_0))
				{
					num++;
				}
			}
			catch
			{
			}
		}
		return (successCount: num, modifiedCount: num2, skippedCount: num3);
	}

	private (int successCount, int modifiedCount, int skippedCount) method_12(Document document_0, long long_0, double double_0, string string_0, bool bool_0)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		List<Duct> list = (from Duct duct_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Duct))
			where method_15(duct_0) == long_0
			select duct_0).ToList();
		List<FamilyInstance> list2 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(FamilyInstance))).Cast<FamilyInstance>().Where(delegate(FamilyInstance familyInstance_0)
		{
			if ((int)((Element)familyInstance_0).Category.Id.Value != -2008010 && (int)((Element)familyInstance_0).Category.Id.Value != -2008016)
			{
				return false;
			}
			Parameter val = ((Element)familyInstance_0).get_Parameter((BuiltInParameter)(-1140333L));
			return val != null && val.HasValue && val.AsElementId().Value == long_0;
		}).ToList();
		Dictionary<ElementId, DuctInsulation> dictionary = new Dictionary<ElementId, DuctInsulation>();
		foreach (DuctInsulation item in ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(DuctInsulation))).Cast<DuctInsulation>())
		{
			try
			{
				dictionary[((InsulationLiningBase)item).HostElementId] = item;
			}
			catch
			{
			}
		}
		foreach (Duct item2 in list)
		{
			try
			{
				if (dictionary.ContainsKey(((Element)item2).Id))
				{
					if (bool_0)
					{
						if (method_8((Element)(object)dictionary[((Element)item2).Id], double_0))
						{
							num2++;
						}
					}
					else
					{
						num3++;
					}
				}
				else if (method_6(document_0, (Element)(object)item2, double_0, string_0))
				{
					num++;
				}
			}
			catch
			{
			}
		}
		foreach (FamilyInstance item3 in list2)
		{
			try
			{
				if (dictionary.ContainsKey(((Element)item3).Id))
				{
					if (bool_0)
					{
						if (method_8((Element)(object)dictionary[((Element)item3).Id], double_0))
						{
							num2++;
						}
					}
					else
					{
						num3++;
					}
				}
				else if (method_6(document_0, (Element)(object)item3, double_0, string_0))
				{
					num++;
				}
			}
			catch
			{
			}
		}
		return (successCount: num, modifiedCount: num2, skippedCount: num3);
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
				List<int> list2 = smethod_5(parameter);
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
				if (smethod_6(parameter2, out var int_))
				{
					list.Add(int_);
				}
			}
		}
		if (list.Count == 0 && aitoolContext_0.HasParameter("cacheId"))
		{
			List<int> list3 = smethod_3(aitoolContext_0, aitoolContext_0.GetParameter<string>("cacheId", (string)null));
			if (list3 != null && list3.Count > 0)
			{
				list.AddRange(list3);
			}
		}
		return list.Distinct().ToList();
	}

	private static List<int>? smethod_3(AIToolContext aitoolContext_0, string? string_0)
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
				if (item != null && (smethod_4(item, "element_id", out var int_) || smethod_4(item, "id", out int_)))
				{
					list.Add(int_);
				}
			}
		}
		else if (smethod_4(cachedData, "element_id", out int_2) || smethod_4(cachedData, "id", out int_2))
		{
			list.Add(int_2);
		}
		return list.Distinct().ToList();
	}

	private static bool smethod_4(object object_0, string string_0, out int int_0)
	{
		int_0 = 0;
		try
		{
			if (object_0 is IDictionary dictionary && dictionary.Contains(string_0))
			{
				return smethod_6(dictionary[string_0], out int_0);
			}
			PropertyInfo property = object_0.GetType().GetProperty(string_0);
			if (property != null)
			{
				return smethod_6(property.GetValue(object_0), out int_0);
			}
		}
		catch
		{
		}
		return false;
	}

	private static List<int>? smethod_5(object? object_0)
	{
		if (object_0 == null)
		{
			return null;
		}
		List<int> list = new List<int>();
		if (smethod_6(object_0, out var int_))
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
				if (smethod_6(item, out var int_2))
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
				if (smethod_6(item2, out var int_3))
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
				if (smethod_6(item3, out var int_4))
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
						if (smethod_6(item4, out var int_5))
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

	private static bool smethod_6(object? object_0, out int int_0)
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
			return smethod_6(val.Value, out int_0);
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
		if (object_0 is decimal num5 && num5 == Math.Floor(num5) && num5 <= 2147483647m && num5 >= -2147483648m)
		{
			int_0 = (int)num5;
			return true;
		}
		return false;
	}

	private bool method_13(MEPSystemType mepsystemType_0)
	{
		return mepsystemType_0 is PipingSystemType;
	}

	private long method_14(Document document_0, Pipe pipe_0)
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

	private long method_15(Duct duct_0)
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
}
