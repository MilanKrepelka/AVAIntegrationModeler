using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DataModels;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModels.Compare;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.DataModels.Compare;

/// <summary>
/// Unit testy pro DataModelComparer — logika hlubokého porovnání DataModelu
/// (vlastnosti modelu + porovnání polí) mezi databází a AVAPlace.
/// </summary>
public class DataModelComparerTests
{
  private static readonly Guid ModelId = Guid.Parse("11111111-1111-1111-1111-111111111111");

  private static DataModelDTO BuildModel(
    string code = "DM-001",
    string name = "Model",
    string? description = null,
    bool isAggregateRoot = false,
    List<DataModelFieldDTO>? fields = null) => new()
  {
    Id = ModelId,
    Code = code,
    Name = name,
    Description = description ?? string.Empty,
    IsAggregateRoot = isAggregateRoot,
    Fields = fields ?? new List<DataModelFieldDTO>()
  };

  private static DataModelFieldDTO BuildField(
    string name,
    string? label = null,
    DataModelFieldType fieldType = DataModelFieldType.Text,
    bool isNullable = false,
    bool isCollection = false,
    bool isLocalized = false,
    bool isPublishedForLookup = false,
    List<Guid>? referencedEntityTypeIds = null) => new()
  {
    Id = Guid.NewGuid(),
    Name = name,
    Label = label ?? string.Empty,
    FieldType = fieldType,
    IsNullable = isNullable,
    IsCollection = isCollection,
    IsLocalized = isLocalized,
    IsPublishedForLookup = isPublishedForLookup,
    ReferencedEntityTypeIds = referencedEntityTypeIds ?? new List<Guid>()
  };

  // --- BuildComparison: status modelu ---

  [Fact]
  public void BuildComparison_BothExist_SameProperties_ReturnsStatusSame()
  {
    var db = BuildModel();
    var ava = BuildModel();

    var result = DataModelComparer.BuildComparison(ModelId, db, ava);

    result.ModelStatus.ShouldBe(ComparisonStatus.Same);
    result.HasDifferences.ShouldBeFalse();
    result.ExistsInDatabase.ShouldBeTrue();
    result.ExistsInAvaPlace.ShouldBeTrue();
  }

  [Fact]
  public void BuildComparison_BothExist_DifferentCode_ReturnsStatusDifferent()
  {
    var db = BuildModel(code: "DM-001");
    var ava = BuildModel(code: "DM-999");

    var result = DataModelComparer.BuildComparison(ModelId, db, ava);

    result.ModelStatus.ShouldBe(ComparisonStatus.Different);
    result.HasDifferences.ShouldBeTrue();
    result.PropertyDiffs.ShouldContain(d => d.PropertyName == "Code");
  }

  [Fact]
  public void BuildComparison_OnlyInDatabase_ReturnsStatusOnlyInDatabase()
  {
    var db = BuildModel();

    var result = DataModelComparer.BuildComparison(ModelId, db, null);

    result.ModelStatus.ShouldBe(ComparisonStatus.OnlyInDatabase);
    result.ExistsInDatabase.ShouldBeTrue();
    result.ExistsInAvaPlace.ShouldBeFalse();
    result.HasDifferences.ShouldBeTrue();
    result.PropertyDiffs.ShouldBeEmpty();
  }

  [Fact]
  public void BuildComparison_OnlyInAvaPlace_ReturnsStatusOnlyInAvaPlace()
  {
    var ava = BuildModel();

    var result = DataModelComparer.BuildComparison(ModelId, null, ava);

    result.ModelStatus.ShouldBe(ComparisonStatus.OnlyInAvaPlace);
    result.ExistsInDatabase.ShouldBeFalse();
    result.ExistsInAvaPlace.ShouldBeTrue();
    result.HasDifferences.ShouldBeTrue();
  }

  [Fact]
  public void BuildComparison_UsesCodeAndNameFromDb_WhenBothExist()
  {
    var db = BuildModel(code: "DB-CODE", name: "DB Name");
    var ava = BuildModel(code: "AVA-CODE", name: "AVA Name");

    var result = DataModelComparer.BuildComparison(ModelId, db, ava);

    result.Code.ShouldBe("DB-CODE");
    result.Name.ShouldBe("DB Name");
  }

  [Fact]
  public void BuildComparison_FallsBackToAvaPlace_WhenDbNull()
  {
    var ava = BuildModel(code: "AVA-CODE", name: "AVA Name");

    var result = DataModelComparer.BuildComparison(ModelId, null, ava);

    result.Code.ShouldBe("AVA-CODE");
    result.Name.ShouldBe("AVA Name");
  }

  // --- CompareModelProperties ---

  [Fact]
  public void CompareModelProperties_SameValues_ReturnsEmptyDiffs()
  {
    var db = BuildModel(code: "X", name: "Y", description: "Desc", isAggregateRoot: true);
    var ava = BuildModel(code: "X", name: "Y", description: "Desc", isAggregateRoot: true);

    var diffs = DataModelComparer.CompareModelProperties(db, ava);

    diffs.ShouldBeEmpty();
  }

  [Fact]
  public void CompareModelProperties_DifferentName_ReturnsDiff()
  {
    var db = BuildModel(name: "Jméno DB");
    var ava = BuildModel(name: "Jméno AVA");

    var diffs = DataModelComparer.CompareModelProperties(db, ava);

    diffs.ShouldContain(d => d.PropertyName == "Name" && d.DatabaseValue == "Jméno DB" && d.AvaPlaceValue == "Jméno AVA");
  }

  [Fact]
  public void CompareModelProperties_DifferentIsAggregateRoot_ReturnsDiff()
  {
    var db = BuildModel(isAggregateRoot: true);
    var ava = BuildModel(isAggregateRoot: false);

    var diffs = DataModelComparer.CompareModelProperties(db, ava);

    diffs.ShouldContain(d => d.PropertyName == "IsAggregateRoot");
  }

  [Fact]
  public void CompareModelProperties_DoesNotCompareNotes()
  {
    // AVAPlace neposkytuje Notes — nesmí být zahrnuty do porovnání
    var db = new DataModelDTO { Id = ModelId, Code = "X", Name = "Y", Notes = "Interní poznámka", Fields = new() };
    var ava = new DataModelDTO { Id = ModelId, Code = "X", Name = "Y", Notes = string.Empty, Fields = new() };

    var diffs = DataModelComparer.CompareModelProperties(db, ava);

    diffs.ShouldNotContain(d => d.PropertyName == "Notes");
  }

  [Fact]
  public void CompareModelProperties_DoesNotCompareAreaId()
  {
    // AVAPlace neposkytuje AreaId — nesmí být zahrnuty do porovnání
    var db = new DataModelDTO { Id = ModelId, Code = "X", Name = "Y", AreaId = Guid.NewGuid(), Fields = new() };
    var ava = new DataModelDTO { Id = ModelId, Code = "X", Name = "Y", AreaId = null, Fields = new() };

    var diffs = DataModelComparer.CompareModelProperties(db, ava);

    diffs.ShouldNotContain(d => d.PropertyName == "AreaId");
  }

  // --- CompareFields ---

  [Fact]
  public void CompareFields_SameFields_AllStatusSame()
  {
    var field = BuildField("FieldA");
    var dbFields = new List<DataModelFieldDTO> { field };
    var avaFields = new List<DataModelFieldDTO>
    {
      BuildField("FieldA") // stejné vlastnosti, jiné ID — párování dle Name
    };

    var result = DataModelComparer.CompareFields(dbFields, avaFields);

    result.ShouldHaveSingleItem();
    result[0].Status.ShouldBe(ComparisonStatus.Same);
    result[0].FieldName.ShouldBe("FieldA");
  }

  [Fact]
  public void CompareFields_MatchesByNameCaseInsensitive()
  {
    var dbFields = new List<DataModelFieldDTO> { BuildField("fieldname") };
    var avaFields = new List<DataModelFieldDTO> { BuildField("FIELDNAME") };

    var result = DataModelComparer.CompareFields(dbFields, avaFields);

    result.ShouldHaveSingleItem();
    result[0].DatabaseField.ShouldNotBeNull();
    result[0].AvaPlaceField.ShouldNotBeNull();
  }

  [Fact]
  public void CompareFields_OnlyInDatabase_ReturnsCorrectStatus()
  {
    var dbFields = new List<DataModelFieldDTO> { BuildField("DBOnlyField") };
    var avaFields = new List<DataModelFieldDTO>();

    var result = DataModelComparer.CompareFields(dbFields, avaFields);

    result.ShouldHaveSingleItem();
    result[0].Status.ShouldBe(ComparisonStatus.OnlyInDatabase);
    result[0].AvaPlaceField.ShouldBeNull();
  }

  [Fact]
  public void CompareFields_OnlyInAvaPlace_ReturnsCorrectStatus()
  {
    var dbFields = new List<DataModelFieldDTO>();
    var avaFields = new List<DataModelFieldDTO> { BuildField("AvaOnlyField") };

    var result = DataModelComparer.CompareFields(dbFields, avaFields);

    result.ShouldHaveSingleItem();
    result[0].Status.ShouldBe(ComparisonStatus.OnlyInAvaPlace);
    result[0].DatabaseField.ShouldBeNull();
  }

  [Fact]
  public void CompareFields_DifferentLabel_ReturnsStatusDifferent()
  {
    var dbFields = new List<DataModelFieldDTO> { BuildField("FieldA", label: "Label DB") };
    var avaFields = new List<DataModelFieldDTO> { BuildField("FieldA", label: "Label AVA") };

    var result = DataModelComparer.CompareFields(dbFields, avaFields);

    result.ShouldHaveSingleItem();
    result[0].Status.ShouldBe(ComparisonStatus.Different);
    result[0].PropertyDiffs.ShouldContain(d => d.PropertyName == "Label");
  }

  [Fact]
  public void CompareFields_DuplicateNamesInAvaPlace_DeduplicatedByGroupBy()
  {
    // AVAPlace může poslat více polí se stejným názvem (Guid.Empty ID) — GroupBy+First() je deduplikuje
    var avaFields = new List<DataModelFieldDTO>
    {
      BuildField("DupField"),
      BuildField("DupField")
    };
    var dbFields = new List<DataModelFieldDTO> { BuildField("DupField") };

    // Nesmí vyhodit ArgumentException z ToDictionary
    var result = DataModelComparer.CompareFields(dbFields, avaFields);

    result.ShouldHaveSingleItem();
  }

  [Fact]
  public void CompareFields_ResultSortedByFieldNameCaseInsensitive()
  {
    var dbFields = new List<DataModelFieldDTO> { BuildField("Zebra"), BuildField("Apple"), BuildField("Mango") };
    var avaFields = new List<DataModelFieldDTO> { BuildField("Zebra"), BuildField("Apple"), BuildField("Mango") };

    var result = DataModelComparer.CompareFields(dbFields, avaFields);

    result.Select(f => f.FieldName).ShouldBe(new[] { "Apple", "Mango", "Zebra" });
  }

  // --- CompareFieldProperties ---

  [Fact]
  public void CompareFieldProperties_SameValues_ReturnsEmpty()
  {
    var field = BuildField("F", label: "L", fieldType: DataModelFieldType.Text);

    var diffs = DataModelComparer.CompareFieldProperties(field, field);

    diffs.ShouldBeEmpty();
  }

  [Fact]
  public void CompareFieldProperties_DifferentFieldType_ReturnsDiff()
  {
    var db = BuildField("F", fieldType: DataModelFieldType.Text);
    var ava = BuildField("F", fieldType: DataModelFieldType.LookupEntity);

    var diffs = DataModelComparer.CompareFieldProperties(db, ava);

    diffs.ShouldContain(d => d.PropertyName == "FieldType");
  }

  [Fact]
  public void CompareFieldProperties_ReferencedEntityTypeIds_OrderIndependent()
  {
    var id1 = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");
    var id2 = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002");

    var db = BuildField("F", referencedEntityTypeIds: new List<Guid> { id1, id2 });
    var ava = BuildField("F", referencedEntityTypeIds: new List<Guid> { id2, id1 });

    var diffs = DataModelComparer.CompareFieldProperties(db, ava);

    diffs.ShouldNotContain(d => d.PropertyName == "ReferencedEntityTypeIds");
  }

  [Fact]
  public void CompareFieldProperties_DifferentReferencedIds_ReturnsDiff()
  {
    var id1 = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");
    var id2 = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002");

    var db = BuildField("F", referencedEntityTypeIds: new List<Guid> { id1 });
    var ava = BuildField("F", referencedEntityTypeIds: new List<Guid> { id2 });

    var diffs = DataModelComparer.CompareFieldProperties(db, ava);

    diffs.ShouldContain(d => d.PropertyName == "ReferencedEntityTypeIds");
  }

  // --- HasDifferences ---

  [Fact]
  public void BuildComparison_HasDifferences_TrueWhenFieldOnlyInDatabase()
  {
    var db = BuildModel(fields: new List<DataModelFieldDTO> { BuildField("ExtraField") });
    var ava = BuildModel(fields: new List<DataModelFieldDTO>());

    var result = DataModelComparer.BuildComparison(ModelId, db, ava);

    result.HasDifferences.ShouldBeTrue();
    result.ModelStatus.ShouldBe(ComparisonStatus.Same); // model vlastnosti jsou stejné
    result.FieldComparisons.ShouldContain(f => f.Status == ComparisonStatus.OnlyInDatabase);
  }
}
