namespace Voicemail.Providers;

/// <summary>
/// Represents a third-party API provider.
/// </summary>
public interface IThirdPartyApi
{
	/// <summary>
	/// Ensures this API is working correctly. Called at app startup.
	/// </summary>
	public Task EnsureApiIsFunctional();
}
