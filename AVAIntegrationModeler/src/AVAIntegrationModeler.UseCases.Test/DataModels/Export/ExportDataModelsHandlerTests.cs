using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Areas;
using AVAIntegrationModeler.UseCases.DataModelRecords;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.DataModels.Export;
using NSubstitute;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.DataModels.Export;

/// <summary>
/// Unit testy pro ExportDataModelsHandler — zejména řazení polí (Fields) v definičním
/// souboru abecedně dle Name, nezávisle na pořadí vráceném dotazovací službou.
/// </summary>
public class ExportDataModelsHandlerTests
{
  private static DataModelFieldDTO Field(string name) => new()
  {
    Name = name,
    FieldType = DataModelFieldType.Text,
    ReferencedEntityTypeIds = new List<Guid>()
  };

  private static (
    IDataModelQueryService Models,
    IDataModelRecordQueryService Records,
    IAreasQueryService Areas) BuildMocks(DataModelDTO model)
  {
    var models = Substitute.For<IDataModelQueryService>();
    var records = Substitute.For<IDataModelRecordQueryService>();
    var areas = Substitute.For<IAreasQueryService>();

    models.ListAsync(Datasource.Database).Returns(Task.FromResult<IEnumerable<DataModelDTO>>(new[] { model }));
    areas.ListAsync(Datasource.Database).Returns(Task.FromResult(Enumerable.Empty<AreaDTO>()));
    records.ListAsync(Datasource.Database, model.Id, cancellationToken: Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Enumerable.Empty<DataModelRecordDTO>()));

    return (models, records, areas);
  }

  [Fact]
  public async Task Handle_ShouldReturnFieldsSortedByName_RegardlessOfInputOrder()
  {
    // Arrange
    var model = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "MODEL-001",
      Name = "Model",
      Fields = new List<DataModelFieldDTO> { Field("Zebra"), Field("Apple"), Field("Mango") }
    };
    var (models, records, areas) = BuildMocks(model);
    var handler = new ExportDataModelsHandler(models, records, areas);

    // Act
    var result = await handler.Handle(new ExportDataModelsQuery(Datasource.Database, new List<Guid> { model.Id }), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    var entry = result.Value.Entries.Single();
    var exportedModel = (DataModelDTO)entry.Data;
    exportedModel.Fields.Select(f => f.Name).ShouldBe(new[] { "Apple", "Mango", "Zebra" });
  }

  [Fact]
  public async Task Handle_ShouldNotMutateOriginalFieldsOrder()
  {
    // Arrange — dotazovací služba může vracet sdílenou/cachovanou instanci DTO;
    // handler nesmí měnit pořadí polí v původním objektu.
    var originalFields = new List<DataModelFieldDTO> { Field("Zebra"), Field("Apple") };
    var model = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "MODEL-001",
      Name = "Model",
      Fields = originalFields
    };
    var (models, records, areas) = BuildMocks(model);
    var handler = new ExportDataModelsHandler(models, records, areas);

    // Act
    await handler.Handle(new ExportDataModelsQuery(Datasource.Database, new List<Guid> { model.Id }), CancellationToken.None);

    // Assert
    originalFields.Select(f => f.Name).ShouldBe(new[] { "Zebra", "Apple" });
  }
}
