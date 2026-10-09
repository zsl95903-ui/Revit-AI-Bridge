using System;

namespace RevitAi.Abstractions.Common;

public sealed class BoundingBox3D
{
	public Point3D Min { get; }

	public Point3D Max { get; }

	public Point3D Center => new Point3D((Min.X + Max.X) / 2.0, (Min.Y + Max.Y) / 2.0, (Min.Z + Max.Z) / 2.0);

	public Vector3D Size => new Vector3D(Max.X - Min.X, Max.Y - Min.Y, Max.Z - Min.Z);

	public BoundingBox3D(Point3D min, Point3D max)
	{
		Min = min ?? throw new ArgumentNullException("min");
		Max = max ?? throw new ArgumentNullException("max");
	}
}
