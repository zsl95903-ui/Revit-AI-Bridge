using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.Revit.WallToRoad;

public sealed class WallToRoadRequest
{
	public List<int> WallElementIds { get; set; } = new List<int>();

	public double LaneWidthMeters { get; set; } = 4.25;

	public double DefaultRoadWidthMeters { get; set; } = 9.0;

	public double FilletRadiusMeters { get; set; } = 10.0;

	public double IntersectionRadiusMeters { get; set; } = 1.5;

	public double SidewalkWidthMeters { get; set; } = 3.0;

	public double CurbWidthMillimeters { get; set; } = 150.0;

	public double CurbHeightMillimeters { get; set; } = 200.0;

	public double CurbBaseOffsetMillimeters { get; set; } = -100.0;

	public double SidewalkFloorOffsetMillimeters { get; set; } = 100.0;

	public double RoadFloorOffsetMillimeters { get; set; }

	public double SiteFloorOffsetMillimeters { get; set; }

	public bool CreateSidewalks { get; set; } = true;

	public bool CreateCurbs { get; set; } = true;

	public bool CreateCenterMarkings { get; set; } = true;

	public bool CreateCrosswalks { get; set; } = true;

	public double CrosswalkLineWidthMillimeters { get; set; } = 450.0;

	public double CrosswalkSpacingMillimeters { get; set; } = 1050.0;

	public double CrosswalkLengthMeters { get; set; } = 5.0;

	public double CrosswalkDistanceFromMarkingMeters { get; set; } = 1.0;

	public bool CreateLaneMarkings { get; set; } = true;

	public double LaneMarkingWidthMillimeters { get; set; } = 150.0;

	public double LaneMarkingSolidLengthMeters { get; set; } = 3.0;

	public double LaneMarkingGapLengthMeters { get; set; } = 3.0;

	public bool CreateDirectionArrows { get; set; } = true;

	public double ArrowSpacingMeters { get; set; } = 20.0;

	public double ArrowSizeMeters { get; set; } = 6.0;

	public double CenterMarkingWidthMillimeters { get; set; } = 150.0;

	public double CenterMarkingThicknessMillimeters { get; set; } = 10.0;

	public bool IsCenterMarkingDoubleLine { get; set; }

	public bool DeleteOriginalWalls { get; set; }

	public int LevelId { get; set; }

	public Action<Exception?, bool>? OnCompleted { get; set; }

	public List<int> CreatedFloorIds { get; set; } = new List<int>();

	public List<int> CreatedLaneFloorIds { get; set; } = new List<int>();

	public List<int> CreatedSidewalkFloorIds { get; set; } = new List<int>();

	public List<int> CreatedCurbFloorIds { get; set; } = new List<int>();

	public List<int> CreatedCenterMarkingFloorIds { get; set; } = new List<int>();

	public List<int> CreatedCrosswalkFloorIds { get; set; } = new List<int>();

	public List<int> CreatedLaneMarkingFloorIds { get; set; } = new List<int>();

	public List<int> CreatedDirectionArrowFloorIds { get; set; } = new List<int>();

	public bool CreateSiteFloor { get; set; } = true;

	public double SiteFloorExtensionMeters { get; set; } = 20.0;

	public List<int> CreatedSiteFloorIds { get; set; } = new List<int>();

	public List<int> CreatedCurbWallIds { get; set; } = new List<int>();

	public object? Document { get; set; }
}
