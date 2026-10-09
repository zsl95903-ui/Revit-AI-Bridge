using System;
using Newtonsoft.Json;
using ns7;

namespace RevitAi.Core.Authentication.Models;

public class PostgresNumericConverter : JsonConverter<decimal>
{
	public override decimal ReadJson(JsonReader reader, Type objectType, decimal existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		if ((int)reader.TokenType == 11)
		{
			return 0m;
		}
		object obj;
		if ((int)reader.TokenType != 7 && (int)reader.TokenType != 8)
		{
			if ((int)reader.TokenType == 9)
			{
				object value = reader.Value;
				if (value == null)
				{
					obj = null;
				}
				else
				{
					obj = value.ToString();
					if (obj != null)
					{
						goto IL_004e;
					}
				}
				obj = "0";
				goto IL_004e;
			}
			goto IL_0059;
		}
		return Convert.ToDecimal(reader.Value);
		IL_004e:
		if (decimal.TryParse((string?)obj, out var result))
		{
			return result;
		}
		goto IL_0059;
		IL_0059:
		if (reader.Value != null)
		{
			string text = reader.Value.ToString();
			if (!string.IsNullOrEmpty(text) && decimal.TryParse(text, out var result2))
			{
				return result2;
			}
		}
		return 0m;
	}

	public override void WriteJson(JsonWriter writer, decimal value, JsonSerializer serializer)
	{
		writer.WriteValue(value);
	}
}
