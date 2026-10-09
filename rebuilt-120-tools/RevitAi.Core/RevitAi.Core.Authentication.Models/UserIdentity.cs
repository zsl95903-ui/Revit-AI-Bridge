using System;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Authentication;

namespace RevitAi.Core.Authentication.Models;

public sealed class UserIdentity : IUserIdentity
{
	[CompilerGenerated]
	private Guid guid_0;

	[CompilerGenerated]
	private string string_0 = string.Empty;

	[CompilerGenerated]
	private string? string_1;

	[CompilerGenerated]
	private string? string_2;

	[CompilerGenerated]
	private DateTime? nullable_0;

	[CompilerGenerated]
	private string? string_3;

	[CompilerGenerated]
	private DateTime? nullable_1;

	[CompilerGenerated]
	private DateTime? nullable_2;

	public Guid UserId
	{
		[CompilerGenerated]
		get
		{
			return guid_0;
		}
		[CompilerGenerated]
		set
		{
			guid_0 = value;
		}
	}

	public string Email
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

	public string? DisplayName
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

	public string? AuthToken
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

	public DateTime? TokenExpiresAt
	{
		[CompilerGenerated]
		get
		{
			return nullable_0;
		}
		[CompilerGenerated]
		set
		{
			nullable_0 = value;
		}
	}

	public string? RefreshToken
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

	public DateTime? CreatedAt
	{
		[CompilerGenerated]
		get
		{
			return nullable_1;
		}
		[CompilerGenerated]
		set
		{
			nullable_1 = value;
		}
	}

	public DateTime? LastSignInAt
	{
		[CompilerGenerated]
		get
		{
			return nullable_2;
		}
		[CompilerGenerated]
		set
		{
			nullable_2 = value;
		}
	}

	string? IUserIdentity.FullName => DisplayName;

	public bool IsAuthTokenValid()
	{
		if (!string.IsNullOrEmpty(AuthToken))
		{
			if (TokenExpiresAt.HasValue)
			{
				return TokenExpiresAt.Value > DateTime.UtcNow;
			}
			return true;
		}
		return false;
	}

	public bool NeedsTokenRefresh(int bufferMinutes = 5)
	{
		if (!TokenExpiresAt.HasValue)
		{
			return false;
		}
		return DateTime.UtcNow.AddMinutes(bufferMinutes) >= TokenExpiresAt.Value;
	}
}
