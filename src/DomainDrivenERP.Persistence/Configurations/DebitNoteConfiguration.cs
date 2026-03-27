using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;

internal sealed class DebitNoteConfiguration : IEntityTypeConfiguration<DebitNote>
{
    public void Configure(EntityTypeBuilder<DebitNote> builder)
    {
        builder.ToTable(TableNames.DebitNotes);
        builder.HasKey(x => x.Id);

        // Indexes
        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.OriginalInvoiceId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.SequenceNumber, x.CompanyId }).IsUnique()
            .HasFilter("[SequenceNumber] IS NOT NULL");

        // Strings
        builder.Property(x => x.SequenceNumber).HasMaxLength(50);
        builder.Property(x => x.Reason).HasMaxLength(1000).IsRequired();

        // Enums → string
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);

        // Decimals
        builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,4)");
        builder.Property(x => x.TotalTax).HasColumnType("decimal(18,4)");
        builder.Property(x => x.GrandTotal).HasColumnType("decimal(18,4)");

        // Snapshot JSON
        builder.Property(x => x.PipelineSnapshot).HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
