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
		
		// TODO: Recipient should be customizable
		To("vm@d.sb")
			.Subject($"Voicemail from {formattedCaller}") 
			.View("~/Views/Mail/NewMessage.cshtml", new ViewModel(
				Call: _call,
				FormattedCaller: formattedCaller
			));
	}

	public readonly record struct ViewModel(
		Call Call,
		string FormattedCaller
	);
}
