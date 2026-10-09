using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface IViewService
{
	object? GetActiveView(object document);

	IEnumerable<object> GetAllViews(object document);

	string? GetViewName(object view);

	string? GetViewType(object view);

	int? GetViewId(object view);

	object? CreateFloorPlan(object document, int levelId, string? viewName = null);

	object? CreateCeilingPlan(object document, int levelId, string? viewName = null);

	object? CreateElevationView(object document, double x, double y, string? viewName = null);

	bool ActivateView(object document, object view);

	bool ZoomToElements(object document, IEnumerable<int> elementIds, object? view = null);

	bool SetViewTemplate(object document, object view, string templateName);

	object? Create3DView(object document, string? viewName = null);

	bool SetSectionBox(object document, object view3D, double minX, double minY, double minZ, double maxX, double maxY, double maxZ);

	bool ApplyViewFilter(object document, object view, string filterName);

	bool RemoveViewFilter(object document, object view, string filterName);

	object? DuplicateView(object document, object sourceView, string newViewName, string duplicateOptions = "WithDetailing");

	bool OverrideElementColor(object document, object view, int elementId, object color);

	bool OverrideElementColor(object document, object view, int elementId, object color, int? patternId);

	bool OverrideElementColors(object document, object view, IEnumerable<int> elementIds, object color, int? transparency = null);

	bool ClearElementOverrides(object document, object view, IEnumerable<int> elementIds);

	object? CreateSchedule(object document, string categoryName, string scheduleName);

	object? CreateSchedule(object document, string categoryName, string scheduleName, int? viewTypeId);

	object? CreateScheduleAdvanced(object document, string categoryName, string scheduleName, object config);

	IEnumerable<object> GetSchedulableFields(object document, string categoryName);

	IEnumerable<object> GetScheduleFields(object scheduleView);

	object? CreateSectionView(object document, string viewName, string direction, object minPoint, object maxPoint);

	object? CreateLegend(object document, string viewName);

	object? CreateLegend(object document, string viewName, int? legendId);

	object? CreateSheet(object document, int sheetNumber, string sheetName);

	object? PlaceViewOnSheet(object document, object sheet, object viewToPlace, object position);

	object? PlaceViewOnSheet(object document, object sheet, object viewToPlace, double x, double y);

	bool IsSystemView(object view);

	bool IsTemplate(object view);

	bool CanPrint(object view);

	int? CreateViewFilter(object document, string filterName, IEnumerable<string> categoryNames, IEnumerable<object> rules);

	bool DeleteViewFilter(object document, string filterName);

	bool SetViewDetailLevel(object view, int detailLevel);

	bool SetViewDisplayStyle(object view, int displayStyle);

	bool SetCategoryVisibility(object document, object view, string categoryName, bool visible);

	bool? GetCategoryVisibility(object document, object view, string categoryName);

	IEnumerable<object> GetAllCategoriesVisibility(object document, object view);

	bool IsTemporaryHideIsolateActive(object view);

	bool IsolateCategories(object document, object view, IEnumerable<int> categoryIds);

	bool IsolateElements(object document, object view, IEnumerable<int> elementIds);

	bool HideCategories(object document, object view, IEnumerable<int> categoryIds);

	bool HideElements(object document, object view, IEnumerable<int> elementIds);

	bool ResetTemporaryViewMode(object document, object view);
}
