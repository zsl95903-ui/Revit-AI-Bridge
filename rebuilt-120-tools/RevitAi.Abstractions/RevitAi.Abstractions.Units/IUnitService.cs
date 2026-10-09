namespace RevitAi.Abstractions.Units;

public interface IUnitService
{
	double MmToInternal(double millimeters);

	double InternalToMm(double internalValue);

	double CmToInternal(double centimeters);

	double InternalToCm(double internalValue);

	double MetersToInternal(double meters);

	double InternalToMeters(double internalValue);

	double SquareMetersToInternal(double squareMeters);

	double InternalToSquareMeters(double internalValue);

	double CubicMetersToInternal(double cubicMeters);

	double InternalToCubicMeters(double internalValue);

	double DegreesToRadians(double degrees);

	double RadiansToDegrees(double radians);

	string FormatValue(double value, UnitType unitType, bool useProjectUnits = true);
}
