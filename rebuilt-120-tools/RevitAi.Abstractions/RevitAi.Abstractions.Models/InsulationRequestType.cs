namespace RevitAi.Abstractions.Models;

public enum InsulationRequestType
{
	LoadAllSystemData,
	LoadSizeRanges,
	SystemAdd,
	SizeBasedAdd,
	ManualAdd,
	IsolateSystem,
	IsolateElements,
	SelectUninsulated,
	RestoreDisplay,
	PickInsulationElements
}
