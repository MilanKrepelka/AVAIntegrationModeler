using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.AreaAggregate;
using AVAIntegrationModeler.Domain.ContributorAggregate;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.Domain.DeploymentAggregate;
using AVAIntegrationModeler.Domain.FeatureAggregate;
using AVAIntegrationModeler.Domain.IntegrationMapAggregate;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.Domain.ValueObjects;

namespace AVAIntegrationModeler.Infrastructure.Data;

public static class SeedData
{
  // Areas
  public static Area AssetManagementArea { get; set; } = new(
    Guid.Parse("5703d985-0dd1-4031-a58e-41a459a3a493"),
    "AssetManagement"
  );

  public static Area BankArea { get; set; } = new(
    Guid.Parse("06fc269e-e9bf-4d62-ad96-1aa694370835"),
    "Bank"
  );

  public static Area BankAppArea { get; set; } = new(
    Guid.Parse("cfa99383-0039-4478-b8d8-c85378ed2096"),
    "BankApp"
  );

  public static Area CashRegisterArea { get; set; } = new(
    Guid.Parse("358abea3-db9d-474a-905f-eb32017b3582"),
    "CashRegister"
  );

  public static Area CommonArea { get; set; } = new(
    Guid.Parse("7d875e4b-0469-47b2-9749-1772aa8e7dad"),
    "Common"
  );

  public static Area CRMArea { get; set; } = new(
    Guid.Parse("34da0a62-6fff-42f3-80a0-75ef196933cc"),
    "CRM"
  );

  public static Area DocumentManagementSystemArea { get; set; } = new(
    Guid.Parse("7b55d0c1-b195-4b50-97a0-7ee37a0bed7f"),
    "DocumentManagementSystem"
  );

  public static Area ExamplesArea { get; set; } = new(
    Guid.Parse("fe285274-7c21-40e0-b7b6-2490196de26c"),
    "Examples"
  );

  public static Area HumanResourcesArea { get; set; } = new(
    Guid.Parse("da54fe04-d9a0-46c2-96b9-17681c243ff0"),
    "HumanResources"
  );

  public static Area InvoicingArea { get; set; } = new(
    Guid.Parse("7a7efc7a-9872-4217-930c-28b87c499805"),
    "Invoicing"
  );

  public static Area IsDocArea { get; set; } = new(
    Guid.Parse("81dd59f6-7a48-4d7c-933a-03b7dd1b1677"),
    "IsDoc"
  );

  public static Area JobOrderArea { get; set; } = new(
    Guid.Parse("054d3681-796f-4fcb-97d0-a9ab216d9ad5"),
    "JobOrder"
  );

  public static Area PeppolArea { get; set; } = new(
    Guid.Parse("5a0f3d4a-876f-40d2-aacd-0ade4bbb9eba"),
    "Peppol"
  );

  public static Area ProjectArea { get; set; } = new(
    Guid.Parse("96401932-42bf-4ec6-8640-1c22073f9a5f"),
    "Project"
  );

  public static Area TestsArea { get; set; } = new(
    Guid.Parse("32cb72d9-1f17-47f0-a7af-4544bb8f5f69"),
    "Tests"
  );

  public static Area VehicleArea { get; set; } = new(
    Guid.Parse("56ba1ca7-36e1-4d7b-a624-eaf218527d3a"),
    "Vehicle"
  );

  public static Area WarehouseArea { get; set; } = new(
    Guid.Parse("24624999-a975-4232-ba17-820d13efb39a"),
    "Warehouse"
  );

  static SeedData()
  {
    
  }
   
  public static async Task InitializeAsync(AppDbContext dbContext)
  {
    if (await dbContext.Areas.AnyAsync()) return;
    await PopulateTestDataAsync(dbContext);
  }

  public static async Task PopulateTestDataAsync(AppDbContext dbContext)
  {
    dbContext.Areas.AddRange([
      AssetManagementArea,
      BankArea,
      BankAppArea,
      CashRegisterArea,
      CommonArea,
      CRMArea,
      DocumentManagementSystemArea,
      ExamplesArea,
      HumanResourcesArea,
      InvoicingArea,
      IsDocArea,
      JobOrderArea,
      PeppolArea,
      ProjectArea,
      TestsArea,
      VehicleArea,
      WarehouseArea
    ]);

    await dbContext.SaveChangesAsync();
  }
}
