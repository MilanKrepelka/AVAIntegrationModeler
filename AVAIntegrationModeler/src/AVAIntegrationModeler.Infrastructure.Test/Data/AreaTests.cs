using AVAIntegrationModeler.Domain.AreaAggregate;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Integration.Test.Data.SqlLite.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AVAIntegrationModeler.Infrastructure.Test.Data;

/// <summary>
/// Integrační testy pro databázovou vrstvu Area aggregate.
/// </summary>
[Collection("AreaTestCollection")] // 🔥 PŘIDAT
public class AreaTests : BaseDbTests
{
  private readonly EfRepository<Area> _repository;

  public AreaTests(ITestOutputHelper testOutputHelper, EfSqlClientTestFixture fixture)
    : base(testOutputHelper, fixture)
  {
    _repository = GetRepository<Area>();
  }

  [Fact]
  public async Task AddArea_ShouldPersistToDatabase()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var areaCode = "TEST_AREA";
    var areaName = "Test Area Name";
    var area = new Area(areaId, areaCode);
    area.SetName(areaName);

    // Act
    await _repository.AddAsync(area, CancellationToken.None);

    // Assert
    var savedArea = (await _repository.ListAsync(CancellationToken.None))
      .FirstOrDefault(a => a.Id == areaId);

    savedArea.ShouldNotBeNull();
    savedArea.Id.ShouldBe(areaId);
    savedArea.Code.ShouldBe(areaCode);
    savedArea.Name.ShouldBe(areaName);
  }

  [Fact]
  public async Task AddArea_WithMinimalData_ShouldSucceed()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var areaCode = "MIN_AREA";
    var area = new Area(areaId, areaCode);

    // Act
    await _repository.AddAsync(area, CancellationToken.None);

    // Assert
    var savedArea = (await _repository.ListAsync(CancellationToken.None))
      .FirstOrDefault(a => a.Id == areaId);

    savedArea.ShouldNotBeNull();
    savedArea.Id.ShouldBe(areaId);
    savedArea.Code.ShouldBe(areaCode);
    savedArea.Name.ShouldBeEmpty();
  }

  [Fact]
  public async Task UpdateArea_ShouldPersistChanges()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var area = new Area(areaId, "ORIG_CODE");
    area.SetName("Original Name");
    await _repository.AddAsync(area, CancellationToken.None);

    // Detach to simulate fetching from database
    DbContext.Entry(area).State = EntityState.Detached;

    // Act
    var fetchedArea = (await _repository.ListAsync(CancellationToken.None))
      .FirstOrDefault(a => a.Id == areaId);

    fetchedArea.ShouldNotBeNull();
    fetchedArea.SetCode("NEW_CODE");
    fetchedArea.SetName("New Name");

    await _repository.UpdateAsync(fetchedArea, CancellationToken.None);

    // Assert
    var updatedArea = (await _repository.ListAsync(CancellationToken.None))
      .FirstOrDefault(a => a.Id == areaId);

    updatedArea.ShouldNotBeNull();
    updatedArea.Code.ShouldBe("NEW_CODE");
    updatedArea.Name.ShouldBe("New Name");
  }

  [Fact]
  public async Task DeleteArea_ShouldRemoveFromDatabase()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var area = new Area(areaId, "DELETE_TEST");
    area.SetName("Area to Delete");
    await _repository.AddAsync(area, CancellationToken.None);

    // Verify it was added
    var addedArea = (await _repository.ListAsync(CancellationToken.None))
      .FirstOrDefault(a => a.Id == areaId);
    addedArea.ShouldNotBeNull();

    // Act
    await _repository.DeleteAsync(addedArea, CancellationToken.None);

    // Assert
    var deletedArea = (await _repository.ListAsync(CancellationToken.None))
      .FirstOrDefault(a => a.Id == areaId);
    deletedArea.ShouldBeNull();
  }

  [Fact]
  public async Task GetById_ShouldReturnCorrectArea()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var area = new Area(areaId, "FIND_ME");
    area.SetName("Findable Area");
    await _repository.AddAsync(area, CancellationToken.None);

    // Act
    var foundArea = await _repository.GetByIdAsync(areaId, CancellationToken.None);

    // Assert
    foundArea.ShouldNotBeNull();
    foundArea.Id.ShouldBe(areaId);
    foundArea.Code.ShouldBe("FIND_ME");
    foundArea.Name.ShouldBe("Findable Area");
  }

  [Fact]
  public async Task ListAreas_ShouldReturnAllAreas()
  {
    // Arrange
    var area1 = new Area(Guid.NewGuid(), "AREA_1");
    area1.SetName("First Area");
    var area2 = new Area(Guid.NewGuid(), "AREA_2");
    area2.SetName("Second Area");
    var area3 = new Area(Guid.NewGuid(), "AREA_3");
    area3.SetName("Third Area");

    await _repository.AddAsync(area1, CancellationToken.None);
    await _repository.AddAsync(area2, CancellationToken.None);
    await _repository.AddAsync(area3, CancellationToken.None);

    // Act
    var allAreas = await _repository.ListAsync(CancellationToken.None);

    // Assert
    allAreas.ShouldNotBeEmpty();
    allAreas.Count.ShouldBe(3);
    allAreas.ShouldContain(a => a.Code == "AREA_1");
    allAreas.ShouldContain(a => a.Code == "AREA_2");
    allAreas.ShouldContain(a => a.Code == "AREA_3");
  }

  [Fact]
  public async Task Area_UniqueCode_ShouldBeEnforced()
  {
    // Arrange
    var area1 = new Area(Guid.NewGuid(), "DUPLICATE_CODE");
    area1.SetName("First Area");
    await _repository.AddAsync(area1, CancellationToken.None);

    var area2 = new Area(Guid.NewGuid(), "DUPLICATE_CODE");
    area2.SetName("Second Area");

    // Act & Assert
    // SQLite s unique indexem by mělo hodit výjimku při pokusu o uložení duplicitního kódu
    var exception = await Should.ThrowAsync<DbUpdateException>(async () =>
    {
      await _repository.AddAsync(area2, CancellationToken.None);
    });

    exception.ShouldNotBeNull();
  }

  [Fact]
  public async Task SaveMultipleAreas_AndQueryThem()
  {
    // Arrange - vytvoříme 5 oblastí
    var areas = new List<Area>
    {
      new Area(Guid.NewGuid(), "SALES"),
      new Area(Guid.NewGuid(), "FINANCE"),
      new Area(Guid.NewGuid(), "HR"),
      new Area(Guid.NewGuid(), "IT"),
      new Area(Guid.NewGuid(), "PRODUCTION")
    };

    areas[0].SetName("Sales Department");
    areas[1].SetName("Finance Department");
    areas[2].SetName("Human Resources");
    areas[3].SetName("Information Technology");
    areas[4].SetName("Production");

    // Act - uložíme všechny oblasti
    foreach (var area in areas)
    {
      await _repository.AddAsync(area, CancellationToken.None);
    }

    // Assert - načteme všechny a ověříme
    var savedAreas = await _repository.ListAsync(CancellationToken.None);
    savedAreas.Count.ShouldBe(5);

    var salesArea = savedAreas.FirstOrDefault(a => a.Code == "SALES");
    salesArea.ShouldNotBeNull();
    salesArea.Name.ShouldBe("Sales Department");
  }

  [Fact]
  public void Area_SetCode_EmptyString_ShouldThrowException()
  {
    // Arrange & Act & Assert
    Should.Throw<ArgumentException>(() =>
      new Area(Guid.NewGuid(), string.Empty));
  }

  [Fact]
  public void Area_SetName_EmptyString_ShouldThrowException()
  {
    // Arrange
    var area = new Area(Guid.NewGuid(), "VALID_CODE");

    // Act & Assert
    Should.Throw<ArgumentException>(() =>
      area.SetName(string.Empty));
  }

  [Fact]
  public async Task Area_FluentAPI_ShouldChainMethods()
  {
    // Arrange
    var areaId = Guid.NewGuid();

    // Act
    var area = new Area(areaId, "FLUENT_TEST")
      .SetName("Fluent API Test");

    await _repository.AddAsync(area, CancellationToken.None);

    // Assert
    var savedArea = await _repository.GetByIdAsync(areaId, CancellationToken.None);
    savedArea.ShouldNotBeNull();
    savedArea.Code.ShouldBe("FLUENT_TEST");
    savedArea.Name.ShouldBe("Fluent API Test");
  }
}

