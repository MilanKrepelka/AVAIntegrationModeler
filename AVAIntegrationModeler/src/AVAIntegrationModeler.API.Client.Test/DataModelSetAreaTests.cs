using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;

namespace AVAIntegrationModeler.API.Client.Test;

/// <summary>
/// Integrační testy API vrstvy ověřující, že nastavení, změna nebo vymazání AreaId
/// na DataModel via UpdateDataModel endpoint nevrátí HTTP 500 (DbUpdateConcurrencyException).
/// </summary>
public class DataModelSetAreaTests : IClassFixture<AVAIntegrationModelerAPIFactory>
{
  private readonly AVAIntegrationModelerAPIFactory _factory;

  public DataModelSetAreaTests(AVAIntegrationModelerAPIFactory factory)
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

  private static DataModelDTO NewDataModel(string? code = null) => new DataModelDTO
  {
    Id = Guid.NewGuid(),
    Code = code ?? $"AREA-TEST-{Guid.NewGuid().ToString()[..8].ToUpper()}",
    Name = "Test model pro AreaId",
    Description = "Regresní test pro DbUpdateConcurrencyException",
    IsAggregateRoot = false,
    Fields = []
  };

  private async Task<Guid> CreateAreaAsync(IAVAIntegrationModelerApiClient client)
  {
    var area = new AreaDTO
    {
      Id = Guid.NewGuid(),
      Code = $"AR-{Guid.NewGuid().ToString()[..8].ToUpper()}",
      Name = "Test Oblast"
    };
    var result = await client.CreateArea(Datasource.Database, area, CancellationToken.None);
    Assert.True(result.IsSuccess, $"Nepodařilo se vytvořit oblast: {string.Join(", ", result.Errors)}");
    return area.Id;
  }

  /// <summary>
  /// Hlavní regresní test: vytvoří DataModel bez oblasti, pak ho aktualizuje s oblastí.
  /// Dříve způsobovalo DbUpdateConcurrencyException (HTTP 500).
  /// </summary>
  [Fact]
  public async Task UpdateDataModel_SetArea_NullToValue_ReturnsSuccess()
  {
    var client = CreateClient();
    var dm = NewDataModel();

    // Vytvoření DataModel bez oblasti
    var createResult = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);
    Assert.True(createResult.IsSuccess);
    var id = createResult.Value;

    var areaId = await CreateAreaAsync(client);

    // Aktualizace — nastavení oblasti (null → hodnota)
    var updateResult = await client.UpdateDataModel(
      Datasource.Database,
      dm with { Id = id, AreaId = areaId },
      CancellationToken.None);

    Assert.True(updateResult.IsSuccess,
      $"UpdateDataModel selhal: {string.Join(", ", updateResult.Errors)}");

    // Ověření, že AreaId bylo uloženo
    var fetched = await client.GetDataModel(Datasource.Database, id, CancellationToken.None);
    Assert.NotNull(fetched);
    Assert.Equal(areaId, fetched.AreaId);
  }

  /// <summary>
  /// Změna oblasti z jedné hodnoty na jinou nesmí způsobit výjimku.
  /// </summary>
  [Fact]
  public async Task UpdateDataModel_ChangeArea_ReturnsSuccess()
  {
    var client = CreateClient();
    var area1Id = await CreateAreaAsync(client);
    var area2Id = await CreateAreaAsync(client);

    var dm = NewDataModel() with { AreaId = area1Id };
    var createResult = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);
    Assert.True(createResult.IsSuccess);
    var id = createResult.Value;

    // Aktualizace — změna oblasti (hodnota → jiná hodnota)
    var updateResult = await client.UpdateDataModel(
      Datasource.Database,
      dm with { Id = id, AreaId = area2Id },
      CancellationToken.None);

    Assert.True(updateResult.IsSuccess,
      $"UpdateDataModel selhal: {string.Join(", ", updateResult.Errors)}");

    var fetched = await client.GetDataModel(Datasource.Database, id, CancellationToken.None);
    Assert.Equal(area2Id, fetched.AreaId);
  }

  /// <summary>
  /// Vymazání oblasti (nastavení na null) musí projít bez HTTP 500.
  /// </summary>
  [Fact]
  public async Task UpdateDataModel_ClearArea_ReturnsSuccess()
  {
    var client = CreateClient();
    var areaId = await CreateAreaAsync(client);

    var dm = NewDataModel() with { AreaId = areaId };
    var createResult = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);
    Assert.True(createResult.IsSuccess);
    var id = createResult.Value;

    // Aktualizace — vymazání oblasti (hodnota → null)
    var updateResult = await client.UpdateDataModel(
      Datasource.Database,
      dm with { Id = id, AreaId = null },
      CancellationToken.None);

    Assert.True(updateResult.IsSuccess,
      $"UpdateDataModel selhal: {string.Join(", ", updateResult.Errors)}");

    var fetched = await client.GetDataModel(Datasource.Database, id, CancellationToken.None);
    Assert.Null(fetched.AreaId);
  }

  /// <summary>
  /// Nastavení oblasti při současné náhradě všech polí — kombinovaný scénář,
  /// který zahrnuje ExecuteDeleteAsync (stará pole) + INSERT (nová pole) + ExecuteUpdateAsync (DataModel).
  /// Pole jsou ověřena z odpovědi UpdateDataModel (GetDataModel nenačítá pole — používá FindAsync).
  /// </summary>
  [Fact]
  public async Task UpdateDataModel_SetAreaAndReplaceFields_ReturnsSuccess()
  {
    var client = CreateClient();
    var areaId = await CreateAreaAsync(client);

    var dm = NewDataModel() with
    {
      Fields =
      [
        new DataModelFieldDTO { Name = "PuvodniPole", FieldType = DataModelFieldType.Text }
      ]
    };
    var createResult = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);
    Assert.True(createResult.IsSuccess);
    var id = createResult.Value;

    // Aktualizace — nastaví oblast + nahradí stávající pole dvěma novými
    var updateResult = await client.UpdateDataModel(
      Datasource.Database,
      dm with
      {
        Id = id,
        AreaId = areaId,
        Fields =
        [
          new DataModelFieldDTO { Name = "NovePole1", FieldType = DataModelFieldType.Text },
          new DataModelFieldDTO { Name = "NovePole2", FieldType = DataModelFieldType.WholeNumber },
        ]
      },
      CancellationToken.None);

    Assert.True(updateResult.IsSuccess,
      $"UpdateDataModel selhal: {string.Join(", ", updateResult.Errors)}");

    // UpdateDataModel vrací DTO z in-memory entity (nezávisle na GetDataModel, který neloaduje pole)
    Assert.Equal(areaId, updateResult.Value.AreaId);
    Assert.Equal(2, updateResult.Value.Fields.Count);
    Assert.Contains(updateResult.Value.Fields, f => f.Name == "NovePole1");
    Assert.Contains(updateResult.Value.Fields, f => f.Name == "NovePole2");

    // AreaId je ověřeno i přes GetDataModel (skalární vlastnost je vždy načtena)
    var fetched = await client.GetDataModel(Datasource.Database, id, CancellationToken.None);
    Assert.Equal(areaId, fetched.AreaId);
  }

  /// <summary>
  /// Vícenásobné aktualizace oblasti po sobě (přezkouší transakční izolaci).
  /// </summary>
  [Fact]
  public async Task UpdateDataModel_MultipleAreaUpdates_AllSucceed()
  {
    var client = CreateClient();
    var area1Id = await CreateAreaAsync(client);
    var area2Id = await CreateAreaAsync(client);
    var area3Id = await CreateAreaAsync(client);

    var dm = NewDataModel();
    var createResult = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);
    Assert.True(createResult.IsSuccess);
    var id = createResult.Value;

    // Tři po sobě jdoucí aktualizace oblasti
    foreach (var (areaId, label) in new[] { (area1Id, "první"), (area2Id, "druhé"), (area3Id, "třetí") })
    {
      var result = await client.UpdateDataModel(
        Datasource.Database,
        dm with { Id = id, AreaId = areaId },
        CancellationToken.None);
      Assert.True(result.IsSuccess, $"UpdateDataModel selhal při {label} aktualizaci");
    }

    var fetched = await client.GetDataModel(Datasource.Database, id, CancellationToken.None);
    Assert.Equal(area3Id, fetched.AreaId);
  }
}
