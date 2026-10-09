using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using RevitAi.Abstractions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.ViewModels;

public class StationPointViewModel : ObservableObject
{
	[ObservableProperty]
	private double _station;

	[ObservableProperty]
	private double _x;

	[ObservableProperty]
	private double _y;

	[ObservableProperty]
	private double _z;

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
			return $"K{num}{value}{(int)num3}";
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
				if (num > 0)
				{
					string input = text.Substring(0, num).Trim();
					string s = text.Substring(num + 1).Trim();
					input = Regex.Replace(input, "[^0-9.]", "");
					double num2 = double.Parse(input);
					double num3 = double.Parse(s);
					Station = num2 + num3 / 1000.0;
				}
				else
				{
					double num4 = double.Parse(text);
					Station = num4 / 1000.0;
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
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Station);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double X
	{
		get
		{
			return _x;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_x, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.X);
				_x = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.X);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double Y
	{
		get
		{
			return _y;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_y, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Y);
				_y = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Y);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double Z
	{
		get
		{
			return _z;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_z, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Z);
				_z = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Z);
			}
		}
	}
}
