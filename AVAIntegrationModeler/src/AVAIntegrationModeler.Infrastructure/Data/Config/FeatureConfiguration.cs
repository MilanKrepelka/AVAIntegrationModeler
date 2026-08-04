using AVAIntegrationModeler.Domain.FeatureAggregate;
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

    builder.HasIndex(f => f.Code)
      .IsUnique();

    // Konfigurace lokalizovaných value objects
    builder.OwnsOne(f => f.Name, name =>
    {
      name.Property(n => n.CzechValue).HasColumnName("Name_CZ").HasMaxLength(200);
      name.Property(n => n.EnglishValue).HasColumnName("Name_EN").HasMaxLength(200);
    });

    builder.OwnsOne(f => f.Description, desc =>
    {
      desc.Property(d => d.CzechValue).HasColumnName("Description_CZ").HasMaxLength(1000);
      desc.Property(d => d.EnglishValue).HasColumnName("Description_EN").HasMaxLength(1000);
    });

    // ✅ AKTUALIZOVANÁ KONFIGURACE - IncludedFeatures jako vlastní entita s Guid PK
    builder.OwnsMany(f => f.IncludedFeatures, includedFeature =>
    {
      includedFeature.ToTable("FeatureIncludedFeatures");
      
      // Primární klíč typu Guid
      includedFeature.HasKey(i => i.Id);
      includedFeature.Property(i => i.Id)
        .HasColumnName("Id")
        .ValueGeneratedNever(); // Guid je generován v konstruktoru
      
      // Foreign key k vlastnící Feature
      includedFeature.WithOwner().HasForeignKey("OwnerFeatureId");
      includedFeature.Property<Guid>("OwnerFeatureId").IsRequired();
      
      // Vlastnosti
      includedFeature.Property(i => i.FeatureId)
        .IsRequired()
        .HasColumnName("IncludedFeatureId");
      
      includedFeature.Property(i => i.ConsumeOnly)
        .IsRequired()
        .HasColumnName("ConsumeOnly")
        .HasDefaultValue(false);

      // Indexy
      includedFeature.HasIndex("OwnerFeatureId");
      includedFeature.HasIndex(i => i.FeatureId);
    });

    // Konfigurace pro IncludedModels (stejný pattern)
    builder.OwnsMany(f => f.IncludedModels, includedModel =>
    {
      includedModel.ToTable("FeatureIncludedModels");

      // Pokud IncludedModel také potřebuje Guid PK
      includedModel.HasKey(i => i.Id);
      includedModel.Property(i => i.Id).ValueGeneratedNever();

      includedModel.WithOwner().HasForeignKey("OwnerFeatureId");
      includedModel.Property<Guid>("OwnerFeatureId").IsRequired();
      
      includedModel.Property(i => i.ModelId)
        .IsRequired()
        .HasColumnName("IncludedModelId");
      
      includedModel.Property(i => i.ReadOnly)
        .IsRequired()
        .HasColumnName("ReadOnly")
        .HasDefaultValue(false);

      includedModel.HasIndex("OwnerFeatureId");
      includedModel.HasIndex(i => i.ModelId);
    });
  }
}
