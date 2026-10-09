using System.Collections.Generic;
using RevitAi.UI.Models;

namespace RevitAi.UI.ViewModels;

public class DefinePileViewModel : BridgeComponentDefinitionViewModelBase
{
	public override string ComponentTypeName => "桩部件";

	protected override void LoadBuiltInStyles()
	{
		base.AvailableStyles.Clear();
		List<(string, string)> obj = new List<(string, string)>
		{
			("钻孔灌注桩", "Pile1.png"),
			("预制桩", "Pile2.png"),
			("钢管桩", "Pile3.png"),
			("PHC管桩", "Pile4.png"),
			("方桩", "Pile5.png"),
			("木桩", "Pile6.png")
		};
		int num = 1;
		foreach (var item in obj)
		{
			base.AvailableStyles.Add(new BridgeComponentStyleItem
			{
				Id = $"{ComponentTypeName}_Style_{num}",
				Name = item.Item1,
				ImagePath = "/RevitAi.UI;component/Resources/BridgeComponents/Pile/" + item.Item2,
				IsBuiltIn = true,
				Description = item.Item1 + "的说明"
			});
			num++;
		}
	}
}
