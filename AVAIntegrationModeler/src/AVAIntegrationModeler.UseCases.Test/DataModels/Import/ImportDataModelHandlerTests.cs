using Ardalis.Result;
using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.DataModels.Import;
using NSubstitute;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.DataModels.Import;

/// <summary>
/// Unit testy pro ImportDataModelHandler — orchestrace mezi AVAPlace providerem
/// a IDataModelImportService (samotný upsert testuje DataModelImportServiceTests).
/// </summary>
public class ImportDataModelHandlerTests
{
  private static DataModelDTO BuildModelDTO(string code = "MODEL-01") =>
    new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = code,
      Name = "Test model",
      Description = "Popis",
      Notes = string.Empty,
      IsAggregateRoot = true,
      AreaId = null,
      Fields = new List<DataModelFieldDTO>()
    };

  private static (
    IIntegrationDataProvider IntegrationDataProvider,
    IDataModelImportService ImportService,
    IDataModelQueryService QueryService) BuildMocks()
  {
    return (
      Substitute.For<IIntegrationDataProvider>(),
      Substitute.For<IDataModelImportService>(),
      Substitute.For<IDataModelQueryService>());
  }

  [Fact]
  public async Task Handle_ReturnsNotFound_KdyzModelNeniNalezenVAVAPlace()
  {
    // Arrange
    var (provider, importService, queryService) = BuildMocks();
    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(null));

    var handler = new ImportDataModelHandler(provider, importService, queryService);

    // Act
    var result = await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    result.Status.ShouldBe(ResultStatus.NotFound);
    await importService.DidNotReceive().ImportModelAsync(Arg.Any<DataModelDTO>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_DelegujeNaImportServiceAVraciJehoResult_PriUspechu()
  {
    // Arrange
    var (provider, importService, queryService) = BuildMocks();
    var dto = BuildModelDTO();
    var avaPlaceModelId = Guid.NewGuid();
    var localModelId = Guid.NewGuid();

    provider.GetDataModelByIdAsync(avaPlaceModelId, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));
    importService.ImportModelAsync(dto, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Result<Guid>.Success(localModelId)));

    var handler = new ImportDataModelHandler(provider, importService, queryService);

    // Act
    var result = await handler.Handle(new ImportDataModelCommand(avaPlaceModelId), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    result.Value.ShouldBe(localModelId);
    await importService.Received(1).ImportModelAsync(dto, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_VraciErrorResult_KdyzImportServiceSelze()
  {
    // Arrange
    var (provider, importService, queryService) = BuildMocks();
    var dto = BuildModelDTO();

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));
    importService.ImportModelAsync(dto, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Result<Guid>.Error("Import selhal.")));

    var handler = new ImportDataModelHandler(provider, importService, queryService);

    // Act
    var result = await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(ResultStatus.Error);
  }

  [Fact]
  public async Task Handle_InvalidujeCache_PouzeKdyzImportUspesny()
  {
    // Arrange
    var (provider, importService, queryService) = BuildMocks();
    var dto = BuildModelDTO();

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));
    importService.ImportModelAsync(dto, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Result<Guid>.Success(Guid.NewGuid())));

    var handler = new ImportDataModelHandler(provider, importService, queryService);

    // Act
    await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    queryService.Received(1).InvalidateCache(Datasource.Database);
  }

  [Fact]
  public async Task Handle_NevolajuCache_KdyzImportServiceSelze()
  {
    // Arrange
    var (provider, importService, queryService) = BuildMocks();
    var dto = BuildModelDTO();

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));
    importService.ImportModelAsync(dto, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Result<Guid>.Error("Import selhal.")));

    var handler = new ImportDataModelHandler(provider, importService, queryService);

    // Act
    await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    queryService.DidNotReceive().InvalidateCache(Arg.Any<Datasource>());
  }

  [Fact]
  public async Task Handle_NevolajuCache_KdyzModelNeniNalezenVAVAPlace()
  {
    // Arrange
    var (provider, importService, queryService) = BuildMocks();
    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(null));

    var handler = new ImportDataModelHandler(provider, importService, queryService);

    // Act
    await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    queryService.DidNotReceive().InvalidateCache(Arg.Any<Datasource>());
  }
}
