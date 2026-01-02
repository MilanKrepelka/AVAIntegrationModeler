using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Features.Get;

public record GetFeatureQuery(Datasource Datasource, Guid FeatureId) : IQuery<Result<FeatureDTO>>;
