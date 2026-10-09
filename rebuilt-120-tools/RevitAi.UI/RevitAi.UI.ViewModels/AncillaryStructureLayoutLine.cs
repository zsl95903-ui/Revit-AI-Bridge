using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace RevitAi.UI.ViewModels;

public sealed class AncillaryStructureLayoutLine
{
	[CompilerGenerated]
	private Point _003CStartPoint_003Ek__BackingField;

	[CompilerGenerated]
	private Point _003CEndPoint_003Ek__BackingField;

	public Point StartPoint
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CStartPoint_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CStartPoint_003Ek__BackingField = value;
		}
	}

	public Point EndPoint
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CEndPoint_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CEndPoint_003Ek__BackingField = value;
		}
	}

	public string Color { get; set; } = "#FF0000";

	public double StrokeThickness { get; set; } = 2.0;

	public DoubleCollection StrokeDashArray { get; set; } = new DoubleCollection();

	public string StructureName { get; set; } = string.Empty;

	public string StartStationDisplay { get; set; } = string.Empty;

	public string EndStationDisplay { get; set; } = string.Empty;
}
