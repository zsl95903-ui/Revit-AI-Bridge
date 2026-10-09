using System.Collections.Generic;
using RevitAi.UI.Models;

namespace RevitAi.UI.ViewModels;

public class DefineFoundationViewModel : BridgeComponentDefinitionViewModelBase
{
	public override string ComponentTypeName => "基础部件";

	protected override void LoadBuiltInStyles()
	{
		base.AvailableStyles.Clear();
		List<(string, string)> obj = new List<(string, string)>
		{
			("扩大基础", "Foundation1.png"),
			("桩基础", "Foundation2.png"),
			("沉井基础", "Foundation3.png"),
			("地下连续墙", "Foundation4.png"),
			("箱型基础", "Foundation5.png"),
			("筏型基础", "Foundation6.png")
		};
		int num = 1;
		foreach (var item in obj)
		{
			base.AvailableStyles.Add(new BridgeComponentStyleItem
			{
				Id = $"{ComponentTypeName}_Style_{num}",
				Name = item.Item1,
				ImagePath = "/RevitAi.UI;component/Resources/BridgeComponents/Foundation/" + item.Item2,
				IsBuiltIn = true,
				Description = item.Item1 + "的说明"
			});
			num++;
		}
	}
}
