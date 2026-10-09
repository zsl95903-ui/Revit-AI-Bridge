using System.Collections.Generic;
using RevitAi.Abstractions.Models;

namespace RevitAi.Abstractions.Services;

public interface IInsulationService
{
	List<SystemInsulationInfo> LoadAllSystemData(object document);

	List<InsulationTypeInfoLite> LoadAllInsulationTypes(object document);

	List<SizeRangeInfo> GetSizeRanges(object document, SystemInsulationInfo system);

	InsulationOperationSummary ExecuteSystemAdd(object document, List<SystemInsulationInfo> selectedSystems);

	InsulationOperationSummary ExecuteSizeBasedAdd(object document, SystemInsulationInfo system, List<SizeRangeInfo> ranges);

	InsulationOperationSummary ExecuteManualAdd(object document, List<SelectedElementInfo> elements, double thicknessMM, string material);

	string? IsolateElements(List<int> elementIds);

	string? IsolateSystem(object document, SystemInsulationInfo system, out int elementCount);

	string RestoreDisplay();

	void SelectElementIds(List<int> elementIds);

	int SelectUninsulated(object document, SystemInsulationInfo system);

	List<SelectedElementInfo> PickInsulationElements(string prompt);
}
