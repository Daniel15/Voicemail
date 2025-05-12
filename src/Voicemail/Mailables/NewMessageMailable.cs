// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2025 Daniel Lo Nigro <d@d.sb>

using Coravel.Mailer.Mail;
using Voicemail.Configuration;
using Voicemail.Extensions;
using Voicemail.Models;

namespace Voicemail.Mailables;
public class NewMessageMailable(
	Call _call,
	AccountConfig _account,
	byte[]? _recording
) : Mailable<NewMessageMailable.ViewModel>
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
		
		To(_account.Email)
			.Subject(subject)
			.View("~/Views/Mail/NewMessage.cshtml", new ViewModel(
				Call: _call,
				FormattedCaller: formattedCaller,
				Subject: subject
			));

		if (_recording != null)
		{
			Attach(new Attachment
			{
				Name = $"recording-{_call.Id}.mp3",
				Bytes = _recording,
			});
		}
	}

	public readonly record struct ViewModel(
		Call Call,
		string FormattedCaller,
		string Subject
	);
}
