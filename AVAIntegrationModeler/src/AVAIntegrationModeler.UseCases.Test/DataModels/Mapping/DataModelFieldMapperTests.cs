using System;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.UseCases.DataModels.Mapping;
using Shouldly;
using Xunit;

namespace AVAIntegrationModeler.UseCases.Test.DataModels.Mapping;

/// <summary>
/// Unit testy pro DataModelFieldMapper — mapování výrazu (Expression) počítaného pole.
/// </summary>
public class DataModelFieldMapperTests
{
  [Fact]
  public void MapToDataModelFieldDTO_ShouldMapExpression_WhenFieldHasExpression()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Computed", DataModelFieldType.Text)
      .SetExpression("A + B", 3);

    // Act
    var result = DataModelFieldMapper.MapToDataModelFieldDTO(field);

    // Assert
    result.Expression.ShouldNotBeNull();
    result.Expression!.Value.ShouldBe("A + B");
    result.Expression!.Order.ShouldBe(3);
  }

  [Fact]
  public void MapToDataModelFieldDTO_ShouldReturnNullExpression_WhenFieldHasNoExpression()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Plain", DataModelFieldType.Text);

    // Act
    var result = DataModelFieldMapper.MapToDataModelFieldDTO(field);

    // Assert
    result.Expression.ShouldBeNull();
  }

  [Fact]
  public void MapToEntity_ShouldSetExpression_WhenDtoHasExpression()
  {
    // Arrange
    var dto = new DataModelFieldDTO
    {
      Id = Guid.NewGuid(),
      Name = "Computed",
      FieldType = DataModelFieldType.Text,
      Expression = new DataModelFieldExpressionDTO { Value = "A + B", Order = 5 }
    };

    // Act
    var result = DataModelFieldMapper.MapToEntity(dto);

    // Assert
    result.ShouldNotBeNull();
    result!.ExpressionValue.ShouldBe("A + B");
    result.ExpressionOrder.ShouldBe(5);
  }

  [Fact]
  public void MapToEntity_ShouldLeaveExpressionNull_WhenDtoHasNoExpression()
  {
    // Arrange
    var dto = new DataModelFieldDTO
    {
      Id = Guid.NewGuid(),
      Name = "Plain",
      FieldType = DataModelFieldType.Text
    };

    // Act
    var result = DataModelFieldMapper.MapToEntity(dto);

    // Assert
    result.ShouldNotBeNull();
    result!.ExpressionValue.ShouldBeNull();
    result.ExpressionOrder.ShouldBeNull();
  }
}
