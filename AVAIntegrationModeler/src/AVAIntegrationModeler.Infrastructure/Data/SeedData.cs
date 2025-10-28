using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.AreaAggregate;
using AVAIntegrationModeler.Domain.ContributorAggregate;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.Domain.FeatureAggregate;
using AVAIntegrationModeler.Domain.IntegrationMapAggregate;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.Domain.ValueObjects;

namespace AVAIntegrationModeler.Infrastructure.Data;

public static class SeedData
{
  public static readonly Contributor Contributor1 = new("Ardalis");
  public static readonly Contributor Contributor2 = new("Snowfrog");

  public static Scenario Scenario1 { get; set; } = new(Guid.Parse("d19b810a-b0d9-4863-82cb-c0f3e71bf02d"));
  public static Feature InputFeatureToScenario1 { get; set; } = new(
    Guid.Parse("da7dafab-d17e-4f09-85d1-83d8f6fc4f4f"),
    "InputFeatureToScenario1"
  );

  public static Feature OutputFeatureToScenario1{ get; set; } = new(
    Guid.Parse("53e2999b-e028-41b1-930b-ac483cf7809e"),
    "OutputFeatureToScenario1"
  );


  public static Scenario Scenario2 { get; set; } = new(Guid.Parse("c7d23c5d-e30f-4001-bd75-c651ec24254d"));
  public static Feature InputFeatureToScenario2 { get; set; } = new(
    Guid.Parse("a2b3c4d5-e6f7-4a8b-9c0d-1e2f3a4b5c6d"),
    "InputFeatureToScenario2"
  );
  public static Feature OutputFeatureToScenario2 { get; set; } = new(
   Guid.Parse("b3c4d5e6-f7a8-4b9c-0d1e-2f3a4b5c6d7e"),
   "OutputFeatureToScenario2"
 );

  public static Scenario Scenario3 { get; set; } = new(Guid.Parse("b36e1803-a6e7-4841-8de6-859a6dee43bf"));

  public static Feature OutputFeatureToScenario3 { get; set; } = new(
   Guid.Parse("d5e6f7a8-b9c0-4d1e-2f3a-4b5c6d7e8f9a"),
   "OutputFeatureToScenario3"
 );

  public static Feature Feature1 { get; set; } = new(
    Guid.Parse("f1000000-0000-0000-0000-000000000001"), 
    "CUSTOMER_SYNC"
  );
  
  public static Feature Feature2 { get; set; } = new(
    Guid.Parse("f1000000-0000-0000-0000-000000000002"), 
    "ORDER_EXPORT"
  );

  // Areas
  public static Area Area1 { get; set; } = new(
    Guid.Parse("a1000000-0000-0000-0000-000000000001"),
    "SALES"
  );

  public static Area Area2 { get; set; } = new(
    Guid.Parse("a1000000-0000-0000-0000-000000000002"),
    "FINANCE"
  );

  public static Area Area3 { get; set; } = new(
    Guid.Parse("a1000000-0000-0000-0000-000000000003"),
    "LOGISTICS"
  );

  public static Area Area4 { get; set; } = new(
    Guid.Parse("a1000000-0000-0000-0000-000000000004"),
    "HR"
  );

  public static Area Area5 { get; set; } = new(
    Guid.Parse("a1000000-0000-0000-0000-000000000005"),
    "PRODUCTION"
  );

  // DataModels
  public static DataModel DataModel1 { get; set; } = new(
    Guid.Parse("d1000000-0000-0000-0000-000000000001"),
    "CUSTOMER"
  );

  public static DataModel DataModel2 { get; set; } = new(
    Guid.Parse("d1000000-0000-0000-0000-000000000002"),
    "ORDER"
  );

  public static DataModel DataModel3 { get; set; } = new(
    Guid.Parse("d1000000-0000-0000-0000-000000000003"),
    "INVOICE"
  );

  public static DataModel DataModel4 { get; set; } = new(
    Guid.Parse("d1000000-0000-0000-0000-000000000004"),
    "PRODUCT"
  );

  public static DataModel DataModel5 { get; set; } = new(
    Guid.Parse("d1000000-0000-0000-0000-000000000005"),
    "EMPLOYEE"
  );

  // Integrations Maps
  public static IntegrationsMap IntegrationMap1 { get; set; } = new(
    Guid.Parse("302fc93b-01bb-4fc3-ac36-11aa30977c8c"),
    Area1.Id // SALES area
  );

  public static IntegrationsMap IntegrationMap2 { get; set; } = new(
    Guid.Parse("191124fe-6def-48e5-90cc-d532e450c604"),
    Area2.Id // FINANCE area
  );

  static SeedData()
  {
    // Scenarios
    Scenario1.SetCode("scenario1Code")
     .SetName(new LocalizedValue() { CzechValue = "Organization přijímač", EnglishValue = "Organization consumer" })
     .SetDescription(new LocalizedValue() { CzechValue = "Organization přijímač popisek", EnglishValue = "Organization consumer description" })
     .SetInputFeature(InputFeatureToScenario1.Id)
     .SetOutputFeature(OutputFeatureToScenario1.Id);

    Scenario2.SetCode("scenario2Code")
      .SetName(new LocalizedValue() { CzechValue = "Faktury přijímač", EnglishValue = "Invoices consumer" })
      .SetDescription(new LocalizedValue() { CzechValue = "Faktury přijímač popisek", EnglishValue = "Invoice consumer description" })
      .SetInputFeature(InputFeatureToScenario2.Id)
      .SetOutputFeature(OutputFeatureToScenario2.Id);


    Scenario3.SetCode("scenario3Code")
      .SetName(new LocalizedValue() { CzechValue = "Osoby přijímač", EnglishValue = "Person consumer" })
      .SetDescription(new LocalizedValue() { CzechValue = "Osoby přijímač popisek", EnglishValue = "Person consumer description" })
      .SetInputFeature(null)
      .SetOutputFeature(OutputFeatureToScenario3.Id);

    // Features
    Feature1
      .SetName(new LocalizedValue() { CzechValue = "Synchronizace zákazníků", EnglishValue = "Customer Synchronization" })
      .SetDescription(new LocalizedValue() { CzechValue = "Synchronizace zákaznických dat mezi systémy", EnglishValue = "Customer data synchronization between systems" })
      .AddIncludedModel(DataModel1.Id, consumeOnly: false)
      .AddIncludedModel(DataModel2.Id, consumeOnly: true);

    Feature2
      .SetName(new LocalizedValue() { CzechValue = "Export objednávek", EnglishValue = "Order Export" })
      .SetDescription(new LocalizedValue() { CzechValue = "Export objednávek do externího systému", EnglishValue = "Export orders to external system" })
      .AddIncludedModel(DataModel2.Id, consumeOnly: false)
      .AddIncludedModel(DataModel3.Id, consumeOnly: false)
      .AddIncludedFeature(Feature1.Id, consumeOnly: true);

    // Areas
    Area1.SetName("Sales");
    Area2.SetName("Finance");
    Area3.SetName("Logistics");
    Area4.SetName("Human Resources");
    Area5.SetName("Production");

    // ============================================
    // DataModel 1: CUSTOMER (Zákazník)
    // ============================================
    DataModel1
      .SetName("Customer")
      .SetDescription("Zákaznický datový model pro evidenci klientů a obchodních partnerů")
      .SetNotes("Hlavní agregát pro správu zákaznických dat. Obsahuje kontaktní údaje, adresy a předvolby.")
      .MarkAsAggregateRoot()
      .SetArea(Area1.Id)
      // Základní identifikační údaje
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000001"), "CustomerId", DataModelFieldType.UniqueIdentifier)
        .SetLabel("Customer ID")
        .SetDescription("Unique identifier for the customer")
        .MarkAsPublishedForLookup())
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000002"), "CustomerNumber", DataModelFieldType.Text)
        .SetLabel("Customer Number")
        .SetDescription("Business customer number")
        .MarkAsPublishedForLookup())
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000003"), "CustomerName", DataModelFieldType.Text)
        .SetLabel("Customer Name")
        .SetDescription("Full name or company name of the customer")
        .MarkAsPublishedForLookup())
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000004"), "LegalName", DataModelFieldType.Text)
        .SetLabel("Legal Name")
        .SetDescription("Official legal name of the customer")
        .MarkAsNullable())
      // Kontaktní údaje
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000005"), "Email", DataModelFieldType.Text)
        .SetLabel("Email Address")
        .SetDescription("Primary email address"))
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000006"), "Phone", DataModelFieldType.Text)
        .SetLabel("Phone Number")
        .SetDescription("Primary phone number")
        .MarkAsNullable())
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000007"), "Mobile", DataModelFieldType.Text)
        .SetLabel("Mobile Number")
        .SetDescription("Mobile phone number")
        .MarkAsNullable())
      // Adresa
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000008"), "Street", DataModelFieldType.Text)
        .SetLabel("Street")
        .SetDescription("Street address")
        .MarkAsNullable())
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000009"), "City", DataModelFieldType.Text)
        .SetLabel("City")
        .SetDescription("City name")
        .MarkAsNullable())
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-00000000000a"), "PostalCode", DataModelFieldType.Text)
        .SetLabel("Postal Code")
        .SetDescription("ZIP/Postal code")
        .MarkAsNullable())
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-00000000000b"), "Country", DataModelFieldType.Text)
        .SetLabel("Country")
        .SetDescription("Country code or name")
        .MarkAsNullable())
      // Finanční údaje
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-00000000000c"), "CreditLimit", DataModelFieldType.CurrencyNumber)
        .SetLabel("Credit Limit")
        .SetDescription("Maximum credit limit for the customer")
        .MarkAsNullable())
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-00000000000d"), "PaymentTerms", DataModelFieldType.WholeNumber)
        .SetLabel("Payment Terms (Days)")
        .SetDescription("Number of days for payment terms")
        .MarkAsNullable())
      // Kategorizace
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-00000000000e"), "CustomerType", DataModelFieldType.SingleSelectOptionSet)
        .SetLabel("Customer Type")
        .SetDescription("Type of customer (B2B, B2C, etc.)"))
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-00000000000f"), "Industry", DataModelFieldType.SingleSelectOptionSet)
        .SetLabel("Industry")
        .SetDescription("Industry sector")
        .MarkAsNullable())
      // Stavy a příznaky
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000010"), "IsActive", DataModelFieldType.TwoOptions)
        .SetLabel("Is Active")
        .SetDescription("Whether the customer is active"))
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000011"), "IsVIP", DataModelFieldType.TwoOptions)
        .SetLabel("Is VIP")
        .SetDescription("VIP customer flag"))
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000012"), "AcceptsMarketing", DataModelFieldType.TwoOptions)
        .SetLabel("Accepts Marketing")
        .SetDescription("Marketing consent flag"))
      // Datumy
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000013"), "CreatedOn", DataModelFieldType.UtcDateTime)
        .SetLabel("Created On")
        .SetDescription("Date when customer was created"))
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000014"), "ModifiedOn", DataModelFieldType.UtcDateTime)
        .SetLabel("Modified On")
        .SetDescription("Last modification date")
        .MarkAsNullable())
      // Poznámky
      .AddField(new DataModelField(Guid.Parse("f0000001-0000-0000-0000-000000000015"), "Notes", DataModelFieldType.MultilineText)
        .SetLabel("Notes")
        .SetDescription("Additional notes about the customer")
        .MarkAsNullable());

    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    // ============================================
    // DataModel 2: ORDER (Objednávka)
    // ============================================
    DataModel2
      .SetName("Order")
      .SetDescription("Objednávkový datový model")
      .SetNotes("Obsahuje údaje o objednávkách")
      .MarkAsAggregateRoot()
      .SetArea(Area1.Id)
      .AddField(new DataModelField(Guid.Parse("f0000002-0000-0000-0000-000000000001"), "OrderNumber", DataModelFieldType.Text)
        .SetLabel("Order Number")
        .SetDescription("Unique order number")
        .MarkAsPublishedForLookup())
      .AddField(new DataModelField(Guid.Parse("f0000002-0000-0000-0000-000000000002"), "OrderDate", DataModelFieldType.UtcDateTime)
        .SetLabel("Order Date")
        .SetDescription("Date when order was created"))
      .AddField(new DataModelField(Guid.Parse("f0000002-0000-0000-0000-000000000003"), "TotalAmount", DataModelFieldType.CurrencyNumber)
        .SetLabel("Total Amount")
        .SetDescription("Total order amount"));

    DataModel3
      .SetName("Invoice")
      .SetDescription("Fakturační datový model")
      .SetNotes("Obsahuje fakturační údaje")
      .MarkAsAggregateRoot()
      .SetArea(Area2.Id)
      .AddField(new DataModelField(Guid.Parse("f0000003-0000-0000-0000-000000000001"), "InvoiceNumber", DataModelFieldType.Text)
        .SetLabel("Invoice Number")
        .SetDescription("Unique invoice number")
        .MarkAsPublishedForLookup())
      .AddField(new DataModelField(Guid.Parse("f0000003-0000-0000-0000-000000000002"), "InvoiceDate", DataModelFieldType.Date)
        .SetLabel("Invoice Date")
        .SetDescription("Invoice issue date"))
      .AddField(new DataModelField(Guid.Parse("f0000003-0000-0000-0000-000000000003"), "DueDate", DataModelFieldType.Date)
        .SetLabel("Due Date")
        .SetDescription("Payment due date"))
      .AddField(new DataModelField(Guid.Parse("f0000003-0000-0000-0000-000000000004"), "IsPaid", DataModelFieldType.TwoOptions)
        .SetLabel("Is Paid")
        .SetDescription("Payment status"));

    DataModel4
      .SetName("Product")
      .SetDescription("Produktový datový model")
      .SetNotes("Katalog produktů")
      .MarkAsAggregateRoot()
      .SetArea(Area5.Id)
      .AddField(new DataModelField(Guid.Parse("f0000004-0000-0000-0000-000000000001"), "ProductCode", DataModelFieldType.Text)
        .SetLabel("Product Code")
        .SetDescription("Unique product code")
        .MarkAsPublishedForLookup())
      .AddField(new DataModelField(Guid.Parse("f0000004-0000-0000-0000-000000000002"), "ProductName", DataModelFieldType.Text)
        .SetLabel("Product Name")
        .SetDescription("Name of the product"))
      .AddField(new DataModelField(Guid.Parse("f0000004-0000-0000-0000-000000000003"), "Price", DataModelFieldType.CurrencyNumber)
        .SetLabel("Price")
        .SetDescription("Product price"))
      .AddField(new DataModelField(Guid.Parse("f0000004-0000-0000-0000-000000000004"), "InStock", DataModelFieldType.WholeNumber)
        .SetLabel("In Stock")
        .SetDescription("Quantity in stock"));

    DataModel5
      .SetName("Employee")
      .SetDescription("Zaměstnanecký datový model")
      .SetNotes("Evidence zaměstnanců")
      .MarkAsAggregateRoot()
      .SetArea(Area4.Id)
      .AddField(new DataModelField(Guid.Parse("f0000005-0000-0000-0000-000000000001"), "EmployeeNumber", DataModelFieldType.Text)
        .SetLabel("Employee Number")
        .SetDescription("Unique employee number")
        .MarkAsPublishedForLookup())
      .AddField(new DataModelField(Guid.Parse("f0000005-0000-0000-0000-000000000002"), "FullName", DataModelFieldType.Text)
        .SetLabel("Full Name")
        .SetDescription("Employee full name"))
      .AddField(new DataModelField(Guid.Parse("f0000005-0000-0000-0000-000000000003"), "HireDate", DataModelFieldType.Date)
        .SetLabel("Hire Date")
        .SetDescription("Date of employment"))
      .AddField(new DataModelField(Guid.Parse("f0000005-0000-0000-0000-000000000004"), "IsActive", DataModelFieldType.TwoOptions)
        .SetLabel("Is Active")
        .SetDescription("Employment status"));

    // ============================================
    // Integration Maps
    // ============================================
    
    // Integration Map 1: SALES area
    var scenario1Item = IntegrationMap1.AddItem(Scenario1.Id);
    scenario1Item.AddKey("ORGANIZATION_CREATED");
    scenario1Item.AddKey("ORGANIZATION_UPDATED");

    var scenario2Item = IntegrationMap1.AddItem(Scenario2.Id);
    scenario2Item.AddKey("INVOICE_CREATED");
    scenario2Item.AddKey("INVOICE_UPDATED");

    // Integration Map 2: FINANCE area
    var scenario3Item = IntegrationMap2.AddItem(Scenario3.Id);
    scenario3Item.AddKey("PERSON_CREATED");
    scenario3Item.AddKey("PERSON_UPDATED");
    scenario3Item.AddKey("PERSON_DELETED");
  }

  public static async Task InitializeAsync(AppDbContext dbContext)
  {
    if (await dbContext.Scenarios.AnyAsync()) return;
    await PopulateTestDataAsync(dbContext);
  }

  public static async Task PopulateTestDataAsync(AppDbContext dbContext)
  {
    dbContext.Scenarios.AddRange([Scenario1, Scenario2, Scenario3]);
    dbContext.Contributors.AddRange([Contributor1, Contributor2]);
    dbContext.Features.AddRange([Feature1, Feature2, InputFeatureToScenario1, OutputFeatureToScenario1, InputFeatureToScenario2, OutputFeatureToScenario2, OutputFeatureToScenario3]);
    dbContext.Areas.AddRange([Area1, Area2, Area3, Area4, Area5]);
    dbContext.DataModels.AddRange([DataModel1, DataModel2, DataModel3, DataModel4, DataModel5]);
    
    // ✅ PŘIDAT INTEGRATION MAPS
    dbContext.IntegrationMaps.AddRange([IntegrationMap1, IntegrationMap2]);
    
    await dbContext.SaveChangesAsync();
  }
}
