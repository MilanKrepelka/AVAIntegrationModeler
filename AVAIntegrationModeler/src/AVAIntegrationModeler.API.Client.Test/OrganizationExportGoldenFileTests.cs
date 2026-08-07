using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;

namespace AVAIntegrationModeler.API.Client.Test;

/// <summary>
/// Golden-file test exportu DataModelu "Organization" — vytvoří DataModel se sadou polí
/// (včetně počítaného pole s Expression) a ověří, že definiční JSON v exportním ZIPu
/// strukturálně odpovídá referenčnímu souboru <c>TestData/OrganizationExport.expected.json</c>.
/// Pole "Id" je v referenčním souboru nahrazeno neutrální hodnotou, protože skutečné Id
/// DataModelu je při každém běhu testu jiné (Guid.NewGuid()).
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
    Description = "Organizace v rámci systému.",
    IsAggregateRoot = true,
    AreaId = null,
    Fields = new List<DataModelFieldDTO>
    {
      new DataModelFieldDTO
      {
        Name = "Code",
        Label = "Kód",
        Description = "Unikátní kód organizace.",
        FieldType = DataModelFieldType.Text,
        IsNullable = false,
        ReferencedEntityTypeIds = new List<Guid>()
      },
      new DataModelFieldDTO
      {
        Name = "FullDisplayName",
        Label = "Celý název pro zobrazení",
        Description = "Počítané pole složené z kódu a názvu.",
        FieldType = DataModelFieldType.Text,
        IsNullable = true,
        ReferencedEntityTypeIds = new List<Guid>(),
        Expression = new DataModelFieldExpressionDTO { Value = "Code + ' - ' + Name", Order = 1 }
      },
      new DataModelFieldDTO
      {
        Name = "IsActive",
        Label = "Aktivní",
        Description = "Příznak, zda je organizace aktivní.",
        FieldType = DataModelFieldType.TwoOptions,
        IsNullable = false,
        ReferencedEntityTypeIds = new List<Guid>()
      },
      new DataModelFieldDTO
      {
        Name = "Name",
        Label = "Název",
        Description = "Název organizace.",
        FieldType = DataModelFieldType.Text,
        IsLocalized = true,
        IsNullable = false,
        ReferencedEntityTypeIds = new List<Guid>()
      },
      new DataModelFieldDTO
      {
        Name = "ParentOrganizationId",
        Label = "Nadřazená organizace",
        Description = "Odkaz na nadřazenou organizaci.",
        FieldType = DataModelFieldType.LookupEntity,
        IsNullable = true,
        ReferencedEntityTypeIds = new List<Guid> { Guid.Parse("11111111-1111-1111-1111-111111111111") }
      },
      new DataModelFieldDTO
      {
        Name = "TaxId",
        Label = "IČO",
        Description = "Identifikační číslo organizace.",
        FieldType = DataModelFieldType.Text,
        IsNullable = true,
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
    actual!["Id"] = PlaceholderId.ToString();
    actual["Code"] = "ORG-PLACEHOLDER";

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
