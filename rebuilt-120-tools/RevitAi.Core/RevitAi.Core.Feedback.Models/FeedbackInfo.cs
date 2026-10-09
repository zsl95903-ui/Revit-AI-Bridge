using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ns7;

namespace RevitAi.Core.Feedback.Models;

public class FeedbackInfo
{
	[CompilerGenerated]
	private Guid guid_0;

	[CompilerGenerated]
	private string string_0 = string.Empty;

	[CompilerGenerated]
	private Guid? nullable_0;

	[CompilerGenerated]
	private string string_1 = string.Empty;

	[CompilerGenerated]
	private string string_2 = string.Empty;

	[CompilerGenerated]
	private string? string_3;

	[CompilerGenerated]
	private object? object_0;

	[CompilerGenerated]
	private string string_4 = "pending";

	[CompilerGenerated]
	private string? string_5;

	[CompilerGenerated]
	private DateTime? nullable_1;

	[CompilerGenerated]
	private DateTime? nullable_2;

	[CompilerGenerated]
	private DateTime? nullable_3;

	[CompilerGenerated]
	private DateTime dateTime_0;

	[CompilerGenerated]
	private DateTime dateTime_1;

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

	[JsonProperty("title")]
	public string Title
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

	[JsonProperty("content")]
	public string Content
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

	[JsonProperty("email")]
	public string? Email
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

	[JsonProperty("attachments")]
	private object? _attachmentsData
	{
		[CompilerGenerated]
		get
		{
			return object_0;
		}
		[CompilerGenerated]
		set
		{
			object_0 = value;
		}
	}

	[JsonIgnore]
	public List<FeedbackAttachment>? Attachments
	{
		get
		{
			if (_attachmentsData == null)
			{
				return null;
			}
			if (_attachmentsData is List<FeedbackAttachment> result)
			{
				return result;
			}
			object? attachmentsData = _attachmentsData;
			JArray val = (JArray)((attachmentsData is JArray) ? attachmentsData : null);
			if (val != null)
			{
				return ((JToken)val).ToObject<List<FeedbackAttachment>>();
			}
			if (_attachmentsData is string text && !string.IsNullOrEmpty(text))
			{
				try
				{
					return JsonConvert.DeserializeObject<List<FeedbackAttachment>>(text);
				}
				catch
				{
					return null;
				}
			}
			return null;
		}
		set
		{
			_attachmentsData = value;
		}
	}

	[JsonProperty("status")]
	public string Status
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

	[JsonProperty("admin_reply")]
	public string? AdminReply
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

	[JsonProperty("admin_reply_at")]
	public DateTime? AdminReplyAt
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

	[JsonProperty("user_viewed_at")]
	public DateTime? UserViewedAt
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

	[JsonProperty("device_viewed_at")]
	public DateTime? DeviceViewedAt
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

	[JsonProperty("created_at")]
	public DateTime CreatedAt
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

	[JsonProperty("updated_at")]
	public DateTime UpdatedAt
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

	public bool HasReply
	{
		get
		{
			if (Status == "replied")
			{
				return AdminReply != null;
			}
			return false;
		}
	}

	public bool IsUserViewed
	{
		get
		{
			if (UserViewedAt.HasValue && AdminReplyAt.HasValue)
			{
				return UserViewedAt >= AdminReplyAt;
			}
			return false;
		}
	}

	public bool IsDeviceViewed
	{
		get
		{
			if (DeviceViewedAt.HasValue && AdminReplyAt.HasValue)
			{
				return DeviceViewedAt >= AdminReplyAt;
			}
			return false;
		}
	}

	public bool HasNewReply
	{
		get
		{
			if (HasReply && !IsUserViewed)
			{
				return !IsDeviceViewed;
			}
			return false;
		}
	}
}
