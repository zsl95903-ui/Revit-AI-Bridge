namespace RevitAi.Abstractions.Infrastructure;

public sealed class StationParameterData
{
	public double StationKm { get; set; }

	public double? RoadWidth { get; set; }

	public double? CrossSectionOffsetAngle { get; set; }

	public string? Note { get; set; }

	public StationParameter ToStationParameter()
	{
		return new StationParameter
		{
			StationKm = StationKm,
			RoadWidth = RoadWidth,
			CrossSectionOffsetAngle = CrossSectionOffsetAngle,
			Note = Note
		};
	}

	public static StationParameterData FromStationParameter(StationParameter parameter)
	{
		return new StationParameterData
		{
			StationKm = parameter.StationKm,
			RoadWidth = parameter.RoadWidth,
			CrossSectionOffsetAngle = parameter.CrossSectionOffsetAngle,
			Note = parameter.Note
		};
	}
}
