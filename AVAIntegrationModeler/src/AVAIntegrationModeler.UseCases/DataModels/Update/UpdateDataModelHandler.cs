using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.UseCases.DataModels.Mapping;

namespace AVAIntegrationModeler.UseCases.DataModels.Update;

public class UpdateDataModelHandler(IRepository<DataModel> repository, IDataModelQueryService queryService)
  : ICommandHandler<UpdateDataModelCommand, Result<DataModelDTO>>
{
  public async Task<Result<DataModelDTO>> Handle(UpdateDataModelCommand request, CancellationToken cancellationToken)
  {
    Guard.Against.Null(request.DataModel, nameof(request.DataModel));

    var existing = await repository.GetByIdAsync(request.DataModel.Id, cancellationToken);
    if (existing is null) return Result<DataModelDTO>.NotFound();

    try
    {
      existing
        .SetCode(request.DataModel.Code)
        .SetName(request.DataModel.Name)
        .SetDescription(request.DataModel.Description)
        .SetNotes(request.DataModel.Notes);

      if (request.DataModel.IsAggregateRoot)
        existing.MarkAsAggregateRoot();
      else
        existing.MarkAsNestedEntity();
    }
    catch (ArgumentException ex)
    {
      return Result<DataModelDTO>.Invalid(new ValidationError(ex.ParamName ?? "DataModel", ex.Message));
    }

    await repository.UpdateAsync(existing, cancellationToken);
    queryService.InvalidateCache(request.Datasource);
    return Result<DataModelDTO>.Success(request.DataModel);
  }
}
