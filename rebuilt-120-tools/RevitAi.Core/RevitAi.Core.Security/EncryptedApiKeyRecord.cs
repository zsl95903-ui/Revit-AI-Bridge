using System.Runtime.CompilerServices;

namespace RevitAi.Core.Security;

public sealed class EncryptedApiKeyRecord
{
	[CompilerGenerated]
	private string? string_0;

	[CompilerGenerated]
	private string string_1 = string.Empty;

	[CompilerGenerated]
	private string? string_2;

	[CompilerGenerated]
	private EncryptedApiKey? encryptedApiKey_0;

	[CompilerGenerated]
	private bool bool_0;

	[CompilerGenerated]
	private string string_3 = string.Empty;

	public string? Id
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

	public string ApiUrl
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

	public string? Notes
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

	public EncryptedApiKey? EncryptedKey
	{
		[CompilerGenerated]
		get
		{
			return encryptedApiKey_0;
		}
		[CompilerGenerated]
		set
		{
			encryptedApiKey_0 = value;
		}
	}

	public bool IsActive
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

	public string CreatedAt
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
}
