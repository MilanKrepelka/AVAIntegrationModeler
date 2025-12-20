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

  /// <summary>
  /// Zpracuje request a vrátí Result&lt;T&gt; místo výjimek.
  /// </summary>
  public static async Task<Result<T>> AsResult<T>(this IRequest request)
  {
    try
    {
      var response = await request.AsResponse();

      if (response.Status == HttpStatusCode.NoContent)
      {
        return Result<T>.NoContent();
      }
      if (response.Status == HttpStatusCode.OK || response.Status == HttpStatusCode.Created)
      {
        var data = await response.As<T>();
        return Result<T>.Success(data);
      }

      var content = await response.AsString();

      // Parsování ValidationProblemDetails (400)
      if (response.Status == HttpStatusCode.BadRequest)
      {
        try
        {
          var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(content, JsonOptions);
          if (problemDetails?.Errors != null)
          {
            var validationErrors = problemDetails.Errors
                .SelectMany(kvp => kvp.Value.Select(msg => new ValidationError
                {
                  Identifier = kvp.Key,
                  ErrorMessage = msg
                }))
                .ToArray(); // ← Převod na pole

            return Result<T>.Invalid(validationErrors);
          }
        }
        catch (JsonException)
        {
          // Pokračuj k obecnému parsování
        }
      }

      // Parsování obecného ProblemDetails
      try
      {
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, JsonOptions);

        return response.Status switch
        {
          HttpStatusCode.NotFound => Result<T>.NotFound(),
          HttpStatusCode.Conflict => Result<T>.Conflict(problemDetails?.Detail ?? content),
          HttpStatusCode.Forbidden => Result<T>.Forbidden(),
          HttpStatusCode.Unauthorized => Result<T>.Unauthorized(),
          _ => Result<T>.Error(problemDetails?.Detail ?? content)
        };
      }
      catch (JsonException)
      {
        return Result<T>.Error($"HTTP {(int)response.Status}: {content}");
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

