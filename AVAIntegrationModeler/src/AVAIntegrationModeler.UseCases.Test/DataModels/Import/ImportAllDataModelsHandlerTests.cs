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
/// Unit testy pro ImportAllDataModelsHandler — hromadný import všech datových modelů z AVAPlace,
/// continue-on-error a jednorázová invalidace cache.
/// </summary>
public class ImportAllDataModelsHandlerTests
{
  private static DataModelDTO BuildModelDTO(string code) =>
    new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = code,
      Name = $"Model {code}",
      Description = string.Empty,
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
  public async Task Handle_VraciSuccessProKazdyModel_KdyzVsechnyImportyUspesne()
  {
    // Arrange
    var (provider, importService, queryService) = BuildMocks();
    var models = new[] { BuildModelDTO("A"), BuildModelDTO("B"), BuildModelDTO("C") };
    provider.GetDataModelsAsync(Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelDTO>>(models));
    importService.ImportModelAsync(Arg.Any<DataModelDTO>(), Arg.Any<CancellationToken>())
      .Returns(ci => Task.FromResult(Result<Guid>.Success(Guid.NewGuid())));

    var handler = new ImportAllDataModelsHandler(provider, importService, queryService);

    // Act
    var result = await handler.Handle(new ImportAllDataModelsCommand(), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    result.Value.SuccessCount.ShouldBe(3);
    result.Value.FailedCount.ShouldBe(0);
    result.Value.Results.Count.ShouldBe(3);
  }

  [Fact]
  public async Task Handle_PokracujeIKdyzJedenModelSelze_ContinueOnError()
  {
    // Arrange
    var (provider, importService, queryService) = BuildMocks();
    var modelA = BuildModelDTO("A");
    var modelB = BuildModelDTO("B");
    var modelC = BuildModelDTO("C");
    provider.GetDataModelsAsync(Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelDTO>>(new[] { modelA, modelB, modelC }));

    importService.ImportModelAsync(modelA, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Result<Guid>.Success(Guid.NewGuid())));
    importService.ImportModelAsync(modelB, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Result<Guid>.Error("Validace pole selhala.")));
    importService.ImportModelAsync(modelC, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Result<Guid>.Success(Guid.NewGuid())));

    var handler = new ImportAllDataModelsHandler(provider, importService, queryService);

    // Act
    var result = await handler.Handle(new ImportAllDataModelsCommand(), CancellationToken.None);

    // Assert — celkový výsledek je úspěšný (dávka doběhla), i když jeden model selhal
    result.IsSuccess.ShouldBeTrue();
    result.Value.SuccessCount.ShouldBe(2);
    result.Value.FailedCount.ShouldBe(1);

    var failed = result.Value.Results.Single(r => r.Code == "B");
    failed.Success.ShouldBeFalse();
    failed.ErrorMessage.ShouldNotBeNullOrEmpty();

    await importService.Received(1).ImportModelAsync(modelA, Arg.Any<CancellationToken>());
    await importService.Received(1).ImportModelAsync(modelB, Arg.Any<CancellationToken>());
    await importService.Received(1).ImportModelAsync(modelC, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ZachytiNeockavanouVyjimkuZImportService_APokracujeSDalsimiModely()
  {
    // Arrange
    var (provider, importService, queryService) = BuildMocks();
    var modelA = BuildModelDTO("A");
    var modelB = BuildModelDTO("B");
    provider.GetDataModelsAsync(Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelDTO>>(new[] { modelA, modelB }));

    importService.ImportModelAsync(modelA, Arg.Any<CancellationToken>())
      .Returns(Task.FromException<Result<Guid>>(new InvalidOperationException("Neočekávaná chyba DB.")));
    importService.ImportModelAsync(modelB, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Result<Guid>.Success(Guid.NewGuid())));

    var handler = new ImportAllDataModelsHandler(provider, importService, queryService);

    // Act
    var result = await handler.Handle(new ImportAllDataModelsCommand(), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    result.Value.SuccessCount.ShouldBe(1);
    result.Value.FailedCount.ShouldBe(1);
    result.Value.Results.Single(r => r.Code == "A").ErrorMessage.ShouldBe("Neočekávaná chyba DB.");
    await importService.Received(1).ImportModelAsync(modelB, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_InvalidateCacheJenJednouPoDokonceniVsech()
  {
    // Arrange
    var (provider, importService, queryService) = BuildMocks();
    var models = new[] { BuildModelDTO("A"), BuildModelDTO("B") };
    provider.GetDataModelsAsync(Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelDTO>>(models));
    importService.ImportModelAsync(Arg.Any<DataModelDTO>(), Arg.Any<CancellationToken>())
      .Returns(ci => Task.FromResult(Result<Guid>.Success(Guid.NewGuid())));

    var handler = new ImportAllDataModelsHandler(provider, importService, queryService);

    // Act
    await handler.Handle(new ImportAllDataModelsCommand(), CancellationToken.None);

    // Assert
    queryService.Received(1).InvalidateCache(Datasource.Database);
  }

  [Fact]
  public async Task Handle_NevolaInvalidateCache_KdyzVsechnyModelySelhaly()
  {
    // Arrange
    var (provider, importService, queryService) = BuildMocks();
    var models = new[] { BuildModelDTO("A"), BuildModelDTO("B") };
    provider.GetDataModelsAsync(Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelDTO>>(models));
    importService.ImportModelAsync(Arg.Any<DataModelDTO>(), Arg.Any<CancellationToken>())
      .Returns(ci => Task.FromResult(Result<Guid>.Error("Selhalo.")));

    var handler = new ImportAllDataModelsHandler(provider, importService, queryService);

    // Act
    var result = await handler.Handle(new ImportAllDataModelsCommand(), CancellationToken.None);

    // Assert
    result.Value.FailedCount.ShouldBe(2);
    queryService.DidNotReceive().InvalidateCache(Arg.Any<Datasource>());
  }

  [Fact]
  public async Task Handle_VraciPrazdnyVysledek_KdyzAVAPlaceNemaZadneModely()
  {
    // Arrange
    var (provider, importService, queryService) = BuildMocks();
    provider.GetDataModelsAsync(Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Enumerable.Empty<DataModelDTO>()));

    var handler = new ImportAllDataModelsHandler(provider, importService, queryService);

    // Act
    var result = await handler.Handle(new ImportAllDataModelsCommand(), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    result.Value.Results.ShouldBeEmpty();
    result.Value.SuccessCount.ShouldBe(0);
    result.Value.FailedCount.ShouldBe(0);
    queryService.DidNotReceive().InvalidateCache(Arg.Any<Datasource>());
  }

  [Fact]
  public async Task Handle_VraciError_KdyzGetDataModelsAsyncSelze()
  {
    // Arrange
    var (provider, importService, queryService) = BuildMocks();
    provider.GetDataModelsAsync(Arg.Any<CancellationToken>())
      .Returns(Task.FromException<IEnumerable<DataModelDTO>>(new InvalidOperationException("AVAPlace nedostupné.")));

    var handler = new ImportAllDataModelsHandler(provider, importService, queryService);

    // Act
    var result = await handler.Handle(new ImportAllDataModelsCommand(), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(ResultStatus.Error);
    await importService.DidNotReceive().ImportModelAsync(Arg.Any<DataModelDTO>(), Arg.Any<CancellationToken>());
  }
}
