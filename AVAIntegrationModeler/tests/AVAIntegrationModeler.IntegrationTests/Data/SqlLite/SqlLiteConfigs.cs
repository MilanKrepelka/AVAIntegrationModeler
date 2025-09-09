using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Infrastructure.Data;

namespace AVAIntegrationModeler.IntegrationTests.Data.SqlLite;
public static class SqlLiteConfigs
{
  public static IServiceCollection AddSqlLiteDatabaseForTesting(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddDbContext<AppDbContext>(options =>
      options.UseSqlite($"Data Source={Guid.NewGuid().ToString()}.sqlite"));
    return serviceCollection;
  }
  
}
