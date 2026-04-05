using DomainDrivenERP.Domain.Entities.PriceLists;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DomainDrivenERP.Persistence.Configurations;
internal sealed class PriceListConfiguration : IEntityTypeConfiguration<PriceList>
{
    public void Configure(EntityTypeBuilder<PriceList> builder)
    {
        builder.ToTable(TableNames.PriceLists);
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.Cancelled);
        builder.Property(x => x.NameEn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.CompanyId);
        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.PriceListId)
            .OnDelete(DeleteBehavior.Cascade);
        // AssignedCustomerIds stored as separate join table
        builder.Ignore(x => x.AssignedCustomerIds);
    }
}
internal sealed class PriceListItemConfiguration : IEntityTypeConfiguration<PriceListItem>
{
    public void Configure(EntityTypeBuilder<PriceListItem> builder)
    {
        builder.ToTable(TableNames.PriceListItems);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FixedPrice).HasColumnType("decimal(18,4)");
        builder.Property(x => x.DiscountPercent).HasColumnType("decimal(5,2)");
        builder.HasIndex(x => new { x.PriceListId, x.ItemId }).IsUnique();
    }
}
