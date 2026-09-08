using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DeploymentAggregate;
using AVAIntegrationModeler.UseCases.Deployments.Mapping;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.Deployments.Mapping;

/// <summary>
/// Unit testy pro DeploymentMapper.
/// </summary>
public class DeploymentMapperTests
{
  [Fact]
  public void MapToDTO_VratiSpravneMapovaneDTO()
  {
    var id = Guid.NewGuid();
    var modelId = Guid.NewGuid();
    var dep = new Deployment(id, "DEP-001");
    dep.SetName("Test Deployment");
    dep.AddDataModel(modelId);

    var dto = DeploymentMapper.MapToDTO(dep);

    dto.ShouldNotBeNull();
    dto!.Id.ShouldBe(id);
    dto.Code.ShouldBe("DEP-001");
    dto.Name.ShouldBe("Test Deployment");
    dto.DataModelIds.ShouldContain(modelId);
    dto.DataModelIds.Count.ShouldBe(1);
  }

  [Fact]
  public void MapToDTO_VratiNull_ProNull()
  {
    DeploymentMapper.MapToDTO(null).ShouldBeNull();
  }

  [Fact]
  public void MapToDTO_PrazdneDataModely_PrazdnySeznam()
  {
    var dep = new Deployment(Guid.NewGuid(), "DEP-EMPTY");
    dep.SetName("Prázdné");

    var dto = DeploymentMapper.MapToDTO(dep);

    dto.ShouldNotBeNull();
    dto!.DataModelIds.ShouldBeEmpty();
  }

  [Fact]
  public void MapToEntity_VratiSpravneMapovanouEntitu()
  {
    var id = Guid.NewGuid();
    var modelId = Guid.NewGuid();
    var dto = new DeploymentDTO
    {
      Id = id,
      Code = "DEP-001",
      Name = "Test",
      DataModelIds = new List<Guid> { modelId }
    };

    var dep = DeploymentMapper.MapToEntity(dto);

    dep.ShouldNotBeNull();
    dep!.Id.ShouldBe(id);
    dep.Code.ShouldBe("DEP-001");
    dep.Name.ShouldBe("Test");
    dep.DataModels.ShouldHaveSingleItem();
    dep.DataModels.First().DataModelId.ShouldBe(modelId);
  }

  [Fact]
  public void MapToEntity_VratiNull_ProNull()
  {
    DeploymentMapper.MapToEntity(null).ShouldBeNull();
  }

  [Fact]
  public void MapToEntity_PrazdneId_GenerujeNoveId()
  {
    var dto = new DeploymentDTO { Id = Guid.Empty, Code = "DEP-X", Name = "X" };
    var dep = DeploymentMapper.MapToEntity(dto);

    dep.ShouldNotBeNull();
    dep!.Id.ShouldNotBe(Guid.Empty);
  }

  [Fact]
  public void RoundTrip_ZachováHodnoty()
  {
    var original = new Deployment(Guid.NewGuid(), "DEP-RT");
    original.SetName("Round Trip");
    original.AddDataModel(Guid.NewGuid());
    original.AddDataModel(Guid.NewGuid());

    var dto = DeploymentMapper.MapToDTO(original)!;
    var restored = DeploymentMapper.MapToEntity(dto)!;

    restored.Id.ShouldBe(original.Id);
    restored.Code.ShouldBe(original.Code);
    restored.Name.ShouldBe(original.Name);
    restored.DataModels.Count.ShouldBe(original.DataModels.Count);
  }

  [Fact]
  public void MapToDTO_MapujeLastSaveDateTime()
  {
    var dt = new DateTime(2026, 8, 31, 10, 0, 0, DateTimeKind.Utc);
    var dep = new Deployment(Guid.NewGuid(), "DEP-DATE");
    dep.SetName("Date Test");
    dep.SetLastSaveDateTime(dt);

    var dto = DeploymentMapper.MapToDTO(dep);

    dto!.LastSaveDateTime.ShouldBe(dt);
    dto.LastDeploymentDateTime.ShouldBeNull();
  }

  [Fact]
  public void MapToDTO_MapujeLastDeploymentDateTime()
  {
    var dt = new DateTime(2026, 8, 31, 18, 0, 0, DateTimeKind.Utc);
    var dep = new Deployment(Guid.NewGuid(), "DEP-DATE2");
    dep.SetName("Date Test 2");
    dep.SetLastDeploymentDateTime(dt);

    var dto = DeploymentMapper.MapToDTO(dep);

    dto!.LastDeploymentDateTime.ShouldBe(dt);
    dto.LastSaveDateTime.ShouldBeNull();
  }

  [Fact]
  public void MapToDTO_ObaDateTimyNull_VratiNull()
  {
    var dep = new Deployment(Guid.NewGuid(), "DEP-NODATES");
    dep.SetName("No Dates");

    var dto = DeploymentMapper.MapToDTO(dep);

    dto!.LastSaveDateTime.ShouldBeNull();
    dto.LastDeploymentDateTime.ShouldBeNull();
  }

  [Fact]
  public void MapToEntity_MapujeLastSaveDateTime()
  {
    var dt = new DateTime(2026, 8, 31, 10, 0, 0, DateTimeKind.Utc);
    var dto = new DeploymentDTO
    {
      Id = Guid.NewGuid(),
      Code = "DEP-DATE",
      Name = "Date Test",
      LastSaveDateTime = dt
    };

    var dep = DeploymentMapper.MapToEntity(dto);

    dep!.LastSaveDateTime.ShouldBe(dt);
    dep.LastDeploymentDateTime.ShouldBeNull();
  }

  [Fact]
  public void MapToEntity_MapujeLastDeploymentDateTime()
  {
    var dt = new DateTime(2026, 8, 31, 18, 0, 0, DateTimeKind.Utc);
    var dto = new DeploymentDTO
    {
      Id = Guid.NewGuid(),
      Code = "DEP-DEPDATE",
      Name = "Deploy Date Test",
      LastDeploymentDateTime = dt
    };

    var dep = DeploymentMapper.MapToEntity(dto);

    dep!.LastDeploymentDateTime.ShouldBe(dt);
    dep.LastSaveDateTime.ShouldBeNull();
  }

  [Fact]
  public void RoundTrip_ZachováiDateTimes()
  {
    var savedt = new DateTime(2026, 8, 31, 10, 0, 0, DateTimeKind.Utc);
    var deploydt = new DateTime(2026, 8, 30, 8, 0, 0, DateTimeKind.Utc);
    var original = new Deployment(Guid.NewGuid(), "DEP-RT2");
    original.SetName("Round Trip Dates");
    original.SetLastSaveDateTime(savedt);
    original.SetLastDeploymentDateTime(deploydt);

    var dto = DeploymentMapper.MapToDTO(original)!;
    var restored = DeploymentMapper.MapToEntity(dto)!;

    restored.LastSaveDateTime.ShouldBe(savedt);
    restored.LastDeploymentDateTime.ShouldBe(deploydt);
  }
}
