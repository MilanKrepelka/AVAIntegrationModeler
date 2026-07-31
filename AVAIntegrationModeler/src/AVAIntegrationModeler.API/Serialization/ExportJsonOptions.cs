using System.Text.Json;
using System.Text.Json.Serialization;

namespace AVAIntegrationModeler.API.Serialization;

/// <summary>
/// Sdílené možnosti JSON serializéru pro všechny export endpointy.
/// Zajišťuje konzistentní výstup: odsazený JSON a enumy jako textové názvy místo čísel.
/// </summary>
public static class ExportJsonOptions
{
  /// <summary>
  /// Sdílená instance serializačních možností — používat ve všech exportních endpointech.
  /// </summary>
  public static readonly JsonSerializerOptions Instance = new()
  {
    WriteIndented = true,
    Converters = { new JsonStringEnumConverter() }
  };
}
