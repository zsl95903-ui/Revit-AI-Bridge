using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Services;
using ns6;

namespace RevitAi.Revit.RoadModeling;

public sealed class RoadModelingExternalEventRequest
{
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private RoadProject roadProject_0 = null;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private bool bool_0 = true;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private int int_0 = 20;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private string? string_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private string string_1 = "AST_R_路基路面_3";

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private bool bool_1;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Dictionary<string, string> dictionary_0 = new Dictionary<string, string>();

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TaskCompletionSource<RoadModelingResult>? taskCompletionSource_0;

	public RoadProject Project
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

	public bool SplitAtIntegerStations
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

	public int IntegerStationInterval
	{
		[CompilerGenerated]
		get
		{
			return int_0;
		}
		[CompilerGenerated]
		set
		{
			int_0 = value;
		}
	}

	public string? FamilyFilePath
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

	public string FamilyName
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

	public bool IsAncillaryStructure
	{
		[CompilerGenerated]
		get
		{
			return bool_1;
		}
		[CompilerGenerated]
		set
		{
			bool_1 = value;
		}
	}

	public Dictionary<string, string> DownloadedFamilyPaths
	{
		[CompilerGenerated]
		get
		{
			return dictionary_0;
		}
		[CompilerGenerated]
		set
		{
			dictionary_0 = value;
		}
	}

	public TaskCompletionSource<RoadModelingResult>? CompletionSource
	{
		[CompilerGenerated]
		get
		{
			return taskCompletionSource_0;
		}
		[CompilerGenerated]
		set
		{
			taskCompletionSource_0 = value;
		}
	}
}
