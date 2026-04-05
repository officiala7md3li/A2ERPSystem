using DomainDrivenERP.Domain.Entities.UnitOfMeasures;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;

internal sealed class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.ToTable(TableNames.UnitOfMeasures);
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.Cancelled);
        builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
        builder.Property(x => x.NameEn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.ConversionFactor).HasColumnType("decimal(18,6)");
        builder.HasIndex(x => x.Code).IsUnique();

        // Self-referencing FK: derived UoM → base UoM (e.g. BOX → PCS)
        builder.HasOne<UnitOfMeasure>()
            .WithMany()
            .HasForeignKey(x => x.BaseUomId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
