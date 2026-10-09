using System;
using RevitAi.Abstractions.Models;
using RevitAi.Abstractions.Revit;
using Autodesk.Revit.DB;

namespace RevitAi.Revit.Services;

public class SatelliteMapImportService : ISatelliteMapImportService
{
	public void PrepareImport()
	{
		SatelliteMapImportHelper.PrepareImport();
	}

	public void StartImport(BoundingBox boundingBox, string mapSource, int zoomLevel = 18, string? tiandituToken = null, string? googleMapsApiKey = null, Action? onCompleted = null)
	{
		SatelliteMapImportHelper.StartImport(boundingBox, mapSource, zoomLevel, tiandituToken, googleMapsApiKey, onCompleted);
	}

	public void Cleanup()
	{
		SatelliteMapImportHelper.Cleanup();
	}

	public GeoReferencePoint? GetReferencePoint(object document)
	{
		Document val = (Document)((document is Document) ? document : null);
		if (val != null)
		{
			return RevitSatelliteMapImporter.GetReferencePoint(val);
		}
		return null;
	}
}
