using AVAIntegrationModeler.Domain.IntegrationMapAggregate;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Integration.Test.Data.SqlLite.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace AVAIntegrationModeler.Infrastructure.Test.Data;

/// <summary>
/// Integrační testy pro IntegrationsMap aggregate v databázi.
/// </summary>
[Collection("IntegrationMapTestCollection")]
public class IntegrationMapTests : BaseDbTests
{
  private readonly EfRepository<IntegrationsMap> _repository;

  public IntegrationMapTests(ITestOutputHelper testOutputHelper, EfSqlClientTestFixture fixture)
    : base(testOutputHelper, fixture)
  {
    _repository = GetRepository<IntegrationsMap>();
  }

  #region Add Tests

  [Fact]
  public async Task AddAsync_SimpleIntegrationMap_ShouldPersistCorrectly()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var mapId = Guid.NewGuid();
    var integrationMap = new IntegrationsMap(mapId, areaId);

    // Act
    await _repository.AddAsync(integrationMap, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    DbContext.ChangeTracker.Clear();
    var savedMap = await _repository.GetByIdAsync(mapId, CancellationToken.None);
    
    savedMap.ShouldNotBeNull();
    savedMap.Id.ShouldBe(mapId);
    savedMap.AreaId.ShouldBe(areaId);
    savedMap.Items.Count.ShouldBe(0);
  }

  [Fact]
  public async Task AddAsync_IntegrationMapWithItems_ShouldPersistCorrectly()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var mapId = Guid.NewGuid();
    var scenarioId1 = Guid.NewGuid();
    var scenarioId2 = Guid.NewGuid();

    var integrationMap = new IntegrationsMap(mapId, areaId);
    integrationMap.AddItem(scenarioId1);
    integrationMap.AddItem(scenarioId2);

    // Act
    await _repository.AddAsync(integrationMap, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    DbContext.ChangeTracker.Clear();
    var savedMap = await _repository.GetByIdAsync(mapId, CancellationToken.None);
    
    savedMap.ShouldNotBeNull();
    savedMap.Items.Count.ShouldBe(2);
    savedMap.Items.ShouldContain(i => i.ScenarioId == scenarioId1);
    savedMap.Items.ShouldContain(i => i.ScenarioId == scenarioId2);
  }

  [Fact]
  public async Task AddAsync_IntegrationMapWithItemsAndKeys_ShouldPersistCorrectly()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var mapId = Guid.NewGuid();
    var scenarioId = Guid.NewGuid();

    var integrationMap = new IntegrationsMap(mapId, areaId);
    var item = integrationMap.AddItem(scenarioId);
    item.AddKey("ORDER_CREATED");
    item.AddKey("ORDER_UPDATED");
    item.AddKey("ORDER_DELETED");

    // Act
    await _repository.AddAsync(integrationMap, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    DbContext.ChangeTracker.Clear();
    var savedMap = await _repository.GetByIdAsync(mapId, CancellationToken.None);
    
    savedMap.ShouldNotBeNull();
    savedMap.Items.Count.ShouldBe(1);
    
    var savedItem = savedMap.GetItem(scenarioId);
    savedItem.ShouldNotBeNull();
    savedItem.Keys.Count.ShouldBe(3);
    savedItem.Keys.ShouldContain(k => k.Key == "ORDER_CREATED");
    savedItem.Keys.ShouldContain(k => k.Key == "ORDER_UPDATED");
    savedItem.Keys.ShouldContain(k => k.Key == "ORDER_DELETED");
  }

  #endregion

  #region Update Tests

  [Fact]
  public async Task UpdateAsync_AddNewItem_ShouldPersistChanges()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var mapId = Guid.NewGuid();
    var scenarioId1 = Guid.NewGuid();

    var integrationMap = new IntegrationsMap(mapId, areaId);
    integrationMap.AddItem(scenarioId1);
    
    await _repository.AddAsync(integrationMap, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Act
    DbContext.ChangeTracker.Clear();
    var fetchedMap = await _repository.GetByIdAsync(mapId, CancellationToken.None);
    fetchedMap.ShouldNotBeNull();
    
    var scenarioId2 = Guid.NewGuid();
    fetchedMap.AddItem(scenarioId2);
    
    await _repository.UpdateAsync(fetchedMap, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    DbContext.ChangeTracker.Clear();
    var updatedMap = await _repository.GetByIdAsync(mapId, CancellationToken.None);
    updatedMap.ShouldNotBeNull();
    updatedMap.Items.Count.ShouldBe(2);
    updatedMap.GetItem(scenarioId2).ShouldNotBeNull();
  }

  [Fact]
  public async Task UpdateAsync_RemoveItem_ShouldPersistChanges()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var mapId = Guid.NewGuid();
    var scenarioId1 = Guid.NewGuid();
    var scenarioId2 = Guid.NewGuid();

    var integrationMap = new IntegrationsMap(mapId, areaId);
    integrationMap.AddItem(scenarioId1);
    integrationMap.AddItem(scenarioId2);
    
    await _repository.AddAsync(integrationMap, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Act
    DbContext.ChangeTracker.Clear();
    var fetchedMap = await _repository.GetByIdAsync(mapId, CancellationToken.None);
    fetchedMap.ShouldNotBeNull();
    
    fetchedMap.RemoveItem(scenarioId1);
    
    await _repository.UpdateAsync(fetchedMap, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    DbContext.ChangeTracker.Clear();
    var updatedMap = await _repository.GetByIdAsync(mapId, CancellationToken.None);
    updatedMap.ShouldNotBeNull();
    updatedMap.Items.Count.ShouldBe(1);
    updatedMap.GetItem(scenarioId1).ShouldBeNull();
    updatedMap.GetItem(scenarioId2).ShouldNotBeNull();
  }

  [Fact]
  public async Task UpdateAsync_AddKeyToItem_ShouldPersistChanges()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var mapId = Guid.NewGuid();
    var scenarioId = Guid.NewGuid();

    var integrationMap = new IntegrationsMap(mapId, areaId);
    var item = integrationMap.AddItem(scenarioId);
    item.AddKey("INITIAL_KEY");
    
    await _repository.AddAsync(integrationMap, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Act
    DbContext.ChangeTracker.Clear();
    var fetchedMap = await _repository.GetByIdAsync(mapId, CancellationToken.None);
    fetchedMap.ShouldNotBeNull();
    
    var fetchedItem = fetchedMap.GetItem(scenarioId);
    fetchedItem.ShouldNotBeNull();
    fetchedItem.AddKey("NEW_KEY");
    
    await _repository.UpdateAsync(fetchedMap, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    DbContext.ChangeTracker.Clear();
    var updatedMap = await _repository.GetByIdAsync(mapId, CancellationToken.None);
    var updatedItem = updatedMap!.GetItem(scenarioId);
    
    updatedItem.ShouldNotBeNull();
    updatedItem.Keys.Count.ShouldBe(2);
    updatedItem.GetKey("INITIAL_KEY").ShouldNotBeNull();
    updatedItem.GetKey("NEW_KEY").ShouldNotBeNull();
  }

  #endregion

  #region Business Rules Tests

  [Fact]
  public void AddItem_DuplicateScenario_ShouldThrowException()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var mapId = Guid.NewGuid();
    var scenarioId = Guid.NewGuid();
    var integrationMap = new IntegrationsMap(mapId, areaId);
    
    integrationMap.AddItem(scenarioId);

    // Act & Assert
    Should.Throw<InvalidOperationException>(() => integrationMap.AddItem(scenarioId));
  }

  [Fact]
  public void AddKey_DuplicateKey_ShouldThrowException()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var mapId = Guid.NewGuid();
    var scenarioId = Guid.NewGuid();
    var integrationMap = new IntegrationsMap(mapId, areaId);
    var item = integrationMap.AddItem(scenarioId);
    
    item.AddKey("DUPLICATE_KEY");

    // Act & Assert
    Should.Throw<InvalidOperationException>(() => item.AddKey("DUPLICATE_KEY"));
  }

  #endregion
}
