using System;
using RevitAi.Abstractions.Infrastructure;

namespace RevitAi.UI.ViewModels;

public class RoadProjectItem
{
	public Guid Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public string DataSourceDescription { get; set; } = string.Empty;

	public string LengthDisplay { get; set; } = string.Empty;

	public DateTime CreatedAt { get; set; }

	public RoadProject Project { get; set; }

	public RoadProjectItem(RoadProject project)
	{
		Id = project.Id;
		Name = project.Name;
		DataSourceDescription = project.GetDataSourceDescription();
		CreatedAt = project.CreatedAt;
		Project = project;
		double? length = project.GetLength();
		LengthDisplay = (length.HasValue ? $"{length.Value:F2}" : "未生成");
	}
}
