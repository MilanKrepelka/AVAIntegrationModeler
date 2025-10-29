using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ASOL.DataService.Connector;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;


namespace AVAIntegrationModeler.AVAPlace.Mapping;
/// <summary>
/// Statická třída pro mapování mezi doménovým objektem scénáře (<see cref="AVAIntegrationModeler.Contracts"/>) a jeho datovým přenosovým objektem (<see cref="FeatureDTO"/>).
/// Poskytuje metody pro převod mezi těmito dvěma reprezentacemi.
/// </summary>
public static class FeatureMapper
{
  /// <summary>
  /// Převede datový přenosový objekt feature (<see cref="IntegrationFeatureSummary"/>) na jeho datový přenosový objekt (<see cref="FeatureSummaryDTO"/>).
  /// </summary>
  /// <param name="integrationDefenitionSummary"><see cref="IntegrationFeatureSummary"/></param>
  /// <returns><see cref="FeatureSummaryDTO"/></returns>
  public static FeatureSummaryDTO FeatureSummaryDTO(IntegrationFeatureSummary integrationDefenitionSummary)
  {
    Guard.Against.Null(integrationDefenitionSummary, $"{nameof(FeatureMapper)} - {nameof(integrationDefenitionSummary)}");
    Guard.Against.NullOrEmpty(integrationDefenitionSummary.Id, $"{nameof(FeatureMapper)} - {nameof(integrationDefenitionSummary)} - {nameof(integrationDefenitionSummary.Id)}");

    return new FeatureSummaryDTO()
    {
      Code = integrationDefenitionSummary.Code,
      Id = Guid.Parse(integrationDefenitionSummary.Id),
    };
  }

  /// <summary>
  /// Převede datový přenosový objekt feature (<see cref="IntegrationFeatureSummary"/>) na jeho datový přenosový objekt (<see cref="FeatureSummaryDTO"/>).
  /// </summary>
  /// <param name="integrationDefenitionSummary"><see cref="IntegrationFeatureSummary"/></param>
  /// <returns><see cref="FeatureSummaryDTO"/></returns>
  public static FeatureDTO FeatureDTO(IntegrationFeatureSummary integrationDefenitionSummary)
  {
    Guard.Against.Null(integrationDefenitionSummary, $"{nameof(FeatureMapper)} - {nameof(integrationDefenitionSummary)}");
    Guard.Against.NullOrEmpty(integrationDefenitionSummary.Id, $"{nameof(FeatureMapper)} - {nameof(integrationDefenitionSummary)} - {nameof(integrationDefenitionSummary.Id)}");

    return new FeatureDTO()
    {
      Code = integrationDefenitionSummary.Code,
      Id = Guid.Parse(integrationDefenitionSummary.Id),
    };
  }
}
