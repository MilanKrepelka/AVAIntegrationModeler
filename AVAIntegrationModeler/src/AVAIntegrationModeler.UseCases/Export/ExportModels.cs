namespace AVAIntegrationModeler.UseCases.Export;

public record ExportEntry<T>(string FileName, T Data);
public record ExportResult<T>(string ZipFileName, IEnumerable<ExportEntry<T>> Entries);
