using System;
using System.Collections.Generic;
using System.Linq;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Infrastructure;

  /// <summary>
  /// Jednoduchá uzlová reprezentace datového modelu pro sestavení stromu vztahů.
  /// Modely jsou propojeny směrem "zdroj -> referencovaný model" podle
  /// `Field.ReferencedEntityTypeIds`.
  /// </summary>
  public class DataModelTree
  {
      /// <summary>
      /// Identifikátor datového modelu.
      /// </summary>
      public Guid Id { get; init; }

      /// <summary>
      /// Zobrazitelný název (pokud je dostupný).
      /// </summary>
      public string? Name { get; init; }

      /// <summary>
      /// Originální DTO (pokud je potřeba další metadata).
      /// </summary>
      public DataModelDTO? Model { get; init; }

      /// <summary>
      /// Děti (modely, na které tento model odkazuje).
      /// </summary>
      public List<DataModelTree> Children { get; } = new();

      /// <summary>
      /// Seznam identifikátorů rodičů (modely, které odkazují na tento model).
      /// Může být více rodičů (grafová povaha dat).
      /// </summary>
      public HashSet<Guid> ParentIds { get; } = new();

      /// <summary>
      /// Postaví les (seznam kořenových uzlů). Směr hran: model -> referencovaný model.
      /// Ochrana proti cyklům: pokud by přidání hrany vytvořilo cyklus, tato hrana
      /// se přeskočí.
      /// </summary>
      /// <param name="dataModels">Vstupní seznam datových modelů</param>
      /// <returns>Seznam kořenů stromu/lesa</returns>
      public static List<DataModelTree> BuildTree(List<DataModelDTO> dataModels)
      {
          if (dataModels is null) return new List<DataModelTree>();

          // Vytvoříme uzly pro každý model
          var nodeById = dataModels.ToDictionary(
              m => m.Id,
              m => new DataModelTree
              {
                  Id = m.Id,
                  Name = string.IsNullOrWhiteSpace(m.Name) ? m.Code : m.Name,
                  Model = m
              });

          // Pomocná funkce pro zjištění, zda existuje cesta z fromId do targetId.
          // Použito k detekci cyklů před přidáním nové hrany.
          bool HasPath(Guid fromId, Guid targetId, HashSet<Guid>? visited = null)
          {
              if (fromId == targetId) return true;
              visited ??= new HashSet<Guid>();
              if (!nodeById.TryGetValue(fromId, out var start)) return false;
              visited.Add(fromId);
              foreach (var child in start.Children)
              {
                  if (visited.Contains(child.Id)) continue;
                  if (child.Id == targetId) return true;
                  if (HasPath(child.Id, targetId, visited)) return true;
              }
              return false;
          }

          // Pro každý model přidáme hrany na referencované modely (pokud existují v kolekci).
          foreach (var model in dataModels)
          {
              if (!nodeById.TryGetValue(model.Id, out var sourceNode)) continue;

              var fields = model.Fields ?? Enumerable.Empty<DataModelFieldDTO>();
              foreach (var field in fields)
              {
                  var referencedIds = field.ReferencedEntityTypeIds ?? Enumerable.Empty<Guid>();
                  foreach (var referencedId in referencedIds)
                  {
                      // pokud referencedId neexistuje ve vstupu, přeskočíme (externí reference)
                      if (!nodeById.TryGetValue(referencedId, out var targetNode)) continue;

                      // detekce cyklu: jestli existuje cesta z targetNode zpět do sourceNode,
                      // přidáním této hrany bychom vytvořili cyklus -> přeskočíme ji
                      if (HasPath(targetNode.Id, sourceNode.Id))
                      {
                          // volitelně můžete logovat/skýtnout informace o přeskočené hraně
                          continue;
                      }

                      // přidání hrany (source -> target)
                      if (!sourceNode.Children.Contains(targetNode))
                      {
                          sourceNode.Children.Add(targetNode);
                          targetNode.ParentIds.Add(sourceNode.Id);
                      }
                  }
              }
          }

          // Kořeny = uzly, které na ně nikdo neodkazuje
          var roots = nodeById.Values
              .Where(n => n.ParentIds.Count == 0)
              .OrderBy(n => n.Name, StringComparer.OrdinalIgnoreCase)
              .ToList();

          // Pokud nejsou žádné kořeny (např. všechny uzly jsou v bloku s cykly nebo vzájemně
          // odkazují), jako fallback vrátíme všechny uzly (alespoň aby neby prázdný výsledek).
          if (roots.Count == 0)
          {
              roots = nodeById.Values.ToList();
          }

          return roots;
      }
  }

