namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class ModelingProgress
{
	public string Stage { get; set; } = string.Empty;

	public int Percentage { get; set; }

	public string Message { get; set; } = string.Empty;

	public int CurrentItem { get; set; }

	public int TotalItems { get; set; }

	public bool IsCompleted { get; set; }

	public bool HasError { get; set; }

	public string? ErrorMessage { get; set; }

	public static ModelingProgress Create(string stage, int percentage, string message)
	{
		return new ModelingProgress
		{
			Stage = stage,
			Percentage = percentage,
			Message = message,
			IsCompleted = false,
			HasError = false
		};
	}

	public static ModelingProgress CreateItemProgress(string stage, int current, int total, string message)
	{
		return new ModelingProgress
		{
			Stage = stage,
			CurrentItem = current,
			TotalItems = total,
			Percentage = ((total > 0) ? ((int)((double)current * 100.0 / (double)total)) : 0),
			Message = message,
			IsCompleted = false,
			HasError = false
		};
	}

	public static ModelingProgress CreateCompleted(string message = "建模完成")
	{
		return new ModelingProgress
		{
			Stage = "完成",
			Percentage = 100,
			Message = message,
			IsCompleted = true,
			HasError = false
		};
	}

	public static ModelingProgress CreateError(string errorMessage)
	{
		return new ModelingProgress
		{
			Stage = "错误",
			Percentage = 0,
			Message = "建模失败",
			IsCompleted = false,
			HasError = true,
			ErrorMessage = errorMessage
		};
	}
}
