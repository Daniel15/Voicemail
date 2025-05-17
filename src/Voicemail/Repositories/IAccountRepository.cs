// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2025 Daniel Lo Nigro <d@d.sb>

using PhoneNumbers;
using Voicemail.Configuration;

namespace Voicemail.Repositories;

/// <summary>
/// Repository for loading accounts.
/// </summary>
public interface IAccountRepository
{
	/// <summary>
	/// Gets the account attached to the specified phone number, or <c>null</c> if the account is
	/// not recognized
	/// </summary>
	public AccountConfig? TryGetAccount(PhoneNumber number);
	
	/// <summary>
	/// Gets the account attached to the specified phone number, or throws if the account is not
	/// recognized.
	/// </summary>
	public AccountConfig GetAccount(PhoneNumber number);

	/// <summary>
	/// Gets all the registered accounts.
	/// </summary>
	/// <returns></returns>
	public IEnumerable<AccountConfig> GetAllAccounts();
}
