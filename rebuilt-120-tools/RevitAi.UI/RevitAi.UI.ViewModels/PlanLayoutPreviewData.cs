using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitAi.UI.ViewModels;

public sealed class PlanLayoutPreviewData : ObservableObject
{
	private PointCollection? _centerlinePoints;

	private Rect? _bounds;

	private double? _totalLength;

	private string? _startStationDisplay;

	private string? _endStationDisplay;

	private double _strokeThickness = 2.0;

	private double _markerSize = 8.0;

	private double _fontSize = 9.0;

	private PointCollection? _leftBoundaryPoints;

	private PointCollection? _rightBoundaryPoints;

	public PointCollection? CenterlinePoints
	{
		get
		{
			return _centerlinePoints;
		}
		set
		{
			SetProperty(ref _centerlinePoints, value, "CenterlinePoints");
		}
	}

	public ObservableCollection<StationMarker> StationMarkers { get; set; } = new ObservableCollection<StationMarker>();

	public Rect? Bounds
	{
		get
		{
			return _bounds;
		}
		set
		{
			SetProperty(ref _bounds, value, "Bounds");
		}
	}

	public double? TotalLength
	{
		get
		{
			return _totalLength;
		}
		set
		{
			SetProperty(ref _totalLength, value, "TotalLength");
		}
	}

	public string? StartStationDisplay
	{
		get
		{
			return _startStationDisplay;
		}
		set
		{
			SetProperty(ref _startStationDisplay, value, "StartStationDisplay");
		}
	}

	public string? EndStationDisplay
	{
		get
		{
			return _endStationDisplay;
		}
		set
		{
			SetProperty(ref _endStationDisplay, value, "EndStationDisplay");
		}
	}

	public double StrokeThickness
	{
		get
		{
			return _strokeThickness;
		}
		set
		{
			if (SetProperty(ref _strokeThickness, value, "StrokeThickness"))
			{
				OnPropertyChanged("BoundaryStrokeThickness");
			}
		}
	}

	public double MarkerSize
	{
		get
		{
			return _markerSize;
		}
		set
		{
			SetProperty(ref _markerSize, value, "MarkerSize");
		}
	}

	public double FontSize
	{
		get
		{
			return _fontSize;
		}
		set
		{
			SetProperty(ref _fontSize, value, "FontSize");
		}
	}

	public PointCollection? LeftBoundaryPoints
	{
		get
		{
			return _leftBoundaryPoints;
		}
		set
		{
			SetProperty(ref _leftBoundaryPoints, value, "LeftBoundaryPoints");
		}
	}

	public PointCollection? RightBoundaryPoints
	{
		get
		{
			return _rightBoundaryPoints;
		}
		set
		{
			SetProperty(ref _rightBoundaryPoints, value, "RightBoundaryPoints");
		}
	}

	public ObservableCollection<CrossSectionLine> CrossSectionLines { get; set; } = new ObservableCollection<CrossSectionLine>();

	public ObservableCollection<AncillaryStructureLayoutLine> AncillaryStructureLayoutLines { get; set; } = new ObservableCollection<AncillaryStructureLayoutLine>();

	public double BoundaryStrokeThickness => StrokeThickness * 0.5;
}
