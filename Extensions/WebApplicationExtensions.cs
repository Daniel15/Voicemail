using Coravel;
using Coravel.Queuing.Interfaces;

namespace Voicemail.Extensions;

/// <summary>
/// Extension methods for <see cref="WebApplication"/>
/// </summary>
public static class WebApplicationExtensions
{
	/// <summary>
	/// Enable logging of errors and task progress for the queue.
	/// </summary>
	public static WebApplication EnableQueueLogging(this WebApplication app)
	{
		var logger = app.Services.GetRequiredService<ILogger<IQueue>>();
		app.Services.ConfigureQueue()
			.OnError(ex => logger.LogError(
				ex, 
				"Unhandled exception while processing a background task"
			))
			.LogQueuedTaskProgress(logger);
		return app;
	}
}
