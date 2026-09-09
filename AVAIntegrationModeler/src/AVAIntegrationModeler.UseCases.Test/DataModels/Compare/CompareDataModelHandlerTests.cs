using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DataModels;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.DataModels.Compare;
using NSubstitute;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.DataModels.Compare;

/// <summary>
/// Unit testy pro CompareDataModelHandler — orchestrace načítání a delegace na DataModelComparer.
/// </summary>
public class CompareDataModelHandlerTests
{
  private static readonly Guid ModelId = Guid.Parse("22222222-2222-2222-2222-222222222222");

  private static DataModelDTO BuildModel(Guid? id = null, string code = "DM-001") => new()
  {
    Id = id ?? ModelId,
    Code = code,
    Name = "Model",
    Fields = new List<DataModelFieldDTO>()
  };

  private static IDataModelQueryService BuildQueryService(
    DataModelDTO? dbModel,
    DataModelDTO? avaModel)
  {
    var svc = Substitute.For<IDataModelQueryService>();
    var dbList = dbModel is not null
      ? new List<DataModelDTO> { dbModel }
      : new List<DataModelDTO>();
    var avaList = avaModel is not null
      ? new List<DataModelDTO> { avaModel }
      : new List<DataModelDTO>();

    svc.ListAsync(Datasource.Database).Returns(Task.FromResult<IEnumerable<DataModelDTO>>(dbList));
    svc.ListAsync(Datasource.AVAPlace).Returns(Task.FromResult<IEnumerable<DataModelDTO>>(avaList));
    return svc;
  }

  [Fact]
  public async Task Handle_BothSourcesLoaded_ReturnsSuccess()
  {
    var db = BuildModel();
    var ava = BuildModel();
    var svc = BuildQueryService(db, ava);
    var handler = new CompareDataModelHandler(svc);

    var result = await handler.Handle(new CompareDataModelQuery(ModelId), CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.DataModelId.ShouldBe(ModelId);
  }

  [Fact]
  public async Task Handle_NeitherSourceHasModel_ReturnsNotFound()
  {
    var svc = BuildQueryService(null, null);
    var handler = new CompareDataModelHandler(svc);

    var result = await handler.Handle(new CompareDataModelQuery(ModelId), CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
  }

  [Fact]
  public async Task Handle_OnlyInDatabase_ReturnsSuccessWithCorrectStatus()
  {
    var db = BuildModel();
    var svc = BuildQueryService(db, null);
    var handler = new CompareDataModelHandler(svc);

    var result = await handler.Handle(new CompareDataModelQuery(ModelId), CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.ModelStatus.ShouldBe(ComparisonStatus.OnlyInDatabase);
    result.Value.ExistsInDatabase.ShouldBeTrue();
    result.Value.ExistsInAvaPlace.ShouldBeFalse();
  }

  [Fact]
  public async Task Handle_OnlyInAvaPlace_ReturnsSuccessWithCorrectStatus()
  {
    var ava = BuildModel();
    var svc = BuildQueryService(null, ava);
    var handler = new CompareDataModelHandler(svc);

    var result = await handler.Handle(new CompareDataModelQuery(ModelId), CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.ModelStatus.ShouldBe(ComparisonStatus.OnlyInAvaPlace);
    result.Value.ExistsInDatabase.ShouldBeFalse();
    result.Value.ExistsInAvaPlace.ShouldBeTrue();
  }

  [Fact]
  public async Task Handle_BothExistWithDifferentCode_ReturnsDifferentStatus()
  {
    var db = BuildModel(code: "CODE-DB");
    var ava = BuildModel(code: "CODE-AVA");
    var svc = BuildQueryService(db, ava);
    var handler = new CompareDataModelHandler(svc);

    var result = await handler.Handle(new CompareDataModelQuery(ModelId), CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.ModelStatus.ShouldBe(ComparisonStatus.Different);
    result.Value.HasDifferences.ShouldBeTrue();
    result.Value.PropertyDiffs.ShouldContain(d => d.PropertyName == "Code");
  }

  [Fact]
  public async Task Handle_QueriesBothSourcesInParallel()
  {
    // Oba zdroje musí být dotázány — jinak handler nemůže sestavit porovnání
    var db = BuildModel();
    var svc = BuildQueryService(db, null);
    var handler = new CompareDataModelHandler(svc);

    await handler.Handle(new CompareDataModelQuery(ModelId), CancellationToken.None);

    await svc.Received(1).ListAsync(Datasource.Database);
    await svc.Received(1).ListAsync(Datasource.AVAPlace);
  }

  [Fact]
  public async Task Handle_IgnoresOtherModels_ReturnsOnlyRequestedId()
  {
    var otherId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    var svc = Substitute.For<IDataModelQueryService>();
    svc.ListAsync(Datasource.Database).Returns(Task.FromResult<IEnumerable<DataModelDTO>>(
      new[] { BuildModel(id: otherId, code: "OTHER"), BuildModel(id: ModelId, code: "TARGET") }));
    svc.ListAsync(Datasource.AVAPlace).Returns(Task.FromResult<IEnumerable<DataModelDTO>>(
      new[] { BuildModel(id: ModelId, code: "TARGET") }));

    var handler = new CompareDataModelHandler(svc);
    var result = await handler.Handle(new CompareDataModelQuery(ModelId), CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.Code.ShouldBe("TARGET");
    result.Value.ModelStatus.ShouldBe(ComparisonStatus.Same);
  }
}
