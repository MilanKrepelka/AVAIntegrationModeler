using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using Shouldly;

namespace AVAIntegrationModeler.Domain.Test.DataModelAggregate;

/// <summary>
/// Unit testy pro entitu DataModelField.
/// </summary>
public class DataModelFieldTests
{
  [Fact]
  public void Constructor_ShouldSetRequiredProperties()
  {
    // Arrange
    var id = Guid.NewGuid();
    var name = "TestField";
    var fieldType = DataModelFieldType.Text;

    // Act
    var field = new DataModelField(id, name, fieldType);

    // Assert
    field.Id.ShouldBe(id);
    field.Name.ShouldBe(name);
    field.FieldType.ShouldBe(fieldType);
  }

  [Fact]
  public void SetName_ShouldUpdateName()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Original", DataModelFieldType.Text);

    // Act
    field.SetName("Updated");

    // Assert
    field.Name.ShouldBe("Updated");
  }

  [Fact]
  public void MarkAsPublishedForLookup_ShouldSetFlagToTrue()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.Text);

    // Act
    field.MarkAsPublishedForLookup();

    // Assert
    field.IsPublishedForLookup.ShouldBeTrue();
  }

  [Fact]
  public void AddReferencedEntityType_ForLookupEntity_ShouldAddReference()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.LookupEntity);
    var entityId = Guid.NewGuid();

    // Act
    field.AddReferencedEntityType(entityId);

    // Assert
    field.ReferencedEntityTypeIds.ShouldContain(entityId);
  }

  [Fact]
  public void AddReferencedEntityType_ForNonReferenceType_ShouldThrowException()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.Text);

    // Act & Assert
    var exception = Should.Throw<InvalidOperationException>(() => 
      field.AddReferencedEntityType(Guid.NewGuid()));
    exception.Message.ShouldContain("LookupEntity or NestedEntity");
  }

  [Fact]
  public void SetExpression_ShouldSetValueAndOrder()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.Text);

    // Act
    field.SetExpression("A + B", 2);

    // Assert
    field.ExpressionValue.ShouldBe("A + B");
    field.ExpressionOrder.ShouldBe(2);
  }

  [Fact]
  public void SetExpression_WithEmptyValue_ShouldThrowException()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.Text);

    // Act & Assert
    Should.Throw<ArgumentException>(() => field.SetExpression(string.Empty, 1));
  }

  [Fact]
  public void ClearExpression_ShouldResetValueAndOrderToNull()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.Text)
      .SetExpression("A + B", 1);

    // Act
    field.ClearExpression();

    // Assert
    field.ExpressionValue.ShouldBeNull();
    field.ExpressionOrder.ShouldBeNull();
  }

  [Fact]
  public void FluentAPI_ShouldChainMethodCalls()
  {
    // Arrange
    var id = Guid.NewGuid();

    // Act
    var field = new DataModelField(id, "Field", DataModelFieldType.Text)
      .SetLabel("Label")
      .SetDescription("Description")
      .MarkAsNullable();

    // Assert
    field.Label.ShouldBe("Label");
    field.Description.ShouldBe("Description");
    field.IsNullable.ShouldBeTrue();
  }
}
