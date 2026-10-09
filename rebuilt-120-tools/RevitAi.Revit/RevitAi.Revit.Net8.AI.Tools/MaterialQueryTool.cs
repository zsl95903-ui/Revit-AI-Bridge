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
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("material_query", Category = "材料管理", Description = "查询材质信息。支持获取所有材质列表、按名称获取材质详情、获取元素的材质。", RequiresTransaction = false, RequiresModification = false)]
public sealed class MaterialQueryTool : IAITool
{
	[CompilerGenerated]
	private static class Class568
	{
		public static Converter<object, int> converter_0;
	}

	[CompilerGenerated]
	public sealed class Class569
	{
		public List<object> list_0;

		internal string method_0(string string_0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 2);
			defaultInterpolatedStringHandler.AppendLiteral("✅ 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个材料\n\n💡 在后续工具调用中使用 cacheId=\"");
			defaultInterpolatedStringHandler.AppendFormatted(string_0);
			defaultInterpolatedStringHandler.AppendLiteral("\" 参数来操作这些材料");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
	}

	[CompilerGenerated]
	public sealed class Class570 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public MaterialQueryTool materialQueryTool_0;

		private IMaterialService imaterialService_0;

		private IElementService ielementService_0;

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
					Class570 stateMachine = this;
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
							string_1 = string_0.ToLower();
							string text = string_1;
							if (!(text == "get_all"))
							{
								if (!(text == "get_by_name"))
								{
									if (!(text == "get_element_materials"))
									{
										val = AIToolResult.Fail("不支持的操作类型: " + string_0);
										break;
									}
									awaiter2 = materialQueryTool_0.method_2(aitoolContext_0, imaterialService_0, ielementService_0).GetAwaiter();
									if (!awaiter2.IsCompleted)
									{
										num = 3;
										int_0 = 3;
										taskAwaiter_1 = awaiter2;
										Class570 stateMachine = this;
										asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
										return;
									}
									goto IL_036e;
								}
								awaiter3 = materialQueryTool_0.method_1(aitoolContext_0, imaterialService_0, ielementService_0).GetAwaiter();
								if (!awaiter3.IsCompleted)
								{
									num = 2;
									int_0 = 2;
									taskAwaiter_1 = awaiter3;
									Class570 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
									return;
								}
								goto IL_0333;
							}
							awaiter4 = materialQueryTool_0.method_0(aitoolContext_0, imaterialService_0, ielementService_0).GetAwaiter();
							if (!awaiter4.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter4;
								Class570 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
								return;
							}
							goto IL_02f8;
						}
						result = AIToolResult.Fail("必须指定 operation 参数");
					}
					goto end_IL_006e;
				}
				case 1:
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_02f8;
				case 2:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0333;
				case 3:
					{
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						goto IL_036e;
					}
					IL_0333:
					aitoolResult_1 = awaiter3.GetResult();
					val = aitoolResult_1;
					aitoolResult_1 = null;
					break;
					IL_036e:
					aitoolResult_2 = awaiter2.GetResult();
					val = aitoolResult_2;
					aitoolResult_2 = null;
					break;
					IL_02f8:
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
				result = AIToolResult.Fail("查询材质失败: " + exception_0.Message);
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
	public sealed class Class571 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public object object_1;

		public MaterialQueryTool materialQueryTool_0;

		private Class569 class569_0;

		private string string_0;

		private IEnumerable ienumerable_0;

		private IEnumerator ienumerator_0;

		private object object_2;

		private object object_3;

		private string string_1;

		private string string_2;

		private object object_4;

		private string string_3;

		private byte byte_0;

		private byte byte_1;

		private byte byte_2;

		private PropertyInfo propertyInfo_0;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo propertyInfo_2;

		private string string_4;

		private string string_5;

		private AIToolResult aitoolResult_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				class569_0 = new Class569();
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class571 stateMachine = this;
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
			string_0 = aitoolContext_0.GetParameter<string>("filter", (string)null);
			ienumerable_0 = object_0.GetType().GetMethod("GetMaterials")?.Invoke(object_0, new object[1] { aitoolContext_0.Document }) as IEnumerable;
			class569_0.list_0 = new List<object>();
			if (ienumerable_0 != null)
			{
				ienumerator_0 = ienumerable_0.GetEnumerator();
				try
				{
					while (ienumerator_0.MoveNext())
					{
						object_2 = ienumerator_0.Current;
						try
						{
							object_3 = object_1.GetType().GetMethod("GetElementId")?.Invoke(object_1, new object[1] { object_2 });
							string_1 = object_1.GetType().GetMethod("GetElementName")?.Invoke(object_1, new object[1] { object_2 })?.ToString();
							string_2 = object_0.GetType().GetMethod("GetMaterialClass")?.Invoke(object_0, new object[1] { object_2 })?.ToString();
							object_4 = object_0.GetType().GetMethod("GetMaterialColor")?.Invoke(object_0, new object[1] { object_2 });
							string_3 = object_0.GetType().GetMethod("GetAppearanceName")?.Invoke(object_0, new object[1] { object_2 })?.ToString();
							if (!string.IsNullOrEmpty(string_0) && !string.IsNullOrEmpty(string_1) && !string_1.Contains(string_0, StringComparison.OrdinalIgnoreCase))
							{
								continue;
							}
							byte_0 = 0;
							byte_1 = 0;
							byte_2 = 0;
							if (object_4 != null)
							{
								propertyInfo_0 = object_4.GetType().GetProperty("R");
								propertyInfo_1 = object_4.GetType().GetProperty("G");
								propertyInfo_2 = object_4.GetType().GetProperty("B");
								if (propertyInfo_0 != null)
								{
									byte_0 = ((byte?)propertyInfo_0.GetValue(object_4)).GetValueOrDefault();
								}
								if (propertyInfo_1 != null)
								{
									byte_1 = ((byte?)propertyInfo_1.GetValue(object_4)).GetValueOrDefault();
								}
								if (propertyInfo_2 != null)
								{
									byte_2 = ((byte?)propertyInfo_2.GetValue(object_4)).GetValueOrDefault();
								}
								propertyInfo_0 = null;
								propertyInfo_1 = null;
								propertyInfo_2 = null;
							}
							class569_0.list_0.Add(new Class251<int, string, string, Class246<byte, byte, byte>, string>(Convert.ToInt32(object_3 ?? ((object)0)), string_1 ?? "未命名", string_2 ?? "未知", new Class246<byte, byte, byte>(byte_0, byte_1, byte_2), string_3 ?? "未设置"));
							object_3 = null;
							string_1 = null;
							string_2 = null;
							object_4 = null;
							string_3 = null;
							goto IL_0497;
						}
						catch
						{
							goto IL_0497;
						}
						IL_0497:
						object_2 = null;
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
			AIToolResult result;
			if (!string.IsNullOrEmpty(aitoolContext_0.SessionId) && aitoolContext_0.DataCache != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
				defaultInterpolatedStringHandler.AppendLiteral("all_materials_");
				defaultInterpolatedStringHandler.AppendFormatted(DateTime.Now.Ticks);
				string_4 = defaultInterpolatedStringHandler.ToStringAndClear();
				string_5 = aitoolContext_0.SessionId ?? string.Empty;
				aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)class569_0.list_0, aitoolContext_0.DataCache, string_5, string_4, "个材料", 200);
				materialQueryTool_0.method_3(aitoolResult_0, delegate(string string_0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(45, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("✅ 找到 ");
					defaultInterpolatedStringHandler3.AppendFormatted(class569_0.list_0.Count);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个材料\n\n💡 在后续工具调用中使用 cacheId=\"");
					defaultInterpolatedStringHandler3.AppendFormatted(string_0);
					defaultInterpolatedStringHandler3.AppendLiteral("\" 参数来操作这些材料");
					return defaultInterpolatedStringHandler3.ToStringAndClear();
				});
				result = aitoolResult_0;
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("找到 ");
				defaultInterpolatedStringHandler2.AppendFormatted(class569_0.list_0.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个材料");
				result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class252<string, string, int, List<object>>("get_all", string_0 ?? null, class569_0.list_0.Count, class569_0.list_0));
			}
			int_0 = -2;
			class569_0 = null;
			string_0 = null;
			ienumerable_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class572 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public object object_1;

		public MaterialQueryTool materialQueryTool_0;

		private List<int> list_0;

		private List<object> list_1;

		private List<int> list_2;

		private int int_1;

		private string string_0;

		private string string_1;

		private string string_2;

		private object object_2;

		private IEnumerable ienumerable_0;

		private IEnumerator ienumerator_0;

		private object object_3;

		private PropertyInfo propertyInfo_0;

		private int int_2;

		private int int_3;

		private object object_4;

		private int int_4;

		private long long_0;

		private int[] int_5;

		private long[] long_1;

		private List<int> list_3;

		private List<long> list_4;

		private List<object> list_5;

		private List<int>.Enumerator enumerator_0;

		private int int_6;

		private object object_5;

		private MethodInfo methodInfo_0;

		private IEnumerable ienumerable_1;

		private List<object> list_6;

		private string string_3;

		private string string_4;

		private IEnumerator ienumerator_1;

		private object object_6;

		private object object_7;

		private string string_5;

		private string string_6;

		private object object_8;

		private byte byte_0;

		private byte byte_1;

		private byte byte_2;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo propertyInfo_2;

		private PropertyInfo propertyInfo_3;

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
					Class572 stateMachine = this;
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
			list_0 = new List<int>();
			AIToolResult result;
			if (aitoolContext_0.HasParameter("cacheId"))
			{
				string_2 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
				if (!string.IsNullOrEmpty(string_2))
				{
					object_2 = aitoolContext_0.GetCachedData<object>(string_2);
					if (object_2 == null)
					{
						result = AIToolResult.Fail("缓存 ID '" + string_2 + "' 无效或已过期，请重新查询元素");
						goto IL_0ce6;
					}
					ienumerable_0 = object_2 as IEnumerable;
					if (ienumerable_0 != null)
					{
						ienumerator_0 = ienumerable_0.GetEnumerator();
						try
						{
							while (ienumerator_0.MoveNext())
							{
								object_3 = ienumerator_0.Current;
								if (object_3 != null)
								{
									propertyInfo_0 = object_3.GetType().GetProperty("id");
									if (propertyInfo_0 != null)
									{
										object value = propertyInfo_0.GetValue(object_3);
										if (value is int)
										{
											int_2 = (int)value;
											if (true)
											{
												list_0.Add(int_2);
											}
										}
									}
									propertyInfo_0 = null;
								}
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
					list_0 = list_0.Distinct().ToList();
					ienumerable_0 = null;
					object_2 = null;
				}
				string_2 = null;
			}
			if (list_0.Count == 0 && aitoolContext_0.HasParameter("elementId"))
			{
				int_3 = aitoolContext_0.GetParameter<int>("elementId", 0);
				if (int_3 > 0)
				{
					list_0.Add(int_3);
				}
			}
			if (list_0.Count == 0 && aitoolContext_0.HasParameter("elementIds"))
			{
				object_4 = aitoolContext_0.GetParameter<object>("elementIds", (object)null);
				if (object_4 is int)
				{
					int_4 = (int)object_4;
					if (true)
					{
						list_0.Add(int_4);
						goto IL_04f2;
					}
				}
				if (object_4 is long)
				{
					long_0 = (long)object_4;
					if (true)
					{
						list_0.Add((int)long_0);
						goto IL_04f2;
					}
				}
				int_5 = object_4 as int[];
				if (int_5 != null)
				{
					list_0.AddRange(int_5);
				}
				else
				{
					long_1 = object_4 as long[];
					if (long_1 != null)
					{
						list_0.AddRange(Array.ConvertAll(long_1, (long long_0) => (int)long_0));
					}
					else
					{
						list_3 = object_4 as List<int>;
						if (list_3 != null)
						{
							list_0.AddRange(list_3);
						}
						else
						{
							list_4 = object_4 as List<long>;
							if (list_4 != null)
							{
								list_0.AddRange(list_4.ConvertAll((long long_0) => (int)long_0));
							}
							else
							{
								list_5 = object_4 as List<object>;
								if (list_5 != null)
								{
									try
									{
										list_0.AddRange(Array.ConvertAll(list_5.ToArray(), Convert.ToInt32));
									}
									catch
									{
										result = AIToolResult.Fail("elementIds 数组中包含非整数值");
										goto IL_0ce6;
									}
								}
								list_5 = null;
							}
							list_4 = null;
						}
						list_3 = null;
					}
					long_1 = null;
				}
				int_5 = null;
				goto IL_04f2;
			}
			goto IL_050f;
			IL_050f:
			if (list_0.Count == 0)
			{
				result = AIToolResult.Fail("get_element_materials 操作需要提供 elementId、elementIds 或 cacheId 参数");
			}
			else
			{
				list_1 = new List<object>();
				list_2 = new List<int>();
				int_1 = 0;
				enumerator_0 = list_0.GetEnumerator();
				try
				{
					while (enumerator_0.MoveNext())
					{
						int_6 = enumerator_0.Current;
						try
						{
							object_5 = object_1.GetType().GetMethod("GetElementById")?.Invoke(object_1, new object[2] { aitoolContext_0.Document, int_6 });
							if (object_5 == null)
							{
								list_2.Add(int_6);
								continue;
							}
							methodInfo_0 = object_0.GetType().GetMethod("GetElementMaterials");
							ienumerable_1 = methodInfo_0?.Invoke(object_0, new object[1] { object_5 }) as IEnumerable;
							list_6 = new List<object>();
							if (ienumerable_1 != null)
							{
								ienumerator_1 = ienumerable_1.GetEnumerator();
								try
								{
									while (ienumerator_1.MoveNext())
									{
										object_6 = ienumerator_1.Current;
										try
										{
											object_7 = object_1.GetType().GetMethod("GetElementId")?.Invoke(object_1, new object[1] { object_6 });
											string_5 = object_1.GetType().GetMethod("GetElementName")?.Invoke(object_1, new object[1] { object_6 })?.ToString();
											string_6 = object_0.GetType().GetMethod("GetMaterialClass")?.Invoke(object_0, new object[1] { object_6 })?.ToString();
											object_8 = object_0.GetType().GetMethod("GetMaterialColor")?.Invoke(object_0, new object[1] { object_6 });
											byte_0 = 0;
											byte_1 = 0;
											byte_2 = 0;
											if (object_8 != null)
											{
												propertyInfo_1 = object_8.GetType().GetProperty("R");
												propertyInfo_2 = object_8.GetType().GetProperty("G");
												propertyInfo_3 = object_8.GetType().GetProperty("B");
												if (propertyInfo_1 != null)
												{
													byte_0 = ((byte?)propertyInfo_1.GetValue(object_8)).GetValueOrDefault();
												}
												if (propertyInfo_2 != null)
												{
													byte_1 = ((byte?)propertyInfo_2.GetValue(object_8)).GetValueOrDefault();
												}
												if (propertyInfo_3 != null)
												{
													byte_2 = ((byte?)propertyInfo_3.GetValue(object_8)).GetValueOrDefault();
												}
												propertyInfo_1 = null;
												propertyInfo_2 = null;
												propertyInfo_3 = null;
											}
											list_6.Add(new Class254<int, string, string, Class246<byte, byte, byte>>(Convert.ToInt32(object_7 ?? ((object)0)), string_5 ?? "未命名", string_6 ?? "未知", new Class246<byte, byte, byte>(byte_0, byte_1, byte_2)));
											object_7 = null;
											string_5 = null;
											string_6 = null;
											object_8 = null;
										}
										catch
										{
										}
										object_6 = null;
									}
								}
								finally
								{
									if (num < 0 && ienumerator_1 is IDisposable disposable2)
									{
										disposable2.Dispose();
									}
								}
								ienumerator_1 = null;
								int_1 += list_6.Count;
							}
							string_3 = object_1.GetType().GetMethod("GetElementName")?.Invoke(object_1, new object[1] { object_5 })?.ToString();
							string_4 = object_1.GetType().GetMethod("GetElementCategory")?.Invoke(object_1, new object[1] { object_5 })?.ToString();
							list_1.Add(new Class255<int, string, string, int, List<object>>(int_6, string_3 ?? "未命名", string_4 ?? "未知", list_6.Count, list_6));
							object_5 = null;
							methodInfo_0 = null;
							ienumerable_1 = null;
							list_6 = null;
							string_3 = null;
							string_4 = null;
						}
						catch
						{
							list_2.Add(int_6);
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
				if (list_1.Count == 0)
				{
					result = AIToolResult.Fail("找不到任何指定的元素");
				}
				else
				{
					string text;
					if (list_0.Count != 1)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
						defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler.AppendLiteral(" 个元素");
						text = defaultInterpolatedStringHandler.ToStringAndClear();
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("元素 ");
						defaultInterpolatedStringHandler2.AppendFormatted(list_0[0]);
						text = defaultInterpolatedStringHandler2.ToStringAndClear();
					}
					string_0 = text;
					string text2;
					if (int_1 != 1)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(4, 1);
						defaultInterpolatedStringHandler3.AppendFormatted(int_1);
						defaultInterpolatedStringHandler3.AppendLiteral(" 个材质");
						text2 = defaultInterpolatedStringHandler3.ToStringAndClear();
					}
					else
					{
						text2 = "个材质";
					}
					string_1 = text2;
					string[] obj4 = new string[5]
					{
						"✅ 成功查询 ",
						string_0,
						"，共找到 ",
						string_1,
						null
					};
					string text3;
					if (list_2.Count <= 0)
					{
						text3 = "";
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(8, 1);
						defaultInterpolatedStringHandler4.AppendLiteral("，");
						defaultInterpolatedStringHandler4.AppendFormatted(list_2.Count);
						defaultInterpolatedStringHandler4.AppendLiteral(" 个元素未找到");
						text3 = defaultInterpolatedStringHandler4.ToStringAndClear();
					}
					obj4[4] = text3;
					result = AIToolResult.Ok(string.Concat(obj4), (object)new Class256<string, int, int, int, List<int>, int, List<object>>("get_element_materials", list_0.Count, list_1.Count, list_2.Count, (list_2.Count > 0) ? list_2 : null, int_1, list_1));
				}
			}
			goto IL_0ce6;
			IL_0ce6:
			int_0 = -2;
			list_0 = null;
			list_1 = null;
			list_2 = null;
			string_0 = null;
			string_1 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
			return;
			IL_04f2:
			list_0 = list_0.Distinct().ToList();
			object_4 = null;
			goto IL_050f;
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class573 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public object object_1;

		public MaterialQueryTool materialQueryTool_0;

		private string string_0;

		private MethodInfo methodInfo_0;

		private object object_2;

		private object object_3;

		private string string_1;

		private object object_4;

		private string string_2;

		private object object_5;

		private byte byte_0;

		private byte byte_1;

		private byte byte_2;

		private float float_0;

		private PropertyInfo propertyInfo_0;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo propertyInfo_2;

		private PropertyInfo propertyInfo_3;

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
					Class573 stateMachine = this;
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
				result = AIToolResult.Fail("get_by_name 操作需要指定 materialName 参数");
			}
			else
			{
				methodInfo_0 = object_0.GetType().GetMethod("GetMaterialByName");
				object_2 = methodInfo_0?.Invoke(object_0, new object[2] { aitoolContext_0.Document, string_0 });
				if (object_2 == null)
				{
					result = AIToolResult.Fail("找不到名称为 '" + string_0 + "' 的材质");
				}
				else
				{
					object_3 = object_1.GetType().GetMethod("GetElementId")?.Invoke(object_1, new object[1] { object_2 });
					string_1 = object_0.GetType().GetMethod("GetMaterialClass")?.Invoke(object_0, new object[1] { object_2 })?.ToString();
					object_4 = object_0.GetType().GetMethod("GetMaterialColor")?.Invoke(object_0, new object[1] { object_2 });
					string_2 = object_0.GetType().GetMethod("GetAppearanceName")?.Invoke(object_0, new object[1] { object_2 })?.ToString();
					object_5 = object_0.GetType().GetMethod("GetMaterialProperties")?.Invoke(object_0, new object[1] { object_2 });
					byte_0 = 0;
					byte_1 = 0;
					byte_2 = 0;
					if (object_4 != null)
					{
						propertyInfo_0 = object_4.GetType().GetProperty("R");
						propertyInfo_1 = object_4.GetType().GetProperty("G");
						propertyInfo_2 = object_4.GetType().GetProperty("B");
						if (propertyInfo_0 != null)
						{
							byte_0 = ((byte?)propertyInfo_0.GetValue(object_4)).GetValueOrDefault();
						}
						if (propertyInfo_1 != null)
						{
							byte_1 = ((byte?)propertyInfo_1.GetValue(object_4)).GetValueOrDefault();
						}
						if (propertyInfo_2 != null)
						{
							byte_2 = ((byte?)propertyInfo_2.GetValue(object_4)).GetValueOrDefault();
						}
						propertyInfo_0 = null;
						propertyInfo_1 = null;
						propertyInfo_2 = null;
					}
					float_0 = 1f;
					if (object_5 != null)
					{
						propertyInfo_3 = object_5.GetType().GetProperty("Alpha");
						if (propertyInfo_3 != null)
						{
							float_0 = ((float?)propertyInfo_3.GetValue(object_5)) ?? 1f;
						}
						propertyInfo_3 = null;
					}
					result = AIToolResult.Ok("找到材质: " + string_0, (object)new Class253<string, int, string, string, Class246<byte, byte, byte>, string, bool, float>("get_by_name", Convert.ToInt32(object_3 ?? ((object)0)), string_0, string_1 ?? "未知", new Class246<byte, byte, byte>(byte_0, byte_1, byte_2), string_2 ?? "未设置", float_0 > 0f, float_0));
				}
			}
			int_0 = -2;
			string_0 = null;
			methodInfo_0 = null;
			object_2 = null;
			object_3 = null;
			string_1 = null;
			object_4 = null;
			string_2 = null;
			object_5 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "material_query";

	public string Category => "材料管理";

	public string Description => "查询材质信息，支持获取所有材质、按名称查询、查询元素材质";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operation\": {\n                \"type\": \"string\",\n                \"enum\": [\"get_all\", \"get_by_name\", \"get_element_materials\"],\n                \"description\": \"查询操作类型：get_all(获取所有材质)、get_by_name(按名称获取材质)、get_element_materials(获取元素的材质)\"\n            },\n            \"filter\": {\n                \"type\": \"string\",\n                \"description\": \"材质名称过滤（仅 get_all 操作使用，可选）。模糊匹配材质名称。\"\n            },\n            \"materialName\": {\n                \"type\": \"string\",\n                \"description\": \"材质名称（仅 get_by_name 操作使用，必需）。支持中文，如'混凝土'、'砖'等。\"\n            },\n            \"elementId\": {\n                \"type\": \"integer\",\n                \"description\": \"单个元素 ID（仅 get_element_materials 操作使用，可选）。查询单个元素的材质。\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"元素 ID 数组（仅 get_element_materials 操作使用，可选）。批量查询多个元素的材质。\"\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（仅 get_element_materials 操作使用，可选）。从上一个查询工具的返回结果中获取 cache_id 字段。\"\n            }\n        },\n        \"required\": [\"operation\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class570))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class570 stateMachine = new Class570();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.materialQueryTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class571))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, object object_0, object object_1)
	{
		Class571 stateMachine = new Class571();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.materialQueryTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.object_1 = object_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class573))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, object object_0, object object_1)
	{
		Class573 stateMachine = new Class573();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.materialQueryTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.object_1 = object_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class572))]
	private Task<AIToolResult> method_2(AIToolContext aitoolContext_0, object object_0, object object_1)
	{
		Class572 stateMachine = new Class572();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.materialQueryTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.object_1 = object_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private void method_3(AIToolResult aitoolResult_0, Func<string, string> func_0)
	{
		try
		{
			if (aitoolResult_0.Data == null)
			{
				return;
			}
			PropertyInfo property = aitoolResult_0.Data.GetType().GetProperty("cache_info");
			if (!(property != null))
			{
				return;
			}
			object value = property.GetValue(aitoolResult_0.Data);
			if (value == null)
			{
				return;
			}
			PropertyInfo property2 = value.GetType().GetProperty("cache_id");
			if (property2 != null)
			{
				string text = property2.GetValue(value)?.ToString();
				if (!string.IsNullOrEmpty(text))
				{
					aitoolResult_0.Message = func_0(text);
				}
			}
		}
		catch
		{
		}
	}
}
