using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.Models;

public class CreditPackageOption : ObservableObject
{
	[ObservableProperty]
	private int _credits;

	[ObservableProperty]
	private bool _isSelected;

	[ObservableProperty]
	private decimal? _customAmount;

	public string DisplayName { get; set; } = string.Empty;

	public decimal OriginalPrice { get; set; }

	public bool IsCustom { get; set; }

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int Credits
	{
		get
		{
			return _credits;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_credits, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Credits);
				_credits = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Credits);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isSelected, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsSelected);
				_isSelected = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsSelected);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public decimal? CustomAmount
	{
		get
		{
			return _customAmount;
		}
		set
		{
			if (!EqualityComparer<decimal?>.Default.Equals(_customAmount, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CustomAmount);
				_customAmount = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CustomAmount);
			}
		}
	}
}
