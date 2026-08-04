using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Web.ViewModels.List;

/// <summary>
/// Třída představující ViewModel pro seznam scénářů.
/// </summary>
public class DataModelListViewModel
{
  /// <summary>
  /// View Model představující prázdný/nezadaný datový model.
  /// </summary>
  public static DataModelListViewModel Empty => new DataModelListViewModel
  {
    Id = Guid.Empty,
    Code = string.Empty,
    Name = string.Empty,
    Description = string.Empty,
    Notes = string.Empty,
    IsAggregateRoot = false,
    AreaId = null
  };

  /// <summary>
  /// <see cref="DataModelDTO.Id"/>
  /// </summary>
  public Guid Id { get; set; } = Guid.Empty;

  /// <summary>
  /// <see cref="DataModelDTO.Code"/>
  /// </summary>
  public string Code { get; set; } = string.Empty;

  /// <summary>
  /// <see cref="DataModelDTO.Name"/>
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// <see cref="DataModelDTO.Description"/>
  /// </summary>
  public string Description { get; set; } = string.Empty;

  /// <summary>
  /// <see cref="DataModelDTO.Notes"/>
  /// </summary>
  public string Notes { get; set; } = string.Empty;

  /// <summary>
  /// <see cref="DataModelDTO.IsAggregateRoot"/>
  /// </summary>
  public bool IsAggregateRoot { get; set; }

  /// <summary>
  /// <see cref="DataModelDTO.AreaId"/>
  /// </summary>
  public Guid? AreaId { get; set; }

  /// <summary>
  /// <see cref="DataModelDTO.Fields"/>
  /// </summary>
  public List<DataModelFieldListViewModel> Fields { get; set; } = [];

  /// <summary>
  /// Textová reprezentace všech fieldů.
  /// </summary>
  public string FieldsText => string.Join(", ", Fields.Select(f => f.Name));

  /// <summary>
  /// Příznak pro zobrazení detailů.
  /// </summary>
  public bool ShowDetails { get; set; } = false;
}

/// <summary>
/// ViewModel pro pole datového modelu.
/// </summary>
public class DataModelFieldListViewModel
{
  /// <summary>
  /// View Model představující prázdné/nezadané pole.
  /// </summary>
  public static DataModelFieldListViewModel Empty => new DataModelFieldListViewModel
  {
    Id = Guid.Empty,
    Name = string.Empty,
    Label = string.Empty,
    Description = string.Empty,
    IsPublishedForLookup = false,
    IsCollection = false,
    IsLocalized = false,
    IsNullable = false,
    FieldType = DataModelFieldType.Text
  };

  /// <summary>
  /// <see cref="DataModelFieldDTO.Id"/>
  /// </summary>
  public Guid Id { get; set; } = Guid.Empty;

  /// <summary>
  /// <see cref="DataModelFieldDTO.Name"/>
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// <see cref="DataModelFieldDTO.Label"/>
  /// </summary>
  public string Label { get; set; } = string.Empty;

  /// <summary>
  /// <see cref="DataModelFieldDTO.Description"/>
  /// </summary>
  public string Description { get; set; } = string.Empty;

  /// <summary>
  /// <see cref="DataModelFieldDTO.IsPublishedForLookup"/>
  /// </summary>
  public bool IsPublishedForLookup { get; set; }

  /// <summary>
  /// <see cref="DataModelFieldDTO.IsCollection"/>
  /// </summary>
  public bool IsCollection { get; set; }

  /// <summary>
  /// <see cref="DataModelFieldDTO.IsLocalized"/>
  /// </summary>
  public bool IsLocalized { get; set; }

  /// <summary>
  /// <see cref="DataModelFieldDTO.IsNullable"/>
  /// </summary>
  public bool IsNullable { get; set; }

  /// <summary>
  /// <see cref="DataModelFieldDTO.FieldType"/>
  /// </summary>
  public DataModelFieldType FieldType { get; set; }

  /// <summary>
  /// <see cref="DataModelFieldDTO.ReferencedEntityTypeIds"/>
  /// </summary>
  public List<Guid> ReferencedEntityTypeIds { get; set; } = [];

  /// <summary>
  /// Vnořená pole (pouze pro ComplexType).
  /// </summary>
  public List<DataModelDTO> ReferencedModels { get; set; } = [];

  /// <summary>
  /// Referencované datové modely jako text
  /// </summary>
  public string ReferencedModelsText => string.Join(",", ReferencedModels.Select(item => item.Code));
}
