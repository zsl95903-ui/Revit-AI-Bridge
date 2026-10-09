using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RevitAi.Abstractions.AI;

public interface IFileAttachmentService
{
	Task<FileAttachment> SaveFileAsync(string sourceFilePath, string originalFileName, CancellationToken cancellationToken = default(CancellationToken));

	Task<FileAttachment> SaveFileAsync(byte[] fileBytes, string originalFileName, CancellationToken cancellationToken = default(CancellationToken));

	Task<FileAttachment?> GetAttachmentAsync(string attachmentId);

	Task<string> ExtractTextAsync(string attachmentId, CancellationToken cancellationToken = default(CancellationToken));

	Task<bool> DeleteAttachmentAsync(string attachmentId, CancellationToken cancellationToken = default(CancellationToken));

	Task<List<FileAttachment>> GetSessionAttachmentsAsync(string sessionId, CancellationToken cancellationToken = default(CancellationToken));

	Task<int> ClearSessionAttachmentsAsync(string sessionId, CancellationToken cancellationToken = default(CancellationToken));

	string GetStorageDirectory();

	bool IsSupportedFileType(string fileExtension, string? provider = null);

	long GetMaxFileSize(string fileExtension, string? provider = null);
}
