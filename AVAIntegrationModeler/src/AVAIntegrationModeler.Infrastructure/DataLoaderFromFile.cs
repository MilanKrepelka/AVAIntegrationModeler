using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Ardalis.Result;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Infrastructure;
public static class DataLoaderFromFile
{
  public static string LoadDataFromFile(string fileName, string[] paths)
  {
    // If the provided path is not rooted, treat it as relative to this assembly's directory.
    string resolvedPath = fileName;

    // Try to get the directory of the executing assembly.
    string? assemblyLocation = Assembly.GetExecutingAssembly().Location;
    string? assemblyDir = null;

    if (!string.IsNullOrEmpty(assemblyLocation))
    {
      assemblyDir = Path.GetDirectoryName(assemblyLocation);
    }

    // Fallback to AppContext.BaseDirectory when assembly location is not available.
    if (string.IsNullOrEmpty(assemblyDir))
    {
      assemblyDir = AppContext.BaseDirectory;
    }
    List<string> pathList = new List<string>();
    pathList.Add(assemblyDir);
    pathList.AddRange(paths);
    pathList.Add(fileName);
    resolvedPath = Path.Combine(pathList.ToArray());

    if (!File.Exists(resolvedPath))
    {
      throw new FileNotFoundException($"The file at path {resolvedPath} was not found.", resolvedPath);
    }

    return File.ReadAllText(resolvedPath);
  }

  /*
  Pseudocode / Plan (detailed):

  - Read JSON text using existing LoadDataFromFile.
  - Try to parse the JSON text into a JsonNode (`JsonNode.Parse`).
  - If the parsed node is a JsonArray, return it.
  - If the parsed node is a JsonObject:
    - If any property value is a JsonArray, return the first such array.
    - Otherwise, wrap the JsonObject into a new JsonArray and return that (so caller always gets an array).
  - If the parsed node is null or of another type (e.g. primitive), throw a JsonException with a helpful message.
  - Keep JsonSerializerOptions defined for possible future use (enum converters / case insensitivity),
    but use JsonNode parsing to preserve raw structure and avoid losing comments/formatting differences.
  */

  public static JsonArray LoadJArrayFromFile(string fileName, string[] paths)
  {
    var options = new JsonSerializerOptions
    {
      Converters = { new JsonStringEnumConverter() },
      PropertyNameCaseInsensitive = true
    };

    string result = LoadDataFromFile(fileName, paths);

    if (string.IsNullOrWhiteSpace(result))
      throw new JsonException("JSON content is empty.");

    JsonNode? root;
    try
    {
      root = JsonNode.Parse(result);
    }
    catch (JsonException ex)
    {
      throw new JsonException("Failed to parse JSON content.", ex);
    }

    if (root == null)
      throw new JsonException("Parsed JSON is null.");

    if (root is JsonArray jsonArray)
    {
      return jsonArray;
    }

    if (root is JsonObject jsonObject)
    {
      // If any property is an array, return the first found.
      foreach (var kvp in jsonObject)
      {
        if (kvp.Value is JsonArray innerArray)
          return innerArray;
      }

      // No array property found: wrap the object into an array so caller always receives a JsonArray.
      var wrapped = new JsonArray { jsonObject };
      return wrapped;
    }

    // For primitive values, wrap into an array containing the value node.
    var primitiveWrapped = new JsonArray { root };
    return primitiveWrapped;
  }
  public static List<T> LoadListFromFile<T>(string fileName, string[] paths)
  {
    var jsonArray = LoadJArrayFromFile(fileName, paths);
    var options = new JsonSerializerOptions
    {
      Converters = { new JsonStringEnumConverter() },
      PropertyNameCaseInsensitive = true
    };
    return JsonSerializer.Deserialize<List<T>>(jsonArray.ToString()!, options)!;
  }
}
