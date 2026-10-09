using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using ns7;

namespace ns1;

internal static class Class75
{
	public static SecureString smethod_0(string string_0)
	{
		if (string.IsNullOrEmpty(string_0))
		{
			return new SecureString();
		}
		SecureString secureString = new SecureString();
		foreach (char c in string_0)
		{
			secureString.AppendChar(c);
		}
		secureString.MakeReadOnly();
		return secureString;
	}

	public static string smethod_1(SecureString secureString_0)
	{
		if (secureString_0 == null)
		{
			throw new ArgumentNullException("secureString");
		}
		nint num = IntPtr.Zero;
		try
		{
			num = Marshal.SecureStringToGlobalAllocUnicode(secureString_0);
			return Marshal.PtrToStringUni(num) ?? string.Empty;
		}
		finally
		{
			if (num != IntPtr.Zero)
			{
				Marshal.ZeroFreeGlobalAllocUnicode(num);
			}
		}
	}

	public static void smethod_2(ref string string_0)
	{
		if (!string.IsNullOrEmpty(string_0))
		{
			string_0 = new string('\0', string_0.Length);
		}
	}

	public static string smethod_3(int int_0, bool bool_0 = false)
	{
		if (int_0 < 1)
		{
			throw new ArgumentException("长度必须至少为 1", "length");
		}
		string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		if (bool_0)
		{
			text += "!@#$%^&*()_+-=[]{}|;:,.<>?";
		}
		char[] array = new char[int_0];
		byte[] array2 = new byte[int_0];
		using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
		{
			randomNumberGenerator.GetBytes(array2);
		}
		for (int i = 0; i < int_0; i++)
		{
			array[i] = text[array2[i] % text.Length];
		}
		return new string(array);
	}

	public static byte[] smethod_4(int int_0)
	{
		if (int_0 < 1)
		{
			throw new ArgumentException("长度必须至少为 1", "length");
		}
		byte[] array = new byte[int_0];
		using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
		randomNumberGenerator.GetBytes(array);
		return array;
	}

	public static string smethod_5(byte[] byte_0)
	{
		if (byte_0 == null)
		{
			throw new ArgumentNullException("bytes");
		}
		char[] array = new char[byte_0.Length * 2];
		for (int i = 0; i < byte_0.Length; i++)
		{
			byte b = byte_0[i];
			array[i * 2] = smethod_7(b >> 4);
			array[i * 2 + 1] = smethod_7(b & 0xF);
		}
		return new string(array);
	}

	public static byte[] smethod_6(string string_0)
	{
		if (string.IsNullOrEmpty(string_0))
		{
			throw new ArgumentException("十六进制字符串不能为空", "hex");
		}
		if (string_0.Length % 2 != 0)
		{
			throw new ArgumentException("十六进制字符串必须具有偶数长度", "hex");
		}
		byte[] array = new byte[string_0.Length / 2];
		for (int i = 0; i < array.Length; i++)
		{
			int num = smethod_8(string_0[i * 2]);
			int num2 = smethod_8(string_0[i * 2 + 1]);
			array[i] = (byte)((num << 4) | num2);
		}
		return array;
	}

	private static char smethod_7(int int_0)
	{
		return (char)((int_0 < 10) ? (48 + int_0) : (97 + int_0 - 10));
	}

	private static int smethod_8(char char_0)
	{
		if (char_0 >= '0' && char_0 <= '9')
		{
			return char_0 - 48;
		}
		if (char_0 >= 'a' && char_0 <= 'f')
		{
			return char_0 - 97 + 10;
		}
		if (char_0 < 'A' || char_0 > 'F')
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler.AppendLiteral("无效的十六进制字符：");
			defaultInterpolatedStringHandler.AppendFormatted(char_0);
			throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "c");
		}
		return char_0 - 65 + 10;
	}

	public static bool smethod_9(byte[] byte_0, byte[] byte_1)
	{
		if (byte_0 != null && byte_1 != null)
		{
			if (byte_0.Length != byte_1.Length)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < byte_0.Length; i++)
			{
				num |= byte_0[i] ^ byte_1[i];
			}
			return num == 0;
		}
		return false;
	}
}
