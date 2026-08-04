using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Features.Get;

public record GetFeatureByCodeQuery(Datasource Datasource, string FeatureCode) : IQuery<Result<FeatureDTO>>;
