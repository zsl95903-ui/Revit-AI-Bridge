using System.Runtime.CompilerServices;

namespace RevitAi.Core.Security;

public sealed class EncryptedApiKey
{
	[CompilerGenerated]
	private string string_0 = string.Empty;

	[CompilerGenerated]
	private string? string_1;

	[CompilerGenerated]
	private string string_2 = string.Empty;

	[CompilerGenerated]
	private string string_3 = string.Empty;

	public string EncryptedData
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

	public string? Tag
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

	public string Salt
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

	public string IV
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
