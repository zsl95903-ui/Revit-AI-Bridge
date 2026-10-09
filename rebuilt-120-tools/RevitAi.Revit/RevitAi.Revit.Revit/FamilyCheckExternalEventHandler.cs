using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Revit;

public sealed class FamilyCheckExternalEventHandler : IExternalEventHandler
{
	[CompilerGenerated]
	public sealed class Class338
	{
		public FamilyCheckRequest familyCheckRequest_0;

		internal bool method_0(Family family_0)
		{
			return ((Element)family_0).Name.Equals(familyCheckRequest_0.FamilyName, StringComparison.OrdinalIgnoreCase);
		}
	}

	public string GetName()
	{
		return "Family Check";
	}

	public void Execute(UIApplication app)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		FamilyCheckRequest familyCheckRequest_0 = FamilyCheckRequestManager.GetAndClearRequest();
		if (familyCheckRequest_0 == null)
		{
			return;
		}
		try
		{
			UIDocument activeUIDocument = app.ActiveUIDocument;
			Document val = ((activeUIDocument != null) ? activeUIDocument.Document : null);
			if (val == null)
			{
				Logger.Warning("[FamilyCheckExternalEventHandler] 没有活动文档");
				familyCheckRequest_0.TaskSource.SetResult(result: false);
				return;
			}
			if (string.IsNullOrWhiteSpace(familyCheckRequest_0.FamilyName))
			{
				Logger.Warning("[FamilyCheckExternalEventHandler] 族名称为空");
				familyCheckRequest_0.TaskSource.SetResult(result: false);
				return;
			}
			Family val2 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(Family))).Cast<Family>().FirstOrDefault((Family family_0) => ((Element)family_0).Name.Equals(familyCheckRequest_0.FamilyName, StringComparison.OrdinalIgnoreCase));
			bool result = val2 != null;
			familyCheckRequest_0.TaskSource.SetResult(result);
		}
		catch (Exception ex)
		{
			Logger.Error("[FamilyCheckExternalEventHandler] 检查族异常", ex);
			familyCheckRequest_0.TaskSource.SetResult(result: false);
		}
	}
}
