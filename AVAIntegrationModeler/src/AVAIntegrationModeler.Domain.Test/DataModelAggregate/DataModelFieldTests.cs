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
