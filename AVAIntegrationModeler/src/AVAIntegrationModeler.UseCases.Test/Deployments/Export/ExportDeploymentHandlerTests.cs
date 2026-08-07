using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Areas;
using AVAIntegrationModeler.UseCases.DataModelRecords;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.Deployments;
using AVAIntegrationModeler.UseCases.Deployments.Export;
using NSubstitute;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.Deployments.Export;

/// <summary>
/// Unit testy pro ExportDeploymentHandler — zejména řazení polí (Fields) v definičním
/// souboru abecedně dle Name, nezávisle na pořadí vráceném dotazovací službou.
/// </summary>
public class ExportDeploymentHandlerTests
{
  private static DataModelFieldDTO Field(string name) => new()
  {
    Name = name,
    FieldType = DataModelFieldType.Text,
    ReferencedEntityTypeIds = new List<Guid>()
  };

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

    var deployment = new DeploymentDTO
    {
      Id = Guid.NewGuid(),
      Code = "DEPLOY-001",
      Name = "Deployment",
      DataModelIds = new List<Guid> { model.Id }
    };

    var deployments = Substitute.For<IDeploymentsQueryService>();
    var models = Substitute.For<IDataModelQueryService>();
    var records = Substitute.For<IDataModelRecordQueryService>();
    var areas = Substitute.For<IAreasQueryService>();

    deployments.GetDeployment(deployment.Code, Arg.Any<CancellationToken>()).Returns(Task.FromResult(deployment));
    models.ListAsync(Datasource.Database).Returns(Task.FromResult<IEnumerable<DataModelDTO>>(new[] { model }));
    areas.ListAsync(Datasource.Database).Returns(Task.FromResult(Enumerable.Empty<AreaDTO>()));
    records.ListAsync(Datasource.Database, model.Id, cancellationToken: Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Enumerable.Empty<DataModelRecordDTO>()));

    var handler = new ExportDeploymentHandler(deployments, models, records, areas);

    // Act
    var result = await handler.Handle(new ExportDeploymentQuery(deployment.Code), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    var entry = result.Value.Entries.Single();
    var exportedModel = (DataModelDTO)entry.Data;
    exportedModel.Fields.Select(f => f.Name).ShouldBe(new[] { "Apple", "Mango", "Zebra" });
  }
}
