namespace RevitAi.Revit.Revit;

public static class FamilyCheckRequestManager
{
	private static FamilyCheckRequest? familyCheckRequest_0;

	public static void SetRequest(FamilyCheckRequest request)
	{
		familyCheckRequest_0 = request;
	}

	public static FamilyCheckRequest? GetAndClearRequest()
	{
		FamilyCheckRequest result = familyCheckRequest_0;
		familyCheckRequest_0 = null;
		return result;
	}
}
