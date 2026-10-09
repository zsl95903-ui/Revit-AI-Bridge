using System;
using RevitAi.Core.Services;
using Autodesk.Revit.DB;

namespace RevitAi.Revit.Services;

public static class RevitGeoCoordinateExtensions
{
	public static XYZ? GeoToRevit(double longitude, double latitude, Document document)
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		try
		{
			ProjectLocation activeProjectLocation = document.ActiveProjectLocation;
			if (activeProjectLocation == null)
			{
				return null;
			}
			ProjectPosition projectPosition = activeProjectLocation.GetProjectPosition(XYZ.Zero);
			if (projectPosition == null)
			{
				return null;
			}
			double lonDelta = longitude - projectPosition.Angle;
			double latDelta = latitude - projectPosition.Elevation;
			double num = GeoCoordinateConverter.LonDeltaToMeters(latitude, lonDelta);
			double num2 = GeoCoordinateConverter.LatDeltaToMeters(latDelta);
			double num3 = num * 3.28084;
			double num4 = num2 * 3.28084;
			double angle = projectPosition.Angle;
			double num5 = num3 * Math.Cos(angle) - num4 * Math.Sin(angle);
			double num6 = num3 * Math.Sin(angle) + num4 * Math.Cos(angle);
			return new XYZ(num5, num6, projectPosition.Elevation);
		}
		catch (Exception)
		{
			return null;
		}
	}
}
