using Microsoft.EntityFrameworkCore;
using Voicemail.Providers;

namespace Voicemail;

/// <summary>
/// Handles initialization that needs to run on startup.
/// </summary>
public class Initialization(
	VoicemailContext _dbContext,
	ITranscriptionProvider _transcriptionProvider,
	IPhoneProvider _phoneProvider,
	IEnumerable<ICallerIdProvider> _callerIdProviders,
	ILogger<Initialization> _logger
)
{

	public async Task Initialize()
	{
		_logger.LogInformation("Running DB migrations if needed...");
		await _dbContext.Database.MigrateAsync();
		
		await EnsureApisAreFunctional();
		_logger.LogInformation("Ready to rock!");
	}

	private async Task EnsureApisAreFunctional()
	{
		var apis = new List<object>
		{
			_transcriptionProvider, 
			_phoneProvider
		}.Concat(_callerIdProviders);
		foreach (var api in apis.Where(x => x is IThirdPartyApi))
		{
			_logger.LogInformation("Ensuring {ApiName} works", api.GetType().Name);
			await ((IThirdPartyApi)api).EnsureApiIsFunctional();
		}
	}
}
