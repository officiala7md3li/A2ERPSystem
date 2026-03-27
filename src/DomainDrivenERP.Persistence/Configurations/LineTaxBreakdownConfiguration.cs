using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;

internal sealed class LineTaxBreakdownConfiguration : IEntityTypeConfiguration<LineTaxBreakdown>
{
    public void Configure(EntityTypeBuilder<LineTaxBreakdown> builder)
    {
        builder.ToTable(TableNames.LineTaxBreakdowns);
        builder.HasKey(x => x.Id);

        // Indexes
        builder.HasIndex(x => x.InvoiceLineId);

        // Strings
        builder.Property(x => x.TaxCode).HasMaxLength(20).IsRequired();
        builder.Property(x => x.TaxName).HasMaxLength(200).IsRequired();

        // Decimals
        builder.Property(x => x.Rate).HasColumnType("decimal(18,6)");
        builder.Property(x => x.TaxAmount).HasColumnType("decimal(18,4)");
    }
}
