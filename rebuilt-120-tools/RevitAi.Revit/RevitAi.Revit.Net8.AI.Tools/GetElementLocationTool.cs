using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_element_location", Category = "元素查询", Description = "获取元素的位置坐标。返回单位：毫米", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetElementLocationTool : IAITool
{
	private readonly IElementService ielementService_0;

	public string Name => "get_element_location";

	public string Category => "元素查询";

	public string Description => "获取元素的位置坐标。返回单位：毫米";

	public string ParametersSchema => "\r\n{\r\n    \"type\": \"object\",\r\n    \"properties\": {\r\n        \"element_id\": {\r\n            \"type\": \"integer\",\r\n            \"description\": \"元素 ID\"\r\n        }\r\n    },\r\n    \"required\": [\"element_id\"]\r\n}";

	public GetElementLocationTool(IElementService elementService)
	{
		ielementService_0 = elementService ?? throw new ArgumentNullException("elementService");
	}

	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken)
	{
		try
		{
			if (context.Document == null)
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("无法获取当前文档"));
			}
			if (!context.Parameters.TryGetValue("element_id", out var value) || value == null)
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("缺少必需参数: element_id"));
			}
			if (!int.TryParse(value.ToString(), out var result))
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("element_id 参数格式错误"));
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[GetElementLocation] 获取元素位置，ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(result);
			Logger.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			object elementById = ielementService_0.GetElementById(context.Document, result);
			if (elementById == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("未找到 ID 为 ");
				defaultInterpolatedStringHandler2.AppendFormatted(result);
				defaultInterpolatedStringHandler2.AppendLiteral(" 的元素");
				return Task.FromResult<AIToolResult>(AIToolResult.Fail(defaultInterpolatedStringHandler2.ToStringAndClear()));
			}
			(double, double, double)? elementLocation = ielementService_0.GetElementLocation(elementById);
			if (!elementLocation.HasValue)
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("该元素没有位置信息"));
			}
			string elementName = ielementService_0.GetElementName(elementById);
			Class35<double, double, double, string> gparam_ = new Class35<double, double, double, string>(Math.Round(elementLocation.Value.Item1 * 304.8, 0), Math.Round(elementLocation.Value.Item2 * 304.8, 0), Math.Round(elementLocation.Value.Item3 * 304.8, 0), "millimeters");
			return Task.FromResult<AIToolResult>(AIToolResult.Ok("成功获取元素位置: " + (elementName ?? "未命名") + "（单位：毫米）", (object)new Class174<int, string, Class35<double, double, double, string>>(result, elementName, gparam_)));
		}
		catch (Exception ex)
		{
			Logger.Error("[GetElementLocation] 执行失败", ex);
			return Task.FromResult<AIToolResult>(AIToolResult.Fail("获取元素位置失败: " + ex.Message));
		}
	}
}
