using System.Collections.Generic;
using Autodesk.Revit.DB.Architecture;

namespace RevitAi.Revit.Models;

public interface IStairTypeManagerService
{
	StairsType? FindCastInPlaceStairType();

	StairsType? CreateCustomStairType(string baseStairTypeName, string newStairTypeName, double runThicknessMm, double landingThicknessMm);

	IList<string> GetStairTypeNames(bool filterCastInPlace = true);

	StairsType? FindStairTypeByName(string typeName);
}
