using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
	public string? NumberFrom { get; set; }
	
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
	/// Transcript of the call.
	/// </summary>
	public Transcript? Transcript { get; set; }
}
