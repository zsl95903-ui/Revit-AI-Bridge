using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class StationParameterDisplayModel : INotifyPropertyChanged
{
	private bool _isLocked;

	public StationParameter Parameter { get; set; } = new StationParameter();

	public double StationKm
	{
		get
		{
			return Parameter.StationKm;
		}
		set
		{
			if (Parameter.StationKm != value)
			{
				Parameter.StationKm = value;
				OnPropertyChanged("StationKm");
				OnPropertyChanged("StationDisplay");
				OnPropertyChanged("StationKmInteger");
				OnPropertyChanged("StationMeter");
			}
		}
	}

	public string StationDisplay => Parameter.GetStationDisplay();

	public int StationKmInteger
	{
		get
		{
			return (int)Math.Floor(Parameter.StationKm);
		}
		set
		{
			double stationMeter = StationMeter;
			double num = (double)value + stationMeter / 1000.0;
			if (Math.Abs(Parameter.StationKm - num) > 1E-10)
			{
				Parameter.StationKm = num;
				OnPropertyChanged("StationKmInteger");
				OnPropertyChanged("StationKm");
				OnPropertyChanged("StationDisplay");
			}
		}
	}

	public double StationMeter
	{
		get
		{
			return Math.Round((Parameter.StationKm - Math.Floor(Parameter.StationKm)) * 1000.0, 3);
		}
		set
		{
			double num = Math.Max(0.0, Math.Min(999.999, value));
			double num2 = (double)StationKmInteger + num / 1000.0;
			if (Math.Abs(Parameter.StationKm - num2) > 1E-10)
			{
				Parameter.StationKm = num2;
				OnPropertyChanged("StationMeter");
				OnPropertyChanged("StationKm");
				OnPropertyChanged("StationDisplay");
			}
		}
	}

	public double? RoadWidth
	{
		get
		{
			return Parameter.RoadWidth;
		}
		set
		{
			if (Parameter.RoadWidth != value)
			{
				Parameter.RoadWidth = value;
				OnPropertyChanged("RoadWidth");
			}
		}
	}

	public double? CrossSectionOffsetAngle
	{
		get
		{
			return Parameter.CrossSectionOffsetAngle;
		}
		set
		{
			if (Parameter.CrossSectionOffsetAngle != value)
			{
				Parameter.CrossSectionOffsetAngle = value;
				OnPropertyChanged("CrossSectionOffsetAngle");
			}
		}
	}

	public bool IsLocked
	{
		get
		{
			return _isLocked;
		}
		set
		{
			_isLocked = value;
			OnPropertyChanged("IsLocked");
		}
	}

	public string? Note
	{
		get
		{
			return Parameter.Note;
		}
		set
		{
			if (Parameter.Note != value)
			{
				Parameter.Note = value;
				OnPropertyChanged("Note");
			}
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public static StationParameterDisplayModel CreateEmpty(double defaultStationKm = 0.0, double defaultRoadWidth = 7.0)
	{
		int num = (int)Math.Floor(defaultStationKm);
		Math.Round((defaultStationKm - (double)num) * 1000.0);
		return new StationParameterDisplayModel
		{
			Parameter = new StationParameter
			{
				StationKm = defaultStationKm,
				RoadWidth = defaultRoadWidth,
				CrossSectionOffsetAngle = 0.0
			}
		};
	}

	public static StationParameterDisplayModel FromParameter(StationParameter parameter)
	{
		return new StationParameterDisplayModel
		{
			Parameter = parameter
		};
	}

	public StationParameter ToParameter()
	{
		return Parameter;
	}
}
