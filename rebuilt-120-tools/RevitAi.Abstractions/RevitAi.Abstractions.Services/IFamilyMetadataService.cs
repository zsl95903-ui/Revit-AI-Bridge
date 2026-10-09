using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface IFamilyMetadataService
{
	IEnumerable<FamilyInfo> GetAllFamilies(object document);

	FamilyMetadataExportResult ExportMetadata(object document, object family, string? exportDirectory);

	(int SuccessCount, int FailCount) ExportMetadataBatch(object document, IEnumerable<object> families, string? exportDirectory, Action<int, int, string>? progressCallback = null);

	string? SaveFamilyToFile(object document, object family, string targetPath, bool overwriteExisting = true);

	FamilyMetadataExportResult ExportFamilyComplete(object document, object family, string exportDirectory, bool saveFamilyFile = false, bool exportThumbnail = true, bool exportMetadata = true, bool overwrite = false);

	FamilyMetadataExportResult ExportFamilyFile(string familyFilePath, string exportDirectory, bool saveFamilyFile = false, bool exportThumbnail = true, bool exportMetadata = true, bool overwrite = false);
}
