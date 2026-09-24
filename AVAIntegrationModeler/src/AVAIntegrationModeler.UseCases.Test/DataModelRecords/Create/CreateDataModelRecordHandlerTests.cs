using Ardalis.Result;
using Ardalis.Specification;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate;
using AVAIntegrationModeler.UseCases.DataModelRecords;
using AVAIntegrationModeler.UseCases.DataModelRecords.Create;
using NSubstitute;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.DataModelRecords.Create;

/// <summary>
/// Testy pro upsert logiku v <see cref="CreateDataModelRecordHandler"/>.
/// </summary>
public class CreateDataModelRecordHandlerTests
{
  private readonly IDataModelRecordRepository _repository;
  private readonly IDataModelRecordQueryService _queryService;
  private readonly IDomainEntityValidationService<DataModelRecord> _validationService;
  private readonly CreateDataModelRecordHandler _handler;

  private static readonly Guid ModelId = Guid.NewGuid();

  public CreateDataModelRecordHandlerTests()
  {
    _repository = Substitute.For<IDataModelRecordRepository>();
    _queryService = Substitute.For<IDataModelRecordQueryService>();
    _validationService = Substitute.For<IDomainEntityValidationService<DataModelRecord>>();

    _validationService.ValidateForCreate(default, default!, default)
      .ReturnsForAnyArgs(Result.Success());
    _validationService.Validate(default, default!, default)
      .ReturnsForAnyArgs(Result.Success());

    _handler = new CreateDataModelRecordHandler(_repository, _queryService, _validationService);
  }

  private static DataModelRecordDTO MakeDto(Guid id, string externalId = "EXT-001", List<DataModelRecordFieldDTO>? fields = null) => new()
  {
    Id = id,
    ModelId = ModelId,
    ExternalId = externalId,
    Fields = fields ?? []
  };

  [Fact]
  public async Task Handle_WhenDatasourceNotDatabase_ReturnsInvalid()
  {
    var cmd = new CreateDataModelRecordCommand(Datasource.AVAPlace, MakeDto(Guid.NewGuid()));

    var result = await _handler.Handle(cmd, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Invalid);
  }

  [Fact]
  public async Task Handle_WhenRecordNotExists_CreatesNewRecord()
  {
    var id = Guid.NewGuid();
    var dto = MakeDto(id);
    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns((DataModelRecord?)null);
    _repository.AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>())
      .Returns(ci => ci.Arg<DataModelRecord>());

    var result = await _handler.Handle(new CreateDataModelRecordCommand(Datasource.Database, dto), CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.ShouldBe(id);
    await _repository.Received(1).AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>());
    await _repository.DidNotReceive().SyncFieldsAndSaveAsync(Arg.Any<DataModelRecord>(), Arg.Any<IList<DataModelRecordField>>(), Arg.Any<IList<DataModelRecordField>>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_WhenRecordAlreadyExists_UpdatesInsteadOfInsert()
  {
    var id = Guid.NewGuid();
    var existing = new DataModelRecord(id, ModelId);
    existing.SetExternalId("OLD");
    var dto = MakeDto(id, "NEW");

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(existing);

    var result = await _handler.Handle(new CreateDataModelRecordCommand(Datasource.Database, dto), CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.ShouldBe(id);
    existing.ExternalId.ShouldBe("NEW");
    await _repository.DidNotReceive().AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>());
    await _repository.Received(1).SyncFieldsAndSaveAsync(existing, Arg.Any<IList<DataModelRecordField>>(), Arg.Any<IList<DataModelRecordField>>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_WhenRecordAlreadyExists_ReplacesFields()
  {
    var id = Guid.NewGuid();
    var existing = new DataModelRecord(id, ModelId);
    existing.AddField(new DataModelRecordField(Guid.NewGuid(), "OldField"));
    var dto = MakeDto(id, fields: [new DataModelRecordFieldDTO { Key = "NewField", StringValue = "val" }]);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(existing);

    await _handler.Handle(new CreateDataModelRecordCommand(Datasource.Database, dto), CancellationToken.None);

    existing.Fields.ShouldContain(f => f.Key == "NewField");
    existing.Fields.ShouldNotContain(f => f.Key == "OldField");
  }

  [Fact]
  public async Task Handle_WhenRecordNotExists_InvalidatesCache()
  {
    var dto = MakeDto(Guid.NewGuid());
    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns((DataModelRecord?)null);
    _repository.AddAsync(Arg.Any<DataModelRecord>(), Arg.Any<CancellationToken>())
      .Returns(ci => ci.Arg<DataModelRecord>());

    await _handler.Handle(new CreateDataModelRecordCommand(Datasource.Database, dto), CancellationToken.None);

    _queryService.Received(1).InvalidateCache(Datasource.Database);
  }

  [Fact]
  public async Task Handle_WhenRecordAlreadyExists_InvalidatesCache()
  {
    var id = Guid.NewGuid();
    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModelRecord>>(), Arg.Any<CancellationToken>())
      .Returns(new DataModelRecord(id, ModelId));

    await _handler.Handle(new CreateDataModelRecordCommand(Datasource.Database, MakeDto(id)), CancellationToken.None);

    _queryService.Received(1).InvalidateCache(Datasource.Database);
  }
}
