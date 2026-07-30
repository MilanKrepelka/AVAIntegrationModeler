using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.AreaAggregate;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.Domain.DataModelAggregate.Specifications;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Integration.Test.Data.SqlLite.Fixtures;
using Shouldly;

namespace AVAIntegrationModeler.Infrastructure.Test.Data;

/// <summary>
/// Testy databázové vrstvy ověřující, že volání SyncFieldsAndSaveAsync
/// (používané při UpdateDataModel) nezpůsobí DbUpdateConcurrencyException
/// při nastavení, změně nebo vymazání AreaId.
/// </summary>
[Collection("DataModelSyncTestCollection")]
public class DataModelSyncFieldsAndSaveTests : BaseDbTests
{
  private readonly DataModelRepository _repo;
  private readonly EfRepository<Area> _areaRepo;

  public DataModelSyncFieldsAndSaveTests(ITestOutputHelper testOutputHelper, EfSqlClientTestFixture fixture)
    : base(testOutputHelper, fixture)
  {
    _repo = new DataModelRepository(DbContext);
    _areaRepo = GetRepository<Area>();
  }

  // ── pomocné metody ───────────────────────────────────────────────────────────

  private async Task<Guid> SeedAreaAsync(string code)
  {
    var id = Guid.NewGuid();
    var area = new Area(id, code);
    area.SetName(code);
    await _areaRepo.AddAsync(area, CancellationToken.None);
    DbContext.ChangeTracker.Clear();
    return id;
  }

  private async Task<Guid> SeedDataModelAsync(string code, Guid? areaId = null, int fieldCount = 0)
  {
    var id = Guid.NewGuid();
    var model = new DataModel(id, code);
    model.SetName($"Test {code}");
    if (areaId.HasValue)
      model.SetArea(areaId.Value);
    for (int i = 0; i < fieldCount; i++)
      model.AddField(new DataModelField(Guid.NewGuid(), $"Field{i}", DataModelFieldType.Text));
    await _repo.AddAsync(model, CancellationToken.None);
    DbContext.ChangeTracker.Clear();
    return id;
  }

  /// <summary>
  /// Simuluje přesně tok UpdateDataModelHandler: načtení entity, modifikace,
  /// sestavení listů pro smazání/přidání, volání SyncFieldsAndSaveAsync.
  /// </summary>
  private async Task SimulateUpdateAsync(
    Guid modelId,
    Guid? newAreaId,
    IReadOnlyList<(string Name, DataModelFieldType Type)>? newFields = null)
  {
    var existing = await _repo.FirstOrDefaultAsync(
      new DataModelByIdWithFieldsSpec(modelId), CancellationToken.None);
    existing.ShouldNotBeNull();

    existing.SetArea(newAreaId);

    var fieldsToDelete = existing.Fields.ToList();
    foreach (var name in fieldsToDelete.Select(f => f.Name))
      existing.RemoveField(name);

    var fieldsToAdd = new List<DataModelField>();
    foreach (var (name, type) in newFields ?? [])
    {
      var f = new DataModelField(Guid.NewGuid(), name, type);
      existing.AddField(f);
      fieldsToAdd.Add(f);
    }

    await _repo.SyncFieldsAndSaveAsync(existing, fieldsToDelete, fieldsToAdd, CancellationToken.None);
    DbContext.ChangeTracker.Clear();
  }

  // Přenačte DataModel se všemi navigacemi přes IQueryable pipeline (AutoInclude).
  // GetByIdAsync používá FindAsync, které AutoInclude po ChangeTracker.Clear() ignoruje.
  private async Task<DataModel?> ReloadAsync(Guid modelId)
    => await _repo.FirstOrDefaultAsync(new DataModelByIdWithFieldsSpec(modelId), CancellationToken.None);

  // ── testy ────────────────────────────────────────────────────────────────────

  /// <summary>
  /// Hlavní regresní test: DataModel bez oblasti → přiřazení oblasti nesmí vyhodit výjimku
  /// a AreaId musí být uloženo.
  /// </summary>
  [Fact]
  public async Task SyncFieldsAndSave_SetArea_NullToValue_PersistsAreaId()
  {
    // Arrange
    var areaId = await SeedAreaAsync("AREA_SET_01");
    var modelId = await SeedDataModelAsync("DM_NULL_TO_VAL");

    // Act — nastaví AreaId z null na platnou hodnotu (přesný scénář bugu)
    await SimulateUpdateAsync(modelId, areaId);

    // Assert
    var updated = await ReloadAsync(modelId);
    updated.ShouldNotBeNull();
    updated.AreaId.ShouldBe(areaId);
  }

  /// <summary>
  /// Změna oblasti z jedné hodnoty na jinou nesmí vyhodit výjimku.
  /// </summary>
  [Fact]
  public async Task SyncFieldsAndSave_ChangeArea_PersistsNewAreaId()
  {
    // Arrange
    var area1Id = await SeedAreaAsync("AREA_CHG_A");
    var area2Id = await SeedAreaAsync("AREA_CHG_B");
    var modelId = await SeedDataModelAsync("DM_CHANGE_AREA", area1Id);

    // Act
    await SimulateUpdateAsync(modelId, area2Id);

    // Assert
    var updated = await ReloadAsync(modelId);
    updated.ShouldNotBeNull();
    updated.AreaId.ShouldBe(area2Id);
    updated.AreaId.ShouldNotBe(area1Id);
  }

  /// <summary>
  /// Vymazání oblasti (nastavení na null) musí projít bez výjimky.
  /// </summary>
  [Fact]
  public async Task SyncFieldsAndSave_ClearArea_PersistsNull()
  {
    // Arrange
    var areaId = await SeedAreaAsync("AREA_CLEAR_01");
    var modelId = await SeedDataModelAsync("DM_CLEAR_AREA", areaId);

    // Act
    await SimulateUpdateAsync(modelId, null);

    // Assert
    var updated = await ReloadAsync(modelId);
    updated.ShouldNotBeNull();
    updated.AreaId.ShouldBeNull();
  }

  /// <summary>
  /// Zároveň s nastavením oblasti se nahradí všechna stávající pole novými —
  /// oba typy změn (UPDATE DataModel + DELETE+INSERT fields) musí projít atomicky.
  /// </summary>
  [Fact]
  public async Task SyncFieldsAndSave_SetAreaAndReplaceFields_PersistsAllChanges()
  {
    // Arrange — 2 původní pole
    var areaId = await SeedAreaAsync("AREA_FIELDS_01");
    var modelId = await SeedDataModelAsync("DM_AREA_FIELDS", fieldCount: 2);

    // Act — nastaví oblast + nahradí pole třemi novými
    await SimulateUpdateAsync(modelId, areaId, new[]
    {
      ("NewFieldA", DataModelFieldType.Text),
      ("NewFieldB", DataModelFieldType.WholeNumber),
      ("NewFieldC", DataModelFieldType.Date),
    });

    // Assert
    var updated = await ReloadAsync(modelId);
    updated.ShouldNotBeNull();
    updated.AreaId.ShouldBe(areaId);
    updated.Fields.Count.ShouldBe(3);
    updated.Fields.ShouldContain(f => f.Name == "NewFieldA");
    updated.Fields.ShouldContain(f => f.Name == "NewFieldB");
    updated.Fields.ShouldContain(f => f.Name == "NewFieldC");
  }

  /// <summary>
  /// DataModel bez polí (prázdné fieldsToDelete i fieldsToAdd) — update pouze oblasti.
  /// </summary>
  [Fact]
  public async Task SyncFieldsAndSave_NoFields_SetArea_PersistsAreaId()
  {
    // Arrange
    var areaId = await SeedAreaAsync("AREA_NOFIELD_01");
    var modelId = await SeedDataModelAsync("DM_NO_FIELDS");

    // Act — model nemá žádná pole
    await SimulateUpdateAsync(modelId, areaId);

    // Assert
    var updated = await ReloadAsync(modelId);
    updated.ShouldNotBeNull();
    updated.AreaId.ShouldBe(areaId);
    updated.Fields.ShouldBeEmpty();
  }

  /// <summary>
  /// Pokud oblast i pole zůstávají beze změny (jiné skaláry se mění),
  /// SyncFieldsAndSave musí projít bez výjimky.
  /// </summary>
  [Fact]
  public async Task SyncFieldsAndSave_KeepExistingArea_ScalarsChange_PersistsAll()
  {
    // Arrange
    var areaId = await SeedAreaAsync("AREA_KEEP_01");
    var modelId = await SeedDataModelAsync("DM_KEEP_AREA", areaId, fieldCount: 1);

    // Act — oblast se nemění (stejná hodnota), ale pole se obnoví
    var existing = await _repo.FirstOrDefaultAsync(
      new DataModelByIdWithFieldsSpec(modelId), CancellationToken.None);
    existing.ShouldNotBeNull();
    existing.SetArea(areaId); // stejná hodnota jako uložená
    existing.SetName("Aktualizovaný název");

    var fieldsToDelete = existing.Fields.ToList();
    foreach (var name in fieldsToDelete.Select(f => f.Name))
      existing.RemoveField(name);

    var newField = new DataModelField(Guid.NewGuid(), "KeptField", DataModelFieldType.Text);
    existing.AddField(newField);

    await _repo.SyncFieldsAndSaveAsync(existing, fieldsToDelete, [newField], CancellationToken.None);
    DbContext.ChangeTracker.Clear();

    // Assert — přenačtení přes spec zajistí AutoInclude
    var updated = await ReloadAsync(modelId);
    updated.ShouldNotBeNull();
    updated.AreaId.ShouldBe(areaId);
    updated.Name.ShouldBe("Aktualizovaný název");
    updated.Fields.Count.ShouldBe(1);
    updated.Fields.First().Name.ShouldBe("KeptField");
  }
}
