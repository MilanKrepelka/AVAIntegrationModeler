using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;
using Pathoschild.Http.Client;

namespace AVAIntegrationModeler.API.Client;

public static class FluentClientExtensions
{
  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };

  // FastEndpoints error response format: { statusCode, message, errors: { field: [msg] } }
  private sealed class FastEndpointsErrorResponse
  {
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, List<string>> Errors { get; set; } = [];
  }

  /// <summary>
  /// Zpracuje request a vrátí Result&lt;T&gt; místo výjimek.
  /// </summary>
  public static async Task<Result<T>> AsResult<T>(this IRequest request)
  {
    try
    {
      // AsResponse() přechází přes Pathoschild error filter, který hází ApiException pro non-2xx.
      // Success kódy (2xx) ale projdou bez výjimky.
      var response = await request.AsResponse();

      if (response.Status == HttpStatusCode.NoContent)
        return Result<T>.NoContent();

      if (response.Status == HttpStatusCode.OK || response.Status == HttpStatusCode.Created)
      {
        var data = await response.As<T>();
        return Result<T>.Success(data);
      }

      return Result<T>.Error($"HTTP {(int)response.Status}");
    }
    catch (Pathoschild.Http.Client.ApiException apiEx)
    {
      // Pathoschild hází ApiException pro non-2xx — zachytíme a namapujeme na Result
      var status = apiEx.Response.Status;

      if (status == HttpStatusCode.NotFound)
        return Result<T>.NotFound();

      if (status == HttpStatusCode.NoContent)
        return Result<T>.NoContent();

      string content;
      try { content = await apiEx.Response.AsString(); }
      catch { content = string.Empty; }

      if (status == HttpStatusCode.BadRequest)
      {
        // FastEndpoints formát: { errors: { field: [msg] } }
        try
        {
          var feError = JsonSerializer.Deserialize<FastEndpointsErrorResponse>(content, JsonOptions);
          if (feError?.Errors is { Count: > 0 })
          {
            var validationErrors = feError.Errors
              .SelectMany(kvp => kvp.Value.Select(msg => new ValidationError
              {
                Identifier = kvp.Key,
                ErrorMessage = msg
              }))
              .ToArray();
            return Result<T>.Invalid(validationErrors);
          }
        }
        catch (JsonException) { }

        // ValidationProblemDetails formát
        try
        {
          var vpd = JsonSerializer.Deserialize<ValidationProblemDetails>(content, JsonOptions);
          if (vpd?.Errors is { Count: > 0 })
          {
            var validationErrors = vpd.Errors
              .SelectMany(kvp => kvp.Value.Select(msg => new ValidationError
              {
                Identifier = kvp.Key,
                ErrorMessage = msg
              }))
              .ToArray();
            return Result<T>.Invalid(validationErrors);
          }
        }
        catch (JsonException) { }

        return Result<T>.Error(content);
      }

      try
      {
        var pd = JsonSerializer.Deserialize<ProblemDetails>(content, JsonOptions);
        return status switch
        {
          HttpStatusCode.Conflict => Result<T>.Conflict(pd?.Detail ?? content),
          HttpStatusCode.Forbidden => Result<T>.Forbidden(),
          HttpStatusCode.Unauthorized => Result<T>.Unauthorized(),
          _ => Result<T>.Error(pd?.Detail ?? content)
        };
      }
      catch (JsonException)
      {
        return Result<T>.Error($"HTTP {(int)status}: {content}");
      }
    }
    catch (Exception ex)
    {
      return Result<T>.Error(ex.Message);
    }
  }



  /// <summary>
  /// Původní varianta - hází výjimky (pro backward compatibility).
  /// </summary>
  public static async Task<T> WithApiExceptionHandling<T>(this IRequest request)
  {
    var response = await request.AsResponse();

    if (response.Status == HttpStatusCode.OK || response.Status == HttpStatusCode.Created)
    {
      return await response.As<T>();
    }

    var content = await response.AsString();

    if (response.Status == HttpStatusCode.BadRequest)
    {
      try
      {
        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(content, JsonOptions);
        if (problemDetails?.Errors != null)
        {
          throw new ApiValidationException(
              (int)response.Status,
              problemDetails.Errors,
              problemDetails.Title);
        }
      }
      catch (JsonException)
      {
        // Pokračuj k obecnému zpracování
      }
    }

    try
    {
      var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, JsonOptions);
      throw new ApiException(
          (int)response.Status,
          problemDetails?.Title,
          problemDetails?.Detail);
    }
    catch (JsonException)
    {
      throw new ApiException((int)response.Status, "API Error", content);
    }
  }
}

