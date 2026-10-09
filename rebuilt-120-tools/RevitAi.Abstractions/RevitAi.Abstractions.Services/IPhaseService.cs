using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface IPhaseService
{
	IEnumerable<object> GetAllPhases(object document);

	object? GetElementCreatedPhase(object element);

	object? GetElementDemolishedPhase(object element);

	bool SetElementCreatedPhase(object element, int phaseId, object document);

	bool SetElementDemolishedPhase(object element, int phaseId, object document);

	object? GetPhaseByName(object document, string phaseName);

	(object? CreatedPhase, object? DemolishedPhase)? GetElementPhases(object element);
}
