using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RevitAi.Abstractions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.ViewModels;

public class HorizontalCurvePointViewModel : ObservableObject
{
	[ObservableProperty]
	private string _ipNumber = "";

	[ObservableProperty]
	private double _station;

	[ObservableProperty]
	private double _x;

	[ObservableProperty]
	private double _y;

	[ObservableProperty]
	private double _radius;

	[ObservableProperty]
	private double _ls1;

	[ObservableProperty]
	private double _ls2;

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
	public string IpNumber
	{
		get
		{
			return _ipNumber;
		}
		[MemberNotNull("_ipNumber")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_ipNumber, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IpNumber);
				_ipNumber = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IpNumber);
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
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Radius);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double Ls1
	{
		get
		{
			return _ls1;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_ls1, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Ls1);
				_ls1 = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Ls1);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double Ls2
	{
		get
		{
			return _ls2;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_ls2, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Ls2);
				_ls2 = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Ls2);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnStationChanged(double value)
	{
		OnPropertyChanged("StationDisplay");
	}
}
