using RevitAi.UI.Models;

namespace RevitAi.UI.ViewModels;

public class DefineBearingViewModel : BridgeComponentDefinitionViewModelBase
{
	public override string ComponentTypeName => "支座";

	protected override void LoadBuiltInStyles()
	{
		base.AvailableStyles.Clear();
		var obj = new[]
		{
			new
			{
				Name = "板式橡胶支座",
				Image = "Bearing1.png"
			},
			new
			{
				Name = "盆式橡胶支座",
				Image = "Bearing2.png"
			},
			new
			{
				Name = "球形钢支座",
				Image = "Bearing3.png"
			},
			new
			{
				Name = "铰轴滑板支座",
				Image = "Bearing4.png"
			},
			new
			{
				Name = "球型支座",
				Image = "Bearing5.png"
			},
			new
			{
				Name = "拉压支座",
				Image = "Bearing6.png"
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
				ImagePath = "/RevitAi.UI;component/Resources/BridgeComponents/Bearing/" + anon.Image,
				IsBuiltIn = true,
				Description = anon.Name + "的说明"
			});
			num++;
		}
	}
}
