namespace RevitAi.Revit.Revit;

public static class FamilyPlacementRequestManager
{
	private static FamilyPlacementRequest? familyPlacementRequest_0;

	public static void SetRequest(FamilyPlacementRequest request)
	{
		familyPlacementRequest_0 = request;
	}

	public static FamilyPlacementRequest? GetAndClearRequest()
	{
		FamilyPlacementRequest result = familyPlacementRequest_0;
		familyPlacementRequest_0 = null;
		return result;
	}
}
