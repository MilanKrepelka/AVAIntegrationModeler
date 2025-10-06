

using System.Text.Json.Serialization;
using AVAIntegrationModeler.API.Configurations;
using AVAIntegrationModeler.AVAPlace.Extensions;
using AVAIntegrationModeler.UseCases.Contributors.Create;


var builder = WebApplication.CreateBuilder(args);

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

// wire up commands
//builder.Services.AddTransient<ICommandHandler<CreateContributorCommand2,Result<int>>, CreateContributorCommandHandler2>();

builder.AddServiceDefaults();


builder.Services.AddAVAPlaceServices(builder.Configuration);

var app = builder.Build();

await app.UseAppMiddlewareAndSeedDatabase();



app.Run();

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building

public partial class Program { }
