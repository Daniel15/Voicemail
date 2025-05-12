// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2025 Daniel Lo Nigro <d@d.sb>

namespace Voicemail.Configuration;

public class AccountConfig
{
	/// <summary>
	/// Phone number for this account.
	/// </summary>
	public required string PhoneNumber { get; set; }
	
	/// <summary>
	/// Path to an MP3 file to play as a voicemail greeting, relative to the wwwroot folder.
	/// </summary>
	public required string GreetingFile { get; set; }
	
	/// <summary>
	/// Email address to send voicemail messages to.
	/// </summary>
	public required string Email { get; set; }
}
