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
using RevitAi.Abstractions.Units;
using RevitAi.Core.Units;
using ns7;

namespace RevitAi.Core.AI;

public sealed class AIToolInvocationHandler
{
	[CompilerGenerated]
	public sealed class Class111
	{
		[StructLayout(LayoutKind.Auto)]
		public struct Struct185 : IAsyncStateMachine
		{
			public int int_0;

			public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

			public Class111 class111_0;

			public AIToolContext aitoolContext_0;

			private TaskAwaiter<AIToolResult> taskAwaiter_0;

			void IAsyncStateMachine.MoveNext()
			{
				int num = int_0;
				Class111 @class = class111_0;
				TaskAwaiter<AIToolResult> awaiter;
				if (num != 0)
				{
					awaiter = @class.iaitool_0.ExecuteAsync(aitoolContext_0, @class.cancellationToken_0).GetAwaiter();
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
				AIToolResult result = awaiter.GetResult();
				int_0 = -2;
				asyncTaskMethodBuilder_0.SetResult(result);
			}

			[DebuggerHidden]
			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
			{
				asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
			}
		}

		public CancellationToken cancellationToken_0;

		public IAITool iaitool_0;

		[AsyncStateMachine(typeof(Struct185))]
		internal Task<AIToolResult> method_0(AIToolContext aitoolContext_0)
		{
			Struct185 stateMachine = default(Struct185);
			stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
			stateMachine.class111_0 = this;
			stateMachine.aitoolContext_0 = aitoolContext_0;
			stateMachine.int_0 = -1;
			stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
			return stateMachine.asyncTaskMethodBuilder_0.Task;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct186 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public CancellationToken cancellationToken_0;

		public ToolCallInfo toolCallInfo_0;

		public AIToolInvocationHandler aitoolInvocationHandler_0;

		public AIToolContext aitoolContext_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AIToolInvocationHandler aIToolInvocationHandler = aitoolInvocationHandler_0;
			Class111 @class = default(Class111);
			AIToolResult result;
			bool flag = default(bool);
			if ((uint)num > 1u)
			{
				@class = new Class111();
				@class.cancellationToken_0 = cancellationToken_0;
				if (toolCallInfo_0 == null)
				{
					throw new ArgumentNullException("toolCall");
				}
				Logger.Info("[AIToolInvocation] 正在执行工具: " + toolCallInfo_0.ToolName);
				if (string.IsNullOrEmpty(toolCallInfo_0.ToolName))
				{
					Logger.Error("[AIToolInvocation] 工具名称为空或 null");
					result = AIToolResult.Fail("工具名称不能为空");
					goto IL_04e2;
				}
				flag = aIToolInvocationHandler.method_5(toolCallInfo_0);
			}
			try
			{
				TaskAwaiter<AIToolResult> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_03e1;
				}
				if (num == 1)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_03b9;
				}
				@class.iaitool_0 = aIToolInvocationHandler.iaitoolRegistry_0.GetTool(toolCallInfo_0.ToolName);
				if (@class.iaitool_0 == null)
				{
					Logger.Error("[AIToolInvocation] 工具不存在: " + toolCallInfo_0.ToolName);
					result = AIToolResult.Fail("工具 '" + toolCallInfo_0.ToolName + "' 不存在");
				}
				else
				{
					if (toolCallInfo_0.Parameters != null)
					{
						List<KeyValuePair<string, object>>.Enumerator enumerator = toolCallInfo_0.Parameters.ToList().GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<string, object> current = enumerator.Current;
								aitoolContext_0.Parameters[current.Key] = current.Value;
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
							}
						}
					}
					if (aIToolInvocationHandler.iaitoolDataCache_0 != null)
					{
						aitoolContext_0.DataCache = aIToolInvocationHandler.iaitoolDataCache_0;
						if (toolCallInfo_0.Parameters != null && toolCallInfo_0.Parameters.TryGetValue("sessionId", out object value))
						{
							aitoolContext_0.SessionId = value?.ToString();
						}
					}
					if (aitoolContext_0.Document != null && aitoolContext_0.RevitAdapter != null)
					{
						try
						{
							IProjectUnitService val = aIToolInvocationHandler.method_4();
							if (val != null)
							{
								UnitService unitService = new UnitService(val, aitoolContext_0.Document);
								aitoolContext_0.UnitService = (IUnitService)(object)unitService;
								aitoolContext_0.ProjectUnitService = val;
							}
							else
							{
								Logger.Warning("[AIToolInvocation] 无法从适配层获取 ProjectUnitService");
							}
						}
						catch (Exception ex)
						{
							Logger.Warning("[AIToolInvocation] 注入单位服务失败: " + ex.Message);
						}
					}
					if (!(aIToolInvocationHandler.method_6(toolCallInfo_0) | flag))
					{
						awaiter = @class.iaitool_0.ExecuteAsync(aitoolContext_0, @class.cancellationToken_0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_03b9;
					}
					if (aitoolContext_0.ExternalEvent != null)
					{
						awaiter = new RevitExternalEventHandler(aitoolContext_0.ExternalEvent, null, "AI Tool: " + toolCallInfo_0.ToolName, aIToolInvocationHandler.iaitoolDataCache_0).ExecuteAsync(aitoolContext_0, flag, @class.method_0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_03e1;
					}
					Logger.Error("[AIToolInvocation] 工具 " + toolCallInfo_0.ToolName + " 需要在 Revit API 上下文中执行，但 ExternalEvent 未提供");
					result = AIToolResult.Fail("工具需要在 Revit API 上下文中执行，但未提供 ExternalEvent");
				}
				goto end_IL_00a3;
				IL_03b9:
				AIToolResult result2 = awaiter.GetResult();
				goto IL_03ea;
				IL_03e1:
				result2 = awaiter.GetResult();
				goto IL_03ea;
				IL_03ea:
				if (result2.Success)
				{
					string text = ((result2.Message == null || result2.Message.Length <= 120) ? result2.Message : (result2.Message.Substring(0, 120) + "...（已截断）"));
					Logger.Info("[AIToolInvocation] 工具 " + toolCallInfo_0.ToolName + " 执行成功: " + text);
				}
				else
				{
					Logger.Error("[AIToolInvocation] 工具 " + toolCallInfo_0.ToolName + " 执行失败: " + result2.Error);
				}
				result = result2;
				end_IL_00a3:;
			}
			catch (Exception ex2)
			{
				Logger.Error("[AIToolInvocation] 执行工具 " + toolCallInfo_0.ToolName + " 时发生异常", ex2);
				result = AIToolResult.Fail("执行工具时发生异常: " + ex2.Message);
			}
			goto IL_04e2;
			IL_04e2:
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly IAIToolRegistry iaitoolRegistry_0;

	private readonly IAIToolDataCache? iaitoolDataCache_0;

	public AIToolInvocationHandler(IAIToolRegistry toolRegistry, IAIToolDataCache? dataCache = null)
	{
		iaitoolRegistry_0 = toolRegistry ?? throw new ArgumentNullException("toolRegistry");
		iaitoolDataCache_0 = dataCache;
	}

	[AsyncStateMachine(typeof(Struct186))]
	public Task<AIToolResult> HandleToolCallAsync(ToolCallInfo toolCall, AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct186 stateMachine = default(Struct186);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.aitoolInvocationHandler_0 = this;
		stateMachine.toolCallInfo_0 = toolCall;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private object? method_0(AIToolContext aitoolContext_0, string string_0)
	{
		try
		{
			if (aitoolContext_0.Document == null)
			{
				return null;
			}
			Type type = aitoolContext_0.Document.GetType();
			if (type.Assembly.GetType("Autodesk.Revit.ApplicationServices.Application") == null)
			{
				return null;
			}
			PropertyInfo property = type.GetProperty("Application");
			if (property == null)
			{
				return null;
			}
			if (property.GetValue(aitoolContext_0.Document) == null)
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
			return constructor.Invoke(new object[2] { aitoolContext_0.Document, string_0 });
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIToolInvocation] 创建事务失败: " + ex.Message);
			return null;
		}
	}

	private void method_1(object object_0)
	{
		try
		{
			MethodInfo methodInfo = object_0.GetType().GetMethods().FirstOrDefault((MethodInfo methodInfo_0) => methodInfo_0.Name == "Start" && methodInfo_0.GetParameters().Length == 0);
			if (methodInfo == null)
			{
				Logger.Warning("[AIToolInvocation] 未找到无参数的 Start 方法");
			}
			else
			{
				methodInfo.Invoke(object_0, null);
			}
		}
		catch (TargetInvocationException ex)
		{
			Exception innerException = ex.InnerException;
			Logger.Error("[AIToolInvocation] 启动事务失败（内部异常）: " + innerException?.GetType().Name + " - " + innerException?.Message);
			if (innerException != null)
			{
				Logger.Error("[AIToolInvocation] 内部异常堆栈: " + innerException.StackTrace);
			}
			throw;
		}
		catch (Exception ex2)
		{
			Logger.Error("[AIToolInvocation] 启动事务失败: " + ex2.GetType().Name + " - " + ex2.Message);
			Logger.Error("[AIToolInvocation] 异常堆栈: " + ex2.StackTrace);
			throw;
		}
	}

	private void method_2(object object_0)
	{
		try
		{
			MethodInfo methodInfo = object_0.GetType().GetMethods().FirstOrDefault((MethodInfo methodInfo_0) => methodInfo_0.Name == "Commit" && methodInfo_0.GetParameters().Length == 0);
			if (methodInfo == null)
			{
				Logger.Warning("[AIToolInvocation] 未找到无参数的 Commit 方法");
			}
			else
			{
				methodInfo.Invoke(object_0, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIToolInvocation] 提交事务失败: " + ex.Message);
		}
	}

	private void method_3(object object_0)
	{
		try
		{
			MethodInfo methodInfo = object_0.GetType().GetMethods().FirstOrDefault((MethodInfo methodInfo_0) => methodInfo_0.Name == "Rollback" && methodInfo_0.GetParameters().Length == 0);
			if (methodInfo == null)
			{
				Logger.Warning("[AIToolInvocation] 未找到无参数的 Rollback 方法");
			}
			else
			{
				methodInfo.Invoke(object_0, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIToolInvocation] 回滚事务失败: " + ex.Message);
		}
	}

	private IProjectUnitService? method_4()
	{
		try
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			int num = 0;
			IProjectUnitService val;
			while (true)
			{
				if (num < assemblies.Length)
				{
					Assembly assembly = assemblies[num];
					if (assembly.FullName != null && assembly.FullName.Contains("RevitAi.Revit"))
					{
						Type type = assembly.GetType("RevitAi.Revit.ProjectUnitService");
						if (type == null)
						{
							type = assembly.GetType("RevitAi.Revit.Units.ProjectUnitService");
						}
						if (type != null)
						{
							object? obj = Activator.CreateInstance(type);
							val = (IProjectUnitService)((obj is IProjectUnitService) ? obj : null);
							if (val != null)
							{
								break;
							}
						}
					}
					num++;
					continue;
				}
				Logger.Warning("[AIToolInvocation] 未找到适配层的 ProjectUnitService");
				return null;
			}
			return val;
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIToolInvocation] 从适配层获取 ProjectUnitService 失败: " + ex.Message);
			return null;
		}
	}

	private bool method_5(ToolCallInfo toolCallInfo_0)
	{
		try
		{
			IAITool tool = iaitoolRegistry_0.GetTool(toolCallInfo_0.ToolName);
			if (tool == null)
			{
				return false;
			}
			object[] customAttributes = ((object)tool).GetType().GetCustomAttributes(typeof(AIToolAttribute), inherit: false);
			if (customAttributes.Length != 0)
			{
				object obj = customAttributes[0];
				AIToolAttribute val = (AIToolAttribute)((obj is AIToolAttribute) ? obj : null);
				if (val != null)
				{
					return val.RequiresTransaction;
				}
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	private bool method_6(ToolCallInfo toolCallInfo_0)
	{
		try
		{
			IAITool tool = iaitoolRegistry_0.GetTool(toolCallInfo_0.ToolName);
			if (tool == null)
			{
				return false;
			}
			object[] customAttributes = ((object)tool).GetType().GetCustomAttributes(typeof(AIToolAttribute), inherit: false);
			if (customAttributes.Length != 0)
			{
				object obj = customAttributes[0];
				AIToolAttribute val = (AIToolAttribute)((obj is AIToolAttribute) ? obj : null);
				if (val != null)
				{
					return val.RequiresActiveDocument;
				}
			}
			return false;
		}
		catch
		{
			return false;
		}
	}
}
