// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2025 Daniel Lo Nigro <d@d.sb>

using Voicemail.Models;

namespace Voicemail.Providers;

/// <summary>
/// Represents a provider that can transcribe phone calls.
/// </summary>
public interface ITranscriptionProvider : IThirdPartyApi
{
	/// <summary>
	/// Transcribe the phone call recording at the specified URL.
	/// </summary>
	public Task<Transcript> Transcribe(string recordingUrl);
}
