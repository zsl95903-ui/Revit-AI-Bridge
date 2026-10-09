namespace RevitAi.Abstractions.Units;

public static class SetProjectUnitRequestManager
{
	private static SetProjectUnitRequest? _currentRequest;

	public static bool HasRequest => _currentRequest != null;

	public static void SetRequest(SetProjectUnitRequest request)
	{
		_currentRequest = request;
	}

	public static SetProjectUnitRequest? GetAndClearRequest()
	{
		SetProjectUnitRequest? currentRequest = _currentRequest;
		_currentRequest = null;
		return currentRequest;
	}
}
