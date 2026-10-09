using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.Authentication;

public class AuthorizationStateChangedEventArgs : EventArgs
{
	public bool HasValidLicense { get; set; }

	public string? LicenseType { get; set; }

	public DateTime? ExpiryDate { get; set; }

	public decimal CreditsBalance { get; set; }

	public Dictionary<string, int> TrialUsage { get; set; } = new Dictionary<string, int>();

	public List<IDeviceInfo>? UserDevices { get; set; }
}
