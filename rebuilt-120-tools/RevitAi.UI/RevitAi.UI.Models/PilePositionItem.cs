using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.Models;

public class PilePositionItem : ObservableObject
{
	[ObservableProperty]
	private string _pierNumber = string.Empty;

	[ObservableProperty]
	private string _pileNumber = string.Empty;

	[ObservableProperty]
	private UserDefinedBridgeComponent? _pileType;

	[ObservableProperty]
	private string _xCoordinate = string.Empty;

	[ObservableProperty]
	private string _yCoordinate = string.Empty;

	[ObservableProperty]
	private string _pileLength = string.Empty;

	public int Number { get; set; }

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string PierNumber
	{
		get
		{
			return _pierNumber;
		}
		[MemberNotNull("_pierNumber")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_pierNumber, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PierNumber);
				_pierNumber = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PierNumber);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string PileNumber
	{
		get
		{
			return _pileNumber;
		}
		[MemberNotNull("_pileNumber")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_pileNumber, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PileNumber);
				_pileNumber = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PileNumber);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public UserDefinedBridgeComponent? PileType
	{
		get
		{
			return _pileType;
		}
		set
		{
			if (!EqualityComparer<UserDefinedBridgeComponent>.Default.Equals(_pileType, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PileType);
				_pileType = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PileType);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string XCoordinate
	{
		get
		{
			return _xCoordinate;
		}
		[MemberNotNull("_xCoordinate")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_xCoordinate, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.XCoordinate);
				_xCoordinate = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.XCoordinate);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string YCoordinate
	{
		get
		{
			return _yCoordinate;
		}
		[MemberNotNull("_yCoordinate")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_yCoordinate, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.YCoordinate);
				_yCoordinate = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.YCoordinate);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string PileLength
	{
		get
		{
			return _pileLength;
		}
		[MemberNotNull("_pileLength")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_pileLength, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PileLength);
				_pileLength = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PileLength);
			}
		}
	}
}
