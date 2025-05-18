// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2024 Daniel Lo Nigro <d@d.sb>

using System.Net.Http.Headers;
using AssemblyAI;
using Coravel;
using Microsoft.AspNetCore.HttpOverrides;
using PhoneNumbers;
using Twilio.AspNet.Core;
using Voicemail;
using Voicemail.Configuration;
using Voicemail.Extensions;
using Voicemail.Providers;
using Voicemail.Repositories;

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
services.Configure<List<AccountConfig>>(builder.Configuration.GetSection("Accounts"));
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
services.AddTransient<Initialization>();
services.AddSingleton<IRecordingRepository, LocalRecordingRepository>();
services.AddSingleton<IAccountRepository, ConfigAccountRepository>();

// Providers, both local and third-party.
services.AddTwilioClient();
services.AddTwilioRequestValidation();
services.AddAssemblyAIClient();
services.AddScoped<IPhoneProvider, TwilioProvider>();
services.AddScoped<ITranscriptionProvider, AssemblyAiProvider>();
services.AddScoped<ICallerIdProvider, LocalCallerIdProvider>();
services.AddSingleton<ICallerIdProvider, TrestleProvider>();
services.AddSingleton(PhoneNumberUtil.GetInstance());

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
	await scope.ServiceProvider.GetRequiredService<Initialization>().Initialize();
}

app.Run();
