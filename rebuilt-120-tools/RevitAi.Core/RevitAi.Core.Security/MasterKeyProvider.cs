using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using ns1;
using ns7;

namespace RevitAi.Core.Security;

public static class MasterKeyProvider
{
	private const string string_0 = "AI_ENCRYPTION_MASTER_KEY";

	private const string string_1 = "ai_master_key.json";

	private const string string_2 = "RevitAi.AI.MasterKey";

	public static string GetMasterKey(bool allowDefaultKey = true)
	{
		string environmentVariable = Environment.GetEnvironmentVariable("AI_ENCRYPTION_MASTER_KEY", EnvironmentVariableTarget.User);
		if (!string.IsNullOrEmpty(environmentVariable))
		{
			return environmentVariable;
		}
		try
		{
			string text = smethod_0();
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
		}
		catch
		{
		}
		try
		{
			string text2 = smethod_2();
			if (!string.IsNullOrEmpty(text2))
			{
				return text2;
			}
		}
		catch
		{
		}
		if (allowDefaultKey)
		{
			try
			{
				string text3 = smethod_4();
				if (!string.IsNullOrEmpty(text3))
				{
					return text3;
				}
			}
			catch
			{
			}
		}
		throw new InvalidOperationException("无法获取主密钥。请通过以下方式之一设置主密钥：\n1. 环境变量: AI_ENCRYPTION_MASTER_KEY\n2. Windows 凭据管理器: RevitAi.AI.MasterKey\n3. 本地配置文件: " + smethod_3());
	}

	public static void SetMasterKeyToCredentialManager(string masterKey)
	{
		if (string.IsNullOrEmpty(masterKey))
		{
			throw new ArgumentException("主密钥不能为空", "masterKey");
		}
		smethod_1("RevitAi.AI.MasterKey", masterKey);
	}

	public static void SetMasterKeyToLocalConfig(string masterKey)
	{
		if (string.IsNullOrEmpty(masterKey))
		{
			throw new ArgumentException("主密钥不能为空", "masterKey");
		}
		NativeCryptoService nativeCryptoService = new NativeCryptoService();
		byte[] bytes = Encoding.UTF8.GetBytes(Environment.MachineName + Environment.UserName);
		byte[] key = nativeCryptoService.DeriveKeyFromPassword("Default_Master_Key_Encryption_Pwd", bytes, 100000, 32);
		byte[] array = new byte[12];
		using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
		{
			randomNumberGenerator.GetBytes(array);
		}
		string encryptedKey = Convert.ToBase64String(nativeCryptoService.EncryptString(masterKey, key, array));
		Class72 @class = new Class72
		{
			EncryptedKey = encryptedKey,
			IV = Convert.ToBase64String(array),
			MachineName = Environment.MachineName,
			UserName = Environment.UserName,
			CreatedAt = DateTime.UtcNow.ToString("o")
		};
		string path = smethod_3();
		string directoryName = Path.GetDirectoryName(path);
		if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		string contents = JsonConvert.SerializeObject((object)@class, (Formatting)1);
		File.WriteAllText(path, contents);
	}

	private static string? smethod_0()
	{
		try
		{
			using Class73.Class74 @class = new Class73.Class74("RevitAi.AI.MasterKey");
			return @class.Password;
		}
		catch
		{
			return null;
		}
	}

	private static void smethod_1(string string_3, string string_4)
	{
		using Class73.Class74 @class = new Class73.Class74(string_3, Environment.UserName, string_4);
		@class.method_0();
	}

	private static string? smethod_2()
	{
		string path = smethod_3();
		if (!File.Exists(path))
		{
			return null;
		}
		try
		{
			Class72 @class = JsonConvert.DeserializeObject<Class72>(File.ReadAllText(path));
			if (@class != null && @class.EncryptedKey != null && @class.IV != null)
			{
				NativeCryptoService nativeCryptoService = new NativeCryptoService();
				byte[] key = nativeCryptoService.DeriveKeyFromPassword(salt: Encoding.UTF8.GetBytes(Environment.MachineName + Environment.UserName), password: "Default_Master_Key_Encryption_Pwd", iterations: 100000, keyLength: 32);
				byte[] iv = Convert.FromBase64String(@class.IV);
				byte[] ciphertext = Convert.FromBase64String(@class.EncryptedKey);
				return nativeCryptoService.DecryptString(ciphertext, key, iv);
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	private static string smethod_3()
	{
		return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "ai_master_key.json");
	}

	private static string? smethod_4()
	{
		try
		{
			return new NativeCryptoService().GetDefaultMasterKey();
		}
		catch
		{
			return null;
		}
	}

	public static bool ValidateMasterKey(string masterKey)
	{
		try
		{
			if (string.IsNullOrEmpty(masterKey))
			{
				return false;
			}
			NativeCryptoService nativeCryptoService = new NativeCryptoService();
			string text = "Test_String_123";
			byte[] key = nativeCryptoService.DeriveKeyFromPassword(masterKey, new byte[16], 10000, 32);
			byte[] ciphertext = nativeCryptoService.EncryptString(text, key, new byte[12]);
			return nativeCryptoService.DecryptString(ciphertext, key, new byte[12]) == text;
		}
		catch
		{
			return false;
		}
	}
}
