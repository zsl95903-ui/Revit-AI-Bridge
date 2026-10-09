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
using ns7;

namespace RevitAi.Core.AI;

public sealed class FileAttachmentService : IFileAttachmentService
{
	[CompilerGenerated]
	public sealed class Class127
	{
		public DateTime dateTime_0;

		public TimeSpan timeSpan_0;

		internal bool method_0(FileAttachment fileAttachment_0)
		{
			return dateTime_0 - fileAttachment_0.UploadedAt > timeSpan_0;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct196 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public FileAttachmentService fileAttachmentService_0;

		private Class127 class127_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FileAttachmentService fileAttachmentService = fileAttachmentService_0;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					this.class127_0 = new Class127();
					this.class127_0.timeSpan_0 = TimeSpan.FromDays(7);
					this.class127_0.dateTime_0 = DateTime.Now;
					awaiter = fileAttachmentService.semaphoreSlim_0.WaitAsync().GetAwaiter();
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
				try
				{
				var class127_0 = this.class127_0;
					List<FileAttachment>.Enumerator enumerator = fileAttachmentService.dictionary_0.Values.Where((FileAttachment fileAttachment_0) => class127_0.dateTime_0 - fileAttachment_0.UploadedAt > class127_0.timeSpan_0).ToList().GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							FileAttachment current = enumerator.Current;
							if (File.Exists(current.FilePath))
							{
								try
								{
									File.Delete(current.FilePath);
								}
								catch
								{
								}
							}
							fileAttachmentService.dictionary_0.Remove(current.AttachmentId);
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
				finally
				{
					if (num < 0)
					{
						fileAttachmentService.semaphoreSlim_0.Release();
					}
				}
				class127_0 = null;
			}
			catch
			{
			}
			int_0 = -2;
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
	public struct Struct197 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<int> asyncTaskMethodBuilder_0;

		public FileAttachmentService fileAttachmentService_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FileAttachmentService fileAttachmentService = fileAttachmentService_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				awaiter = fileAttachmentService.semaphoreSlim_0.WaitAsync(cancellationToken_0).GetAwaiter();
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
			int result;
			try
			{
				int count = fileAttachmentService.dictionary_0.Count;
				Dictionary<string, FileAttachment>.ValueCollection.Enumerator enumerator = fileAttachmentService.dictionary_0.Values.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						FileAttachment current = enumerator.Current;
						if (File.Exists(current.FilePath))
						{
							try
							{
								File.Delete(current.FilePath);
							}
							catch
							{
							}
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
				fileAttachmentService.dictionary_0.Clear();
				result = count;
			}
			finally
			{
				if (num < 0)
				{
					fileAttachmentService.semaphoreSlim_0.Release();
				}
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
	public struct Struct198 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<bool> asyncTaskMethodBuilder_0;

		public FileAttachmentService fileAttachmentService_0;

		public CancellationToken cancellationToken_0;

		public string string_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FileAttachmentService fileAttachmentService = fileAttachmentService_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				awaiter = fileAttachmentService.semaphoreSlim_0.WaitAsync(cancellationToken_0).GetAwaiter();
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
			bool result;
			try
			{
				if (!fileAttachmentService.dictionary_0.TryGetValue(string_0, out FileAttachment value))
				{
					result = false;
				}
				else
				{
					if (File.Exists(value.FilePath))
					{
						try
						{
							File.Delete(value.FilePath);
						}
						catch
						{
						}
					}
					fileAttachmentService.dictionary_0.Remove(string_0);
					result = true;
				}
			}
			finally
			{
				if (num < 0)
				{
					fileAttachmentService.semaphoreSlim_0.Release();
				}
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
	public struct Struct199 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

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
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
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
			string empty = string.Empty;
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(empty);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct200 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<FileAttachment> asyncTaskMethodBuilder_0;

		public FileAttachmentService fileAttachmentService_0;

		public string string_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FileAttachmentService fileAttachmentService = fileAttachmentService_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
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
			fileAttachmentService.dictionary_0.TryGetValue(string_0, out FileAttachment value);
			FileAttachment result = value;
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
	public struct Struct201 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<List<FileAttachment>> asyncTaskMethodBuilder_0;

		public FileAttachmentService fileAttachmentService_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FileAttachmentService fileAttachmentService = fileAttachmentService_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
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
			List<FileAttachment> result = fileAttachmentService.dictionary_0.Values.ToList();
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
	public struct Struct202 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<FileAttachment> asyncTaskMethodBuilder_0;

		public FileAttachmentService fileAttachmentService_0;

		public CancellationToken cancellationToken_0;

		public string string_0;

		public string string_1;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Expected O, but got Unknown
			int num = int_0;
			FileAttachmentService fileAttachmentService = fileAttachmentService_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				awaiter = fileAttachmentService.semaphoreSlim_0.WaitAsync(cancellationToken_0).GetAwaiter();
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
			FileAttachment result;
			try
			{
				if (!File.Exists(string_0))
				{
					throw new FileNotFoundException("源文件不存在: " + string_0);
				}
				FileInfo fileInfo = new FileInfo(string_0);
				if (fileInfo.Length > 52428800L)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
					defaultInterpolatedStringHandler.AppendLiteral("文件大小超过限制 (");
					defaultInterpolatedStringHandler.AppendFormatted(50L);
					defaultInterpolatedStringHandler.AppendLiteral("MB)");
					throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				string text = Guid.NewGuid().ToString();
				string extension = Path.GetExtension(string_1);
				string path = text + extension;
				string text2 = Path.Combine(fileAttachmentService.string_0, path);
				File.Copy(string_0, text2, overwrite: true);
				FileAttachment val = new FileAttachment
				{
					AttachmentId = text,
					FileName = string_1,
					FileExtension = extension,
					FileType = FileAttachment.GetFileTypeFromExtension(extension),
					FileSize = fileInfo.Length,
					FilePath = text2,
					MimeType = FileAttachment.GetMimeType(extension),
					UploadedAt = DateTime.Now
				};
				fileAttachmentService.dictionary_0[text] = val;
				result = val;
			}
			finally
			{
				if (num < 0)
				{
					fileAttachmentService.semaphoreSlim_0.Release();
				}
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
	public struct Struct203 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<FileAttachment> asyncTaskMethodBuilder_0;

		public FileAttachmentService fileAttachmentService_0;

		public CancellationToken cancellationToken_0;

		public byte[] byte_0;

		public string string_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Expected O, but got Unknown
			int num = int_0;
			FileAttachmentService fileAttachmentService = fileAttachmentService_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				awaiter = fileAttachmentService.semaphoreSlim_0.WaitAsync(cancellationToken_0).GetAwaiter();
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
			FileAttachment result;
			try
			{
				if (byte_0.Length > 52428800L)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
					defaultInterpolatedStringHandler.AppendLiteral("文件大小超过限制 (");
					defaultInterpolatedStringHandler.AppendFormatted(50L);
					defaultInterpolatedStringHandler.AppendLiteral("MB)");
					throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				string text = Guid.NewGuid().ToString();
				string extension = Path.GetExtension(string_0);
				string path = text + extension;
				string text2 = Path.Combine(fileAttachmentService.string_0, path);
				File.WriteAllBytes(text2, byte_0);
				FileAttachment val = new FileAttachment
				{
					AttachmentId = text,
					FileName = string_0,
					FileExtension = extension,
					FileType = FileAttachment.GetFileTypeFromExtension(extension),
					FileSize = byte_0.Length,
					FilePath = text2,
					MimeType = FileAttachment.GetMimeType(extension),
					UploadedAt = DateTime.Now
				};
				fileAttachmentService.dictionary_0[text] = val;
				result = val;
			}
			finally
			{
				if (num < 0)
				{
					fileAttachmentService.semaphoreSlim_0.Release();
				}
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

	private const long long_0 = 52428800L;

	private const int int_0 = 100;

	private readonly string string_0;

	private readonly Dictionary<string, FileAttachment> dictionary_0;

	private readonly SemaphoreSlim semaphoreSlim_0;

	public FileAttachmentService()
	{
		string tempPath = Path.GetTempPath();
		string_0 = Path.Combine(tempPath, "RevitAi", "Attachments");
		Directory.CreateDirectory(string_0);
		dictionary_0 = new Dictionary<string, FileAttachment>();
		semaphoreSlim_0 = new SemaphoreSlim(1, 1);
		Task.Run(() => method_0());
	}

	[AsyncStateMachine(typeof(Struct202))]
	public Task<FileAttachment> SaveFileAsync(string sourceFilePath, string originalFileName, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct202 stateMachine = default(Struct202);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<FileAttachment>.Create();
		stateMachine.fileAttachmentService_0 = this;
		stateMachine.string_0 = sourceFilePath;
		stateMachine.string_1 = originalFileName;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct203))]
	public Task<FileAttachment> SaveFileAsync(byte[] fileBytes, string originalFileName, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct203 stateMachine = default(Struct203);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<FileAttachment>.Create();
		stateMachine.fileAttachmentService_0 = this;
		stateMachine.byte_0 = fileBytes;
		stateMachine.string_0 = originalFileName;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct200))]
	public Task<FileAttachment?> GetAttachmentAsync(string attachmentId)
	{
		Struct200 stateMachine = default(Struct200);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<FileAttachment>.Create();
		stateMachine.fileAttachmentService_0 = this;
		stateMachine.string_0 = attachmentId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct199))]
	public Task<string> ExtractTextAsync(string attachmentId, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct199 stateMachine = default(Struct199);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct198))]
	public Task<bool> DeleteAttachmentAsync(string attachmentId, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct198 stateMachine = default(Struct198);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<bool>.Create();
		stateMachine.fileAttachmentService_0 = this;
		stateMachine.string_0 = attachmentId;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct201))]
	public Task<List<FileAttachment>> GetSessionAttachmentsAsync(string sessionId, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct201 stateMachine = default(Struct201);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<List<FileAttachment>>.Create();
		stateMachine.fileAttachmentService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct197))]
	public Task<int> ClearSessionAttachmentsAsync(string sessionId, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct197 stateMachine = default(Struct197);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<int>.Create();
		stateMachine.fileAttachmentService_0 = this;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public string GetStorageDirectory()
	{
		return string_0;
	}

	public bool IsSupportedFileType(string fileExtension, string? provider = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected I4, but got Unknown
		FileAttachmentType fileTypeFromExtension = FileAttachment.GetFileTypeFromExtension(fileExtension);
		if (string.IsNullOrEmpty(provider))
		{
			return ((int)(fileTypeFromExtension) - 2) switch
			{
				0 => true, 
				1 => true, 
				2 => true, 
				3 => true, 
				_ => false, 
			};
		}
		return PredefinedFileCapabilities.GetCapability(provider, true).IsSupported(fileTypeFromExtension);
	}

	public long GetMaxFileSize(string fileExtension, string? provider = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		FileAttachmentType fileTypeFromExtension = FileAttachment.GetFileTypeFromExtension(fileExtension);
		if (!string.IsNullOrEmpty(provider))
		{
			return PredefinedFileCapabilities.GetCapability(provider, true).GetMaxFileSize(fileTypeFromExtension);
		}
		return 52428800L;
	}

	public Task<byte[]> ReadFileBytesAsync(string attachmentId, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (!dictionary_0.TryGetValue(attachmentId, out FileAttachment value))
		{
			throw new ArgumentException("附件不存在: " + attachmentId);
		}
		if (!File.Exists(value.FilePath))
		{
			throw new FileNotFoundException("附件文件不存在: " + value.FilePath);
		}
		return Task.FromResult(File.ReadAllBytes(value.FilePath));
	}

	[AsyncStateMachine(typeof(Struct196))]
	private Task method_0()
	{
		Struct196 stateMachine = default(Struct196);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.fileAttachmentService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public void Dispose()
	{
		semaphoreSlim_0?.Dispose();
	}

	[CompilerGenerated]
	private Task? method_1()
	{
		return method_0();
	}
}
