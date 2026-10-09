using System;
using System.Security.Cryptography;
using System.Text;

namespace RevitAi.Core.Security;

/// <summary>
/// Managed replacement for the original native security module.
///
/// The shipped AS.Tools package contained a native component
/// (AS.Tools.Security.Native.dll) that supplied the Supabase endpoint, the hardware
/// fingerprint, AES, PBKDF2, SHA-256 and random helpers. That binary cannot be
/// rebuilt from the recovered source, and it belongs to the cloud-licensing stack
/// which this rebuild intentionally stubs, so the same INativeCryptoService
/// contract is now served by managed .NET cryptography.
///
/// Data written by this implementation is only readable by this implementation;
/// no attempt is made to stay byte-compatible with the native module.
/// </summary>
public sealed class NativeCryptoService : INativeCryptoService
{
	private const int IV_LENGTH = 16;
	private const int SALT_LENGTH = 32;
	private const int DEFAULT_KEY_LENGTH = 32;
	private const string LOCAL_MASTER_KEY = "RevitAi.Local.MasterKey.2026";

	public string GetSupabaseUrl()
	{
		// Cloud licensing is stubbed in this build.
		return string.Empty;
	}

	internal string method_0()
	{
		// Supabase API key - not used in this build.
		return string.Empty;
	}

	public string GetHardwareFingerprint()
	{
		return GetHardwareFingerprintV2();
	}

	public string GetHardwareFingerprintV2()
	{
		return Fingerprint("v2");
	}

	public string GetHardwareFingerprintV1()
	{
		return Fingerprint("v1");
	}

	public string GetHardwareInfo()
	{
		return Environment.MachineName + "|" + Environment.OSVersion.VersionString + "|" + Environment.ProcessorCount;
	}

	public byte[] EncryptData(byte[] plaintext, byte[] key, byte[] iv)
	{
		if (plaintext == null)
		{
			throw new ArgumentNullException(nameof(plaintext));
		}
		if (key == null)
		{
			throw new ArgumentNullException(nameof(key));
		}
		if (iv == null)
		{
			throw new ArgumentNullException(nameof(iv));
		}

		using Aes aes = Aes.Create();
		aes.Key = key;
		aes.IV = iv;
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;
		using ICryptoTransform transform = aes.CreateEncryptor();
		return transform.TransformFinalBlock(plaintext, 0, plaintext.Length);
	}

	public byte[] DecryptData(byte[] ciphertext, byte[] key, byte[] iv)
	{
		if (ciphertext == null)
		{
			throw new ArgumentNullException(nameof(ciphertext));
		}
		if (key == null)
		{
			throw new ArgumentNullException(nameof(key));
		}
		if (iv == null)
		{
			throw new ArgumentNullException(nameof(iv));
		}

		using Aes aes = Aes.Create();
		aes.Key = key;
		aes.IV = iv;
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;
		using ICryptoTransform transform = aes.CreateDecryptor();
		return transform.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
	}

	public byte[] DeriveKeyFromPassword(string password, byte[] salt, int iterations, int keyLength)
	{
		if (string.IsNullOrEmpty(password))
		{
			throw new ArgumentException("password must not be empty", nameof(password));
		}
		if (salt == null || salt.Length == 0)
		{
			throw new ArgumentException("salt must not be empty", nameof(salt));
		}
		if (iterations <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(iterations));
		}
		if (keyLength <= 0)
		{
			keyLength = DEFAULT_KEY_LENGTH;
		}

		using Rfc2898DeriveBytes derive = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
		return derive.GetBytes(keyLength);
	}

	public byte[] ComputeSha256(byte[] data)
	{
		if (data == null)
		{
			throw new ArgumentNullException(nameof(data));
		}
		using SHA256 sha = SHA256.Create();
		return sha.ComputeHash(data);
	}

	public byte[] GenerateRandomIV() => GenerateRandomBytes(IV_LENGTH);

	public byte[] GenerateRandomSalt() => GenerateRandomBytes(SALT_LENGTH);

	public byte[] GenerateRandomBytes(int length)
	{
		if (length <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(length));
		}
		byte[] buffer = new byte[length];
		RandomNumberGenerator.Fill(buffer);
		return buffer;
	}

	public byte[] EncryptString(string plaintext, byte[] key, byte[] iv)
	{
		if (plaintext == null)
		{
			throw new ArgumentNullException(nameof(plaintext));
		}
		return EncryptData(Encoding.UTF8.GetBytes(plaintext), key, iv);
	}

	public string DecryptString(byte[] ciphertext, byte[] key, byte[] iv)
	{
		return Encoding.UTF8.GetString(DecryptData(ciphertext, key, iv));
	}

	public string GetDefaultMasterKey()
	{
		using SHA256 sha = SHA256.Create();
		byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(LOCAL_MASTER_KEY));
		StringBuilder builder = new StringBuilder(hash.Length * 2);
		foreach (byte b in hash)
		{
			builder.Append(b.ToString("x2"));
		}
		return builder.ToString();
	}

	private static string Fingerprint(string variant)
	{
		string raw = variant + "|" + Environment.MachineName + "|" + Environment.OSVersion.VersionString + "|" + Environment.ProcessorCount;
		using SHA256 sha = SHA256.Create();
		byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
		StringBuilder builder = new StringBuilder(hash.Length * 2);
		foreach (byte b in hash)
		{
			builder.Append(b.ToString("x2"));
		}
		return builder.ToString();
	}
}