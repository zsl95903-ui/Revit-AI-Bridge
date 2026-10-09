using System.Collections.Generic;

namespace RevitAi.Abstractions.FamilyLibrary;

public class FamilyLibraryDetail
{
	public FamilyLibraryItem Family { get; set; }

	public List<FamilyParameter> Parameters { get; set; } = new List<FamilyParameter>();

	public List<FamilyTypeParameterGroup> TypeGroups { get; set; } = new List<FamilyTypeParameterGroup>();

	public FamilyParameterStats ParamStats { get; set; }
}
