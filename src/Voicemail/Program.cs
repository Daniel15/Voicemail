// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2024 Daniel Lo Nigro <d@d.sb>

using System.Net.Http.Headers;
using AssemblyAI;
using Coravel;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Twilio.AspNet.Core;
using Voicemail;
using Voicemail.Configuration;
using Voicemail.Extensions;
using Voicemail.Services;

var builder = WebApplication.CreateBuilder(args);

// Add hard-coded settings
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
	{"Coravel:Queue:ConsummationDelay", "1"}
});

var services = builder.Services;
builder.AddMailer();
services.AddControllers();
services.AddDbContext<VoicemailContext>();
services.Configure<ForwardedHeadersOptions>(
	options => options.ForwardedHeaders = ForwardedHeaders.All
);
services.Configure<TrestleConfig>(builder.Configuration.GetSection("Trestle"));
services.AddQueue();
services.AddHttpClient();
services.ConfigureHttpClientDefaults(options =>
{
	options.ConfigureHttpClient(client =>
	{
		client.DefaultRequestHeaders.UserAgent.Add(
			new ProductInfoHeaderValue("Daniel15-Voicemail", "1.0.0")
		);
	});
});
services.AddTransient<VoicemailProcessor>();

// Third-party services
services.AddTwilioClient();
services.AddTwilioRequestValidation();
services.AddAssemblyAIClient();
services.AddScoped<IPhoneService, TwilioService>();
services.AddScoped<ITranscriptionService, AssemblyAiService>();
services.AddSingleton<ICallerIdService, TrestleService>();

var app = builder.Build();
app.EnableQueueLogging();
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapGet("/", () => "Voicemail Server");
app.MapControllers();

Console.WriteLine($"Storing data in {VoicemailContext.DataPath}");
Directory.CreateDirectory(VoicemailContext.DataPath);
Directory.CreateDirectory(Path.Combine(VoicemailContext.DataPath, "recordings"));

using (var scope = app.Services.CreateScope())
{
	Console.WriteLine("Running DB migrations if needed...");
	var db = scope.ServiceProvider.GetRequiredService<VoicemailContext>();
	db.Database.Migrate();
	
	Console.WriteLine("Checking third-party APIs work...");
	await scope.ServiceProvider.GetRequiredService<ITranscriptionService>().EnsureApiIsFunctional();
	await scope.ServiceProvider.GetRequiredService<IPhoneService>().EnsureApiIsFunctional();
	await scope.ServiceProvider.GetRequiredService<ICallerIdService>().EnsureApiIsFunctional();
}

Console.WriteLine("Ready to rock!");
app.Run();
