namespace RevitAi.Abstractions.Models;

public static class InsulationRequestManager
{
	private static InsulationRequest? _currentRequest;

	public static bool HasRequest => _currentRequest != null;

	public static void SetRequest(InsulationRequest request)
	{
		_currentRequest = request;
	}

	public static InsulationRequest? GetAndClearRequest()
	{
		InsulationRequest? currentRequest = _currentRequest;
		_currentRequest = null;
		return currentRequest;
	}
}
