using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AVAIntegrationModeler.Integration.Test.Data.Data.SqlLite;

/// <summary>
/// Konfigurace SQLite pro testování.
/// </summary>
public static class SqlLiteConfigs
{
  /// <summary>
  /// Přidá SQLite databázi pro testování s unikátním jménem pro každý test.
  /// </summary>
  public static IServiceCollection AddSqlLiteDatabaseForTesting(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddDbContext<AppDbContext>(options =>
      options.UseSqlite($"Data Source={Guid.NewGuid()}.sqlite"));

    return serviceCollection;
  }
}
