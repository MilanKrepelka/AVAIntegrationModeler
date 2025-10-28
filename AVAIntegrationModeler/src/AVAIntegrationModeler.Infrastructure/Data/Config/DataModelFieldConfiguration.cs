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
builder.HasKey(e => e.Id);    
    builder.Property(e => e.IsPublishedForLookup).IsRequired();
    builder.Property(e => e.IsCollection).IsRequired();
    builder.Property(e => e.IsLocalized).IsRequired();
    builder.Property(e => e.IsNullable).IsRequired();
    
    builder.Property(e => e.FieldType)
      .IsRequired()
      .HasConversion<int>();
    
    // 🔥 KLÍČOVÁ OPRAVA: Správná konfigurace pro EntityTypeReferences
    builder.HasMany<DataModelFieldEntityTypeReference>(e => e.EntityTypeReferences)
      .WithOne()
      .HasForeignKey("DataModelFieldId")
      .OnDelete(DeleteBehavior.Cascade);

    // 🔥 Nastavení přístupu k private backing field
    builder.Navigation(e => e.EntityTypeReferences)
      .UsePropertyAccessMode(PropertyAccessMode.Field)
      .AutoInclude(); // 🔥 Automaticky načíst i vnořené entity!
    
    // Ignorovat computed property (není v DB)
    builder.Ignore(e => e.ReferencedEntityTypeIds);

    // Shadow property pro foreign key do DataModel
    builder.Property<Guid>("DataModelId").IsRequired();
  }
}

