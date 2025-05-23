using Microsoft.EntityFrameworkCore;
using Voicemail.Models;

namespace Voicemail;

public class VoicemailContext : DbContext
{
	/// <summary>
	/// Calls that have been received.
	/// </summary>
	public DbSet<Call> Calls { get; init; }
	
	/// <summary>
	/// Address book entries (manually entered caller ID entries).
	/// </summary>
	public DbSet<AddressBookEntry> AddressBook { get; init; }
	
	/// <summary>
	/// Path the data is being stored at.
	/// </summary>
	public static string DataPath => Path.Combine(AppContext.BaseDirectory, "data");

	protected override void OnConfiguring(DbContextOptionsBuilder options)
	{
		base.OnConfiguring(options);
		Directory.CreateDirectory(DataPath);
		var dbPath = Path.Combine(DataPath, "voicemail.db");
		options.UseSqlite($"Data Source={dbPath}");
	}
}
