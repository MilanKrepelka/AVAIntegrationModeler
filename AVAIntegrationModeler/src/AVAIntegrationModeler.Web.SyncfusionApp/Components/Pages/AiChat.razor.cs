using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.SyncfusionApp.AI;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

/// <summary>
/// Chatovací stránka pro analytiky — umožňuje dotazovat se na doménové modely pomocí Claude AI.
/// </summary>
public partial class AiChat : ComponentBase, IDisposable
{
    private bool _disposed;
    private CancellationTokenSource? _cts;

    [Inject] private DomainContextBuilder _contextBuilder { get; set; } = default!;
    [Inject] private AnthropicChatService _chatService { get; set; } = default!;

    private string _datasourceString = "Database";
    private Datasource _datasource = Datasource.Database;

    private readonly List<ChatMessage> _history = [];
    private string _input = string.Empty;
    private bool _isLoading;
    private bool _contextLoaded;
    private string? _errorMessage;
    private string _systemPrompt = string.Empty;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        _cts = new CancellationTokenSource();
        await LoadContextAsync();
    }

    private async Task LoadContextAsync()
    {
        if (_disposed) return;

        _contextLoaded = false;
        _errorMessage = null;
        await InvokeAsync(StateHasChanged);

        try
        {
            _systemPrompt = await _contextBuilder.BuildAsync(_datasource, _cts?.Token ?? CancellationToken.None);
            _contextLoaded = true;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            if (!_disposed)
                _errorMessage = $"Nepodařilo se načíst kontext doménových modelů: {ex.Message}";
        }

        if (!_disposed)
            await InvokeAsync(StateHasChanged);
    }

    private async Task OnDatasourceChangedAsync(Datasource ds)
    {
        _datasource = ds;
        _history.Clear();
        _errorMessage = null;
        await LoadContextAsync();
    }

    private async Task SendAsync()
    {
        if (_disposed || string.IsNullOrWhiteSpace(_input) || !_contextLoaded || _isLoading)
            return;

        var userMessage = _input.Trim();
        _input = string.Empty;
        _errorMessage = null;
        _isLoading = true;

        _history.Add(new ChatMessage("user", userMessage));
        await InvokeAsync(StateHasChanged);

        try
        {
            // Předáme historii bez poslední zprávy (service ji přidá sama)
            var historySnapshot = _history.Take(_history.Count - 1).ToList();

            var reply = await _chatService.SendMessageAsync(
                _systemPrompt,
                historySnapshot,
                userMessage,
                _cts?.Token ?? CancellationToken.None);

            if (!_disposed)
                _history.Add(new ChatMessage("assistant", reply));
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            if (!_disposed)
            {
                _history.RemoveAt(_history.Count - 1);
                _input = userMessage;
                _errorMessage = $"Chyba při komunikaci s AI: {ex.Message}";
            }
        }
        finally
        {
            if (!_disposed)
            {
                _isLoading = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && e.CtrlKey)
            await SendAsync();
    }

    private void ClearHistory()
    {
        _history.Clear();
        _errorMessage = null;
        StateHasChanged();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }
}
