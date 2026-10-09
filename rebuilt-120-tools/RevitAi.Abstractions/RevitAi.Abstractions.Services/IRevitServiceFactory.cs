namespace RevitAi.Abstractions.Services;

public interface IRevitServiceFactory
{
	IRoadProjectService? CreateRoadProjectService();

	IModelPlacementService? CreateModelPlacementService();
}
