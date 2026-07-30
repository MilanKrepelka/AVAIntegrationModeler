using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Areas.Get;

/// <summary>
/// Handler pro dotazy na oblast.
/// </summary>
public class GetAreaHandler(IAreasQueryService query)
  : IQueryHandler<GetAreaQuery, Result<AreaDTO>>, IQueryHandler<GetAreaByCodeQuery, Result<AreaDTO>>
{
  public async Task<Result<AreaDTO>> Handle(GetAreaQuery request, CancellationToken cancellationToken)
  {
    try
    {
      var area = await query.GetArea(request.Datasource, request.AreaId, cancellationToken);
      return Result.Success(area);
    }
    catch (NotFoundException)
    {
      return Result.NotFound();
    }
  }

  public async Task<Result<AreaDTO>> Handle(GetAreaByCodeQuery request, CancellationToken cancellationToken)
  {
    try
    {
      var area = await query.GetArea(request.Datasource, request.AreaCode, cancellationToken);
      return Result.Success(area);
    }
    catch (NotFoundException)
    {
      return Result.NotFound();
    }
  }
}
