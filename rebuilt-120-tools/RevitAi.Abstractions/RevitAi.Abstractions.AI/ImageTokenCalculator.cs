using System;

namespace RevitAi.Abstractions.AI;

public static class ImageTokenCalculator
{
	public static int EstimateTokensFromDimensions(int width, int height)
	{
		if (width <= 0 || height <= 0)
		{
			return 0;
		}
		long num = (((long)width * (long)height >= 147456) ? 640000 : 147456);
		double a = Math.Sqrt(num) / 512.0 * 170.0;
		return (int)Math.Min(384.0, Math.Ceiling(a));
	}

	public static int EstimateTokensFromBytes(byte[] imageBytes, int width = 0, int height = 0)
	{
		if (width > 0 && height > 0)
		{
			return EstimateTokensFromDimensions(width, height);
		}
		if (imageBytes == null || imageBytes.Length == 0)
		{
			return 0;
		}
		int num = (int)Math.Sqrt(imageBytes.Length * 10);
		return EstimateTokensFromDimensions(num, num);
	}

	public static int EstimateTokensFromText(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < text.Length; i++)
		{
			num = ((!IsChineseCharacter(text[i])) ? (num + 1) : (num + 2));
		}
		return (int)Math.Ceiling((double)num / 3.0);
	}

	private static bool IsChineseCharacter(char c)
	{
		if ((c < '一' || c > '鿿') && (c < '㐀' || c > '䶿') && (c < 131072 || c > 173791) && (c < '\u3000' || c > '〿'))
		{
			if (c >= '\uff00')
			{
				return c <= '\uffef';
			}
			return false;
		}
		return true;
	}

	public static string GetImageInfoString(int width, int height)
	{
		int value = EstimateTokensFromDimensions(width, height);
		return $"{width}×{height}px, 估算 {value} tokens";
	}
}
