using RevitAi.Abstractions.Revit.WallToRoad;

namespace RevitAi.Abstractions.Revit;

public static class WallToRoadRequestManager
{
	private static WallToRoadRequest? _currentRequest;

	public static bool HasRequest => _currentRequest != null;

	public static void SetRequest(WallToRoadRequest request)
	{
		_currentRequest = request;
	}

	public static WallToRoadRequest? GetAndClearRequest()
	{
		WallToRoadRequest? currentRequest = _currentRequest;
		_currentRequest = null;
		return currentRequest;
	}
}
