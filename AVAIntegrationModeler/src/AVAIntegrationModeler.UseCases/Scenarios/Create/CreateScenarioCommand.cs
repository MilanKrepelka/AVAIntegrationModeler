using System.Diagnostics;
using System.Reflection;
using AVAIntegrationModeler.Core.ContributorAggregate;
using AVAIntegrationModeler.Core.ValueObjects;
using AVAIntegrationModeler.Core.ScenarioAggregate;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AVAIntegrationModeler.UseCases.Scenarios.Create;

/// <summary>
/// Create a new Scenario.
/// </summary>
/// <param name="Code">Jedinečný kód scénáře.</param>
/// <param name="Name">Lokalizovaný název scénáře.</param>
/// <param name="Decsription">Lokalizovaný popis scénáře.</param>
/// <param name="InputFeatureId">Id vstupní feature.</param>
/// <param name="OutputFeatureId">Id výstupní feature.</param>
public record CreateScenarioCommand(
    string Code,
    LocalizedValue Name,
    LocalizedValue Decsription,
    Guid? InputFeatureId,
    Guid? OutputFeatureId
) : Ardalis.SharedKernel.ICommand<Result<int>>;

