using AssemblyAI;
using AssemblyAI.Transcripts;

namespace Voicemail.Services;

/// <summary>
/// Handles communicating with AssemblyAI's API.
/// </summary>
public class AssemblyAiService(AssemblyAIClient _client) : ITranscriptionService
{
	/// <summary>
	/// Transcribe the phone call recording at the specified URL.
	/// </summary>
	public async Task<Voicemail.Models.Transcript> Transcribe(string recordingUrl)
	{
		var transcript = await _client.Transcripts.TranscribeAsync(new TranscriptParams
		{
			AudioUrl = recordingUrl,
			SpeechModel = SpeechModel.Best,
			EntityDetection = true,
			Summarization = true,
		});
		transcript.EnsureStatusCompleted();

		return new Voicemail.Models.Transcript
		{
			TranscriptText = transcript.Text,
			TranscriptSummary = transcript.Summary,
			MentionedCompanies = EntityTextByType(transcript, EntityType.Organization),
			MentionedNumbers = EntityTextByType(transcript, EntityType.PhoneNumber),
			MentionedPeople = EntityTextByType(transcript, EntityType.PersonName),
		};
	}
	
	private static List<string> EntityTextByType(Transcript transcript,  EntityType type)
	{
		return transcript.Entities
			?.Where(x => x.EntityType == type)
			.Select(x => x.Text)
			.Distinct()
			.ToList() ?? [];
	}

	/// <summary>
	/// Ensures this API is working correctly. Called at app startup
	/// </summary>
	public async Task EnsureApiIsFunctional()
	{
		await _client.Transcripts.ListAsync();
	}
}
