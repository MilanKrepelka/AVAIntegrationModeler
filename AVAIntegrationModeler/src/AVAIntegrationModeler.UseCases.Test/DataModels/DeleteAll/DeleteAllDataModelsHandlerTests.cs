using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModelRecords;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.DataModels.DeleteAll;
using NSubstitute;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.DataModels.DeleteAll;

/// <summary>
/// Unit testy pro DeleteAllDataModelsHandler — smazání všech datových modelů a jejich záznamů.
/// </summary>
public class DeleteAllDataModelsHandlerTests
{
  private static (
    IDataModelRepository DataModelRepo,
    IDataModelRecordRepository RecordRepo,
    IDataModelQueryService DataModelQueryService,
    IDataModelRecordQueryService RecordQueryService) BuildMocks()
  {
    return (
      Substitute.For<IDataModelRepository>(),
      Substitute.For<IDataModelRecordRepository>(),
      Substitute.For<IDataModelQueryService>(),
      Substitute.For<IDataModelRecordQueryService>());
  }

  [Fact]
  public async Task Handle_VraciSpravnePocty_ZeVsechRepositoriu()
  {
    // Arrange
    var (dataModelRepo, recordRepo, dataModelQueryService, recordQueryService) = BuildMocks();
    recordRepo.DeleteAllAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(7));
    dataModelRepo.DeleteAllAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(3));

    var handler = new DeleteAllDataModelsHandler(dataModelRepo, recordRepo, dataModelQueryService, recordQueryService);

    // Act
    var result = await handler.Handle(new DeleteAllDataModelsCommand(), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    result.Value.DeletedModelsCount.ShouldBe(3);
    result.Value.DeletedRecordsCount.ShouldBe(7);
  }

  [Fact]
  public async Task Handle_SmazeNejprveZaznamyAPakModely()
  {
    // Arrange — pořadí volání není z hlediska DB nutné (žádný FK), ale mělo by být deterministické.
    var (dataModelRepo, recordRepo, dataModelQueryService, recordQueryService) = BuildMocks();
    var callOrder = new List<string>();

    recordRepo.DeleteAllAsync(Arg.Any<CancellationToken>()).Returns(ci =>
    {
      callOrder.Add("Records");
      return Task.FromResult(0);
    });
    dataModelRepo.DeleteAllAsync(Arg.Any<CancellationToken>()).Returns(ci =>
    {
      callOrder.Add("Models");
      return Task.FromResult(0);
    });

    var handler = new DeleteAllDataModelsHandler(dataModelRepo, recordRepo, dataModelQueryService, recordQueryService);

    // Act
    await handler.Handle(new DeleteAllDataModelsCommand(), CancellationToken.None);

    // Assert
    callOrder.ShouldBe(new List<string> { "Records", "Models" });
  }

  [Fact]
  public async Task Handle_InvalidujeCacheProModelyIZaznamy()
  {
    // Arrange
    var (dataModelRepo, recordRepo, dataModelQueryService, recordQueryService) = BuildMocks();
    recordRepo.DeleteAllAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(0));
    dataModelRepo.DeleteAllAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(0));

    var handler = new DeleteAllDataModelsHandler(dataModelRepo, recordRepo, dataModelQueryService, recordQueryService);

    // Act
    await handler.Handle(new DeleteAllDataModelsCommand(), CancellationToken.None);

    // Assert
    dataModelQueryService.Received(1).InvalidateCache(Datasource.Database);
    recordQueryService.Received(1).InvalidateCache(Datasource.Database);
  }

  [Fact]
  public async Task Handle_VraciNulovePocty_KdyzDatabazeJizPrazdna()
  {
    // Arrange
    var (dataModelRepo, recordRepo, dataModelQueryService, recordQueryService) = BuildMocks();
    recordRepo.DeleteAllAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(0));
    dataModelRepo.DeleteAllAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(0));

    var handler = new DeleteAllDataModelsHandler(dataModelRepo, recordRepo, dataModelQueryService, recordQueryService);

    // Act
    var result = await handler.Handle(new DeleteAllDataModelsCommand(), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    result.Value.DeletedModelsCount.ShouldBe(0);
    result.Value.DeletedRecordsCount.ShouldBe(0);
  }
}
