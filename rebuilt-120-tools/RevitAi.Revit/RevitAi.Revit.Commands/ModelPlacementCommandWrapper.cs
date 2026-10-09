using Autodesk.Revit.Attributes;
using ns6;

namespace RevitAi.Revit.Commands;

[Transaction(TransactionMode.Manual)]
public class ModelPlacementCommandWrapper : RevitCommandWrapperBase
{
	protected override string CommandName => "ModelPlacementCommand";
}
