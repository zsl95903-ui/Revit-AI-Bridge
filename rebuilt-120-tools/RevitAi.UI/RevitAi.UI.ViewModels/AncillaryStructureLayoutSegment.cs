using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.ViewModels;

public class AncillaryStructureLayoutSegment : ObservableObject
{
	[ObservableProperty]
	private double _startStationKm;

	[ObservableProperty]
	private int _startStationKmInteger;

	[ObservableProperty]
	private double _startStationMeter;

	[ObservableProperty]
	private double _endStationKm;

	[ObservableProperty]
	private int _endStationKmInteger;

	[ObservableProperty]
	private double _endStationMeter;

	[ObservableProperty]
	private double _modelLength = 5.0;

	public Guid SegmentId { get; set; } = Guid.NewGuid();

	public string StartStationDisplay
	{
		get
		{
			int num = (int)Math.Floor(StartStationKm);
			double num2 = (StartStationKm - (double)num) * 1000.0;
			if (!(Math.Abs(num2 - Math.Round(num2)) < 0.001))
			{
				return $"K{num}+{num2:F3}";
			}
			return $"K{num}+{(int)Math.Round(num2):D3}";
		}
	}

	public string EndStationDisplay
	{
		get
		{
			int num = (int)Math.Floor(EndStationKm);
			double num2 = (EndStationKm - (double)num) * 1000.0;
			if (!(Math.Abs(num2 - Math.Round(num2)) < 0.001))
			{
				return $"K{num}+{num2:F3}";
			}
			return $"K{num}+{(int)Math.Round(num2):D3}";
		}
	}

	public double LayoutLength => (EndStationKm - StartStationKm) * 1000.0;

	public int EstimatedModelCount
	{
		get
		{
			if (!(ModelLength > 0.0))
			{
				return 0;
			}
			return (int)Math.Ceiling(LayoutLength / ModelLength);
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double StartStationKm
	{
		get
		{
			return _startStationKm;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_startStationKm, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StartStationKm);
				_startStationKm = value;
				OnStartStationKmChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StartStationKm);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int StartStationKmInteger
	{
		get
		{
			return _startStationKmInteger;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_startStationKmInteger, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StartStationKmInteger);
				_startStationKmInteger = value;
				OnStartStationKmIntegerChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StartStationKmInteger);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double StartStationMeter
	{
		get
		{
			return _startStationMeter;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_startStationMeter, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StartStationMeter);
				_startStationMeter = value;
				OnStartStationMeterChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StartStationMeter);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double EndStationKm
	{
		get
		{
			return _endStationKm;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_endStationKm, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.EndStationKm);
				_endStationKm = value;
				OnEndStationKmChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.EndStationKm);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int EndStationKmInteger
	{
		get
		{
			return _endStationKmInteger;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_endStationKmInteger, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.EndStationKmInteger);
				_endStationKmInteger = value;
				OnEndStationKmIntegerChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.EndStationKmInteger);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double EndStationMeter
	{
		get
		{
			return _endStationMeter;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_endStationMeter, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.EndStationMeter);
				_endStationMeter = value;
				OnEndStationMeterChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.EndStationMeter);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double ModelLength
	{
		get
		{
			return _modelLength;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_modelLength, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ModelLength);
				_modelLength = value;
				OnModelLengthChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ModelLength);
			}
		}
	}

	public static AncillaryStructureLayoutSegment CreateEmpty()
	{
		return new AncillaryStructureLayoutSegment
		{
			StartStationKm = 0.0,
			EndStationKm = 0.1,
			ModelLength = 5.0
		};
	}

	public AncillaryStructureLayoutSegment Clone()
	{
		return new AncillaryStructureLayoutSegment
		{
			SegmentId = SegmentId,
			StartStationKm = StartStationKm,
			EndStationKm = EndStationKm,
			ModelLength = ModelLength
		};
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnStartStationKmChanged(double value)
	{
		int num = (int)Math.Floor(value);
		double startStationMeter = (value - (double)num) * 1000.0;
		StartStationKmInteger = num;
		StartStationMeter = startStationMeter;
		OnPropertyChanged("StartStationDisplay");
		OnPropertyChanged("LayoutLength");
		OnPropertyChanged("EstimatedModelCount");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnStartStationKmIntegerChanged(int value)
	{
		StartStationKm = (double)value + StartStationMeter / 1000.0;
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnStartStationMeterChanged(double value)
	{
		if (value < 0.0 || value >= 1000.0)
		{
			value = Math.Max(0.0, Math.Min(999.999, value));
		}
		StartStationKm = (double)StartStationKmInteger + value / 1000.0;
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnEndStationKmChanged(double value)
	{
		int num = (int)Math.Floor(value);
		double endStationMeter = (value - (double)num) * 1000.0;
		EndStationKmInteger = num;
		EndStationMeter = endStationMeter;
		OnPropertyChanged("EndStationDisplay");
		OnPropertyChanged("LayoutLength");
		OnPropertyChanged("EstimatedModelCount");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnEndStationKmIntegerChanged(int value)
	{
		EndStationKm = (double)value + EndStationMeter / 1000.0;
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnEndStationMeterChanged(double value)
	{
		if (value < 0.0 || value >= 1000.0)
		{
			value = Math.Max(0.0, Math.Min(999.999, value));
		}
		EndStationKm = (double)EndStationKmInteger + value / 1000.0;
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnModelLengthChanged(double value)
	{
		OnPropertyChanged("EstimatedModelCount");
	}
}
