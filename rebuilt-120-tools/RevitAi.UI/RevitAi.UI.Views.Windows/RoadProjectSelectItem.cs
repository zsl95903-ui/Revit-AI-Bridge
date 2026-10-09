using System.ComponentModel;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Infrastructure;

namespace RevitAi.UI.Views.Windows;

public class RoadProjectSelectItem : INotifyPropertyChanged
{
	private bool _isSelected = true;

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
			}
		}
	}

	public RoadProject Project { get; set; }

	public string Name => Project.Name;

	public event PropertyChangedEventHandler? PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
