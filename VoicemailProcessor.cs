using Coravel.Invocable;
using Voicemail.Models;
using Voicemail.Services;

namespace Voicemail;

/// <summary>
/// Background processing of voicemail messages.
/// </summary>
public class VoicemailProcessor(
	ILogger<VoicemailProcessor> _logger,
	IHttpClientFactory _httpClientFactory,
	VoicemailContext _dbContext,
	ITranscriptionService _transcriptionService
)
	: IInvocable, IInvocableWithPayload<int>
{
	/// <summary>
	/// Payload = Call ID
	/// </summary>
	public int Payload { get; set; }
	
	public async Task Invoke()
	{
		_logger.LogInformation("[{Id}] Processing call", Payload);
		
		var call = _dbContext.Calls.First(x => x.Id == Payload);

		// The tasks are intentionally not `await`ed here, so that they run concurrently.
		var downloadTask = Download(call);
		var transcriptTask = Transcribe(call);

		await downloadTask;
		var transcript = await transcriptTask;

		if (transcript != null)
		{
			transcript.CallId = call.Id;
			call.Transcript = transcript;
		}
		
		await _dbContext.SaveChangesAsync();
		_logger.LogInformation("[{Id}] Processed call", call.Id);
	}

	/// <summary>
	/// Download recording and store it locally
	/// </summary>
	private async Task Download(Call call)
	{
		_logger.LogInformation("[{Id}] Downloading recording", call.Id);
		try
		{
			var client = _httpClientFactory.CreateClient();
			var recordingResponse = await client.GetAsync(call.RecordingUrl);
			recordingResponse.EnsureSuccessStatusCode();
			var recordingFilePath =
				Path.Combine(VoicemailContext.DataPath, "recordings", $"{call.Id}.mp3");
			await using var stream = File.OpenWrite(recordingFilePath);
			await recordingResponse.Content.CopyToAsync(stream);
			_logger.LogInformation("[{Id}] Download complete", call.Id);
		}
		catch (Exception ex)
		{
			_logger.LogError(
				ex, 
				"[{Id}] Failed to download recording: {Error}", 
				call.Id,
				ex.Message
			);
		}
	}

	private async Task<Transcript?> Transcribe(Call call)
	{
		if (call.RecordingUrl == null)
		{
			_logger.LogError("[{Id}] Recording URL is missing", call.Id);
			return null;
		}
		
		_logger.LogInformation("[{Id}] Transcribing recording", call.Id);
		try
		{
			return await _transcriptionService.Transcribe(call.RecordingUrl);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "[{Id}] Failed to transcribe: {Error}", call.Id, ex.Message);
			return null;
		}
	}
}
