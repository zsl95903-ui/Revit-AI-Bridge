using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Models.CADAnalysis;
using Autodesk.Revit.DB;

namespace RevitAi.Revit.Revit.PipeNetwork;

public sealed class PipeNetworkModelingRequest
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private PipeNetworkModelingData? pipeNetworkModelingData_0;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Document? document_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private PipeNetworkModelingOptions? pipeNetworkModelingOptions_0;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<PipeNetworkModelingResult>? action_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private static PipeNetworkModelingRequest? pipeNetworkModelingRequest_0;

	public PipeNetworkModelingData? ModelingData
	{
		[CompilerGenerated]
		get
		{
			return pipeNetworkModelingData_0;
		}
		[CompilerGenerated]
		set
		{
			pipeNetworkModelingData_0 = value;
		}
	}

	public Document? Document
	{
		[CompilerGenerated]
		get
		{
			return document_0;
		}
		[CompilerGenerated]
		set
		{
			document_0 = value;
		}
	}

	public PipeNetworkModelingOptions? Options
	{
		[CompilerGenerated]
		get
		{
			return pipeNetworkModelingOptions_0;
		}
		[CompilerGenerated]
		set
		{
			pipeNetworkModelingOptions_0 = value;
		}
	}

	public Action<PipeNetworkModelingResult>? OnCompleted
	{
		[CompilerGenerated]
		get
		{
			return action_0;
		}
		[CompilerGenerated]
		set
		{
			action_0 = value;
		}
	}

	public static PipeNetworkModelingRequest? CurrentRequest
	{
		[CompilerGenerated]
		get
		{
			return pipeNetworkModelingRequest_0;
		}
		[CompilerGenerated]
		set
		{
			pipeNetworkModelingRequest_0 = value;
		}
	}
}
