using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ns7;

namespace RevitAi.Core.Authentication.Models;

public static class FeatureGroupConfig
{
	public static readonly HashSet<string> ValidGroupIdentifiers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		"core",
		"ai",
		"room",
		"grid",
		"structure",
		"mep",
		"dimension",
		"view",
		"family",
		"batch",
		"data",
		"tool",
		"infrastructure"
	};

	public static readonly Dictionary<string, int> DefaultTrialCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
	{
		{
			"ai",
			5
		},
		{
			"room",
			10
		},
		{
			"grid",
			10
		},
		{
			"structure",
			10
		},
		{
			"mep",
			5
		},
		{
			"dimension",
			10
		},
		{
			"view",
			5
		},
		{
			"family",
			10
		},
		{
			"batch",
			5
		},
		{
			"data",
			5
		},
		{
			"tool",
			10
		},
		{
			"infrastructure",
			5
		}
	};

	public static bool IsValidGroupIdentifier(string groupIdentifier)
	{
		if (string.IsNullOrEmpty(groupIdentifier))
		{
			return false;
		}
		return ValidGroupIdentifiers.Contains(groupIdentifier);
	}

	public static int GetDefaultTrialCount(string groupIdentifier)
	{
		if (string.IsNullOrEmpty(groupIdentifier))
		{
			return 10;
		}
		if (DefaultTrialCounts.TryGetValue(groupIdentifier, out var value))
		{
			return value;
		}
		return 10;
	}

	public static (bool isValid, List<string> errors) ValidateConfiguration(List<TrialLimitConfig> configs)
	{
		List<string> list = new List<string>();
		if (configs != null && configs.Count != 0)
		{
			foreach (TrialLimitConfig config in configs)
			{
				if (config.IsGroupConfig && !string.IsNullOrEmpty(config.FeatureGroup) && !IsValidGroupIdentifier(config.FeatureGroup))
				{
					list.Add("无效的功能组: '" + config.FeatureGroup + "'");
				}
				if (config.IsFeatureConfig && !string.IsNullOrEmpty(config.FeatureGroup) && !IsValidGroupIdentifier(config.FeatureGroup))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 2);
					defaultInterpolatedStringHandler.AppendLiteral("功能 '");
					defaultInterpolatedStringHandler.AppendFormatted(config.FeatureId);
					defaultInterpolatedStringHandler.AppendLiteral("' 引用了无效的功能组: '");
					defaultInterpolatedStringHandler.AppendFormatted(config.FeatureGroup);
					defaultInterpolatedStringHandler.AppendLiteral("'");
					list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			return (isValid: list.Count == 0, errors: list);
		}
		list.Add("配置列表为空");
		return (isValid: false, errors: list);
	}

	public static string GetConfigurationSummary()
	{
		string text = "功能组配置摘要:\n";
		foreach (string item in ValidGroupIdentifiers.OrderBy((string string_0) => string_0))
		{
			GetDefaultTrialCount(item);
			int value = (DefaultTrialCounts.TryGetValue(item, out var value2) ? value2 : 10);
			string text2 = text;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2);
			defaultInterpolatedStringHandler.AppendLiteral("  - ");
			defaultInterpolatedStringHandler.AppendFormatted(item);
			defaultInterpolatedStringHandler.AppendLiteral(": 默认 ");
			defaultInterpolatedStringHandler.AppendFormatted(value);
			defaultInterpolatedStringHandler.AppendLiteral(" 次\n");
			text = text2 + defaultInterpolatedStringHandler.ToStringAndClear();
		}
		return text;
	}
}
