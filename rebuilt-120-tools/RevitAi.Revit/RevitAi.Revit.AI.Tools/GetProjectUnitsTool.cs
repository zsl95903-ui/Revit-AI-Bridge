using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Units;
using ns6;

namespace RevitAi.Revit.AI.Tools;

[AITool("get_project_units", Category = "项目设置", Description = "获取当前项目的所有单位设置")]
public sealed class GetProjectUnitsTool : IAITool
{
	public string Name => "get_project_units";

	public string Category => "项目设置";

	public string Description => "获取当前项目的所有单位设置";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {},\r\n        \"required\": []\r\n    }";

	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken)
	{
		try
		{
			if (context.Document == null)
			{
				Logger.Error("[GetProjectUnitsTool] 文档为 null");
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("文档不可用"));
			}
			if (context.ProjectUnitService == null)
			{
				Logger.Error("[GetProjectUnitsTool] ProjectUnitService 未注入");
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("项目单位服务不可用"));
			}
			Dictionary<UnitType, ProjectUnitInfo> projectUnits = context.ProjectUnitService.GetProjectUnits(context.Document);
			var list = projectUnits.Select(delegate(KeyValuePair<UnitType, ProjectUnitInfo> keyValuePair_0)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return new
				{
					unit_type = ((object)keyValuePair_0.Key/*cast due to constrained. prefix*/).ToString(),
					display_name = keyValuePair_0.Value.DisplayName,
					symbol = keyValuePair_0.Value.DisplayUnitSymbol,
					is_metric = keyValuePair_0.Value.IsMetric,
					precision = keyValuePair_0.Value.Precision,
					format_string = keyValuePair_0.Value.FormatString
				};
			}).ToList();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[GetProjectUnitsTool] 成功获取 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个单位类型");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return Task.FromResult<AIToolResult>(AIToolResult.Ok("成功获取项目单位设置", (object)list));
		}
		catch (Exception ex)
		{
			Logger.Error("[GetProjectUnitsTool] 执行失败", ex);
			return Task.FromResult<AIToolResult>(AIToolResult.Fail("获取项目单位失败: " + ex.Message));
		}
	}
}
