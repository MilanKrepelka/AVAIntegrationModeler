using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.ContributorAggregate;

namespace AVAIntegrationModeler.UseCases.Contributors.Update;

public class UpdateContributorHandler(
  IRepository<Contributor> _repository,
  IDomainEntityValidationService<Contributor> _contributorValidationService)
  : ICommandHandler<UpdateContributorCommand, Result<ContributorDTO>>
{
  public async Task<Result<ContributorDTO>> Handle(UpdateContributorCommand request, CancellationToken cancellationToken)
  {
    var existingContributor = await _repository.GetByIdAsync(request.ContributorId, cancellationToken);
    if (existingContributor == null)
    {
      return Result.NotFound();
    }

    existingContributor.UpdateName(request.NewName!);

    var validationResult = await _contributorValidationService.Validate(Datasource.Database, existingContributor, cancellationToken);
    if (!validationResult.IsSuccess)
      return validationResult;

    await _repository.UpdateAsync(existingContributor, cancellationToken);

    return new ContributorDTO(existingContributor.Id,
      existingContributor.Name, existingContributor.PhoneNumber?.Number ?? "");
  }
}
