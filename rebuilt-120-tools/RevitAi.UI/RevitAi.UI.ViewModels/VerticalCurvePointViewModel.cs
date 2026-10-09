using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RevitAi.Abstractions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.ViewModels;

public class VerticalCurvePointViewModel : ObservableObject
{
	[ObservableProperty]
	private string _pointNumber = "";

	[ObservableProperty]
	private double _station;

	[ObservableProperty]
	private double _elevation;

	[ObservableProperty]
	private double _radius;

	public string CurveType
	{
		get
		{
			if (!(Radius < 0.0))
			{
				return "凹";
			}
			return "凸";
		}
		set
		{
		}
	}

	public string StationDisplay
	{
		get
		{
			int num = (int)Station;
			double num2 = (Station - (double)num) * 1000.0;
			string value = ((num2 >= 0.0) ? "+" : "-");
			double num3 = Math.Abs(num2);
			if (!(Math.Abs(num3 - Math.Round(num3)) < 0.0001))
			{
				return $"K{num}{value}{num3:F3}";
			}
			return $"K{num}{value}{(int)num3:D3}";
		}
		set
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				Station = 0.0;
				return;
			}
			try
			{
				string text = value.Trim();
				int num = text.LastIndexOf('+');
				double result;
				if (num > 0)
				{
					double num2 = double.Parse(text.Substring(num + 1).Trim());
					string text2 = text.Substring(0, num);
					int num3 = text2.LastIndexOf('K');
					if (num3 < 0)
					{
						num3 = text2.LastIndexOf('k');
					}
					if (num3 >= 0)
					{
						double num4 = double.Parse(text2.Substring(num3 + 1).Trim());
						Station = num4 + num2 / 1000.0;
					}
					else
					{
						double num5 = double.Parse(text2.Trim());
						Station = num5 + num2 / 1000.0;
					}
				}
				else if (double.TryParse(text, out result))
				{
					Station = result;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("解析桩号失败: " + value + ", " + ex.Message);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string PointNumber
	{
		get
		{
			return _pointNumber;
		}
		[MemberNotNull("_pointNumber")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_pointNumber, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PointNumber);
				_pointNumber = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PointNumber);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double Station
	{
		get
		{
			return _station;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_station, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Station);
				_station = value;
				OnStationChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Station);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double Elevation
	{
		get
		{
			return _elevation;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_elevation, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Elevation);
				_elevation = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Elevation);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double Radius
	{
		get
		{
			return _radius;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_radius, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Radius);
				_radius = value;
				OnRadiusChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Radius);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnStationChanged(double value)
	{
		OnPropertyChanged("StationDisplay");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnRadiusChanged(double value)
	{
		OnPropertyChanged("CurveType");
	}
}
