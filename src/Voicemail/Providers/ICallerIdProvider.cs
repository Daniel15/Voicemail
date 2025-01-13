// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2024 Daniel Lo Nigro <d@d.sb>

using PhoneNumbers;
using Voicemail.Models;

namespace Voicemail.Providers;

/// <summary>
/// Represents a provider that can do caller ID lookups.
/// </summary>
public interface ICallerIdProvider
{
	/// <summary>
	/// Look up the specified phone number.
	/// </summary>
	/// <param name="phoneNumber">Phone number to look up</param>
	/// <returns>Details about the caller</returns>
	public Task<CallerId?> GetCallerId(PhoneNumber phoneNumber);
}
