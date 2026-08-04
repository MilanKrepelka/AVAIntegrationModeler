namespace AVAIntegrationModeler.Contracts.DTO;

/// <summary>
/// Entry exportu nasazení — definice DataModelu spolu s jeho záznamy.
/// </summary>
public record DeploymentDataModelExportEntryDTO(DataModelDTO Model, IEnumerable<DataModelRecordDTO> Records);
