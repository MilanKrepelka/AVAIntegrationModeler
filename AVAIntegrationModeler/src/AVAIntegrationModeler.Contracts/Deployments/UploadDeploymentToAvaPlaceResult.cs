namespace AVAIntegrationModeler.Contracts.Deployments;

/// <summary>
/// Výsledek nahrání DataModelů nasazení do AVAPlace.
/// </summary>
public class UploadDeploymentToAvaPlaceResult
{
  /// <summary>Kód nově vytvořené verze metadat v DataService.</summary>
  public string VersionCode { get; set; } = string.Empty;

  /// <summary>Počet importovaných datových modelů (definice).</summary>
  public int ModelsImported { get; set; }

  /// <summary>Počet importovaných skupin unifikovaných dat (DataModelRecordy).</summary>
  public int RecordGroupsImported { get; set; }
}
