using Ardalis.SharedKernel;
using AVAIntegrationModeler.Infrastructure;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Integration.Test.Data.Data.SqlLite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace AVAIntegrationModeler.Integration.Test.Data.SqlLite.Fixtures;

/// <summary>
/// Fixtura pro testy s použitím Entity Framework a SQLite.
/// </summary>
public class EfSqlClientTestFixture : TestBedFixture
{
  protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
  {
    // Vytvoření fake event dispatchera
    var fakeEventDispatcher = Substitute.For<IDomainEventDispatcher>();
    services.AddTransient<IDomainEventDispatcher>(service => fakeEventDispatcher);

    // Přidání SQLite databáze pro testování
    services.AddSqlLiteDatabaseForTesting();

    // Přidání Infrastructure services (repositories, query services, atd.)
    services.AddInfrastructureServices(new LoggerFactory().CreateLogger(this.GetType()));
  }

  public EfSqlClientTestFixture()
  {
  }

  protected override IEnumerable<TestAppSettings> GetTestAppSettings()
  {
    return [];
  }

  protected override ValueTask DisposeAsyncCore()
  {
    return new ValueTask(Task.CompletedTask);
  }
}
