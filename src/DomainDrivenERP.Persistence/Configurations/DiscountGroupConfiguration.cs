using DomainDrivenERP.Domain.Entities.DiscountGroups;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DomainDrivenERP.Persistence.Configurations;
internal sealed class DiscountGroupConfiguration : IEntityTypeConfiguration<DiscountGroup>
{
    public void Configure(EntityTypeBuilder<DiscountGroup> builder)
    {
        builder.ToTable(TableNames.DiscountGroups);
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.Cancelled);
        builder.Property(x => x.NameEn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.CompanyId);
        builder.HasMany(x => x.Rules)
            .WithOne()
            .HasForeignKey(x => x.DiscountGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
internal sealed class DiscountRuleConfiguration : IEntityTypeConfiguration<DiscountRule>
{
    public void Configure(EntityTypeBuilder<DiscountRule> builder)
    {
        builder.ToTable(TableNames.DiscountRules);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Value).HasColumnType("decimal(18,4)");
        builder.Property(x => x.TiersJson).HasColumnType("nvarchar(max)");
    }
}
