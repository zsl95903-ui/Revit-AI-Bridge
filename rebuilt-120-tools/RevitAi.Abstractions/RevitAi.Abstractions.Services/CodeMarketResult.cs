namespace RevitAi.Abstractions.Services;

public sealed class CodeMarketResult<T>
{
	public bool Success { get; set; }

	public T? Data { get; set; }

	public string? Error { get; set; }

	public string? Message { get; set; }

	public static CodeMarketResult<T> Ok(T data, string? message = null)
	{
		return new CodeMarketResult<T>
		{
			Success = true,
			Data = data,
			Message = message
		};
	}

	public static CodeMarketResult<T> Fail(string error)
	{
		return new CodeMarketResult<T>
		{
			Success = false,
			Error = error
		};
	}
}
