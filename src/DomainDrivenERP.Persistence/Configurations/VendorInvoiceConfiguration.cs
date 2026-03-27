using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;

internal sealed class VendorInvoiceConfiguration : IEntityTypeConfiguration<VendorInvoice>
{
    public void Configure(EntityTypeBuilder<VendorInvoice> builder)
    {
        builder.ToTable(TableNames.VendorInvoices);
        builder.HasKey(x => x.Id);

        // Soft delete filter
        builder.HasQueryFilter(x => x.Status != Domain.Enums.InvoiceStatus.Cancelled);

        // Indexes
        builder.HasIndex(x => x.VendorId);
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.InvoiceDate);
        builder.HasIndex(x => new { x.SequenceNumber, x.CompanyId }).IsUnique()
            .HasFilter("[SequenceNumber] IS NOT NULL");
        builder.HasIndex(x => new { x.VendorInvoiceNumber, x.VendorId })
            .HasFilter("[VendorInvoiceNumber] IS NOT NULL");

        // Strings
        builder.Property(x => x.SequenceNumber).HasMaxLength(50);
        builder.Property(x => x.VendorInvoiceNumber).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        // Enums → string
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.TaxOrderSetting).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.StackingMode).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.InvoiceHiddenDiscountType).HasConversion<string>().HasMaxLength(30);

        // Decimals
        builder.Property(x => x.SubTotal).HasColumnType("decimal(18,4)");
        builder.Property(x => x.TotalLineDiscount).HasColumnType("decimal(18,4)");
        builder.Property(x => x.TotalTax).HasColumnType("decimal(18,4)");
        builder.Property(x => x.TotalInvoiceDiscount).HasColumnType("decimal(18,4)");
        builder.Property(x => x.TotalHiddenDiscount).HasColumnType("decimal(18,4)");
        builder.Property(x => x.GrandTotal).HasColumnType("decimal(18,4)");
        builder.Property(x => x.InvoiceHiddenDiscountAmount).HasColumnType("decimal(18,4)");

        // Snapshot JSON
        builder.Property(x => x.PipelineSnapshot).HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.InvoiceDiscounts)
            .WithOne()
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
