using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface IAnnotationService
{
	object? CreateDimension(object document, object view, IList<object> references, object linePosition, int? dimensionTypeId = null);

	object? CreateDimensionByPoints(object document, object view, IList<object> points, object linePosition, int? dimensionTypeId = null);

	object? CreateDimensionByElements(object document, object view, IList<int> elementIds, object dimensionLineOrigin, object dimensionDirection, int? dimensionTypeId = null, double? offset = null, string position = "auto", double? autoOffsetDistance = null);

	object? CreateTextNote(object document, object view, string text, object position, int? textTypeId = null);

	object? CreateTextNote(object document, object view, string text, object position, int? textTypeId = null, bool addLeader = false);

	IEnumerable<object> GetAllDimensions(object document);

	IEnumerable<object> GetAllTextNotes(object document);

	object? CreateTag(object document, object view, object element, int tagTypeId, object position);

	IEnumerable<object> GetAllTags(object document);

	int? GetTaggedElementId(object tag);

	IEnumerable<object> GetTagsInView(object document, object view);

	IList<int> GetTaggedElementIds(object tag);

	object? CreateTag(object document, object view, object element, object position, int tagTypeId = 0, bool addLeader = false);

	object? CreateRevisionCloud(object document, object view, IList<object> points, int revisionId);

	object? CreateAngularDimension(object document, object view, int element1Id, int element2Id, object arcPosition, int? dimensionTypeId = null);

	object? CreateSpotDimension(object document, object view, int elementId, object point, string @operator, object? bend = null, object? end = null, bool hasLeader = true, int? spotDimensionTypeId = null);
}
