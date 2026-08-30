using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DataModels;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.DataModels.Compare;
using NSubstitute;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.DataModels.Compare;

/// <summary>
/// Unit testy pro GetDeploymentChangesSummaryHandler — agregace výsledků porovnání
/// všech DataModelů nasazení, bez DB lookupu nasazení.
/// </summary>
public class GetDeploymentChangesSummaryHandlerTests
{
  private const string DeploymentCode = "DEP-001";
  private const string DeploymentName = "Testovací nasazení";

  private static DataModelDTO BuildModel(Guid id, string code = "DM") => new()
  {
    Id = id,
    Code = code,
    Name = "Model " + code,
    Fields = new List<DataModelFieldDTO>()
  };

  private static IDataModelQueryService BuildQueryService(
    IEnumerable<DataModelDTO> dbModels,
    IEnumerable<DataModelDTO> avaModels)
  {
    var svc = Substitute.For<IDataModelQueryService>();
    svc.ListAsync(Datasource.Database).Returns(Task.FromResult<IEnumerable<DataModelDTO>>(dbModels.ToList()));
    svc.ListAsync(Datasource.AVAPlace).Returns(Task.FromResult<IEnumerable<DataModelDTO>>(avaModels.ToList()));
    return svc;
  }

  [Fact]
  public async Task Handle_EmptyDataModelIds_ReturnsSummaryWithZeroModels()
  {
    var svc = BuildQueryService(Array.Empty<DataModelDTO>(), Array.Empty<DataModelDTO>());
    var handler = new GetDeploymentChangesSummaryHandler(svc);

    var result = await handler.Handle(
      new GetDeploymentChangesSummaryQuery(DeploymentCode, DeploymentName, new List<Guid>()),
      CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.TotalModels.ShouldBe(0);
    result.Value.DeploymentCode.ShouldBe(DeploymentCode);
    result.Value.DeploymentName.ShouldBe(DeploymentName);
  }

  [Fact]
  public async Task Handle_AllModelsInBothSources_ModelsSameCounted()
  {
    var id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");
    var svc = BuildQueryService(
      new[] { BuildModel(id, "DM-001") },
      new[] { BuildModel(id, "DM-001") });

    var handler = new GetDeploymentChangesSummaryHandler(svc);
    var result = await handler.Handle(
      new GetDeploymentChangesSummaryQuery(DeploymentCode, DeploymentName, new List<Guid> { id }),
      CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.TotalModels.ShouldBe(1);
    result.Value.ModelsSame.ShouldBe(1);
    result.Value.ModelsDifferent.ShouldBe(0);
    result.Value.HasDifferences.ShouldBeFalse();
  }

  [Fact]
  public async Task Handle_ModelOnlyInDatabase_CountedAsOnlyInDatabase()
  {
    var id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002");
    var svc = BuildQueryService(
      new[] { BuildModel(id, "DM-DB") },
      Array.Empty<DataModelDTO>());

    var handler = new GetDeploymentChangesSummaryHandler(svc);
    var result = await handler.Handle(
      new GetDeploymentChangesSummaryQuery(DeploymentCode, DeploymentName, new List<Guid> { id }),
      CancellationToken.None);

    result.Value.ModelsOnlyInDatabase.ShouldBe(1);
    result.Value.ModelsOnlyInAvaPlace.ShouldBe(0);
    result.Value.HasDifferences.ShouldBeTrue();
  }

  [Fact]
  public async Task Handle_ModelOnlyInAvaPlace_CountedAsOnlyInAvaPlace()
  {
    var id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003");
    var svc = BuildQueryService(
      Array.Empty<DataModelDTO>(),
      new[] { BuildModel(id, "DM-AVA") });

    var handler = new GetDeploymentChangesSummaryHandler(svc);
    var result = await handler.Handle(
      new GetDeploymentChangesSummaryQuery(DeploymentCode, DeploymentName, new List<Guid> { id }),
      CancellationToken.None);

    result.Value.ModelsOnlyInAvaPlace.ShouldBe(1);
    result.Value.ModelsOnlyInDatabase.ShouldBe(0);
    result.Value.HasDifferences.ShouldBeTrue();
  }

  [Fact]
  public async Task Handle_ModelWithDifferentCode_CountedAsDifferent()
  {
    var id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004");
    var svc = BuildQueryService(
      new[] { BuildModel(id, "CODE-DB") },
      new[] { BuildModel(id, "CODE-AVA") });

    var handler = new GetDeploymentChangesSummaryHandler(svc);
    var result = await handler.Handle(
      new GetDeploymentChangesSummaryQuery(DeploymentCode, DeploymentName, new List<Guid> { id }),
      CancellationToken.None);

    result.Value.ModelsDifferent.ShouldBe(1);
    result.Value.ModelsSame.ShouldBe(0);
    result.Value.HasDifferences.ShouldBeTrue();
  }

  [Fact]
  public async Task Handle_MultipleModels_CountsAreAggregatedCorrectly()
  {
    var idSame = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000010");
    var idDiff = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000011");
    var idDbOnly = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000012");

    var svc = BuildQueryService(
      new[] { BuildModel(idSame, "SAME"), BuildModel(idDiff, "DM-DB"), BuildModel(idDbOnly, "DB-ONLY") },
      new[] { BuildModel(idSame, "SAME"), BuildModel(idDiff, "DM-AVA") });

    var handler = new GetDeploymentChangesSummaryHandler(svc);
    var result = await handler.Handle(
      new GetDeploymentChangesSummaryQuery(DeploymentCode, DeploymentName,
        new List<Guid> { idSame, idDiff, idDbOnly }),
      CancellationToken.None);

    result.Value.TotalModels.ShouldBe(3);
    result.Value.ModelsSame.ShouldBe(1);
    result.Value.ModelsDifferent.ShouldBe(1);
    result.Value.ModelsOnlyInDatabase.ShouldBe(1);
    result.Value.ModelsOnlyInAvaPlace.ShouldBe(0);
    result.Value.HasDifferences.ShouldBeTrue();
  }

  [Fact]
  public async Task Handle_ModelsSortedByCodeCaseInsensitive()
  {
    var idZ = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000020");
    var idA = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000021");
    var idM = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000022");

    var svc = BuildQueryService(
      new[] { BuildModel(idZ, "Zebra"), BuildModel(idA, "Apple"), BuildModel(idM, "Mango") },
      Array.Empty<DataModelDTO>());

    var handler = new GetDeploymentChangesSummaryHandler(svc);
    var result = await handler.Handle(
      new GetDeploymentChangesSummaryQuery(DeploymentCode, DeploymentName,
        new List<Guid> { idZ, idA, idM }),
      CancellationToken.None);

    result.Value.Models.Select(m => m.Code).ShouldBe(new[] { "Apple", "Mango", "Zebra" });
  }

  [Fact]
  public async Task Handle_GuidEmptyInDataModelIds_IsIgnored()
  {
    // Guid.Empty v seznamu DataModelIds nesmí způsobit chybu
    var id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000030");
    var svc = BuildQueryService(
      new[] { BuildModel(id, "DM-001") },
      new[] { BuildModel(id, "DM-001") });

    var handler = new GetDeploymentChangesSummaryHandler(svc);
    var result = await handler.Handle(
      new GetDeploymentChangesSummaryQuery(DeploymentCode, DeploymentName,
        new List<Guid> { Guid.Empty, id }),
      CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.TotalModels.ShouldBe(1); // Guid.Empty se ignoruje
  }

  [Fact]
  public async Task Handle_AvaPlaceModelWithGuidEmptyId_Deduplicated()
  {
    // AVAPlace může vrátit více modelů s Guid.Empty ID — GroupBy+First() je filtruje
    var emptyId = Guid.Empty;
    var svc = Substitute.For<IDataModelQueryService>();
    svc.ListAsync(Datasource.Database).Returns(Task.FromResult<IEnumerable<DataModelDTO>>(
      new List<DataModelDTO>()));
    svc.ListAsync(Datasource.AVAPlace).Returns(Task.FromResult<IEnumerable<DataModelDTO>>(
      new List<DataModelDTO>
      {
        BuildModel(emptyId, "DM-A"),
        BuildModel(emptyId, "DM-B")
      }));

    var id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000040");
    var handler = new GetDeploymentChangesSummaryHandler(svc);

    // Nesmí vyhodit ArgumentException z ToDictionary
    var result = await handler.Handle(
      new GetDeploymentChangesSummaryQuery(DeploymentCode, DeploymentName, new List<Guid> { id }),
      CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }

  [Fact]
  public async Task Handle_SummaryItemContainsFieldDiffCounts()
  {
    var id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000050");
    var dbModel = new DataModelDTO
    {
      Id = id, Code = "DM-001", Name = "Model",
      Fields = new List<DataModelFieldDTO>
      {
        new() { Id = Guid.NewGuid(), Name = "Field1", FieldType = DataModelFieldType.Text, ReferencedEntityTypeIds = new() },
        new() { Id = Guid.NewGuid(), Name = "DbOnly", FieldType = DataModelFieldType.Text, ReferencedEntityTypeIds = new() }
      }
    };
    var avaModel = new DataModelDTO
    {
      Id = id, Code = "DM-001", Name = "Model",
      Fields = new List<DataModelFieldDTO>
      {
        new() { Id = Guid.NewGuid(), Name = "Field1", FieldType = DataModelFieldType.Text, ReferencedEntityTypeIds = new() }
      }
    };

    var svc = BuildQueryService(new[] { dbModel }, new[] { avaModel });
    var handler = new GetDeploymentChangesSummaryHandler(svc);
    var result = await handler.Handle(
      new GetDeploymentChangesSummaryQuery(DeploymentCode, DeploymentName, new List<Guid> { id }),
      CancellationToken.None);

    var item = result.Value.Models.Single();
    item.FieldsOnlyInDatabase.ShouldBe(1);
    item.FieldsOnlyInAvaPlace.ShouldBe(0);
    item.FieldsDifferent.ShouldBe(0);
  }
}
