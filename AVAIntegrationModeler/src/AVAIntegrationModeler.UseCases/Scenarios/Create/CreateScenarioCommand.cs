using System.Diagnostics;
using System.Reflection;
using AVAIntegrationModeler.Domain.ContributorAggregate;
using AVAIntegrationModeler.Domain.ValueObjects;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Scenarios.Create;

/// <summary>
/// Příkaz pro vytvoření nového integračního scénáře.
/// </summary>
/// <param name="Datasource"><see cref="AVAIntegrationModeler.Contracts.Datasource"/></param>
/// <param name="Scenario"><see cref="AVAIntegrationModeler.Contracts.DTO.ScenarioDTO"/></param>
public record CreateScenarioCommand(
    Datasource Datasource,
    ScenarioDTO Scenario
) : Ardalis.SharedKernel.ICommand<Result<Guid>>;

public record CreateScenarioCommand2(Datasource Datasource,
    ScenarioDTO Scenario) : FastEndpoints.ICommand<Result<Guid>>;

