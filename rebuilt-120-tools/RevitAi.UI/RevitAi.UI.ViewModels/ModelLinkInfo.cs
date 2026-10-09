using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitAi.UI.ViewModels;

public sealed class ModelLinkInfo : ObservableObject
{
	private bool _isSelected;

	private bool _isLoadedDesired;

	private double _positionX;

	private double _positionY;

	private double _positionZ;

	private double _rotation;

	private string? _filePath;

	private bool _hasUnsavedChanges;

	private bool _originalIsLoadedDesired;

	private double _originalPositionX;

	private double _originalPositionY;

	private double _originalPositionZ;

	private double _originalRotation;

	private bool _isAutoSelected;

	internal bool PositionSuccessfullyUpdated { get; set; }

	internal bool RotationSuccessfullyUpdated { get; set; }

	public string Id { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public string Status { get; set; } = "已载入";

	public bool IsLoaded { get; set; } = true;

	public bool IsLoadedDesired
	{
		get
		{
			return _isLoadedDesired;
		}
		set
		{
			if (SetProperty(ref _isLoadedDesired, value, "IsLoadedDesired"))
			{
				UpdateHasUnsavedChanges();
			}
		}
	}

	public string FilePath
	{
		get
		{
			return _filePath ?? string.Empty;
		}
		set
		{
			if (SetProperty(ref _filePath, value, "FilePath"))
			{
				UpdateHasUnsavedChanges();
			}
		}
	}

	public string OriginalFilePath { get; set; } = string.Empty;

	public bool IsPathChanged => !string.Equals(FilePath, OriginalFilePath, StringComparison.OrdinalIgnoreCase);

	public double PositionX
	{
		get
		{
			return _positionX;
		}
		set
		{
			if (SetProperty(ref _positionX, value, "PositionX"))
			{
				UpdateHasUnsavedChanges();
			}
		}
	}

	public double PositionY
	{
		get
		{
			return _positionY;
		}
		set
		{
			if (SetProperty(ref _positionY, value, "PositionY"))
			{
				UpdateHasUnsavedChanges();
			}
		}
	}

	public double PositionZ
	{
		get
		{
			return _positionZ;
		}
		set
		{
			if (SetProperty(ref _positionZ, value, "PositionZ"))
			{
				UpdateHasUnsavedChanges();
			}
		}
	}

	public double Rotation
	{
		get
		{
			return _rotation;
		}
		set
		{
			if (SetProperty(ref _rotation, value, "Rotation"))
			{
				UpdateHasUnsavedChanges();
			}
		}
	}

	public bool HasUnsavedChanges
	{
		get
		{
			return _hasUnsavedChanges;
		}
		set
		{
			SetProperty(ref _hasUnsavedChanges, value, "HasUnsavedChanges");
		}
	}

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
				_isAutoSelected = false;
			}
			SetProperty(ref _isSelected, value, "IsSelected");
		}
	}

	public bool IsPositionChanged
	{
		get
		{
			if (!(Math.Abs(PositionX - _originalPositionX) > 0.001) && !(Math.Abs(PositionY - _originalPositionY) > 0.001))
			{
				return Math.Abs(PositionZ - _originalPositionZ) > 0.001;
			}
			return true;
		}
	}

	public bool IsRotationChanged => Math.Abs(Rotation - _originalRotation) > 0.01;

	public object? LinkInstance { get; set; }

	public object? LinkType { get; set; }

	public int LinkTypeId { get; set; }

	public void UpdateHasUnsavedChanges()
	{
		bool flag = (HasUnsavedChanges = IsLoadedDesired != _originalIsLoadedDesired || IsPathChanged || Math.Abs(PositionX - _originalPositionX) > 0.001 || Math.Abs(PositionY - _originalPositionY) > 0.001 || Math.Abs(PositionZ - _originalPositionZ) > 0.001 || Math.Abs(Rotation - _originalRotation) > 0.01);
		if (flag && !IsSelected)
		{
			_isAutoSelected = true;
			_isSelected = true;
			OnPropertyChanged("IsSelected");
		}
		else if (!flag && _isAutoSelected && IsSelected)
		{
			_isAutoSelected = false;
			_isSelected = false;
			OnPropertyChanged("IsSelected");
		}
	}

	public void MarkAsSaved()
	{
		_originalIsLoadedDesired = IsLoadedDesired;
		_originalPositionX = PositionX;
		_originalPositionY = PositionY;
		_originalPositionZ = PositionZ;
		_originalRotation = Rotation;
		HasUnsavedChanges = false;
		UpdateHasUnsavedChanges();
	}

	public void InitializeOriginalValues()
	{
		_originalIsLoadedDesired = IsLoadedDesired;
		_originalPositionX = PositionX;
		_originalPositionY = PositionY;
		_originalPositionZ = PositionZ;
		_originalRotation = Rotation;
	}

	public void InitializeAllValues(string id, string name, string filePath, string status, bool isLoaded, bool isLoadedDesired, double positionX, double positionY, double positionZ, double rotation, object? linkInstance, object? linkType, int linkTypeId)
	{
		_isSelected = false;
		_isAutoSelected = false;
		_isLoadedDesired = isLoadedDesired;
		_positionX = positionX;
		_positionY = positionY;
		_positionZ = positionZ;
		_rotation = rotation;
		_filePath = filePath;
		_hasUnsavedChanges = false;
		Id = id;
		Name = name;
		OriginalFilePath = filePath;
		Status = status;
		IsLoaded = isLoaded;
		LinkInstance = linkInstance;
		LinkType = linkType;
		LinkTypeId = linkTypeId;
		_originalIsLoadedDesired = isLoadedDesired;
		_originalPositionX = positionX;
		_originalPositionY = positionY;
		_originalPositionZ = positionZ;
		_originalRotation = rotation;
	}
}
