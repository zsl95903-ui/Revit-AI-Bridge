using System;

namespace RevitAi.Abstractions.Services;

public sealed class CADLineInfo
{
	public string Id { get; set; } = Guid.NewGuid().ToString();

	public double StartX { get; set; }

	public double StartY { get; set; }

	public double StartZ { get; set; }

	public double EndX { get; set; }

	public double EndY { get; set; }

	public double EndZ { get; set; }

	public string LayerName { get; set; } = string.Empty;

	public string LineType { get; set; } = string.Empty;

	public double Length
	{
		get
		{
			double num = EndX - StartX;
			double num2 = EndY - StartY;
			double num3 = EndZ - StartZ;
			return Math.Sqrt(num * num + num2 * num2 + num3 * num3);
		}
	}

	public double DirectionAngle
	{
		get
		{
			double x = EndX - StartX;
			return Math.Atan2(EndY - StartY, x);
		}
	}

	public object? GeometryObject { get; set; }

	public string? BlockName { get; set; }

	public (double X, double Y, double Z) GetStartPoint()
	{
		return (X: StartX, Y: StartY, Z: StartZ);
	}

	public (double X, double Y, double Z) GetEndPoint()
	{
		return (X: EndX, Y: EndY, Z: EndZ);
	}

	public double DistanceToPoint(double x, double y, double z = 0.0)
	{
		double num = EndX - StartX;
		double num2 = EndY - StartY;
		double num3 = EndZ - StartZ;
		double num4 = num * num + num2 * num2 + num3 * num3;
		if (num4 == 0.0)
		{
			double num5 = x - StartX;
			double num6 = y - StartY;
			double num7 = z - StartZ;
			return Math.Sqrt(num5 * num5 + num6 * num6 + num7 * num7);
		}
		double val = ((x - StartX) * num + (y - StartY) * num2 + (z - StartZ) * num3) / num4;
		val = Math.Max(0.0, Math.Min(1.0, val));
		double num8 = StartX + val * num;
		double num9 = StartY + val * num2;
		double num10 = StartZ + val * num3;
		double num11 = x - num8;
		double num12 = y - num9;
		double num13 = z - num10;
		return Math.Sqrt(num11 * num11 + num12 * num12 + num13 * num13);
	}

	public bool IsConnectedTo(CADLineInfo other, double tolerance = 10.0)
	{
		if (DistanceToPoint(other.StartX, other.StartY, other.StartZ) <= tolerance)
		{
			return true;
		}
		if (DistanceToPoint(other.EndX, other.EndY, other.EndZ) <= tolerance)
		{
			return true;
		}
		if (other.DistanceToPoint(StartX, StartY, StartZ) <= tolerance)
		{
			return true;
		}
		if (other.DistanceToPoint(EndX, EndY, EndZ) <= tolerance)
		{
			return true;
		}
		return false;
	}
}
