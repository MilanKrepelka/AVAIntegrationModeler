using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModelRecords;
using AVAIntegrationModeler.UseCases.DataModelRecords.Xls.ExportToXls;
using AVAIntegrationModeler.UseCases.DataModels;
using NSubstitute;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.DataModelRecords.Xls;

/// <summary>
/// Testy pro <see cref="ExportDataModelRecordsToXlsHandler"/>.
/// </summary>
public class ExportDataModelRecordsToXlsHandlerTests
{
  private readonly IDataModelRecordQueryService _records;
  private readonly IDataModelQueryService _models;
  private readonly ExportDataModelRecordsToXlsHandler _handler;

  private static readonly Guid ModelId = Guid.NewGuid();

  public ExportDataModelRecordsToXlsHandlerTests()
  {
    _records = Substitute.For<IDataModelRecordQueryService>();
    _models = Substitute.For<IDataModelQueryService>();
    _handler = new ExportDataModelRecordsToXlsHandler(_records, _models);
  }

  [Fact]
  public async Task Handle_WhenDatasourceNotDatabase_ReturnsInvalid()
  {
    var result = await _handler.Handle(
      new ExportDataModelRecordsToXlsQuery(Datasource.AVAPlace, ModelId),
      CancellationToken.None);

    result.Status.ShouldBe(Ardalis.Result.ResultStatus.Invalid);
  }

  [Fact]
  public async Task Handle_WhenModelNotFound_ReturnsNotFound()
  {
    _models.ListAsync(Datasource.Database)
      .Returns(Task.FromResult<IEnumerable<DataModelDTO>>([]));

    var result = await _handler.Handle(
      new ExportDataModelRecordsToXlsQuery(Datasource.Database, ModelId),
      CancellationToken.None);

    result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
  }

  [Fact]
  public async Task Handle_WhenModelExists_ReturnsBytesAndFileName()
  {
    var model = new DataModelDTO { Id = ModelId, Code = "TEST-MODEL", Name = "Test Model" };
    _models.ListAsync(Datasource.Database)
      .Returns(Task.FromResult<IEnumerable<DataModelDTO>>([model]));
    _records.ListAsync(Datasource.Database, ModelId, cancellationToken: Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>([]));

    var result = await _handler.Handle(
      new ExportDataModelRecordsToXlsQuery(Datasource.Database, ModelId),
      CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.Content.ShouldNotBeNull();
    result.Value.Content.Length.ShouldBeGreaterThan(0);
    result.Value.FileName.ShouldStartWith("records-TEST-MODEL-");
    result.Value.FileName.ShouldEndWith(".xlsx");
  }

  [Fact]
  public async Task Handle_FileName_ContainsDateInEnglishFormat()
  {
    var model = new DataModelDTO { Id = ModelId, Code = "MYMODEL", Name = "My Model" };
    _models.ListAsync(Datasource.Database)
      .Returns(Task.FromResult<IEnumerable<DataModelDTO>>([model]));
    _records.ListAsync(Datasource.Database, ModelId, cancellationToken: Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>([]));

    var result = await _handler.Handle(
      new ExportDataModelRecordsToXlsQuery(Datasource.Database, ModelId),
      CancellationToken.None);

    // Datum musí být ve formátu yyyy-MM-dd (anglický formát)
    var datePart = DateTime.UtcNow.ToString("yyyy-MM-dd");
    result.Value.FileName.ShouldContain(datePart);
  }

  [Fact]
  public async Task Handle_QueriesBothModelAndRecords()
  {
    var model = new DataModelDTO { Id = ModelId, Code = "M", Name = "M" };
    _models.ListAsync(Datasource.Database)
      .Returns(Task.FromResult<IEnumerable<DataModelDTO>>([model]));
    _records.ListAsync(Datasource.Database, ModelId, cancellationToken: Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<IEnumerable<DataModelRecordDTO>>([]));

    await _handler.Handle(
      new ExportDataModelRecordsToXlsQuery(Datasource.Database, ModelId),
      CancellationToken.None);

    await _models.Received(1).ListAsync(Datasource.Database);
    await _records.Received(1).ListAsync(Datasource.Database, ModelId, cancellationToken: Arg.Any<CancellationToken>());
  }
}
