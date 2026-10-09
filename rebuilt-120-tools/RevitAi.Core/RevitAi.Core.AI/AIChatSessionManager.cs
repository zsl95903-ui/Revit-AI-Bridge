using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using ns7;

namespace RevitAi.Core.AI;

public sealed class AIChatSessionManager
{
	[CompilerGenerated]
	public sealed class Class109
	{
		public AIChatSession aichatSession_0;

		internal bool method_0(SessionIndexItem sessionIndexItem_0)
		{
			return sessionIndexItem_0.SessionId == aichatSession_0.SessionId;
		}
	}

	[CompilerGenerated]
	public sealed class Class110
	{
		public string string_0;

		internal bool method_0(SessionIndexItem sessionIndexItem_0)
		{
			return sessionIndexItem_0.SessionId == string_0;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct175 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<bool> asyncTaskMethodBuilder_0;

		public AIChatSessionManager aichatSessionManager_0;

		public string string_0;

		public CancellationToken cancellationToken_0;

		private bool bool_0;

		private TaskAwaiter<bool> taskAwaiter_0;

		private TaskAwaiter taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AIChatSessionManager aIChatSessionManager = aichatSessionManager_0;
			bool result2;
			try
			{
				TaskAwaiter awaiter;
				TaskAwaiter<bool> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter);
						num = -1;
						int_0 = -1;
						goto IL_0101;
					}
					string text = aIChatSessionManager.method_0(string_0);
					awaiter2 = aIChatSessionManager.ifileStorageService_0.DeleteAsync(text, cancellationToken_0).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<bool>);
					num = -1;
					int_0 = -1;
				}
				bool result = awaiter2.GetResult();
				bool_0 = result;
				if (bool_0)
				{
					awaiter = aIChatSessionManager.method_3(string_0, cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0101;
				}
				goto IL_0122;
				IL_0101:
				awaiter.GetResult();
				Logger.Info("[SessionManager] 会话删除成功: " + string_0);
				goto IL_0122;
				IL_0122:
				result2 = bool_0;
			}
			catch (Exception ex)
			{
				Logger.Error("[SessionManager] 删除会话失败: " + string_0, ex);
				result2 = false;
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
	public struct Struct176 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<List<SessionIndexItem>> asyncTaskMethodBuilder_0;

		public AIChatSessionManager aichatSessionManager_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<List<SessionIndexItem>?> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AIChatSessionManager aIChatSessionManager = aichatSessionManager_0;
			List<SessionIndexItem> result2;
			try
			{
				TaskAwaiter<List<SessionIndexItem>> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_0;
						taskAwaiter_0 = default(TaskAwaiter<List<SessionIndexItem>>);
						num = -1;
						int_0 = -1;
						goto IL_0122;
					}
					awaiter = aIChatSessionManager.method_1(cancellationToken_0).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<List<SessionIndexItem>>);
					num = -1;
					int_0 = -1;
				}
				List<SessionIndexItem> result = awaiter.GetResult();
				if (result == null || result.Count <= 0)
				{
					Logger.Warning("[SessionManager] 索引文件不存在，正在重建索引...");
					awaiter = aIChatSessionManager.method_4(cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0122;
				}
				result2 = result.OrderByDescending((SessionIndexItem sessionIndexItem_0) => sessionIndexItem_0.UpdatedAt).ToList();
				goto end_IL_000f;
				IL_0122:
				result2 = awaiter.GetResult();
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[SessionManager] 获取会话列表失败", ex);
				result2 = new List<SessionIndexItem>();
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
	public struct Struct177 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIChatSession> asyncTaskMethodBuilder_0;

		public string string_0;

		public AIChatSessionManager aichatSessionManager_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<AIChatSession?> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AIChatSessionManager aIChatSessionManager = aichatSessionManager_0;
			if (num != 0 && string.IsNullOrWhiteSpace(string_0))
			{
				throw new ArgumentException("会话 ID 不能为空", "sessionId");
			}
			AIChatSession result;
			try
			{
				TaskAwaiter<AIChatSession> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<AIChatSession>);
					num = -1;
					int_0 = -1;
					goto IL_00d3;
				}
				string text = aIChatSessionManager.method_0(string_0);
				if (aIChatSessionManager.ifileStorageService_0.FileExists(text))
				{
					awaiter = aIChatSessionManager.ifileStorageService_0.LoadAsync<AIChatSession>(text, cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00d3;
				}
				Logger.Warning("[SessionManager] 会话文件不存在: " + text);
				result = null;
				goto end_IL_0039;
				IL_00d3:
				result = awaiter.GetResult();
				end_IL_0039:;
			}
			catch (Exception ex)
			{
				Logger.Error("[SessionManager] 加载会话失败: " + string_0, ex);
				result = null;
			}
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
	public struct Struct178 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<List<SessionIndexItem>> asyncTaskMethodBuilder_0;

		public AIChatSessionManager aichatSessionManager_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<List<SessionIndexItem>?> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AIChatSessionManager aIChatSessionManager = aichatSessionManager_0;
			List<SessionIndexItem> result;
			try
			{
				TaskAwaiter<List<SessionIndexItem>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<List<SessionIndexItem>>);
					num = -1;
					int_0 = -1;
					goto IL_008e;
				}
				if (aIChatSessionManager.ifileStorageService_0.FileExists(aIChatSessionManager.string_1))
				{
					awaiter = aIChatSessionManager.ifileStorageService_0.LoadAsync<List<SessionIndexItem>>(aIChatSessionManager.string_1, cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_008e;
				}
				result = null;
				goto end_IL_000f;
				IL_008e:
				result = awaiter.GetResult();
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[SessionManager] 加载索引失败", ex);
				result = null;
			}
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
	public struct Struct179 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<List<SessionIndexItem>> asyncTaskMethodBuilder_0;

		public AIChatSessionManager aichatSessionManager_0;

		public CancellationToken cancellationToken_0;

		private List<SessionIndexItem> list_0;

		private TaskAwaiter<List<string>> taskAwaiter_0;

		private List<string>.Enumerator enumerator_0;

		private string string_0;

		private TaskAwaiter<AIChatSession?> taskAwaiter_1;

		private TaskAwaiter<bool> taskAwaiter_2;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AIChatSessionManager aIChatSessionManager = aichatSessionManager_0;
			List<SessionIndexItem> result3;
			try
			{
				TaskAwaiter<List<string>> awaiter2;
				TaskAwaiter<bool> awaiter;
				List<string> result2;
				switch (num)
				{
				default:
					awaiter2 = aIChatSessionManager.ifileStorageService_0.ListAsync(aIChatSessionManager.string_0, "session_*.json", cancellationToken_0).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_0093;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<List<string>>);
					num = -1;
					int_0 = -1;
					goto IL_0093;
				case 1:
					try
					{
						if (num == 1)
						{
							goto IL_00bd;
						}
						goto IL_01e5;
						IL_01e5:
						if (enumerator_0.MoveNext())
						{
							string_0 = enumerator_0.Current;
							goto IL_00bd;
						}
						goto end_IL_00b3;
						IL_00bd:
						try
						{
							TaskAwaiter<AIChatSession> awaiter3;
							if (num != 1)
							{
								awaiter3 = aIChatSessionManager.ifileStorageService_0.LoadAsync<AIChatSession>(string_0, cancellationToken_0).GetAwaiter();
								if (!awaiter3.IsCompleted)
								{
									num = 1;
									int_0 = 1;
									taskAwaiter_1 = awaiter3;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
									return;
								}
							}
							else
							{
								awaiter3 = taskAwaiter_1;
								taskAwaiter_1 = default(TaskAwaiter<AIChatSession>);
								num = -1;
								int_0 = -1;
							}
							AIChatSession result = awaiter3.GetResult();
							if (result != null)
							{
								list_0.Add(new SessionIndexItem
								{
									SessionId = result.SessionId,
									Title = result.Title,
									CreatedAt = result.CreatedAt,
									UpdatedAt = result.UpdatedAt,
									MessageCount = result.Messages.Count,
									RoundCount = result.RoundCount,
									Tags = result.Tags,
									Preview = aIChatSessionManager.method_5(result)
								});
							}
						}
						catch (Exception ex)
						{
							Logger.Warning("[SessionManager] 跳过无效的会话文件: " + string_0 + " - " + ex.Message);
						}
						string_0 = null;
						goto IL_01e5;
						end_IL_00b3:;
					}
					finally
					{
						if (num < 0)
						{
							((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
						}
					}
					enumerator_0 = default(List<string>.Enumerator);
					awaiter = aIChatSessionManager.ifileStorageService_0.SaveAsync<List<SessionIndexItem>>(aIChatSessionManager.string_1, list_0, cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 2;
						int_0 = 2;
						taskAwaiter_2 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					break;
				case 2:
					{
						awaiter = taskAwaiter_2;
						taskAwaiter_2 = default(TaskAwaiter<bool>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0093:
					result2 = awaiter2.GetResult();
					list_0 = new List<SessionIndexItem>();
					enumerator_0 = result2.GetEnumerator();
					goto case 1;
				}
				awaiter.GetResult();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[SessionManager] 索引重建完成，共 ");
				defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个会话");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				result3 = list_0.OrderByDescending((SessionIndexItem sessionIndexItem_0) => sessionIndexItem_0.UpdatedAt).ToList();
			}
			catch (Exception ex2)
			{
				Logger.Error("[SessionManager] 重建索引失败", ex2);
				result3 = new List<SessionIndexItem>();
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result3);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct180 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public string string_0;

		public AIChatSessionManager aichatSessionManager_0;

		public CancellationToken cancellationToken_0;

		private Class110 class110_0;

		private TaskAwaiter<List<SessionIndexItem>?> taskAwaiter_0;

		private TaskAwaiter<bool> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AIChatSessionManager aIChatSessionManager = aichatSessionManager_0;
			if ((uint)num > 1u)
			{
				this.class110_0 = new Class110();
				this.class110_0.string_0 = string_0;
			}
			try
			{
				TaskAwaiter<bool> awaiter;
				TaskAwaiter<List<SessionIndexItem>> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<bool>);
						num = -1;
						int_0 = -1;
						goto IL_0131;
					}
					awaiter2 = aIChatSessionManager.method_1(cancellationToken_0).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<List<SessionIndexItem>>);
					num = -1;
					int_0 = -1;
				}
				List<SessionIndexItem> result = awaiter2.GetResult();
				if (result != null)
				{
				var class110_0 = this.class110_0;
					SessionIndexItem sessionIndexItem = result.FirstOrDefault((SessionIndexItem sessionIndexItem_0) => sessionIndexItem_0.SessionId == class110_0.string_0);
					if (sessionIndexItem != null)
					{
						result.Remove(sessionIndexItem);
						awaiter = aIChatSessionManager.ifileStorageService_0.SaveAsync<List<SessionIndexItem>>(aIChatSessionManager.string_1, result, cancellationToken_0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0131;
					}
				}
				goto end_IL_002f;
				IL_0131:
				awaiter.GetResult();
				end_IL_002f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[SessionManager] 从索引移除失败", ex);
			}
			int_0 = -2;
			class110_0 = null;
			asyncTaskMethodBuilder_0.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct181 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<bool> asyncTaskMethodBuilder_0;

		public AIChatSession aichatSession_0;

		public AIChatSessionManager aichatSessionManager_0;

		public CancellationToken cancellationToken_0;

		private string string_0;

		private TaskAwaiter<bool> taskAwaiter_0;

		private TaskAwaiter taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AIChatSessionManager aIChatSessionManager = aichatSessionManager_0;
			if ((uint)num > 1u && aichatSession_0 == null)
			{
				throw new ArgumentNullException("session");
			}
			bool result;
			try
			{
				TaskAwaiter awaiter;
				TaskAwaiter<bool> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter);
						num = -1;
						int_0 = -1;
						goto IL_0159;
					}
					aichatSession_0.UpdatedAt = DateTime.Now;
					string_0 = aIChatSessionManager.method_0(aichatSession_0.SessionId);
					awaiter2 = aIChatSessionManager.ifileStorageService_0.SaveAsync<AIChatSession>(string_0, aichatSession_0, cancellationToken_0).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<bool>);
					num = -1;
					int_0 = -1;
				}
				if (awaiter2.GetResult())
				{
					awaiter = aIChatSessionManager.method_2(aichatSession_0, cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0159;
				}
				Logger.Error("[SessionManager] 保存会话文件失败: " + string_0);
				result = false;
				goto end_IL_002b;
				IL_0159:
				awaiter.GetResult();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[SessionManager] 会话保存成功: ");
				defaultInterpolatedStringHandler.AppendFormatted(aichatSession_0.Title);
				defaultInterpolatedStringHandler.AppendLiteral(" (");
				defaultInterpolatedStringHandler.AppendFormatted(aichatSession_0.SessionId);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				result = true;
				end_IL_002b:;
			}
			catch (Exception ex)
			{
				Logger.Error("[SessionManager] 保存会话失败: " + aichatSession_0.SessionId, ex);
				result = false;
			}
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
	public struct Struct182 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public AIChatSession aichatSession_0;

		public AIChatSessionManager aichatSessionManager_0;

		public CancellationToken cancellationToken_0;

		private Class109 class109_0;

		private TaskAwaiter<List<SessionIndexItem>?> taskAwaiter_0;

		private TaskAwaiter<bool> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AIChatSessionManager aIChatSessionManager = aichatSessionManager_0;
			if ((uint)num > 1u)
			{
				this.class109_0 = new Class109();
				this.class109_0.aichatSession_0 = aichatSession_0;
			}
			try
			{
				TaskAwaiter<bool> awaiter;
				TaskAwaiter<List<SessionIndexItem>> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<bool>);
						num = -1;
						int_0 = -1;
						goto IL_027d;
					}
					awaiter2 = aIChatSessionManager.method_1(cancellationToken_0).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<List<SessionIndexItem>>);
					num = -1;
					int_0 = -1;
				}
				List<SessionIndexItem> list = awaiter2.GetResult() ?? new List<SessionIndexItem>();
				var class109_0 = this.class109_0;
				SessionIndexItem sessionIndexItem = list.FirstOrDefault((SessionIndexItem sessionIndexItem_0) => sessionIndexItem_0.SessionId == class109_0.aichatSession_0.SessionId);
				if (sessionIndexItem != null)
				{
					sessionIndexItem.Title = class109_0.aichatSession_0.Title;
					sessionIndexItem.UpdatedAt = class109_0.aichatSession_0.UpdatedAt;
					sessionIndexItem.MessageCount = class109_0.aichatSession_0.Messages.Count;
					sessionIndexItem.RoundCount = class109_0.aichatSession_0.RoundCount;
					sessionIndexItem.Tags = class109_0.aichatSession_0.Tags;
					sessionIndexItem.Preview = aIChatSessionManager.method_5(class109_0.aichatSession_0);
				}
				else
				{
					list.Add(new SessionIndexItem
					{
						SessionId = class109_0.aichatSession_0.SessionId,
						Title = class109_0.aichatSession_0.Title,
						CreatedAt = class109_0.aichatSession_0.CreatedAt,
						UpdatedAt = class109_0.aichatSession_0.UpdatedAt,
						MessageCount = class109_0.aichatSession_0.Messages.Count,
						RoundCount = class109_0.aichatSession_0.RoundCount,
						Tags = class109_0.aichatSession_0.Tags,
						Preview = aIChatSessionManager.method_5(class109_0.aichatSession_0)
					});
				}
				awaiter = aIChatSessionManager.ifileStorageService_0.SaveAsync<List<SessionIndexItem>>(aIChatSessionManager.string_1, list, cancellationToken_0).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
				goto IL_027d;
				IL_027d:
				awaiter.GetResult();
			}
			catch (Exception ex)
			{
				Logger.Error("[SessionManager] 更新索引失败", ex);
			}
			int_0 = -2;
			class109_0 = null;
			asyncTaskMethodBuilder_0.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly IFileStorageService ifileStorageService_0;

	private readonly string string_0;

	private readonly string string_1;

	private const int int_0 = 10;

	private const string string_2 = "session_index.json";

	public AIChatSessionManager(IFileStorageService storage, string? sessionsDirectory = null)
	{
		ifileStorageService_0 = storage ?? throw new ArgumentNullException("storage");
		string_0 = ((!string.IsNullOrEmpty(sessionsDirectory)) ? sessionsDirectory : Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "AIChat"), "sessions"));
		string_1 = Path.Combine(string_0, "session_index.json");
		ifileStorageService_0.EnsureDirectoryExists(string_0);
	}

	[AsyncStateMachine(typeof(Struct181))]
	public Task<bool> SaveSessionAsync(AIChatSession session, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct181 stateMachine = default(Struct181);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<bool>.Create();
		stateMachine.aichatSessionManager_0 = this;
		stateMachine.aichatSession_0 = session;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct177))]
	public Task<AIChatSession?> LoadSessionAsync(string sessionId, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct177 stateMachine = default(Struct177);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIChatSession>.Create();
		stateMachine.aichatSessionManager_0 = this;
		stateMachine.string_0 = sessionId;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct176))]
	public Task<List<SessionIndexItem>> GetSessionListAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct176 stateMachine = default(Struct176);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<List<SessionIndexItem>>.Create();
		stateMachine.aichatSessionManager_0 = this;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct175))]
	public Task<bool> DeleteSessionAsync(string sessionId, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct175 stateMachine = default(Struct175);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<bool>.Create();
		stateMachine.aichatSessionManager_0 = this;
		stateMachine.string_0 = sessionId;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public bool ShouldAutoSave(AIChatSession session)
	{
		if (session == null)
		{
			return false;
		}
		return session.RoundCount >= 10;
	}

	private string method_0(string string_3)
	{
		return Path.Combine(string_0, "session_" + string_3 + ".json");
	}

	[AsyncStateMachine(typeof(Struct178))]
	private Task<List<SessionIndexItem>?> method_1(CancellationToken cancellationToken_0)
	{
		Struct178 stateMachine = default(Struct178);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<List<SessionIndexItem>>.Create();
		stateMachine.aichatSessionManager_0 = this;
		stateMachine.cancellationToken_0 = cancellationToken_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct182))]
	private Task method_2(AIChatSession aichatSession_0, CancellationToken cancellationToken_0)
	{
		Struct182 stateMachine = default(Struct182);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.aichatSessionManager_0 = this;
		stateMachine.aichatSession_0 = aichatSession_0;
		stateMachine.cancellationToken_0 = cancellationToken_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct180))]
	private Task method_3(string string_3, CancellationToken cancellationToken_0)
	{
		Struct180 stateMachine = default(Struct180);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.aichatSessionManager_0 = this;
		stateMachine.string_0 = string_3;
		stateMachine.cancellationToken_0 = cancellationToken_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct179))]
	private Task<List<SessionIndexItem>> method_4(CancellationToken cancellationToken_0)
	{
		Struct179 stateMachine = default(Struct179);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<List<SessionIndexItem>>.Create();
		stateMachine.aichatSessionManager_0 = this;
		stateMachine.cancellationToken_0 = cancellationToken_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private string method_5(AIChatSession aichatSession_0)
	{
		if (aichatSession_0.Messages != null && aichatSession_0.Messages.Count != 0)
		{
			string text = aichatSession_0.Messages[aichatSession_0.Messages.Count - 1].Content ?? "";
			if (text.Length <= 100)
			{
				return text;
			}
			return text.Substring(0, 100) + "...";
		}
		return "空会话";
	}
}
