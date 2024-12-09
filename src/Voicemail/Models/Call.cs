// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2024 Daniel Lo Nigro <d@d.sb>

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PhoneNumbers;

namespace Voicemail.Models;

/// <summary>
/// Represents a call that was received.
/// </summary>
[Index("ExternalId", IsUnique = true)]
[Table("Calls")]
public class Call
{
	/// <summary>
	/// Auto-increment ID for the call
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// ID from the upstream provider (e.g. Twilio)
	/// </summary>
	public string? ExternalId { get; set; }
	
	/// <summary>
	/// Phone number of the caller.
	/// </summary>
	[Column("NumberFrom")]
	public string? NumberFromRaw { get; set; }

	/// <summary>
	/// Phone number of the caller.
	/// </summary>
	[NotMapped]
	public PhoneNumber? NumberFrom => 
		PhoneNumberUtil.GetInstance().Parse(NumberFromRaw, "US");
	
	/// <summary>
	/// Phone number the call was forwarded from.
	/// </summary>
	public string? NumberForwardedFrom { get; set; }
	
	/// <summary>
	/// Phone number the call went to.
	/// </summary>
	public string? NumberTo { get; set; }
	
	/// <summary>
	/// How long the recording is, in seconds.
	/// </summary>
	public int? RecordingDurationSeconds { get; set; }
	
	/// <summary>
	/// URL the recording was downloaded from.
	/// </summary>
	public string? RecordingUrl { get; set; }

	/// <summary>
	/// Whether this call has been fully processed yet.
	/// </summary>
	public bool Processed { get; set; } = false;
	
	/// <summary>
	/// Transcript of the call.
	/// </summary>
	public Transcript? Transcript { get; set; }
	
	/// <summary>
	/// Caller ID for the call (i.e. who was calling).
	/// </summary>
	public CallerId? CallerId { get; set; }
}
