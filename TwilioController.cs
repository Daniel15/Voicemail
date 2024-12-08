using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PhoneNumbers;
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
	VoicemailContext _dbContext,
	IQueue _queue
) : ControllerBase
{
	[Route("incoming_call")]
	[ValidateRequest]
	public async Task<TwiMLResult> IncomingCall([FromForm] VoiceRequest request)
	{
		var numberParser = PhoneNumberUtil.GetInstance();
		var number = numberParser.Parse(request.From, "US");
		var formattedNumber = numberParser.Format(number, PhoneNumberFormat.NATIONAL);
		_logger.LogInformation(
			"[{CallSid}] Received call from {Number}",
			request.CallSid,
			formattedNumber
		);

		_dbContext.Calls.Add(new Call
		{
			ExternalId = request.CallSid,
			NumberFromRaw = numberParser.Format(number, PhoneNumberFormat.E164),
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
		
		_logger.LogInformation("[{CallSid}] Internal ID is {Id}", call.ExternalId, call.Id);
		
		call.RecordingDurationSeconds = data.RecordingDuration;
		// Twilio provides a WAV by default, but there's no disadvantage to getting an MP3 instead.
		call.RecordingUrl = data.RecordingUrl + ".mp3";
		await _dbContext.SaveChangesAsync();

		_queue.QueueInvocableWithPayload<VoicemailProcessor, int>(call.Id);
		_logger.LogInformation("[{Id}] Added to processing queue", call.Id);
		return Results.Created();
	}
}
