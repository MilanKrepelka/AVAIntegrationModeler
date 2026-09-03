using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;

namespace AVAIntegrationModeler.API.Client.Test;

/// <summary>
/// Golden-file test exportu DataModelu "Organization" — vytvoří DataModel se stejnou sadou
/// polí, jakou má reálný model "Organization" v <c>DataModelsFromAVA.json</c>
/// (Addresses, BankAccounts, Code, Contacts, CountryCode, DateOfFoundation, DateOfTermination,
/// IdentificationNumber, LegalForm, Name, TaxId, VatIn), a ověří, že definiční JSON v exportním
/// ZIPu strukturálně odpovídá referenčnímu souboru <c>TestData/OrganizationExport.expected.json</c>.
/// Pole "Id" a "Code" jsou v referenčním souboru nahrazena neutrální hodnotou, protože se
/// při každém běhu testu liší (Guid.NewGuid()).
/// </summary>
public class OrganizationExportGoldenFileTests : IClassFixture<AVAIntegrationModelerAPIFactory>
{
  private static readonly Guid PlaceholderId = Guid.Empty;

  private readonly AVAIntegrationModelerAPIFactory _factory;

  public OrganizationExportGoldenFileTests(AVAIntegrationModelerAPIFactory factory)
  {
    _factory = factory;
  }

  private IAVAIntegrationModelerApiClient CreateClient()
  {
    var http = _factory.CreateClient(new WebApplicationFactoryClientOptions
    {
      BaseAddress = new Uri("http://0.0.0.0:5005")
    });
    return new AVAIntegrationModelerApiClient(http, new TestHttpClientFactory(http), NullLogger<AVAIntegrationModelerApiClient>.Instance);
  }

  private static DataModelDTO BuildOrganizationDataModel(Guid id, string code) => new()
  {
    Id = id,
    Code = code,
    Name = "Organization",
    Description = "public information about organization, its addresses, contacts and bank accounts",
    IsAggregateRoot = true,
    AreaId = null,
    Fields = new List<DataModelFieldDTO>
    {
      new DataModelFieldDTO
      {
        Name = "Addresses", Label = "Addresses", Description = "company addresses",
        FieldType = DataModelFieldType.NestedEntity, IsCollection = true, IsNullable = true,
        ReferencedEntityTypeIds = new List<Guid> { Guid.Parse("4b3e7a23-f83e-432c-8b03-2b8054169106") }
      },
      new DataModelFieldDTO
      {
        Name = "BankAccounts", Label = "BankAccounts", Description = "company bank accounts",
        FieldType = DataModelFieldType.NestedEntity, IsCollection = true, IsNullable = true,
        ReferencedEntityTypeIds = new List<Guid> { Guid.Parse("d77c9e98-c027-4c08-836d-132589babfbe") }
      },
      new DataModelFieldDTO
      {
        Name = "Code", Label = "Code",
        Description = "company identifier combined from IdentificationNumber and CountryCode",
        FieldType = DataModelFieldType.Text, IsPublishedForLookup = true, IsNullable = false,
        ReferencedEntityTypeIds = new List<Guid>()
      },
      new DataModelFieldDTO
      {
        Name = "Contacts", Label = "Contacts", Description = "company contacts",
        FieldType = DataModelFieldType.NestedEntity, IsCollection = true, IsNullable = true,
        ReferencedEntityTypeIds = new List<Guid> { Guid.Parse("ef0acd96-3e88-4fc6-a331-4f0f2c571395") }
      },
      new DataModelFieldDTO
      {
        Name = "CountryCode", Label = "CountryCode",
        Description = "country code which issued national identification number (ISO 3166-1 alpha-2 format)",
        FieldType = DataModelFieldType.Text, IsPublishedForLookup = true, IsNullable = false,
        ReferencedEntityTypeIds = new List<Guid>()
      },
      new DataModelFieldDTO
      {
        Name = "DateOfFoundation", Label = "DateOfFoundation", Description = "date of foundation",
        FieldType = DataModelFieldType.Date, IsNullable = true,
        ReferencedEntityTypeIds = new List<Guid>()
      },
      new DataModelFieldDTO
      {
        Name = "DateOfTermination", Label = "DateOfTermination", Description = "date of termination",
        FieldType = DataModelFieldType.Date, IsNullable = true,
        ReferencedEntityTypeIds = new List<Guid>()
      },
      new DataModelFieldDTO
      {
        Name = "IdentificationNumber", Label = "IdentificationNumber",
        Description = "national identification number of company issued by national authority (notes: IC in CZ)",
        FieldType = DataModelFieldType.Text, IsPublishedForLookup = true, IsNullable = false,
        ReferencedEntityTypeIds = new List<Guid>()
      },
      new DataModelFieldDTO
      {
        Name = "LegalForm", Label = "LegalForm", Description = "legal form of company",
        FieldType = DataModelFieldType.LookupEntity, IsNullable = true,
        ReferencedEntityTypeIds = new List<Guid> { Guid.Parse("6a763172-3b82-4074-8ee6-0bfa86f15e50") }
      },
      new DataModelFieldDTO
      {
        Name = "Name", Label = "Name", Description = "company name",
        FieldType = DataModelFieldType.Text, IsPublishedForLookup = true, IsNullable = true,
        ReferencedEntityTypeIds = new List<Guid>()
      },
      new DataModelFieldDTO
      {
        Name = "TaxId", Label = "TaxId", Description = "tax identifier (notes: DIC in CZ)",
        FieldType = DataModelFieldType.Text, IsNullable = true,
        ReferencedEntityTypeIds = new List<Guid>()
      },
      new DataModelFieldDTO
      {
        Name = "VatIn", Label = "VatIn", Description = "vat identification number (notes: IC DPH in SK)",
        FieldType = DataModelFieldType.Text, IsNullable = true,
        ReferencedEntityTypeIds = new List<Guid>()
      }
    }
  };

  /// <summary>
  /// Exportuje čerstvě vytvořený DataModel "Organization" a porovná definiční JSON
  /// z ZIP archívu s referenčním (golden) souborem — kromě Id, které je při každém běhu jiné.
  /// </summary>
  [Fact]
  public async Task ExportDataModels_Organization_MatchesGoldenFile()
  {
    // Arrange
    var client = CreateClient();
    var modelId = Guid.NewGuid();
    var code = $"ORG-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var dto = BuildOrganizationDataModel(modelId, code);

    // CreateDataModel vytvoří model bez polí (pole se ukládají teprve přes Update — stejný vzor jako DataModelEdit.razor.cs).
    var createDto = dto with { Fields = new List<DataModelFieldDTO>() };
    var createResult = await client.CreateDataModel(Datasource.Database, createDto, CancellationToken.None);
    Assert.True(createResult.IsSuccess, $"Nepodařilo se vytvořit DataModel: {string.Join(", ", createResult.Errors)}");

    var updateResult = await client.UpdateDataModel(Datasource.Database, dto, CancellationToken.None);
    Assert.True(updateResult.IsSuccess, $"Nepodařilo se uložit pole DataModelu: {string.Join(", ", updateResult.Errors)}");

    // Act
    var bytes = await client.ExportDataModels(Datasource.Database, [modelId], CancellationToken.None);

    using var ms = new MemoryStream(bytes);
    using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
    var entry = Assert.Single(zip.Entries, e => e.FullName == "datamodels/bez-oblasti/dm-Organization.json");

    JsonNode? actual;
    using (var entryStream = entry.Open())
    {
      actual = await JsonNode.ParseAsync(entryStream);
    }
    Assert.NotNull(actual);
    actual!["id"] = PlaceholderId.ToString();
    actual["code"] = "ORG-PLACEHOLDER";

    var expectedPath = Path.Combine(AppContext.BaseDirectory, "TestData", "OrganizationExport.expected.json");
    var expectedJson = await File.ReadAllTextAsync(expectedPath);
    var expected = JsonNode.Parse(expectedJson);
    Assert.NotNull(expected);

    // Assert
    var matches = JsonNode.DeepEquals(actual, expected);
    Assert.True(matches,
      $"Exportovaný JSON neodpovídá referenčnímu souboru.\nOčekáváno:\n{expected!.ToJsonString(new JsonSerializerOptions { WriteIndented = true })}\n\nSkutečnost:\n{actual.ToJsonString(new JsonSerializerOptions { WriteIndented = true })}");
  }
}
