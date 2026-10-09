using RevitAi.UI.Models;

namespace RevitAi.UI.ViewModels;

public class DefinePierViewModel : BridgeComponentDefinitionViewModelBase
{
	public override string ComponentTypeName => "墩柱";

	protected override void LoadBuiltInStyles()
	{
		base.AvailableStyles.Clear();
		var obj = new[]
		{
			new
			{
				Name = "圆形墩柱",
				Image = "Pier1.png"
			},
			new
			{
				Name = "矩形墩柱",
				Image = "Pier2.png"
			},
			new
			{
				Name = "薄壁墩",
				Image = "Pier3.png"
			},
			new
			{
				Name = "双柱式墩",
				Image = "Pier4.png"
			},
			new
			{
				Name = "三柱式墩",
				Image = "Pier5.png"
			},
			new
			{
				Name = "Y型墩",
				Image = "Pier6.png"
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
				ImagePath = "/RevitAi.UI;component/Resources/BridgeComponents/Pier/" + anon.Image,
				IsBuiltIn = true,
				Description = anon.Name + "的说明"
			});
			num++;
		}
	}
}
