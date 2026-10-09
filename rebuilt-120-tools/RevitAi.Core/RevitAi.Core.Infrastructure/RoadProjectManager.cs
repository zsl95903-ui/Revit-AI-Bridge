using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using ns7;

namespace RevitAi.Core.Infrastructure;

public sealed class RoadProjectManager : IRoadProjectManager
{
	[CompilerGenerated]
	public sealed class Class76
	{
		public string string_0;

		internal bool method_0(RoadProject roadProject_0)
		{
			return string.Equals(roadProject_0.Name, string_0, StringComparison.OrdinalIgnoreCase);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct43 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<bool>> asyncTaskMethodBuilder_0;

		public RoadProjectManager roadProjectManager_0;

		public string string_0;

		private TaskAwaiter<Result<bool>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			RoadProjectManager roadProjectManager = roadProjectManager_0;
			Result<bool> result;
			TaskAwaiter<Result<bool>> awaiter;
			if (num != 0)
			{
				IReadOnlyList<RoadProject> allProjects = roadProjectManager.GetAllProjects();
				if (allProjects.Count == 0)
				{
					result = Result<bool>.Failure("没有可导出的项目");
					goto IL_0096;
				}
				awaiter = RoadProjectRepository.ExportAllProjectsAsync(allProjects, string_0).GetAwaiter();
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
				taskAwaiter_0 = default(TaskAwaiter<Result<bool>>);
				num = -1;
				int_0 = -1;
			}
			result = awaiter.GetResult();
			goto IL_0096;
			IL_0096:
			int_0 = -2;
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
	public struct Struct44 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<bool>> asyncTaskMethodBuilder_0;

		public RoadProjectManager roadProjectManager_0;

		public Guid guid_0;

		public string string_0;

		private TaskAwaiter<Result<bool>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			RoadProjectManager roadProjectManager = roadProjectManager_0;
			Result<bool> result;
			TaskAwaiter<Result<bool>> awaiter;
			if (num != 0)
			{
				if (!roadProjectManager.dictionary_0.TryGetValue(guid_0, out RoadProject value))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
					defaultInterpolatedStringHandler.AppendLiteral("项目 ID ");
					defaultInterpolatedStringHandler.AppendFormatted(guid_0);
					defaultInterpolatedStringHandler.AppendLiteral(" 不存在");
					result = Result<bool>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
					goto IL_00d5;
				}
				awaiter = RoadProjectRepository.ExportProjectAsync(value, string_0).GetAwaiter();
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
				taskAwaiter_0 = default(TaskAwaiter<Result<bool>>);
				num = -1;
				int_0 = -1;
			}
			result = awaiter.GetResult();
			goto IL_00d5;
			IL_00d5:
			int_0 = -2;
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
	public struct Struct45 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<RoadProject>>> asyncTaskMethodBuilder_0;

		public string string_0;

		public RoadProjectManager roadProjectManager_0;

		private TaskAwaiter<Result<List<RoadProject>>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Invalid comparison between Unknown and I4
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Expected O, but got Unknown
			int num = int_0;
			RoadProjectManager roadProjectManager = roadProjectManager_0;
			TaskAwaiter<Result<List<RoadProject>>> awaiter;
			if (num != 0)
			{
				awaiter = RoadProjectRepository.ImportMultipleProjectsAsync(string_0).GetAwaiter();
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
				taskAwaiter_0 = default(TaskAwaiter<Result<List<RoadProject>>>);
				num = -1;
				int_0 = -1;
			}
			Result<List<RoadProject>> result = awaiter.GetResult();
			Result<List<RoadProject>> result2;
			if (result.IsSuccess && result.Value != null)
			{
				List<RoadProject> value = result.Value;
				List<RoadProject> list = new List<RoadProject>();
				List<RoadProject>.Enumerator enumerator = value.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						RoadProject current = enumerator.Current;
						if (current != null)
						{
							if ((int)current.DataSource == 1 && current.StationElevationData != null && current.StationElevationData.Count > 0 && (current.Centerline3DPoints == null || current.Centerline3DPoints.Count != current.StationElevationData.Count))
							{
								current.Centerline3DPoints = current.StationElevationData;
								ILogger ilogger_ = roadProjectManager.ilogger_0;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(62, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[RoadProjectManager] 导入桩号高程表项目 ");
								defaultInterpolatedStringHandler.AppendFormatted(current.Name);
								defaultInterpolatedStringHandler.AppendLiteral(": 已同步 ");
								defaultInterpolatedStringHandler.AppendFormatted(current.StationElevationData.Count);
								defaultInterpolatedStringHandler.AppendLiteral(" 个三维点到 Centerline3DPoints");
								ilogger_.Info(defaultInterpolatedStringHandler.ToStringAndClear());
							}
							string name = current.Name;
							int num2 = 1;
							while (roadProjectManager.GetProjectByName(current.Name) != null)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
								defaultInterpolatedStringHandler2.AppendFormatted(name);
								defaultInterpolatedStringHandler2.AppendLiteral(" (");
								defaultInterpolatedStringHandler2.AppendFormatted(num2);
								defaultInterpolatedStringHandler2.AppendLiteral(")");
								current.Name = defaultInterpolatedStringHandler2.ToStringAndClear();
								num2++;
							}
							roadProjectManager.dictionary_0[current.Id] = current;
							list.Add(current);
							ILogger ilogger_2 = roadProjectManager.ilogger_0;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(31, 2);
							defaultInterpolatedStringHandler3.AppendLiteral("[RoadProjectManager] 已导入项目: ");
							defaultInterpolatedStringHandler3.AppendFormatted(current.Name);
							defaultInterpolatedStringHandler3.AppendLiteral(" (");
							defaultInterpolatedStringHandler3.AppendFormatted(current.Id);
							defaultInterpolatedStringHandler3.AppendLiteral(")");
							ilogger_2.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
						}
					}
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
					}
				}
				roadProjectManager.method_0(new RoadProjectChangedEventArgs
				{
					ChangeType = (RoadProjectChangeType)4
				});
				result2 = Result<List<RoadProject>>.Success(list);
			}
			else
			{
				result2 = result;
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result2);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct46 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<RoadProject>> asyncTaskMethodBuilder_0;

		public string string_0;

		public RoadProjectManager roadProjectManager_0;

		private TaskAwaiter<Result<RoadProject>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Invalid comparison between Unknown and I4
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Expected O, but got Unknown
			int num = int_0;
			RoadProjectManager roadProjectManager = roadProjectManager_0;
			TaskAwaiter<Result<RoadProject>> awaiter;
			if (num != 0)
			{
				awaiter = RoadProjectRepository.ImportProjectAsync(string_0).GetAwaiter();
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
				taskAwaiter_0 = default(TaskAwaiter<Result<RoadProject>>);
				num = -1;
				int_0 = -1;
			}
			Result<RoadProject> result = awaiter.GetResult();
			Result<RoadProject> result2;
			if (!result.IsSuccess)
			{
				result2 = result;
			}
			else
			{
				RoadProject value = result.Value;
				if (value == null)
				{
					roadProjectManager.ilogger_0.Error("[RoadProjectManager] 导入的项目为 null", (Exception)null);
					result2 = Result<RoadProject>.Failure("导入的项目为 null");
				}
				else
				{
					if ((int)value.DataSource == 1 && value.StationElevationData != null && value.StationElevationData.Count > 0 && (value.Centerline3DPoints == null || value.Centerline3DPoints.Count != value.StationElevationData.Count))
					{
						value.Centerline3DPoints = value.StationElevationData;
						ILogger ilogger_ = roadProjectManager.ilogger_0;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(61, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[RoadProjectManager] 导入桩号高程表项目: 已同步 ");
						defaultInterpolatedStringHandler.AppendFormatted(value.StationElevationData.Count);
						defaultInterpolatedStringHandler.AppendLiteral(" 个三维点到 Centerline3DPoints");
						ilogger_.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					string name = value.Name;
					int num2 = 1;
					while (roadProjectManager.GetProjectByName(value.Name) != null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
						defaultInterpolatedStringHandler2.AppendFormatted(name);
						defaultInterpolatedStringHandler2.AppendLiteral(" (");
						defaultInterpolatedStringHandler2.AppendFormatted(num2);
						defaultInterpolatedStringHandler2.AppendLiteral(")");
						value.Name = defaultInterpolatedStringHandler2.ToStringAndClear();
						num2++;
					}
					roadProjectManager.dictionary_0[value.Id] = value;
					ILogger ilogger_2 = roadProjectManager.ilogger_0;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(31, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("[RoadProjectManager] 已导入项目: ");
					defaultInterpolatedStringHandler3.AppendFormatted(value.Name);
					defaultInterpolatedStringHandler3.AppendLiteral(" (");
					defaultInterpolatedStringHandler3.AppendFormatted(value.Id);
					defaultInterpolatedStringHandler3.AppendLiteral(")");
					ilogger_2.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
					roadProjectManager.method_0(new RoadProjectChangedEventArgs
					{
						ChangeType = (RoadProjectChangeType)0,
						Project = value
					});
					result2 = Result<RoadProject>.Success(value);
				}
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result2);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly Dictionary<Guid, RoadProject> dictionary_0 = new Dictionary<Guid, RoadProject>();

	private Guid? nullable_0;

	private readonly ILogger ilogger_0;

	[CompilerGenerated]
	private EventHandler<RoadProjectChangedEventArgs>? eventHandler_0;

	public event EventHandler<RoadProjectChangedEventArgs>? ProjectChanged
	{
		[CompilerGenerated]
		add
		{
			EventHandler<RoadProjectChangedEventArgs> eventHandler = eventHandler_0;
			EventHandler<RoadProjectChangedEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler<RoadProjectChangedEventArgs> value2 = (EventHandler<RoadProjectChangedEventArgs>)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref eventHandler_0, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			EventHandler<RoadProjectChangedEventArgs> eventHandler = eventHandler_0;
			EventHandler<RoadProjectChangedEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler<RoadProjectChangedEventArgs> value2 = (EventHandler<RoadProjectChangedEventArgs>)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref eventHandler_0, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	public RoadProjectManager()
	{
		ilogger_0 = ServiceProvider.GetLogger();
	}

	public IReadOnlyList<RoadProject> GetAllProjects()
	{
		return dictionary_0.Values.ToList().AsReadOnly();
	}

	public RoadProject? GetProject(Guid id)
	{
		if (!dictionary_0.TryGetValue(id, out RoadProject value))
		{
			return null;
		}
		return value;
	}

	public RoadProject? GetProjectByName(string name)
	{
		return dictionary_0.Values.FirstOrDefault((RoadProject roadProject_0) => string.Equals(roadProject_0.Name, name, StringComparison.OrdinalIgnoreCase));
	}

	public Result<RoadProject> AddProject(string name, string? description = null)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		if (string.IsNullOrWhiteSpace(name))
		{
			return Result<RoadProject>.Failure("项目名称不能为空");
		}
		if (GetProjectByName(name) != null)
		{
			return Result<RoadProject>.Failure("项目名称 '" + name + "' 已存在");
		}
		RoadProject val = new RoadProject
		{
			Name = name,
			Description = description
		};
		dictionary_0[val.Id] = val;
		ILogger obj = ilogger_0;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 2);
		defaultInterpolatedStringHandler.AppendLiteral("[RoadProjectManager] 已添加项目: ");
		defaultInterpolatedStringHandler.AppendFormatted(val.Name);
		defaultInterpolatedStringHandler.AppendLiteral(" (");
		defaultInterpolatedStringHandler.AppendFormatted(val.Id);
		defaultInterpolatedStringHandler.AppendLiteral(")");
		obj.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		method_0(new RoadProjectChangedEventArgs
		{
			ChangeType = (RoadProjectChangeType)0,
			Project = val
		});
		return Result<RoadProject>.Success(val);
	}

	public Result<bool> UpdateProject(RoadProject project)
	{
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		if (project == null)
		{
			return Result<bool>.Failure("项目不能为空");
		}
		if (!dictionary_0.ContainsKey(project.Id))
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler.AppendLiteral("项目 ID ");
			defaultInterpolatedStringHandler.AppendFormatted(project.Id);
			defaultInterpolatedStringHandler.AppendLiteral(" 不存在");
			return Result<bool>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		project.UpdatedAt = DateTime.Now;
		dictionary_0[project.Id] = project;
		ILogger obj = ilogger_0;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(31, 2);
		defaultInterpolatedStringHandler2.AppendLiteral("[RoadProjectManager] 已更新项目: ");
		defaultInterpolatedStringHandler2.AppendFormatted(project.Name);
		defaultInterpolatedStringHandler2.AppendLiteral(" (");
		defaultInterpolatedStringHandler2.AppendFormatted(project.Id);
		defaultInterpolatedStringHandler2.AppendLiteral(")");
		obj.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
		method_0(new RoadProjectChangedEventArgs
		{
			ChangeType = (RoadProjectChangeType)1,
			Project = project
		});
		return Result<bool>.Success(true);
	}

	public Result<bool> DeleteProject(Guid id)
	{
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		if (!dictionary_0.TryGetValue(id, out RoadProject value))
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler.AppendLiteral("项目 ID ");
			defaultInterpolatedStringHandler.AppendFormatted(id);
			defaultInterpolatedStringHandler.AppendLiteral(" 不存在");
			return Result<bool>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		dictionary_0.Remove(id);
		if (nullable_0 == id)
		{
			nullable_0 = null;
		}
		ILogger obj = ilogger_0;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(31, 2);
		defaultInterpolatedStringHandler2.AppendLiteral("[RoadProjectManager] 已删除项目: ");
		defaultInterpolatedStringHandler2.AppendFormatted(value.Name);
		defaultInterpolatedStringHandler2.AppendLiteral(" (");
		defaultInterpolatedStringHandler2.AppendFormatted(id);
		defaultInterpolatedStringHandler2.AppendLiteral(")");
		obj.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
		method_0(new RoadProjectChangedEventArgs
		{
			ChangeType = (RoadProjectChangeType)2,
			ProjectId = id
		});
		return Result<bool>.Success(true);
	}

	public Result<bool> SetActiveProject(Guid id)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		if (!dictionary_0.ContainsKey(id))
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler.AppendLiteral("项目 ID ");
			defaultInterpolatedStringHandler.AppendFormatted(id);
			defaultInterpolatedStringHandler.AppendLiteral(" 不存在");
			return Result<bool>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		RoadProject val = dictionary_0[id];
		nullable_0 = id;
		ILogger obj = ilogger_0;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(33, 2);
		defaultInterpolatedStringHandler2.AppendLiteral("[RoadProjectManager] 已设置活动项目: ");
		defaultInterpolatedStringHandler2.AppendFormatted(val.Name);
		defaultInterpolatedStringHandler2.AppendLiteral(" (");
		defaultInterpolatedStringHandler2.AppendFormatted(id);
		defaultInterpolatedStringHandler2.AppendLiteral(")");
		obj.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
		method_0(new RoadProjectChangedEventArgs
		{
			ChangeType = (RoadProjectChangeType)3,
			Project = val
		});
		return Result<bool>.Success(true);
	}

	public RoadProject? GetActiveProject()
	{
		if (!nullable_0.HasValue)
		{
			return null;
		}
		return GetProject(nullable_0.Value);
	}

	public void ClearAll()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		int count = dictionary_0.Count;
		dictionary_0.Clear();
		nullable_0 = null;
		ILogger obj = ilogger_0;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[RoadProjectManager] 已清空所有项目 (共 ");
		defaultInterpolatedStringHandler.AppendFormatted(count);
		defaultInterpolatedStringHandler.AppendLiteral(" 个)");
		obj.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		method_0(new RoadProjectChangedEventArgs
		{
			ChangeType = (RoadProjectChangeType)4
		});
	}

	[AsyncStateMachine(typeof(Struct44))]
	public Task<Result<bool>> ExportProjectAsync(Guid projectId, string filePath)
	{
		Struct44 stateMachine = default(Struct44);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<bool>>.Create();
		stateMachine.roadProjectManager_0 = this;
		stateMachine.guid_0 = projectId;
		stateMachine.string_0 = filePath;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct46))]
	public Task<Result<RoadProject>> ImportProjectAsync(string filePath)
	{
		Struct46 stateMachine = default(Struct46);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<RoadProject>>.Create();
		stateMachine.roadProjectManager_0 = this;
		stateMachine.string_0 = filePath;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct43))]
	public Task<Result<bool>> ExportAllProjectsAsync(string filePath)
	{
		Struct43 stateMachine = default(Struct43);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<bool>>.Create();
		stateMachine.roadProjectManager_0 = this;
		stateMachine.string_0 = filePath;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct45))]
	public Task<Result<List<RoadProject>>> ImportMultipleProjectsAsync(string filePath)
	{
		Struct45 stateMachine = default(Struct45);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<RoadProject>>>.Create();
		stateMachine.roadProjectManager_0 = this;
		stateMachine.string_0 = filePath;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private void method_0(RoadProjectChangedEventArgs roadProjectChangedEventArgs_0)
	{
		eventHandler_0?.Invoke(this, roadProjectChangedEventArgs_0);
	}
}
