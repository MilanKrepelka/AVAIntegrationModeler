using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.Interfaces;
using AVAIntegrationModeler.Domain.Services;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Infrastructure.Data.Queries;
using AVAIntegrationModeler.Infrastructure.Infrastructure.Data;
using AVAIntegrationModeler.UseCases.Areas;
using AVAIntegrationModeler.UseCases.Contributors.List;
using AVAIntegrationModeler.UseCases.DataModelRecords;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.Deployments;
using AVAIntegrationModeler.UseCases.Features;
using AVAIntegrationModeler.UseCases.IntegrationMaps;
using AVAIntegrationModeler.UseCases.Scenarios;

namespace AVAIntegrationModeler.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ILogger logger)
  {
    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
            .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
            .AddScoped<IDataModelRepository, DataModelRepository>()
            .AddScoped<IDataModelRecordRepository, DataModelRecordRepository>()
            .AddScoped<IDataModelRecordQueryService, DataModelRecordsQueryService>()
            .AddScoped<IListContributorsQueryService, ListContributorsQueryService>()
            .AddScoped<IAreasQueryService, AreasQueryService>()
            .AddScoped<IScenariosQueryService, ScenariosQueryService>()
            .AddScoped<IFeaturesQueryService, FeaturesQueryService>()
            .AddScoped<IDataModelQueryService, DataModelsQueryService>()
            .AddScoped<IIntegrationMapsQueryService, IntegrationMapsQueryService>()
            .AddScoped<IDeploymentsQueryService, DeploymentsQueryService>()
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

  public static IServiceCollection AddDomainValidationServices(
    this IServiceCollection services,
    ILogger logger)
  {
    services.AddScoped<IDomainEntityValidationService<Scenario>, ValidationServices.ScenarioValidationService>();
    logger.LogInformation("DomainValidationServices services registered");
    return services;
  }
}
