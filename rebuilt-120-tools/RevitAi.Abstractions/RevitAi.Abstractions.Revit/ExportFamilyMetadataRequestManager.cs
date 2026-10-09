namespace RevitAi.Abstractions.Revit;

public static class ExportFamilyMetadataRequestManager
{
	private static ExportFamilyMetadataRequest? _currentRequest;

	public static bool HasRequest => _currentRequest != null;

	public static void SetRequest(ExportFamilyMetadataRequest request)
	{
		_currentRequest = request;
	}

	public static ExportFamilyMetadataRequest? GetAndClearRequest()
	{
		ExportFamilyMetadataRequest? currentRequest = _currentRequest;
		_currentRequest = null;
		return currentRequest;
	}
}
