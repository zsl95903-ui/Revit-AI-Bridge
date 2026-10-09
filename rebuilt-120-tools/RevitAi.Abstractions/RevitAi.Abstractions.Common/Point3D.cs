using System;

namespace RevitAi.Abstractions.Common;

public sealed class Point3D
{
	public static readonly Point3D Origin = new Point3D(0.0, 0.0, 0.0);

	public double X { get; }

	public double Y { get; }

	public double Z { get; }

	public Point3D(double x, double y, double z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public double DistanceTo(Point3D other)
	{
		double num = X - other.X;
		double num2 = Y - other.Y;
		double num3 = Z - other.Z;
		return Math.Sqrt(num * num + num2 * num2 + num3 * num3);
	}

	public override string ToString()
	{
		return $"({X:F3}, {Y:F3}, {Z:F3})";
	}

	public override bool Equals(object? obj)
	{
		if (obj is Point3D point3D)
		{
			if (X == point3D.X && Y == point3D.Y)
			{
				return Z == point3D.Z;
			}
			return false;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((17 * 31 + X.GetHashCode()) * 31 + Y.GetHashCode()) * 31 + Z.GetHashCode();
	}
}
