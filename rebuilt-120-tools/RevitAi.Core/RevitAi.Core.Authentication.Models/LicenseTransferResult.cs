using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using ns7;

namespace RevitAi.Core.Authentication.Models;

public sealed class LicenseTransferResult
{
	[CompilerGenerated]
	private bool bool_0;

	[CompilerGenerated]
	private Guid? nullable_0;

	[CompilerGenerated]
	private string? string_0;

	[CompilerGenerated]
	private DateTime? nullable_1;

	[CompilerGenerated]
	private DateTime? nullable_2;

	[CompilerGenerated]
	private string? string_1;

	[CompilerGenerated]
	private string? string_2;

	[JsonProperty("has_license")]
	public bool HasLicense
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

	[JsonProperty("license_id")]
	public Guid? LicenseId
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

	[JsonProperty("license_type")]
	public string? LicenseType
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

	[JsonProperty("valid_from")]
	public DateTime? ValidFrom
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

	[JsonProperty("valid_to")]
	public DateTime? ValidTo
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

	[JsonProperty("features")]
	public string? Features
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

	[JsonProperty("action_taken")]
	public string? ActionTaken
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

	public bool IsValid
	{
		get
		{
			if (HasLicense)
			{
				if (ValidTo.HasValue)
				{
					return ValidTo > DateTime.UtcNow;
				}
				return true;
			}
			return false;
		}
	}

	public override string ToString()
	{
		if (!HasLicense)
		{
			return "无授权 (" + ActionTaken + ")";
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 3);
		defaultInterpolatedStringHandler.AppendLiteral("授权: ");
		defaultInterpolatedStringHandler.AppendFormatted(LicenseType);
		defaultInterpolatedStringHandler.AppendLiteral(", 到期: ");
		DateTime? validTo = ValidTo;
		object obj;
		if (!validTo.HasValue)
		{
			obj = null;
		}
		else
		{
			obj = validTo.GetValueOrDefault().ToString("yyyy-MM-dd");
			if (obj != null)
			{
				goto IL_009d;
			}
		}
		obj = "永久";
		goto IL_009d;
		IL_009d:
		defaultInterpolatedStringHandler.AppendFormatted((string?)obj);
		defaultInterpolatedStringHandler.AppendLiteral(" (");
		defaultInterpolatedStringHandler.AppendFormatted(ActionTaken);
		defaultInterpolatedStringHandler.AppendLiteral(")");
		return defaultInterpolatedStringHandler.ToStringAndClear();
	}
}
