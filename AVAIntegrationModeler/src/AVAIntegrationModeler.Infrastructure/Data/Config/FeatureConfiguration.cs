using AVAIntegrationModeler.Domain.FeatureAggregate;
using AVAIntegrationModeler.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AVAIntegrationModeler.Infrastructure.Data.Config;

/// <summary>
/// EF Core konfigurace pro agregát Feature.
/// </summary>
public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
  public void Configure(EntityTypeBuilder<Feature> builder)
  {
    builder.ToTable("Features");
    
    builder.HasKey(f => f.Id);
    
    builder.Property(f => f.Code)
      .IsRequired()
      .HasMaxLength(100);

    // Konfigurace lokalizovaných value objects
    builder.OwnsOne(f => f.Name, name =>
    {
      name.Property(n => n.CzechValue).HasColumnName("Name_CZ");
      name.Property(n => n.EnglishValue).HasColumnName("Name_EN");
    });

    builder.OwnsOne(f => f.Description, desc =>
    {
      desc.Property(d => d.CzechValue).HasColumnName("Description_CZ");
      desc.Property(d => d.EnglishValue).HasColumnName("Description_EN");
    });

    // ⚠️ KLÍČOVÁ KONFIGURACE - vztah k IncludedFeatures
    builder.OwnsMany(f => f.IncludedFeatures, includedFeature =>
    {
      includedFeature.ToTable("FeatureIncludedFeatures");
      includedFeature.WithOwner().HasForeignKey("OwnerFeatureId");
      includedFeature.Property<int>("Id").ValueGeneratedOnAdd();
      includedFeature.HasKey("Id");
      
      includedFeature.Property(i => i.FeatureId)
        .IsRequired()
        .HasColumnName("IncludedFeatureId");
      
      includedFeature.Property(i => i.ConsumeOnly)
        .IsRequired()
        .HasColumnName("ConsumeOnly");
    });

    // Konfigurace pro IncludedModels (podobně)
    builder.OwnsMany(f => f.IncludedModels, includedModel =>
    {
      includedModel.ToTable("FeatureIncludedModels");
      includedModel.WithOwner().HasForeignKey("OwnerFeatureId");
      includedModel.Property<int>("Id").ValueGeneratedOnAdd();
      includedModel.HasKey("Id");
      
      includedModel.Property(i => i.ModelId)
        .IsRequired()
        .HasColumnName("IncludedModelId");
      
      includedModel.Property(i => i.ConsumeOnly)
        .IsRequired()
        .HasColumnName("ConsumeOnly");
    });
  }
}
