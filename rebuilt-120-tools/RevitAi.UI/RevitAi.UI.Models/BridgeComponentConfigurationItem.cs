using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitAi.UI.Models;

public class BridgeComponentConfigurationItem : ObservableObject
{
	private int _number;

	private string _station = "0+000";

	private UserDefinedBridgeComponent? _bridgeType;

	private string _bridgeParameters = string.Empty;

	private UserDefinedBridgeComponent? _foundationType;

	private string _foundationParameters = string.Empty;

	private UserDefinedBridgeComponent? _pierType;

	private string _pierParameters = string.Empty;

	private UserDefinedBridgeComponent? _beamType;

	private string _beamParameters = string.Empty;

	private UserDefinedBridgeComponent? _bearingType;

	private string _bearingParameters = string.Empty;

	public int Number
	{
		get
		{
			return _number;
		}
		set
		{
			SetProperty(ref _number, value, "Number");
		}
	}

	public string Station
	{
		get
		{
			return _station;
		}
		set
		{
			SetProperty(ref _station, value, "Station");
		}
	}

	public UserDefinedBridgeComponent? BridgeType
	{
		get
		{
			return _bridgeType;
		}
		set
		{
			SetProperty(ref _bridgeType, value, "BridgeType");
		}
	}

	public string BridgeParameters
	{
		get
		{
			return _bridgeParameters;
		}
		set
		{
			SetProperty(ref _bridgeParameters, value, "BridgeParameters");
		}
	}

	public UserDefinedBridgeComponent? FoundationType
	{
		get
		{
			return _foundationType;
		}
		set
		{
			SetProperty(ref _foundationType, value, "FoundationType");
		}
	}

	public string FoundationParameters
	{
		get
		{
			return _foundationParameters;
		}
		set
		{
			SetProperty(ref _foundationParameters, value, "FoundationParameters");
		}
	}

	public UserDefinedBridgeComponent? PierType
	{
		get
		{
			return _pierType;
		}
		set
		{
			SetProperty(ref _pierType, value, "PierType");
		}
	}

	public string PierParameters
	{
		get
		{
			return _pierParameters;
		}
		set
		{
			SetProperty(ref _pierParameters, value, "PierParameters");
		}
	}

	public UserDefinedBridgeComponent? BeamType
	{
		get
		{
			return _beamType;
		}
		set
		{
			SetProperty(ref _beamType, value, "BeamType");
		}
	}

	public string BeamParameters
	{
		get
		{
			return _beamParameters;
		}
		set
		{
			SetProperty(ref _beamParameters, value, "BeamParameters");
		}
	}

	public UserDefinedBridgeComponent? BearingType
	{
		get
		{
			return _bearingType;
		}
		set
		{
			SetProperty(ref _bearingType, value, "BearingType");
		}
	}

	public string BearingParameters
	{
		get
		{
			return _bearingParameters;
		}
		set
		{
			SetProperty(ref _bearingParameters, value, "BearingParameters");
		}
	}
}
