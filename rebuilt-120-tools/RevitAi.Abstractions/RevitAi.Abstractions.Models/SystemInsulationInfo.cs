using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RevitAi.Abstractions.Models;

public class SystemInsulationInfo : INotifyPropertyChanged
{
	private bool _isSelected;

	public int SystemTypeId { get; set; }

	public string SystemTypeName { get; set; } = "";

	public SystemElementType ElementType { get; set; }

	public string ElementTypeDisplay
	{
		get
		{
			if (ElementType != SystemElementType.Pipe)
			{
				return "风管";
			}
			return "水管";
		}
	}

	public string ElementTypeColor
	{
		get
		{
			if (ElementType != SystemElementType.Pipe)
			{
				return "#FF6B35";
			}
			return "#007ACC";
		}
	}

	public int ElementCount { get; set; }

	public int InsulatedCount { get; set; }

	public int UninsulatedCount => ElementCount - InsulatedCount;

	public string ThicknessInput { get; set; } = "0";

	public string MaterialInput { get; set; } = "";

	public bool OverrideExisting { get; set; } = true;

	public bool IsSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			if (_isSelected != value)
			{
				_isSelected = value;
				OnPropertyChanged("IsSelected");
				OnPropertyChanged("UninsulatedCount");
			}
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
