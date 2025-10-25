using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AVAIntegrationModeler.Domain.DataModelAggregate;

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
    
    // Mapování private kolekce _fields
    builder.HasMany(typeof(DataModelField), "_fields")
      .WithOne()
      .HasForeignKey("DataModelId")
      .OnDelete(DeleteBehavior.Cascade);
  }
}
