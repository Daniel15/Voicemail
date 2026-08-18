// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Daniel Lo Nigro <d@d.sb>

using AssemblyAI;
using Microsoft.Extensions.Options;
using Voicemail.Configuration;

namespace Voicemail.Providers;

/// <summary>
/// Handles communicating with AssemblyAI's API.
/// </summary>
public class AssemblyAiProvider(IOptionsMonitor<AssemblyAIConfig> options)
	: ITranscriptionProvider
{
	private readonly AssemblyAIClient _client = new(options.CurrentValue.ApiKey);
	private readonly TimeSpan _timeout = TimeSpan.FromMinutes(5);

	/// <summary>
	/// Transcribe the phone call recording at the specified URL.
	/// </summary>
	public async Task<Voicemail.Models.Transcript> Transcribe(string recordingUrl)
	{
		var queued = await _client.Transcripts.SubmitAsync(
			TranscriptParams.FromUrl(recordingUrl, new TranscriptOptionalParams
			{
				EntityDetection = true,
				
			})
		);
		var transcript = await PollUntilComplete(queued.Id);

		return new Voicemail.Models.Transcript
		{
			TranscriptText = transcript.Text,
			MentionedCompanies = EntityTextByType(transcript, EntityType.Organization),
			MentionedNumbers = EntityTextByType(transcript, EntityType.PhoneNumber),
			MentionedPeople = EntityTextByType(transcript, EntityType.PersonName),
		};
	}

	private async Task<Transcript> PollUntilComplete(Guid id)
	{
		var startedAt = DateTime.UtcNow;
		while (DateTime.UtcNow - startedAt < _timeout)
		{
			var transcript = await _client.Transcripts.GetAsync(id.ToString());
			switch (transcript.Status)
			{
				case TranscriptStatus.Completed:
					return transcript;
				case TranscriptStatus.Error:
					throw new Exception($"Transcript failed: {transcript.Error ?? "No error message"}");
				default:
					await Task.Delay(TimeSpan.FromSeconds(1));
					break;
			}
		}
		throw new Exception("Transcription timed out");
	}
	
	private static List<string> EntityTextByType(Transcript transcript,  EntityType type)
	{
		return transcript.Entities
			?.Where(x => x.EntityType == type)
			.Select(x => x.Text.Trim())
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
