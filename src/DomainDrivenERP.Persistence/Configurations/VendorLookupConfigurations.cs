using DomainDrivenERP.Domain.Entities.Vendors;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;

internal sealed class VendorTypeConfiguration : IEntityTypeConfiguration<VendorType>
{
    public void Configure(EntityTypeBuilder<VendorType> builder)
    {
        builder.ToTable(TableNames.VendorTypes);
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.Cancelled);
        builder.Property(x => x.Code).HasMaxLength(30).IsRequired();
        builder.Property(x => x.NameEn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

internal sealed class VendorCategoryConfiguration : IEntityTypeConfiguration<VendorCategory>
{
    public void Configure(EntityTypeBuilder<VendorCategory> builder)
    {
        builder.ToTable(TableNames.VendorCategories);
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.Cancelled);
        builder.Property(x => x.Code).HasMaxLength(30).IsRequired();
        builder.Property(x => x.NameEn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

internal sealed class VendorGroupConfiguration : IEntityTypeConfiguration<VendorGroup>
{
    public void Configure(EntityTypeBuilder<VendorGroup> builder)
    {
        builder.ToTable(TableNames.VendorGroups);
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.Cancelled);
        builder.Property(x => x.Code).HasMaxLength(30).IsRequired();
        builder.Property(x => x.NameEn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
