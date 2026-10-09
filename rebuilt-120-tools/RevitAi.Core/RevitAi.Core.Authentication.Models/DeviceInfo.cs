using System;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Authentication;
using Newtonsoft.Json;

namespace RevitAi.Core.Authentication.Models;

public sealed class DeviceInfo : IDeviceInfo
{
	[CompilerGenerated]
	private Guid guid_0;

	[CompilerGenerated]
	private string string_0 = string.Empty;

	[CompilerGenerated]
	private string string_1 = string.Empty;

	[CompilerGenerated]
	private string? string_2;

	[CompilerGenerated]
	private string? string_3;

	[CompilerGenerated]
	private string? string_4;

	[CompilerGenerated]
	private string? string_5;

	[CompilerGenerated]
	private string? string_6;

	[CompilerGenerated]
	private Guid? nullable_0;

	[CompilerGenerated]
	private DateTime? nullable_1;

	[CompilerGenerated]
	private DateTime? nullable_2;

	[CompilerGenerated]
	private bool bool_0;

	[CompilerGenerated]
	private bool bool_1;

	[CompilerGenerated]
	private string? string_7;

	[CompilerGenerated]
	private string? string_8;

	[CompilerGenerated]
	private int int_0;

	[CompilerGenerated]
	private DateTime? nullable_3;

	[CompilerGenerated]
	private bool bool_2;

	[CompilerGenerated]
	private bool bool_3;

	[CompilerGenerated]
	private bool bool_4;

	[CompilerGenerated]
	private bool bool_5;

	[CompilerGenerated]
	private bool bool_6;

	[CompilerGenerated]
	private bool bool_7;

	[CompilerGenerated]
	private bool bool_8;

	[CompilerGenerated]
	private bool bool_9;

	[CompilerGenerated]
	private bool bool_10;

	[JsonProperty("id")]
	public Guid Id
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

	[JsonProperty("device_id")]
	public string DeviceId
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

	[JsonProperty("device_name")]
	public string DeviceName
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

	[JsonIgnore]
	public string? CpuId
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

	[JsonIgnore]
	public string? MacAddress
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

	[JsonIgnore]
	public string? MotherboardSerial
	{
		[CompilerGenerated]
		get
		{
			return string_4;
		}
		[CompilerGenerated]
		set
		{
			string_4 = value;
		}
	}

	[JsonProperty("os_info")]
	public string? OsVersion
	{
		[CompilerGenerated]
		get
		{
			return string_5;
		}
		[CompilerGenerated]
		set
		{
			string_5 = value;
		}
	}

	[JsonProperty("last_revit_version")]
	public string? RevitVersion
	{
		[CompilerGenerated]
		get
		{
			return string_6;
		}
		[CompilerGenerated]
		set
		{
			string_6 = value;
		}
	}

	[JsonProperty("user_id")]
	public Guid? UserId
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

	string IDeviceInfo.OsVersion => OsVersion ?? string.Empty;

	[JsonProperty("first_active_at")]
	public DateTime? RegisteredAt
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

	[JsonProperty("last_active_at")]
	public DateTime? LastSeenAt
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

	[JsonIgnore]
	public bool IsActive
	{
		get
		{
			return !IsDisabled;
		}
		set
		{
		}
	}

	[JsonProperty("is_bound")]
	public bool IsBound
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

	[JsonProperty("is_disabled")]
	public bool IsDisabled
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

	[JsonProperty("ip_address")]
	public string? IpAddress
	{
		[CompilerGenerated]
		get
		{
			return string_7;
		}
		[CompilerGenerated]
		set
		{
			string_7 = value;
		}
	}

	[JsonProperty("first_ip_address")]
	public string? FirstIpAddress
	{
		[CompilerGenerated]
		get
		{
			return string_8;
		}
		[CompilerGenerated]
		set
		{
			string_8 = value;
		}
	}

	[JsonProperty("ip_changes_count")]
	public int IpChangesCount
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

	[JsonProperty("last_ip_change_at")]
	public DateTime? LastIpChangeAt
	{
		[CompilerGenerated]
		get
		{
			return nullable_3;
		}
		[CompilerGenerated]
		set
		{
			nullable_3 = value;
		}
	}

	[JsonProperty("revit_2018")]
	public bool Revit2018
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

	[JsonProperty("revit_2019")]
	public bool Revit2019
	{
		[CompilerGenerated]
		get
		{
			return bool_3;
		}
		[CompilerGenerated]
		set
		{
			bool_3 = value;
		}
	}

	[JsonProperty("revit_2020")]
	public bool Revit2020
	{
		[CompilerGenerated]
		get
		{
			return bool_4;
		}
		[CompilerGenerated]
		set
		{
			bool_4 = value;
		}
	}

	[JsonProperty("revit_2021")]
	public bool Revit2021
	{
		[CompilerGenerated]
		get
		{
			return bool_5;
		}
		[CompilerGenerated]
		set
		{
			bool_5 = value;
		}
	}

	[JsonProperty("revit_2022")]
	public bool Revit2022
	{
		[CompilerGenerated]
		get
		{
			return bool_6;
		}
		[CompilerGenerated]
		set
		{
			bool_6 = value;
		}
	}

	[JsonProperty("revit_2023")]
	public bool Revit2023
	{
		[CompilerGenerated]
		get
		{
			return bool_7;
		}
		[CompilerGenerated]
		set
		{
			bool_7 = value;
		}
	}

	[JsonProperty("revit_2024")]
	public bool Revit2024
	{
		[CompilerGenerated]
		get
		{
			return bool_8;
		}
		[CompilerGenerated]
		set
		{
			bool_8 = value;
		}
	}

	[JsonProperty("revit_2025")]
	public bool Revit2025
	{
		[CompilerGenerated]
		get
		{
			return bool_9;
		}
		[CompilerGenerated]
		set
		{
			bool_9 = value;
		}
	}

	[JsonProperty("revit_2026")]
	public bool Revit2026
	{
		[CompilerGenerated]
		get
		{
			return bool_10;
		}
		[CompilerGenerated]
		set
		{
			bool_10 = value;
		}
	}

	public bool IsValid()
	{
		if (!string.IsNullOrEmpty(DeviceId) && DeviceId.Length == 64)
		{
			string deviceId = DeviceId;
			int num = 0;
			while (true)
			{
				if (num < deviceId.Length)
				{
					char c = deviceId[num];
					if ((c < '0' || c > '9') && (c < 'a' || c > 'f') && (c < 'A' || c > 'F'))
					{
						break;
					}
					num++;
					continue;
				}
				return true;
			}
			return false;
		}
		return false;
	}

	public bool IsRegistered()
	{
		return UserId.HasValue;
	}
}
