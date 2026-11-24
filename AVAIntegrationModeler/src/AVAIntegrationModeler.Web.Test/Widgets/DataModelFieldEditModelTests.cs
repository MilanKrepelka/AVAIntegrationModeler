using AVAIntegrationModeler.Web.SyncfusionApp.Components.Widgets;
using Shouldly;

namespace AVAIntegrationModeler.Web.Test.Widgets;

/// <summary>
/// Testy validace povinných položek editačního modelu pole datového modelu.
/// </summary>
public class DataModelFieldEditModelTests
{
  /// <summary>
  /// Vyplněný název, popisek i popis nevrací žádnou chybu.
  /// </summary>
  [Fact]
  public void Validace_VyplneneVsechnyPovinnePolozky_BezChyb()
  {
    var model = new DataModelFieldEditModel { Name = "Code", Label = "Kód", Description = "Kód záznamu" };

    model.GetNameError().ShouldBeEmpty();
    model.GetLabelError().ShouldBeEmpty();
    model.GetDescriptionError().ShouldBeEmpty();
  }

  /// <summary>
  /// Prázdný nebo jen z mezer složený popisek je chyba.
  /// </summary>
  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public void GetLabelError_PrazdnyPopisek_VraciChybu(string label)
  {
    var model = new DataModelFieldEditModel { Name = "Code", Label = label, Description = "Kód záznamu" };

    model.GetLabelError().ShouldBe(DataModelFieldEditModel.LabelRequiredMessage);
  }

  /// <summary>
  /// Prázdný nebo jen z mezer složený popis je chyba.
  /// </summary>
  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public void GetDescriptionError_PrazdnyPopis_VraciChybu(string description)
  {
    var model = new DataModelFieldEditModel { Name = "Code", Label = "Kód", Description = description };

    model.GetDescriptionError().ShouldBe(DataModelFieldEditModel.DescriptionRequiredMessage);
  }

  /// <summary>
  /// Prázdný nebo jen z mezer složený název je chyba.
  /// </summary>
  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public void GetNameError_PrazdnyNazev_VraciChybu(string name)
  {
    var model = new DataModelFieldEditModel { Name = name, Label = "Kód", Description = "Kód záznamu" };

    model.GetNameError().ShouldBe(DataModelFieldEditModel.NameRequiredMessage);
  }

  /// <summary>
  /// Nový editační model bez vyplněných hodnot hlásí chybu názvu, popisku i popisu.
  /// </summary>
  [Fact]
  public void Validace_NovyModel_HlasiVsechnyChyby()
  {
    var model = new DataModelFieldEditModel();

    model.GetNameError().ShouldNotBeEmpty();
    model.GetLabelError().ShouldNotBeEmpty();
    model.GetDescriptionError().ShouldNotBeEmpty();
  }
}
