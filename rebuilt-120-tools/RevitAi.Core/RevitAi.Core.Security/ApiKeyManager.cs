using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Core.Authentication;
using ns7;

namespace RevitAi.Core.Security;

public class ApiKeyManager
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct35 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public ApiKeyManager apiKeyManager_0;

		public string string_0;

		private string string_1;

		private TaskAwaiter<Result<EncryptedApiKeyRecord>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ApiKeyManager apiKeyManager = apiKeyManager_0;
			TaskAwaiter<Result<EncryptedApiKeyRecord>> awaiter;
			if (num != 0)
			{
				string_1 = MasterKeyProvider.GetMasterKey();
				awaiter = apiKeyManager.isupabaseClient_0.GetEncryptedApiKeyAsync(string_0).GetAwaiter();
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
				taskAwaiter_0 = default(TaskAwaiter<Result<EncryptedApiKeyRecord>>);
				num = -1;
				int_0 = -1;
			}
			Result<EncryptedApiKeyRecord> result = awaiter.GetResult();
			if (result.IsSuccess && result.Value != null)
			{
				string result2 = apiKeyManager.DecryptApiKey(result.Value.EncryptedKey, string_1);
				int_0 = -2;
				string_1 = null;
				asyncTaskMethodBuilder_0.SetResult(result2);
				return;
			}
			throw new InvalidOperationException("未找到 API 地址为 " + string_0 + " 的加密密钥: " + result.Error);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct36 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public ApiKeyManager apiKeyManager_0;

		public string string_0;

		private string string_1;

		private TaskAwaiter<Result<EncryptedApiKeyRecord>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ApiKeyManager apiKeyManager = apiKeyManager_0;
			TaskAwaiter<Result<EncryptedApiKeyRecord>> awaiter;
			if (num != 0)
			{
				string_1 = MasterKeyProvider.GetMasterKey();
				awaiter = apiKeyManager.isupabaseClient_0.GetEncryptedApiKeyByNotesAsync(string_0).GetAwaiter();
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
				taskAwaiter_0 = default(TaskAwaiter<Result<EncryptedApiKeyRecord>>);
				num = -1;
				int_0 = -1;
			}
			Result<EncryptedApiKeyRecord> result = awaiter.GetResult();
			if (result.IsSuccess && result.Value != null)
			{
				string result2 = apiKeyManager.DecryptApiKey(result.Value.EncryptedKey, string_1);
				int_0 = -2;
				string_1 = null;
				asyncTaskMethodBuilder_0.SetResult(result2);
				return;
			}
			throw new InvalidOperationException("未找到备注为 " + string_0 + " 的加密密钥: " + result.Error);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly INativeCryptoService inativeCryptoService_0;

	private readonly ISupabaseClient isupabaseClient_0;

	public ApiKeyManager(INativeCryptoService crypto, ISupabaseClient supabase)
	{
		inativeCryptoService_0 = crypto ?? throw new ArgumentNullException("crypto");
		isupabaseClient_0 = supabase ?? throw new ArgumentNullException("supabase");
	}

	[AsyncStateMachine(typeof(Struct35))]
	public Task<string> GetApiKeyAsync(string apiUrl)
	{
		Struct35 stateMachine = default(Struct35);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.apiKeyManager_0 = this;
		stateMachine.string_0 = apiUrl;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct36))]
	public Task<string> GetApiKeyByNotesAsync(string notes)
	{
		Struct36 stateMachine = default(Struct36);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.apiKeyManager_0 = this;
		stateMachine.string_0 = notes;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public string DecryptApiKey(EncryptedApiKey encryptedKey, string masterKey)
	{
		try
		{
			byte[] salt = Convert.FromBase64String(encryptedKey.Salt);
			byte[] iv = Convert.FromBase64String(encryptedKey.IV);
			byte[] array = Convert.FromBase64String(encryptedKey.EncryptedData);
			byte[] key = inativeCryptoService_0.DeriveKeyFromPassword(masterKey, salt, 100000, 32);
			byte[] array3;
			if (!string.IsNullOrEmpty(encryptedKey.Tag))
			{
				byte[] array2 = Convert.FromBase64String(encryptedKey.Tag);
				array3 = new byte[array.Length + array2.Length];
				Buffer.BlockCopy(array, 0, array3, 0, array.Length);
				Buffer.BlockCopy(array2, 0, array3, array.Length, array2.Length);
			}
			else
			{
				array3 = array;
			}
			byte[] bytes = inativeCryptoService_0.DecryptData(array3, key, iv);
			return Encoding.UTF8.GetString(bytes);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("解密 API Key 失败: " + ex.Message, ex);
		}
	}

	public bool ValidateApiKey(string apiKey)
	{
		if (string.IsNullOrEmpty(apiKey))
		{
			return false;
		}
		if (apiKey.Length < 20)
		{
			return false;
		}
		string[] array = new string[5]
		{
			"sk-ant-",
			"sk-proj-",
			"sk-",
			"Bearer ",
			"https://"
		};
		int num = 0;
		while (true)
		{
			if (num < array.Length)
			{
				string value = array[num];
				if (apiKey.StartsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					break;
				}
				num++;
				continue;
			}
			if (!apiKey.Contains("-"))
			{
				return apiKey.Length >= 32;
			}
			return true;
		}
		return true;
	}
}
