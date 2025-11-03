using AVAIntegrationModeler.Domain.Interfaces;
using AVAIntegrationModeler.Domain.Services;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Infrastructure.Data.Queries;
using AVAIntegrationModeler.Infrastructure.Infrastructure.Data;
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
            .AddScoped<UseCases.Features.List.IListFeaturesQueryService, ListFeaturesQueryService>()
            .AddScoped<UseCases.DataModels.List.IListDataModelQueryService, ListDataModelsQueryService>()
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
    services.AddScoped<Infrastructure.Data.EventDispatchInterceptor>();
    string? connectionString = config.GetConnectionString("SqliteConnection");
    Guard.Against.Null(connectionString);

    services.AddDbContext<AppDbContext>((provider, options) =>
    {
      var eventDispatchInterceptor = provider.GetRequiredService<EventDispatchInterceptor>();
      options.UseSqlite(connectionString);
      options.AddInterceptors(eventDispatchInterceptor);
    });
    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
