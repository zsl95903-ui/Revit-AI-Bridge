using System;
using RevitAi.Abstractions.Models;

namespace RevitAi.Abstractions.Revit;

public interface ISatelliteMapImportService
{
	void PrepareImport();

	void StartImport(BoundingBox boundingBox, string mapSource, int zoomLevel = 18, string? tiandituToken = null, string? googleMapsApiKey = null, Action? onCompleted = null);

	void Cleanup();

	GeoReferencePoint? GetReferencePoint(object document);
}
