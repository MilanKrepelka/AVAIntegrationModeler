using System;
using System.Collections.Generic;

namespace AVAIntegrationModeler.API.Client;

/// <summary>
/// Výjimka reprezentující validační chyby z API (400 ValidationProblemDetails).
/// </summary>
public class ApiValidationException : Exception
{
    /// <summary>
    /// Slovník chyb z ValidationProblemDetails: klíč = název pole, hodnota = pole chybových zpráv.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// HTTP status kód (obvykle 400).
    /// </summary>
    public int StatusCode { get; }

    public ApiValidationException(int statusCode, IDictionary<string, string[]> errors, string? message = null)
        : base(message ?? "Validační chyba z API.")
    {
        StatusCode = statusCode;
        Errors = errors ?? new Dictionary<string, string[]>();
    }
}

/// <summary>
/// Obecná API výjimka pro ostatní chyby (404, 409, 500...).
/// </summary>
public class ApiException : Exception
{
    public int StatusCode { get; }
    public string? Detail { get; }
    public string? Title { get; }

    public ApiException(int statusCode, string? title, string? detail)
        : base(detail ?? title ?? $"API chyba se statusem {statusCode}")
    {
        StatusCode = statusCode;
        Title = title;
        Detail = detail;
    }
}
