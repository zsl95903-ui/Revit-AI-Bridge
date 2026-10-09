using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Core.Authentication;
using RevitAi.Core.Feedback.Models;
using Newtonsoft.Json;
using ns7;

namespace RevitAi.Core.Feedback;

public class FeedbackService
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct51 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public FeedbackService feedbackService_0;

		public Guid guid_0;

		private TaskAwaiter<Result> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FeedbackService feedbackService = feedbackService_0;
			Result result2;
			try
			{
				TaskAwaiter<Result> awaiter;
				if (num != 0)
				{
					awaiter = feedbackService.isupabaseClient_0.DeleteFeedbackAsync(guid_0).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
				}
				Result result = awaiter.GetResult();
				if (result.IsSuccess)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[FeedbackService] 删除反馈成功: ");
					defaultInterpolatedStringHandler.AppendFormatted(guid_0);
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				result2 = result;
			}
			catch (Exception ex)
			{
				Logger.Error("[FeedbackService] 删除反馈异常", ex);
				result2 = Result.Failure("删除反馈异常：" + ex.Message);
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
	public struct Struct52 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<FeedbackInfo>>> asyncTaskMethodBuilder_0;

		public FeedbackService feedbackService_0;

		private TaskAwaiter<Result<List<FeedbackInfo>>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FeedbackService feedbackService = feedbackService_0;
			Result<List<FeedbackInfo>> result;
			try
			{
				TaskAwaiter<Result<List<FeedbackInfo>>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<List<FeedbackInfo>>>);
					num = -1;
					int_0 = -1;
					goto IL_00ca;
				}
				IDeviceInfo currentDevice = feedbackService.iauthManager_0.CurrentDevice;
				if (currentDevice != null)
				{
					IUserIdentity currentUser = feedbackService.iauthManager_0.CurrentUser;
					awaiter = feedbackService.isupabaseClient_0.GetFeedbackListAsync(currentDevice.DeviceId ?? string.Empty, (currentUser != null) ? new Guid?(currentUser.UserId) : ((Guid?)null)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00ca;
				}
				result = Result<List<FeedbackInfo>>.Failure("设备信息不可用");
				goto end_IL_000f;
				IL_00ca:
				result = awaiter.GetResult();
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[FeedbackService] 获取反馈列表异常", ex);
				result = Result<List<FeedbackInfo>>.Failure("获取反馈列表异常：" + ex.Message);
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
	public struct Struct53 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<int>> asyncTaskMethodBuilder_0;

		public FeedbackService feedbackService_0;

		private TaskAwaiter<Result<int>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FeedbackService feedbackService = feedbackService_0;
			Result<int> result;
			try
			{
				TaskAwaiter<Result<int>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<int>>);
					num = -1;
					int_0 = -1;
					goto IL_00c1;
				}
				IDeviceInfo currentDevice = feedbackService.iauthManager_0.CurrentDevice;
				if (currentDevice != null)
				{
					IUserIdentity currentUser = feedbackService.iauthManager_0.CurrentUser;
					awaiter = feedbackService.isupabaseClient_0.GetUnreadReplyCountAsync(currentDevice.DeviceId ?? string.Empty, (currentUser != null) ? new Guid?(currentUser.UserId) : ((Guid?)null)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00c1;
				}
				result = Result<int>.Success(0);
				goto end_IL_000f;
				IL_00c1:
				result = awaiter.GetResult();
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[FeedbackService] 获取未读回复数量异常", ex);
				result = Result<int>.Success(0);
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
	public struct Struct54 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public FeedbackService feedbackService_0;

		private IEnumerable<FeedbackInfo> ienumerable_0;

		private TaskAwaiter<Result<List<FeedbackInfo>>> taskAwaiter_0;

		private IEnumerator<FeedbackInfo> ienumerator_0;

		private TaskAwaiter<Result> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FeedbackService feedbackService = feedbackService_0;
			Result result2;
			try
			{
				TaskAwaiter<Result<List<FeedbackInfo>>> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_00d0;
					}
					awaiter = feedbackService.GetFeedbackListAsync().GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result<List<FeedbackInfo>>>);
					num = -1;
					int_0 = -1;
				}
				Result<List<FeedbackInfo>> result = awaiter.GetResult();
				if (result.IsSuccess && result.Value != null)
				{
					ienumerable_0 = result.Value.Where((FeedbackInfo feedbackInfo_0) => feedbackInfo_0.HasReply && !feedbackInfo_0.IsUserViewed);
					ienumerator_0 = ienumerable_0.GetEnumerator();
					goto IL_00d0;
				}
				result2 = Result.Failure("获取反馈列表失败");
				goto end_IL_000f;
				IL_00d0:
				try
				{
					if (num != 1)
					{
						goto IL_00f5;
					}
					TaskAwaiter<Result> awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
					goto IL_012c;
					IL_012c:
					awaiter2.GetResult();
					goto IL_00f5;
					IL_00f5:
					if (ienumerator_0.MoveNext())
					{
						FeedbackInfo current = ienumerator_0.Current;
						awaiter2 = feedbackService.MarkFeedbackAsReadAsync(current.Id).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_012c;
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
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[FeedbackService] 标记 ");
				defaultInterpolatedStringHandler.AppendFormatted(ienumerable_0.Count());
				defaultInterpolatedStringHandler.AppendLiteral(" 条反馈为已读");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				result2 = Result.Success();
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[FeedbackService] 标记所有已读异常", ex);
				result2 = Result.Failure("标记所有已读异常：" + ex.Message);
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
	public struct Struct55 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public FeedbackService feedbackService_0;

		public Guid guid_0;

		private IDeviceInfo ideviceInfo_0;

		private IUserIdentity iuserIdentity_0;

		private TaskAwaiter<Result> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FeedbackService feedbackService = feedbackService_0;
			Result result;
			try
			{
				TaskAwaiter<Result> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
					goto IL_01b8;
				}
				if (num == 1)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
					goto IL_012c;
				}
				ideviceInfo_0 = feedbackService.iauthManager_0.CurrentDevice;
				if (ideviceInfo_0 != null)
				{
					iuserIdentity_0 = feedbackService.iauthManager_0.CurrentUser;
					if (iuserIdentity_0 != null)
					{
						awaiter = feedbackService.isupabaseClient_0.MarkFeedbackAsReadAsync(guid_0, iuserIdentity_0.UserId).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_01b8;
					}
					awaiter = feedbackService.isupabaseClient_0.MarkFeedbackAsReadAsync(guid_0, ideviceInfo_0.DeviceId ?? string.Empty).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_012c;
				}
				result = Result.Failure("设备信息不可用");
				goto end_IL_000f;
				IL_012c:
				Result result2 = awaiter.GetResult();
				if (result2.IsSuccess)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[FeedbackService] 标记反馈已读成功（设备级别）: ");
					defaultInterpolatedStringHandler.AppendFormatted(guid_0);
					defaultInterpolatedStringHandler.AppendLiteral(", 设备: ");
					defaultInterpolatedStringHandler.AppendFormatted(ideviceInfo_0.DeviceId);
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				goto IL_021f;
				IL_021f:
				result = result2;
				goto end_IL_000f;
				IL_01b8:
				result2 = awaiter.GetResult();
				if (result2.IsSuccess)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("[FeedbackService] 标记反馈已读成功（用户级别）: ");
					defaultInterpolatedStringHandler2.AppendFormatted(guid_0);
					defaultInterpolatedStringHandler2.AppendLiteral(", 用户: ");
					defaultInterpolatedStringHandler2.AppendFormatted(iuserIdentity_0.Email);
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
				goto IL_021f;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[FeedbackService] 标记反馈已读异常", ex);
				result = Result.Failure("标记反馈已读异常：" + ex.Message);
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
	public struct Struct56 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<Guid>> asyncTaskMethodBuilder_0;

		public FeedbackService feedbackService_0;

		public SubmitFeedbackRequest submitFeedbackRequest_0;

		private IDeviceInfo ideviceInfo_0;

		private IUserIdentity iuserIdentity_0;

		private List<FeedbackAttachment> list_0;

		private TaskAwaiter<FeedbackAttachment[]> taskAwaiter_0;

		private TaskAwaiter<Result<Guid>> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FeedbackService feedbackService = feedbackService_0;
			Result<Guid> result;
			try
			{
				TaskAwaiter<FeedbackAttachment[]> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<FeedbackAttachment[]>);
					num = -1;
					int_0 = -1;
					goto IL_015e;
				}
				TaskAwaiter<Result<Guid>> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result<Guid>>);
					num = -1;
					int_0 = -1;
					goto IL_0292;
				}
				ideviceInfo_0 = feedbackService.iauthManager_0.CurrentDevice;
				if (ideviceInfo_0 != null)
				{
					iuserIdentity_0 = feedbackService.iauthManager_0.CurrentUser;
					list_0 = null;
					if (submitFeedbackRequest_0.AttachmentPaths != null && submitFeedbackRequest_0.AttachmentPaths.Count > 0)
					{
						list_0 = new List<FeedbackAttachment>();
						List<Task<FeedbackAttachment>> list = new List<Task<FeedbackAttachment>>();
						List<string>.Enumerator enumerator = submitFeedbackRequest_0.AttachmentPaths.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								string current = enumerator.Current;
								list.Add(feedbackService.method_0(current));
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
							}
						}
						awaiter = Task.WhenAll(list).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_015e;
					}
					goto IL_0192;
				}
				result = Result<Guid>.Failure("设备信息不可用");
				goto end_IL_000f;
				IL_015e:
				FeedbackAttachment[] result2 = awaiter.GetResult();
				foreach (FeedbackAttachment feedbackAttachment in result2)
				{
					if (feedbackAttachment != null)
					{
						list_0.Add(feedbackAttachment);
					}
				}
				goto IL_0192;
				IL_0192:
				string attachments = null;
				if (list_0 != null && list_0.Count > 0)
				{
					attachments = JsonConvert.SerializeObject((object)list_0);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[FeedbackService] 成功上传 ");
					defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
					defaultInterpolatedStringHandler.AppendLiteral(" 个附件");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				ISupabaseClient isupabaseClient_ = feedbackService.isupabaseClient_0;
				string deviceId = ideviceInfo_0.DeviceId ?? string.Empty;
				IUserIdentity obj = iuserIdentity_0;
				awaiter2 = isupabaseClient_.SubmitFeedbackAsync(deviceId, (obj != null) ? new Guid?(obj.UserId) : ((Guid?)null), submitFeedbackRequest_0.Title, submitFeedbackRequest_0.Content, submitFeedbackRequest_0.Email, attachments).GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_0292;
				IL_0292:
				Result<Guid> result3 = awaiter2.GetResult();
				if (result3.IsSuccess)
				{
					Logger.Info("[FeedbackService] 反馈提交成功: " + submitFeedbackRequest_0.Title);
				}
				result = result3;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[FeedbackService] 提交反馈异常", ex);
				result = Result<Guid>.Failure("提交反馈异常：" + ex.Message);
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
	public struct Struct57 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<FeedbackAttachment> asyncTaskMethodBuilder_0;

		public string string_0;

		public FeedbackService feedbackService_0;

		private FileInfo fileInfo_0;

		private byte[] byte_0;

		private string string_1;

		private CosPresignedUrlInfo cosPresignedUrlInfo_0;

		private TaskAwaiter<Result<CosPresignedUrlInfo>> taskAwaiter_0;

		private TaskAwaiter<Result> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FeedbackService feedbackService = feedbackService_0;
			FeedbackAttachment result;
			try
			{
				TaskAwaiter<Result<CosPresignedUrlInfo>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<CosPresignedUrlInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_017c;
				}
				TaskAwaiter<Result> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
					goto IL_0267;
				}
				if (File.Exists(string_0))
				{
					fileInfo_0 = new FileInfo(string_0);
					byte_0 = File.ReadAllBytes(string_0);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[FeedbackService] 准备上传文件: ");
					defaultInterpolatedStringHandler.AppendFormatted(fileInfo_0.Name);
					defaultInterpolatedStringHandler.AppendLiteral(" (");
					defaultInterpolatedStringHandler.AppendFormatted(byte_0.Length);
					defaultInterpolatedStringHandler.AppendLiteral(" bytes)");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					string_1 = feedbackService.method_1(fileInfo_0.Extension);
					awaiter = feedbackService.isupabaseClient_0.GetCosPresignedUrlAsync(fileInfo_0.Name, string_1).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_017c;
				}
				Logger.Warning("[FeedbackService] 文件不存在: " + string_0);
				result = null;
				goto end_IL_000f;
				IL_0267:
				Result result2 = awaiter2.GetResult();
				if (!result2.IsSuccess)
				{
					Logger.Error("[FeedbackService] 上传到 COS 失败: " + fileInfo_0.Name + ", " + result2.Error);
					result = null;
				}
				else
				{
					result = new FeedbackAttachment
					{
						FileName = fileInfo_0.Name,
						FileSize = fileInfo_0.Length,
						StoragePath = cosPresignedUrlInfo_0.ObjectKey,
						PublicUrl = cosPresignedUrlInfo_0.PublicUrl,
						ContentType = string_1,
						UploadedAt = DateTime.UtcNow
					};
				}
				goto end_IL_000f;
				IL_017c:
				Result<CosPresignedUrlInfo> result3 = awaiter.GetResult();
				if (!result3.IsSuccess)
				{
					Logger.Error("[FeedbackService] 获取预签名 URL 失败: " + fileInfo_0.Name + ", " + result3.Error);
					result = null;
				}
				else
				{
					cosPresignedUrlInfo_0 = result3.Value;
					if (!string.IsNullOrEmpty(cosPresignedUrlInfo_0?.PresignedUrl))
					{
						string presignedUrl = cosPresignedUrlInfo_0.PresignedUrl;
						awaiter2 = feedbackService.isupabaseClient_0.UploadToCosWithPresignedUrlAsync(presignedUrl, byte_0, string_1).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_0267;
					}
					Logger.Error("[FeedbackService] 预签名 URL 为空: " + fileInfo_0.Name);
					result = null;
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[FeedbackService] 上传附件异常: " + string_0, ex);
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

	private readonly IAuthManager iauthManager_0;

	private readonly ISupabaseClient isupabaseClient_0;

	public FeedbackService(IAuthManager authManager, ISupabaseClient supabaseClient)
	{
		iauthManager_0 = authManager;
		isupabaseClient_0 = supabaseClient;
	}

	[AsyncStateMachine(typeof(Struct56))]
	public Task<Result<Guid>> SubmitFeedbackAsync(SubmitFeedbackRequest request)
	{
		Struct56 stateMachine = default(Struct56);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<Guid>>.Create();
		stateMachine.feedbackService_0 = this;
		stateMachine.submitFeedbackRequest_0 = request;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct57))]
	private Task<FeedbackAttachment?> method_0(string string_0)
	{
		Struct57 stateMachine = default(Struct57);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<FeedbackAttachment>.Create();
		stateMachine.feedbackService_0 = this;
		stateMachine.string_0 = string_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private string method_1(string string_0)
	{
		string text = string_0.ToLower();
		if (text != null)
		{
			switch (text.Length)
			{
			case 3:
				if (text == ".7z")
				{
					return "application/x-7z-compressed";
				}
				break;
			case 4:
				switch (text[1])
				{
				case 'g':
					if (text == ".gif")
					{
						return "image/gif";
					}
					goto end_IL_0017;
				case 'd':
					if (text == ".doc")
					{
						return "application/msword";
					}
					goto end_IL_0017;
				case 'b':
					if (text == ".bmp")
					{
						return "image/bmp";
					}
					goto end_IL_0017;
				case 'p':
					break;
				case 'r':
					if (text == ".rar")
					{
						return "application/x-rar-compressed";
					}
					goto end_IL_0017;
				case 't':
					if (text == ".txt")
					{
						return "text/plain";
					}
					goto end_IL_0017;
				case 'j':
					goto IL_01a8;
				case 'z':
					if (text == ".zip")
					{
						return "application/zip";
					}
					goto end_IL_0017;
				case 'x':
					if (text == ".xls")
					{
						return "application/vnd.ms-excel";
					}
					goto end_IL_0017;
				default:
					goto end_IL_0017;
				}
				if (!(text == ".png"))
				{
					if (text == ".pdf")
					{
						return "application/pdf";
					}
					break;
				}
				return "image/png";
			case 5:
				{
					char c = text[1];
					if ((uint)c <= 106u)
					{
						if (c != 'd')
						{
							if (c != 'j' || !(text == ".jpeg"))
							{
								break;
							}
							goto IL_0249;
						}
						if (text == ".docx")
						{
							return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
						}
						break;
					}
					switch (c)
					{
					case 'x':
						if (text == ".xlsx")
						{
							return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
						}
						break;
					case 'w':
						if (text == ".webp")
						{
							return "image/webp";
						}
						break;
					}
					break;
				}
				IL_0249:
				return "image/jpeg";
				IL_01a8:
				if (!(text == ".jpg"))
				{
					break;
				}
				goto IL_0249;
				end_IL_0017:
				break;
			}
		}
		return "application/octet-stream";
	}

	[AsyncStateMachine(typeof(Struct52))]
	public Task<Result<List<FeedbackInfo>>> GetFeedbackListAsync()
	{
		Struct52 stateMachine = default(Struct52);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<FeedbackInfo>>>.Create();
		stateMachine.feedbackService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct53))]
	public Task<Result<int>> GetUnreadReplyCountAsync()
	{
		Struct53 stateMachine = default(Struct53);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<int>>.Create();
		stateMachine.feedbackService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct51))]
	public Task<Result> DeleteFeedbackAsync(Guid feedbackId)
	{
		Struct51 stateMachine = default(Struct51);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.feedbackService_0 = this;
		stateMachine.guid_0 = feedbackId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct55))]
	public Task<Result> MarkFeedbackAsReadAsync(Guid feedbackId)
	{
		Struct55 stateMachine = default(Struct55);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.feedbackService_0 = this;
		stateMachine.guid_0 = feedbackId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct54))]
	public Task<Result> MarkAllRepliedAsReadAsync()
	{
		Struct54 stateMachine = default(Struct54);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.feedbackService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
