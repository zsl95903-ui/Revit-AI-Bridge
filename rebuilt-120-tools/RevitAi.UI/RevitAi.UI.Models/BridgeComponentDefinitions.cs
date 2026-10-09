using System.Collections.ObjectModel;

namespace RevitAi.UI.Models;

public class BridgeComponentDefinitions
{
	public ObservableCollection<UserDefinedBridgeComponent> Piles { get; set; } = new ObservableCollection<UserDefinedBridgeComponent>();

	public ObservableCollection<UserDefinedBridgeComponent> Foundations { get; set; } = new ObservableCollection<UserDefinedBridgeComponent>();

	public ObservableCollection<UserDefinedBridgeComponent> Piers { get; set; } = new ObservableCollection<UserDefinedBridgeComponent>();

	public ObservableCollection<UserDefinedBridgeComponent> Beams { get; set; } = new ObservableCollection<UserDefinedBridgeComponent>();

	public ObservableCollection<UserDefinedBridgeComponent> Bearings { get; set; } = new ObservableCollection<UserDefinedBridgeComponent>();

	public ObservableCollection<UserDefinedBridgeComponent> BridgeTypes { get; set; } = new ObservableCollection<UserDefinedBridgeComponent>();

	public int TotalCount => Piles.Count + Foundations.Count + Piers.Count + Beams.Count + Bearings.Count + BridgeTypes.Count;
}
