using System;

namespace RevitAi.Abstractions.Authentication;

public interface IDeviceInfo
{
	Guid Id { get; }

	string DeviceId { get; }

	string DeviceName { get; }

	string OsVersion { get; }

	bool IsActive { get; }

	bool IsBound { get; }
}
