using Microsoft.AspNetCore.Mvc;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;
using Voicemail.Models;
using Voicemail.Models.Twilio;

namespace Voicemail;

[ApiController]
[Route("twilio")]
public class TwilioController(
	ILogger<TwilioController> _logger,
	IHttpClientFactory _httpClientFactory,
	VoicemailContext _dbContext
) : ControllerBase
{
	[Route("incoming_call")]
	[ValidateRequest]
	public async Task<TwiMLResult> IncomingCall([FromForm] VoiceRequest request) {
		_logger.LogInformation(
			"[{CallSid}] Received call from {Number}",
			request.CallSid,
			request.From
		);

		_dbContext.Calls.Add(new Call
		{
			ExternalId = request.CallSid,
			NumberFrom = request.From,
			NumberTo = request.To,
			NumberForwardedFrom = request.ForwardedFrom,
		});
		await _dbContext.SaveChangesAsync();
		
		var response = new VoiceResponse();
		response.Play(new Uri(Url.Content("~/greeting.mp3")));
		response.Record(
			action: new Uri(Url.ActionAbsolute("Twilio", "CallCompleted")),
			recordingStatusCallback: new Uri(Url.ActionAbsolute(
				"Twilio",
				"SaveRecording"
			))
		);
		return this.TwiML(response);
	}

	[Route("call_completed")]
	[ValidateRequest]
	public TwiMLResult CallCompleted([FromForm] VoiceRequest request)
	{
		_logger.LogInformation("[{CallSid}] Call completed", request.CallSid);
		var response = new VoiceResponse();
		response.Hangup();
		return this.TwiML(response);
	}
	
	[Route("save_recording")]
	[ValidateRequest]
	public async Task<IResult> SaveRecording([FromForm] RecordingStatusCallbackData data)
	{
		_logger.LogInformation("[{CallSid}] Received recording", data.CallSid);

		// Ensure call is recognised
		var call = _dbContext.Calls.FirstOrDefault(x => x.ExternalId == data.CallSid);
		if (call == null)
		{
			return Results.BadRequest("Call ID is not valid");
		}
		
		// Download recording
		var client = _httpClientFactory.CreateClient();
		var recordingResponse = await client.GetAsync(data.RecordingUrl);
		recordingResponse.EnsureSuccessStatusCode();
		var recordingFilePath =
			Path.Combine(VoicemailContext.DataPath, "recordings", $"{call.Id}.mp3");
		await using (var stream = System.IO.File.OpenWrite(recordingFilePath))
		{
			await recordingResponse.Content.CopyToAsync(stream);
		}
		
		call.RecordingDurationSeconds = data.RecordingDuration;
		call.RecordingUrl = data.RecordingUrl;
		await _dbContext.SaveChangesAsync();
	
		return Results.Created();
	}
}