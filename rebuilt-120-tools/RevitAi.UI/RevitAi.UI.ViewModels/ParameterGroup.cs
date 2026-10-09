using System.Collections.Generic;
using RevitAi.Abstractions.FamilyLibrary;

namespace RevitAi.UI.ViewModels;

public class ParameterGroup
{
	public string GroupName { get; set; } = string.Empty;

	public List<FamilyParameter> Parameters { get; set; } = new List<FamilyParameter>();
}
