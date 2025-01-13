// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2025 Daniel Lo Nigro <d@d.sb>

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PhoneNumbers;

namespace Voicemail.Models;

/// <summary>
/// Represents manually-entered data about a person / phone number.
/// </summary>
[Table("AddressBookEntries")]
[Index(nameof(NumberRaw), IsUnique = true)]
public class AddressBookEntry
{
	/// <summary>
	/// Auto-increment ID for the address book entry.
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// Phone number for this entry, in E164 format.
	/// </summary>
	[Column("Number")]
	public required string NumberRaw { get; set; }
	
	/// <summary>
	/// Phone number for this entry
	/// </summary>
	public PhoneNumber Number => PhoneNumberUtil.GetInstance().Parse(NumberRaw, "US");
	
	/// <summary>
	/// Name for this entry
	/// </summary>
	public required string Name { get; set; }
}
