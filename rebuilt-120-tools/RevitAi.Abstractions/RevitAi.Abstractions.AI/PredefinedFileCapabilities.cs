using System.Collections.Generic;

namespace RevitAi.Abstractions.AI;

public static class PredefinedFileCapabilities
{
	public static AIProviderFileCapability Claude => new AIProviderFileCapability
	{
		ContentFormat = ContentFormatType.ClaudeNative,
		DefaultMaxFileSize = 20971520L,
		MaxFileCount = 5,
		SupportsMultimodal = true,
		SupportedTypes = new Dictionary<FileAttachmentType, FileTypeCapability>
		{
			[FileAttachmentType.Image] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Base64,
				MaxFileSize = 20971520L,
				IsContentItem = true,
				ApiContentType = "image"
			},
			[FileAttachmentType.Pdf] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Base64,
				MaxFileSize = 20971520L,
				IsContentItem = true,
				ApiContentType = "document"
			},
			[FileAttachmentType.Text] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.AsText,
				MaxFileSize = 5242880L,
				IsContentItem = true,
				ApiContentType = "text"
			}
		}
	};

	public static AIProviderFileCapability DeepSeek => new AIProviderFileCapability
	{
		ContentFormat = ContentFormatType.OpenAICompatible,
		DefaultMaxFileSize = 20971520L,
		MaxFileCount = 10,
		SupportsMultimodal = true,
		SupportedTypes = new Dictionary<FileAttachmentType, FileTypeCapability>
		{
			[FileAttachmentType.Image] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Base64,
				MaxFileSize = 20971520L,
				IsContentItem = true,
				ApiContentType = "image_url"
			},
			[FileAttachmentType.Pdf] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Base64,
				MaxFileSize = 20971520L,
				IsContentItem = true,
				ApiContentType = "image_url"
			},
			[FileAttachmentType.Text] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.AsText,
				MaxFileSize = 10485760L,
				IsContentItem = true,
				ApiContentType = "text"
			},
			[FileAttachmentType.Csv] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.AsText,
				MaxFileSize = 10485760L,
				IsContentItem = true,
				ApiContentType = "text"
			},
			[FileAttachmentType.Excel] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Preprocessed,
				MaxFileSize = 10485760L,
				IsContentItem = false
			},
			[FileAttachmentType.Word] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Preprocessed,
				MaxFileSize = 10485760L,
				IsContentItem = false
			},
			[FileAttachmentType.Cad] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.None
			}
		}
	};

	public static AIProviderFileCapability OpenAI => new AIProviderFileCapability
	{
		ContentFormat = ContentFormatType.OpenAICompatible,
		DefaultMaxFileSize = 20971520L,
		MaxFileCount = 5,
		SupportsMultimodal = true,
		SupportedTypes = new Dictionary<FileAttachmentType, FileTypeCapability>
		{
			[FileAttachmentType.Image] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Base64,
				MaxFileSize = 20971520L,
				IsContentItem = true,
				ApiContentType = "image_url"
			},
			[FileAttachmentType.Pdf] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.None
			},
			[FileAttachmentType.Text] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.AsText,
				MaxFileSize = 5242880L,
				IsContentItem = true,
				ApiContentType = "text"
			}
		}
	};

	public static AIProviderFileCapability Ollama => new AIProviderFileCapability
	{
		ContentFormat = ContentFormatType.OpenAICompatible,
		DefaultMaxFileSize = 10485760L,
		MaxFileCount = 3,
		SupportsMultimodal = false,
		SupportedTypes = new Dictionary<FileAttachmentType, FileTypeCapability>
		{
			[FileAttachmentType.Image] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Preprocessed,
				MaxFileSize = 10485760L
			},
			[FileAttachmentType.Pdf] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Preprocessed,
				MaxFileSize = 5242880L
			},
			[FileAttachmentType.Text] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.AsText,
				MaxFileSize = 5242880L,
				IsContentItem = true,
				ApiContentType = "text"
			}
		}
	};

	public static AIProviderFileCapability Zhipu => new AIProviderFileCapability
	{
		ContentFormat = ContentFormatType.OpenAICompatible,
		DefaultMaxFileSize = 10485760L,
		MaxFileCount = 5,
		SupportsMultimodal = true,
		SupportedTypes = new Dictionary<FileAttachmentType, FileTypeCapability>
		{
			[FileAttachmentType.Image] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Base64,
				MaxFileSize = 10485760L,
				IsContentItem = true,
				ApiContentType = "image_url"
			},
			[FileAttachmentType.Pdf] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Base64,
				MaxFileSize = 10485760L,
				IsContentItem = true,
				ApiContentType = "image_url"
			},
			[FileAttachmentType.Text] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.AsText,
				MaxFileSize = 5242880L,
				IsContentItem = true,
				ApiContentType = "text"
			}
		}
	};

	public static AIProviderFileCapability Qwen => new AIProviderFileCapability
	{
		ContentFormat = ContentFormatType.OpenAICompatible,
		DefaultMaxFileSize = 10485760L,
		MaxFileCount = 5,
		SupportsMultimodal = true,
		SupportedTypes = new Dictionary<FileAttachmentType, FileTypeCapability>
		{
			[FileAttachmentType.Image] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Base64,
				MaxFileSize = 10485760L,
				IsContentItem = true,
				ApiContentType = "image_url"
			},
			[FileAttachmentType.Pdf] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Base64,
				MaxFileSize = 10485760L,
				IsContentItem = true,
				ApiContentType = "image_url"
			},
			[FileAttachmentType.Text] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.AsText,
				MaxFileSize = 5242880L,
				IsContentItem = true,
				ApiContentType = "text"
			}
		}
	};

	public static AIProviderFileCapability SiliconFlow => new AIProviderFileCapability
	{
		ContentFormat = ContentFormatType.OpenAICompatible,
		DefaultMaxFileSize = 20971520L,
		MaxFileCount = 10,
		SupportsMultimodal = true,
		SupportedTypes = new Dictionary<FileAttachmentType, FileTypeCapability>
		{
			[FileAttachmentType.Image] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Base64,
				MaxFileSize = 20971520L,
				IsContentItem = true,
				ApiContentType = "image_url"
			},
			[FileAttachmentType.Pdf] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Base64,
				MaxFileSize = 20971520L,
				IsContentItem = true,
				ApiContentType = "image_url"
			},
			[FileAttachmentType.Text] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.AsText,
				MaxFileSize = 10485760L,
				IsContentItem = true,
				ApiContentType = "text"
			}
		}
	};

	private static AIProviderFileCapability DefaultOpenAICompatible => new AIProviderFileCapability
	{
		ContentFormat = ContentFormatType.OpenAICompatible,
		DefaultMaxFileSize = 20971520L,
		MaxFileCount = 5,
		SupportsMultimodal = true,
		SupportedTypes = new Dictionary<FileAttachmentType, FileTypeCapability>
		{
			[FileAttachmentType.Image] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Base64,
				MaxFileSize = 20971520L,
				IsContentItem = true,
				ApiContentType = "image_url"
			},
			[FileAttachmentType.Pdf] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.Base64,
				MaxFileSize = 20971520L,
				IsContentItem = true,
				ApiContentType = "image_url"
			},
			[FileAttachmentType.Text] = new FileTypeCapability
			{
				CapabilityType = FileCapabilityType.AsText,
				MaxFileSize = 10485760L,
				IsContentItem = true,
				ApiContentType = "text"
			}
		}
	};

	public static AIProviderFileCapability GetCapability(string provider, bool supportsVision = true)
	{
		AIProviderFileCapability aIProviderFileCapability;
		switch (provider.ToLower())
		{
		case "claude":
		case "anthropic":
			aIProviderFileCapability = Claude;
			break;
		case "deepseek":
			aIProviderFileCapability = DeepSeek;
			break;
		case "openai":
			aIProviderFileCapability = OpenAI;
			break;
		case "ollama":
			aIProviderFileCapability = Ollama;
			break;
		case "bigmodel":
		case "zhipu":
			aIProviderFileCapability = Zhipu;
			break;
		case "dashscope":
		case "qwen":
			aIProviderFileCapability = Qwen;
			break;
		case "siliconflow":
			aIProviderFileCapability = SiliconFlow;
			break;
		default:
			aIProviderFileCapability = null;
			break;
		}
		AIProviderFileCapability aIProviderFileCapability2 = aIProviderFileCapability;
		if (aIProviderFileCapability2 != null)
		{
			return aIProviderFileCapability2;
		}
		if (supportsVision)
		{
			return DefaultOpenAICompatible;
		}
		return new AIProviderFileCapability();
	}
}
