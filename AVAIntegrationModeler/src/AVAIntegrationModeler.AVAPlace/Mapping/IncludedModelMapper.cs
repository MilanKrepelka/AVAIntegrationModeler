using System;
using System.Collections.Generic;
using System.Linq;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.AVAPlace.Mapping;

/// <summary>
/// Statická třída pro mapování zahrnutých datových modelů.
/// </summary>
public static class IncludedModelMapper
{
  /// <summary>
  /// Převede datový přenosový objekt zahrnutého modelu (<see cref="ASOL.DataService.Contracts.IntegrationFeatureDefinition.IncludedModel"/>) na jeho datový přenosový objekt (<see cref="Contracts.DTO.IncludedDataModelDTO"/>).
  /// </summary>
  /// <param name="includedModel"><see cref="ASOL.DataService.Contracts.IntegrationFeatureDefinition.IncludedModel"/></param>
  /// <param name="models">Datové modely které se používají k doplňování</param>
  /// <returns><see cref="Contracts.DTO.IncludedDataModelDTO"/></returns>
  public static Contracts.DTO.IncludedDataModelDTO IncludedDataModelDTO(
    ASOL.DataService.Contracts.IntegrationFeatureDefinition.IncludedModel includedModel,
    IEnumerable<Contracts.DTO.DataModelSummaryDTO> models)
  {
    if (includedModel == null)
    {
      throw new ArgumentNullException(nameof(includedModel), $"{nameof(IncludedModelMapper)} - {nameof(IncludedDataModelDTO)}");
    }

    DataModelSummaryDTO modelSummary = Contracts.DTO.DataModelSummaryDTO.Empty;
    
    if (Guid.TryParse(includedModel.CodeOrId, out Guid guid))
    {
      modelSummary = models?.FirstOrDefault(m => m.Id == Guid.Parse(includedModel.CodeOrId)) ?? DataModelSummaryDTO.Empty;
    }
    else
    {
      modelSummary = models?.FirstOrDefault(m => m.Code == includedModel.CodeOrId) ?? DataModelSummaryDTO.Empty;
    }

    return new Contracts.DTO.IncludedDataModelDTO()
    {
      ReadOnly = includedModel.ReadOnly,
      DataModel = modelSummary ?? Contracts.DTO.DataModelSummaryDTO.Empty,
    };
  }
}
