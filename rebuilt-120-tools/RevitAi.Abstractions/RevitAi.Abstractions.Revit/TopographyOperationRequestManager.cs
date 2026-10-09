namespace RevitAi.Abstractions.Revit;

public static class TopographyOperationRequestManager
{
	private static TopographyOperationRequest? _currentRequest;

	private static CreateTopographyRequest? _createTopographyRequest;

	public static bool HasRequest => _currentRequest != null;

	public static bool HasCreateTopographyRequest => _createTopographyRequest != null;

	public static void SetRequest(TopographyOperationRequest request)
	{
		_currentRequest = request;
	}

	public static TopographyOperationRequest? GetAndClearRequest()
	{
		TopographyOperationRequest? currentRequest = _currentRequest;
		_currentRequest = null;
		return currentRequest;
	}

	public static void SetCreateTopographyRequest(CreateTopographyRequest request)
	{
		_createTopographyRequest = request;
	}

	public static CreateTopographyRequest? GetAndClearCreateTopographyRequest()
	{
		CreateTopographyRequest? createTopographyRequest = _createTopographyRequest;
		_createTopographyRequest = null;
		return createTopographyRequest;
	}
}
