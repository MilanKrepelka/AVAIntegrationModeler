using AVAIntegrationModeler.Integration.Test.Data.SqlLite.Fixtures;
using Xunit;

namespace AVAIntegrationModeler.Infrastructure.Test.Data;

/// <summary>
/// Test collection pro AreaTests - izolovaná databáze.
/// </summary>
[CollectionDefinition("AreaTestCollection")]
public class AreaTestCollection : ICollectionFixture<EfSqlClientTestFixture>
{
    // Tato třída se nikdy neinstancuje.
    // Slouží pouze pro registraci collection fixture.
}

/// <summary>
/// Test collection pro DataModelTests - izolovaná databáze.
/// </summary>
[CollectionDefinition("DataModelTestCollection")]
public class DataModelTestCollection : ICollectionFixture<EfSqlClientTestFixture>
{
    // Tato třída se nikdy neinstancuje.
    // Slouží pouze pro registraci collection fixture.
}

/// <summary>
/// Test collection pro FeatureTests - izolovaná databáze.
/// </summary>
[CollectionDefinition("FeatureTestCollection")]
public class FeatureTestCollection : ICollectionFixture<EfSqlClientTestFixture>
{
    // Tato třída se nikdy neinstancuje.
    // Slouží pouze pro registraci collection fixture.
}

/// <summary>
/// Test collection pro FeatureTests - izolovaná databáze.
/// </summary>
[CollectionDefinition("ScenarioTestCollection")]
public class ScenarioTestCollection : ICollectionFixture<EfSqlClientTestFixture>
{
  // Tato třída se nikdy neinstancuje.
  // Slouží pouze pro registraci collection fixture.
}

/// <summary>
/// Test collection pro IntegrationMapTests - izolovaná databáze.
/// </summary>
[CollectionDefinition("IntegrationMapTestCollection")]
public class IntegrationMapTestCollection : ICollectionFixture<EfSqlClientTestFixture>
{
  // Tato třída se nikdy neinstancuje.
  // Slouží pouze pro registraci collection fixture.
}


