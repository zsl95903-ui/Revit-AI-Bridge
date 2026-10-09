using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using RevitAi.Abstractions.FamilyLibrary;

namespace RevitAi.UI.Models;

public class FamilyCategoryNode : INotifyPropertyChanged
{
	private bool _isSelected;

	public FamilyCategory Category { get; }

	public string Name => Category.Name;

	public Guid Id => Category.Id;

	public Guid? ParentId => Category.ParentId;

	public int DisplayOrder => Category.DisplayOrder;

	public string? IconName => Category.IconName;

	public ObservableCollection<FamilyCategoryNode> SubCategories { get; }

	public int Count { get; set; }

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
			}
		}
	}

	public string FullPath { get; set; } = string.Empty;

	public event PropertyChangedEventHandler? PropertyChanged;

	public FamilyCategoryNode(FamilyCategory category)
	{
		Category = category;
		SubCategories = new ObservableCollection<FamilyCategoryNode>();
	}
}
