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
using RevitAi.Revit.Net8.AI.Security;
using Autodesk.Revit.DB;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Execution;

public static class CodeExecutor
{
	[CompilerGenerated]
	public sealed class Class652 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public string string_0;

		public CodeExecutionGlobals codeExecutionGlobals_0;

		public int int_1;

		private ValidationResult validationResult_0;

		private ScriptOptions scriptOptions_0;

		private int int_2;

		private CancellationTokenSource cancellationTokenSource_0;

		private string string_1;

		private object object_0;

		private object object_1;

		private object object_2;

		private CompilationErrorException compilationErrorException_0;

		private string string_2;

		private KeyNotFoundException keyNotFoundException_0;

		private Exception exception_0;

		private Exception exception_1;

		private TaskAwaiter<object> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_040a: Expected O, but got Unknown
			int num = int_0;
			if (num == 0)
			{
				goto IL_022d;
			}
			AIToolResult result;
			if (string.IsNullOrWhiteSpace(string_0))
			{
				result = AIToolResult.Fail("[参数错误] code 不能为空");
			}
			else if (string_0.Length > 50000)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[参数错误] 代码长度 ");
				defaultInterpolatedStringHandler.AppendFormatted(string_0.Length);
				defaultInterpolatedStringHandler.AppendLiteral(" 超过限制 ");
				defaultInterpolatedStringHandler.AppendFormatted(50000);
				defaultInterpolatedStringHandler.AppendLiteral(" 字符");
				result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				validationResult_0 = CodeSandboxValidator.Validate(string_0);
				if (validationResult_0.IsValid)
				{
					scriptOptions_0 = ScriptOptionsFactory.Create();
					int_2 = Math.Min(120, Math.Max(1, int_1));
					cancellationTokenSource_0 = new CancellationTokenSource(TimeSpan.FromSeconds(int_2));
					goto IL_022d;
				}
				string_1 = "[" + validationResult_0.ErrorType;
				if (validationResult_0.LineNumber.HasValue)
				{
					string text = string_1;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 1);
					defaultInterpolatedStringHandler2.AppendLiteral(" 第");
					defaultInterpolatedStringHandler2.AppendFormatted(validationResult_0.LineNumber.Value);
					defaultInterpolatedStringHandler2.AppendLiteral("行");
					string_1 = text + defaultInterpolatedStringHandler2.ToStringAndClear();
				}
				string_1 = string_1 + "] " + validationResult_0.Error;
				if (!string.IsNullOrEmpty(validationResult_0.CodeSnippet))
				{
					string_1 = string_1 + "\n违规代码: " + validationResult_0.CodeSnippet;
				}
				Logger.Warning("[ExecuteCode] 安全审查拦截: " + string_1);
				result = AIToolResult.Fail(string_1);
			}
			goto IL_06eb;
			IL_06eb:
			int_0 = -2;
			validationResult_0 = null;
			scriptOptions_0 = null;
			cancellationTokenSource_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
			return;
			IL_022d:
			try
			{
				if (num == 0)
				{
				}
				try
				{
					TaskAwaiter<object> awaiter;
					if (num != 0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(30, 2);
						defaultInterpolatedStringHandler3.AppendLiteral("[ExecuteCode] 开始执行 (");
						defaultInterpolatedStringHandler3.AppendFormatted(string_0.Length);
						defaultInterpolatedStringHandler3.AppendLiteral(" 字符, 超时 ");
						defaultInterpolatedStringHandler3.AppendFormatted(int_2);
						defaultInterpolatedStringHandler3.AppendLiteral("s)");
						Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
						awaiter = CSharpScript.EvaluateAsync<object>(string_0, scriptOptions_0, (object)codeExecutionGlobals_0, (Type)null, cancellationTokenSource_0.Token).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							Class652 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
							return;
						}
					}
					else
					{
						awaiter = taskAwaiter_0;
						taskAwaiter_0 = default(TaskAwaiter<object>);
						num = -1;
						int_0 = -1;
					}
					object_2 = awaiter.GetResult();
					object_0 = object_2;
					object_2 = null;
					object_1 = smethod_0(object_0);
					Logger.Info("[ExecuteCode] 执行成功");
					result = AIToolResult.Ok("代码执行成功", (object)new Class0<object>(object_1));
				}
				catch (OperationCanceledException)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(22, 1);
					defaultInterpolatedStringHandler4.AppendLiteral("[ExecuteCode] 执行超时 (");
					defaultInterpolatedStringHandler4.AppendFormatted(int_2);
					defaultInterpolatedStringHandler4.AppendLiteral("s)");
					Logger.Warning(defaultInterpolatedStringHandler4.ToStringAndClear());
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(27, 1);
					defaultInterpolatedStringHandler5.AppendLiteral("[超时] 代码执行超过 ");
					defaultInterpolatedStringHandler5.AppendFormatted(int_2);
					defaultInterpolatedStringHandler5.AppendLiteral(" 秒，请优化算法或减少循环次数");
					result = AIToolResult.Fail(defaultInterpolatedStringHandler5.ToStringAndClear());
				}
				catch (CompilationErrorException ex2)
				{
					CompilationErrorException ex3 = ex2;
					compilationErrorException_0 = ex3;
					string_2 = string.Join("\n", compilationErrorException_0.Diagnostics.Where(delegate(Diagnostic diagnostic_0)
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Invalid comparison between Unknown and I4
						//IL_000a: Unknown result type (might be due to invalid IL or missing references)
						//IL_0010: Invalid comparison between Unknown and I4
						return (int)diagnostic_0.Severity == 3 || (int)diagnostic_0.Severity == 2;
					}).Select(delegate(Diagnostic diagnostic_0)
					{
						//IL_0006: Unknown result type (might be due to invalid IL or missing references)
						//IL_000b: Unknown result type (might be due to invalid IL or missing references)
						//IL_000e: Unknown result type (might be due to invalid IL or missing references)
						//IL_0013: Unknown result type (might be due to invalid IL or missing references)
						//IL_0054: Unknown result type (might be due to invalid IL or missing references)
						FileLinePositionSpan lineSpan = diagnostic_0.Location.GetLineSpan();
						LinePosition startLinePosition = ((FileLinePositionSpan)lineSpan).StartLinePosition;
						int value = ((LinePosition)startLinePosition).Line + 1;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(6, 3);
						defaultInterpolatedStringHandler8.AppendLiteral("[第");
						defaultInterpolatedStringHandler8.AppendFormatted(value);
						defaultInterpolatedStringHandler8.AppendLiteral("行 ");
						defaultInterpolatedStringHandler8.AppendFormatted<DiagnosticSeverity>(diagnostic_0.Severity);
						defaultInterpolatedStringHandler8.AppendLiteral("] ");
						defaultInterpolatedStringHandler8.AppendFormatted(diagnostic_0.GetMessage((IFormatProvider)null));
						return defaultInterpolatedStringHandler8.ToStringAndClear();
					}));
					Logger.Warning("[ExecuteCode] 编译错误:\n" + string_2);
					result = AIToolResult.Fail("[编译错误]\n" + string_2 + "\n请修复上述错误后重试。");
				}
				catch (KeyNotFoundException ex4)
				{
					keyNotFoundException_0 = ex4;
					Logger.Error("[ExecuteCode] 字典访问错误: " + keyNotFoundException_0.Message);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(203, 3);
					defaultInterpolatedStringHandler6.AppendLiteral("[运行时异常] ");
					defaultInterpolatedStringHandler6.AppendFormatted(keyNotFoundException_0.GetType().Name);
					defaultInterpolatedStringHandler6.AppendLiteral(": ");
					defaultInterpolatedStringHandler6.AppendFormatted(keyNotFoundException_0.Message);
					defaultInterpolatedStringHandler6.AppendLiteral("\n\n");
					defaultInterpolatedStringHandler6.AppendLiteral("💡 提示：尝试访问字典中不存在的键。常见原因：\n");
					defaultInterpolatedStringHandler6.AppendLiteral("   - tools.Call() 返回的 Data 结构可能不包含预期的键\n");
					defaultInterpolatedStringHandler6.AppendLiteral("   - 建议先检查 result.Success 再访问 Data\n");
					defaultInterpolatedStringHandler6.AppendLiteral("   - 使用 result.Data is Dictionary<string, object> dict && dict.ContainsKey(\"key\") 安全访问\n\n");
					defaultInterpolatedStringHandler6.AppendLiteral("堆栈:\n");
					defaultInterpolatedStringHandler6.AppendFormatted(keyNotFoundException_0.StackTrace);
					result = AIToolResult.Fail(defaultInterpolatedStringHandler6.ToStringAndClear());
				}
				catch (Exception ex5)
				{
					exception_0 = ex5;
					exception_1 = exception_0;
					while (exception_1.InnerException != null)
					{
						exception_1 = exception_1.InnerException;
					}
					Logger.Error("[ExecuteCode] 运行时异常: " + exception_0.GetType().Name + ": " + exception_1.Message);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(15, 3);
					defaultInterpolatedStringHandler7.AppendLiteral("[运行时异常] ");
					defaultInterpolatedStringHandler7.AppendFormatted(exception_1.GetType().Name);
					defaultInterpolatedStringHandler7.AppendLiteral(": ");
					defaultInterpolatedStringHandler7.AppendFormatted(exception_1.Message);
					defaultInterpolatedStringHandler7.AppendLiteral("\n");
					defaultInterpolatedStringHandler7.AppendLiteral("堆栈:\n");
					defaultInterpolatedStringHandler7.AppendFormatted(exception_1.StackTrace);
					result = AIToolResult.Fail(defaultInterpolatedStringHandler7.ToStringAndClear());
				}
			}
			finally
			{
				if (num < 0 && cancellationTokenSource_0 != null)
				{
					((IDisposable)cancellationTokenSource_0).Dispose();
				}
			}
			goto IL_06eb;
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public const int MaxCodeLength = 50000;

	public const int DefaultTimeoutSeconds = 60;

	public const int MaxTimeoutSeconds = 120;

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class652))]
	public static Task<AIToolResult> ExecuteAsync(string code, CodeExecutionGlobals globals, int timeoutSeconds = 60)
	{
		Class652 stateMachine = new Class652();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.string_0 = code;
		stateMachine.codeExecutionGlobals_0 = globals;
		stateMachine.int_1 = timeoutSeconds;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private static object? smethod_0(object? object_0)
	{
		if (object_0 == null)
		{
			return null;
		}
		if (object_0 is string || object_0 is int || object_0 is long || object_0 is double || object_0 is float || object_0 is bool || object_0 is decimal)
		{
			return object_0;
		}
		ElementId val = (ElementId)((object_0 is ElementId) ? object_0 : null);
		if (val != null)
		{
			return (int)val.Value;
		}
		Element val2 = (Element)((object_0 is Element) ? object_0 : null);
		if (val2 != null)
		{
			int gparam_ = (int)val2.Id.Value;
			string name = val2.Name;
			Category category = val2.Category;
			return new Class1<int, string, string>(gparam_, name, (category != null) ? category.Name : null);
		}
		if (object_0 is IEnumerable enumerable && !(object_0 is string))
		{
			List<object> list = new List<object>();
			foreach (object item in enumerable)
			{
				list.Add(smethod_0(item));
			}
			return new Class2<int, List<object>>(list.Count, list);
		}
		if (object_0 is IDictionary dictionary)
		{
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			{
				IDictionaryEnumerator dictionaryEnumerator = dictionary.GetEnumerator();
				try
				{
					object obj;
					DictionaryEntry dictionaryEntry;
					for (; dictionaryEnumerator.MoveNext(); dictionary2[(string)obj] = smethod_0(dictionaryEntry.Value))
					{
						dictionaryEntry = (DictionaryEntry)dictionaryEnumerator.Current;
						object key = dictionaryEntry.Key;
						if (key == null)
						{
							obj = null;
						}
						else
						{
							obj = key.ToString();
							if (obj != null)
							{
								continue;
							}
						}
						obj = "";
					}
				}
				finally
				{
					IDisposable disposable = dictionaryEnumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			return dictionary2;
		}
		string name2 = object_0.GetType().Name;
		if (name2.Contains("AnonymousType") || name2.Contains("f__Anonymous"))
		{
			return object_0;
		}
		return object_0.ToString();
	}
}
