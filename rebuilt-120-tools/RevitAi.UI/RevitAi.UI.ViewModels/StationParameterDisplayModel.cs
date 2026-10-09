using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RevitAi.Abstractions.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.ViewModels;

public class StationParameterDisplayModel : ObservableObject
{
	[ObservableProperty]
	private double _stationKm;

	[ObservableProperty]
	private int _stationKmInteger;

	[ObservableProperty]
	private double _stationMeter;

	[ObservableProperty]
	private double? _roadWidth;

	[ObservableProperty]
	private double? _crossSectionOffsetAngle;

	[ObservableProperty]
	private string? _note;

	[ObservableProperty]
	private bool _isLocked;

	public StationParameter Parameter { get; set; } = new StationParameter();

	public string StationDisplay
	{
		get
		{
			int num = (int)Math.Floor(StationKm);
			double num2 = (StationKm - (double)num) * 1000.0;
			if (!(Math.Abs(num2 - Math.Round(num2)) < 0.001))
			{
				return $"K{num}+{num2:000.000}";
			}
			return $"K{num}+{(int)Math.Round(num2):D3}";
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double StationKm
	{
		get
		{
			return _stationKm;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_stationKm, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StationKm);
				_stationKm = value;
				OnStationKmChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StationKm);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int StationKmInteger
	{
		get
		{
			return _stationKmInteger;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_stationKmInteger, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StationKmInteger);
				_stationKmInteger = value;
				OnStationKmIntegerChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StationKmInteger);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double StationMeter
	{
		get
		{
			return _stationMeter;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_stationMeter, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StationMeter);
				_stationMeter = value;
				OnStationMeterChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StationMeter);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double? RoadWidth
	{
		get
		{
			return _roadWidth;
		}
		set
		{
			if (!EqualityComparer<double?>.Default.Equals(_roadWidth, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.RoadWidth);
				_roadWidth = value;
				OnRoadWidthChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.RoadWidth);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double? CrossSectionOffsetAngle
	{
		get
		{
			return _crossSectionOffsetAngle;
		}
		set
		{
			if (!EqualityComparer<double?>.Default.Equals(_crossSectionOffsetAngle, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CrossSectionOffsetAngle);
				_crossSectionOffsetAngle = value;
				OnCrossSectionOffsetAngleChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CrossSectionOffsetAngle);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? Note
	{
		get
		{
			return _note;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_note, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Note);
				_note = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Note);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsLocked
	{
		get
		{
			return _isLocked;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isLocked, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsLocked);
				_isLocked = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsLocked);
			}
		}
	}

	public static StationParameterDisplayModel CreateEmpty(double stationKm, double defaultRoadWidth)
	{
		return new StationParameterDisplayModel
		{
			Parameter = new StationParameter
			{
				StationKm = stationKm
			},
			StationKm = stationKm,
			StationKmInteger = (int)Math.Floor(stationKm),
			StationMeter = (stationKm - Math.Floor(stationKm)) * 1000.0,
			RoadWidth = defaultRoadWidth,
			CrossSectionOffsetAngle = 0.0,
			IsLocked = false
		};
	}

	public static StationParameterDisplayModel FromParameter(StationParameter parameter)
	{
		return new StationParameterDisplayModel
		{
			Parameter = parameter,
			StationKm = parameter.StationKm,
			StationKmInteger = (int)Math.Floor(parameter.StationKm),
			StationMeter = (parameter.StationKm - Math.Floor(parameter.StationKm)) * 1000.0,
			RoadWidth = parameter.RoadWidth,
			CrossSectionOffsetAngle = parameter.CrossSectionOffsetAngle,
			Note = parameter.Note,
			IsLocked = false
		};
	}

	public override string ToString()
	{
		return StationDisplay;
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnStationKmChanged(double value)
	{
		int num = (int)Math.Floor(value);
		double stationMeter = (value - (double)num) * 1000.0;
		StationKmInteger = num;
		StationMeter = stationMeter;
		OnPropertyChanged("StationDisplay");
		Parameter.StationKm = value;
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnStationKmIntegerChanged(int value)
	{
		StationKm = (double)value + StationMeter / 1000.0;
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnStationMeterChanged(double value)
	{
		if (value < 0.0 || value >= 1000.0)
		{
			value = Math.Max(0.0, Math.Min(999.999, value));
		}
		StationKm = (double)StationKmInteger + value / 1000.0;
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnRoadWidthChanged(double? value)
	{
		Parameter.RoadWidth = value;
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnCrossSectionOffsetAngleChanged(double? value)
	{
		Parameter.CrossSectionOffsetAngle = value;
	}
}
