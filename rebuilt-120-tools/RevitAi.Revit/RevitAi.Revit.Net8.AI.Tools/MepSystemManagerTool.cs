using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.Exceptions;
using ns0;
using ns6;

using ArgumentException = System.ArgumentException;
using InvalidOperationException = System.InvalidOperationException;
namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("mep_system_manager", Category = "机电管理", Description = "管理 MEP 系统类型，支持复制、重命名、删除等操作", RequiresTransaction = true, RequiresModification = true)]
public sealed class MepSystemManagerTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class574
	{
		public string string_0;
	}

	[CompilerGenerated]
	public sealed class Class575
	{
		public ElementType elementType_0;

		public Class574 class574_0;

		internal bool method_0(ElementType elementType_1)
		{
			return !((object)((Element)elementType_1).Id).Equals((object?)((Element)elementType_0).Id) && ((Element)elementType_1).Name.Equals(class574_0.string_0, StringComparison.OrdinalIgnoreCase);
		}
	}

	[CompilerGenerated]
	public sealed class Class576
	{
		public MepSystemManagerTool mepSystemManagerTool_0;

		public Document document_0;

		public long long_0;

		internal bool method_0(Pipe pipe_0)
		{
			return mepSystemManagerTool_0.method_4(document_0, pipe_0) == long_0;
		}

		internal bool method_1(Duct duct_0)
		{
			return mepSystemManagerTool_0.method_5(duct_0) == long_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class577
	{
		public string string_0;

		internal bool method_0(ElementType elementType_0)
		{
			return ((Element)elementType_0).Name.Equals(string_0, StringComparison.OrdinalIgnoreCase);
		}
	}

	[CompilerGenerated]
	public sealed class Class578 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public AIToolContext aitoolContext_0;

		public MepSystemManagerTool mepSystemManagerTool_0;

		private long long_0;

		private bool bool_0;

		private ElementType elementType_0;

		private string string_0;

		private int int_1;

		private ICollection<ElementId> icollection_0;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Expected O, but got Unknown
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Expected O, but got Unknown
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class578 stateMachine = this;
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
			bool_0 = aitoolContext_0.GetParameter<bool>("force", false);
			AIToolResult result;
			if (long_0 <= 0L)
			{
				result = AIToolResult.Fail("必须指定有效的 system_type_id");
			}
			else
			{
				try
				{
					Element element = document_0.GetElement(new ElementId(long_0));
					elementType_0 = (ElementType)(object)((element is ElementType) ? element : null);
					if (elementType_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到系统类型 ID: ");
						defaultInterpolatedStringHandler.AppendFormatted(long_0);
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						string_0 = ((Element)elementType_0).Name;
						if (!elementType_0.CanBeDeleted)
						{
							result = AIToolResult.Fail("系统类型 '" + string_0 + "' 不允许删除（可能是内置系统类型）");
						}
						else
						{
							int_1 = mepSystemManagerTool_0.method_3(document_0, long_0);
							if (int_1 > 0 && !bool_0)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(41, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("系统类型 '");
								defaultInterpolatedStringHandler2.AppendFormatted(string_0);
								defaultInterpolatedStringHandler2.AppendLiteral("' 正在被 ");
								defaultInterpolatedStringHandler2.AppendFormatted(int_1);
								defaultInterpolatedStringHandler2.AppendLiteral(" 个元素使用，无法删除。请先将这些元素更改为其他系统类型。");
								result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class259<long, string, int, bool, string>(long_0, string_0, int_1, gparam_8: false, "使用 change_system 操作先将元素更改为其他系统类型"));
							}
							else
							{
								icollection_0 = document_0.Delete(new ElementId(long_0));
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(49, 2);
								defaultInterpolatedStringHandler3.AppendLiteral("[MepSystemManagerTool] Delete: 成功删除系统类型 '");
								defaultInterpolatedStringHandler3.AppendFormatted(string_0);
								defaultInterpolatedStringHandler3.AppendLiteral("' (ID: ");
								defaultInterpolatedStringHandler3.AppendFormatted(long_0);
								defaultInterpolatedStringHandler3.AppendLiteral(")");
								Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
								result = AIToolResult.Ok("成功删除系统类型 '" + string_0 + "'", (object)new Class260<long, string, bool, string>(long_0, string_0, gparam_6: true, "系统类型已删除"));
							}
						}
					}
				}
				catch (Exception ex)
				{
					exception_0 = ex;
					Logger.Error("[MepSystemManagerTool] Delete: " + exception_0.Message);
					result = AIToolResult.Fail("删除失败: " + exception_0.Message);
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
	public sealed class Class579 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public AIToolContext aitoolContext_0;

		public MepSystemManagerTool mepSystemManagerTool_0;

		private Class577 class577_0;

		private long long_0;

		private ElementType elementType_0;

		private Type type_0;

		private ElementType elementType_1;

		private bool bool_0;

		private InvalidOperationException invalidOperationException_0;

		private ArgumentException argumentException_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_041d: Expected O, but got Unknown
			//IL_0468: Expected O, but got Unknown
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Expected O, but got Unknown
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Expected O, but got Unknown
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				class577_0 = new Class577();
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class579 stateMachine = this;
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
			long_0 = aitoolContext_0.GetParameter<long>("source_system_type_id", 0L);
			class577_0.string_0 = aitoolContext_0.GetParameter<string>("new_system_name", (string)null) ?? aitoolContext_0.GetParameter<string>("new_name", (string)null);
			AIToolResult result;
			if (long_0 <= 0L)
			{
				result = AIToolResult.Fail("必须指定有效的 source_system_type_id");
			}
			else if (string.IsNullOrEmpty(class577_0.string_0))
			{
				result = AIToolResult.Fail("必须指定新系统名称 (new_system_name)");
			}
			else
			{
				try
				{
					Element element = document_0.GetElement(new ElementId(long_0));
					elementType_0 = (ElementType)(object)((element is ElementType) ? element : null);
					if (elementType_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到源系统类型 ID: ");
						defaultInterpolatedStringHandler.AppendFormatted(long_0);
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else if (!elementType_0.CanBeCopied)
					{
						result = AIToolResult.Fail("系统类型 '" + ((Element)elementType_0).Name + "' 不允许复制");
					}
					else
					{
						type_0 = ((object)document_0.GetElement(new ElementId(long_0)))?.GetType();
						if (!(type_0 != null))
						{
							goto IL_026d;
						}
						bool_0 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(type_0)).Cast<ElementType>().Any((ElementType elementType_0) => ((Element)elementType_0).Name.Equals(class577_0.string_0, StringComparison.OrdinalIgnoreCase));
						if (!bool_0)
						{
							goto IL_026d;
						}
						result = AIToolResult.Fail("系统类型名称 '" + class577_0.string_0 + "' 已存在");
					}
					goto end_IL_0124;
					IL_026d:
					elementType_1 = elementType_0.Duplicate(class577_0.string_0);
					if (elementType_1 == null)
					{
						result = AIToolResult.Fail("复制系统类型失败");
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(54, 3);
						defaultInterpolatedStringHandler2.AppendLiteral("[MepSystemManagerTool] Duplicate: 成功复制 '");
						defaultInterpolatedStringHandler2.AppendFormatted(((Element)elementType_0).Name);
						defaultInterpolatedStringHandler2.AppendLiteral("' → '");
						defaultInterpolatedStringHandler2.AppendFormatted(class577_0.string_0);
						defaultInterpolatedStringHandler2.AppendLiteral("' (新ID: ");
						defaultInterpolatedStringHandler2.AppendFormatted(((Element)elementType_1).Id.Value);
						defaultInterpolatedStringHandler2.AppendLiteral(")");
						Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(16, 2);
						defaultInterpolatedStringHandler3.AppendLiteral("成功复制系统类型 '");
						defaultInterpolatedStringHandler3.AppendFormatted(((Element)elementType_0).Name);
						defaultInterpolatedStringHandler3.AppendLiteral("' → '");
						defaultInterpolatedStringHandler3.AppendFormatted(class577_0.string_0);
						defaultInterpolatedStringHandler3.AppendLiteral("'");
						string text = defaultInterpolatedStringHandler3.ToStringAndClear();
						long gparam_ = long_0;
						string name = ((Element)elementType_0).Name;
						long value = ((Element)elementType_1).Id.Value;
						string gparam_2 = class577_0.string_0;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(12, 1);
						defaultInterpolatedStringHandler4.AppendLiteral("系统类型已创建，ID: ");
						defaultInterpolatedStringHandler4.AppendFormatted(((Element)elementType_1).Id.Value);
						result = AIToolResult.Ok(text, (object)new Class257<long, string, long, string, string>(gparam_, name, value, gparam_2, defaultInterpolatedStringHandler4.ToStringAndClear()));
					}
					end_IL_0124:;
				}
				catch (InvalidOperationException ex)
				{
					InvalidOperationException ex2 = ex;
					invalidOperationException_0 = ex2;
					Logger.Error("[MepSystemManagerTool] Duplicate: 操作无效 - " + ((Exception)(object)invalidOperationException_0).Message);
					result = AIToolResult.Fail("复制失败：" + ((Exception)(object)invalidOperationException_0).Message);
				}
				catch (ArgumentException ex3)
				{
					ArgumentException ex4 = ex3;
					argumentException_0 = ex4;
					Logger.Error("[MepSystemManagerTool] Duplicate: 参数错误 - " + ((Exception)(object)argumentException_0).Message);
					result = AIToolResult.Fail("复制失败：新名称 '" + class577_0.string_0 + "' 可能包含非法字符");
				}
			}
			int_0 = -2;
			class577_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class580 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public MepSystemManagerTool mepSystemManagerTool_0;

		private Document document_0;

		private string string_0;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private AIToolResult aitoolResult_2;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if ((uint)(num - 1) <= 2u)
				{
					goto IL_006e;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class580 stateMachine = this;
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
								Logger.Info("[MepSystemManagerTool] 执行操作: " + string_0);
								string_1 = string_0.ToLower();
								string text = string_1;
								if (!(text == "duplicate"))
								{
									if (!(text == "rename"))
									{
										if (!(text == "delete"))
										{
											val = AIToolResult.Fail("不支持的操作类型: " + string_0);
											break;
										}
										awaiter2 = mepSystemManagerTool_0.method_2(document_0, aitoolContext_0).GetAwaiter();
										if (!awaiter2.IsCompleted)
										{
											num = 3;
											int_0 = 3;
											taskAwaiter_1 = awaiter2;
											Class580 stateMachine = this;
											asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
											return;
										}
										goto IL_0338;
									}
									awaiter3 = mepSystemManagerTool_0.method_1(document_0, aitoolContext_0).GetAwaiter();
									if (!awaiter3.IsCompleted)
									{
										num = 2;
										int_0 = 2;
										taskAwaiter_1 = awaiter3;
										Class580 stateMachine = this;
										asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
										return;
									}
									goto IL_02fd;
								}
								awaiter4 = mepSystemManagerTool_0.method_0(document_0, aitoolContext_0).GetAwaiter();
								if (!awaiter4.IsCompleted)
								{
									num = 1;
									int_0 = 1;
									taskAwaiter_1 = awaiter4;
									Class580 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
									return;
								}
								goto IL_02c2;
							}
							result = AIToolResult.Fail("必须指定 operation 参数");
						}
					}
					goto end_IL_006e;
				case 1:
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_02c2;
				case 2:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_02fd;
				case 3:
					{
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						goto IL_0338;
					}
					IL_02fd:
					aitoolResult_1 = awaiter3.GetResult();
					val = aitoolResult_1;
					aitoolResult_1 = null;
					break;
					IL_0338:
					aitoolResult_2 = awaiter2.GetResult();
					val = aitoolResult_2;
					aitoolResult_2 = null;
					break;
					IL_02c2:
					aitoolResult_0 = awaiter4.GetResult();
					val = aitoolResult_0;
					aitoolResult_0 = null;
					break;
				}
				result = val;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[MepSystemManagerTool] 执行失败: " + exception_0.Message);
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
	public sealed class Class581 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public Document document_0;

		public AIToolContext aitoolContext_0;

		public MepSystemManagerTool mepSystemManagerTool_0;

		private Class574 class574_0;

		private long long_0;

		private Class575 class575_0;

		private string string_0;

		private Type type_0;

		private bool bool_0;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Expected O, but got Unknown
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				class574_0 = new Class574();
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class581 stateMachine = this;
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
			class574_0.string_0 = aitoolContext_0.GetParameter<string>("new_system_name", (string)null) ?? aitoolContext_0.GetParameter<string>("new_name", (string)null);
			AIToolResult result;
			if (long_0 <= 0L)
			{
				result = AIToolResult.Fail("必须指定有效的 system_type_id");
			}
			else if (string.IsNullOrEmpty(class574_0.string_0))
			{
				result = AIToolResult.Fail("必须指定新名称 (new_system_name)");
			}
			else
			{
				try
				{
					class575_0 = new Class575();
					class575_0.class574_0 = class574_0;
					Class575 @class = class575_0;
					Element element = document_0.GetElement(new ElementId(long_0));
					@class.elementType_0 = (ElementType)(object)((element is ElementType) ? element : null);
					if (class575_0.elementType_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到系统类型 ID: ");
						defaultInterpolatedStringHandler.AppendFormatted(long_0);
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						string_0 = ((Element)class575_0.elementType_0).Name;
						if (!class575_0.elementType_0.CanBeRenamed)
						{
							result = AIToolResult.Fail("系统类型 '" + string_0 + "' 不允许重命名");
						}
						else
						{
							type_0 = ((object)class575_0.elementType_0).GetType();
							bool_0 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(type_0)).Cast<ElementType>().Any((ElementType elementType_1) => !((object)((Element)elementType_1).Id).Equals((object?)((Element)class575_0.elementType_0).Id) && ((Element)elementType_1).Name.Equals(class575_0.class574_0.string_0, StringComparison.OrdinalIgnoreCase));
							if (bool_0)
							{
								result = AIToolResult.Fail("系统类型名称 '" + class575_0.class574_0.string_0 + "' 已存在");
							}
							else
							{
								((Element)class575_0.elementType_0).Name = class575_0.class574_0.string_0;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(44, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("[MepSystemManagerTool] Rename: 成功重命名 '");
								defaultInterpolatedStringHandler2.AppendFormatted(string_0);
								defaultInterpolatedStringHandler2.AppendLiteral("' → '");
								defaultInterpolatedStringHandler2.AppendFormatted(class575_0.class574_0.string_0);
								defaultInterpolatedStringHandler2.AppendLiteral("'");
								Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(18, 2);
								defaultInterpolatedStringHandler3.AppendLiteral("成功将系统类型 '");
								defaultInterpolatedStringHandler3.AppendFormatted(string_0);
								defaultInterpolatedStringHandler3.AppendLiteral("' 重命名为 '");
								defaultInterpolatedStringHandler3.AppendFormatted(class575_0.class574_0.string_0);
								defaultInterpolatedStringHandler3.AppendLiteral("'");
								result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class258<long, string, string, string>(long_0, string_0, class575_0.class574_0.string_0, "系统类型已重命名"));
							}
						}
					}
				}
				catch (Exception ex)
				{
					exception_0 = ex;
					Logger.Error("[MepSystemManagerTool] Rename: " + exception_0.Message);
					result = AIToolResult.Fail("重命名失败: " + exception_0.Message);
				}
			}
			int_0 = -2;
			class574_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "mep_system_manager";

	public string Category => "机电管理";

	public string Description => "管理 MEP 系统类型，支持复制、重命名、删除等操作";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operation\": {\n                \"type\": \"string\",\n                \"enum\": [\n                    \"duplicate\",\n                    \"rename\",\n                    \"delete\"\n                ],\n                \"description\": \"管理操作类型：duplicate(复制)、rename(重命名)、delete(删除)\"\n            },\n            \"source_system_type_id\": {\n                \"type\": \"integer\",\n                \"description\": \"源系统类型 ID（复制操作时使用）\"\n            },\n            \"system_type_id\": {\n                \"type\": \"integer\",\n                \"description\": \"系统类型 ID（重命名、删除操作时使用）\"\n            },\n            \"new_system_name\": {\n                \"type\": \"string\",\n                \"description\": \"新系统名称（复制、重命名操作时使用）\"\n            },\n            \"new_name\": {\n                \"type\": \"string\",\n                \"description\": \"新名称（重命名操作时使用，与 new_system_name 二选一）\"\n            },\n            \"force\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否强制删除（删除操作时使用，默认 false）\"\n            }\n        },\n        \"required\": [\"operation\"]\n    }";

	[AsyncStateMachine(typeof(Class580))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class580 stateMachine = new Class580();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mepSystemManagerTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class579))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_0(Document document_0, AIToolContext aitoolContext_0)
	{
		Class579 stateMachine = new Class579();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mepSystemManagerTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class581))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(Document document_0, AIToolContext aitoolContext_0)
	{
		Class581 stateMachine = new Class581();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mepSystemManagerTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class578))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_2(Document document_0, AIToolContext aitoolContext_0)
	{
		Class578 stateMachine = new Class578();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mepSystemManagerTool_0 = this;
		stateMachine.document_0 = document_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private int method_3(Document document_0, long long_0)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			int num = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Pipe))).Cast<Pipe>().Count((Pipe pipe_0) => method_4(document_0, pipe_0) == long_0);
			int num2 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Duct))).Cast<Duct>().Count((Duct duct_0) => method_5(duct_0) == long_0);
			return num + num2;
		}
		catch
		{
			return 0;
		}
	}

	private long method_4(Document document_0, Pipe pipe_0)
	{
		try
		{
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

	private long method_5(Duct duct_0)
	{
		try
		{
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
