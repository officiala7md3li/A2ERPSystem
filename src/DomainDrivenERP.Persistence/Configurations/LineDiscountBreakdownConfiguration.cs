using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;

internal sealed class LineDiscountBreakdownConfiguration : IEntityTypeConfiguration<LineDiscountBreakdown>
{
    public void Configure(EntityTypeBuilder<LineDiscountBreakdown> builder)
    {
        builder.ToTable(TableNames.LineDiscountBreakdowns);
        builder.HasKey(x => x.Id);

        // Indexes
        builder.HasIndex(x => x.InvoiceLineId);

        // Enums → string
        builder.Property(x => x.Source).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(30);

        // Decimals
        builder.Property(x => x.DiscountValue).HasColumnType("decimal(18,4)");
        builder.Property(x => x.DiscountAmount).HasColumnType("decimal(18,4)");
    }
}
