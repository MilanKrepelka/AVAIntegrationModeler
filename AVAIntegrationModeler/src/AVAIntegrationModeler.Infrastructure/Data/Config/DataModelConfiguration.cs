using AVAIntegrationModeler.Domain.AreaAggregate;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// EF Core konfigurace pro agregát DataModel.
/// </summary>
public class DataModelConfiguration : IEntityTypeConfiguration<DataModel>
{
  public void Configure(EntityTypeBuilder<DataModel> builder)
  {
    builder.ToTable("DataModels");
    builder.HasKey(e => e.Id);

    builder.Property(e => e.Code)
      .IsRequired()
      .HasMaxLength(100);

    builder.Property(e => e.Name)
      .IsRequired()
      .HasMaxLength(200);

    builder.Property(e => e.Description)
      .HasMaxLength(1000);

    builder.Property(e => e.Notes)
      .HasMaxLength(2000);

    builder.Property(e => e.IsAggregateRoot)
      .IsRequired()
      .HasDefaultValue(false);

    builder.Property(e => e.AreaId)
      .IsRequired(false);

    // Vztah k Area (optional foreign key)
    builder.HasOne<Area>()
      .WithMany()
      .HasForeignKey(e => e.AreaId)
      .OnDelete(DeleteBehavior.SetNull)
      .IsRequired(false);

    // ✅ KLÍČ: Ignorovat readonly property Fields
    builder.Ignore(e => e.Fields);

    // ✅ Mapování private kolekce _fields pomocí reflection
    var navigation = builder.Metadata.FindNavigation(nameof(DataModel.Fields));
    if (navigation != null)
    {
      navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
      navigation.SetField("_fields");
    }

    // Explicitní konfigurace vztahu k DataModelField
    builder.HasMany(typeof(DataModelField), "_fields")
      .WithOne()
      .HasForeignKey("DataModelId")
      .OnDelete(DeleteBehavior.Cascade);
  }
}

