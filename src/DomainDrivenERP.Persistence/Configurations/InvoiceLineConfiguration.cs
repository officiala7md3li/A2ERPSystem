using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.ValueObjects;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;

internal sealed class InvoiceLineConfiguration : IEntityTypeConfiguration<InvoiceLine>
{
    public void Configure(EntityTypeBuilder<InvoiceLine> builder)
    {
        builder.ToTable(TableNames.InvoiceLines);
        builder.HasKey(x => x.Id);

        // Value Objects
        builder.Property(x => x.Quantity)
            .HasConversion(
                q => $"{q.Value}|{q.Unit}",
                s => Quantity.Create(
                    decimal.Parse(s.Split(new[] { '|' })[0]),
                    s.Split(new[] { '|' })[1]).Value)
            .HasColumnName("Quantity")
            .HasMaxLength(50);

        builder.Property(x => x.UnitPrice)
            .HasConversion(
                m => $"{m.Amount}|{m.Currency}",
                s => Money.Create(
                    decimal.Parse(s.Split(new[] { '|' })[0]),
                    s.Split(new[] { '|' })[1]).Value)
            .HasColumnName("UnitPrice")
            .HasMaxLength(50);

        // Decimals
        builder.Property(x => x.TotalDiscountAmount).HasColumnType("decimal(18,4)");
        builder.Property(x => x.TotalTaxAmount).HasColumnType("decimal(18,4)");
        builder.Property(x => x.HiddenDiscountAmount).HasColumnType("decimal(18,4)");

        // Snapshots — JSON columns
        builder.Property(x => x.TaxGroupSnapshot).HasColumnType("nvarchar(max)");
        builder.Property(x => x.DiscountGroupSnapshot).HasColumnType("nvarchar(max)");

        // Enums
        builder.Property(x => x.HiddenDiscountType).HasConversion<string>().HasMaxLength(30);

        // Relationships
        builder.HasMany(x => x.TaxBreakdowns)
            .WithOne()
            .HasForeignKey(x => x.InvoiceLineId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.DiscountBreakdowns)
            .WithOne()
            .HasForeignKey(x => x.InvoiceLineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
