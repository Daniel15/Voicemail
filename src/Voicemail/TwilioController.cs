// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2025 Daniel Lo Nigro <d@d.sb>

using System.Net;
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
	private const string CALL_STATUS_COMPLETED = "completed";
	
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

		var call = new Call
		{
			ExternalId = request.CallSid,
			NumberFromRaw = numberParser.Format(number, PhoneNumberFormat.E164),
			NumberTo = request.To,
			NumberForwardedFrom = request.ForwardedFrom,
		};
		_dbContext.Calls.Add(call);
		await _dbContext.SaveChangesAsync();
		_logger.LogInformation(
			"[{CallSid}] Saved as {Id}",
			request.CallSid,
			call.Id
		);
		
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
	public IResult CallCompleted([FromForm] VoiceRequest request)
	{
		var call = FindCall(request.CallSid);
		_logger.LogInformation("[{Id}] Recording completed", call.Id);
		call.IsCompleted = true;
		_dbContext.SaveChanges();
		
		var response = new VoiceResponse();
		response.Hangup();
		return this.TwiML(response);
	}
	
	[Route("save_recording")]
	[ValidateRequest]
	public async Task<IResult> SaveRecording([FromForm] RecordingStatusCallbackData data)
	{
		var call = FindCall(data.CallSid);
		_logger.LogInformation("[{Id}] Received recording", call.Id);
		
		call.RecordingDurationSeconds = data.RecordingDuration;
		// Twilio provides a WAV by default, but there's no disadvantage to getting an MP3 instead.
		call.RecordingUrl = data.RecordingUrl + ".mp3";
		await _dbContext.SaveChangesAsync();

		_queue.QueueInvocableWithPayload<VoicemailProcessor, int>(call.Id);
		_logger.LogInformation("[{Id}] Added to processing queue", call.Id);
		return Results.Created();
	}

	[Route("status_update")]
	[ValidateRequest]
	public IResult StatusUpdate([FromForm] StatusCallbackRequest data)
	{
		var call = FindCall(data.CallSid);
		_logger.LogInformation("[{Id}] Call status = {Status}", call.Id, data.CallStatus);
		if (data.CallStatus == CALL_STATUS_COMPLETED && !call.IsCompleted)
		{
			_logger.LogInformation(
				"[{Id}] Caller hung up before leaving a message. Adding to processing queue",
				call.Id
			);
			_queue.QueueInvocableWithPayload<VoicemailProcessor, int>(call.Id);
		}
		return Results.Ok();
	}

	private Call FindCall(string callSid)
	{
		return _dbContext.Calls.FirstOrDefault(x => x.ExternalId == callSid) 
			?? throw new HttpRequestException(
				message: "Could not find call",
				statusCode: HttpStatusCode.BadRequest,
				inner: null
			);
	}
}
