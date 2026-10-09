using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using RevitAi.Abstractions.Logging;
using RevitAi.UI.Models;

namespace RevitAi.UI.Services;

public class LocalAIConfigService
{
	private const int CURRENT_VERSION = 2;

	private readonly string _configDirectory;

	private readonly string _configFilePath;

	private readonly string _defaultModelPath;

	public LocalAIConfigService()
	{
		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		_configDirectory = Path.Combine(folderPath, "RevitAi");
		_configFilePath = Path.Combine(_configDirectory, "ai_configs.json");
		_defaultModelPath = Path.Combine(_configDirectory, "default_ai_model.txt");
		if (!Directory.Exists(_configDirectory))
		{
			Directory.CreateDirectory(_configDirectory);
		}
	}

	public void SaveConfigs(ObservableCollection<AIProviderConfig> configs)
	{
		try
		{
			string contents = JsonSerializer.Serialize(new AIConfigStorage
			{
				Version = 2,
				Configs = configs.Select((AIProviderConfig c) => CloneConfig(c)).ToList()
			}, new JsonSerializerOptions
			{
				WriteIndented = true
			});
			File.WriteAllText(_configFilePath, contents);
			Logger.Info($"[LocalAIConfig] 保存了 {configs.Count} 个AI配置（版本 {2}，明文存储）");
		}
		catch (Exception ex)
		{
			Logger.Error("[LocalAIConfig] 保存AI配置失败", ex);
			throw new Exception("保存AI配置失败: " + ex.Message, ex);
		}
	}

	public ObservableCollection<AIProviderConfig> LoadConfigs()
	{
		try
		{
			if (!File.Exists(_configFilePath))
			{
				Logger.Info("[LocalAIConfig] 配置文件不存在，返回空列表");
				return new ObservableCollection<AIProviderConfig>();
			}
			AIConfigStorage aIConfigStorage = JsonSerializer.Deserialize<AIConfigStorage>(File.ReadAllText(_configFilePath));
			if (aIConfigStorage == null || aIConfigStorage.Configs == null)
			{
				Logger.Warning("[LocalAIConfig] 配置文件格式无效，返回空列表");
				return new ObservableCollection<AIProviderConfig>();
			}
			if (aIConfigStorage.Version < 2)
			{
				Logger.Warning($"[LocalAIConfig] 检测到旧版本配置文件（版本 {aIConfigStorage.Version}），已不再支持。清除旧配置，请重新配置。");
				ClearAllConfigs();
				return new ObservableCollection<AIProviderConfig>();
			}
			ObservableCollection<AIProviderConfig> observableCollection = new ObservableCollection<AIProviderConfig>();
			foreach (AIProviderConfig config in aIConfigStorage.Configs)
			{
				if (string.IsNullOrEmpty(config.EncryptedApiKey))
				{
					Logger.Warning("[LocalAIConfig] 配置 " + config.DisplayName + " 的 API Key 为空，跳过");
					continue;
				}
				observableCollection.Add(config);
				int length = config.EncryptedApiKey.Length;
				Logger.Info($"[LocalAIConfig] 加载配置: {config.DisplayName}, API Key 长度: {length}");
			}
			Logger.Info($"[LocalAIConfig] 成功加载 {observableCollection.Count} 个AI配置");
			return observableCollection;
		}
		catch (Exception ex)
		{
			Logger.Error("[LocalAIConfig] 加载AI配置失败", ex);
			throw new Exception("加载AI配置失败: " + ex.Message, ex);
		}
	}

	public void ClearAllConfigs()
	{
		try
		{
			if (File.Exists(_configFilePath))
			{
				File.Delete(_configFilePath);
			}
		}
		catch (Exception ex)
		{
			throw new Exception("清除AI配置失败: " + ex.Message, ex);
		}
	}

	private AIProviderConfig CloneConfig(AIProviderConfig original)
	{
		return new AIProviderConfig
		{
			Id = original.Id,
			Provider = original.Provider,
			ModelName = original.ModelName,
			DisplayName = original.DisplayName,
			ProviderEndpoint = original.ProviderEndpoint,
			EncryptedApiKey = original.EncryptedApiKey,
			MaxTokens = original.MaxTokens,
			Temperature = original.Temperature,
			Priority = original.Priority,
			IsDefault = original.IsDefault,
			SupportsVision = original.SupportsVision
		};
	}

	public string GetConfigFilePath()
	{
		return _configFilePath;
	}

	public void SaveDefaultModelId(string modelId)
	{
		try
		{
			File.WriteAllText(_defaultModelPath, modelId);
		}
		catch (Exception ex)
		{
			Logger.Error("[LocalAIConfig] 保存默认模型 ID 失败", ex);
			throw new Exception("保存默认模型 ID 失败: " + ex.Message, ex);
		}
	}

	public string? GetDefaultModelId()
	{
		try
		{
			if (!File.Exists(_defaultModelPath))
			{
				return null;
			}
			return File.ReadAllText(_defaultModelPath);
		}
		catch (Exception ex)
		{
			Logger.Error("[LocalAIConfig] 读取默认模型 ID 失败", ex);
			return null;
		}
	}

	public void ClearDefaultModelId()
	{
		try
		{
			if (File.Exists(_defaultModelPath))
			{
				File.Delete(_defaultModelPath);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[LocalAIConfig] 清除默认模型 ID 失败", ex);
			throw new Exception("清除默认模型 ID 失败: " + ex.Message, ex);
		}
	}
}
