namespace AVAIntegrationModeler.UseCases.Areas.Delete;

/// <summary>
/// Příkaz pro smazání oblasti.
/// </summary>
/// <param name="AreaCode">Kód oblasti, která má být smazána.</param>
public record DeleteAreaCommand(string AreaCode) : ICommand<Result>;
