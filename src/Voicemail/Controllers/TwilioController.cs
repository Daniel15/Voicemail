// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2025 Daniel Lo Nigro <d@d.sb>

using System.Net;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PhoneNumbers;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;
using Voicemail.Extensions;
using Voicemail.Models;
using Voicemail.Models.Twilio;
using Voicemail.Repositories;
using static Voicemail.Constants;

namespace Voicemail.Controllers;

[ApiController]
[Route("twilio")]
public class TwilioController(
	ILogger<TwilioController> _logger,
	VoicemailContext _dbContext,
	IAccountRepository _accounts,
	IQueue _queue,
	PhoneNumberUtil _numberParser
) : ControllerBase
{
	private const string CALL_STATUS_COMPLETED = "completed";
	
	[Route("incoming_call")]
	[ValidateRequest]
	public async Task<TwiMLResult> IncomingCall([FromForm] VoiceRequest request)
	{
		var numberFrom = _numberParser.Parse(request.From, DefaultRegion);

		var numberForwardedFromRaw = request.ForwardedFrom ?? request.Digits;
		if (numberForwardedFromRaw is null)
		{
			_logger.LogInformation(
				"[{CallSid}] Received call from {Number} with no valid forwarded number. Prompting for mailbox number",
				request.CallSid,
				numberFrom.ToPrettyFormat()
			);
			var requestNumberResponse = new VoiceResponse();
			requestNumberResponse.Say(
				"Please enter the mailbox you want to leave a message for, "+
				"then press pound."
			);
			requestNumberResponse.Gather(finishOnKey: "#");
			return this.TwiML(requestNumberResponse);
		}
		
		var numberForwardedFrom = _numberParser.Parse(numberForwardedFromRaw, DefaultRegion);
		_logger.LogInformation(
			"[{CallSid}] Received call from {Number} for {NumberForwardedFrom}",
			request.CallSid,
			numberFrom.ToPrettyFormat(),
			numberForwardedFrom.ToPrettyFormat()
		);
		
		// Ensure phone number is actually in the system.
		var account = _accounts.TryGetAccount(numberForwardedFrom);
		if (account is null)
		{
			_logger.LogInformation(
				"[{CallSid}] Invalid mailbox! Returning an error.",
				request.CallSid
			);
			var errorResponse = new VoiceResponse();
			errorResponse.Say("Sorry, this is an invalid mailbox. Good bye.");
			return this.TwiML(errorResponse);
		}

		var call = new Call
		{
			ExternalId = request.CallSid,
			NumberFromRaw = numberFrom.ToE164(),
			NumberTo = request.To,
			NumberForwardedFromRaw = request.ForwardedFrom,
		};
		_dbContext.Calls.Add(call);
		await _dbContext.SaveChangesAsync();
		_logger.LogInformation(
			"[{CallSid}] Saved as {Id}",
			request.CallSid,
			call.Id
		);
		
		var response = new VoiceResponse();
		response.Play(new Uri(Url.Content("~/" + account.GreetingFile)));
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
