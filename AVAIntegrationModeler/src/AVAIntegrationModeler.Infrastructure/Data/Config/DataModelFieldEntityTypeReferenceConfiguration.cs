using AVAIntegrationModeler.Domain.DataModelAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AVAIntegrationModeler.Infrastructure.Data.Config;

/// <summary>
/// EF Core konfigurace pro DataModelFieldEntityTypeReference.
/// </summary>
public class DataModelFieldEntityTypeReferenceConfiguration 
  : IEntityTypeConfiguration<DataModelFieldEntityTypeReference>
{
  public void Configure(EntityTypeBuilder<DataModelFieldEntityTypeReference> builder)
  {
    builder.ToTable("DataModelFieldEntityTypeReferences");
    builder.HasKey(e => e.Id);
    
    builder.Property(e => e.DataModelFieldId)
      .IsRequired();
    
    builder.Property(e => e.ReferencedEntityTypeId)
      .IsRequired();
    
    // Index pro rychlé vyhledávání
    builder.HasIndex(e => new { e.DataModelFieldId, e.ReferencedEntityTypeId })
      .IsUnique(); // Zamezí duplicitám
    
    // Foreign key na DataModelField (cascade delete)
    builder.HasOne<DataModelField>()
      .WithMany()
      .HasForeignKey(e => e.DataModelFieldId)
      .OnDelete(DeleteBehavior.Cascade);
    
    // Poznámka: Foreign key na ReferencedEntityTypeId nelze vytvořit,
    // protože odkazuje na DataModel.Id (jiná tabulka) - musí se řešit aplikační logikou
  }
}
