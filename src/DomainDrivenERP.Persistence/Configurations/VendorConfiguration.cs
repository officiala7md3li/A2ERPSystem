using DomainDrivenERP.Domain.Entities.Vendors;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DomainDrivenERP.Persistence.Configurations;
internal sealed class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.ToTable(TableNames.Vendors);
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.Cancelled);
        builder.Property(x => x.NameEn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(x => x.TaxRegistrationNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.CommercialRegistrationNumber).HasMaxLength(50);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.ContactPersonName).HasMaxLength(200);
        builder.Property(x => x.CreditLimit).HasColumnType("decimal(18,4)");

        // Classification FK — all nullable
        builder.HasIndex(x => x.VendorTypeId);
        builder.HasIndex(x => x.VendorCategoryId);
        builder.HasIndex(x => x.VendorGroupId);

        builder.HasIndex(x => new { x.TaxRegistrationNumber, x.CompanyId }).IsUnique();
        builder.HasIndex(x => x.CompanyId);
    }
}
