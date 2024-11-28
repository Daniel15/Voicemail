namespace Voicemail.Services;

/// <summary>
/// Represents a service that calls a third-party API.
/// </summary>
public interface IThirdPartyApi
{
	/// <summary>
	/// Ensures this API is working correctly. Called at app startup.
	/// </summary>
	public Task EnsureApiIsFunctional();
}
