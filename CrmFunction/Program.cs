using System.Text.Json;
using CrmFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// In the isolated worker model the func host transports local.settings.json Values as
// environment variables, but keys containing ':' are silently dropped or misrouted by
// the env-var configuration provider. Read the file directly and promote every entry
// in the Values section to the configuration root so IConfiguration["Email:SmtpHost"]
// works locally. In Azure the file is absent (optional) and App Settings env-vars apply.
if (File.Exists("local.settings.json"))
{
    using var stream = File.OpenRead("local.settings.json");
    using var doc = JsonDocument.Parse(stream);
    if (doc.RootElement.TryGetProperty("Values", out var values))
    {
        foreach (var prop in values.EnumerateObject())
            builder.Configuration[prop.Name] = prop.Value.GetString();
    }
}

builder.Services.AddSingleton<EmailService>();

builder.Build().Run();
