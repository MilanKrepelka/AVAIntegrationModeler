namespace AVAIntegrationModeler.Web.SyncfusionApp.AI;

/// <summary>
/// Konfigurace pro Anthropic Claude API.
/// </summary>
public class AnthropicOptions
{
    /// <summary>
    /// API klíč. Pokud je prázdný, použije se proměnná prostředí ANTHROPIC_API_KEY.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Identifikátor modelu Claude (např. "claude-opus-5").
    /// </summary>
    public string Model { get; set; } = "claude-opus-5";
}
