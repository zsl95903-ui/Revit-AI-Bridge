using System.Collections.Generic;
using Newtonsoft.Json;

namespace RevitAi.Abstractions.AI;

public sealed class ToolCallResult
{
	public bool Success { get; }

	public string? Message { get; }

	public object? Data { get; }

	public string? Error { get; }

	internal ToolCallResult(bool success, string? message, object? data, string? error)
	{
		Success = success;
		Message = message;
		Data = data;
		Error = error;
	}

	public T? AsData<T>()
	{
		if (Data == null)
		{
			return default(T);
		}
		return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(Data));
	}

	public Dictionary<string, object?>? AsDict()
	{
		return AsData<Dictionary<string, object>>();
	}

	public override string ToString()
	{
		if (!Success)
		{
			return "[失败] " + Error;
		}
		return "[成功] " + Message;
	}
}
