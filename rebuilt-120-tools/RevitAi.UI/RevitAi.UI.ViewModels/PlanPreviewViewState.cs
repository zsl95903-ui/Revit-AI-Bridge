using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitAi.UI.ViewModels;

public sealed class PlanPreviewViewState : ObservableObject
{
	private double _scale = 1.0;

	private double _panX;

	private double _panY;

	private double _rotationAngle;

	private bool _isInitialized;

	public double Scale
	{
		get
		{
			return _scale;
		}
		set
		{
			SetProperty(ref _scale, value, "Scale");
		}
	}

	public double PanX
	{
		get
		{
			return _panX;
		}
		set
		{
			SetProperty(ref _panX, value, "PanX");
		}
	}

	public double PanY
	{
		get
		{
			return _panY;
		}
		set
		{
			SetProperty(ref _panY, value, "PanY");
		}
	}

	public double RotationAngle
	{
		get
		{
			return _rotationAngle;
		}
		set
		{
			SetProperty(ref _rotationAngle, value, "RotationAngle");
		}
	}

	public bool IsInitialized
	{
		get
		{
			return _isInitialized;
		}
		set
		{
			SetProperty(ref _isInitialized, value, "IsInitialized");
		}
	}

	public void Reset()
	{
		Scale = 1.0;
		PanX = 0.0;
		PanY = 0.0;
		RotationAngle = 0.0;
		IsInitialized = false;
	}
}
