using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;

namespace Voicemail.Services;

public class TwilioService(ITwilioRestClient _client): IPhoneService
{
	/// <summary>
	/// Ensures this API is working correctly. Called at app startup
	/// </summary>
	public async Task EnsureApiIsFunctional()
	{
		await CallResource.ReadAsync(limit: 1, client: _client);
	}
}
