using System.Collections.Generic;

namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class PipeNetworkModelingResult
{
	public bool Success { get; set; }

	public int CreatedManholesCount { get; set; }

	public int CreatedPipesCount { get; set; }

	public List<string> Warnings { get; set; } = new List<string>();

	public List<string> Errors { get; set; } = new List<string>();

	public List<string> MissingDataReport { get; set; } = new List<string>();

	public long ElapsedMilliseconds { get; set; }

	public string? ErrorMessage { get; set; }

	public string GetSummary()
	{
		if (!Success)
		{
			return "建模失败: " + ErrorMessage;
		}
		string text = $"建模完成，耗时 {ElapsedMilliseconds} ms\n";
		text += $"创建管井: {CreatedManholesCount} 个\n";
		text += $"创建管道: {CreatedPipesCount} 个\n";
		if (Warnings.Count > 0)
		{
			text += $"\n警告 ({Warnings.Count} 条):\n";
			foreach (string warning in Warnings)
			{
				text = text + "  - " + warning + "\n";
			}
		}
		if (MissingDataReport.Count > 0)
		{
			text += $"\n缺失数据 ({MissingDataReport.Count} 条):\n";
			foreach (string item in MissingDataReport)
			{
				text = text + "  - " + item + "\n";
			}
		}
		return text;
	}

	public static PipeNetworkModelingResult CreateFailure(string errorMessage)
	{
		return new PipeNetworkModelingResult
		{
			Success = false,
			ErrorMessage = errorMessage
		};
	}

	public static PipeNetworkModelingResult CreateSuccess(int manholesCount, int pipesCount, long elapsedMs)
	{
		return new PipeNetworkModelingResult
		{
			Success = true,
			CreatedManholesCount = manholesCount,
			CreatedPipesCount = pipesCount,
			ElapsedMilliseconds = elapsedMs
		};
	}
}
