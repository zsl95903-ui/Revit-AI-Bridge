using System;
using System.ComponentModel;

namespace RevitAi.UI.Views.Windows;

public class FamilyDisplayItem : INotifyPropertyChanged
{
	private bool _isSelected;

	public object Family { get; set; }

	public string DisplayName { get; set; } = "";

	public string FamilyName { get; set; } = "";

	public string CategoryName { get; set; } = "";

	public bool IsExternalFamily { get; set; }

	public string? FilePath { get; set; }

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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
				SelectionChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	public event EventHandler? SelectionChanged;
}
