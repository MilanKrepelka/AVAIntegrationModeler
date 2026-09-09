using System.Text.Json;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Services;

/// <summary>Načítá hinty integrační architektury z wwwroot/data/integration-hints.json a cachuje je po dobu životnosti aplikace.</summary>
public sealed class IntegrationHintService
{
    private readonly IReadOnlyList<IntegrationHint> _hints;

    public IntegrationHintService(IWebHostEnvironment env)
    {
        var path = Path.Combine(env.WebRootPath, "data", "integration-hints.json");
        var json = File.ReadAllText(path);
        _hints = JsonSerializer.Deserialize<List<IntegrationHint>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
    }

    /// <summary>Vrátí všechny hinty v pořadí dle čísla.</summary>
    public IReadOnlyList<IntegrationHint> GetHints() => _hints;
}
