// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2024 Daniel Lo Nigro <d@d.sb>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voicemail.Models;

/// <summary>
/// Data about a caller.
/// </summary>
[Table("Calls")]
public class CallerId
{
	/// <summary>
	/// ID for the associated call.
	/// </summary>
	[Key]
	public int CallId { get; set; }
	
	/// <summary>
	/// Name of the person that called
	/// </summary>
	public string? CallerName { get; set; }
}
