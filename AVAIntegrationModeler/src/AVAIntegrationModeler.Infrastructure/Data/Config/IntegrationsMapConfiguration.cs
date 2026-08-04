using AVAIntegrationModeler.Domain.IntegrationMapAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AVAIntegrationModeler.Infrastructure.Data.Config;

/// <summary>
/// EF Core konfigurace pro IntegrationsMap agregát.
/// </summary>
public class IntegrationsMapConfiguration : IEntityTypeConfiguration<IntegrationsMap>
{
  public void Configure(EntityTypeBuilder<IntegrationsMap> builder)
  {
    builder.ToTable("IntegrationMaps");

    builder.HasKey(m => m.Id);

    builder.Property(m => m.AreaId)
      .IsRequired();

    builder.HasIndex(m => m.AreaId);

    // Konfigurace owned entity pro Items
    builder.OwnsMany(m => m.Items, item =>
    {
      item.ToTable("IntegrationMapItems");
      
      item.HasKey(i => i.Id);
      
      item.Property(i => i.Id)
        .ValueGeneratedNever(); // Guid generován v konstruktoru
      
      item.WithOwner().HasForeignKey("IntegrationsMapId");
      item.Property<Guid>("IntegrationsMapId").IsRequired();

      item.Property(i => i.ScenarioId)
        .IsRequired();

      item.HasIndex(i => i.ScenarioId);
      item.HasIndex("IntegrationsMapId");

      // Konfigurace owned entity pro Keys
      item.OwnsMany(i => i.Keys, key =>
      {
        key.ToTable("IntegrationMapActivationKeys");
        
        key.HasKey(k => k.Id);
        
        key.Property(k => k.Id)
          .ValueGeneratedNever(); // Guid generován v konstruktoru
        
        key.WithOwner().HasForeignKey("IntegrationMapItemId");
        key.Property<Guid>("IntegrationMapItemId").IsRequired();

        key.Property(k => k.Key)
          .IsRequired()
          .HasMaxLength(200);

        key.HasIndex(k => k.Key);
        key.HasIndex("IntegrationMapItemId");
      });
    });
  }
}
