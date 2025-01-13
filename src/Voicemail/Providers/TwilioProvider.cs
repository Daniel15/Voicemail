using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;

namespace Voicemail.Providers;

public class TwilioProvider(ITwilioRestClient _client): IPhoneProvider
{
	/// <summary>
	/// Ensures this API is working correctly. Called at app startup
	/// </summary>
	public async Task EnsureApiIsFunctional()
	{
		await CallResource.ReadAsync(limit: 1, client: _client);
	}
}
