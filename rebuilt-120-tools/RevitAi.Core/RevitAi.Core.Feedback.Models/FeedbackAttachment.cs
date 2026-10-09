using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace RevitAi.Core.Feedback.Models;

public class FeedbackAttachment
{
	[CompilerGenerated]
	private string string_0 = string.Empty;

	[CompilerGenerated]
	private long long_0;

	[CompilerGenerated]
	private string string_1 = string.Empty;

	[CompilerGenerated]
	private string string_2 = string.Empty;

	[CompilerGenerated]
	private string string_3 = string.Empty;

	[CompilerGenerated]
	private DateTime dateTime_0;

	[JsonProperty("fileName")]
	public string FileName
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

	[JsonProperty("fileSize")]
	public long FileSize
	{
		[CompilerGenerated]
		get
		{
			return long_0;
		}
		[CompilerGenerated]
		set
		{
			long_0 = value;
		}
	}

	[JsonProperty("storagePath")]
	public string StoragePath
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

	[JsonProperty("publicUrl")]
	public string PublicUrl
	{
		[CompilerGenerated]
		get
		{
			return string_2;
		}
		[CompilerGenerated]
		set
		{
			string_2 = value;
		}
	}

	[JsonProperty("contentType")]
	public string ContentType
	{
		[CompilerGenerated]
		get
		{
			return string_3;
		}
		[CompilerGenerated]
		set
		{
			string_3 = value;
		}
	}

	[JsonProperty("uploadedAt")]
	public DateTime UploadedAt
	{
		[CompilerGenerated]
		get
		{
			return dateTime_0;
		}
		[CompilerGenerated]
		set
		{
			dateTime_0 = value;
		}
	}
}
