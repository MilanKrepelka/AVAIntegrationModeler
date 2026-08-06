using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Integration.Test.Data.SqlLite.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace AVAIntegrationModeler.Infrastructure.Test.Data;

/// <summary>
/// Integrační testy pro DeleteAllAsync na DataModelRepository a DataModelRecordRepository —
/// hromadné smazání všech datových modelů a jejich záznamů z databáze.
/// </summary>
[Collection("DataModelDeleteAllTestCollection")]
public class DataModelDeleteAllTests : BaseDbTests
{
  private readonly DataModelRepository _dataModelRepo;
  private readonly DataModelRecordRepository _recordRepo;

  public DataModelDeleteAllTests(ITestOutputHelper testOutputHelper, EfSqlClientTestFixture fixture)
    : base(testOutputHelper, fixture)
  {
    _dataModelRepo = new DataModelRepository(DbContext);
    _recordRepo = new DataModelRecordRepository(DbContext);
  }

  private async Task<Guid> SeedDataModelAsync(string code, int fieldCount = 0)
  {
    var id = Guid.NewGuid();
    var model = new DataModel(id, code);
    model.SetName($"Test {code}");
    for (int i = 0; i < fieldCount; i++)
      model.AddField(new DataModelField(Guid.NewGuid(), $"Field{i}", DataModelFieldType.Text));
    await _dataModelRepo.AddAsync(model, CancellationToken.None);
    DbContext.ChangeTracker.Clear();
    return id;
  }

  private async Task<Guid> SeedRecordAsync(Guid modelId, string externalId, int fieldCount = 0)
  {
    var id = Guid.NewGuid();
    var record = new DataModelRecord(id, modelId);
    record.SetExternalId(externalId);
    for (int i = 0; i < fieldCount; i++)
    {
      var field = new DataModelRecordField(Guid.NewGuid(), $"Key{i}");
      field.SetStringValue($"Value{i}");
      record.AddField(field);
    }
    await _recordRepo.AddAsync(record, CancellationToken.None);
    DbContext.ChangeTracker.Clear();
    return id;
  }

  [Fact]
  public async Task DataModelRepository_DeleteAllAsync_SmazeVsechnyModelyIJejichPole()
  {
    // Arrange
    await SeedDataModelAsync("MODEL-A", fieldCount: 2);
    await SeedDataModelAsync("MODEL-B", fieldCount: 3);
    await SeedDataModelAsync("MODEL-C");

    // Act
    var deletedCount = await _dataModelRepo.DeleteAllAsync(CancellationToken.None);

    // Assert
    deletedCount.ShouldBe(3);
    (await _dataModelRepo.ListAsync(CancellationToken.None)).ShouldBeEmpty();
    (await DbContext.Set<DataModelField>().ToListAsync(CancellationToken.None)).ShouldBeEmpty();
  }

  [Fact]
  public async Task DataModelRecordRepository_DeleteAllAsync_SmazeVsechnyZaznamyIJejichPole()
  {
    // Arrange
    var modelId = await SeedDataModelAsync("MODEL-S");
    await SeedRecordAsync(modelId, "EXT-1", fieldCount: 2);
    await SeedRecordAsync(modelId, "EXT-2", fieldCount: 1);

    // Act
    var deletedCount = await _recordRepo.DeleteAllAsync(CancellationToken.None);

    // Assert
    deletedCount.ShouldBe(2);
    (await _recordRepo.ListAsync(CancellationToken.None)).ShouldBeEmpty();
    (await DbContext.Set<DataModelRecordField>().ToListAsync(CancellationToken.None)).ShouldBeEmpty();
  }

  [Fact]
  public async Task DataModelRepository_DeleteAllAsync_NemazeZaznamyDataModelu()
  {
    // Arrange — DeleteAllAsync na DataModelRepository se týká pouze DataModelů, ne DataModelRecordů.
    var modelId = await SeedDataModelAsync("MODEL-X");
    await SeedRecordAsync(modelId, "EXT-1");

    // Act
    await _dataModelRepo.DeleteAllAsync(CancellationToken.None);

    // Assert
    (await _recordRepo.ListAsync(CancellationToken.None)).Count.ShouldBe(1);
  }

  [Fact]
  public async Task DataModelRepository_DeleteAllAsync_VraciNulu_KdyzZadnyModelNeexistuje()
  {
    // Act
    var deletedCount = await _dataModelRepo.DeleteAllAsync(CancellationToken.None);

    // Assert
    deletedCount.ShouldBe(0);
  }
}
