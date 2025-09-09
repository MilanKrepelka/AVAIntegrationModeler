using AVAIntegrationModeler.Core.Interfaces;
using AVAIntegrationModeler.Core.Services;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Infrastructure.Data.Queries;
using AVAIntegrationModeler.UseCases.Contributors.List;
using AVAIntegrationModeler.UseCases.Scenarios.List;


namespace AVAIntegrationModeler.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ILogger logger)
  {
    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
            .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
            .AddScoped<IListContributorsQueryService, ListContributorsQueryService>()
            .AddScoped<IListScenariosQueryService, ListScenariosQueryService>()
            .AddScoped<IDeleteContributorService, DeleteContributorService>();

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }

  /// <summary>
  /// Přidá služby databáze bez dalších závislostí, vhodné pro testování
  /// </summary>
  /// <param name="services"><see cref="IServiceCollection"/></param>
  /// <param name="config"><see cref="ConfigurationManager"/></param>
  /// <param name="logger"><see cref="ILogger"/></param>
  /// <returns><see cref="IServiceCollection"/></returns>
  public static IServiceCollection AddDatabaseServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {

    string? connectionString = config.GetConnectionString("SqliteConnection");
    Guard.Against.Null(connectionString);
    services.AddDbContext<AppDbContext>(options =>
     options.UseSqlite(connectionString));
    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
