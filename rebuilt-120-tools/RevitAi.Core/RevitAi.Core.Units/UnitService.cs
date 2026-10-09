using System;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Units;
using ns7;

namespace RevitAi.Core.Units;

public sealed class UnitService : IUnitService
{
	private const double double_0 = 304.8;

	private const double double_1 = 30.48;

	private const double double_2 = 0.3048;

	private const double double_3 = 0.09290304;

	private const double double_4 = 0.0283168466;

	private const double double_5 = 180.0 / Math.PI;

	private readonly IProjectUnitService iprojectUnitService_0;

	private readonly object object_0;

	public UnitService(IProjectUnitService projectUnitService, object document)
	{
		iprojectUnitService_0 = projectUnitService ?? throw new ArgumentNullException("projectUnitService");
		object_0 = document ?? throw new ArgumentNullException("document");
	}

	public double MmToInternal(double millimeters)
	{
		return millimeters / 304.8;
	}

	public double InternalToMm(double internalValue)
	{
		return internalValue * 304.8;
	}

	public double CmToInternal(double centimeters)
	{
		return centimeters / 30.48;
	}

	public double InternalToCm(double internalValue)
	{
		return internalValue * 30.48;
	}

	public double MetersToInternal(double meters)
	{
		return meters / 0.3048;
	}

	public double InternalToMeters(double internalValue)
	{
		return internalValue * 0.3048;
	}

	public double SquareMetersToInternal(double squareMeters)
	{
		return squareMeters / 0.09290304;
	}

	public double InternalToSquareMeters(double internalValue)
	{
		return internalValue * 0.09290304;
	}

	public double CubicMetersToInternal(double cubicMeters)
	{
		return cubicMeters / 0.0283168466;
	}

	public double InternalToCubicMeters(double internalValue)
	{
		return internalValue * 0.0283168466;
	}

	public double DegreesToRadians(double degrees)
	{
		return degrees * Math.PI / 180.0;
	}

	public double RadiansToDegrees(double radians)
	{
		return radians * (180.0 / Math.PI);
	}

	public string FormatValue(double value, UnitType unitType, bool useProjectUnits = true)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected I4, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected I4, but got Unknown
		try
		{
			if (useProjectUnits)
			{
				ProjectUnitInfo projectUnit = iprojectUnitService_0.GetProjectUnit(object_0, unitType);
				if (projectUnit != null)
				{
					return (((int)(unitType) - 1) switch
					{
						0 => projectUnit.IsMetric ? InternalToMm(value) : InternalToMeters(value), 
						1 => InternalToSquareMeters(value), 
						2 => InternalToCubicMeters(value), 
						3 => value, 
						_ => value, 
					}).ToString(projectUnit.FormatString) + " " + projectUnit.DisplayUnitSymbol;
				}
			}
			string result;
			switch ((int)(unitType) - 1)
			{
			default:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler5.AppendFormatted(value, "F2");
				result = defaultInterpolatedStringHandler5.ToStringAndClear();
				break;
			}
			case 0:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(3, 1);
				defaultInterpolatedStringHandler4.AppendFormatted(InternalToMm(value), "F2");
				defaultInterpolatedStringHandler4.AppendLiteral(" mm");
				result = defaultInterpolatedStringHandler4.ToStringAndClear();
				break;
			}
			case 1:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(3, 1);
				defaultInterpolatedStringHandler3.AppendFormatted(InternalToSquareMeters(value), "F2");
				defaultInterpolatedStringHandler3.AppendLiteral(" m²");
				result = defaultInterpolatedStringHandler3.ToStringAndClear();
				break;
			}
			case 2:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 1);
				defaultInterpolatedStringHandler2.AppendFormatted(InternalToCubicMeters(value), "F2");
				defaultInterpolatedStringHandler2.AppendLiteral(" m³");
				result = defaultInterpolatedStringHandler2.ToStringAndClear();
				break;
			}
			case 3:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
				defaultInterpolatedStringHandler.AppendFormatted(value, "F2");
				defaultInterpolatedStringHandler.AppendLiteral("°");
				result = defaultInterpolatedStringHandler.ToStringAndClear();
				break;
			}
			}
			return result;
		}
		catch (Exception ex)
		{
			Logger.Error("[UnitService] 格式化值失败: " + ex.Message);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler6.AppendFormatted(value, "F2");
			return defaultInterpolatedStringHandler6.ToStringAndClear();
		}
	}
}
