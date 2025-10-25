using AVAIntegrationModeler.Core.ContributorAggregate;
using AVAIntegrationModeler.Core.ScenarioAggregate;
using Org.BouncyCastle.Bcpg.Sig;

namespace AVAIntegrationModeler.Infrastructure.Data;

public static class SeedData
{
  public static readonly Contributor Contributor1 = new("Ardalis");
  public static readonly Contributor Contributor2 = new("Snowfrog");

  public static  Scenario Scenario1 { get; set; } = new(Guid.Parse("d19b810a-b0d9-4863-82cb-c0f3e71bf02d"));
     
  public static  Scenario Scenario2 { get; set; } = new(Guid.Parse("c7d23c5d-e30f-4001-bd75-c651ec24254d"));

  public static Scenario Scenario3 { get; set; } = new(Guid.Parse("b36e1803-a6e7-4841-8de6-859a6dee43bf"));

  static SeedData()
  {
    Scenario1.SetCode("scenario1Code")
     .SetName(new Core.ValueObjects.LocalizedValue() { CzechValue = "Organization přijímač", EnglishValue = "Organization consumer" })
     .SetDescription(new Core.ValueObjects.LocalizedValue() { CzechValue = "Organization přijímač popisek", EnglishValue = "Organization consumer description" })
     .SetInputFeature(null)
     .SetOutputFeature(null);

    Scenario2.SetCode("scenario2Code")
      .SetName(new Core.ValueObjects.LocalizedValue() { CzechValue = "Faktury přijímač", EnglishValue = "Invoices consumer" })
      .SetDescription(new Core.ValueObjects.LocalizedValue() { CzechValue = "Faktury přijímač popisek", EnglishValue = "Invoice consumer description" })
      .SetInputFeature(null)
      .SetOutputFeature(Guid.Parse("b36e1803-a6e7-4841-8de6-859a6dee43bf")
      );

    Scenario3.SetCode("scenario3Code")
      .SetName(new Core.ValueObjects.LocalizedValue() { CzechValue = "Osoby přijímač", EnglishValue = "Person consumer" })
      .SetDescription(new Core.ValueObjects.LocalizedValue() { CzechValue = "Osoby přijímač popisek", EnglishValue = "Person consumer description" })
      .SetInputFeature(null)
      .SetOutputFeature(Guid.Parse("b36e1803-a6e7-4841-8de6-859a6dee43bf")
      );
  }

  public static async Task InitializeAsync(AppDbContext dbContext)
  {
    if (await dbContext.Contributors.AnyAsync()) return; // DB has been seeded
    await PopulateTestDataAsync(dbContext);
  }

  public static async Task PopulateTestDataAsync(AppDbContext dbContext)
  {
    dbContext.Scenarios.AddRange([Scenario1, Scenario2, Scenario3]);
    dbContext.Contributors.AddRange([Contributor1, Contributor2]);
    
    await dbContext.SaveChangesAsync();
  }
}
