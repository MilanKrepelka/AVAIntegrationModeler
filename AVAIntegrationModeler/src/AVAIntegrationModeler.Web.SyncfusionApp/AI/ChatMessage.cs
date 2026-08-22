namespace AVAIntegrationModeler.Web.SyncfusionApp.AI;

/// <summary>
/// Zpráva v chatovacím rozhovoru.
/// </summary>
/// <param name="Role">Role autora — "user" nebo "assistant".</param>
/// <param name="Content">Text zprávy.</param>
public record ChatMessage(string Role, string Content);
