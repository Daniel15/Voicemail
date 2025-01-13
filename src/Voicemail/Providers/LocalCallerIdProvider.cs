using PhoneNumbers;
using Voicemail.Extensions;
using Voicemail.Models;

namespace Voicemail.Providers;

/// <summary>
/// Handles caller ID lookups using the local database.
/// </summary>
public class LocalCallerIdProvider(VoicemailContext _dbContext) : ICallerIdProvider
{
	/// <inheritdoc/>
	public Task<CallerId?> GetCallerId(PhoneNumber phoneNumber)
	{
		var entry = _dbContext.AddressBook.FirstOrDefault(
			x => x.NumberRaw == phoneNumber.ToE164()
		);
		return Task.FromResult(entry is null
			? null
			: new CallerId { CallerName = entry.Name }
		);
	}
}
