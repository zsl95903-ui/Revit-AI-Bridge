using System.Linq;
using System.Reflection;
using System.Text;
using RevitAi.Abstractions.AI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.CodeAnalysis.Scripting;
using ns6;

namespace RevitAi.Revit.Net8.AI.Execution;

public static class ScriptOptionsFactory
{
	public static ScriptOptions Create()
	{
		return ScriptOptions.Default.AddReferences(new Assembly[6]
		{
			typeof(object).Assembly,
			typeof(Enumerable).Assembly,
			typeof(StringBuilder).Assembly,
			typeof(Document).Assembly,
			typeof(UIDocument).Assembly,
			typeof(IAICodeToolInvoker).Assembly
		}).AddImports(new string[12]
		{
			"System",
			"System.Collections.Generic",
			"System.Linq",
			"System.Text",
			"Autodesk.Revit.DB",
			"Autodesk.Revit.DB.Structure",
			"Autodesk.Revit.DB.Architecture",
			"Autodesk.Revit.DB.Mechanical",
			"Autodesk.Revit.DB.Electrical",
			"Autodesk.Revit.DB.Plumbing",
			"Autodesk.Revit.UI",
			"RevitAi.Abstractions.AI"
		});
	}
}
