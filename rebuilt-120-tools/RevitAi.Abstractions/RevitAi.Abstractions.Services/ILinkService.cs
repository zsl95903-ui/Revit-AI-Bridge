using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface ILinkService
{
	IEnumerable<object> GetAllLinks(object document);

	IEnumerable<object> GetLinks(object document);

	object? LoadLink(object document, string filePath, string? linkTypeName = null);

	object? LoadLink(object document, string filePath, int linkTypeId, object? position = null);

	bool UnloadLink(object document, int linkId);

	bool ReloadLink(object document, int linkId);

	(string? FilePath, string? LinkType, bool IsLoaded, int? LinkTypeId)? GetLinkInfo(object linkElement);

	string? GetLinkName(object linkElement);

	string? GetLinkPath(object linkElement);

	string? GetLinkType(object linkElement);

	bool IsLinkLoaded(object linkElement);

	(double X, double Y, double Z)? GetLinkPosition(object linkElement);

	double? GetLinkRotation(object linkElement);

	bool UpdateLinkPosition(object document, object linkElement, double x, double y, double z);

	bool UpdateLinkRotation(object document, object linkElement, double rotationAngle);

	bool DeleteLink(object document, object linkElement);

	int LoadLinks(object document, IEnumerable<string> filePaths);

	IEnumerable<object> GetAllLinkInstances(object document);

	object? GetLinkTypeFromInstance(object linkInstance);

	bool ReplaceLinkPath(object document, object linkInstance, string newFilePath);

	object? GetLinkDocument(object linkInstance);
}
