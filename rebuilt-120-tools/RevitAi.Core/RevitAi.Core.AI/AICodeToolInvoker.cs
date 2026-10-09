using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ns7;

namespace RevitAi.Core.AI;

public sealed class AICodeToolInvoker : IAICodeToolInvoker
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct183 : IAsyncStateMachine
	{
			private AIToolContext val;
			private IAITool tool;
		public int int_0;

		public AsyncTaskMethodBuilder<ToolCallResult> asyncTaskMethodBuilder_0;

		public string string_0;

		public AICodeToolInvoker aicodeToolInvoker_0;

		public object object_0;

		private int int_1;

		private TaskAwaiter<AIToolResult> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Expected O, but got Unknown
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Expected O, but got Unknown
			int num = int_0;
			AICodeToolInvoker aICodeToolInvoker = aicodeToolInvoker_0;
			if ((uint)num <= 1u)
			{
				goto IL_02f5;
			}
			ToolCallResult result;
			if (string.IsNullOrWhiteSpace(string_0))
			{
				result = smethod_7("工具名称不能为空");
			}
			else
			{
				int_1 = asyncLocal_0.Value;
				if (int_1 >= 3)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 3);
					defaultInterpolatedStringHandler.AppendLiteral("[AICodeToolInvoker] 调用深度超限（当前 ");
					defaultInterpolatedStringHandler.AppendFormatted(int_1);
					defaultInterpolatedStringHandler.AppendLiteral("，最大 ");
					defaultInterpolatedStringHandler.AppendFormatted(3);
					defaultInterpolatedStringHandler.AppendLiteral("），工具: ");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("调用深度超限（最大 ");
					defaultInterpolatedStringHandler2.AppendFormatted(3);
					defaultInterpolatedStringHandler2.AppendLiteral(" 层嵌套调用）");
					result = smethod_7(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
				else if (hashSet_0.Contains(string_0))
				{
					Logger.Warning("[AICodeToolInvoker] 禁止在代码片段中调用工具 '" + string_0 + "'（避免递归）");
					result = smethod_7("禁止调用工具 '" + string_0 + "'（避免代码片段递归调用）");
				}
				else
				{
					tool = aICodeToolInvoker.iaitoolRegistry_0.GetTool(string_0);
					if (tool != null)
					{
						val = new AIToolContext
						{
							Document = aICodeToolInvoker.aitoolContext_0.Document,
							RevitAdapter = aICodeToolInvoker.aitoolContext_0.RevitAdapter,
							ExternalEvent = aICodeToolInvoker.aitoolContext_0.ExternalEvent,
							UserData = aICodeToolInvoker.aitoolContext_0.UserData,
							SessionId = aICodeToolInvoker.aitoolContext_0.SessionId,
							DataCache = (aICodeToolInvoker.iaitoolDataCache_0 ?? aICodeToolInvoker.aitoolContext_0.DataCache),
							UnitService = aICodeToolInvoker.aitoolContext_0.UnitService,
							ProjectUnitService = aICodeToolInvoker.aitoolContext_0.ProjectUnitService,
							Parameters = new Dictionary<string, object>()
						};
						if (object_0 != null)
						{
							try
							{
								IEnumerator<JProperty> enumerator = JObject.Parse(JsonConvert.SerializeObject(object_0)).Properties().GetEnumerator();
								try
								{
									while (enumerator.MoveNext())
									{
										JProperty current = enumerator.Current;
										val.Parameters[current.Name] = smethod_1(current.Value) ?? new object();
									}
								}
								finally
								{
									if (num < 0)
									{
										enumerator?.Dispose();
									}
								}
							}
							catch (Exception ex)
							{
								result = smethod_7("参数序列化失败: " + ex.Message);
								goto IL_04df;
							}
						}
						asyncLocal_0.Value = int_1 + 1;
						goto IL_02f5;
					}
					result = smethod_7("工具 '" + string_0 + "' 不存在");
				}
			}
			goto IL_04df;
			IL_04df:
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
			return;
			IL_02f5:
			try
			{
				TaskAwaiter<AIToolResult> awaiter;
				AIToolResult result2;
				if (num != 0)
				{
					if (num != 1)
					{
						if (smethod_0(tool))
						{
							awaiter = aICodeToolInvoker.method_0(tool, val, string_0).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = 0;
								int_0 = 0;
								taskAwaiter_0 = awaiter;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
							goto IL_03d6;
						}
						awaiter = tool.ExecuteAsync(val, CancellationToken.None).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = taskAwaiter_0;
						taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
					}
					result2 = awaiter.GetResult();
					goto IL_03df;
				}
				awaiter = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
				num = -1;
				int_0 = -1;
				goto IL_03d6;
				IL_03d6:
				result2 = awaiter.GetResult();
				goto IL_03df;
				IL_03df:
				object obj = smethod_2(result2.Data);
				if (result2.Success)
				{
					Logger.Info("[AICodeToolInvoker] 工具 " + string_0 + " 调用成功: " + result2.Message);
				}
				else
				{
					Logger.Warning("[AICodeToolInvoker] 工具 " + string_0 + " 调用失败: " + result2.Error);
				}
				result = new ToolCallResult(result2.Success, result2.Message, obj, result2.Error);
			}
			catch (Exception ex2)
			{
				Logger.Error("[AICodeToolInvoker] 调用工具 " + string_0 + " 时发生异常: " + ex2.Message, ex2);
				result = smethod_7("调用工具 " + string_0 + " 时发生异常: " + ex2.Message);
			}
			finally
			{
				if (num < 0)
				{
					asyncLocal_0.Value = int_1;
				}
			}
			goto IL_04df;
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct184 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public string string_0;

		public IAITool iaitool_0;

		private object object_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			if (num == 0)
			{
				goto IL_00e6;
			}
			AIToolResult result;
			if (aitoolContext_0.Document == null)
			{
				result = AIToolResult.Fail("工具 '" + string_0 + "' 需要事务，但上下文中没有 Document");
			}
			else
			{
				object_0 = smethod_3(aitoolContext_0, "AI Code: " + string_0);
				if (object_0 != null)
				{
					try
					{
						smethod_4(object_0);
					}
					catch (Exception ex)
					{
						Logger.Error("[AICodeToolInvoker] 启动事务失败: " + ex.Message);
						result = AIToolResult.Fail("启动事务失败: " + ex.Message);
						goto IL_01f9;
					}
					goto IL_00e6;
				}
				result = AIToolResult.Fail("工具 '" + string_0 + "' 需要事务，但创建事务失败");
			}
			goto IL_01f9;
			IL_00e6:
			AIToolResult result2;
			try
			{
				TaskAwaiter<AIToolResult> awaiter;
				if (num != 0)
				{
					awaiter = iaitool_0.ExecuteAsync(aitoolContext_0, CancellationToken.None).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
				}
				result2 = awaiter.GetResult();
			}
			catch (Exception ex2)
			{
				Logger.Error("[AICodeToolInvoker] 工具 " + string_0 + " 抛出异常，回滚事务", ex2);
				smethod_6(object_0);
				result = AIToolResult.Fail("工具执行抛出异常: " + ex2.Message);
				goto IL_01f9;
			}
			if (result2.Success)
			{
				smethod_5(object_0);
			}
			else
			{
				Logger.Warning("[AICodeToolInvoker] 工具 " + string_0 + " 执行失败，回滚事务: " + result2.Error);
				smethod_6(object_0);
			}
			result = result2;
			goto IL_01f9;
			IL_01f9:
			int_0 = -2;
			object_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly IAIToolRegistry iaitoolRegistry_0;

	private readonly AIToolContext aitoolContext_0;

	private readonly IAIToolDataCache? iaitoolDataCache_0;

	private const int int_0 = 3;

	private static readonly AsyncLocal<int> asyncLocal_0 = new AsyncLocal<int>();

	private static readonly HashSet<string> hashSet_0 = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		"execute_code",
		"code_snippet"
	};

	public AICodeToolInvoker(IAIToolRegistry registry, AIToolContext parentContext, IAIToolDataCache? dataCache = null)
	{
		iaitoolRegistry_0 = registry ?? throw new ArgumentNullException("registry");
		aitoolContext_0 = parentContext ?? throw new ArgumentNullException("parentContext");
		iaitoolDataCache_0 = dataCache;
	}

	public ToolCallResult Call(string toolName, object? parameters = null)
	{
		return CallAsync(toolName, parameters).GetAwaiter().GetResult();
	}

	[AsyncStateMachine(typeof(Struct183))]
	public Task<ToolCallResult> CallAsync(string toolName, object? parameters = null)
	{
		Struct183 stateMachine = default(Struct183);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<ToolCallResult>.Create();
		stateMachine.aicodeToolInvoker_0 = this;
		stateMachine.string_0 = toolName;
		stateMachine.object_0 = parameters;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public IEnumerable<string> ListTools()
	{
		return from iaitool_0 in iaitoolRegistry_0.GetAllTools()
			where !hashSet_0.Contains(iaitool_0.Name)
			select iaitool_0.Name into string_0
			orderby string_0
			select string_0;
	}

	public string? GetToolDescription(string toolName)
	{
		IAITool tool = iaitoolRegistry_0.GetTool(toolName);
		if (tool == null)
		{
			return null;
		}
		return tool.Description;
	}

	[AsyncStateMachine(typeof(Struct184))]
	private Task<AIToolResult> method_0(IAITool iaitool_0, AIToolContext aitoolContext_1, string string_0)
	{
		Struct184 stateMachine = default(Struct184);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.iaitool_0 = iaitool_0;
		stateMachine.aitoolContext_0 = aitoolContext_1;
		stateMachine.string_0 = string_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private static bool smethod_0(IAITool iaitool_0)
	{
		try
		{
			object? obj = ((object)iaitool_0).GetType().GetCustomAttributes(typeof(AIToolAttribute), inherit: false).FirstOrDefault();
			object? obj2 = ((obj is AIToolAttribute) ? obj : null);
			return obj2 != null && ((AIToolAttribute)obj2).RequiresTransaction;
		}
		catch
		{
			return false;
		}
	}

	private static object? smethod_1(JToken? jtoken_0)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected I4, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		if (jtoken_0 == null)
		{
			return null;
		}
		JTokenType type = jtoken_0.Type;
		switch ((int)(type) - 1)
		{
		case 0:
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			{
				foreach (JProperty item in ((JObject)jtoken_0).Properties())
				{
					dictionary[item.Name] = smethod_1(item.Value) ?? new object();
				}
				return dictionary;
			}
		}
		case 1:
		{
			List<object> list = new List<object>();
			{
				foreach (JToken item2 in (JArray)jtoken_0)
				{
					list.Add(smethod_1(item2) ?? new object());
				}
				return list;
			}
		}
		default:
			return ((JValue)jtoken_0).Value;
		case 5:
			return ((JValue)jtoken_0).Value;
		case 6:
			return ((JValue)jtoken_0).Value;
		case 7:
			return ((object)jtoken_0).ToString();
		case 8:
			return (bool)(JToken)(JValue)jtoken_0;
		case 9:
		case 10:
			return null;
		}
	}

	private static object? smethod_2(object? object_0)
	{
		if (object_0 == null)
		{
			return null;
		}
		if (!(object_0 is string) && !(object_0 is bool) && !(object_0 is char) && !(object_0 is byte) && !(object_0 is sbyte) && !(object_0 is short) && !(object_0 is ushort) && !(object_0 is int) && !(object_0 is uint) && !(object_0 is long) && !(object_0 is ulong) && !(object_0 is float) && !(object_0 is double) && !(object_0 is decimal) && !(object_0 is DateTime) && !(object_0 is TimeSpan) && !(object_0 is Enum))
		{
			try
			{
				return smethod_1(JToken.Parse(JsonConvert.SerializeObject(object_0)));
			}
			catch
			{
				return object_0;
			}
		}
		return object_0;
	}

	private static object? smethod_3(AIToolContext aitoolContext_1, string string_0)
	{
		try
		{
			if (aitoolContext_1.Document == null)
			{
				return null;
			}
			Type type = aitoolContext_1.Document.GetType();
			if (type.Assembly.GetType("Autodesk.Revit.ApplicationServices.Application") == null)
			{
				return null;
			}
			PropertyInfo property = type.GetProperty("Application");
			if (property == null)
			{
				return null;
			}
			if (property.GetValue(aitoolContext_1.Document) == null)
			{
				return null;
			}
			Type type2 = type.Assembly.GetType("Autodesk.Revit.DB.Transaction");
			if (type2 == null)
			{
				return null;
			}
			ConstructorInfo constructor = type2.GetConstructor(new Type[2]
			{
				type,
				typeof(string)
			});
			if (constructor == null)
			{
				return null;
			}
			return constructor.Invoke(new object[2] { aitoolContext_1.Document, string_0 });
		}
		catch (Exception ex)
		{
			Logger.Warning("[AICodeToolInvoker] 创建事务失败: " + ex.Message);
			return null;
		}
	}

	private static void smethod_4(object object_0)
	{
		MethodInfo methodInfo = object_0.GetType().GetMethods().FirstOrDefault((MethodInfo methodInfo_0) => methodInfo_0.Name == "Start" && methodInfo_0.GetParameters().Length == 0);
		if (methodInfo == null)
		{
			Logger.Warning("[AICodeToolInvoker] 未找到无参数的 Start 方法");
		}
		else
		{
			methodInfo.Invoke(object_0, null);
		}
	}

	private static void smethod_5(object object_0)
	{
		MethodInfo methodInfo = object_0.GetType().GetMethods().FirstOrDefault((MethodInfo methodInfo_0) => methodInfo_0.Name == "Commit" && methodInfo_0.GetParameters().Length == 0);
		if (methodInfo == null)
		{
			Logger.Warning("[AICodeToolInvoker] 未找到无参数的 Commit 方法");
		}
		else
		{
			methodInfo.Invoke(object_0, null);
		}
	}

	private static void smethod_6(object object_0)
	{
		MethodInfo methodInfo = object_0.GetType().GetMethods().FirstOrDefault((MethodInfo methodInfo_0) => methodInfo_0.Name == "Rollback" && methodInfo_0.GetParameters().Length == 0);
		if (methodInfo == null)
		{
			Logger.Warning("[AICodeToolInvoker] 未找到无参数的 Rollback 方法");
		}
		else
		{
			methodInfo.Invoke(object_0, null);
		}
	}

	private static ToolCallResult smethod_7(string string_0)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		return new ToolCallResult(false, (string)null, (object)null, string_0);
	}
}
