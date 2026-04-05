using DomainDrivenERP.Domain.Entities.TaxGroups;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DomainDrivenERP.Persistence.Configurations;
internal sealed class TaxGroupConfiguration : IEntityTypeConfiguration<TaxGroup>
{
    public void Configure(EntityTypeBuilder<TaxGroup> builder)
    {
        builder.ToTable(TableNames.TaxGroups);
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.Cancelled);
        builder.Property(x => x.NameEn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.CompanyId);
        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.TaxGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
internal sealed class TaxGroupItemConfiguration : IEntityTypeConfiguration<TaxGroupItem>
{
    public void Configure(EntityTypeBuilder<TaxGroupItem> builder)
    {
        builder.ToTable(TableNames.TaxGroupItems);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OverrideRate).HasColumnType("decimal(18,6)");
        builder.HasIndex(x => new { x.TaxGroupId, x.TaxDefinitionId }).IsUnique();
    }
}
