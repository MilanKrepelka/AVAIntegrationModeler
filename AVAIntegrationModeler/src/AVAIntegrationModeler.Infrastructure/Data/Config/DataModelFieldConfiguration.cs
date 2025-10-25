// src\AVAIntegrationModeler.Infrastructure\Data\Config\DataModelFieldConfiguration.cs
using AVAIntegrationModeler.Domain.DataModelAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AVAIntegrationModeler.Infrastructure.Data.Config;

public class DataModelFieldConfiguration : IEntityTypeConfiguration<DataModelField>
{
  public void Configure(EntityTypeBuilder<DataModelField> builder)
  {
    builder.ToTable("DataModelFields");
    builder.HasKey(e => e.Id);
    
    builder.Property(e => e.Name)
      .IsRequired()
      .HasMaxLength(100);
    
    builder.Property(e => e.Label)
      .HasMaxLength(200);
    
    builder.Property(e => e.Description)
      .HasMaxLength(1000);
    
    builder.Property(e => e.IsPublishedForLookup).IsRequired();
    builder.Property(e => e.IsCollection).IsRequired();
    builder.Property(e => e.IsLocalized).IsRequired();
    builder.Property(e => e.IsNullable).IsRequired();
    
    builder.Property(e => e.FieldType)
      .IsRequired()
      .HasConversion<int>();
    
    // ✅ Mapování private kolekce _entityTypeReferences
    var navigation = builder.Metadata.FindNavigation(nameof(DataModelField.EntityTypeReferences));
    if (navigation != null)
    {
      navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
      navigation.SetField("_entityTypeReferences");
    }
    
    // Explicitní konfigurace vztahu
    builder.HasMany(typeof(DataModelFieldEntityTypeReference), "_entityTypeReferences")
      .WithOne()
      .HasForeignKey("DataModelFieldId")
      .OnDelete(DeleteBehavior.Cascade);
    
    // Ignorovat computed property (není v DB)
    builder.Ignore(e => e.ReferencedEntityTypeIds);
  }
}

