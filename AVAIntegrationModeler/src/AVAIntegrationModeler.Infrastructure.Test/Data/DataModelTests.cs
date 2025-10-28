using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Integration.Test.Data.SqlLite.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace AVAIntegrationModeler.Infrastructure.Test.Data;

/// <summary>
/// Integrační testy pro databázovou vrstvu DataModel aggregate.
/// </summary>
[Collection("DataModelTestCollection")] // 🔥 Vlastní collection
public class DataModelTests : BaseDbTests
{
  private readonly EfRepository<DataModel> _repository;

  public DataModelTests(ITestOutputHelper testOutputHelper, EfSqlClientTestFixture fixture)
    : base(testOutputHelper, fixture)
  {
    _repository = GetRepository<DataModel>();
  }

  [Fact]
  public async Task AddDataModel_ShouldPersistToDatabase()
  {
    // Arrange
    var dataModelId = Guid.NewGuid();
    var dataModelCode = "CUSTOMER";
    var dataModelName = "Customer Data Model";
    var dataModel = new DataModel(dataModelId, dataModelCode);
    dataModel.SetName(dataModelName)
             .SetDescription("Customer entity model")
             .MarkAsAggregateRoot();

    // Act
    await _repository.AddAsync(dataModel, CancellationToken.None);

    // Assert
    var savedDataModel = (await _repository.ListAsync(CancellationToken.None))
      .FirstOrDefault(dm => dm.Id == dataModelId);

    savedDataModel.ShouldNotBeNull();
    savedDataModel.Id.ShouldBe(dataModelId);
    savedDataModel.Code.ShouldBe(dataModelCode);
    savedDataModel.Name.ShouldBe(dataModelName);
    savedDataModel.IsAggregateRoot.ShouldBeTrue();
  }

  [Fact]
  public async Task AddDataModel_WithMinimalData_ShouldSucceed()
  {
    // Arrange
    var dataModelId = Guid.NewGuid();
    var dataModelCode = "MINIMAL";
    var dataModel = new DataModel(dataModelId, dataModelCode);

    // Act
    await _repository.AddAsync(dataModel, CancellationToken.None);

    // Assert
    var savedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);

    savedDataModel.ShouldNotBeNull();
    savedDataModel.Id.ShouldBe(dataModelId);
    savedDataModel.Code.ShouldBe(dataModelCode);
    savedDataModel.Name.ShouldBeEmpty();
    savedDataModel.IsAggregateRoot.ShouldBeFalse();
  }

  [Fact]
  public async Task AddDataModel_WithArea_ShouldPersistRelation()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var area = new Domain.AreaAggregate.Area(areaId, "SALES");
    area.SetName("Sales Department");
    await GetRepository<Domain.AreaAggregate.Area>().AddAsync(area, CancellationToken.None);

    var dataModelId = Guid.NewGuid();
    var dataModel = new DataModel(dataModelId, "ORDER");
    dataModel.SetName("Order")
             .SetArea(areaId);

    // Act
    await _repository.AddAsync(dataModel, CancellationToken.None);

    // Assert
    var savedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    savedDataModel.ShouldNotBeNull();
    savedDataModel.AreaId.ShouldBe(areaId);
  }

  [Fact]
  public async Task UpdateDataModel_ShouldPersistChanges()
  {
    // Arrange
    var dataModelId = Guid.NewGuid();
    var dataModel = new DataModel(dataModelId, "ORIG_CODE");
    dataModel.SetName("Original Name")
             .SetDescription("Original Description");
    await _repository.AddAsync(dataModel, CancellationToken.None);

    // Detach to simulate fetching from database
    DbContext.Entry(dataModel).State = EntityState.Detached;

    // Act
    var fetchedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    fetchedDataModel.ShouldNotBeNull();
    fetchedDataModel.SetCode("NEW_CODE")
                    .SetName("New Name")
                    .SetDescription("New Description")
                    .MarkAsAggregateRoot();

    await _repository.UpdateAsync(fetchedDataModel, CancellationToken.None);

    // Assert
    var updatedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    updatedDataModel.ShouldNotBeNull();
    updatedDataModel.Code.ShouldBe("NEW_CODE");
    updatedDataModel.Name.ShouldBe("New Name");
    updatedDataModel.Description.ShouldBe("New Description");
    updatedDataModel.IsAggregateRoot.ShouldBeTrue();
  }

  [Fact]
  public async Task DeleteDataModel_ShouldRemoveFromDatabase()
  {
    // Arrange
    var dataModelId = Guid.NewGuid();
    var dataModel = new DataModel(dataModelId, "DELETE_TEST");
    dataModel.SetName("Data Model to Delete");
    await _repository.AddAsync(dataModel, CancellationToken.None);

    // Verify it was added
    var addedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    addedDataModel.ShouldNotBeNull();

    // Act
    await _repository.DeleteAsync(addedDataModel, CancellationToken.None);

    // Assert
    var deletedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    deletedDataModel.ShouldBeNull();
  }

  [Fact]
  public async Task AddDataModel_WithFields_ShouldPersistFields()
  {
    // Arrange
    var dataModelId = Guid.NewGuid();
    var dataModel = new DataModel(dataModelId, "CUSTOMER");
    dataModel.SetName("Customer")
             .MarkAsAggregateRoot();

    var field1 = new DataModelField(Guid.NewGuid(), "CustomerName", DataModelFieldType.Text);
    field1.SetLabel("Customer Name")
          .SetDescription("Full name of the customer")
          .MarkAsPublishedForLookup();

    var field2 = new DataModelField(Guid.NewGuid(), "Email", DataModelFieldType.Text);
    field2.SetLabel("Email Address")
          .SetDescription("Customer email");

    var field3 = new DataModelField(Guid.NewGuid(), "IsActive", DataModelFieldType.TwoOptions);
    field3.SetLabel("Is Active")
          .SetDescription("Active customer status");

    dataModel.AddField(field1)
             .AddField(field2)
             .AddField(field3);

    // Act
    await _repository.AddAsync(dataModel, CancellationToken.None);

    // Assert
    var savedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    savedDataModel.ShouldNotBeNull();
    savedDataModel.Fields.Count.ShouldBe(3);
    savedDataModel.Fields.ShouldContain(f => f.Name == "CustomerName");
    savedDataModel.Fields.ShouldContain(f => f.Name == "Email");
    savedDataModel.Fields.ShouldContain(f => f.Name == "IsActive");

    var customerNameField = savedDataModel.Fields.First(f => f.Name == "CustomerName");
    customerNameField.Label.ShouldBe("Customer Name");
    customerNameField.IsPublishedForLookup.ShouldBeTrue();
  }

  [Fact]
  public async Task AddDataModel_WithLookupEntityField_ShouldPersistReferences()
  {
    // Arrange - vytvoříme referenční model
    var referenceModelId = Guid.NewGuid();
    var referenceModel = new DataModel(referenceModelId, "COUNTRY");
    referenceModel.SetName("Country");
    await _repository.AddAsync(referenceModel, CancellationToken.None);

    // Vytvoříme hlavní model s lookup fieldem
    var dataModelId = Guid.NewGuid();
    var dataModel = new DataModel(dataModelId, "CUSTOMER");
    dataModel.SetName("Customer");

    var lookupField = new DataModelField(Guid.NewGuid(), "Country", DataModelFieldType.LookupEntity);
    lookupField.SetLabel("Country")
               .SetDescription("Customer country")
               .AddReferencedEntityType(referenceModelId);

    dataModel.AddField(lookupField);

    // Act
    await _repository.AddAsync(dataModel, CancellationToken.None);

    // Assert
    var savedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    savedDataModel.ShouldNotBeNull();
    savedDataModel.Fields.Count.ShouldBe(1);

    var savedLookupField = savedDataModel.Fields.First();
    savedLookupField.FieldType.ShouldBe(DataModelFieldType.LookupEntity);
    savedLookupField.EntityTypeReferences.Count.ShouldBe(1);
    savedLookupField.ReferencedEntityTypeIds.ShouldContain(referenceModelId);
  }

  [Fact]
  public async Task RemoveField_ShouldPersistRemoval()
  {
    // Arrange
    var dataModelId = Guid.NewGuid();
    var dataModel = new DataModel(dataModelId, "PRODUCT");
    dataModel.SetName("Product");

    var field1 = new DataModelField(Guid.NewGuid(), "Name", DataModelFieldType.Text);
    var field2 = new DataModelField(Guid.NewGuid(), "Price", DataModelFieldType.CurrencyNumber);
    var field3 = new DataModelField(Guid.NewGuid(), "Description", DataModelFieldType.MultilineText);

    dataModel.AddField(field1)
             .AddField(field2)
             .AddField(field3);

    await _repository.AddAsync(dataModel, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Detach všechny entity
    DbContext.ChangeTracker.Clear(); // 🔥 Lepší než manuální detach

    // Act - načteme s explicitním Include
    var fetchedDataModel = await DbContext.DataModels
        .Include(dm => dm.Fields) // 🔥 KLÍČOVÉ!
        .FirstOrDefaultAsync(dm => dm.Id == dataModelId, CancellationToken.None);
    
    fetchedDataModel.ShouldNotBeNull();
    
    var fieldsCountBefore = fetchedDataModel.Fields.Count;
    this._testOutputHelper.WriteLine($"Fields count before remove: {fieldsCountBefore}");
    fieldsCountBefore.ShouldBe(3); // 🔥 Mělo by být 3!
    
    fetchedDataModel.RemoveField("Price");
    
    var fieldsCountAfter = fetchedDataModel.Fields.Count;
    this._testOutputHelper.WriteLine($"Fields count after remove: {fieldsCountAfter}");
    
    await _repository.UpdateAsync(fetchedDataModel, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert - znovu načteme s Include
    var updatedDataModel = await DbContext.DataModels
        .Include(dm => dm.Fields) // 🔥 KLÍČOVÉ!
        .FirstOrDefaultAsync(dm => dm.Id == dataModelId, CancellationToken.None);
    
    updatedDataModel.ShouldNotBeNull();
    
    this._testOutputHelper.WriteLine($"Fields count after reload: {updatedDataModel.Fields.Count}");
    
    updatedDataModel.Fields.Count.ShouldBe(2);
    updatedDataModel.Fields.ShouldNotContain(f => f.Name == "Price");
    updatedDataModel.Fields.ShouldContain(f => f.Name == "Name");
    updatedDataModel.Fields.ShouldContain(f => f.Name == "Description");
  }

  [Fact]
  public void AddDataModel_DuplicateFieldName_ShouldThrowException()
  {
    // Arrange
    var dataModelId = Guid.NewGuid();
    var dataModel = new DataModel(dataModelId, "TEST");

    var field1 = new DataModelField(Guid.NewGuid(), "Name", DataModelFieldType.Text);
    var field2 = new DataModelField(Guid.NewGuid(), "Name", DataModelFieldType.Text); // duplicate

    dataModel.AddField(field1);

    // Act & Assert
    Should.Throw<InvalidOperationException>(() => dataModel.AddField(field2));
  }

  [Fact]
  public async Task MarkAsAggregateRoot_ShouldSetFlag()
  {
    // Arrange
    var dataModelId = Guid.NewGuid();
    var dataModel = new DataModel(dataModelId, "AGGREGATE");

    // Act
    dataModel.MarkAsAggregateRoot();
    await _repository.AddAsync(dataModel, CancellationToken.None);

    // Assert
    var savedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    savedDataModel.ShouldNotBeNull();
    savedDataModel.IsAggregateRoot.ShouldBeTrue();
  }

  [Fact]
  public async Task MarkAsNestedEntity_ShouldClearFlag()
  {
    // Arrange
    var dataModelId = Guid.NewGuid();
    var dataModel = new DataModel(dataModelId, "NESTED");
    dataModel.MarkAsAggregateRoot();
    await _repository.AddAsync(dataModel, CancellationToken.None);

    DbContext.Entry(dataModel).State = EntityState.Detached;

    // Act
    var fetchedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    fetchedDataModel.ShouldNotBeNull();
    fetchedDataModel.MarkAsNestedEntity();
    await _repository.UpdateAsync(fetchedDataModel, CancellationToken.None);

    // Assert
    var updatedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    updatedDataModel.ShouldNotBeNull();
    updatedDataModel.IsAggregateRoot.ShouldBeFalse();
  }

  [Fact]
  public async Task SetNotes_ShouldPersist()
  {
    // Arrange
    var dataModelId = Guid.NewGuid();
    var notes = "This is a test data model with important notes";
    var dataModel = new DataModel(dataModelId, "NOTES_TEST");
    dataModel.SetNotes(notes);

    // Act
    await _repository.AddAsync(dataModel, CancellationToken.None);

    // Assert
    var savedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    savedDataModel.ShouldNotBeNull();
    savedDataModel.Notes.ShouldBe(notes);
  }

  [Fact]
  public async Task GetField_ShouldReturnCorrectField()
  {
    // Arrange
    var dataModelId = Guid.NewGuid();
    var dataModel = new DataModel(dataModelId, "FIELD_TEST");

    var field = new DataModelField(Guid.NewGuid(), "TestField", DataModelFieldType.Text);
    field.SetLabel("Test Field");
    dataModel.AddField(field);

    await _repository.AddAsync(dataModel, CancellationToken.None);

    // Act
    var savedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    var retrievedField = savedDataModel?.GetField("TestField");

    // Assert
    retrievedField.ShouldNotBeNull();
    retrievedField.Name.ShouldBe("TestField");
    retrievedField.Label.ShouldBe("Test Field");
  }

  [Fact]
  public async Task ListDataModels_ShouldReturnAll()
  {
    // Arrange
    var dm1 = new DataModel(Guid.NewGuid(), "DM1").SetName("First Model");
    var dm2 = new DataModel(Guid.NewGuid(), "DM2").SetName("Second Model");
    var dm3 = new DataModel(Guid.NewGuid(), "DM3").SetName("Third Model");

    await _repository.AddAsync(dm1, CancellationToken.None);
    await _repository.AddAsync(dm2, CancellationToken.None);
    await _repository.AddAsync(dm3, CancellationToken.None);

    // Act
    var allDataModels = await _repository.ListAsync(CancellationToken.None);

    // Assert
    allDataModels.Count.ShouldBe(3);
    allDataModels.ShouldContain(dm => dm.Code == "DM1");
    allDataModels.ShouldContain(dm => dm.Code == "DM2");
    allDataModels.ShouldContain(dm => dm.Code == "DM3");
  }

  [Fact]
  public async Task DataModel_WithComplexFields_ShouldPersistCorrectly()
  {
    // Arrange
    var dataModelId = Guid.NewGuid();
    var dataModel = new DataModel(dataModelId, "COMPLEX");
    dataModel.SetName("Complex Model")
             .SetDescription("Model with various field types")
             .MarkAsAggregateRoot();

    // Různé typy fieldů
    dataModel.AddField(new DataModelField(Guid.NewGuid(), "TextField", DataModelFieldType.Text)
      .SetLabel("Text Field"));

    dataModel.AddField(new DataModelField(Guid.NewGuid(), "NumberField", DataModelFieldType.WholeNumber)
      .SetLabel("Number Field")
      .MarkAsNullable());

    dataModel.AddField(new DataModelField(Guid.NewGuid(), "DateField", DataModelFieldType.Date)
      .SetLabel("Date Field"));

    dataModel.AddField(new DataModelField(Guid.NewGuid(), "MoneyField", DataModelFieldType.CurrencyNumber)
      .SetLabel("Money Field"));

    dataModel.AddField(new DataModelField(Guid.NewGuid(), "BoolField", DataModelFieldType.TwoOptions)
      .SetLabel("Boolean Field"));

    var collectionField = new DataModelField(Guid.NewGuid(), "Tags", DataModelFieldType.MultiSelectOptionSet);
    collectionField.SetLabel("Tags")
                   .MarkAsCollection();
    dataModel.AddField(collectionField);

    var localizedField = new DataModelField(Guid.NewGuid(), "Description", DataModelFieldType.MultilineText);
    localizedField.SetLabel("Description")
                  .MarkAsLocalized();
    dataModel.AddField(localizedField);

    // Act
    await _repository.AddAsync(dataModel, CancellationToken.None);

    // Assert
    var savedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    savedDataModel.ShouldNotBeNull();
    savedDataModel.Fields.Count.ShouldBe(7);

    var textField = savedDataModel.GetField("TextField");
    textField.ShouldNotBeNull();
    textField.FieldType.ShouldBe(DataModelFieldType.Text);

    var tagsField = savedDataModel.GetField("Tags");
    tagsField.ShouldNotBeNull();
    tagsField.IsCollection.ShouldBeTrue();

    var descField = savedDataModel.GetField("Description");
    descField.ShouldNotBeNull();
    descField.IsLocalized.ShouldBeTrue();
  }

  [Fact]
  public void DataModel_SetCode_EmptyString_ShouldThrowException()
  {
    // Arrange & Act & Assert
    Should.Throw<ArgumentException>(() =>
      new DataModel(Guid.NewGuid(), string.Empty));
  }

  [Fact]
  public void DataModel_SetName_EmptyString_ShouldThrowException()
  {
    // Arrange
    var dataModel = new DataModel(Guid.NewGuid(), "VALID_CODE");

    // Act & Assert
    Should.Throw<ArgumentException>(() =>
      dataModel.SetName(string.Empty));
  }

  [Fact]
  public async Task DataModel_FluentAPI_ShouldChainMethods()
  {
    // Arrange
    var dataModelId = Guid.NewGuid();

    // Act
    var dataModel = new DataModel(dataModelId, "FLUENT_TEST")
      .SetName("Fluent API Test")
      .SetDescription("Testing fluent API")
      .SetNotes("Some notes")
      .MarkAsAggregateRoot();

    await _repository.AddAsync(dataModel, CancellationToken.None);

    // Assert
    var savedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    savedDataModel.ShouldNotBeNull();
    savedDataModel.Code.ShouldBe("FLUENT_TEST");
    savedDataModel.Name.ShouldBe("Fluent API Test");
    savedDataModel.Description.ShouldBe("Testing fluent API");
    savedDataModel.Notes.ShouldBe("Some notes");
    savedDataModel.IsAggregateRoot.ShouldBeTrue();
  }

  [Fact]
  public async Task DeleteDataModel_WithFields_ShouldCascadeDelete()
  {
    // Arrange
    var dataModelId = Guid.NewGuid();
    var dataModel = new DataModel(dataModelId, "CASCADE_TEST");

    var field1 = new DataModelField(Guid.NewGuid(), "Field1", DataModelFieldType.Text);
    var field2 = new DataModelField(Guid.NewGuid(), "Field2", DataModelFieldType.Text);

    dataModel.AddField(field1).AddField(field2);
    await _repository.AddAsync(dataModel, CancellationToken.None);

    // Act
    await _repository.DeleteAsync(dataModel, CancellationToken.None);

    // Assert
    var deletedDataModel = await _repository.GetByIdAsync(dataModelId, CancellationToken.None);
    deletedDataModel.ShouldBeNull();

    // Ověření, že fields byly také smazány
    var remainingFields = DbContext.DataModelFields
      .Where(f => f.Id == field1.Id || f.Id == field2.Id)
      .ToList();
    remainingFields.ShouldBeEmpty();
  }
}
