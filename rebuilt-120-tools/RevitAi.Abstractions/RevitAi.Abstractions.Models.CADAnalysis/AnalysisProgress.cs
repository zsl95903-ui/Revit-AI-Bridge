namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class AnalysisProgress
{
	public string Stage { get; set; } = string.Empty;

	public int Percentage { get; set; }

	public string Message { get; set; } = string.Empty;

	public bool IsCompleted { get; set; }

	public bool HasError { get; set; }

	public string? ErrorMessage { get; set; }

	public static AnalysisProgress Create(string stage, int percentage, string message)
	{
		return new AnalysisProgress
		{
			Stage = stage,
			Percentage = percentage,
			Message = message,
			IsCompleted = false,
			HasError = false
		};
	}

	public static AnalysisProgress CreateCompleted(string message = "分析完成")
	{
		return new AnalysisProgress
		{
			Stage = "完成",
			Percentage = 100,
			Message = message,
			IsCompleted = true,
			HasError = false
		};
	}

	public static AnalysisProgress CreateError(string errorMessage)
	{
		return new AnalysisProgress
		{
			Stage = "错误",
			Percentage = 0,
			Message = "分析失败",
			IsCompleted = false,
			HasError = true,
			ErrorMessage = errorMessage
		};
	}
}
