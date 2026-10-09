using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RevitAi.Abstractions.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.ViewModels;

public class SubgradeLayerDisplayModel : ObservableObject
{
	[ObservableProperty]
	private string _name = "路基层";

	public static readonly char[] ForbiddenCharacters = new char[13]
	{
		'\\', ':', '{', '}', '[', ']', '|', ';', '<', '>',
		'?', '`', '~'
	};

	[ObservableProperty]
	private double _widthIncrement;

	[ObservableProperty]
	private WidthIncrementType _incrementType = WidthIncrementType.BothSides;

	[ObservableProperty]
	private double _slopeRatio = 1.5;

	[ObservableProperty]
	private double _height = 300.0;

	[ObservableProperty]
	private int _order;

	[ObservableProperty]
	private string _color = "#4CAF50";

	public int IncrementTypeIndex
	{
		get
		{
			return (int)IncrementType;
		}
		set
		{
			if (value >= 0 && value < 2 && IncrementType != (WidthIncrementType)value)
			{
				IncrementType = (WidthIncrementType)value;
			}
		}
	}

	public string IncrementTypeDisplay
	{
		get
		{
			if (IncrementType != WidthIncrementType.SingleSide)
			{
				return "两侧";
			}
			return "单侧";
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Name
	{
		get
		{
			return _name;
		}
		[MemberNotNull("_name")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_name, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Name);
				_name = value;
				OnNameChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Name);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double WidthIncrement
	{
		get
		{
			return _widthIncrement;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_widthIncrement, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.WidthIncrement);
				_widthIncrement = value;
				OnWidthIncrementChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.WidthIncrement);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public WidthIncrementType IncrementType
	{
		get
		{
			return _incrementType;
		}
		set
		{
			if (!EqualityComparer<WidthIncrementType>.Default.Equals(_incrementType, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IncrementType);
				_incrementType = value;
				OnIncrementTypeChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IncrementType);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double SlopeRatio
	{
		get
		{
			return _slopeRatio;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_slopeRatio, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SlopeRatio);
				_slopeRatio = value;
				OnSlopeRatioChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SlopeRatio);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double Height
	{
		get
		{
			return _height;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_height, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Height);
				_height = value;
				OnHeightChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Height);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int Order
	{
		get
		{
			return _order;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_order, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Order);
				_order = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Order);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Color
	{
		get
		{
			return _color;
		}
		[MemberNotNull("_color")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_color, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Color);
				_color = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Color);
			}
		}
	}

	public event Action? ParameterChanged;

	public event Action<string>? NameValidationFailed;

	public double GetTopWidth(double baseWidth)
	{
		double num = ((IncrementType == WidthIncrementType.SingleSide) ? (WidthIncrement * 2.0) : WidthIncrement);
		return baseWidth + num;
	}

	public double GetBottomWidth(double baseWidth)
	{
		return GetTopWidth(baseWidth) + 2.0 * Height * SlopeRatio;
	}

	public SubgradeLayer ToSubgradeLayer()
	{
		return new SubgradeLayer
		{
			Name = Name,
			WidthIncrement = WidthIncrement,
			IncrementType = IncrementType,
			SlopeRatio = SlopeRatio,
			Height = Height,
			Order = Order,
			Color = Color
		};
	}

	public override string ToString()
	{
		string value = ((IncrementType == WidthIncrementType.BothSides) ? "两侧" : "单侧");
		return $"{Name} (增量:{WidthIncrement}mm/{value}, 坡度:1:{SlopeRatio}, 高:{Height}mm)";
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnNameChanged(string value)
	{
		if (!string.IsNullOrEmpty(value) && value.IndexOfAny(ForbiddenCharacters) >= 0)
		{
			NameValidationFailed?.Invoke(value);
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnWidthIncrementChanged(double value)
	{
		ParameterChanged?.Invoke();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnIncrementTypeChanged(WidthIncrementType value)
	{
		ParameterChanged?.Invoke();
		OnPropertyChanged("IncrementTypeDisplay");
		OnPropertyChanged("IncrementTypeIndex");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSlopeRatioChanged(double value)
	{
		ParameterChanged?.Invoke();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnHeightChanged(double value)
	{
		ParameterChanged?.Invoke();
	}
}
