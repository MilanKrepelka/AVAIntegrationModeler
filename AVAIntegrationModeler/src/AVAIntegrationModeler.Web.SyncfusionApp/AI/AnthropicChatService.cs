using Anthropic;
using Anthropic.Models.Messages;
using Microsoft.Extensions.Options;

namespace AVAIntegrationModeler.Web.SyncfusionApp.AI;

/// <summary>
/// Odesílá zprávy do Anthropic Claude API s kontextem doménových modelů.
/// </summary>
public class AnthropicChatService
{
    private readonly AnthropicClient _client;
    private readonly string _model;

    /// <summary>
    /// Inicializuje novou instanci <see cref="AnthropicChatService"/>.
    /// </summary>
    public AnthropicChatService(IOptions<AnthropicOptions> options)
    {
        var opts = options.Value;
        _client = string.IsNullOrWhiteSpace(opts.ApiKey)
            ? new AnthropicClient()
            : new AnthropicClient { ApiKey = opts.ApiKey };
        _model = opts.Model;
    }

    /// <summary>
    /// Odešle zprávu Claudovi a vrátí jeho odpověď jako text.
    /// </summary>
    /// <param name="systemPrompt">Systémový prompt s kontextem doménových modelů.</param>
    /// <param name="history">Předchozí zprávy v konverzaci (bez aktuální user zprávy).</param>
    /// <param name="userMessage">Aktuální dotaz uživatele.</param>
    /// <param name="ct">Token pro zrušení operace.</param>
    public async Task<string> SendMessageAsync(
        string systemPrompt,
        IReadOnlyList<ChatMessage> history,
        string userMessage,
        CancellationToken ct)
    {
        var messages = history
            .Select(m => new MessageParam
            {
                Role = m.Role == "user" ? Role.User : Role.Assistant,
                Content = m.Content
            })
            .ToList();

        messages.Add(new MessageParam { Role = Role.User, Content = userMessage });

        var response = await _client.Messages.Create(new MessageCreateParams
        {
            Model = _model,
            MaxTokens = 4096,
            System = systemPrompt,
            Messages = messages
        }, cancellationToken: ct);

        return response.Content
            .Select(b => b.Value)
            .OfType<TextBlock>()
            .FirstOrDefault()?.Text ?? string.Empty;
    }
}
