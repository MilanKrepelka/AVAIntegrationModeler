using Ardalis.ListStartupServices;
using Ardalis.Result;
using AVAIntegrationModeler.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AVAIntegrationModeler.API.Configurations;

public static class MiddlewareConfig
{
  public static async Task<IApplicationBuilder> UseAppMiddlewareAndSeedDatabase(this WebApplication app)
  {
    if (app.Environment.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseShowAllServicesMiddleware(); // see https://github.com/ardalis/AspNetCoreStartupServices
    }
    else
    {
      app.UseDefaultExceptionHandler(); // from FastEndpoints
      app.UseHsts();
    }

    app.UseResponseCaching();
    app.UseFastEndpoints();
    app.UseSwaggerGen(); // Includes AddFileServer and static files middleware

    app.UseHttpsRedirection(); // Note this will drop Authorization headers

    await SeedDatabase(app);

    return app;
  }

  static async Task SeedDatabase(WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
      var context = services.GetRequiredService<AppDbContext>();

      // Pokud databáze existuje ale nemá __EFMigrationsHistory (vytvořena starším EnsureCreated),
      // smažeme ji — MigrateAsync ji znovu vytvoří kompletně správně.
      if (await context.Database.CanConnectAsync() && !await HasMigrationHistoryAsync(context))
      {
        await context.Database.EnsureDeletedAsync();
      }

      await context.Database.MigrateAsync();
      await SeedData.InitializeAsync(context);
    }
    catch (Exception ex)
    {
      var logger = services.GetRequiredService<ILogger<Program>>();
      logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
    }
  }

  static async Task<bool> HasMigrationHistoryAsync(AppDbContext context)
  {
    try
    {
      var conn = context.Database.GetDbConnection();
      var wasOpen = conn.State == System.Data.ConnectionState.Open;
      if (!wasOpen) await conn.OpenAsync();
      using var cmd = conn.CreateCommand();
      cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='__EFMigrationsHistory'";
      var result = await cmd.ExecuteScalarAsync();
      if (!wasOpen) await conn.CloseAsync();
      return Convert.ToInt64(result) > 0;
    }
    catch
    {
      return false;
    }
  }

    public static void UseResultToProblemDetails(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            await next();

            // Pokud endpoint vrací Result a ten není Success, převeď na ProblemDetails
            if (context.Response.StatusCode == 200 && context.Items["Result"] is Result result && !result.IsSuccess)
            {
                var problemDetails = result.Status switch
                {
                    ResultStatus.Invalid => new ValidationProblemDetails(
                        result.ValidationErrors.GroupBy(e => e.Identifier ?? "Model")
                              .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Validační chyba"
                    },
                    ResultStatus.NotFound => new Microsoft.AspNetCore.Mvc.ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "Nenalezeno",
                        Detail = string.Join("; ", result.Errors)
                    },
                    ResultStatus.Conflict => new Microsoft.AspNetCore.Mvc.ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = "Konflikt",
                        Detail = string.Join("; ", result.Errors)
                    },
                    _ => new Microsoft.AspNetCore.Mvc.ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Chyba",
                        Detail = string.Join("; ", result.Errors)
                    }
                };

                context.Response.StatusCode = problemDetails.Status ?? 500;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        });
    }
}
