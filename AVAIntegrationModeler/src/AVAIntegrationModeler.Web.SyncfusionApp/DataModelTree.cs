using System;
using System.Collections.Generic;
using System.Linq;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Infrastructure;

/// <summary>
/// Jednoduchá uzlová reprezentace datového modelu pro sestavení stromu vztahů.
/// Modely jsou propojeny směrem "zdroj -> referencovaný model" podle
/// `Field.ReferencedEntityTypeIds`.
/// </summary>
public class DataModelTree
{
  public Guid Id { get; init; }
  public string? Name { get; init; }
  public DataModelDTO? Model { get; init; }
  public List<DataModelTree> Children { get; } = new();
  public HashSet<Guid> ParentIds { get; } = new();

  /*
   * PSEUDOCODE / PLAN:
   * 1) Validate inputs: if dataModels is null -> return null.
   * 2) Build a dictionary nodeById: id -> DataModelTree (shallow nodes with Model).
   * 3) Build adjacency (outgoing edges) for each DTO by inspecting its Fields:
   *    - only consider FieldType == LookupEntity or NestedEntity
   *    - collect distinct referenced entity ids that exist in nodeById and are not self or Guid.Empty
   * 4) Ensure startingModelId exists in nodeById; if not, return null.
   * 5) Perform DFS from startingModelId to collect reachable ids (HashSet reachable).
   * 6) Recursively build the subtree for reachable ids:
   *    - use a 'built' dictionary to avoid infinite recursion and to preserve graph structure
   *    - for each child id in adjacency, if in reachable set, BuildNode(child)
   *    - add child nodes to parent.Children and update child.ParentIds
   * 7) Return the built node corresponding to startingModelId (or null on failure).
   *
   * Notes:
   * - Method now returns a single DataModelTree? corresponding to the provided startingModelId.
   * - If no startingModelId is provided (not applicable here), caller must supply a valid id.
   */

  /// <summary>
  /// Postaví podstrom začínající od `startingModelId` (následujeme pouze outgoing references).
  /// Vrací kořenový uzel typu <see cref="DataModelTree"/> nebo null pokud není start nalezen.
  /// </summary>
  public static DataModelTree? BuildTree(List<DataModelDTO> dataModels, Guid startingModelId)
  {
    if (dataModels is null) return null;

    // Rychlá mapa id -> DTO a id -> uzel (prázdné)
    var nodeById = dataModels.ToDictionary(
      m => m.Id,
      m => new DataModelTree
      {
        Id = m.Id,
        Name = string.IsNullOrWhiteSpace(m.Name) ? m.Code : m.Name,
        Model = m
      });

    // Postavíme adjacency (outgoing edges) pouze pro relevantní field typy
    var adjacency = new Dictionary<Guid, List<Guid>>();
    foreach (var dto in dataModels)
    {
      var list = new List<Guid>();
      foreach (var field in dto.Fields ?? Enumerable.Empty<DataModelFieldDTO>())
      {
        if (field.FieldType != DataModelFieldType.LookupEntity && field.FieldType != DataModelFieldType.NestedEntity)
          continue;

        foreach (var rid in field.ReferencedEntityTypeIds ?? Enumerable.Empty<Guid>())
        {
          if (rid == Guid.Empty || rid == dto.Id) continue;
          if (!nodeById.ContainsKey(rid)) continue; // pouze pokud cíl existuje ve vstupu
          if (!list.Contains(rid)) list.Add(rid);
        }
      }
      adjacency[dto.Id] = list;
    }

    // start must exist
    if (!nodeById.ContainsKey(startingModelId))
      return null;

    // Najdeme všechny dosažitelné ID z startu (DFS)
    var reachable = new HashSet<Guid>();
    void Collect(Guid id)
    {
      if (reachable.Contains(id)) return;
      reachable.Add(id);
      if (!adjacency.TryGetValue(id, out var children)) return;
      foreach (var c in children)
      {
        Collect(c);
      }
    }

    Collect(startingModelId);

    // Rekurzivně sestavíme kopii podstromu pouze z reachable setu.
    var built = new Dictionary<Guid, DataModelTree>();
    DataModelTree BuildNode(Guid id)
    {
      if (built.TryGetValue(id, out var existing)) return existing;

      var src = nodeById[id];
      var node = new DataModelTree
      {
        Id = src.Id,
        Name = src.Name,
        Model = src.Model
      };
      built[id] = node;

      if (adjacency.TryGetValue(id, out var children))
      {
        foreach (var c in children)
        {
          if (!reachable.Contains(c)) continue;
          var childNode = BuildNode(c);
          if (!node.Children.Contains(childNode))
          {
            node.Children.Add(childNode);
            childNode.ParentIds.Add(node.Id);
          }
        }
      }

      return node;
    }

    var rootCopy = BuildNode(startingModelId);
    return rootCopy;
  }
}
