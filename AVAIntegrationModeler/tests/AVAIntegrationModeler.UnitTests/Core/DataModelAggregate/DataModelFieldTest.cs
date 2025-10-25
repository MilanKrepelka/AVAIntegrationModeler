using System;
using AVAIntegrationModeler.Core.DataModelAggregate;
using Shouldly;
using Xunit;

namespace AVAIntegrationModeler.UnitTests.Core.DataModelAggregate;

/// <summary>
/// Unit testy pro entitu DataModelField.
/// </summary>
public class DataModelFieldTest
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
    var newName = "UpdatedName";

    // Act
    field.SetName(newName);

    // Assert
    field.Name.ShouldBe(newName);
  }

  [Fact]
  public void SetName_WithNullOrEmpty_ShouldThrowException()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Original", DataModelFieldType.Text);

    // Act & Assert
    Should.Throw<ArgumentException>(() => field.SetName(null!));
    Should.Throw<ArgumentException>(() => field.SetName(string.Empty));
  }

  [Fact]
  public void SetLabel_ShouldUpdateLabel()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.Text);
    var label = "Human Readable Label";

    // Act
    field.SetLabel(label);

    // Assert
    field.Label.ShouldBe(label);
  }

  [Fact]
  public void SetDescription_ShouldUpdateDescription()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.Text);
    var description = "Field description";

    // Act
    field.SetDescription(description);

    // Assert
    field.Description.ShouldBe(description);
  }

  [Fact]
  public void SetFieldType_ShouldUpdateFieldType()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.Text);

    // Act
    field.SetFieldType(DataModelFieldType.WholeNumber);

    // Assert
    field.FieldType.ShouldBe(DataModelFieldType.WholeNumber);
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
  public void MarkAsCollection_ShouldSetFlagToTrue()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.Text);

    // Act
    field.MarkAsCollection();

    // Assert
    field.IsCollection.ShouldBeTrue();
  }

  [Fact]
  public void MarkAsLocalized_ShouldSetFlagToTrue()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.Text);

    // Act
    field.MarkAsLocalized();

    // Assert
    field.IsLocalized.ShouldBeTrue();
  }

  [Fact]
  public void MarkAsNullable_ShouldSetFlagToTrue()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.Text);

    // Act
    field.MarkAsNullable();

    // Assert
    field.IsNullable.ShouldBeTrue();
  }

  [Fact]
  public void AddReferencedEntityType_ForLookupEntity_ShouldAddReference()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.LookupEntity);
    var entityTypeId = Guid.NewGuid();

    // Act
    field.AddReferencedEntityType(entityTypeId);

    // Assert
    field.ReferencedEntityTypeIds.ShouldContain(entityTypeId);
    field.ReferencedEntityTypeIds.Count.ShouldBe(1);
  }

  [Fact]
  public void AddReferencedEntityType_ForNestedEntity_ShouldAddReference()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.NestedEntity);
    var entityTypeId = Guid.NewGuid();

    // Act
    field.AddReferencedEntityType(entityTypeId);

    // Assert
    field.ReferencedEntityTypeIds.ShouldContain(entityTypeId);
  }

  [Fact]
  public void AddReferencedEntityType_ForNonReferenceType_ShouldThrowException()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.Text);
    var entityTypeId = Guid.NewGuid();

    // Act & Assert
    var exception = Should.Throw<InvalidOperationException>(() => 
      field.AddReferencedEntityType(entityTypeId));
    
    exception.Message.ShouldContain("LookupEntity or NestedEntity");
  }

  [Fact]
  public void AddReferencedEntityType_ShouldNotAddDuplicates()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.LookupEntity);
    var entityTypeId = Guid.NewGuid();

    // Act
    field.AddReferencedEntityType(entityTypeId);
    field.AddReferencedEntityType(entityTypeId); // duplicate

    // Assert
    field.ReferencedEntityTypeIds.Count.ShouldBe(1);
  }

  [Fact]
  public void RemoveReferencedEntityType_ShouldRemoveReference()
  {
    // Arrange
    var field = new DataModelField(Guid.NewGuid(), "Field", DataModelFieldType.LookupEntity);
    var entityTypeId = Guid.NewGuid();
    field.AddReferencedEntityType(entityTypeId);

    // Act
    field.RemoveReferencedEntityType(entityTypeId);

    // Assert
    field.ReferencedEntityTypeIds.ShouldBeEmpty();
  }

  [Fact]
  public void FluentAPI_ShouldChainMethodCalls()
  {
    // Arrange
    var id = Guid.NewGuid();
    var entityRefId = Guid.NewGuid();

    // Act
    var field = new DataModelField(id, "TestField", DataModelFieldType.LookupEntity)
      .SetLabel("Test Label")
      .SetDescription("Test Description")
      .MarkAsNullable()
      .MarkAsPublishedForLookup()
      .AddReferencedEntityType(entityRefId);

    // Assert
    field.Id.ShouldBe(id);
    field.Name.ShouldBe("TestField");
    field.Label.ShouldBe("Test Label");
    field.Description.ShouldBe("Test Description");
    field.IsNullable.ShouldBeTrue();
    field.IsPublishedForLookup.ShouldBeTrue();
    field.ReferencedEntityTypeIds.ShouldContain(entityRefId);
  }
}
