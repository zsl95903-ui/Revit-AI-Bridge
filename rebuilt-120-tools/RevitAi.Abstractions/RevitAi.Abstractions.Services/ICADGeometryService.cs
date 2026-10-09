using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface ICADGeometryService
{
	IEnumerable<object> GetCADImportInstances(object document);

	IEnumerable<object> GetGeometryObjects(object importInstance);

	string? GetGeometryObjectType(object geometryObject);

	string? GetGeometryObjectLayer(object geometryObject, object document);

	(string? content, object? position) GetTextInfo(object textObject);

	(object? start, object? end) GetCurveEndpoints(object curveObject);

	(string? name, object? position) GetBlockInfo(object instanceObject);

	IEnumerable<object> FilterByLayer(IEnumerable<object> geometryObjects, string layerName, object document);

	Dictionary<string, List<object>> GroupByType(IEnumerable<object> geometryObjects);

	List<CADTextInfo> GetTextsByExploding(object importInstance, object document);

	(double X, double Y, double Z)? GetCADImportPosition(object importInstance);

	double? GetCADImportRotation(object importInstance);

	int? GetCADImportOwnerViewId(object importInstance);

	int? GetCADImportLevelId(object importInstance);
}
