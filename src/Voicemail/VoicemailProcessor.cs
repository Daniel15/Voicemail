// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2024 Daniel Lo Nigro <d@d.sb>

using System.Text.Json;
using Coravel.Invocable;
using Coravel.Mailer.Mail.Interfaces;
using Voicemail.Extensions;
using Voicemail.Mailables;
using Voicemail.Models;
using Voicemail.Providers;
using Voicemail.Repositories;

namespace Voicemail;

/// <summary>
/// Background processing of voicemail messages.
/// </summary>
public class VoicemailProcessor(
	ILogger<VoicemailProcessor> _logger,
	VoicemailContext _dbContext,
	ITranscriptionProvider transcriptionProvider,
	IEnumerable<ICallerIdProvider> _callerIdProviders,
	IMailer _mailer,
	IRecordingRepository _recordingRepository,
	IHttpClientFactory _httpClientFactory 
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
		var callerIdTask = GetCallerId(call);

		await downloadTask;
		var transcript = await transcriptTask;
		var callerId = await callerIdTask;

		if (transcript != null)
		{
			transcript.CallId = call.Id;
			call.Transcript = transcript;
		}

		if (callerId != null)
		{
			callerId.CallId = call.Id;
			call.CallerId = callerId;
		}

		call.Processed = true;
		await _dbContext.SaveChangesAsync();
		_logger.LogInformation("[{Id}] Processed call", call.Id);

		await SendEmail(call);
	}

	/// <summary>
	/// Download recording and store it locally
	/// </summary>
	private async Task Download(Call call)
	{
		if (call.RecordingUrl == null)
		{
			_logger.LogInformation("[{Id}] No recording URL; couldn't download recording", call.Id);
			return;
		}
		
		_logger.LogInformation("[{Id}] Downloading recording", call.Id);
		try
		{
			var client = _httpClientFactory.CreateClient();
			var recordingResponse = await client.GetAsync(call.RecordingUrl);
			recordingResponse.EnsureSuccessStatusCode();
			await using var stream = await recordingResponse.Content.ReadAsStreamAsync();
			await _recordingRepository.Save(call, stream);
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
			_logger.LogInformation("[{Id}][Transcribe] Recording URL is missing", call.Id);
			return null;
		}
		
		_logger.LogInformation("[{Id}][Transcribe] Starting", call.Id);
		try
		{
			var result = await transcriptionProvider.Transcribe(call.RecordingUrl);
			_logger.LogInformation(
				"[{Id}][Transcribe] Result: {Result}", 
				call.Id, 
				JsonSerializer.Serialize(result)
			);
			return result;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "[{Id}][Transcribe] FAILED: {Error}", call.Id, ex.Message);
			return null;
		}
	}

	private async Task<CallerId?> GetCallerId(Call call)
	{
		if (call.NumberFrom is null)
		{
			_logger.LogWarning(
				"[{Id}][CallerID] Phone number is missing; couldn't get caller ID",
				call.Id
			);
			return null;
		}
		
		_logger.LogInformation("[{Id}][CallerID] Starting", call.Id);
		try
		{
			// Try all caller ID providers sequentially - it's assumed that 'cheaper' providers
			// are listed before 'expensive' providers, and that we don't want to call the expensive
			// providers at all if a cheaper provider (e.g. a local DB lookup) has the result.
			foreach (var provider in _callerIdProviders)
			{
				_logger.LogInformation(
					"[{Id}][CallerID] Trying {Provider}",
					call.Id,
					provider.GetType().Name
				);
				var result = await provider.GetCallerId(call.NumberFrom);
				if (result != null)
				{
					_logger.LogInformation(
						"[{Id}][CallerID] Identified: {Number} -> {CallerId}'",
						call.Id,
						call.NumberFrom.ToPrettyFormat(),
						JsonSerializer.Serialize(result)
					);
					return result;	
				}
			}
			_logger.LogInformation(
				"[{Id}][CallerID] No caller ID provider returned a result",
				call.Id
			);
			return null;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "[{Id}][CallerID] FAILED: {Error}", call.Id, ex.Message);
			return null;
		}
	}

	private async Task SendEmail(Call call)
	{
		_logger.LogInformation("[{Id}][SendEmail] Sending email", call.Id);
		try
		{
			byte[]? recording = null;
			await using var stream = _recordingRepository.Get(call);
			if (stream != null)
			{
				using var memoryStream = new MemoryStream();
				await stream.CopyToAsync(memoryStream);
				recording = memoryStream.ToArray();
			}
			await _mailer.SendAsync(new NewMessageMailable(call, recording));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "[{Id}][SendEmail] FAILED: {Error}", call.Id, ex.Message);
		}
	}
}
