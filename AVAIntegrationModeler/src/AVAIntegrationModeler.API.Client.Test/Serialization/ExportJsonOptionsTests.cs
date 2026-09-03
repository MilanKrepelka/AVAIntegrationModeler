using System.Text.Json;
using AVAIntegrationModeler.API.Serialization;

namespace AVAIntegrationModeler.API.Client.Test.Serialization;

/// <summary>
/// Testy pro sdílenou instanci <see cref="ExportJsonOptions.Instance"/> —
/// ověřují, že null hodnoty a prázdné kolekce jsou při serializaci vynechány.
/// </summary>
public class ExportJsonOptionsTests
{
  private record ModelWithNullable(string Name, string? Description, List<string>? Tags);
  private record ModelWithCollection(string Name, List<string> Items, List<int> Numbers);
  private record ModelWithNestedNull(string Code, Inner? Child);
  private record Inner(string Value);

  [Fact]
  public void Serialize_NullProperty_IsOmitted()
  {
    var obj = new ModelWithNullable("Test", null, null);

    var json = JsonSerializer.Serialize(obj, ExportJsonOptions.Instance);
    using var doc = JsonDocument.Parse(json);

    Assert.True(doc.RootElement.TryGetProperty("name", out _));
    Assert.False(doc.RootElement.TryGetProperty("description", out _), "Null 'description' nesmí být v JSON.");
    Assert.False(doc.RootElement.TryGetProperty("tags", out _), "Null 'tags' nesmí být v JSON.");
  }

  [Fact]
  public void Serialize_EmptyList_IsOmitted()
  {
    var obj = new ModelWithCollection("Test", new List<string>(), new List<int>());

    var json = JsonSerializer.Serialize(obj, ExportJsonOptions.Instance);
    using var doc = JsonDocument.Parse(json);

    Assert.True(doc.RootElement.TryGetProperty("name", out _));
    Assert.False(doc.RootElement.TryGetProperty("items", out _), "Prázdný 'items' nesmí být v JSON.");
    Assert.False(doc.RootElement.TryGetProperty("numbers", out _), "Prázdný 'numbers' nesmí být v JSON.");
  }

  [Fact]
  public void Serialize_NonEmptyList_IsPresent()
  {
    var obj = new ModelWithCollection("Test", new List<string> { "a", "b" }, new List<int> { 1 });

    var json = JsonSerializer.Serialize(obj, ExportJsonOptions.Instance);
    using var doc = JsonDocument.Parse(json);

    Assert.True(doc.RootElement.TryGetProperty("items", out var items));
    Assert.Equal(2, items.GetArrayLength());
    Assert.True(doc.RootElement.TryGetProperty("numbers", out var numbers));
    Assert.Equal(1, numbers.GetArrayLength());
  }

  [Fact]
  public void Serialize_NullNestedObject_IsOmitted()
  {
    var obj = new ModelWithNestedNull("X", null);

    var json = JsonSerializer.Serialize(obj, ExportJsonOptions.Instance);
    using var doc = JsonDocument.Parse(json);

    Assert.False(doc.RootElement.TryGetProperty("child", out _), "Null nested objekt nesmí být v JSON.");
  }

  [Fact]
  public void Serialize_NonNullNestedObject_IsPresent()
  {
    var obj = new ModelWithNestedNull("X", new Inner("hello"));

    var json = JsonSerializer.Serialize(obj, ExportJsonOptions.Instance);
    using var doc = JsonDocument.Parse(json);

    Assert.True(doc.RootElement.TryGetProperty("child", out var child));
    Assert.Equal("hello", child.GetProperty("value").GetString());
  }
}
