using Ardalis.Result;
using Ardalis.Specification;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.DeploymentAggregate;
using AVAIntegrationModeler.UseCases.Deployments;
using AVAIntegrationModeler.UseCases.Deployments.CompleteDevelopment;
using NSubstitute;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.Deployments.CompleteDevelopment;

/// <summary>
/// Unit testy pro CompleteDeploymentDevelopmentHandler — ukončení vývoje nasazení.
/// </summary>
public class CompleteDeploymentDevelopmentHandlerTests
{
  private readonly IDeploymentRepository _repository = Substitute.For<IDeploymentRepository>();
  private readonly IDeploymentsQueryService _queryService = Substitute.For<IDeploymentsQueryService>();

  private CompleteDeploymentDevelopmentHandler CreateHandler() => new(_repository, _queryService);

  [Fact]
  public async Task Handle_ShouldReturnNotFound_WhenDeploymentDoesNotExist()
  {
    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Deployment>>(), Arg.Any<CancellationToken>())
      .Returns((Deployment?)null);

    var result = await CreateHandler().Handle(new CompleteDeploymentDevelopmentCommand("DEP-404"), TestContext.Current.CancellationToken);

    result.Status.ShouldBe(ResultStatus.NotFound);
    await _repository.DidNotReceive().SyncDataModelsAndSaveAsync(
      Arg.Any<Deployment>(), Arg.Any<IList<DeploymentDataModel>>(), Arg.Any<IList<DeploymentDataModel>>(), Arg.Any<CancellationToken>());
  }

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public async Task Handle_ShouldReturnInvalid_WhenCodeIsEmpty(string code)
  {
    var result = await CreateHandler().Handle(new CompleteDeploymentDevelopmentCommand(code), TestContext.Current.CancellationToken);

    result.Status.ShouldBe(ResultStatus.Invalid);
    await _repository.DidNotReceive().FirstOrDefaultAsync(Arg.Any<ISpecification<Deployment>>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ShouldSetLastDeploymentToNow_SaveAndInvalidateCache()
  {
    var modelId = Guid.NewGuid();
    var deployment = new Deployment(Guid.NewGuid(), "DEP-001").SetName("Nasazení");
    deployment.AddDataModel(modelId);
    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Deployment>>(), Arg.Any<CancellationToken>())
      .Returns(deployment);

    var before = DateTime.UtcNow;
    var result = await CreateHandler().Handle(new CompleteDeploymentDevelopmentCommand("DEP-001"), TestContext.Current.CancellationToken);
    var after = DateTime.UtcNow;

    result.IsSuccess.ShouldBeTrue();
    result.Value.LastDeploymentDateTime.ShouldNotBeNull();
    result.Value.LastDeploymentDateTime!.Value.ShouldBeInRange(before, after);
    result.Value.LastSaveDateTime.ShouldBe(result.Value.LastDeploymentDateTime);
    result.Value.Code.ShouldBe("DEP-001");
    result.Value.DataModelIds.ShouldBe(new[] { modelId });
    deployment.LastDeploymentDateTime.ShouldBe(result.Value.LastDeploymentDateTime);

    await _repository.Received(1).SyncDataModelsAndSaveAsync(
      deployment,
      Arg.Is<IList<DeploymentDataModel>>(l => l.Count == 0),
      Arg.Is<IList<DeploymentDataModel>>(l => l.Count == 0),
      Arg.Any<CancellationToken>());
    _queryService.Received(1).InvalidateCache(Datasource.Database);
  }
}
