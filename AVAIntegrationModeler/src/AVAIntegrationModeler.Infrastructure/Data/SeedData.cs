using AVAIntegrationModeler.Domain.ContributorAggregate;
using AVAIntegrationModeler.Domain.FeatureAggregate;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.Domain.ValueObjects;

namespace AVAIntegrationModeler.Infrastructure.Data;

public static class SeedData
{
  public static readonly Contributor Contributor1 = new("Ardalis");
  public static readonly Contributor Contributor2 = new("Snowfrog");

  public static Scenario Scenario1 { get; set; } = new(Guid.Parse("d19b810a-b0d9-4863-82cb-c0f3e71bf02d"));
  public static Scenario Scenario2 { get; set; } = new(Guid.Parse("c7d23c5d-e30f-4001-bd75-c651ec24254d"));
  public static Scenario Scenario3 { get; set; } = new(Guid.Parse("b36e1803-a6e7-4841-8de6-859a6dee43bf"));

  public static Feature Feature1 { get; set; } = new(
    Guid.Parse("f1000000-0000-0000-0000-000000000001"), 
    "CUSTOMER_SYNC"
  );
  
  public static Feature Feature2 { get; set; } = new(
    Guid.Parse("f1000000-0000-0000-0000-000000000002"), 
    "ORDER_EXPORT"
  );

  static SeedData()
  {
    // Scenarios
    Scenario1.SetCode("scenario1Code")
     .SetName(new LocalizedValue() { CzechValue = "Organization přijímač", EnglishValue = "Organization consumer" })
     .SetDescription(new LocalizedValue() { CzechValue = "Organization přijímač popisek", EnglishValue = "Organization consumer description" })
     .SetInputFeature(null)
     .SetOutputFeature(null);

    Scenario2.SetCode("scenario2Code")
      .SetName(new LocalizedValue() { CzechValue = "Faktury přijímač", EnglishValue = "Invoices consumer" })
      .SetDescription(new LocalizedValue() { CzechValue = "Faktury přijímač popisek", EnglishValue = "Invoice consumer description" })
      .SetInputFeature(null)
      .SetOutputFeature(Guid.Parse("b36e1803-a6e7-4841-8de6-859a6dee43bf"));

    Scenario3.SetCode("scenario3Code")
      .SetName(new LocalizedValue() { CzechValue = "Osoby přijímač", EnglishValue = "Person consumer" })
      .SetDescription(new LocalizedValue() { CzechValue = "Osoby přijímač popisek", EnglishValue = "Person consumer description" })
      .SetInputFeature(null)
      .SetOutputFeature(Guid.Parse("b36e1803-a6e7-4841-8de6-859a6dee43bf"));
  }

  public static async Task InitializeAsync(AppDbContext dbContext)
  {
    if (await dbContext.Contributors.AnyAsync()) return;
    await PopulateTestDataAsync(dbContext);
  }

  public static async Task PopulateTestDataAsync(AppDbContext dbContext)
  {
    dbContext.Scenarios.AddRange([Scenario1, Scenario2, Scenario3]);
    dbContext.Contributors.AddRange([Contributor1, Contributor2]);
    dbContext.Features.AddRange([Feature1, Feature2]); // ✅ PŘIDÁNO
    
    await dbContext.SaveChangesAsync();
  }
}
