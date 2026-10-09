using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace RevitAi.Core.Authentication.Models;

public class MigrateDeviceResult
{
	[CompilerGenerated]
	private bool bool_0;

	[CompilerGenerated]
	private string? string_0;

	[CompilerGenerated]
	private string? string_1;

	[CompilerGenerated]
	private int int_0;

	[CompilerGenerated]
	private int int_1;

	[CompilerGenerated]
	private bool bool_1;

	[CompilerGenerated]
	private string? string_2;

	[JsonProperty("success")]
	public bool Success
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

	[JsonProperty("new_device_id")]
	public string? NewDeviceId
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

	[JsonProperty("new_device_uuid")]
	public string? NewDeviceUuid
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

	[JsonProperty("copied_credits")]
	public int CopiedCredits
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

	[JsonProperty("copied_trial_records")]
	public int CopiedTrialRecords
	{
		[CompilerGenerated]
		get
		{
			return int_1;
		}
		[CompilerGenerated]
		set
		{
			int_1 = value;
		}
	}

	[JsonProperty("old_device_unbound")]
	public bool OldDeviceUnbound
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

	[JsonProperty("error")]
	public string? Error
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
}
