using Ardalis.Result;
using Ardalis.Specification;
using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate;
using AVAIntegrationModeler.UseCases.DataModelRecords;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.DataModels.Import;
using NSubstitute;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.DataModels.Import;

/// <summary>
/// Unit testy pro ImportDataModelHandler.
/// </summary>
public class ImportDataModelHandlerTests
{
  #region Pomocné metody

  private static DataModelDTO BuildModelDTO(string code = "MODEL-01", string name = "Test model") =>
    new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = code,
      Name = name,
      Description = "Popis",
      Notes = string.Empty,
      IsAggregateRoot = true,
      AreaId = null,
      Fields = new List<DataModelFieldDTO>()
    };

  private static DataModelRecordDTO BuildRecordDTO(string externalId = "EXT-01") =>
    new DataModelRecordDTO
    {
      Id = Guid.NewGuid(),
      ModelId = Guid.NewGuid(),
      ExternalId = externalId,
      Fields = new List<DataModelRecordFieldDTO>
      {
        new DataModelRecordFieldDTO { Key = "Code", IsLocalized = false, StringValue = "hodnota" }
      }
    };

  private static (
    IDataModelRepository DataModelRepo,
    IDataModelRecordRepository RecordRepo,
    IIntegrationDataProvider IntegrationDataProvider,
    IDataModelQueryService QueryService) BuildMocks()
  {
    var dataModelRepo = Substitute.For<IDataModelRepository>();
    var recordRepo = Substitute.For<IDataModelRecordRepository>();
    var provider = Substitute.For<IIntegrationDataProvider>();
    var queryService = Substitute.For<IDataModelQueryService>();

    dataModelRepo.AddAsync(Arg.Any<DataModel>(), Arg.Any<CancellationToken>())
      .Returns(ci => Task.FromResult<DataModel>(ci.Arg<DataModel>()));

    recordRepo.AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>())
      .Returns(ci => Task.FromResult<DataModelRecord>(ci.Arg<DataModelRecord>()));

    recordRepo.ListAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(new List<DataModelRecord>()));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Enumerable.Empty<DataModelRecordDTO>()));

    return (dataModelRepo, recordRepo, provider, queryService);
  }

  #endregion

  #region Handle — základní scénáře

  [Fact]
  public async Task Handle_ReturnsNotFound_KdyzModelNeniNalezenVAVAPlace()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(null));

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    var result = await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    result.Status.ShouldBe(ResultStatus.NotFound);
  }

  [Fact]
  public async Task Handle_VytvoriNovyDataModel_KdyzNeexistujeModelSeStejnymKodem()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO("NOVY-KOD");

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    var result = await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    await dataModelRepo.Received(1).AddAsync(Arg.Any<DataModel>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_AktualizujeExistujiciDataModel_KdyzModelSeStejnymKodemExistuje()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO("EXISTUJICI-KOD");
    var existing = new DataModel(Guid.NewGuid(), "EXISTUJICI-KOD");

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(existing));

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    var result = await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    result.Value.ShouldBe(existing.Id);
    await dataModelRepo.DidNotReceive().AddAsync(Arg.Any<DataModel>(), Arg.Any<CancellationToken>());
    await dataModelRepo.Received(1).SyncFieldsAndSaveAsync(
      existing,
      Arg.Any<IList<DataModelField>>(),
      Arg.Any<IList<DataModelField>>(),
      Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_VraciSpravneLocalModelId_PoUspesnomImportu()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO();
    var avaPlaceModelId = Guid.NewGuid();
    DataModel? capturedModel = null;

    provider.GetDataModelByIdAsync(avaPlaceModelId, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    dataModelRepo.AddAsync(Arg.Any<DataModel>(), Arg.Any<CancellationToken>())
      .Returns(ci =>
      {
        capturedModel = ci.Arg<DataModel>();
        return Task.FromResult<DataModel>(capturedModel);
      });

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    var result = await handler.Handle(new ImportDataModelCommand(avaPlaceModelId), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    capturedModel.ShouldNotBeNull();
    result.Value.ShouldBe(capturedModel!.Id);
  }

  [Fact]
  public async Task Handle_VraciError_KdyzAddAsyncVratiNull()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO();

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    dataModelRepo.AddAsync(Arg.Any<DataModel>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel>(null!));

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    var result = await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(ResultStatus.Error);
  }

  [Fact]
  public async Task Handle_InvalidujeCache_PoUspesnomImportu()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO();

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    queryService.Received(1).InvalidateCache(Datasource.Database);
  }

  [Fact]
  public async Task Handle_NevolajuCache_KdyzImportSelhal()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(null));

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    queryService.DidNotReceive().InvalidateCache(Arg.Any<Datasource>());
  }

  #endregion

  #region Handle — import polí datového modelu

  [Fact]
  public async Task Handle_SynchronizujePole_KdyzDTOMaPole()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO();
    dto.Fields.Add(new DataModelFieldDTO
    {
      Id = Guid.NewGuid(),
      Name = "TestField",
      Label = "Test",
      FieldType = DataModelFieldType.Text,
      ReferencedEntityTypeIds = new List<Guid>()
    });

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    var result = await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    await dataModelRepo.Received(1).SyncFieldsAndSaveAsync(
      Arg.Any<DataModel>(),
      Arg.Any<IList<DataModelField>>(),
      Arg.Is<IList<DataModelField>>(f => f.Count == 1),
      Arg.Any<CancellationToken>());
  }

  #endregion

  #region ImportRecords — záznamy datového modelu

  [Fact]
  public async Task Handle_NevolajuRecordRepository_KdyzAVAPlaceNevraciZadneZaznamy()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO();

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Enumerable.Empty<DataModelRecordDTO>()));

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    await recordRepo.DidNotReceive().ListAsync(
      Arg.Any<ISpecification<DataModelRecord>>(),
      Arg.Any<CancellationToken>());
    await recordRepo.DidNotReceive().AddAsync(
      Arg.Any<DataModelRecord>(),
      Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_VytvoriNovyZaznam_KdyzExternalIdNeniNalezeno()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO();
    var recordDto = BuildRecordDTO("EXT-NOVY");

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>(new[] { recordDto }));

    recordRepo.ListAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(new List<DataModelRecord>()));

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    var result = await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    await recordRepo.Received(1).AddAsync(
      Arg.Is<DataModelRecord>(r => r.ExternalId == "EXT-NOVY"),
      Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_AktualizujeExistujiciZaznam_KdyzExternalIdShoduje()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO();
    var localModelId = Guid.NewGuid();
    var existingRecord = new DataModelRecord(Guid.NewGuid(), localModelId);
    existingRecord.SetExternalId("EXT-EXISTUJICI");

    var recordDto = BuildRecordDTO("EXT-EXISTUJICI");

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    // Existující model se stejným ID
    var existingModel = new DataModel(localModelId, dto.Code);
    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(existingModel));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>(new[] { recordDto }));

    recordRepo.ListAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(new List<DataModelRecord> { existingRecord }));

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    var result = await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    await recordRepo.DidNotReceive().AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>());
    await recordRepo.Received(1).SyncFieldsAndSaveAsync(
      existingRecord,
      Arg.Any<IList<DataModelRecordField>>(),
      Arg.Any<IList<DataModelRecordField>>(),
      Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_SynchronizujePoleZaznamu_PriVytvoreniNovehoZaznamu()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO();
    var recordDto = new DataModelRecordDTO
    {
      Id = Guid.NewGuid(),
      ModelId = Guid.NewGuid(),
      ExternalId = "EXT-POLE",
      Fields = new List<DataModelRecordFieldDTO>
      {
        new DataModelRecordFieldDTO { Key = "Nazev", IsLocalized = true, CzechValue = "Česky", EnglishValue = "English" },
        new DataModelRecordFieldDTO { Key = "Kod", IsLocalized = false, StringValue = "KOD-123" }
      }
    };

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>(new[] { recordDto }));

    recordRepo.ListAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(new List<DataModelRecord>()));

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    var result = await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    await recordRepo.Received(1).SyncFieldsAndSaveAsync(
      Arg.Any<DataModelRecord>(),
      Arg.Any<IList<DataModelRecordField>>(),
      Arg.Is<IList<DataModelRecordField>>(f => f.Count == 2),
      Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_VolaGetDataModelRecordsAsync_SeSpravaymAvaPlaceModelId()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO();
    var avaPlaceModelId = Guid.NewGuid();

    provider.GetDataModelByIdAsync(avaPlaceModelId, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    await handler.Handle(new ImportDataModelCommand(avaPlaceModelId), CancellationToken.None);

    // Assert
    await provider.Received(1).GetDataModelRecordsAsync(avaPlaceModelId, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ZpracujeViceZaznamu_KdyzAVAPlaceVratiViceZaznamu()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO();
    var records = new[]
    {
      BuildRecordDTO("EXT-01"),
      BuildRecordDTO("EXT-02"),
      BuildRecordDTO("EXT-03")
    };

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>(records));

    recordRepo.ListAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(new List<DataModelRecord>()));

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    var result = await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    await recordRepo.Received(3).AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>());
  }

  #endregion

  #region Regrese — UNIQUE constraint bug (AddAsync musí předcházet BuildRecordFields)

  /// <summary>
  /// Regresní test pro chybu "UNIQUE constraint failed: DataModelRecordFields.Id".
  /// Příčina: BuildRecordFields bylo voláno PŘED AddAsync, takže EF Core uložil pole
  /// jako součást grafu agregátu v AddAsync. Následné SyncFieldsAndSaveAsync pak znovu
  /// volalo AddRange na stejné instance → druhý INSERT → constraint violation.
  /// Oprava: AddAsync se volá na prázdném záznamu; BuildRecordFields až po něm.
  /// </summary>
  [Fact]
  public async Task Handle_VolaAddAsyncSePrazdnymZaznamem_NezPridaPole()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO();
    var recordDto = new DataModelRecordDTO
    {
      Id = Guid.NewGuid(),
      ModelId = Guid.NewGuid(),
      ExternalId = "EXT-REGRESE",
      Fields = new List<DataModelRecordFieldDTO>
      {
        new DataModelRecordFieldDTO { Key = "Kod", IsLocalized = false, StringValue = "KOD-1" },
        new DataModelRecordFieldDTO { Key = "Nazev", IsLocalized = true, CzechValue = "Česky", EnglishValue = "English" }
      }
    };

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>(new[] { recordDto }));

    recordRepo.ListAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(new List<DataModelRecord>()));

    // Zachytit počet polí V MOMENTĚ volání AddAsync — před opravou by bylo 2,
    // po opravě musí být 0 (pole se přidávají teprve přes SyncFieldsAndSaveAsync).
    // Poznámka: nelze testovat Fields.Count po skončení Handle — vrácená instance
    // je stejný objekt jako 'created', který pole dostane po AddAsync přes BuildRecordFields.
    int fieldsCountAtAddAsync = -1;
    recordRepo.AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>())
      .Returns(ci =>
      {
        var record = ci.Arg<DataModelRecord>();
        fieldsCountAtAddAsync = record.Fields.Count;
        return Task.FromResult<DataModelRecord>(record);
      });

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert — při volání AddAsync nesmí mít záznam žádná pole;
    // pokud by měl, EF by je uložil v AddAsync a SyncFieldsAndSaveAsync by způsobilo
    // UNIQUE constraint failure při druhém pokusu o INSERT stejných Id.
    fieldsCountAtAddAsync.ShouldBe(0,
      "DataModelRecord musí být prázdný při volání AddAsync — pole se přidávají až po uložení záznamu.");
  }

  [Fact]
  public async Task Handle_VolaSyncFieldsAndSaveAsync_AzPoAddAsync()
  {
    // Arrange — ověřuje pořadí operací: AddAsync → SyncFieldsAndSaveAsync
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO();
    var recordDto = BuildRecordDTO("EXT-PORADI");

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>(new[] { recordDto }));

    recordRepo.ListAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(new List<DataModelRecord>()));

    var callOrder = new List<string>();

    recordRepo.AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>())
      .Returns(ci =>
      {
        callOrder.Add("AddAsync");
        return Task.FromResult<DataModelRecord>(ci.Arg<DataModelRecord>());
      });

    recordRepo.SyncFieldsAndSaveAsync(
        Arg.Any<DataModelRecord>(),
        Arg.Any<IList<DataModelRecordField>>(),
        Arg.Any<IList<DataModelRecordField>>(),
        Arg.Any<CancellationToken>())
      .Returns(ci =>
      {
        callOrder.Add("SyncFieldsAndSaveAsync");
        return Task.CompletedTask;
      });

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert — AddAsync musí předcházet SyncFieldsAndSaveAsync
    callOrder.Count.ShouldBe(2);
    callOrder[0].ShouldBe("AddAsync");
    callOrder[1].ShouldBe("SyncFieldsAndSaveAsync");
  }

  [Fact]
  public async Task Handle_SyncFieldsAndSaveAsync_DostavaNovaPolaNikoli_InstanceZEntityPredAddAsync()
  {
    // Arrange — ověřuje, že pole předaná SyncFieldsAndSaveAsync jsou nové instance
    // vytvořené po AddAsync, nikoliv ty, které by byly přidány do entity před AddAsync.
    var (dataModelRepo, recordRepo, provider, queryService) = BuildMocks();
    var dto = BuildModelDTO();
    var recordDto = BuildRecordDTO("EXT-INSTANCE");
    recordDto.Fields.Add(new DataModelRecordFieldDTO { Key = "DalsiPole", IsLocalized = false, StringValue = "X" });

    provider.GetDataModelByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModelDTO?>(dto));

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>(new[] { recordDto }));

    recordRepo.ListAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(new List<DataModelRecord>()));

    IList<DataModelRecordField>? capturedFieldsToAdd = null;
    int fieldsCountAtAddAsync = -1;

    recordRepo.AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>())
      .Returns(ci =>
      {
        var record = ci.Arg<DataModelRecord>();
        fieldsCountAtAddAsync = record.Fields.Count;
        return Task.FromResult<DataModelRecord>(record);
      });

    recordRepo.SyncFieldsAndSaveAsync(
        Arg.Any<DataModelRecord>(),
        Arg.Any<IList<DataModelRecordField>>(),
        Arg.Any<IList<DataModelRecordField>>(),
        Arg.Any<CancellationToken>())
      .Returns(ci =>
      {
        capturedFieldsToAdd = ci.ArgAt<IList<DataModelRecordField>>(2);
        return Task.CompletedTask;
      });

    var handler = new ImportDataModelHandler(dataModelRepo, recordRepo, provider, queryService);

    // Act
    await handler.Handle(new ImportDataModelCommand(Guid.NewGuid()), CancellationToken.None);

    // Assert — při AddAsync musí být entita prázdná; pole přijdou až přes SyncFieldsAndSaveAsync
    fieldsCountAtAddAsync.ShouldBe(0,
      "DataModelRecord musí být prázdný při AddAsync — jinak EF uloží pole dvakrát (UNIQUE constraint).");

    capturedFieldsToAdd.ShouldNotBeNull();
    // recordDto má 2 pole (BuildRecordDTO vrátí 1 pole "Code" + přidáno "DalsiPole")
    capturedFieldsToAdd!.Count.ShouldBe(2,
      "SyncFieldsAndSaveAsync musí dostat všechna pole z DTO.");
  }

  #endregion
}
