namespace RevitAi.Abstractions.Revit;

public static class RoadSurfaceRefinementRequestManager
{
	private static RoadSurfaceSelectionRequest? _currentSelectionRequest;

	private static RoadSurfaceCutRequest? _currentCutRequest;

	private static RoadSurfaceVoidGenerationRequest? _voidGenerationRequest;

	public static bool HasSelectionRequest => _currentSelectionRequest != null;

	public static bool HasCutRequest => _currentCutRequest != null;

	public static bool HasVoidGenerationRequest => _voidGenerationRequest != null;

	public static void SetSelectionRequest(RoadSurfaceSelectionRequest request)
	{
		_currentSelectionRequest = request;
	}

	public static RoadSurfaceSelectionRequest? GetAndClearSelectionRequest()
	{
		RoadSurfaceSelectionRequest? currentSelectionRequest = _currentSelectionRequest;
		_currentSelectionRequest = null;
		return currentSelectionRequest;
	}

	public static void SetCutRequest(RoadSurfaceCutRequest request)
	{
		_currentCutRequest = request;
	}

	public static RoadSurfaceCutRequest? GetAndClearCutRequest()
	{
		RoadSurfaceCutRequest? currentCutRequest = _currentCutRequest;
		_currentCutRequest = null;
		return currentCutRequest;
	}

	public static void SetVoidGenerationRequest(RoadSurfaceVoidGenerationRequest request)
	{
		_voidGenerationRequest = request;
	}

	public static RoadSurfaceVoidGenerationRequest? GetAndClearVoidGenerationRequest()
	{
		RoadSurfaceVoidGenerationRequest? voidGenerationRequest = _voidGenerationRequest;
		_voidGenerationRequest = null;
		return voidGenerationRequest;
	}
}
