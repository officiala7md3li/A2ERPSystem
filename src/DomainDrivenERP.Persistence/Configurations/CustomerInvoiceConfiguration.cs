using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;

internal sealed class CustomerInvoiceConfiguration : IEntityTypeConfiguration<CustomerInvoice>
{
    public void Configure(EntityTypeBuilder<CustomerInvoice> builder)
    {
        builder.ToTable(TableNames.CustomerInvoices);
        builder.HasKey(x => x.Id);

        // Soft delete filter
        builder.HasQueryFilter(x => !x.Cancelled);

        // Indexes
        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.InvoiceDate);
        builder.HasIndex(x => new { x.SequenceNumber, x.CompanyId }).IsUnique()
            .HasFilter("[SequenceNumber] IS NOT NULL");

        // Strings
        builder.Property(x => x.SequenceNumber).HasMaxLength(50);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        // Enums → string (readable in DB)
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
