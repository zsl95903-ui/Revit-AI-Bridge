using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Authentication;

namespace RevitAi.Core.Authentication.Models;

public sealed class LicenseInfo : ILicenseInfo
{
	[CompilerGenerated]
	private Guid guid_0;

	[CompilerGenerated]
	private Guid guid_1;

	[CompilerGenerated]
	private string string_0 = string.Empty;

	[CompilerGenerated]
	private bool bool_0;

	[CompilerGenerated]
	private DateTime dateTime_0;

	[CompilerGenerated]
	private DateTime? nullable_0;

	[CompilerGenerated]
	private string string_1 = string.Empty;

	[CompilerGenerated]
	private List<string> list_0 = new List<string>();

	[CompilerGenerated]
	private int int_0;

	[CompilerGenerated]
	private bool bool_1;

	[CompilerGenerated]
	private DateTime dateTime_1;

	[CompilerGenerated]
	private DateTime? nullable_1;

	[CompilerGenerated]
	private bool bool_2;

	public Guid LicenseId
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

	public Guid UserId
	{
		[CompilerGenerated]
		get
		{
			return guid_1;
		}
		[CompilerGenerated]
		set
		{
			guid_1 = value;
		}
	}

	public string LicenseType
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

	public bool IsTrial
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

	public DateTime ValidFrom
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

	public DateTime? ValidTo
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

	public string DeviceFingerprint
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

	public List<string> FeaturesList
	{
		[CompilerGenerated]
		get
		{
			return list_0;
		}
		[CompilerGenerated]
		set
		{
			list_0 = value;
		}
	}

	string[] ILicenseInfo.Features => FeaturesList.ToArray();

	public int MaxDevices
	{
		[CompilerGenerated]
		get
		{
			return int_0;
		}
		[CompilerGenerated]
		set
		{
			int_0 = value;
		}
	}

	public bool AutoRenew
	{
		[CompilerGenerated]
		get
		{
			return bool_1;
		}
		[CompilerGenerated]
		set
		{
			bool_1 = value;
		}
	}

	public DateTime CreatedAt
	{
		[CompilerGenerated]
		get
		{
			return dateTime_1;
		}
		[CompilerGenerated]
		set
		{
			dateTime_1 = value;
		}
	}

	public DateTime? UpdatedAt
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

	public bool IsActive
	{
		[CompilerGenerated]
		get
		{
			return bool_2;
		}
		[CompilerGenerated]
		set
		{
			bool_2 = value;
		}
	}

	public bool IsValid()
	{
		if (!IsActive)
		{
			return false;
		}
		DateTime utcNow = DateTime.UtcNow;
		if (utcNow < ValidFrom)
		{
			return false;
		}
		if (ValidTo.HasValue && utcNow > ValidTo.Value)
		{
			return false;
		}
		return true;
	}

	public bool HasFeature(string featureId)
	{
		if (string.IsNullOrEmpty(featureId))
		{
			return false;
		}
		return FeaturesList.Contains(featureId);
	}

	bool ILicenseInfo.HasFeature(string featureId)
	{
		return HasFeature(featureId);
	}

	public bool IsExpiringSoon(int daysThreshold = 7)
	{
		if (!ValidTo.HasValue)
		{
			return false;
		}
		return DateTime.UtcNow.AddDays(daysThreshold) >= ValidTo.Value;
	}
}
