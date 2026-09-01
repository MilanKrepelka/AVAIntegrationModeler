namespace AVAIntegrationModeler.Domain.DeploymentAggregate;

/// <summary>
/// Aggregate root pro nasazení — seskupuje datové modely, které jsou součástí jednoho nasazení.
/// </summary>
public class Deployment : EntityBase<Guid>, IAggregateRoot
{
  /// <summary>
  /// Privátní konstruktor pro EF Core.
  /// </summary>
  private Deployment() { }

  /// <summary>
  /// Konstruktor pro vytvoření nového nasazení.
  /// </summary>
  /// <param name="id">Identifikátor nasazení.</param>
  /// <param name="code">Kód nasazení.</param>
  public Deployment(Guid id, string code)
  {
    Id = id;
    SetCode(code);
  }

  /// <summary>
  /// Technický kód nasazení pro jednoznačnou identifikaci.
  /// </summary>
  public string Code { get; private set; } = string.Empty;

  /// <summary>
  /// Název nasazení.
  /// </summary>
  public string Name { get; private set; } = string.Empty;

  /// <summary>
  /// URL odkaz na ticket nasazení (volitelné).
  /// </summary>
  public string? Ticket { get; private set; }

  /// <summary>
  /// Popis nasazení (volitelné).
  /// </summary>
  public string? Description { get; private set; }

  /// <summary>
  /// Datum a čas posledního uložení nasazení (UTC).
  /// </summary>
  public DateTime? LastSaveDateTime { get; private set; }

  /// <summary>
  /// Datum a čas posledního nasazení do AVAPlace (UTC). Nastavuje se při skutečném nasazení.
  /// </summary>
  public DateTime? LastDeploymentDateTime { get; private set; }

  private readonly List<DeploymentDataModel> _dataModels = new();

  /// <summary>
  /// Datové modely zahrnuté v nasazení.
  /// </summary>
  public IReadOnlyCollection<DeploymentDataModel> DataModels => _dataModels.AsReadOnly();

  /// <summary>
  /// Nastaví kód nasazení.
  /// </summary>
  public Deployment SetCode(string code)
  {
    Code = Guard.Against.NullOrEmpty(code, nameof(code));
    return this;
  }

  /// <summary>
  /// Nastaví název nasazení.
  /// </summary>
  public Deployment SetName(string name)
  {
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
    return this;
  }

  /// <summary>
  /// Nastaví URL odkaz na ticket nasazení.
  /// </summary>
  public Deployment SetTicket(string? ticket)
  {
    Ticket = ticket;
    return this;
  }

  /// <summary>
  /// Nastaví popis nasazení.
  /// </summary>
  public Deployment SetDescription(string? description)
  {
    Description = description;
    return this;
  }

  /// <summary>
  /// Nastaví datum a čas posledního uložení nasazení.
  /// </summary>
  public Deployment SetLastSaveDateTime(DateTime? value)
  {
    LastSaveDateTime = value;
    return this;
  }

  /// <summary>
  /// Nastaví datum a čas posledního nasazení do AVAPlace.
  /// </summary>
  public Deployment SetLastDeploymentDateTime(DateTime? value)
  {
    LastDeploymentDateTime = value;
    return this;
  }

  /// <summary>
  /// Přidá datový model do nasazení. Duplikáty jsou ignorovány.
  /// </summary>
  /// <param name="dataModelId">Identifikátor datového modelu.</param>
  public Deployment AddDataModel(Guid dataModelId)
  {
    Guard.Against.Default(dataModelId, nameof(dataModelId));
    if (_dataModels.Any(m => m.DataModelId == dataModelId))
      return this;
    _dataModels.Add(new DeploymentDataModel(Id, dataModelId));
    return this;
  }

  /// <summary>
  /// Odebere datový model z nasazení.
  /// </summary>
  /// <param name="dataModelId">Identifikátor datového modelu.</param>
  public Deployment RemoveDataModel(Guid dataModelId)
  {
    var item = _dataModels.FirstOrDefault(m => m.DataModelId == dataModelId);
    if (item != null)
      _dataModels.Remove(item);
    return this;
  }
}
