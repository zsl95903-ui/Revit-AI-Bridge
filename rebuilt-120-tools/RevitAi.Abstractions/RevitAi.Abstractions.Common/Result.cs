namespace RevitAi.Abstractions.Common;

public sealed class Result<T>
{
	public bool IsSuccess { get; }

	public bool IsFailure => !IsSuccess;

	public T? Value { get; }

	public string? Error { get; }

	private Result(bool isSuccess, T? value, string? error)
	{
		IsSuccess = isSuccess;
		Value = value;
		Error = error;
	}

	public static Result<T> Success(T value)
	{
		return new Result<T>(isSuccess: true, value, null);
	}

	public static Result<T> Failure(string error)
	{
		return new Result<T>(isSuccess: false, default(T), error);
	}

	public static implicit operator Result<T>(T value)
	{
		return Success(value);
	}

	public override string ToString()
	{
		if (!IsSuccess)
		{
			return "Failure: " + Error;
		}
		return $"Success: {Value}";
	}
}
public sealed class Result
{
	public bool IsSuccess { get; }

	public bool IsFailure => !IsSuccess;

	public string? Error { get; }

	private Result(bool isSuccess, string? error)
	{
		IsSuccess = isSuccess;
		Error = error;
	}

	public static Result Success()
	{
		return new Result(isSuccess: true, null);
	}

	public static Result Failure(string error)
	{
		return new Result(isSuccess: false, error);
	}

	public override string ToString()
	{
		if (!IsSuccess)
		{
			return "Failure: " + Error;
		}
		return "Success";
	}
}
