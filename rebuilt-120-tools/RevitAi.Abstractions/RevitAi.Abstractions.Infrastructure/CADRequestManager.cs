namespace RevitAi.Abstractions.Infrastructure;

public static class CADRequestManager
{
	private static CADElementSelectionRequest? _currentRequest;

	public static bool HasRequest => _currentRequest != null;

	public static void SetRequest(CADElementSelectionRequest request)
	{
		_currentRequest = request;
	}

	public static CADElementSelectionRequest? GetAndClearRequest()
	{
		CADElementSelectionRequest? currentRequest = _currentRequest;
		_currentRequest = null;
		return currentRequest;
	}
}
