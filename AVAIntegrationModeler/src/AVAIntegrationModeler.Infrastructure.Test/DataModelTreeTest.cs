using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Infrastructure.Test;

/// <summary>
/// Jednoduchá uzlová reprezentace datového modelu pro sestavení stromu vztahů.
/// Modely jsou propojeny směrem "zdroj -> referencovaný model" podle
/// `Field.ReferencedEntityTypeIds`.
/// </summary>
public class DataModelTreeTest
{
  [Fact]
  public void TestMethod()
  {
    var result = DataLoaderFromFile.LoadListFromFile<DataModelDTO>("DataModelsFromAVA.json", new string[] { "Data", "DataModels" });

    var treeBuilder = DataModelTree.BuildTree(result,new Guid("b6530960-bb27-4980-b1bf-80ba28e78e0e"));
    Assert.NotNull(treeBuilder!.Name);
  }
}
