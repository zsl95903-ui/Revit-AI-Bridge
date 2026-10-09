using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.Models;

public class UserDefinedBridgeComponent : ObservableObject
{
	[ObservableProperty]
	private string _name = string.Empty;

	[ObservableProperty]
	private BridgeComponentStyleItem? _selectedStyle;

	[ObservableProperty]
	private string _componentType = string.Empty;

	public string Id { get; set; } = Guid.NewGuid().ToString();

	public DateTime CreatedAt { get; set; } = DateTime.Now;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Name
	{
		get
		{
			return _name;
		}
		[MemberNotNull("_name")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_name, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Name);
				_name = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Name);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public BridgeComponentStyleItem? SelectedStyle
	{
		get
		{
			return _selectedStyle;
		}
		set
		{
			if (!EqualityComparer<BridgeComponentStyleItem>.Default.Equals(_selectedStyle, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedStyle);
				_selectedStyle = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedStyle);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string ComponentType
	{
		get
		{
			return _componentType;
		}
		[MemberNotNull("_componentType")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_componentType, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ComponentType);
				_componentType = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ComponentType);
			}
		}
	}
}
