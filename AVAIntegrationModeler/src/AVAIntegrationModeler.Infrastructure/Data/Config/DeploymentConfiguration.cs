using AVAIntegrationModeler.Domain.DeploymentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AVAIntegrationModeler.Infrastructure.Data.Config;

/// <summary>
/// EF Core konfigurace pro agregát Deployment.
/// </summary>
public class DeploymentConfiguration : IEntityTypeConfiguration<Deployment>
{
  public void Configure(EntityTypeBuilder<Deployment> builder)
  {
    builder.ToTable("Deployments");
    builder.HasKey(e => e.Id);

    builder.Property(e => e.Code)
      .IsRequired()
      .HasMaxLength(100);

    builder.Property(e => e.Name)
      .IsRequired()
      .HasMaxLength(200);

    builder.Property(e => e.Ticket)
      .IsRequired(false)
      .HasMaxLength(500);

    builder.Property(e => e.Description)
      .IsRequired(false)
      .HasMaxLength(2000);

    builder.Property(e => e.LastSaveDateTime)
      .IsRequired(false);

    builder.Property(e => e.LastDeploymentDateTime)
      .IsRequired(false);

    builder.HasIndex(e => e.Code).IsUnique();

    builder.HasMany(e => e.DataModels)
      .WithOne()
      .HasForeignKey(m => m.DeploymentId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
