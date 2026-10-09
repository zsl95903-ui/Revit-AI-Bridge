using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.ViewModels;

public class LayerPreviewData : ObservableObject
{
	[ObservableProperty]
	private double _strokeThickness = 0.8;

	public PointCollection Points { get; set; } = new PointCollection();

	public string FillColor { get; set; } = "#4CAF50";

	public string Name { get; set; } = string.Empty;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double StrokeThickness
	{
		get
		{
			return _strokeThickness;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_strokeThickness, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StrokeThickness);
				_strokeThickness = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StrokeThickness);
			}
		}
	}
}
