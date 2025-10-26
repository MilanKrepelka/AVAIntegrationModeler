using AVAIntegrationModeler.Domain.DataModelAggregate;
using Shouldly;

namespace AVAIntegrationModeler.Domain.Test.DataModelAggregate;

/// <summary>
/// Unit testy pro agregát DataModel.
/// </summary>
public class DataModelTests
{
  [Fact]
  public void Constructor_ShouldSetIdAndCode()
  {
    // Arrange
    var id = Guid.NewGuid();
    var code = "TEST_MODEL";

    // Act
    var model = new DataModel(id, code);

    // Assert
    model.Id.ShouldBe(id);
    model.Code.ShouldBe(code);
  }

  [Fact]
  public void Constructor_WithNullOrEmptyCode_ShouldThrowException()
  {
    // Arrange
    var id = Guid.NewGuid();

    // Act & Assert
    Should.Throw<ArgumentException>(() => new DataModel(id, null!));
    Should.Throw<ArgumentException>(() => new DataModel(id, string.Empty));
  }

  [Fact]
  public void SetName_ShouldUpdateName()
  {
    // Arrange
    var model = new DataModel(Guid.NewGuid(), "CODE");
    var name = "Test Model";

    // Act
    model.SetName(name);

    // Assert
    model.Name.ShouldBe(name);
  }

  [Fact]
  public void MarkAsAggregateRoot_ShouldSetFlagToTrue()
  {
    // Arrange
    var model = new DataModel(Guid.NewGuid(), "CODE");

    // Act
    model.MarkAsAggregateRoot();

    // Assert
    model.IsAggregateRoot.ShouldBeTrue();
  }

  [Fact]
  public void AddField_ShouldAddFieldToCollection()
  {
    // Arrange
    var model = new DataModel(Guid.NewGuid(), "CODE");
    var field = new DataModelField(Guid.NewGuid(), "Field1", DataModelFieldType.Text);

    // Act
    model.AddField(field);

    // Assert
    model.Fields.Count.ShouldBe(1);
    model.Fields.ShouldContain(field);
  }

  [Fact]
  public void AddField_WithDuplicateName_ShouldThrowException()
  {
    // Arrange
    var model = new DataModel(Guid.NewGuid(), "CODE");
    var field1 = new DataModelField(Guid.NewGuid(), "Field1", DataModelFieldType.Text);
    var field2 = new DataModelField(Guid.NewGuid(), "Field1", DataModelFieldType.WholeNumber);
    model.AddField(field1);

    // Act & Assert
    var exception = Should.Throw<InvalidOperationException>(() => model.AddField(field2));
    exception.Message.ShouldContain("already exists");
  }

  [Fact]
  public void RemoveField_ShouldRemoveFieldFromCollection()
  {
    // Arrange
    var model = new DataModel(Guid.NewGuid(), "CODE");
    var field = new DataModelField(Guid.NewGuid(), "Field1", DataModelFieldType.Text);
    model.AddField(field);

    // Act
    model.RemoveField("Field1");

    // Assert
    model.Fields.ShouldBeEmpty();
  }

  [Fact]
  public void GetField_ShouldReturnField()
  {
    // Arrange
    var model = new DataModel(Guid.NewGuid(), "CODE");
    var field = new DataModelField(Guid.NewGuid(), "Field1", DataModelFieldType.Text);
    model.AddField(field);

    // Act
    var result = model.GetField("Field1");

    // Assert
    result.ShouldNotBeNull();
    result.ShouldBe(field);
  }

  [Fact]
  public void GetField_IsCaseInsensitive()
  {
    // Arrange
    var model = new DataModel(Guid.NewGuid(), "CODE");
    var field = new DataModelField(Guid.NewGuid(), "Field1", DataModelFieldType.Text);
    model.AddField(field);

    // Act
    var result = model.GetField("FIELD1");

    // Assert
    result.ShouldNotBeNull();
  }

  [Fact]
  public void FluentAPI_ShouldChainMethodCalls()
  {
    // Arrange
    var id = Guid.NewGuid();
    var field = new DataModelField(Guid.NewGuid(), "Field1", DataModelFieldType.Text);

    // Act
    var model = new DataModel(id, "CODE")
      .SetName("Test Model")
      .SetDescription("Description")
      .MarkAsAggregateRoot()
      .AddField(field);

    // Assert
    model.Id.ShouldBe(id);
    model.Name.ShouldBe("Test Model");
    model.IsAggregateRoot.ShouldBeTrue();
    model.Fields.Count.ShouldBe(1);
  }
}
