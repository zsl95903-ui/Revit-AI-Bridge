using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;

namespace RevitAi.Abstractions.Infrastructure;

public interface IRoadProjectManager
{
	event EventHandler<RoadProjectChangedEventArgs>? ProjectChanged;

	IReadOnlyList<RoadProject> GetAllProjects();

	RoadProject? GetProject(Guid id);

	RoadProject? GetProjectByName(string name);

	Result<RoadProject> AddProject(string name, string? description = null);

	Result<bool> UpdateProject(RoadProject project);

	Result<bool> DeleteProject(Guid id);

	Result<bool> SetActiveProject(Guid id);

	RoadProject? GetActiveProject();

	void ClearAll();

	Task<Result<bool>> ExportProjectAsync(Guid projectId, string filePath);

	Task<Result<RoadProject>> ImportProjectAsync(string filePath);

	Task<Result<bool>> ExportAllProjectsAsync(string filePath);

	Task<Result<List<RoadProject>>> ImportMultipleProjectsAsync(string filePath);
}
