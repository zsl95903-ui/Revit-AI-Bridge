using System.Runtime.CompilerServices;
using System.Windows;

namespace RevitAi.UI.ViewModels;

public sealed class StationMarker
{
	[CompilerGenerated]
	private Point _003CPosition_003Ek__BackingField;

	public Point Position
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CPosition_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CPosition_003Ek__BackingField = value;
		}
	}

	public string DisplayText { get; set; } = string.Empty;

	public double StationKm { get; set; }

	public string Color { get; set; } = "#FF0000";

	public bool IsSelected { get; set; }

	public bool IsSpecial { get; set; }
}
