using System;

namespace RevitAi.Abstractions.Authentication;

public interface ILicenseInfo
{
	Guid LicenseId { get; }

	string LicenseType { get; }

	DateTime ValidFrom { get; }

	DateTime? ValidTo { get; }

	string DeviceFingerprint { get; }

	string[] Features { get; }

	bool IsActive { get; }

	bool IsValid();

	bool HasFeature(string featureId);
}
