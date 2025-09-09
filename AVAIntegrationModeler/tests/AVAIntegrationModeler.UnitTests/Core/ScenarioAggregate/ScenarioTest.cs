using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Core.ValueObjects;
using Shouldly;

namespace AVAIntegrationModeler.UnitTests.Core.ScenarioAggregate;
public class ScenarioTest
{
    /// <summary>
    /// Ověřuje, že metoda SetCode správně nastaví kód scénáře.
    /// </summary>
    [Fact]
    public void Scenario_SetCode_Set_Code()
    {
        Scenario scenario = new Scenario(Guid.NewGuid());

        scenario.SetCode("TestCode");

        scenario.Code.ShouldBe("TestCode");
    }

    /// <summary>
    /// Ověřuje, že metoda SetId správně nastaví identifikátor scénáře.
    /// </summary>
    [Fact]
    public void Scenario_SetId_Sets_Id()
    {
        var originalId = Guid.NewGuid();
        var newId = Guid.NewGuid();
        Scenario scenario = new Scenario(originalId);

        scenario.SetId(newId);

        scenario.Id.ShouldBe(newId);
    }

    /// <summary>
    /// Ověřuje, že metoda SetName správně nastaví název scénáře.
    /// </summary>
    [Fact]
    public void Scenario_SetName_Sets_Name()
    {
        Scenario scenario = new Scenario(Guid.NewGuid());
        var name = new LocalizedValue { CzechValue = "Název", EnglishValue = "Name" };

        scenario.SetName(name);

        scenario.Name.ShouldBe(name);
    }

    /// <summary>
    /// Ověřuje, že metoda SetDescription správně nastaví popis scénáře.
    /// </summary>
    [Fact]
    public void Scenario_SetDescription_Sets_Description()
    {
        Scenario scenario = new Scenario(Guid.NewGuid());
        var description = new LocalizedValue { CzechValue = "Popis", EnglishValue = "Description" };

        scenario.SetDescription(description);

        scenario.Description.ShouldBe(description);
    }

    /// <summary>
    /// Ověřuje, že metoda SetInputFeature správně nastaví vstupní feature scénáře.
    /// </summary>
    [Fact]
    public void Scenario_SetInputFeature_Sets_InputFeature()
    {
        Scenario scenario = new Scenario(Guid.NewGuid());
        var feature = new Feature(Guid.NewGuid());

        scenario.SetInputFeature(feature);

        scenario.InputFeature.ShouldBe(feature);
    }

    /// <summary>
    /// Ověřuje, že metoda SetOutputFeature správně nastaví výstupní feature scénáře.
    /// </summary>
    [Fact]
    public void Scenario_SetOutputFeature_Sets_OutputFeature()
    {
        Scenario scenario = new Scenario(Guid.NewGuid());
        var feature = new Feature(Guid.NewGuid());

        scenario.SetOutputFeature(feature);

        scenario.OutputFeature.ShouldBe(feature);
    }
}
