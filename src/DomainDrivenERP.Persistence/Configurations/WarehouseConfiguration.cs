using DomainDrivenERP.Domain.Entities.Warehouses;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DomainDrivenERP.Persistence.Configurations;
internal sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable(TableNames.Warehouses);
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.Cancelled);
        builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
        builder.Property(x => x.NameEn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.ManagerName).HasMaxLength(200);
        builder.HasIndex(x => new { x.Code, x.CompanyId }).IsUnique();
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.ParentWarehouseId);
        // Self-referencing hierarchy
        builder.HasMany(x => x.SubWarehouses)
            .WithOne()
            .HasForeignKey(x => x.ParentWarehouseId)
            .OnDelete(DeleteBehavior.Restrict); // prevent cascade delete of sub-warehouses
    }
}
