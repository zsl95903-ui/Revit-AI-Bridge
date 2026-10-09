using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_all_elements", Category = "元素查询", Description = "获取当前 Revit 文档的模型概览，统计各个类别的元素数量（不返回具体元素列表）", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetAllElementsTool : IAITool
{
	public string Name => "get_all_elements";

	public string Category => "元素查询";

	public string Description => "获取当前 Revit 文档的模型概览，统计各个类别的元素数量";

	public string ParametersSchema => "\r\n{\r\n    \"type\": \"object\",\r\n    \"properties\": {},\r\n    \"required\": []\r\n}";

	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken)
	{
		try
		{
			IRevitAdapter revitAdapter = context.RevitAdapter;
			IElementService val = ((revitAdapter != null) ? revitAdapter.ElementService : null);
			if (val == null)
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("无法获取 ElementService"));
			}
			if (context.Document == null)
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("无法获取当前文档"));
			}
			Logger.Debug("[GetAllElements] 开始获取模型概览，统计各类别元素数量");
			IEnumerable<object> allElements = val.GetAllElements(context.Document);
			if (allElements == null)
			{
				Logger.Warning("[GetAllElements] GetAllElements 返回 null");
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("无法获取文档元素列表"));
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			int num = 0;
			foreach (object item in allElements)
			{
				if (item != null && !val.IsElementType(item))
				{
					string text = val.GetElementCategory(item);
					if (string.IsNullOrEmpty(text))
					{
						text = "未分类";
					}
					string key = text ?? "未分类";
					if (dictionary.ContainsKey(key))
					{
						dictionary[key]++;
					}
					else
					{
						dictionary[key] = 1;
					}
					num++;
				}
			}
			var gparam_ = (from keyValuePair_0 in dictionary
				orderby keyValuePair_0.Value descending
				select new _003C_003Ef__AnonymousType143<string, int>(keyValuePair_0.Key, keyValuePair_0.Value)).ToList();
			if (num == 0)
			{
				Logger.Warning("[GetAllElements] 未找到任何模型元素");
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[GetAllElements] 统计完成，共 ");
			defaultInterpolatedStringHandler.AppendFormatted(dictionary.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个类别，");
			defaultInterpolatedStringHandler.AppendFormatted(num);
			defaultInterpolatedStringHandler.AppendLiteral(" 个模型元素");
			Logger.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(28, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("已获取模型概览：文档中共有 ");
			defaultInterpolatedStringHandler2.AppendFormatted(num);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个模型元素，分为 ");
			defaultInterpolatedStringHandler2.AppendFormatted(dictionary.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个类别");
			return Task.FromResult<AIToolResult>(AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class139<int, int, List<_003C_003Ef__AnonymousType143<string, int>>>(num, dictionary.Count, gparam_)));
		}
		catch (Exception ex)
		{
			Logger.Error("[GetAllElements] 执行失败", ex);
			return Task.FromResult<AIToolResult>(AIToolResult.Fail("获取模型概览失败: " + ex.Message));
		}
	}
}
