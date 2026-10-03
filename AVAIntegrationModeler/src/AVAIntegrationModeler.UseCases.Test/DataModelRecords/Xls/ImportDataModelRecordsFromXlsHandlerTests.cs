using Ardalis.Result;
using Ardalis.Specification;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate;
using AVAIntegrationModeler.UseCases.DataModelRecords;
using AVAIntegrationModeler.UseCases.DataModelRecords.Xls;
using AVAIntegrationModeler.UseCases.DataModelRecords.Xls.ImportFromXls;
using NSubstitute;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.DataModelRecords.Xls;

/// <summary>
/// Testy pro <see cref="ImportDataModelRecordsFromXlsHandler"/>.
/// </summary>
public class ImportDataModelRecordsFromXlsHandlerTests
{
  private readonly IDataModelRecordRepository _repository;
  private readonly IDataModelRecordQueryService _queryService;
  private readonly IDomainEntityValidationService<DataModelRecord> _validationService;
  private readonly ImportDataModelRecordsFromXlsHandler _handler;

  private static readonly Guid ModelId = Guid.NewGuid();

  public ImportDataModelRecordsFromXlsHandlerTests()
  {
    _repository = Substitute.For<IDataModelRecordRepository>();
    _queryService = Substitute.For<IDataModelRecordQueryService>();
    _validationService = Substitute.For<IDomainEntityValidationService<DataModelRecord>>();

    _validationService.ValidateForCreate(default, default!, default)
      .ReturnsForAnyArgs(Result.Success());
    _validationService.Validate(default, default!, default)
      .ReturnsForAnyArgs(Result.Success());

    _handler = new ImportDataModelRecordsFromXlsHandler(_repository, _queryService, _validationService);
  }

  private static byte[] BuildXlsWith(params DataModelRecordDTO[] records) =>
    DataModelRecordXlsService.BuildXls(records);

  [Fact]
  public async Task Handle_WhenDatasourceNotDatabase_ReturnsInvalid()
  {
    var result = await _handler.Handle(
      new ImportDataModelRecordsFromXlsCommand(Datasource.AVAPlace, ModelId, []),
      CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Invalid);
  }

  [Fact]
  public async Task Handle_WithInvalidFile_ReturnsSuccessWithParseErrors()
  {
    var badBytes = "not xlsx"u8.ToArray();

    var result = await _handler.Handle(
      new ImportDataModelRecordsFromXlsCommand(Datasource.Database, ModelId, badBytes),
      CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.Errors.Count.ShouldBeGreaterThan(0);
    result.Value.CreatedCount.ShouldBe(0);
    result.Value.UpdatedCount.ShouldBe(0);
  }

  [Fact]
  public async Task Handle_NewRecord_GuidEmpty_CreatesNew()
  {
    var xlsx = BuildXlsWith(new DataModelRecordDTO
    {
      Id = Guid.Empty,
      ModelId = ModelId,
      ExternalId = "NEW-001",
      Fields = []
    });

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns((DataModelRecord?)null);
    _repository.AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>())
      .Returns(ci => ci.Arg<DataModelRecord>());

    var result = await _handler.Handle(
      new ImportDataModelRecordsFromXlsCommand(Datasource.Database, ModelId, xlsx),
      CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.CreatedCount.ShouldBe(1);
    result.Value.UpdatedCount.ShouldBe(0);
    result.Value.Errors.ShouldBeEmpty();
    await _repository.Received(1).AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ExistingRecord_Updates()
  {
    var existingId = Guid.NewGuid();
    var existing = new DataModelRecord(existingId, ModelId);
    existing.SetExternalId("OLD");

    var xlsx = BuildXlsWith(new DataModelRecordDTO
    {
      Id = existingId,
      ModelId = ModelId,
      ExternalId = "UPDATED",
      Fields = []
    });

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(existing);

    var result = await _handler.Handle(
      new ImportDataModelRecordsFromXlsCommand(Datasource.Database, ModelId, xlsx),
      CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.UpdatedCount.ShouldBe(1);
    result.Value.CreatedCount.ShouldBe(0);
    result.Value.Errors.ShouldBeEmpty();
    existing.ExternalId.ShouldBe("UPDATED");
    await _repository.Received(1).SyncFieldsAndSaveAsync(
      existing,
      Arg.Any<IList<DataModelRecordField>>(),
      Arg.Any<IList<DataModelRecordField>>(),
      Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_IdNotInDb_CreatesWithThatId()
  {
    var newId = Guid.NewGuid();
    var xlsx = BuildXlsWith(new DataModelRecordDTO
    {
      Id = newId,
      ModelId = ModelId,
      ExternalId = "EXT-NEW",
      Fields = []
    });

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns((DataModelRecord?)null);
    _repository.AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>())
      .Returns(ci => ci.Arg<DataModelRecord>());

    var result = await _handler.Handle(
      new ImportDataModelRecordsFromXlsCommand(Datasource.Database, ModelId, xlsx),
      CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.CreatedCount.ShouldBe(1);
    await _repository.Received(1).AddAsync(
      Arg.Is<DataModelRecord>(r => r.Id == newId),
      Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_MultipleRecords_CountsCorrectly()
  {
    var id1 = Guid.NewGuid();
    var existing = new DataModelRecord(id1, ModelId);
    var callCount = 0;

    var xlsx = BuildXlsWith(
      new DataModelRecordDTO { Id = id1, ModelId = ModelId, ExternalId = "EXT-1", Fields = [] },
      new DataModelRecordDTO { Id = Guid.Empty, ModelId = ModelId, ExternalId = "EXT-2", Fields = [] }
    );

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(_ => callCount++ == 0 ? existing : (DataModelRecord?)null);
    _repository.AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>())
      .Returns(ci => ci.Arg<DataModelRecord>());

    var result = await _handler.Handle(
      new ImportDataModelRecordsFromXlsCommand(Datasource.Database, ModelId, xlsx),
      CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    (result.Value.CreatedCount + result.Value.UpdatedCount).ShouldBe(2);
  }

  [Fact]
  public async Task Handle_WhenCreateFails_CollectsError()
  {
    var xlsx = BuildXlsWith(new DataModelRecordDTO
    {
      Id = Guid.Empty,
      ModelId = ModelId,
      ExternalId = "FAIL",
      Fields = []
    });

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns((DataModelRecord?)null);
    _repository.AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>())
      .Returns((DataModelRecord)null!); // Simuluje selhání uložení

    var result = await _handler.Handle(
      new ImportDataModelRecordsFromXlsCommand(Datasource.Database, ModelId, xlsx),
      CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.CreatedCount.ShouldBe(0);
    result.Value.Errors.Count.ShouldBe(1);
    result.Value.Errors[0].RowNumber.ShouldBe(2);
  }

  [Fact]
  public async Task Handle_WhenAnySuccess_InvalidatesCache()
  {
    var xlsx = BuildXlsWith(new DataModelRecordDTO
    {
      Id = Guid.Empty,
      ModelId = ModelId,
      ExternalId = "EXT-001",
      Fields = []
    });

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns((DataModelRecord?)null);
    _repository.AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>())
      .Returns(ci => ci.Arg<DataModelRecord>());

    await _handler.Handle(
      new ImportDataModelRecordsFromXlsCommand(Datasource.Database, ModelId, xlsx),
      CancellationToken.None);

    _queryService.Received(1).InvalidateCache(Datasource.Database);
  }

  [Fact]
  public async Task Handle_EmptyXls_ReturnsZeroCounts()
  {
    var xlsx = DataModelRecordXlsService.BuildXls([]);

    var result = await _handler.Handle(
      new ImportDataModelRecordsFromXlsCommand(Datasource.Database, ModelId, xlsx),
      CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.CreatedCount.ShouldBe(0);
    result.Value.UpdatedCount.ShouldBe(0);
    result.Value.Errors.ShouldBeEmpty();
  }
}
