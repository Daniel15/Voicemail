// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2025 Daniel Lo Nigro <d@d.sb>

using Microsoft.EntityFrameworkCore;
using PhoneNumbers;
using Voicemail.Providers;
using Voicemail.Repositories;
using static Voicemail.Constants;

namespace Voicemail;

/// <summary>
/// Handles initialization that needs to run on startup.
/// </summary>
public class Initialization(
	VoicemailContext _dbContext,
	ITranscriptionProvider _transcriptionProvider,
	IPhoneProvider _phoneProvider,
	IEnumerable<ICallerIdProvider> _callerIdProviders,
	IAccountRepository _accountRepository,
	IWebHostEnvironment _webHostEnvironment,
	ILogger<Initialization> _logger
)
{

	public async Task Initialize()
	{
		_logger.LogInformation("Running DB migrations if needed...");
		await _dbContext.Database.MigrateAsync();
		
		await EnsureApisAreFunctional();
		CheckAccounts();
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

	private void CheckAccounts()
	{
		_logger.LogInformation("Checking all accounts are valid");
		var numberParser = PhoneNumberUtil.GetInstance();
		var accounts = _accountRepository.GetAllAccounts();
		foreach (var account in accounts)
		{
			var number = numberParser.Parse(account.PhoneNumber, DefaultRegion);
			if (number == null)
			{
				throw new Exception($"Unknown phone number format: {account.PhoneNumber}");
			}
			
			var greetingPath = Path.Combine(_webHostEnvironment.WebRootPath, account.GreetingFile);
			if (!File.Exists(greetingPath))
			{
				throw new Exception($"Could not find greeting file: {greetingPath}");
			}
		}
	}
}
