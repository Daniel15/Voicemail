// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2024 Daniel Lo Nigro <d@d.sb>

namespace Voicemail.Configuration;

/// <summary>
/// Configuration for TrestleIQ API.
/// </summary>
public class TrestleConfig
{
	/// <summary>
	/// API key
	/// </summary>
	public required string ApiKey { get; set; }
}
