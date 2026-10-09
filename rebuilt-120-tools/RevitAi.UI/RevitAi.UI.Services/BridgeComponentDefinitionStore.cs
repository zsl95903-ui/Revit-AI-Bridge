using System;
using System.Collections.Generic;
using System.Linq;
using RevitAi.Abstractions.Logging;
using RevitAi.UI.Models;

namespace RevitAi.UI.Services;

public sealed class BridgeComponentDefinitionStore
{
	private static readonly Lazy<BridgeComponentDefinitionStore> _instance = new Lazy<BridgeComponentDefinitionStore>(() => new BridgeComponentDefinitionStore());

	private readonly Dictionary<string, List<UserDefinedBridgeComponent>> _storage = new Dictionary<string, List<UserDefinedBridgeComponent>>();

	public static BridgeComponentDefinitionStore Instance => _instance.Value;

	private BridgeComponentDefinitionStore()
	{
		Logger.Info("[BridgeComponentDefinitionStore] 初始化内存存储");
	}

	public void SetComponents(string componentType, List<UserDefinedBridgeComponent> components)
	{
		if (string.IsNullOrWhiteSpace(componentType))
		{
			Logger.Warning("[BridgeComponentDefinitionStore] 部件类型名称为空，无法保存");
			return;
		}
		_storage[componentType] = new List<UserDefinedBridgeComponent>(components);
		Logger.Info($"[BridgeComponentDefinitionStore] 已保存 {componentType}: {components.Count} 个部件");
	}

	public List<UserDefinedBridgeComponent> GetComponents(string componentType)
	{
		if (string.IsNullOrWhiteSpace(componentType))
		{
			Logger.Warning("[BridgeComponentDefinitionStore] 部件类型名称为空，无法获取");
			return new List<UserDefinedBridgeComponent>();
		}
		if (_storage.TryGetValue(componentType, out List<UserDefinedBridgeComponent> value))
		{
			Logger.Debug($"[BridgeComponentDefinitionStore] 获取 {componentType}: {value.Count} 个部件");
			return new List<UserDefinedBridgeComponent>(value);
		}
		Logger.Debug("[BridgeComponentDefinitionStore] " + componentType + " 尚未定义任何部件");
		return new List<UserDefinedBridgeComponent>();
	}

	public BridgeComponentDefinitions GetAllDefinitions()
	{
		BridgeComponentDefinitions bridgeComponentDefinitions = new BridgeComponentDefinitions();
		foreach (KeyValuePair<string, List<UserDefinedBridgeComponent>> item in _storage)
		{
			string key = item.Key;
			List<UserDefinedBridgeComponent> value = item.Value;
			switch (key)
			{
			case "桩部件":
				bridgeComponentDefinitions.Piles.Clear();
				foreach (UserDefinedBridgeComponent item2 in value)
				{
					bridgeComponentDefinitions.Piles.Add(item2);
				}
				break;
			case "基础部件":
				bridgeComponentDefinitions.Foundations.Clear();
				foreach (UserDefinedBridgeComponent item3 in value)
				{
					bridgeComponentDefinitions.Foundations.Add(item3);
				}
				break;
			case "墩柱":
				bridgeComponentDefinitions.Piers.Clear();
				foreach (UserDefinedBridgeComponent item4 in value)
				{
					bridgeComponentDefinitions.Piers.Add(item4);
				}
				break;
			case "盖梁":
				bridgeComponentDefinitions.Beams.Clear();
				foreach (UserDefinedBridgeComponent item5 in value)
				{
					bridgeComponentDefinitions.Beams.Add(item5);
				}
				break;
			case "支座":
				bridgeComponentDefinitions.Bearings.Clear();
				foreach (UserDefinedBridgeComponent item6 in value)
				{
					bridgeComponentDefinitions.Bearings.Add(item6);
				}
				break;
			case "桥型":
				bridgeComponentDefinitions.BridgeTypes.Clear();
				foreach (UserDefinedBridgeComponent item7 in value)
				{
					bridgeComponentDefinitions.BridgeTypes.Add(item7);
				}
				break;
			default:
				Logger.Warning("[BridgeComponentDefinitionStore] 未知的部件类型: " + key);
				break;
			}
		}
		Logger.Info($"[BridgeComponentDefinitionStore] 获取所有定义: 总计 {bridgeComponentDefinitions.TotalCount} 个部件");
		return bridgeComponentDefinitions;
	}

	public void ClearComponents(string componentType)
	{
		if (!string.IsNullOrWhiteSpace(componentType) && _storage.ContainsKey(componentType))
		{
			int count = _storage[componentType].Count;
			_storage.Remove(componentType);
			Logger.Info($"[BridgeComponentDefinitionStore] 已清除 {componentType}: {count} 个部件");
		}
	}

	public void ClearAll()
	{
		int value = _storage.Values.Sum((List<UserDefinedBridgeComponent> list) => list.Count);
		_storage.Clear();
		Logger.Info($"[BridgeComponentDefinitionStore] 已清除所有定义: 总计 {value} 个部件");
	}

	public List<string> GetDefinedTypes()
	{
		return _storage.Keys.ToList();
	}

	public int GetCount(string componentType)
	{
		if (_storage.TryGetValue(componentType, out List<UserDefinedBridgeComponent> value))
		{
			return value.Count;
		}
		return 0;
	}

	public int GetTotalCount()
	{
		return _storage.Values.Sum((List<UserDefinedBridgeComponent> list) => list.Count);
	}
}
