using AVAIntegrationModeler.Domain.DeploymentAggregate;
using AVAIntegrationModeler.Domain.DeploymentAggregate.Specifications;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Integration.Test.Data.SqlLite.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Collections.Generic;

namespace AVAIntegrationModeler.Infrastructure.Test.Data;

/// <summary>
/// Integrační testy pro databázovou vrstvu Deployment aggregate.
/// </summary>
[Collection("DeploymentTestCollection")]
public class DeploymentTests : BaseDbTests
{
  private readonly EfRepository<Deployment> _repository;
  private readonly DeploymentRepository _deploymentRepository;

  public DeploymentTests(ITestOutputHelper testOutputHelper, EfSqlClientTestFixture fixture)
    : base(testOutputHelper, fixture)
  {
    _repository = GetRepository<Deployment>();
    _deploymentRepository = new DeploymentRepository(DbContext);
  }

  [Fact]
  public async Task AddDeployment_UloziDoDatabaze()
  {
    var id = Guid.NewGuid();
    var dep = new Deployment(id, "DEP-TEST");
    dep.SetName("Test Deployment");

    await _repository.AddAsync(dep, CancellationToken.None);

    var saved = await _repository.GetByIdAsync(id, CancellationToken.None);
    saved.ShouldNotBeNull();
    saved!.Id.ShouldBe(id);
    saved.Code.ShouldBe("DEP-TEST");
    saved.Name.ShouldBe("Test Deployment");
  }

  [Fact]
  public async Task AddDeployment_SDataModely_UloziVse()
  {
    var id = Guid.NewGuid();
    var modelId1 = Guid.NewGuid();
    var modelId2 = Guid.NewGuid();
    var dep = new Deployment(id, "DEP-DM");
    dep.SetName("With DataModels");
    dep.AddDataModel(modelId1);
    dep.AddDataModel(modelId2);

    await _repository.AddAsync(dep, CancellationToken.None);

    var saved = await _repository.FirstOrDefaultAsync(new DeploymentByIdSpec(id), CancellationToken.None);
    saved.ShouldNotBeNull();
    saved!.DataModels.Count.ShouldBe(2);
    saved.DataModels.ShouldContain(m => m.DataModelId == modelId1);
    saved.DataModels.ShouldContain(m => m.DataModelId == modelId2);
  }

  [Fact]
  public async Task UpdateDeployment_ZmeniHodnoty()
  {
    var id = Guid.NewGuid();
    var dep = new Deployment(id, "DEP-ORIG");
    dep.SetName("Original");
    await _repository.AddAsync(dep, CancellationToken.None);
    DbContext.Entry(dep).State = EntityState.Detached;

    var fetched = await _repository.GetByIdAsync(id, CancellationToken.None);
    fetched.ShouldNotBeNull();
    fetched!.SetCode("DEP-NEW").SetName("Updated");
    await _repository.UpdateAsync(fetched, CancellationToken.None);

    var updated = await _repository.GetByIdAsync(id, CancellationToken.None);
    updated.ShouldNotBeNull();
    updated!.Code.ShouldBe("DEP-NEW");
    updated.Name.ShouldBe("Updated");
  }

  [Fact]
  public async Task DeleteDeployment_OdebeZDatabaze()
  {
    var id = Guid.NewGuid();
    var dep = new Deployment(id, "DEP-DEL");
    dep.SetName("To Delete");
    await _repository.AddAsync(dep, CancellationToken.None);

    var added = await _repository.GetByIdAsync(id, CancellationToken.None);
    added.ShouldNotBeNull();
    await _repository.DeleteAsync(added!, CancellationToken.None);

    var deleted = await _repository.GetByIdAsync(id, CancellationToken.None);
    deleted.ShouldBeNull();
  }

  [Fact]
  public async Task UniqueCode_Vynucen()
  {
    var dep1 = new Deployment(Guid.NewGuid(), "DUP-CODE");
    dep1.SetName("First");
    await _repository.AddAsync(dep1, CancellationToken.None);

    var dep2 = new Deployment(Guid.NewGuid(), "DUP-CODE");
    dep2.SetName("Second");

    await Should.ThrowAsync<DbUpdateException>(async () =>
      await _repository.AddAsync(dep2, CancellationToken.None));
  }

  [Fact]
  public async Task FindByCode_SpecificationVraciSpravnyZaznam()
  {
    var id = Guid.NewGuid();
    var dep = new Deployment(id, "DEP-SPEC");
    dep.SetName("Spec Test");
    await _repository.AddAsync(dep, CancellationToken.None);

    var found = await _repository.FirstOrDefaultAsync(new DeploymentByCodeSpec("DEP-SPEC"), CancellationToken.None);

    found.ShouldNotBeNull();
    found!.Id.ShouldBe(id);
  }

  [Fact]
  public async Task SyncDataModels_PridaNovaZaznam()
  {
    var id = Guid.NewGuid();
    var dep = new Deployment(id, "SYNC-ADD");
    dep.SetName("Sync Add Test");
    await _repository.AddAsync(dep, CancellationToken.None);
    DbContext.ChangeTracker.Clear();

    var fetched = await _repository.FirstOrDefaultAsync(new DeploymentByIdSpec(id), CancellationToken.None);
    fetched.ShouldNotBeNull();

    var modelId = Guid.NewGuid();
    fetched!.AddDataModel(modelId);
    var toAdd = fetched.DataModels.ToList();

    await _deploymentRepository.SyncDataModelsAndSaveAsync(fetched, new List<DeploymentDataModel>(), toAdd, TestContext.Current.CancellationToken);
    DbContext.ChangeTracker.Clear();

    var result = await _repository.FirstOrDefaultAsync(new DeploymentByIdSpec(id), CancellationToken.None);
    result.ShouldNotBeNull();
    result!.DataModels.Count.ShouldBe(1);
    result.DataModels.ShouldContain(m => m.DataModelId == modelId);
  }

  [Fact]
  public async Task SyncDataModels_OdebreExistujiciZaznam()
  {
    var id = Guid.NewGuid();
    var modelId1 = Guid.NewGuid();
    var modelId2 = Guid.NewGuid();
    var dep = new Deployment(id, "SYNC-DEL");
    dep.SetName("Sync Delete Test");
    dep.AddDataModel(modelId1);
    dep.AddDataModel(modelId2);
    await _repository.AddAsync(dep, CancellationToken.None);
    DbContext.ChangeTracker.Clear();

    var fetched = await _repository.FirstOrDefaultAsync(new DeploymentByIdSpec(id), CancellationToken.None);
    fetched.ShouldNotBeNull();
    fetched!.DataModels.Count.ShouldBe(2);

    var toDelete = fetched.DataModels.Where(m => m.DataModelId == modelId1).ToList();
    fetched.RemoveDataModel(modelId1);

    await _deploymentRepository.SyncDataModelsAndSaveAsync(fetched, toDelete, new List<DeploymentDataModel>(), TestContext.Current.CancellationToken);
    DbContext.ChangeTracker.Clear();

    var result = await _repository.FirstOrDefaultAsync(new DeploymentByIdSpec(id), CancellationToken.None);
    result.ShouldNotBeNull();
    result!.DataModels.Count.ShouldBe(1);
    result.DataModels.ShouldNotContain(m => m.DataModelId == modelId1);
    result.DataModels.ShouldContain(m => m.DataModelId == modelId2);
  }

  [Fact]
  public async Task SyncDataModels_NahradiVsechnyZaznamy()
  {
    var id = Guid.NewGuid();
    var oldModelId = Guid.NewGuid();
    var newModelId = Guid.NewGuid();
    var dep = new Deployment(id, "SYNC-REPLACE");
    dep.SetName("Sync Replace Test");
    dep.AddDataModel(oldModelId);
    await _repository.AddAsync(dep, CancellationToken.None);
    DbContext.ChangeTracker.Clear();

    var fetched = await _repository.FirstOrDefaultAsync(new DeploymentByIdSpec(id), CancellationToken.None);
    fetched.ShouldNotBeNull();

    var toDelete = fetched!.DataModels.ToList();
    fetched.RemoveDataModel(oldModelId);
    fetched.AddDataModel(newModelId);
    var toAdd = fetched.DataModels.ToList();

    await _deploymentRepository.SyncDataModelsAndSaveAsync(fetched, toDelete, toAdd, TestContext.Current.CancellationToken);
    DbContext.ChangeTracker.Clear();

    var result = await _repository.FirstOrDefaultAsync(new DeploymentByIdSpec(id), CancellationToken.None);
    result.ShouldNotBeNull();
    result!.DataModels.Count.ShouldBe(1);
    result.DataModels.ShouldNotContain(m => m.DataModelId == oldModelId);
    result.DataModels.ShouldContain(m => m.DataModelId == newModelId);
  }

  [Fact]
  public async Task SyncDataModels_AktualizujeNazev()
  {
    var id = Guid.NewGuid();
    var dep = new Deployment(id, "SYNC-NAME");
    dep.SetName("Old Name");
    await _repository.AddAsync(dep, CancellationToken.None);
    DbContext.ChangeTracker.Clear();

    var fetched = await _repository.FirstOrDefaultAsync(new DeploymentByIdSpec(id), CancellationToken.None);
    fetched.ShouldNotBeNull();
    fetched!.SetName("New Name");

    await _deploymentRepository.SyncDataModelsAndSaveAsync(
      fetched, new List<DeploymentDataModel>(), new List<DeploymentDataModel>(), TestContext.Current.CancellationToken);
    DbContext.ChangeTracker.Clear();

    var result = await _repository.GetByIdAsync(id, CancellationToken.None);
    result.ShouldNotBeNull();
    result!.Name.ShouldBe("New Name");
  }
}
