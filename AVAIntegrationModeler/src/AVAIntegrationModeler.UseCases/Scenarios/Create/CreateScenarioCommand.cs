using System.Diagnostics;
using System.Reflection;
using AVAIntegrationModeler.Domain.ContributorAggregate;
using AVAIntegrationModeler.Domain.ValueObjects;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AVAIntegrationModeler.UseCases.Scenarios.Create;

/// <summary>
/// Create a new Scenario.
/// </summary>
/// <param name="Id">Identifikátor scénáře.</param>
/// <param name="Code">Jedinečný kód scénáře.</param>
/// <param name="Name">Lokalizovaný název scénáře.</param>
/// <param name="Decsription">Lokalizovaný popis scénáře.</param>
/// <param name="InputFeatureId">Id vstupní feature.</param>
/// <param name="OutputFeatureId">Id výstupní feature.</param>
public record CreateScenarioCommand(
    Guid Id,
    string Code,
    LocalizedValue Name,
    LocalizedValue Decsription,
    Guid? InputFeatureId,
    Guid? OutputFeatureId
) : Ardalis.SharedKernel.ICommand<Result<Guid>>;

