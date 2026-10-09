using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Infrastructure;
using Autodesk.Revit.DB;

namespace RevitAi.Revit.RoadCenterline;

public sealed class RoadCenterlineExternalEventRequest
{
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private List<RoadCenterlinePoint3D> list_0 = new List<RoadCenterlinePoint3D>();

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private RoadProject? roadProject_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private Document? document_0;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool bool_0;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string? string_0;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private double double_0;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string? string_1;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<Exception?, bool>? action_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private ElementId? elementId_0;

	public List<RoadCenterlinePoint3D> Points3D
	{
		[CompilerGenerated]
		get
		{
			return list_0;
		}
		[CompilerGenerated]
		set
		{
			list_0 = value;
		}
	}

	public RoadProject? RoadProject
	{
		[CompilerGenerated]
		get
		{
			return roadProject_0;
		}
		[CompilerGenerated]
		set
		{
			roadProject_0 = value;
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

	public bool CreateAnnotations
	{
		[CompilerGenerated]
		get
		{
			return bool_0;
		}
		[CompilerGenerated]
		set
		{
			bool_0 = value;
		}
	}

	public string? AnnotationFamilyPath
	{
		[CompilerGenerated]
		get
		{
			return string_0;
		}
		[CompilerGenerated]
		set
		{
			string_0 = value;
		}
	}

	public double AnnotationIntervalFeet
	{
		[CompilerGenerated]
		get
		{
			return double_0;
		}
		[CompilerGenerated]
		set
		{
			double_0 = value;
		}
	}

	public string? RoadProjectName
	{
		[CompilerGenerated]
		get
		{
			return string_1;
		}
		[CompilerGenerated]
		set
		{
			string_1 = value;
		}
	}

	public Action<Exception?, bool>? OnCompleted
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

	public ElementId? CreatedCurveId
	{
		[CompilerGenerated]
		get
		{
			return elementId_0;
		}
		[CompilerGenerated]
		set
		{
			elementId_0 = value;
		}
	}
}
