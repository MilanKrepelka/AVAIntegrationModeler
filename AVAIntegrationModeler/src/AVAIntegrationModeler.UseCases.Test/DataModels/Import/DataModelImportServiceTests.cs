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
/// Unit testy pro DataModelImportService — upsert datového modelu podle Code
/// a jeho DataModelRecordů podle ExternalId.
/// </summary>
public class DataModelImportServiceTests
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
    IIntegrationDataProvider IntegrationDataProvider) BuildMocks()
  {
    var dataModelRepo = Substitute.For<IDataModelRepository>();
    var recordRepo = Substitute.For<IDataModelRecordRepository>();
    var provider = Substitute.For<IIntegrationDataProvider>();

    dataModelRepo.AddAsync(Arg.Any<DataModel>(), Arg.Any<CancellationToken>())
      .Returns(ci => Task.FromResult<DataModel>(ci.Arg<DataModel>()));

    recordRepo.AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>())
      .Returns(ci => Task.FromResult<DataModelRecord>(ci.Arg<DataModelRecord>()));

    recordRepo.ListAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(new List<DataModelRecord>()));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Enumerable.Empty<DataModelRecordDTO>()));

    return (dataModelRepo, recordRepo, provider);
  }

  #endregion

  #region ImportModelAsync — základní scénáře

  [Fact]
  public async Task ImportModelAsync_VytvoriNovyDataModel_KdyzNeexistujeModelSeStejnymKodem()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider) = BuildMocks();
    var dto = BuildModelDTO("NOVY-KOD");

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    var service = new DataModelImportService(dataModelRepo, recordRepo, provider);

    // Act
    var result = await service.ImportModelAsync(dto, CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    await dataModelRepo.Received(1).AddAsync(Arg.Any<DataModel>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task ImportModelAsync_AktualizujeExistujiciDataModel_KdyzModelSeStejnymKodemExistuje()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider) = BuildMocks();
    var dto = BuildModelDTO("EXISTUJICI-KOD");
    var existing = new DataModel(Guid.NewGuid(), "EXISTUJICI-KOD");

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(existing));

    var service = new DataModelImportService(dataModelRepo, recordRepo, provider);

    // Act
    var result = await service.ImportModelAsync(dto, CancellationToken.None);

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
  public async Task ImportModelAsync_VraciSpravneLocalModelId_PoUspesnomImportu()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider) = BuildMocks();
    var dto = BuildModelDTO();
    DataModel? capturedModel = null;

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    dataModelRepo.AddAsync(Arg.Any<DataModel>(), Arg.Any<CancellationToken>())
      .Returns(ci =>
      {
        capturedModel = ci.Arg<DataModel>();
        return Task.FromResult<DataModel>(capturedModel);
      });

    var service = new DataModelImportService(dataModelRepo, recordRepo, provider);

    // Act
    var result = await service.ImportModelAsync(dto, CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    capturedModel.ShouldNotBeNull();
    result.Value.ShouldBe(capturedModel!.Id);
  }

  [Fact]
  public async Task ImportModelAsync_VraciError_KdyzAddAsyncVratiNull()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider) = BuildMocks();
    var dto = BuildModelDTO();

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    dataModelRepo.AddAsync(Arg.Any<DataModel>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel>(null!));

    var service = new DataModelImportService(dataModelRepo, recordRepo, provider);

    // Act
    var result = await service.ImportModelAsync(dto, CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(ResultStatus.Error);
  }

  #endregion

  #region ImportModelAsync — import polí datového modelu

  [Fact]
  public async Task ImportModelAsync_SynchronizujePole_KdyzDTOMaPole()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider) = BuildMocks();
    var dto = BuildModelDTO();
    dto.Fields.Add(new DataModelFieldDTO
    {
      Id = Guid.NewGuid(),
      Name = "TestField",
      Label = "Test",
      FieldType = DataModelFieldType.Text,
      ReferencedEntityTypeIds = new List<Guid>()
    });

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    var service = new DataModelImportService(dataModelRepo, recordRepo, provider);

    // Act
    var result = await service.ImportModelAsync(dto, CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    await dataModelRepo.Received(1).SyncFieldsAndSaveAsync(
      Arg.Any<DataModel>(),
      Arg.Any<IList<DataModelField>>(),
      Arg.Is<IList<DataModelField>>(f => f.Count == 1),
      Arg.Any<CancellationToken>());
  }

  #endregion

  #region ImportModelAsync — záznamy datového modelu

  [Fact]
  public async Task ImportModelAsync_NevolajuRecordRepository_KdyzAVAPlaceNevraciZadneZaznamy()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider) = BuildMocks();
    var dto = BuildModelDTO();

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Enumerable.Empty<DataModelRecordDTO>()));

    var service = new DataModelImportService(dataModelRepo, recordRepo, provider);

    // Act
    await service.ImportModelAsync(dto, CancellationToken.None);

    // Assert
    await recordRepo.DidNotReceive().ListAsync(
      Arg.Any<ISpecification<DataModelRecord>>(),
      Arg.Any<CancellationToken>());
    await recordRepo.DidNotReceive().AddAsync(
      Arg.Any<DataModelRecord>(),
      Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task ImportModelAsync_VytvoriNovyZaznam_KdyzExternalIdNeniNalezeno()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider) = BuildMocks();
    var dto = BuildModelDTO();
    var recordDto = BuildRecordDTO("EXT-NOVY");

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>(new[] { recordDto }));

    recordRepo.ListAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(new List<DataModelRecord>()));

    var service = new DataModelImportService(dataModelRepo, recordRepo, provider);

    // Act
    var result = await service.ImportModelAsync(dto, CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    await recordRepo.Received(1).AddAsync(
      Arg.Is<DataModelRecord>(r => r.ExternalId == "EXT-NOVY"),
      Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task ImportModelAsync_AktualizujeExistujiciZaznam_KdyzExternalIdShoduje()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider) = BuildMocks();
    var dto = BuildModelDTO();
    var existingModel = new DataModel(Guid.NewGuid(), dto.Code);
    var existingRecord = new DataModelRecord(Guid.NewGuid(), existingModel.Id);
    existingRecord.SetExternalId("EXT-EXISTUJICI");

    var recordDto = BuildRecordDTO("EXT-EXISTUJICI");

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(existingModel));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>(new[] { recordDto }));

    recordRepo.ListAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(new List<DataModelRecord> { existingRecord }));

    var service = new DataModelImportService(dataModelRepo, recordRepo, provider);

    // Act
    var result = await service.ImportModelAsync(dto, CancellationToken.None);

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
  public async Task ImportModelAsync_SynchronizujePoleZaznamu_PriVytvoreniNovehoZaznamu()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider) = BuildMocks();
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

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>(new[] { recordDto }));

    recordRepo.ListAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(new List<DataModelRecord>()));

    var service = new DataModelImportService(dataModelRepo, recordRepo, provider);

    // Act
    var result = await service.ImportModelAsync(dto, CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    await recordRepo.Received(1).SyncFieldsAndSaveAsync(
      Arg.Any<DataModelRecord>(),
      Arg.Any<IList<DataModelRecordField>>(),
      Arg.Is<IList<DataModelRecordField>>(f => f.Count == 2),
      Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task ImportModelAsync_VolaGetDataModelRecordsAsync_SeSpravnymAvaPlaceModelId()
  {
    // Arrange — dto.Id (AVAPlace GUID modelu) musí být použito pro dotaz na záznamy
    var (dataModelRepo, recordRepo, provider) = BuildMocks();
    var dto = BuildModelDTO();

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    var service = new DataModelImportService(dataModelRepo, recordRepo, provider);

    // Act
    await service.ImportModelAsync(dto, CancellationToken.None);

    // Assert
    await provider.Received(1).GetDataModelRecordsAsync(dto.Id, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task ImportModelAsync_ZpracujeViceZaznamu_KdyzAVAPlaceVratiViceZaznamu()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider) = BuildMocks();
    var dto = BuildModelDTO();
    var records = new[]
    {
      BuildRecordDTO("EXT-01"),
      BuildRecordDTO("EXT-02"),
      BuildRecordDTO("EXT-03")
    };

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>(records));

    recordRepo.ListAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(new List<DataModelRecord>()));

    var service = new DataModelImportService(dataModelRepo, recordRepo, provider);

    // Act
    var result = await service.ImportModelAsync(dto, CancellationToken.None);

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
  public async Task ImportModelAsync_VolaAddAsyncSePrazdnymZaznamem_NezPridaPole()
  {
    // Arrange
    var (dataModelRepo, recordRepo, provider) = BuildMocks();
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

    dataModelRepo.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(null));

    provider.GetDataModelRecordsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>(new[] { recordDto }));

    recordRepo.ListAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(new List<DataModelRecord>()));

    // Zachytit počet polí V MOMENTĚ volání AddAsync — před opravou by bylo 2,
    // po opravě musí být 0 (pole se přidávají teprve přes SyncFieldsAndSaveAsync).
    int fieldsCountAtAddAsync = -1;
    recordRepo.AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>())
      .Returns(ci =>
      {
        var record = ci.Arg<DataModelRecord>();
        fieldsCountAtAddAsync = record.Fields.Count;
        return Task.FromResult<DataModelRecord>(record);
      });

    var service = new DataModelImportService(dataModelRepo, recordRepo, provider);

    // Act
    await service.ImportModelAsync(dto, CancellationToken.None);

    // Assert — při volání AddAsync nesmí mít záznam žádná pole;
    // pokud by měl, EF by je uložil v AddAsync a SyncFieldsAndSaveAsync by způsobilo
    // UNIQUE constraint failure při druhém pokusu o INSERT stejných Id.
    fieldsCountAtAddAsync.ShouldBe(0,
      "DataModelRecord musí být prázdný při volání AddAsync — pole se přidávají až po uložení záznamu.");
  }

  [Fact]
  public async Task ImportModelAsync_VolaSyncFieldsAndSaveAsync_AzPoAddAsync()
  {
    // Arrange — ověřuje pořadí operací: AddAsync → SyncFieldsAndSaveAsync
    var (dataModelRepo, recordRepo, provider) = BuildMocks();
    var dto = BuildModelDTO();
    var recordDto = BuildRecordDTO("EXT-PORADI");

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

    var service = new DataModelImportService(dataModelRepo, recordRepo, provider);

    // Act
    await service.ImportModelAsync(dto, CancellationToken.None);

    // Assert — AddAsync musí předcházet SyncFieldsAndSaveAsync
    callOrder.Count.ShouldBe(2);
    callOrder[0].ShouldBe("AddAsync");
    callOrder[1].ShouldBe("SyncFieldsAndSaveAsync");
  }

  [Fact]
  public async Task ImportModelAsync_SyncFieldsAndSaveAsync_DostavaNovaPolaNikoli_InstanceZEntityPredAddAsync()
  {
    // Arrange — ověřuje, že pole předaná SyncFieldsAndSaveAsync jsou nové instance
    // vytvořené po AddAsync, nikoliv ty, které by byly přidány do entity před AddAsync.
    var (dataModelRepo, recordRepo, provider) = BuildMocks();
    var dto = BuildModelDTO();
    var recordDto = BuildRecordDTO("EXT-INSTANCE");
    recordDto.Fields.Add(new DataModelRecordFieldDTO { Key = "DalsiPole", IsLocalized = false, StringValue = "X" });

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

    var service = new DataModelImportService(dataModelRepo, recordRepo, provider);

    // Act
    await service.ImportModelAsync(dto, CancellationToken.None);

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
