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
}
