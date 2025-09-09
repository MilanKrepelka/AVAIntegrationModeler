using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Core.ContributorAggregate;
using AVAIntegrationModeler.Infrastructure;
using AVAIntegrationModeler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using AVAIntegrationModeler.IntegrationTests.Data.SqlLite;

namespace AVAIntegrationModeler.IntegrationTests.Data.SqlLite.Fixtures;
/// <summary>
/// Fixtura pro testy s použitím Entity Framework a SQLLite.
/// </summary>
public class EfSqlClientTestFixture : TestBedFixture
{
  protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
  {
    var _fakeEventDispatcher = Substitute.For<IDomainEventDispatcher>();
    services.AddTransient<IDomainEventDispatcher>(service=> _fakeEventDispatcher);
    services.AddSqlLiteDatabaseForTesting();
    services.AddInfrastructureServices(new LoggerFactory().CreateLogger(this.GetType()));
  }
  
  public EfSqlClientTestFixture()
  {
  }
  
  protected override IEnumerable<TestAppSettings> GetTestAppSettings()
  {
     return new List<TestAppSettings>
     {
     };
  }

  protected override ValueTask DisposeAsyncCore()
  {
    return new ValueTask(Task.CompletedTask);
  }
}
