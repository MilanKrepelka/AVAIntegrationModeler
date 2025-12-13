using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AVAIntegrationModeler.API.Configurations;
using AVAIntegrationModeler.AVAPlace.Extensions;
using AVAIntegrationModeler.UseCases.Contributors.Create;
using Serilog;
using Serilog.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
namespace AVAIntegrationModeler.API;
public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var urls = builder.Configuration["ASPNETCORE_URLS"]
                   ?? builder.Configuration.GetSection("AVAIntegrationModelerAPIOptions").GetValue<string>("BaseUrl")
                   ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
                   ?? "http://0.0.0.0:5005"; // fallback

        var logger = Log.Logger = new LoggerConfiguration()
          .Enrich.FromLogContext()
          .WriteTo.Console()
          .CreateLogger();

        logger.Information("Starting web host");

        builder.AddLoggerConfigs();

        var appLogger = new SerilogLoggerFactory(logger)
            .CreateLogger<Program>();

        builder.Services.AddOptionConfigs(builder.Configuration, appLogger, builder);
        builder.Services.AddServiceConfigs(appLogger, builder);

        builder.Services.ConfigureHttpJsonOptions(opt =>
        {
          opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddFastEndpoints()
                        .SwaggerDocument(o =>
                        {
                          o.ShortSchemaNames = true;
                        })
                        .AddCommandMiddleware(c =>
                        {
                          c.Register(typeof(CommandLogger<,>));
                        });
        builder.Services.AddResponseCaching();
        builder.Services.AddMemoryCache();

        // wire up commands
        //builder.Services.AddTransient<ICommandHandler<CreateContributorCommand2,Result<int>>, CreateContributorCommandHandler2>();

        builder.AddServiceDefaults();

        builder.Services.AddAVAPlaceServices(builder.Configuration);

        // Apply computed URLs to the host
        builder.WebHost.UseUrls(urls);

        var app = builder.Build();

        await app.UseAppMiddlewareAndSeedDatabase();

        app.Run();
    }
}
