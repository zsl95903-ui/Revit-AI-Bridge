using RevitAi.UI.Models;

namespace RevitAi.UI.ViewModels;

public class DefineBridgeTypeViewModel : BridgeComponentDefinitionViewModelBase
{
	public override string ComponentTypeName => "桥型";

	protected override void LoadBuiltInStyles()
	{
		base.AvailableStyles.Clear();
		var obj = new[]
		{
			new
			{
				Name = "梁桥",
				Image = "BridgeType1.png"
			},
			new
			{
				Name = "拱桥",
				Image = "BridgeType2.png"
			},
			new
			{
				Name = "悬索桥",
				Image = "BridgeType3.png"
			},
			new
			{
				Name = "斜拉桥",
				Image = "BridgeType4.png"
			},
			new
			{
				Name = "刚构桥",
				Image = "BridgeType5.png"
			},
			new
			{
				Name = "组合桥",
				Image = "BridgeType6.png"
			}
		};
		int num = 1;
		var array = obj;
		foreach (var anon in array)
		{
			base.AvailableStyles.Add(new BridgeComponentStyleItem
			{
				Id = $"{ComponentTypeName}_Style_{num}",
				Name = anon.Name,
				ImagePath = "/RevitAi.UI;component/Resources/BridgeComponents/BridgeType/" + anon.Image,
				IsBuiltIn = true,
				Description = anon.Name + "的说明"
			});
			num++;
		}
	}
}
