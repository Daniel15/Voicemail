// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2025 Daniel Lo Nigro <d@d.sb>

using Microsoft.Extensions.Options;
using PhoneNumbers;
using Voicemail.Configuration;
using Voicemail.Extensions;

namespace Voicemail.Repositories;

/// <summary>
/// Repository that gets accounts from the appsettings.json config file.
/// </summary>
public class ConfigAccountRepository : IAccountRepository
{
	private readonly IOptionsMonitor<List<AccountConfig>> _accounts;

	public ConfigAccountRepository(IOptionsMonitor<List<AccountConfig>> accounts)
	{
		_accounts = accounts;
	}
	
	/// <inheritdoc />
	public AccountConfig? TryGetAccount(PhoneNumber number)
	{
		var numberE164 = number.ToE164();
		return _accounts.CurrentValue.FirstOrDefault(x => x.PhoneNumber == numberE164);
	}

	/// <inheritdoc />
	public AccountConfig GetAccount(PhoneNumber number)
	{
		return TryGetAccount(number) ?? 
		       throw new ArgumentException($"Account {number.ToPrettyFormat()} not found");
	}

	public IEnumerable<AccountConfig> GetAllAccounts()
	{
		return _accounts.CurrentValue.ToList();
	}
}
