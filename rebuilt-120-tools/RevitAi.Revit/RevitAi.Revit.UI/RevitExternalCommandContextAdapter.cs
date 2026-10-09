using System;
using RevitAi.Abstractions.UI;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.UI;

public sealed class RevitExternalCommandContextAdapter : IExternalCommandContext
{
	private readonly ExternalCommandData externalCommandData_0;

	public object? ActiveDocument
	{
		get
		{
			UIDocument activeUIDocument = externalCommandData_0.Application.ActiveUIDocument;
			return (activeUIDocument != null) ? activeUIDocument.Document : null;
		}
	}

	public object? UIApplication => externalCommandData_0.Application;

	public bool IsRevitInternalTrigger => false;

	public RevitExternalCommandContextAdapter(ExternalCommandData commandData)
	{
		externalCommandData_0 = commandData ?? throw new ArgumentNullException("commandData");
	}
}
