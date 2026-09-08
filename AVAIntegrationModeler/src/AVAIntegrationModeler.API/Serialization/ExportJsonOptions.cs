using System.Collections;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AVAIntegrationModeler.API.Serialization;

/// <summary>
/// Sdílené možnosti JSON serializéru pro všechny export endpointy.
/// Zajišťuje konzistentní výstup: odsazený JSON, enumy jako textové názvy místo čísel,
/// null a prázdné kolekce jsou vynechány.
/// </summary>
public static class ExportJsonOptions
{
  /// <summary>
  /// Sdílená instance serializačních možností — používat ve všech exportních endpointech.
  /// </summary>
  public static readonly JsonSerializerOptions Instance = new()
  {
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    TypeInfoResolver = new DefaultJsonTypeInfoResolver
    {
      Modifiers = { SkipEmptyCollections }
    },
    Converters = { new JsonStringEnumConverter() }
  };

  private static void SkipEmptyCollections(JsonTypeInfo typeInfo)
  {
    foreach (var prop in typeInfo.Properties)
    {
      if (prop.Get is null)
        continue;

      var orig = prop.ShouldSerialize;
      prop.ShouldSerialize = (obj, value) =>
      {
        if (value is null) return false;
        if (value is ICollection { Count: 0 }) return false;
        return orig?.Invoke(obj, value) ?? true;
      };
    }
  }
}
