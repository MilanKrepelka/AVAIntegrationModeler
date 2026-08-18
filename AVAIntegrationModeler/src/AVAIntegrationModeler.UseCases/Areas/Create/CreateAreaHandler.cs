using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.AreaAggregate;
using AVAIntegrationModeler.UseCases.Areas.Mapping;

namespace AVAIntegrationModeler.UseCases.Areas.Create;

/// <summary>
/// Handler pro příkaz vytvoření oblasti.
/// </summary>
public class CreateAreaHandler(
  IRepository<Area> areaRepository,
  IAreasQueryService areasQueryService,
  IDomainEntityValidationService<Area> areaValidationService
) : ICommandHandler<CreateAreaCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateAreaCommand request, CancellationToken cancellationToken)
  {
    Area area;
    try
    {
      area = new Area(request.Area.Id == Guid.Empty ? Guid.NewGuid() : request.Area.Id, request.Area.Code);
      area.SetName(request.Area.Name);
    }
    catch (ArgumentException ex)
    {
      return Result<Guid>.Invalid(new ValidationError
      {
        Identifier = ex.ParamName ?? "Area",
        ErrorMessage = ex.Message
      });
    }
    catch (Exception ex)
    {
      return Result<Guid>.Invalid(new ValidationError
      {
        Identifier = "Area",
        ErrorMessage = ex.Message
      });
    }

    var validationResult = await areaValidationService.ValidateForCreate(request.Datasource, area, cancellationToken);
    if (!validationResult.IsSuccess)
      return validationResult;

    Area? created;
    try
    {
      created = await areaRepository.AddAsync(area, cancellationToken);
      areasQueryService.InvalidateCache(request.Datasource);
    }
    catch (Exception ex)
    {
      return Result<Guid>.Error($"Oblast se nepodařilo vytvořit: {ex.Message}");
    }

    if (created is null)
      return Result<Guid>.Error("Oblast nebyla vytvořena.");

    return Result<Guid>.Success(created.Id);
  }
}
