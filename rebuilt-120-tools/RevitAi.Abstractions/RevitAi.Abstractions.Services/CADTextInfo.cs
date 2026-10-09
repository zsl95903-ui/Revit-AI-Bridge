using System;

namespace RevitAi.Abstractions.Services;

public sealed class CADTextInfo
{
	public string Id { get; set; } = Guid.NewGuid().ToString();

	public string Content { get; set; } = string.Empty;

	public double X { get; set; }

	public double Y { get; set; }

	public double Z { get; set; }

	public string LayerName { get; set; } = string.Empty;

	public double? Height { get; set; }

	public double? Width { get; set; }

	public double? Rotation { get; set; }

	public string? HorizontalAlignment { get; set; } = "Left";

	public string? VerticalAlignment { get; set; } = "Baseline";

	public double RotationAngle => Rotation.GetValueOrDefault();

	public double TextHeight => Height.GetValueOrDefault();

	public double TextWidth => Width ?? Height.GetValueOrDefault();

	public object? GeometryObject { get; set; }

	public string? BlockName { get; set; }

	public (double X, double Y, double Z) GetPosition()
	{
		return (X: X, Y: Y, Z: Z);
	}

	public (double X, double Y, double Z) GetCenterPoint()
	{
		double item = X;
		double item2 = Y;
		double valueOrDefault = Width.GetValueOrDefault();
		double valueOrDefault2 = Height.GetValueOrDefault();
		string? obj = HorizontalAlignment ?? "Left";
		string text = VerticalAlignment ?? "Baseline";
		switch (obj.ToLower())
		{
		case "left":
		case "baseline":
		case "aligned":
		case "fit":
			item = X + valueOrDefault / 2.0;
			break;
		case "center":
			item = X;
			break;
		case "right":
			item = X - valueOrDefault / 2.0;
			break;
		}
		switch (text.ToLower())
		{
		case "baseline":
		case "bottom":
			item2 = Y + valueOrDefault2 / 2.0;
			break;
		case "middle":
		case "center":
			item2 = Y;
			break;
		case "top":
			item2 = Y - valueOrDefault2 / 2.0;
			break;
		}
		return (X: item, Y: item2, Z: 0.0);
	}

	public double DistanceToPoint(double x, double y, double z = 0.0)
	{
		double num = x - X;
		double num2 = y - Y;
		double num3 = z - Z;
		return Math.Sqrt(num * num + num2 * num2 + num3 * num3);
	}

	public double DistanceFromCenterToPoint(double x, double y, double z = 0.0)
	{
		(double, double, double) centerPoint = GetCenterPoint();
		double num = x - centerPoint.Item1;
		double num2 = y - centerPoint.Item2;
		double num3 = z - centerPoint.Item3;
		return Math.Sqrt(num * num + num2 * num2 + num3 * num3);
	}

	public bool IsParallelTo(double angle, double toleranceDegrees = 15.0)
	{
		double num = toleranceDegrees * Math.PI / 180.0;
		double num2;
		for (num2 = Math.Abs(RotationAngle - angle); num2 > Math.PI; num2 -= Math.PI)
		{
		}
		if (!(num2 <= num))
		{
			return num2 >= Math.PI - num;
		}
		return true;
	}

	public double CalculatePerpendicularDistanceToLine(double lineStartX, double lineStartY, double lineStartZ, double lineEndX, double lineEndY, double lineEndZ)
	{
		(double, double, double) centerPoint = GetCenterPoint();
		double num = lineEndX - lineStartX;
		double num2 = lineEndY - lineStartY;
		double num3 = lineEndZ - lineStartZ;
		double num4 = num * num + num2 * num2 + num3 * num3;
		if (num4 == 0.0)
		{
			double num5 = centerPoint.Item1 - lineStartX;
			double num6 = centerPoint.Item2 - lineStartY;
			double num7 = centerPoint.Item3 - lineStartZ;
			return Math.Sqrt(num5 * num5 + num6 * num6 + num7 * num7);
		}
		double num8 = centerPoint.Item1 - lineStartX;
		double num9 = centerPoint.Item2 - lineStartY;
		double num10 = centerPoint.Item3 - lineStartZ;
		double num11 = num2 * num10 - num3 * num9;
		double num12 = num3 * num8 - num * num10;
		double num13 = num * num9 - num2 * num8;
		return Math.Sqrt(num11 * num11 + num12 * num12 + num13 * num13) / Math.Sqrt(num4);
	}
}
