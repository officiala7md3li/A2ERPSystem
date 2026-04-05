using DomainDrivenERP.Domain.Entities.PromoCodes;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DomainDrivenERP.Persistence.Configurations;
internal sealed class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.ToTable(TableNames.PromoCodes);
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.Cancelled);
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.DiscountType).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.DiscountValue).HasColumnType("decimal(18,4)");
        builder.Property(x => x.MinimumOrderAmount).HasColumnType("decimal(18,4)");
        builder.HasIndex(x => new { x.Code, x.CompanyId }).IsUnique();
        builder.HasIndex(x => x.CompanyId);
        builder.HasMany(x => x.Usages)
            .WithOne()
            .HasForeignKey(x => x.PromoCodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
internal sealed class PromoCodeUsageConfiguration : IEntityTypeConfiguration<PromoCodeUsage>
{
    public void Configure(EntityTypeBuilder<PromoCodeUsage> builder)
    {
        builder.ToTable(TableNames.PromoCodeUsages);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DiscountApplied).HasColumnType("decimal(18,4)");
        builder.HasIndex(x => new { x.PromoCodeId, x.CustomerId });
        builder.HasIndex(x => x.InvoiceId);
    }
}
