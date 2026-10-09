using RevitAi.UI.Models;

namespace RevitAi.UI.ViewModels;

public class DefineBeamViewModel : BridgeComponentDefinitionViewModelBase
{
	public override string ComponentTypeName => "盖梁";

	protected override void LoadBuiltInStyles()
	{
		base.AvailableStyles.Clear();
		var obj = new[]
		{
			new
			{
				Name = "矩形盖梁",
				Image = "Beam1.png"
			},
			new
			{
				Name = "T型盖梁",
				Image = "Beam2.png"
			},
			new
			{
				Name = "倒T型盖梁",
				Image = "Beam3.png"
			},
			new
			{
				Name = "箱型盖梁",
				Image = "Beam4.png"
			},
			new
			{
				Name = "预应力盖梁",
				Image = "Beam5.png"
			},
			new
			{
				Name = "组合盖梁",
				Image = "Beam6.png"
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
				ImagePath = "/RevitAi.UI;component/Resources/BridgeComponents/Beam/" + anon.Image,
				IsBuiltIn = true,
				Description = anon.Name + "的说明"
			});
			num++;
		}
	}
}
