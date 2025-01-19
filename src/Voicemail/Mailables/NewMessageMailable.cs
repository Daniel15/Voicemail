// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2024 Daniel Lo Nigro <d@d.sb>

using Coravel.Mailer.Mail;
using Voicemail.Extensions;
using Voicemail.Models;

namespace Voicemail.Mailables;
public class NewMessageMailable(Call _call) : Mailable<NewMessageMailable.ViewModel>
{
	public override void Build()
	{
		var formattedCaller = $"{_call.NumberFrom?.ToPrettyFormat() ?? "Unknown Caller"}";
		if (_call.CallerId?.CallerName != null)
		{
			formattedCaller += $" ({_call.CallerId.CallerName})";
		}
		var subject = _call.IsCompleted
			? $"Voicemail from {formattedCaller}"
			: $"Missed call from {formattedCaller}";
		
		// TODO: Recipient should be customizable
		To("vm@d.sb")
			.Subject(subject)
			.View("~/Views/Mail/NewMessage.cshtml", new ViewModel(
				Call: _call,
				FormattedCaller: formattedCaller,
				Subject: subject
			));
		
		// TODO: Remove duplication with VoicemailProcessor
		var recordingFilePath =
			Path.Combine(VoicemailContext.DataPath, "recordings", $"{_call.Id}.mp3");
		if (File.Exists(recordingFilePath))
		{
			var bytes = File.ReadAllBytes(recordingFilePath);
			Attach(new Attachment
			{
				Name = Path.GetFileName(recordingFilePath),
				Bytes = bytes,
			});
		}
	}

	public readonly record struct ViewModel(
		Call Call,
		string FormattedCaller,
		string Subject
	);
}
