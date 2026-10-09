using System.Runtime.CompilerServices;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitAi.UI.ViewModels;

public sealed class CrossSectionLine : ObservableObject
{
	[CompilerGenerated]
	private Point _003CCenterPoint_003Ek__BackingField;

	[CompilerGenerated]
	private Point _003CLeftPoint_003Ek__BackingField;

	[CompilerGenerated]
	private Point _003CRightPoint_003Ek__BackingField;

	private bool _isSelected;

	public Point CenterPoint
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CCenterPoint_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CCenterPoint_003Ek__BackingField = value;
		}
	}

	public Point LeftPoint
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CLeftPoint_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CLeftPoint_003Ek__BackingField = value;
		}
	}

	public Point RightPoint
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CRightPoint_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CRightPoint_003Ek__BackingField = value;
		}
	}

	public string StationDisplay { get; set; } = string.Empty;

	public double RoadWidth { get; set; }

	public double OffsetAngle { get; set; }

	public double StrokeThickness { get; set; } = 1.0;

	public double StationKm { get; set; }

	public bool IsSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			SetProperty(ref _isSelected, value, "IsSelected");
		}
	}
}
