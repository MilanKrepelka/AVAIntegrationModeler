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
    builder.HasKey(e => e.Id);
    
    builder.Property(e => e.Code)
      .IsRequired()
      .HasMaxLength(100);
    
    // Mapování LocalizedValue pro Name (owned type)
    builder.OwnsOne(e => e.Name, name =>
    {
      name.Property(n => n.CzechValue)
        .HasColumnName("Name_CzechValue")
        .IsRequired();
      
      name.Property(n => n.EnglishValue)
        .HasColumnName("Name_EnglishValue")
        .IsRequired();
    });
    
    // Mapování LocalizedValue pro Description (owned type)
    builder.OwnsOne(e => e.Description, desc =>
    {
      desc.Property(d => d.CzechValue)
        .HasColumnName("Description_CzechValue")
        .IsRequired();
      
      desc.Property(d => d.EnglishValue)
        .HasColumnName("Description_EnglishValue")
        .IsRequired();
    });
    
    // ✅ Ignorovat readonly properties
    builder.Ignore(e => e.IncludedFeatures);
    builder.Ignore(e => e.IncludedModels);
    
    // ✅ Mapování private kolekce _includedFeatures
    var includedFeaturesNav = builder.Metadata.FindNavigation(nameof(Feature.IncludedFeatures));
    if (includedFeaturesNav != null)
    {
      includedFeaturesNav.SetPropertyAccessMode(PropertyAccessMode.Field);
      includedFeaturesNav.SetField("_includedFeatures");
    }
    
    // ✅ Mapování private kolekce _includedModels
    var includedModelsNav = builder.Metadata.FindNavigation(nameof(Feature.IncludedModels));
    if (includedModelsNav != null)
    {
      includedModelsNav.SetPropertyAccessMode(PropertyAccessMode.Field);
      includedModelsNav.SetField("_includedModels");
    }
    
    // Mapování kolekce IncludedFeatures (owned type collection)
    builder.OwnsMany(typeof(IncludedFeature), "_includedFeatures", includedFeatures =>
    {
      includedFeatures.ToTable("IncludedFeatures");

      includedFeatures.WithOwner()
        .HasForeignKey("FeatureId");

      includedFeatures.Property<Guid>("Id")
        .ValueGeneratedOnAdd();

      includedFeatures.HasKey("Id");

      includedFeatures.Property<Guid>("FeatureId")
        .HasColumnName("ReferencedFeatureId")
        .IsRequired();

      includedFeatures.Property<bool>("ConsumeOnly")
        .IsRequired();
    });

    // Mapování kolekce IncludedModels (owned type collection)
    builder.OwnsMany(typeof(IncludedModel), "_includedModels", includedModels =>
    {
      includedModels.ToTable("IncludedModels");

      includedModels.WithOwner()
        .HasForeignKey("FeatureId");

      includedModels.Property<Guid>("Id")
        .ValueGeneratedOnAdd();

      includedModels.HasKey("Id");

      includedModels.Property<Guid>("ModelId")
        .IsRequired();

      includedModels.Property<bool>("ConsumeOnly")
        .IsRequired();
    });
  }
}
