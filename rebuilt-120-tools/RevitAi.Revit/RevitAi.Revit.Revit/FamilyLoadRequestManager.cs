namespace RevitAi.Revit.Revit;

public static class FamilyLoadRequestManager
{
	private static FamilyLoadRequest? familyLoadRequest_0;

	public static void SetRequest(FamilyLoadRequest request)
	{
		familyLoadRequest_0 = request;
	}

	public static FamilyLoadRequest? GetAndClearRequest()
	{
		FamilyLoadRequest result = familyLoadRequest_0;
		familyLoadRequest_0 = null;
		return result;
	}
}
