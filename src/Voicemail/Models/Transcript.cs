using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voicemail.Models;

/// <summary>
/// Represents the result of transcribing a recording.
/// </summary>
[Table("Calls")]
public class Transcript
{
	/// <summary>
	/// ID for the associated call.
	/// </summary>
	[Key]
	public int CallId { get; set; }
	
	/// <summary>
	/// Transcript of the call.
	/// </summary>
	public required string? TranscriptText { get; set; }
	
	/// <summary>
	/// Phone numbers mentioned during the call.
	/// </summary>
	public IList<string>? MentionedNumbers { get; set; }
	
	/// <summary>
	/// People mentioned during the call.
	/// </summary>
	public IList<string>? MentionedPeople { get; set; }
	
	/// <summary>
	/// Companies mentioned during the call.
	/// </summary>
	public IList<string>? MentionedCompanies { get; set; }
}
