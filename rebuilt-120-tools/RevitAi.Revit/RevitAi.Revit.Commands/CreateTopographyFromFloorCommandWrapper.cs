using Autodesk.Revit.Attributes;
using ns6;

namespace RevitAi.Revit.Commands;

[Transaction(TransactionMode.Manual)]
public class CreateTopographyFromFloorCommandWrapper : RevitCommandWrapperBase
{
	protected override string CommandName => "CreateTopographyFromFloorCommand";
}
