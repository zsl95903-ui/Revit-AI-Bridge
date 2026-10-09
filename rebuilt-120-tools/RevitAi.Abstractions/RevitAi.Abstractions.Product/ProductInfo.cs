using System;
using System.Collections.Generic;
using System.Text;

namespace RevitAi.Abstractions.Product;

public static class ProductInfo
{
	private const string VERSION = "4.1.0";

	public static readonly Dictionary<string, string> ReleaseNotes = new Dictionary<string, string>
	{
		{ "4.1.0", " Version 4.1.0 (2026-10-09)\n\n \ud83d\udd27 重建版本\n- 由 1.2.5.3 安装包与二进制还原为可编译源码（7 个程序集全部编译通过）\n- 产品更名为 RevitAi\n- 授权层改为本地实现：不再连接 Supabase，不做设备/许可证联网校验\n- 云端密钥已移除；自动更新与在线代码市场在无云端配置时优雅降级\n" },
		{ "1.2.5.3", " Version 1.2.5.3 (2026-08-28)\n\n \ud83d\udd27 改进\n- 优化 AI 对话记忆功能：提升多轮对话的上下文连贯性与记忆管理能力\n- 优化部分 AI 工具：改进工具调用逻辑\n" },
		{ "1.2.5.2", " Version 1.2.5.2 (2026-08-25)\n\n ✨ 新功能\n- AI 对话支持图片附件上传：可在对话中上传图片进行识别和分析\n- 新增 高程标注 AI 工具：支持创建点高程标注\n \ud83d\udd27 改进\n- 优化 AI 工具组合：合并相似功能工具，减少 token 消耗\n- 优化提示词：简化 AI 工具描述，提高响应效率\n" },
		{ "1.2.4.8", " Version 1.2.4.8 (2026-08-19)\n\n ✨ 新功能\n- 新增 保温查询和管理 AI 工具：实现对管道和风管的保温层，添加、删除、修改和查询\n- 新增 MEP 系统查询和管理 AI 工具：支持查询和过滤 MEP 系统\n- 新增 视图临时隐藏/隔离 AI 工具：支持临时隐藏或隔离指定元素\n \ud83d\udd27 改进\n- 优化 MEP 系统工具：优化相关工具为一个管理工具\n \ud83d\udd27 修复\n- 修复 AI 服务返回无效响应的问题\n- 修复优惠码无法使用的问题\n" },
		{ "1.2.4.4", " Version 1.2.4.4 (2026-08-10)\n\n ✨ 新功能\n- 新增 管理全局参数 AI 工具：支持查看、修改、删除 Revit 全局参数\n- 新增 管理项目参数 AI 工具：支持查看、修改、删除项目参数\n- 新增 CAD 图层概览 AI 工具：快速查看 CAD 图层分布和统计信息\n \ud83d\udd27 改进\n- 优化 管理共享参数 AI 工具：使用默认参数文件，自动定位共享参数文件\n- 优化 设置参数 AI 工具：合并参数查询、设置等功能为一个统一工具\n" }
	};

	private const string ENCRYPTED_SECRET_ID = "";

	private const string ENCRYPTED_SECRET_KEY = "";

	private static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("ASTools2026Secret");

	public const string CosBucketName = "astools-1314165830";

	public const string CosRegion = "ap-shanghai";

	public const string CosKeyPrefix = "";

	public static string Version => "4.1.0";

	public static string ProductName => "RevitAi";

	public static string Company => "AST";

	public static string Copyright => "Copyright © 2026";

	public static string CosSecretId => string.Empty;

	public static string CosSecretKey => string.Empty;

	public static string UpdateBaseUrl => "https://astools-1314165830.cos.ap-shanghai.myqcloud.com";

	public static string GetCurrentVersionReleaseNotes()
	{
		if (!ReleaseNotes.TryGetValue(Version, out string value))
		{
			return string.Empty;
		}
		return value;
	}

	public static string GetReleaseNotes(string version)
	{
		if (!ReleaseNotes.TryGetValue(version, out string value))
		{
			return string.Empty;
		}
		return value;
	}

	public static string GetFullVersionInfo()
	{
		return ProductName + " v" + Version;
	}

	public static string GetUserAgent()
	{
		return ProductName + "/" + Version + " (Windows)";
	}

	private static string DecryptString(string encrypted)
	{
		try
		{
			byte[] array = Convert.FromBase64String(encrypted);
			byte[] array2 = new byte[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = (byte)(array[i] ^ EncryptionKey[i % EncryptionKey.Length]);
			}
			return Encoding.UTF8.GetString(array2);
		}
		catch (Exception innerException)
		{
			throw new InvalidOperationException("解密凭据失败，请检查 ENCRYPTED_SECRET_ID 和 ENCRYPTED_SECRET_KEY", innerException);
		}
	}
}
