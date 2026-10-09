using RevitAi.Abstractions.Services;

namespace RevitAi.Revit.Services;

public sealed class RevitServiceFactory : IRevitServiceFactory
{
	public IRoadProjectService? CreateRoadProjectService()
	{
		try
		{
			InfrastructureService infrastructureService = new InfrastructureService();
			return (IRoadProjectService?)(object)new RoadProjectService((IInfrastructureService)(object)infrastructureService);
		}
		catch
		{
			return null;
		}
	}

	public IModelPlacementService? CreateModelPlacementService()
	{
		return null;
	}
}
