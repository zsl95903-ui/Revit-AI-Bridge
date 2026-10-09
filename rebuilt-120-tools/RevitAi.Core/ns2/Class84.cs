using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Core.Configuration;
using RevitAi.Core.Security;
using Newtonsoft.Json;
using ns7;

namespace ns2;

internal sealed class Class84 : IConfigManager
{
	private readonly INativeCryptoService inativeCryptoService_0;

	private readonly Dictionary<string, object> dictionary_0;

	private readonly string string_0;

	private readonly byte[] byte_0;

	private readonly byte[] byte_1;

	private bool bool_0;

	public Class84(INativeCryptoService inativeCryptoService_1, string string_1, string string_2)
	{
		inativeCryptoService_0 = inativeCryptoService_1 ?? throw new ArgumentNullException("cryptoService");
		string_0 = string_1 ?? throw new ArgumentNullException("configFilePath");
		if (string.IsNullOrEmpty(string_2))
		{
			throw new ArgumentException("设备指纹不能为空", "deviceFingerprint");
		}
		byte[] bytes = Encoding.UTF8.GetBytes(string_2.Substring(0, 32));
		byte_0 = inativeCryptoService_0.DeriveKeyFromPassword(string_2, bytes, 100000, 32);
		byte_1 = new byte[12];
		Array.Clear(byte_1, 0, 12);
		dictionary_0 = new Dictionary<string, object>();
		bool_0 = false;
	}

	public Task<Result<T>> GetAsync<T>(string key, T? defaultValue = default(T?))
	{
		try
		{
			if (string.IsNullOrEmpty(key))
			{
				return Task.FromResult<Result<T>>(Result<T>.Failure("配置键不能为空"));
			}
			if (dictionary_0.TryGetValue(key, out object value))
			{
				if (value is T val)
				{
					return Task.FromResult<Result<T>>(Result<T>.Success(val));
				}
				try
				{
					return Task.FromResult<Result<T>>(Result<T>.Success((T)Convert.ChangeType(value, typeof(T))));
				}
				catch
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
					defaultInterpolatedStringHandler.AppendLiteral("配置值 '");
					defaultInterpolatedStringHandler.AppendFormatted(key);
					defaultInterpolatedStringHandler.AppendLiteral("' 不是 ");
					defaultInterpolatedStringHandler.AppendFormatted(typeof(T).Name);
					defaultInterpolatedStringHandler.AppendLiteral(" 类型");
					return Task.FromResult<Result<T>>(Result<T>.Failure(defaultInterpolatedStringHandler.ToStringAndClear()));
				}
			}
			if (defaultValue != null)
			{
				return Task.FromResult<Result<T>>(Result<T>.Success(defaultValue));
			}
			return Task.FromResult<Result<T>>(Result<T>.Success(default(T)));
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result<T>>(Result<T>.Failure("获取配置值失败：" + ex.Message));
		}
	}

	public Task<Result> SetAsync<T>(string key, T value)
	{
		try
		{
			if (string.IsNullOrEmpty(key))
			{
				return Task.FromResult<Result>(Result.Failure("配置键不能为空"));
			}
			Dictionary<string, object> dictionary = dictionary_0;
			if (value == null)
			{
				throw new ArgumentException("配置值不能为空", "value");
			}
			dictionary[key] = value;
			bool_0 = true;
			return Task.FromResult<Result>(Result.Success());
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result>(Result.Failure("设置配置值失败：" + ex.Message));
		}
	}

	public Task<Result<bool>> ExistsAsync(string key)
	{
		try
		{
			if (string.IsNullOrEmpty(key))
			{
				return Task.FromResult<Result<bool>>(Result<bool>.Failure("配置键不能为空"));
			}
			return Task.FromResult<Result<bool>>(Result<bool>.Success(dictionary_0.ContainsKey(key)));
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result<bool>>(Result<bool>.Failure("检查配置键失败：" + ex.Message));
		}
	}

	public Task<Result> RemoveAsync(string key)
	{
		try
		{
			if (string.IsNullOrEmpty(key))
			{
				return Task.FromResult<Result>(Result.Failure("配置键不能为空"));
			}
			dictionary_0.Remove(key);
			bool_0 = true;
			return Task.FromResult<Result>(Result.Success());
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result>(Result.Failure("移除配置键失败：" + ex.Message));
		}
	}

	public Task<Result> SaveAsync()
	{
		try
		{
			if (!bool_0)
			{
				return Task.FromResult<Result>(Result.Success());
			}
			string s = JsonConvert.SerializeObject((object)dictionary_0, (Formatting)1);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			byte[] bytes2 = inativeCryptoService_0.EncryptData(bytes, byte_0, byte_1);
			string directoryName = Path.GetDirectoryName(string_0);
			if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllBytes(string_0, bytes2);
			bool_0 = false;
			return Task.FromResult<Result>(Result.Success());
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result>(Result.Failure("保存配置失败：" + ex.Message));
		}
	}

	public Task<Result> ReloadAsync()
	{
		try
		{
			if (!File.Exists(string_0))
			{
				dictionary_0.Clear();
				return Task.FromResult<Result>(Result.Success());
			}
			byte[] ciphertext = File.ReadAllBytes(string_0);
			byte[] bytes = inativeCryptoService_0.DecryptData(ciphertext, byte_0, byte_1);
			Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Encoding.UTF8.GetString(bytes));
			dictionary_0.Clear();
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, object> item in dictionary)
				{
					dictionary_0[item.Key] = item.Value;
				}
			}
			bool_0 = false;
			return Task.FromResult<Result>(Result.Success());
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result>(Result.Failure("重新加载配置失败：" + ex.Message));
		}
	}

	public Task<Result> ClearAsync()
	{
		try
		{
			dictionary_0.Clear();
			bool_0 = true;
			return Task.FromResult<Result>(Result.Success());
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result>(Result.Failure("清除配置失败：" + ex.Message));
		}
	}

	public Task<Result<string[]>> GetKeysInSectionAsync(string section)
	{
		try
		{
			if (string.IsNullOrEmpty(section))
			{
				return Task.FromResult<Result<string[]>>(Result<string[]>.Failure("节名称不能为空"));
			}
			string value = section + ":";
			List<string> list = new List<string>();
			foreach (string key in dictionary_0.Keys)
			{
				if (key.StartsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					list.Add(key);
				}
			}
			return Task.FromResult<Result<string[]>>(Result<string[]>.Success(list.ToArray()));
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result<string[]>>(Result<string[]>.Failure("获取节中的键失败：" + ex.Message));
		}
	}

	public Task<Result<Dictionary<string, object>>> GetSectionAsync(string section)
	{
		try
		{
			if (string.IsNullOrEmpty(section))
			{
				return Task.FromResult<Result<Dictionary<string, object>>>(Result<Dictionary<string, object>>.Failure("节名称不能为空"));
			}
			string text = section + ":";
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> item in dictionary_0)
			{
				if (item.Key.StartsWith(text, StringComparison.OrdinalIgnoreCase))
				{
					string key = item.Key.Substring(text.Length);
					dictionary[key] = item.Value;
				}
			}
			return Task.FromResult<Result<Dictionary<string, object>>>(Result<Dictionary<string, object>>.Success(dictionary));
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result<Dictionary<string, object>>>(Result<Dictionary<string, object>>.Failure("获取节失败：" + ex.Message));
		}
	}
}
