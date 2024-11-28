namespace Voicemail.Models.Twilio;

/// <summary>
/// https://www.twilio.com/docs/voice/twiml/record#attributes-recording-status-callback-parameters
/// </summary>
public record RecordingStatusCallbackData(
	string RecordingSource,
	string RecordingSid,
	string RecordingUrl,
	string RecordingStatus,
	int RecordingChannels,
	int ErrorCode,
	string CallSid,
	string RecordingStartTime,
	string AccountSid,
	int RecordingDuration
);
