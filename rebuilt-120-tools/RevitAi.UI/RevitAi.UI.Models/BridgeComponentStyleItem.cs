using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.Models;

public class BridgeComponentStyleItem : ObservableObject
{
	[ObservableProperty]
	private string _name = string.Empty;

	[ObservableProperty]
	private string _imagePath = string.Empty;

	[ObservableProperty]
	private bool _isBuiltIn = true;

	[ObservableProperty]
	private string _description = string.Empty;

	public string Id { get; set; } = Guid.NewGuid().ToString();

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
	public string ImagePath
	{
		get
		{
			return _imagePath;
		}
		[MemberNotNull("_imagePath")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_imagePath, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ImagePath);
				_imagePath = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ImagePath);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsBuiltIn
	{
		get
		{
			return _isBuiltIn;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isBuiltIn, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsBuiltIn);
				_isBuiltIn = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsBuiltIn);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Description
	{
		get
		{
			return _description;
		}
		[MemberNotNull("_description")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_description, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Description);
				_description = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Description);
			}
		}
	}
}
