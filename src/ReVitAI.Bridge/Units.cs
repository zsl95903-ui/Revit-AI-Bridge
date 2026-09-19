using Autodesk.Revit.DB;

namespace ReVitAI.Bridge;

internal static class Units
{
    public const double MillimetersPerFoot = 304.8;
    public const double FeetPerMillimeter = 1.0 / MillimetersPerFoot;

    public static double MmToFeet(double millimeters) => millimeters * FeetPerMillimeter;
    public static double FeetToMm(double feet) => feet * MillimetersPerFoot;
    public static double CubicFeetToCubicMeters(double cubicFeet) => cubicFeet * 0.028316846592;
    public static XYZ Point(double xMm, double yMm, double zMm = 0) =>
        new(MmToFeet(xMm), MmToFeet(yMm), MmToFeet(zMm));

    public static XYZ Vector(double xMm, double yMm, double zMm = 0) => Point(xMm, yMm, zMm);

    public static object PointDto(XYZ point) => new
    {
        X = Math.Round(FeetToMm(point.X), 4),
        Y = Math.Round(FeetToMm(point.Y), 4),
        Z = Math.Round(FeetToMm(point.Z), 4),
    };
}
