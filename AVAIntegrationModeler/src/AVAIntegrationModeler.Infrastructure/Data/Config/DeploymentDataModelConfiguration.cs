using AVAIntegrationModeler.Domain.DeploymentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AVAIntegrationModeler.Infrastructure.Data.Config;

/// <summary>
/// EF Core konfigurace pro entitu DeploymentDataModel.
/// </summary>
public class DeploymentDataModelConfiguration : IEntityTypeConfiguration<DeploymentDataModel>
{
  public void Configure(EntityTypeBuilder<DeploymentDataModel> builder)
  {
    builder.ToTable("DeploymentDataModels");
    builder.HasKey(e => e.Id);

    builder.Property(e => e.DeploymentId).IsRequired();
    builder.Property(e => e.DataModelId).IsRequired();
  }
}
