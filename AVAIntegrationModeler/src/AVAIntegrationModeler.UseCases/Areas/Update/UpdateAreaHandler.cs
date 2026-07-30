using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.AreaAggregate;

namespace AVAIntegrationModeler.UseCases.Areas.Update;

/// <summary>
/// Handler pro příkaz aktualizace oblasti.
/// </summary>
public class UpdateAreaHandler(
  IRepository<Area> repository,
  IAreasQueryService areasQueryService
) : ICommandHandler<UpdateAreaCommand, Result<AreaDTO>>
{
  public async Task<Result<AreaDTO>> Handle(UpdateAreaCommand request, CancellationToken cancellationToken)
  {
    Guard.Against.Null(request, nameof(request));
    Guard.Against.Null(request.Area, nameof(request.Area));

    var existing = await repository.GetByIdAsync(request.Area.Id, cancellationToken);
    if (existing == null)
      return Result.NotFound();

    try
    {
      existing.SetCode(request.Area.Code);
      existing.SetName(request.Area.Name);
    }
    catch (ArgumentException ex)
    {
      return Result<AreaDTO>.Invalid(new ValidationError
      {
        Identifier = ex.ParamName ?? "Area",
        ErrorMessage = ex.Message
      });
    }

    await repository.UpdateAsync(existing, cancellationToken);
    areasQueryService.InvalidateCache(request.Datasource);
    return Result<AreaDTO>.Success(request.Area);
  }
}
