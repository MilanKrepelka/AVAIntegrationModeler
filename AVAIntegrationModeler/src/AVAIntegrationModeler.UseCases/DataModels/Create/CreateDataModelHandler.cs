using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DataModelAggregate;

namespace AVAIntegrationModeler.UseCases.DataModels.Create;

public class CreateDataModelHandler(IRepository<DataModel> repository)
  : ICommandHandler<CreateDataModelCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateDataModelCommand request, CancellationToken cancellationToken)
  {
    Guard.Against.Null(request.DataModel, nameof(request.DataModel));

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
    return Result<Guid>.Success(created.Id);
  }
}
