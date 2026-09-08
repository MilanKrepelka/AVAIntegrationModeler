namespace AVAIntegrationModeler.Web.SyncfusionApp.Services;

/// <summary>Jeden hint integrační architektury načtený z JSON souboru.</summary>
public sealed class IntegrationHint
{
    public int Number { get; init; }
    public string Title { get; init; } = string.Empty;
    public IReadOnlyList<string> Points { get; init; } = [];
}
