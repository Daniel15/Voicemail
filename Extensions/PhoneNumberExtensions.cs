using PhoneNumbers;

namespace Voicemail.Extensions;

/// <summary>
/// Extensions to <see cref="PhoneNumber"/>.
/// </summary>
public static class PhoneNumberExtensions
{
	public static string ToPrettyFormat(this PhoneNumber phoneNumber)
	{
		return PhoneNumberUtil.GetInstance().Format(phoneNumber, PhoneNumberFormat.NATIONAL);
	}

	public static string ToE164(this PhoneNumber phoneNumber)
	{
		return PhoneNumberUtil.GetInstance().Format(phoneNumber, PhoneNumberFormat.E164);
	}
}
