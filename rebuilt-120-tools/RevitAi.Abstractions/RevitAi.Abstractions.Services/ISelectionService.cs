using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface ISelectionService
{
	IEnumerable<object> PickElements(object document, string prompt = "请选择元素");

	IEnumerable<object> GetSelectedElements(object document);

	void ClearSelection(object document);

	bool SelectElements(object document, IEnumerable<int> elementIds, bool append = false);
}
