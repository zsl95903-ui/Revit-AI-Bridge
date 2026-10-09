using System;

namespace RevitAi.Abstractions.Common;

public sealed class Vector3D
{
	public static readonly Vector3D Zero = new Vector3D(0.0, 0.0, 0.0);

	public static readonly Vector3D BasisX = new Vector3D(1.0, 0.0, 0.0);

	public static readonly Vector3D BasisY = new Vector3D(0.0, 1.0, 0.0);

	public static readonly Vector3D BasisZ = new Vector3D(0.0, 0.0, 1.0);

	public double X { get; }

	public double Y { get; }

	public double Z { get; }

	public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

	public Vector3D(double x, double y, double z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public Vector3D Normalize()
	{
		double length = Length;
		if (length < 1E-10)
		{
			return Zero;
		}
		return new Vector3D(X / length, Y / length, Z / length);
	}

	public override string ToString()
	{
		return $"[{X:F3}, {Y:F3}, {Z:F3}]";
	}

	public override bool Equals(object? obj)
	{
		if (obj is Vector3D vector3D)
		{
			if (X == vector3D.X && Y == vector3D.Y)
			{
				return Z == vector3D.Z;
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
