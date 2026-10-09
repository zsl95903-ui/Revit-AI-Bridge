using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using RevitAi.Abstractions.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.ViewModels;

public class AncillaryStructureDisplayModel : ObservableObject
{
	[ObservableProperty]
	private string _name = string.Empty;

	[ObservableProperty]
	private AncillaryStructureType _structureType;

	[ObservableProperty]
	private AncillaryPositionType _positionType;

	[ObservableProperty]
	private double _horizontalOffset;

	[ObservableProperty]
	private double _elevationDifference;

	[ObservableProperty]
	private double _width;

	[ObservableProperty]
	private double _height;

	[ObservableProperty]
	private double? _slopeRatio;

	[ObservableProperty]
	private string _color = "#808080";

	[ObservableProperty]
	private bool _isEnabled = true;

	[ObservableProperty]
	private int _order;

	private ObservableCollection<AncillaryStructureLayoutSegment> _layoutSegments;

	private static readonly string[] RainbowColors = new string[7] { "#FF9800", "#4CAF50", "#2196F3", "#9C27B0", "#F44336", "#00BCD4", "#795548" };

	public AncillaryStructure Structure { get; set; } = new AncillaryStructure();

	public int StructureTypeIndex
	{
		get
		{
			return (int)StructureType;
		}
		set
		{
			if (value >= 0 && value < 6)
			{
				StructureType = (AncillaryStructureType)value;
			}
		}
	}

	public int PositionTypeIndex
	{
		get
		{
			return (int)PositionType;
		}
		set
		{
			if (value >= 0 && value < 3)
			{
				PositionType = (AncillaryPositionType)value;
			}
		}
	}

	public ObservableCollection<AncillaryStructureLayoutSegment> LayoutSegments
	{
		get
		{
			return _layoutSegments;
		}
		set
		{
			if (_layoutSegments != null)
			{
				_layoutSegments.CollectionChanged -= OnLayoutSegmentsChanged;
			}
			_layoutSegments = value;
			if (_layoutSegments == null)
			{
				return;
			}
			_layoutSegments.CollectionChanged += OnLayoutSegmentsChanged;
			foreach (AncillaryStructureLayoutSegment layoutSegment in _layoutSegments)
			{
				layoutSegment.PropertyChanged += OnLayoutSegmentPropertyChanged;
			}
		}
	}

	public double TotalLayoutLength => LayoutSegments.Sum((AncillaryStructureLayoutSegment s) => s.LayoutLength);

	public int TotalEstimatedCount => LayoutSegments.Sum((AncillaryStructureLayoutSegment s) => s.EstimatedModelCount);

	public string StructureTypeDisplay => GetStructureTypeDisplay();

	public string PositionTypeDisplay => GetPositionTypeDisplay();

	public string LayoutSegmentsCountText => $"{LayoutSegments.Count} 段";

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
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Name);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public AncillaryStructureType StructureType
	{
		get
		{
			return _structureType;
		}
		set
		{
			if (!EqualityComparer<AncillaryStructureType>.Default.Equals(_structureType, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StructureType);
				_structureType = value;
				OnStructureTypeChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StructureType);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public AncillaryPositionType PositionType
	{
		get
		{
			return _positionType;
		}
		set
		{
			if (!EqualityComparer<AncillaryPositionType>.Default.Equals(_positionType, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PositionType);
				_positionType = value;
				OnPositionTypeChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PositionType);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double HorizontalOffset
	{
		get
		{
			return _horizontalOffset;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_horizontalOffset, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HorizontalOffset);
				_horizontalOffset = value;
				OnHorizontalOffsetChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HorizontalOffset);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double ElevationDifference
	{
		get
		{
			return _elevationDifference;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_elevationDifference, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ElevationDifference);
				_elevationDifference = value;
				OnElevationDifferenceChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ElevationDifference);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double Width
	{
		get
		{
			return _width;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_width, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Width);
				_width = value;
				OnWidthChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Width);
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
	public double? SlopeRatio
	{
		get
		{
			return _slopeRatio;
		}
		set
		{
			if (!EqualityComparer<double?>.Default.Equals(_slopeRatio, value))
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

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isEnabled, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsEnabled);
				_isEnabled = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsEnabled);
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

	public event Action? ParameterChanged;

	public AncillaryStructureDisplayModel()
	{
		LayoutSegments = new ObservableCollection<AncillaryStructureLayoutSegment>();
		LayoutSegments.CollectionChanged += OnLayoutSegmentsChanged;
	}

	private void OnLayoutSegmentsChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.NewItems != null)
		{
			foreach (AncillaryStructureLayoutSegment newItem in e.NewItems)
			{
				newItem.PropertyChanged += OnLayoutSegmentPropertyChanged;
			}
		}
		if (e.OldItems != null)
		{
			foreach (AncillaryStructureLayoutSegment oldItem in e.OldItems)
			{
				oldItem.PropertyChanged -= OnLayoutSegmentPropertyChanged;
			}
		}
		OnPropertyChanged("TotalLayoutLength");
		OnPropertyChanged("TotalEstimatedCount");
		OnPropertyChanged("LayoutSegmentsCountText");
	}

	private void OnLayoutSegmentPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "StartStationKm" || e.PropertyName == "EndStationKm" || e.PropertyName == "ModelLength" || e.PropertyName == "LayoutLength" || e.PropertyName == "EstimatedModelCount")
		{
			OnPropertyChanged("TotalLayoutLength");
			OnPropertyChanged("TotalEstimatedCount");
		}
	}

	private static string GetRainbowColor(int index)
	{
		return RainbowColors[index % RainbowColors.Length];
	}

	public static AncillaryStructureDisplayModel CreateEmpty(int order = 0)
	{
		return new AncillaryStructureDisplayModel
		{
			Structure = new AncillaryStructure
			{
				Order = order
			},
			Name = $"附属结构{order + 1}",
			StructureType = AncillaryStructureType.Rectangle,
			PositionType = AncillaryPositionType.Both,
			HorizontalOffset = 0.0,
			ElevationDifference = 150.0,
			Width = 2000.0,
			Height = 150.0,
			SlopeRatio = null,
			Color = GetRainbowColor(order),
			IsEnabled = true,
			Order = order
		};
	}

	public static AncillaryStructureDisplayModel FromStructure(AncillaryStructure structure)
	{
		return new AncillaryStructureDisplayModel
		{
			Structure = structure,
			Name = structure.Name,
			StructureType = structure.StructureType,
			PositionType = structure.PositionType,
			HorizontalOffset = structure.HorizontalOffset,
			ElevationDifference = structure.ElevationDifference,
			Width = structure.Width,
			Height = structure.Height,
			SlopeRatio = structure.SlopeRatio,
			Color = structure.Color,
			IsEnabled = structure.IsEnabled,
			Order = structure.Order
		};
	}

	public AncillaryStructure ToStructure()
	{
		return new AncillaryStructure
		{
			Id = Structure.Id,
			Name = Name,
			StructureType = StructureType,
			PositionType = PositionType,
			HorizontalOffset = HorizontalOffset,
			ElevationDifference = ElevationDifference,
			Width = Width,
			Height = Height,
			SlopeRatio = SlopeRatio,
			Color = Color,
			IsEnabled = IsEnabled,
			Order = Order
		};
	}

	public override string ToString()
	{
		string value = PositionType switch
		{
			AncillaryPositionType.Left => "左", 
			AncillaryPositionType.Right => "右", 
			AncillaryPositionType.Both => "两侧", 
			_ => "", 
		};
		string value2 = StructureType switch
		{
			AncillaryStructureType.Trapezoid => "梯形", 
			AncillaryStructureType.Rectangle => "矩形", 
			AncillaryStructureType.RectangleHollow => "矩形空心", 
			AncillaryStructureType.SingleSlopeSurface => "单坡面层", 
			AncillaryStructureType.DoubleSlopeSurface => "双坡面层", 
			AncillaryStructureType.RoundedCurb => "圆角路沿石", 
			_ => "其他", 
		};
		return $"{Name} ({value}{value2}, 宽:{Width}mm, 偏移距路面边缘:{HorizontalOffset}mm)";
	}

	public string GetStructureTypeDisplay()
	{
		return StructureType switch
		{
			AncillaryStructureType.Trapezoid => "梯形", 
			AncillaryStructureType.Rectangle => "矩形", 
			AncillaryStructureType.RectangleHollow => "矩形空心", 
			AncillaryStructureType.SingleSlopeSurface => "单坡面层", 
			AncillaryStructureType.DoubleSlopeSurface => "双坡面层", 
			AncillaryStructureType.RoundedCurb => "圆角路沿石", 
			_ => "其他", 
		};
	}

	public string GetPositionTypeDisplay()
	{
		return PositionType switch
		{
			AncillaryPositionType.Left => "左侧", 
			AncillaryPositionType.Right => "右侧", 
			AncillaryPositionType.Both => "两侧", 
			_ => "", 
		};
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnStructureTypeChanged(AncillaryStructureType value)
	{
		OnPropertyChanged("StructureTypeIndex");
		ParameterChanged?.Invoke();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnPositionTypeChanged(AncillaryPositionType value)
	{
		OnPropertyChanged("PositionTypeIndex");
		ParameterChanged?.Invoke();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnHorizontalOffsetChanged(double value)
	{
		ParameterChanged?.Invoke();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnElevationDifferenceChanged(double value)
	{
		ParameterChanged?.Invoke();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnWidthChanged(double value)
	{
		ParameterChanged?.Invoke();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnHeightChanged(double value)
	{
		ParameterChanged?.Invoke();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSlopeRatioChanged(double? value)
	{
		ParameterChanged?.Invoke();
	}
}
