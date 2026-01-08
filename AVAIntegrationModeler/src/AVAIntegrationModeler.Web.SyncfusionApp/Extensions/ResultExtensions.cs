using Ardalis.Result;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Extensions;

/// <summary>
/// Extension methods for Ardalis.Result types.
/// </summary>
public static class ResultExtensions
{
  /// <summary>
  /// Converts Result<T> to Result, preserving status, errors, and validation errors.
  /// </summary>
  /// <typeparam name="T">The type of the Result value.</typeparam>
  /// <param name="result">The Result<T> to convert.</param>
  /// <returns>A Result without the typed value.</returns>
  public static Result ToResult<T>(this Result<T> result)
  {
    if (result.IsSuccess)
    {
      return Result.Success();
    }

    // Handle different result statuses
    return result.Status switch
    {
      ResultStatus.NotFound => Result.NotFound(),
      ResultStatus.Unauthorized => Result.Unauthorized(),
      ResultStatus.Forbidden => Result.Forbidden(),
      ResultStatus.Invalid => Result.Invalid(result.ValidationErrors),
      ResultStatus.Error => Result.Error(new ErrorList(result.Errors)),
      ResultStatus.Conflict => Result.Conflict(),
      ResultStatus.CriticalError => Result.CriticalError(result.Errors.ToArray()),
      ResultStatus.Unavailable => Result.Unavailable(result.Errors.ToArray()),
      _ => Result.Error(new ErrorList(result.Errors))
    };
  }
}
