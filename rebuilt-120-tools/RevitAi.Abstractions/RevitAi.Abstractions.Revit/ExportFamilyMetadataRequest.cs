using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.Revit;

public class ExportFamilyMetadataRequest
{
	public List<FamilyExportItem> Families { get; set; } = new List<FamilyExportItem>();

	public string ExportDirectory { get; set; } = string.Empty;

	public bool SaveFamilyFile { get; set; }

	public bool ExportThumbnail { get; set; }

	public bool ExportMetadata { get; set; }

	public bool Overwrite { get; set; }

	public Action<ExportFamilyMetadataResult>? OnCompleted { get; set; }

	public Action<int, int, string>? OnProgress { get; set; }
}
