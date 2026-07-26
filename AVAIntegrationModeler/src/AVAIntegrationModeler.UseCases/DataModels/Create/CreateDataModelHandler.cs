using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.Domain.DataModelAggregate.Specifications;

namespace AVAIntegrationModeler.UseCases.DataModels.Create;

public class CreateDataModelHandler(IRepository<DataModel> repository, IDataModelQueryService queryService)
  : ICommandHandler<CreateDataModelCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateDataModelCommand request, CancellationToken cancellationToken)
  {
    Guard.Against.Null(request.DataModel, nameof(request.DataModel));

    var existing = await repository.FirstOrDefaultAsync(new DataModelByCodeSpec(request.DataModel.Code), cancellationToken);
    if (existing is not null)
      return Result<Guid>.Error($"DataModel s kódem '{request.DataModel.Code}' již existuje.");

    DataModel dataModel;
    try
    {
      dataModel = new DataModel(request.DataModel.Id == Guid.Empty ? Guid.NewGuid() : request.DataModel.Id,
                                request.DataModel.Code);
      dataModel
        .SetName(request.DataModel.Name)
        .SetDescription(request.DataModel.Description)
        .SetNotes(request.DataModel.Notes);

      if (request.DataModel.IsAggregateRoot)
        dataModel.MarkAsAggregateRoot();
      else
        dataModel.MarkAsNestedEntity();
    }
    catch (ArgumentException ex)
    {
      return Result<Guid>.Invalid(new ValidationError(ex.ParamName ?? "DataModel", ex.Message));
    }

    var created = await repository.AddAsync(dataModel, cancellationToken);
    if (created is null) return Result<Guid>.Error("Nepodařilo se vytvořit datový model.");

    queryService.InvalidateCache(request.Datasource);
    return Result<Guid>.Success(created.Id);
  }
}
