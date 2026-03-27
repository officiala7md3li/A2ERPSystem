using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;

internal sealed class InvoiceLevelDiscountConfiguration : IEntityTypeConfiguration<InvoiceLevelDiscount>
{
    public void Configure(EntityTypeBuilder<InvoiceLevelDiscount> builder)
    {
        builder.ToTable(TableNames.InvoiceLevelDiscounts);
        builder.HasKey(x => x.Id);

        // Indexes
        builder.HasIndex(x => x.InvoiceId);

        // Enums → string
        builder.Property(x => x.Source).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(30);

        // Decimals
        builder.Property(x => x.DiscountValue).HasColumnType("decimal(18,4)");
        builder.Property(x => x.DiscountAmount).HasColumnType("decimal(18,4)");
    }
}
