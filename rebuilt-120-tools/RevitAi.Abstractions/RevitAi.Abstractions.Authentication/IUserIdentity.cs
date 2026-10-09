using System;

namespace RevitAi.Abstractions.Authentication;

public interface IUserIdentity
{
	Guid UserId { get; }

	string Email { get; }

	string? FullName { get; }
}
