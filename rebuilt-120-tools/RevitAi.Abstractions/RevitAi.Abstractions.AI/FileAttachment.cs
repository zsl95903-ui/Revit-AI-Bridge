using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.AI;

public sealed class FileAttachment
{
	public string AttachmentId { get; set; } = Guid.NewGuid().ToString();

	public string FileName { get; set; } = string.Empty;

	public string FileExtension { get; set; } = string.Empty;

	public FileAttachmentType FileType { get; set; }

	public long FileSize { get; set; }

	public string FileSizeDisplay => FormatFileSize(FileSize);

	public string FilePath { get; set; } = string.Empty;

	public string MimeType { get; set; } = string.Empty;

	public DateTime UploadedAt { get; set; } = DateTime.Now;

	public bool IsExtracted { get; set; }

	public string? ExtractedText { get; set; }

	public string? PreviewText
	{
		get
		{
			if (string.IsNullOrEmpty(ExtractedText))
			{
				return null;
			}
			string extractedText = ExtractedText;
			if (extractedText.Length <= 500)
			{
				return extractedText;
			}
			return extractedText.Substring(0, 500) + "...";
		}
	}

	public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

	public static FileAttachmentType GetFileTypeFromExtension(string extension)
	{
		switch (extension.ToLower())
		{
		case ".png":
		case ".jpg":
		case ".gif":
		case ".dib":
		case ".tif":
		case ".bmp":
		case ".svg":
		case ".ico":
		case ".xbm":
		case ".pjp":
		case ".jpeg":
		case ".jfif":
		case ".webp":
		case ".svgz":
		case ".tiff":
		case ".avif":
		case ".apng":
		case ".pjpeg":
			return FileAttachmentType.Image;
		case ".pdf":
			return FileAttachmentType.Pdf;
		case ".xls":
		case ".xlsx":
			return FileAttachmentType.Excel;
		case ".csv":
		case ".tsv":
			return FileAttachmentType.Csv;
		case ".doc":
		case ".docx":
			return FileAttachmentType.Word;
		case ".ini":
		case ".xml":
		case ".yml":
		case ".css":
		case ".tsx":
		case ".jsx":
		case ".log":
		case ".txt":
		case ".htm":
		case ".vue":
		case ".json":
		case ".toml":
		case ".yaml":
		case ".conf":
		case ".html":
		case ".md":
		case ".js":
		case ".ts":
			return FileAttachmentType.Text;
		case ".cpp":
		case ".hpp":
		case ".cxx":
		case ".kts":
		case ".lua":
		case ".py3":
		case ".php":
		case ".sql":
		case ".java":
		case ".dart":
		case ".bash":
		case ".perl":
		case ".ipynb":
		case ".swift":
		case ".scala":
		case ".cs":
		case ".rs":
		case ".py":
		case ".cc":
		case ".go":
		case ".rb":
		case ".kt":
		case ".sh":
		case ".pl":
		case ".m":
		case ".r":
		case ".c":
		case ".h":
			return FileAttachmentType.Text;
		case ".dxf":
		case ".dwg":
			return FileAttachmentType.Cad;
		default:
			return FileAttachmentType.Unknown;
		}
	}

	public static string GetMimeType(string extension)
	{
		switch (extension.ToLower())
		{
		case ".png":
			return "image/png";
		case ".jpg":
		case ".jpeg":
			return "image/jpeg";
		case ".gif":
			return "image/gif";
		case ".bmp":
			return "image/bmp";
		case ".webp":
			return "image/webp";
		case ".svg":
		case ".svgz":
			return "image/svg+xml";
		case ".ico":
			return "image/x-icon";
		case ".tif":
		case ".tiff":
			return "image/tiff";
		case ".avif":
			return "image/avif";
		case ".apng":
			return "image/apng";
		case ".pdf":
			return "application/pdf";
		case ".xlsx":
			return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
		case ".xls":
			return "application/vnd.ms-excel";
		case ".csv":
			return "text/csv";
		case ".tsv":
			return "text/tab-separated-values";
		case ".docx":
			return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
		case ".doc":
			return "application/msword";
		case ".txt":
			return "text/plain";
		case ".md":
			return "text/markdown";
		case ".json":
			return "application/json";
		case ".xml":
			return "application/xml";
		case ".log":
			return "text/plain";
		case ".yml":
		case ".yaml":
			return "application/x-yaml";
		case ".ini":
		case ".conf":
			return "text/plain";
		case ".toml":
			return "application/toml";
		case ".htm":
		case ".html":
			return "text/html";
		case ".css":
			return "text/css";
		case ".js":
			return "text/javascript";
		case ".ts":
			return "text/typescript";
		case ".jsx":
			return "text/jsx";
		case ".tsx":
			return "text/tsx";
		case ".vue":
			return "text/vue";
		case ".py3":
		case ".py":
			return "text/x-python";
		case ".ipynb":
			return "application/x-ipynb+json";
		case ".java":
			return "text/x-java-source";
		case ".c":
			return "text/x-c";
		case ".cpp":
		case ".cxx":
		case ".cc":
			return "text/x-c++";
		case ".cs":
			return "text/x-csharp";
		case ".go":
			return "text/x-go";
		case ".hpp":
		case ".h":
			return "text/x-c";
		case ".php":
			return "text/x-php";
		case ".rb":
			return "text/x-ruby";
		case ".rs":
			return "text/x-rust";
		case ".swift":
			return "text/x-swift";
		case ".kts":
		case ".kt":
			return "text/x-kotlin";
		case ".scala":
			return "text/x-scala";
		case ".r":
			return "text/x-r";
		case ".m":
			return "text/x-matlab";
		case ".bash":
		case ".sh":
			return "text/x-shellscript";
		case ".sql":
			return "text/x-sql";
		case ".dart":
			return "application/dart";
		case ".lua":
			return "text/x-lua";
		case ".perl":
		case ".pl":
			return "text/x-perl";
		case ".dwg":
			return "application/acad";
		case ".dxf":
			return "application/dxf";
		default:
			return "application/octet-stream";
		}
	}

	private static string FormatFileSize(long bytes)
	{
		string[] array = new string[4] { "B", "KB", "MB", "GB" };
		double num = bytes;
		int num2 = 0;
		while (num >= 1024.0 && num2 < array.Length - 1)
		{
			num2++;
			num /= 1024.0;
		}
		return $"{num:0.##} {array[num2]}";
	}

	public override string ToString()
	{
		return FileName;
	}
}
