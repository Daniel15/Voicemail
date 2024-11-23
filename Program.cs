using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Twilio.AspNet.Core;
using Voicemail;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddControllers();
services.AddDbContext<VoicemailContext>();
services.Configure<ForwardedHeadersOptions>(
	options => options.ForwardedHeaders = ForwardedHeaders.All
);
services.AddTwilioClient();
services.AddTwilioRequestValidation();

var app = builder.Build();
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapGet("/", () => "Voicemail Server");
app.MapControllers();

Console.WriteLine($"Storing data in {VoicemailContext.DataPath}");
Directory.CreateDirectory(VoicemailContext.DataPath);
Directory.CreateDirectory(Path.Combine(VoicemailContext.DataPath, "recordings"));

Console.WriteLine("Running DB migrations if needed...");
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<VoicemailContext>();
	db.Database.Migrate();
}

app.Run();