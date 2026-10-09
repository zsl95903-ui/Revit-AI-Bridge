using System;
using System.Collections;
using System.Diagnostics;
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

[AITool("material_manager", Category = "材料管理", Description = "管理材质的创建、删除和属性修改。支持创建材质、删除材质、设置颜色、设置透明度。", RequiresTransaction = true, RequiresModification = true)]
public sealed class MaterialManagerTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class563 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public object object_1;

		public MaterialManagerTool materialManagerTool_0;

		private string string_0;

		private MethodInfo methodInfo_0;

		private object object_2;

		private MethodInfo methodInfo_1;

		private object object_3;

		private object object_4;

		private object object_5;

		private byte byte_0;

		private byte byte_1;

		private byte byte_2;

		private object object_6;

		private PropertyInfo propertyInfo_0;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo propertyInfo_2;

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
					Class563 stateMachine = this;
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
			string_0 = aitoolContext_0.GetParameter<string>("materialName", (string)null);
			AIToolResult result;
			if (string.IsNullOrWhiteSpace(string_0))
			{
				result = AIToolResult.Fail("create 操作需要指定 materialName 参数");
			}
			else
			{
				Logger.Info("[MaterialManagerTool] Create: 创建材质 '" + string_0 + "'");
				methodInfo_0 = object_0.GetType().GetMethod("GetMaterialByName");
				object_2 = methodInfo_0?.Invoke(object_0, new object[2] { aitoolContext_0.Document, string_0 });
				if (object_2 != null)
				{
					object_6 = object_1.GetType().GetMethod("GetElementId")?.Invoke(object_1, new object[1] { object_2 });
					Logger.Info("[MaterialManagerTool] Create: 材质 '" + string_0 + "' 已存在");
					result = AIToolResult.Ok("材质 '" + string_0 + "' 已存在", (object)new Class244<string, int, string, bool>("create", Convert.ToInt32(object_6 ?? ((object)0)), string_0, gparam_7: true));
				}
				else
				{
					methodInfo_1 = object_0.GetType().GetMethod("CreateMaterial");
					object_3 = methodInfo_1?.Invoke(object_0, new object[2] { aitoolContext_0.Document, string_0 });
					if (object_3 == null)
					{
						Logger.Error("[MaterialManagerTool] Create: 创建材质 '" + string_0 + "' 失败");
						result = AIToolResult.Fail("创建材质 '" + string_0 + "' 失败");
					}
					else
					{
						object_4 = object_1.GetType().GetMethod("GetElementId")?.Invoke(object_1, new object[1] { object_3 });
						object_5 = object_0.GetType().GetMethod("GetMaterialColor")?.Invoke(object_0, new object[1] { object_3 });
						byte_0 = 0;
						byte_1 = 0;
						byte_2 = 0;
						if (object_5 != null)
						{
							propertyInfo_0 = object_5.GetType().GetProperty("R");
							propertyInfo_1 = object_5.GetType().GetProperty("G");
							propertyInfo_2 = object_5.GetType().GetProperty("B");
							if (propertyInfo_0 != null)
							{
								byte_0 = ((byte?)propertyInfo_0.GetValue(object_5)).GetValueOrDefault();
							}
							if (propertyInfo_1 != null)
							{
								byte_1 = ((byte?)propertyInfo_1.GetValue(object_5)).GetValueOrDefault();
							}
							if (propertyInfo_2 != null)
							{
								byte_2 = ((byte?)propertyInfo_2.GetValue(object_5)).GetValueOrDefault();
							}
							propertyInfo_0 = null;
							propertyInfo_1 = null;
							propertyInfo_2 = null;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[MaterialManagerTool] Create: 成功创建材质 '");
						defaultInterpolatedStringHandler.AppendFormatted(string_0);
						defaultInterpolatedStringHandler.AppendLiteral("'，ID: ");
						defaultInterpolatedStringHandler.AppendFormatted<object>(object_4);
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						result = AIToolResult.Ok("成功创建材质: " + string_0, (object)new Class245<string, int, string, Class246<byte, byte, byte>, bool>("create", Convert.ToInt32(object_4 ?? ((object)0)), string_0, new Class246<byte, byte, byte>(byte_0, byte_1, byte_2), gparam_9: true));
					}
				}
			}
			int_0 = -2;
			string_0 = null;
			methodInfo_0 = null;
			object_2 = null;
			methodInfo_1 = null;
			object_3 = null;
			object_4 = null;
			object_5 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class564 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public object object_1;

		public MaterialManagerTool materialManagerTool_0;

		private int int_1;

		private bool bool_0;

		private object object_2;

		private string string_0;

		private string string_1;

		private MethodInfo methodInfo_0;

		private bool? nullable_0;

		private MethodInfo methodInfo_1;

		private bool? nullable_1;

		private MethodInfo methodInfo_2;

		private IEnumerable ienumerable_0;

		private int int_2;

		private IEnumerator ienumerator_0;

		private object object_3;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
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
					Class564 stateMachine = this;
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
			int_1 = aitoolContext_0.GetParameter<int>("materialId", 0);
			bool_0 = aitoolContext_0.GetParameter<bool>("force", false);
			AIToolResult result;
			if (int_1 <= 0)
			{
				result = AIToolResult.Fail("delete 操作需要指定有效的 materialId 参数");
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[MaterialManagerTool] Delete: 删除材质 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(int_1);
				defaultInterpolatedStringHandler.AppendLiteral("，force: ");
				defaultInterpolatedStringHandler.AppendFormatted(bool_0);
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				object_2 = object_1.GetType().GetMethod("GetElementById")?.Invoke(object_1, new object[2] { aitoolContext_0.Document, int_1 });
				if (object_2 == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(43, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("[MaterialManagerTool] Delete: 找不到 ID 为 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_1);
					defaultInterpolatedStringHandler2.AppendLiteral(" 的材质");
					Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(13, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("找不到 ID 为 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_1);
					defaultInterpolatedStringHandler3.AppendLiteral(" 的材质");
					result = AIToolResult.Fail(defaultInterpolatedStringHandler3.ToStringAndClear());
				}
				else
				{
					string_0 = object_1.GetType().GetMethod("GetElementName")?.Invoke(object_1, new object[1] { object_2 })?.ToString();
					string text = string_0;
					if (text == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(6, 1);
						defaultInterpolatedStringHandler4.AppendLiteral("材质 ID ");
						defaultInterpolatedStringHandler4.AppendFormatted(int_1);
						text = defaultInterpolatedStringHandler4.ToStringAndClear();
					}
					string_1 = text;
					if (!bool_0)
					{
						methodInfo_1 = object_0.GetType().GetMethod("IsMaterialInUse");
						nullable_1 = methodInfo_1?.Invoke(object_0, new object[2] { aitoolContext_0.Document, int_1 }) as bool?;
						if (nullable_1 == true)
						{
							methodInfo_2 = object_0.GetType().GetMethod("GetElementsUsingMaterial");
							ienumerable_0 = methodInfo_2?.Invoke(object_0, new object[2] { aitoolContext_0.Document, int_1 }) as IEnumerable;
							int_2 = 0;
							if (ienumerable_0 != null)
							{
								ienumerator_0 = ienumerable_0.GetEnumerator();
								try
								{
									while (ienumerator_0.MoveNext())
									{
										object_3 = ienumerator_0.Current;
										int_2++;
										object_3 = null;
									}
								}
								finally
								{
									if (num < 0 && ienumerator_0 is IDisposable disposable)
									{
										disposable.Dispose();
									}
								}
								ienumerator_0 = null;
							}
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(46, 2);
							defaultInterpolatedStringHandler5.AppendLiteral("[MaterialManagerTool] Delete: 材质 '");
							defaultInterpolatedStringHandler5.AppendFormatted(string_1);
							defaultInterpolatedStringHandler5.AppendLiteral("' 正在被 ");
							defaultInterpolatedStringHandler5.AppendFormatted(int_2);
							defaultInterpolatedStringHandler5.AppendLiteral(" 个元素使用");
							Logger.Warning(defaultInterpolatedStringHandler5.ToStringAndClear());
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(50, 3);
							defaultInterpolatedStringHandler6.AppendLiteral("材质 '");
							defaultInterpolatedStringHandler6.AppendFormatted(string_1);
							defaultInterpolatedStringHandler6.AppendLiteral("' (ID: ");
							defaultInterpolatedStringHandler6.AppendFormatted(int_1);
							defaultInterpolatedStringHandler6.AppendLiteral(") 正在被 ");
							defaultInterpolatedStringHandler6.AppendFormatted(int_2);
							defaultInterpolatedStringHandler6.AppendLiteral(" 个元素使用，无法删除。如需强制删除，请设置 force=true");
							result = AIToolResult.Fail(defaultInterpolatedStringHandler6.ToStringAndClear());
							goto IL_069f;
						}
						methodInfo_1 = null;
					}
					methodInfo_0 = object_0.GetType().GetMethod("DeleteMaterial");
					nullable_0 = methodInfo_0?.Invoke(object_0, new object[3] { aitoolContext_0.Document, string_1, false }) as bool?;
					if (nullable_0 != true)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(47, 2);
						defaultInterpolatedStringHandler7.AppendLiteral("[MaterialManagerTool] Delete: 删除材质 '");
						defaultInterpolatedStringHandler7.AppendFormatted(string_1);
						defaultInterpolatedStringHandler7.AppendLiteral("' (ID: ");
						defaultInterpolatedStringHandler7.AppendFormatted(int_1);
						defaultInterpolatedStringHandler7.AppendLiteral(") 失败");
						Logger.Error(defaultInterpolatedStringHandler7.ToStringAndClear());
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(17, 2);
						defaultInterpolatedStringHandler8.AppendLiteral("删除材质 '");
						defaultInterpolatedStringHandler8.AppendFormatted(string_1);
						defaultInterpolatedStringHandler8.AppendLiteral("' (ID: ");
						defaultInterpolatedStringHandler8.AppendFormatted(int_1);
						defaultInterpolatedStringHandler8.AppendLiteral(") 失败");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler8.ToStringAndClear());
					}
					else
					{
						Logger.Info("[MaterialManagerTool] Delete: 成功删除材质 '" + string_1 + "'");
						result = AIToolResult.Ok("成功删除材质: " + string_1, (object)new Class247<string, int, string, bool, bool>("delete", int_1, string_1, gparam_8: true, bool_0));
					}
				}
			}
			goto IL_069f;
			IL_069f:
			int_0 = -2;
			object_2 = null;
			string_0 = null;
			string_1 = null;
			methodInfo_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class565 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public MaterialManagerTool materialManagerTool_0;

		private IMaterialService imaterialService_0;

		private IElementService ielementService_0;

		private string string_0;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private AIToolResult aitoolResult_2;

		private AIToolResult aitoolResult_3;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if ((uint)(num - 1) <= 3u)
				{
					goto IL_006e;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class565 stateMachine = this;
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
				switch (num)
				{
				default:
				{
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					imaterialService_0 = ((revitAdapter != null) ? revitAdapter.MaterialService : null);
					IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
					ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
					if (imaterialService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 MaterialService");
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
						string_0 = aitoolContext_0.GetParameter<string>("operation", (string)null);
						if (!string.IsNullOrEmpty(string_0))
						{
							Logger.Info("[MaterialManagerTool] 执行操作: " + string_0);
							string_1 = string_0.ToLower();
							string text = string_1;
							if (!(text == "create"))
							{
								if (!(text == "delete"))
								{
									if (!(text == "set_color"))
									{
										if (!(text == "set_transparency"))
										{
											val = AIToolResult.Fail("不支持的操作类型: " + string_0);
											break;
										}
										awaiter2 = materialManagerTool_0.method_3(aitoolContext_0, imaterialService_0, ielementService_0).GetAwaiter();
										if (!awaiter2.IsCompleted)
										{
											num = 4;
											int_0 = 4;
											taskAwaiter_1 = awaiter2;
											Class565 stateMachine = this;
											asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
											return;
										}
										goto IL_0437;
									}
									awaiter3 = materialManagerTool_0.method_2(aitoolContext_0, imaterialService_0, ielementService_0).GetAwaiter();
									if (!awaiter3.IsCompleted)
									{
										num = 3;
										int_0 = 3;
										taskAwaiter_1 = awaiter3;
										Class565 stateMachine = this;
										asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
										return;
									}
									goto IL_03fc;
								}
								awaiter4 = materialManagerTool_0.method_1(aitoolContext_0, imaterialService_0, ielementService_0).GetAwaiter();
								if (!awaiter4.IsCompleted)
								{
									num = 2;
									int_0 = 2;
									taskAwaiter_1 = awaiter4;
									Class565 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
									return;
								}
								goto IL_03c1;
							}
							awaiter5 = materialManagerTool_0.method_0(aitoolContext_0, imaterialService_0, ielementService_0).GetAwaiter();
							if (!awaiter5.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter5;
								Class565 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
								return;
							}
							goto IL_0383;
						}
						result = AIToolResult.Fail("必须指定 operation 参数");
					}
					goto end_IL_006e;
				}
				case 1:
					awaiter5 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0383;
				case 2:
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_03c1;
				case 3:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_03fc;
				case 4:
					{
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						goto IL_0437;
					}
					IL_0437:
					aitoolResult_3 = awaiter2.GetResult();
					val = aitoolResult_3;
					aitoolResult_3 = null;
					break;
					IL_0383:
					aitoolResult_0 = awaiter5.GetResult();
					val = aitoolResult_0;
					aitoolResult_0 = null;
					break;
					IL_03fc:
					aitoolResult_2 = awaiter3.GetResult();
					val = aitoolResult_2;
					aitoolResult_2 = null;
					break;
					IL_03c1:
					aitoolResult_1 = awaiter4.GetResult();
					val = aitoolResult_1;
					aitoolResult_1 = null;
					break;
				}
				result = val;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[MaterialManagerTool] 执行失败: " + exception_0.Message);
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
	public sealed class Class566 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public object object_1;

		public MaterialManagerTool materialManagerTool_0;

		private string string_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private MethodInfo methodInfo_0;

		private object object_2;

		private MethodInfo methodInfo_1;

		private bool? nullable_0;

		private object object_3;

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
					Class566 stateMachine = this;
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
			string_0 = aitoolContext_0.GetParameter<string>("materialName", (string)null);
			int_1 = aitoolContext_0.GetParameter<int>("red", 0);
			int_2 = aitoolContext_0.GetParameter<int>("green", 0);
			int_3 = aitoolContext_0.GetParameter<int>("blue", 0);
			AIToolResult result;
			if (int_1 < 0 || int_1 > 255 || int_2 < 0 || int_2 > 255 || int_3 < 0 || int_3 > 255)
			{
				result = AIToolResult.Fail("RGB值必须在 0-255 范围内");
			}
			else if (string.IsNullOrWhiteSpace(string_0))
			{
				result = AIToolResult.Fail("set_color 操作需要指定 materialName 参数");
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 4);
				defaultInterpolatedStringHandler.AppendLiteral("[MaterialManagerTool] SetColor: 设置材质 '");
				defaultInterpolatedStringHandler.AppendFormatted(string_0);
				defaultInterpolatedStringHandler.AppendLiteral("' 颜色为 RGB(");
				defaultInterpolatedStringHandler.AppendFormatted(int_1);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted(int_2);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted(int_3);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				methodInfo_0 = object_0.GetType().GetMethod("GetMaterialByName");
				object_2 = methodInfo_0?.Invoke(object_0, new object[2] { aitoolContext_0.Document, string_0 });
				if (object_2 == null)
				{
					Logger.Warning("[MaterialManagerTool] SetColor: 找不到名称为 '" + string_0 + "' 的材质");
					result = AIToolResult.Fail("找不到名称为 '" + string_0 + "' 的材质");
				}
				else
				{
					methodInfo_1 = object_0.GetType().GetMethod("SetMaterialColor");
					nullable_0 = methodInfo_1?.Invoke(object_0, new object[4]
					{
						object_2,
						(byte)int_1,
						(byte)int_2,
						(byte)int_3
					}) as bool?;
					if (nullable_0 != true)
					{
						Logger.Error("[MaterialManagerTool] SetColor: 设置材质 '" + string_0 + "' 的颜色失败");
						result = AIToolResult.Fail("设置材质 '" + string_0 + "' 的颜色失败");
					}
					else
					{
						object_3 = object_1.GetType().GetMethod("GetElementId")?.Invoke(object_1, new object[1] { object_2 });
						Logger.Info("[MaterialManagerTool] SetColor: 成功设置材质 '" + string_0 + "' 颜色");
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(25, 4);
						defaultInterpolatedStringHandler2.AppendLiteral("成功将材质 '");
						defaultInterpolatedStringHandler2.AppendFormatted(string_0);
						defaultInterpolatedStringHandler2.AppendLiteral("' 的颜色设置为 RGB(");
						defaultInterpolatedStringHandler2.AppendFormatted(int_1);
						defaultInterpolatedStringHandler2.AppendLiteral(", ");
						defaultInterpolatedStringHandler2.AppendFormatted(int_2);
						defaultInterpolatedStringHandler2.AppendLiteral(", ");
						defaultInterpolatedStringHandler2.AppendFormatted(int_3);
						defaultInterpolatedStringHandler2.AppendLiteral(")");
						result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class248<string, int, string, Class249<int, int, int>>("set_color", Convert.ToInt32(object_3 ?? ((object)0)), string_0, new Class249<int, int, int>(int_1, int_2, int_3)));
					}
				}
			}
			int_0 = -2;
			string_0 = null;
			methodInfo_0 = null;
			object_2 = null;
			methodInfo_1 = null;
			object_3 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class567 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public object object_1;

		public MaterialManagerTool materialManagerTool_0;

		private string string_0;

		private int int_1;

		private MethodInfo methodInfo_0;

		private object object_2;

		private MethodInfo methodInfo_1;

		private bool? nullable_0;

		private object object_3;

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
					Class567 stateMachine = this;
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
			string_0 = aitoolContext_0.GetParameter<string>("materialName", (string)null);
			int_1 = aitoolContext_0.GetParameter<int>("transparency", 0);
			AIToolResult result;
			if (int_1 < 0 || int_1 > 100)
			{
				result = AIToolResult.Fail("透明度值必须在 0-100 范围内");
			}
			else if (string.IsNullOrWhiteSpace(string_0))
			{
				result = AIToolResult.Fail("set_transparency 操作需要指定 materialName 参数");
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[MaterialManagerTool] SetTransparency: 设置材质 '");
				defaultInterpolatedStringHandler.AppendFormatted(string_0);
				defaultInterpolatedStringHandler.AppendLiteral("' 透明度为 ");
				defaultInterpolatedStringHandler.AppendFormatted(int_1);
				defaultInterpolatedStringHandler.AppendLiteral("%");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				methodInfo_0 = object_0.GetType().GetMethod("GetMaterialByName");
				object_2 = methodInfo_0?.Invoke(object_0, new object[2] { aitoolContext_0.Document, string_0 });
				if (object_2 == null)
				{
					Logger.Warning("[MaterialManagerTool] SetTransparency: 找不到名称为 '" + string_0 + "' 的材质");
					result = AIToolResult.Fail("找不到名称为 '" + string_0 + "' 的材质");
				}
				else
				{
					methodInfo_1 = object_0.GetType().GetMethod("SetMaterialTransparency");
					nullable_0 = methodInfo_1?.Invoke(object_0, new object[2] { object_2, int_1 }) as bool?;
					if (nullable_0 != true)
					{
						Logger.Error("[MaterialManagerTool] SetTransparency: 设置材质 '" + string_0 + "' 的透明度失败");
						result = AIToolResult.Fail("设置材质 '" + string_0 + "' 的透明度失败");
					}
					else
					{
						object_3 = object_1.GetType().GetMethod("GetElementId")?.Invoke(object_1, new object[1] { object_2 });
						Logger.Info("[MaterialManagerTool] SetTransparency: 成功设置材质 '" + string_0 + "' 透明度");
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(18, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("成功将材质 '");
						defaultInterpolatedStringHandler2.AppendFormatted(string_0);
						defaultInterpolatedStringHandler2.AppendLiteral("' 的透明度设置为 ");
						defaultInterpolatedStringHandler2.AppendFormatted(int_1);
						defaultInterpolatedStringHandler2.AppendLiteral("%");
						result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class250<string, int, string, int>("set_transparency", Convert.ToInt32(object_3 ?? ((object)0)), string_0, int_1));
					}
				}
			}
			int_0 = -2;
			string_0 = null;
			methodInfo_0 = null;
			object_2 = null;
			methodInfo_1 = null;
			object_3 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "material_manager";

	public string Category => "材料管理";

	public string Description => "管理材质的创建、删除和属性修改";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operation\": {\n                \"type\": \"string\",\n                \"enum\": [\"create\", \"delete\", \"set_color\", \"set_transparency\"],\n                \"description\": \"管理操作类型：create(创建新材质)、delete(删除材质)、set_color(设置材质颜色)、set_transparency(设置材质透明度)\"\n            },\n            \"materialName\": {\n                \"type\": \"string\",\n                \"description\": \"材质名称（create、set_color、set_transparency 操作使用）。create 时为要创建的材质名称；set_color/set_transparency 时为要修改的材质名称。支持中文。\"\n            },\n            \"materialId\": {\n                \"type\": \"integer\",\n                \"description\": \"材质 ID（delete 操作使用，必需）。从 get_materials 或其他工具返回的材质数据中获取。\"\n            },\n            \"force\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否强制删除（delete 操作使用，可选）。默认 false。即使材质正在使用也会删除。\"\n            },\n            \"red\": {\n                \"type\": \"integer\",\n                \"description\": \"红色分量（set_color 操作使用，必需）。范围 0-255。\"\n            },\n            \"green\": {\n                \"type\": \"integer\",\n                \"description\": \"绿色分量（set_color 操作使用，必需）。范围 0-255。\"\n            },\n            \"blue\": {\n                \"type\": \"integer\",\n                \"description\": \"蓝色分量（set_color 操作使用，必需）。范围 0-255。\"\n            },\n            \"transparency\": {\n                \"type\": \"integer\",\n                \"description\": \"透明度值（set_transparency 操作使用，必需）。范围 0-100，0=完全不透明，100=完全透明。\"\n            }\n        },\n        \"required\": [\"operation\"]\n    }";

	[AsyncStateMachine(typeof(Class565))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class565 stateMachine = new Class565();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.materialManagerTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class563))]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, object object_0, object object_1)
	{
		Class563 stateMachine = new Class563();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.materialManagerTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.object_1 = object_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class564))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, object object_0, object object_1)
	{
		Class564 stateMachine = new Class564();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.materialManagerTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.object_1 = object_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class566))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_2(AIToolContext aitoolContext_0, object object_0, object object_1)
	{
		Class566 stateMachine = new Class566();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.materialManagerTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.object_1 = object_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class567))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_3(AIToolContext aitoolContext_0, object object_0, object object_1)
	{
		Class567 stateMachine = new Class567();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.materialManagerTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.object_1 = object_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
