using Voicemail.Models;

namespace Voicemail.Services;

/// <summary>
/// Represents a service that can transcribe phone calls.
/// </summary>
public interface ITranscriptionService : IThirdPartyApi
{
	/// <summary>
	/// Transcribe the phone call recording at the specified URL.
	/// </summary>
	public Task<Transcript> Transcribe(string recordingUrl);
}
