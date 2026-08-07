using Ardalis.Result;
using Ardalis.Specification;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.DataModels.Update;
using NSubstitute;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.DataModels.Update;

/// <summary>
/// Unit testy pro UpdateDataModelHandler — zejména regresní test na mapování
/// Expression u polí (handler pole vytváří manuálně, nikoli přes DataModelFieldMapper).
/// </summary>
public class UpdateDataModelHandlerTests
{
  private static (
    IDataModelRepository Repository,
    IDataModelQueryService QueryService,
    IDomainEntityValidationService<DataModel> Validator) BuildMocks(DataModel existing)
  {
    var repository = Substitute.For<IDataModelRepository>();
    var queryService = Substitute.For<IDataModelQueryService>();
    var validator = Substitute.For<IDomainEntityValidationService<DataModel>>();

    repository.FirstOrDefaultAsync(Arg.Any<ISpecification<DataModel>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<DataModel?>(existing));

    validator.Validate(Arg.Any<Datasource>(), Arg.Any<DataModel>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(Result.Success()));

    return (repository, queryService, validator);
  }

  [Fact]
  public async Task Handle_ShouldPersistExpression_WhenFieldDtoHasExpression()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var existing = new DataModel(modelId, "MODEL-001");
    var (repository, queryService, validator) = BuildMocks(existing);

    IList<DataModelField>? capturedFieldsToAdd = null;
    repository.SyncFieldsAndSaveAsync(
        Arg.Any<DataModel>(),
        Arg.Any<IList<DataModelField>>(),
        Arg.Any<IList<DataModelField>>(),
        Arg.Any<CancellationToken>())
      .Returns(ci =>
      {
        capturedFieldsToAdd = ci.ArgAt<IList<DataModelField>>(2);
        return Task.CompletedTask;
      });

    var dto = new DataModelDTO
    {
      Id = modelId,
      Code = "MODEL-001",
      Name = "Test",
      Fields = new List<DataModelFieldDTO>
      {
        new DataModelFieldDTO
        {
          Name = "Computed",
          FieldType = DataModelFieldType.Text,
          Expression = new DataModelFieldExpressionDTO { Value = "A + B", Order = 2 },
          ReferencedEntityTypeIds = new List<Guid>()
        }
      }
    };

    var handler = new UpdateDataModelHandler(repository, queryService, validator);

    // Act
    var result = await handler.Handle(new UpdateDataModelCommand(Datasource.Database, dto), CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    capturedFieldsToAdd.ShouldNotBeNull();
    capturedFieldsToAdd!.Count.ShouldBe(1);
    capturedFieldsToAdd[0].ExpressionValue.ShouldBe("A + B");
    capturedFieldsToAdd[0].ExpressionOrder.ShouldBe(2);
  }

  [Fact]
  public async Task Handle_ShouldLeaveExpressionNull_WhenFieldDtoHasNoExpression()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var existing = new DataModel(modelId, "MODEL-001");
    var (repository, queryService, validator) = BuildMocks(existing);

    IList<DataModelField>? capturedFieldsToAdd = null;
    repository.SyncFieldsAndSaveAsync(
        Arg.Any<DataModel>(),
        Arg.Any<IList<DataModelField>>(),
        Arg.Any<IList<DataModelField>>(),
        Arg.Any<CancellationToken>())
      .Returns(ci =>
      {
        capturedFieldsToAdd = ci.ArgAt<IList<DataModelField>>(2);
        return Task.CompletedTask;
      });

    var dto = new DataModelDTO
    {
      Id = modelId,
      Code = "MODEL-001",
      Name = "Test",
      Fields = new List<DataModelFieldDTO>
      {
        new DataModelFieldDTO
        {
          Name = "Plain",
          FieldType = DataModelFieldType.Text,
          ReferencedEntityTypeIds = new List<Guid>()
        }
      }
    };

    var handler = new UpdateDataModelHandler(repository, queryService, validator);

    // Act
    await handler.Handle(new UpdateDataModelCommand(Datasource.Database, dto), CancellationToken.None);

    // Assert
    capturedFieldsToAdd.ShouldNotBeNull();
    capturedFieldsToAdd![0].ExpressionValue.ShouldBeNull();
    capturedFieldsToAdd[0].ExpressionOrder.ShouldBeNull();
  }
}
