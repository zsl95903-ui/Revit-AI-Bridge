using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using ns7;

namespace RevitAi.Core.Infrastructure;

public static class RoadProjectRepository
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct47 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<bool>> asyncTaskMethodBuilder_0;

		public string string_0;

		public IReadOnlyList<RoadProject> ireadOnlyList_0;

		private ILogger ilogger_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			if (num != 0)
			{
				ilogger_0 = ServiceProvider.GetLogger();
			}
			Result<bool> result;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					string directoryName = Path.GetDirectoryName(string_0);
					if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
					{
						Directory.CreateDirectory(directoryName);
					}
					string contents = JsonSerializer.Serialize(ireadOnlyList_0, jsonSerializerOptions_0);
					awaiter = File.WriteAllTextAsync(string_0, contents, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
				}
				awaiter.GetResult();
				ILogger obj = ilogger_0;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[RoadProjectRepository] 已导出 ");
				defaultInterpolatedStringHandler.AppendFormatted(ireadOnlyList_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个项目 -> ");
				defaultInterpolatedStringHandler.AppendFormatted(string_0);
				obj.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				result = Result<bool>.Success(true);
			}
			catch (Exception ex)
			{
				ilogger_0.Error("[RoadProjectRepository] 导出所有项目失败: " + string_0, ex);
				result = Result<bool>.Failure("导出失败: " + ex.Message);
			}
			int_0 = -2;
			ilogger_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct48 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<bool>> asyncTaskMethodBuilder_0;

		public string string_0;

		public RoadProject roadProject_0;

		private ILogger ilogger_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			if (num != 0)
			{
				ilogger_0 = ServiceProvider.GetLogger();
			}
			Result<bool> result;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					string directoryName = Path.GetDirectoryName(string_0);
					if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
					{
						Directory.CreateDirectory(directoryName);
					}
					string contents = JsonSerializer.Serialize<RoadProject>(roadProject_0, jsonSerializerOptions_0);
					awaiter = File.WriteAllTextAsync(string_0, contents, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
				}
				awaiter.GetResult();
				ilogger_0.Info("[RoadProjectRepository] 已导出项目: " + roadProject_0.Name + " -> " + string_0);
				result = Result<bool>.Success(true);
			}
			catch (Exception ex)
			{
				ilogger_0.Error("[RoadProjectRepository] 导出项目失败: " + string_0, ex);
				result = Result<bool>.Failure("导出失败: " + ex.Message);
			}
			int_0 = -2;
			ilogger_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct49 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<RoadProject>>> asyncTaskMethodBuilder_0;

		public string string_0;

		private ILogger ilogger_0;

		private TaskAwaiter<string> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			if (num != 0)
			{
				ilogger_0 = ServiceProvider.GetLogger();
			}
			Result<List<RoadProject>> result;
			try
			{
				TaskAwaiter<string> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_00ac;
				}
				if (File.Exists(string_0))
				{
					awaiter = File.ReadAllTextAsync(string_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00ac;
				}
				result = Result<List<RoadProject>>.Failure("文件不存在: " + string_0);
				goto end_IL_0016;
				IL_00ac:
				string result2 = awaiter.GetResult();
				result2 = result2.Trim();
				if (result2.StartsWith("["))
				{
					List<RoadProject> list = JsonSerializer.Deserialize<List<RoadProject>>(result2, jsonSerializerOptions_0);
					if (list != null && list.Count != 0)
					{
						ILogger obj = ilogger_0;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[RoadProjectRepository] 已导入 ");
						defaultInterpolatedStringHandler.AppendFormatted(list.Count);
						defaultInterpolatedStringHandler.AppendLiteral(" 个项目 <- ");
						defaultInterpolatedStringHandler.AppendFormatted(string_0);
						obj.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						result = Result<List<RoadProject>>.Success(list);
					}
					else
					{
						result = Result<List<RoadProject>>.Failure("反序列化失败：项目数据为空");
					}
				}
				else
				{
					RoadProject val = JsonSerializer.Deserialize<RoadProject>(result2, jsonSerializerOptions_0);
					if (val == null)
					{
						result = Result<List<RoadProject>>.Failure("反序列化失败：项目数据为空");
					}
					else
					{
						ilogger_0.Info("[RoadProjectRepository] 文件包含单个项目，已转换 <- " + string_0);
						result = Result<List<RoadProject>>.Success(new List<RoadProject> { val });
					}
				}
				end_IL_0016:;
			}
			catch (Exception ex)
			{
				ilogger_0.Error("[RoadProjectRepository] 导入项目失败: " + string_0, ex);
				result = Result<List<RoadProject>>.Failure("导入失败: " + ex.Message);
			}
			int_0 = -2;
			ilogger_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct50 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<RoadProject>> asyncTaskMethodBuilder_0;

		public string string_0;

		private ILogger ilogger_0;

		private TaskAwaiter<string> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			if (num != 0)
			{
				ilogger_0 = ServiceProvider.GetLogger();
			}
			Result<RoadProject> result;
			try
			{
				TaskAwaiter<string> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_00a9;
				}
				if (File.Exists(string_0))
				{
					awaiter = File.ReadAllTextAsync(string_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00a9;
				}
				result = Result<RoadProject>.Failure("文件不存在: " + string_0);
				goto end_IL_0016;
				IL_00a9:
				RoadProject val = JsonSerializer.Deserialize<RoadProject>(awaiter.GetResult(), jsonSerializerOptions_0);
				if (val == null)
				{
					result = Result<RoadProject>.Failure("反序列化失败：项目数据为空");
				}
				else
				{
					ilogger_0.Info("[RoadProjectRepository] 已导入项目: " + val.Name + " <- " + string_0);
					result = Result<RoadProject>.Success(val);
				}
				end_IL_0016:;
			}
			catch (Exception ex)
			{
				ilogger_0.Error("[RoadProjectRepository] 导入项目失败: " + string_0, ex);
				result = Result<RoadProject>.Failure("导入失败: " + ex.Message);
			}
			int_0 = -2;
			ilogger_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private static readonly JsonSerializerOptions jsonSerializerOptions_0 = new JsonSerializerOptions
	{
		WriteIndented = true,
		PropertyNameCaseInsensitive = true,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};

	[AsyncStateMachine(typeof(Struct48))]
	public static Task<Result<bool>> ExportProjectAsync(RoadProject project, string filePath)
	{
		Struct48 stateMachine = default(Struct48);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<bool>>.Create();
		stateMachine.roadProject_0 = project;
		stateMachine.string_0 = filePath;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct50))]
	public static Task<Result<RoadProject>> ImportProjectAsync(string filePath)
	{
		Struct50 stateMachine = default(Struct50);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<RoadProject>>.Create();
		stateMachine.string_0 = filePath;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct47))]
	public static Task<Result<bool>> ExportAllProjectsAsync(IReadOnlyList<RoadProject> projects, string filePath)
	{
		Struct47 stateMachine = default(Struct47);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<bool>>.Create();
		stateMachine.ireadOnlyList_0 = projects;
		stateMachine.string_0 = filePath;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct49))]
	public static Task<Result<List<RoadProject>>> ImportMultipleProjectsAsync(string filePath)
	{
		Struct49 stateMachine = default(Struct49);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<RoadProject>>>.Create();
		stateMachine.string_0 = filePath;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
