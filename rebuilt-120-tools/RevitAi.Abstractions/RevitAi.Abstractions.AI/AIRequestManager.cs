namespace RevitAi.Abstractions.AI;

public static class AIRequestManager
{
	private static IRevitExternalEventRequest? _currentRequest;

	public static bool HasRequest => _currentRequest != null;

	public static void SetRequest(IRevitExternalEventRequest request)
	{
		_currentRequest = request;
	}

	public static IRevitExternalEventRequest? GetAndClearRequest()
	{
		IRevitExternalEventRequest? currentRequest = _currentRequest;
		_currentRequest = null;
		return currentRequest;
	}
}
