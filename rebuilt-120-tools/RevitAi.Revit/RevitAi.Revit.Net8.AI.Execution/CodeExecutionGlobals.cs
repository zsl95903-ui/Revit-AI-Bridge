using System.Diagnostics;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.AI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAi.Revit.Net8.AI.Execution;

public sealed class CodeExecutionGlobals
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private readonly Document document_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private readonly UIDocument uidocument_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private readonly CodeExecutionLogger codeExecutionLogger_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private readonly IAICodeToolInvoker? iaicodeToolInvoker_0;

	public Document doc
	{
		[CompilerGenerated]
		get
		{
			return document_0;
		}
	}

	public UIDocument uidoc
	{
		[CompilerGenerated]
		get
		{
			return uidocument_0;
		}
	}

	public CodeExecutionLogger logger
	{
		[CompilerGenerated]
		get
		{
			return codeExecutionLogger_0;
		}
	}

	public IAICodeToolInvoker? tools
	{
		[CompilerGenerated]
		get
		{
			return iaicodeToolInvoker_0;
		}
	}

	public CodeExecutionGlobals(Document document, UIDocument uiDocument)
	{
		document_0 = document;
		uidocument_0 = uiDocument;
		codeExecutionLogger_0 = new CodeExecutionLogger();
		iaicodeToolInvoker_0 = null;
	}

	public CodeExecutionGlobals(Document document, UIDocument uiDocument, IAICodeToolInvoker? toolInvoker)
	{
		document_0 = document;
		uidocument_0 = uiDocument;
		codeExecutionLogger_0 = new CodeExecutionLogger();
		iaicodeToolInvoker_0 = toolInvoker;
	}
}
