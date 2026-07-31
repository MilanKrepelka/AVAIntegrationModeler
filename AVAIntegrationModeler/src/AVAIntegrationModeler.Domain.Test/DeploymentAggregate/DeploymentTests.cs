using AVAIntegrationModeler.Domain.DeploymentAggregate;
using Ardalis.GuardClauses;
using Shouldly;

namespace AVAIntegrationModeler.Domain.Test.DeploymentAggregate;

/// <summary>
/// Unit testy pro agregát Deployment.
/// </summary>
public class DeploymentTests
{
  [Fact]
  public void Konstruktor_NastaviIdAKod()
  {
    var id = Guid.NewGuid();
    var dep = new Deployment(id, "DEP-001");

    dep.Id.ShouldBe(id);
    dep.Code.ShouldBe("DEP-001");
  }

  [Fact]
  public void SetName_NastaviNazev()
  {
    var dep = new Deployment(Guid.NewGuid(), "DEP-001");
    dep.SetName("Nasazení pro zákazníky");

    dep.Name.ShouldBe("Nasazení pro zákazníky");
  }

  [Fact]
  public void SetCode_NastaviKod()
  {
    var dep = new Deployment(Guid.NewGuid(), "DEP-001");
    dep.SetCode("DEP-002");

    dep.Code.ShouldBe("DEP-002");
  }

  [Fact]
  public void AddDataModel_PridaModel()
  {
    var dep = new Deployment(Guid.NewGuid(), "DEP-001");
    var modelId = Guid.NewGuid();
    dep.AddDataModel(modelId);

    dep.DataModels.ShouldHaveSingleItem();
    dep.DataModels.First().DataModelId.ShouldBe(modelId);
  }

  [Fact]
  public void AddDataModel_DuplikatIgnoruje()
  {
    var dep = new Deployment(Guid.NewGuid(), "DEP-001");
    var modelId = Guid.NewGuid();
    dep.AddDataModel(modelId);
    dep.AddDataModel(modelId);

    dep.DataModels.Count.ShouldBe(1);
  }

  [Fact]
  public void RemoveDataModel_OdebreModel()
  {
    var dep = new Deployment(Guid.NewGuid(), "DEP-001");
    var modelId = Guid.NewGuid();
    dep.AddDataModel(modelId);
    dep.RemoveDataModel(modelId);

    dep.DataModels.ShouldBeEmpty();
  }

  [Fact]
  public void RemoveDataModel_NeexistujiciIgnoruje()
  {
    var dep = new Deployment(Guid.NewGuid(), "DEP-001");
    Should.NotThrow(() => dep.RemoveDataModel(Guid.NewGuid()));
  }

  [Fact]
  public void AddDataModel_PrazdnyGuid_VyhodiVyjimku()
  {
    var dep = new Deployment(Guid.NewGuid(), "DEP-001");
    Should.Throw<ArgumentException>(() => dep.AddDataModel(Guid.Empty));
  }

  [Fact]
  public void SetCode_PrazdnyRetezec_VyhodiVyjimku()
  {
    var dep = new Deployment(Guid.NewGuid(), "DEP-001");
    Should.Throw<ArgumentException>(() => dep.SetCode(string.Empty));
  }

  [Fact]
  public void SetName_PrazdnyRetezec_VyhodiVyjimku()
  {
    var dep = new Deployment(Guid.NewGuid(), "DEP-001");
    Should.Throw<ArgumentException>(() => dep.SetName(string.Empty));
  }

  [Fact]
  public void FluentAPI_ReturnujeInstanci()
  {
    var dep = new Deployment(Guid.NewGuid(), "DEP-001");
    var result = dep.SetName("Nasazení").SetCode("DEP-002");

    result.ShouldBeSameAs(dep);
  }

  [Fact]
  public void DataModels_JeReadOnly()
  {
    var dep = new Deployment(Guid.NewGuid(), "DEP-001");
    dep.DataModels.ShouldBeAssignableTo<IReadOnlyCollection<DeploymentDataModel>>();
  }

  [Fact]
  public void AddDataModel_ViceModelu_VsechnyUlozeny()
  {
    var dep = new Deployment(Guid.NewGuid(), "DEP-001");
    var id1 = Guid.NewGuid();
    var id2 = Guid.NewGuid();
    var id3 = Guid.NewGuid();

    dep.AddDataModel(id1).AddDataModel(id2).AddDataModel(id3);

    dep.DataModels.Count.ShouldBe(3);
  }
}
