using DomainDrivenERP.Domain.Entities.Products;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DomainDrivenERP.Domain.ValueObjects;

namespace DomainDrivenERP.Persistence.Configurations;
internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(TableNames.Products);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Model)
            .IsRequired();

        builder.Property(x => x.SKU)
            .HasConversion(x => x.Value, v => SKU.Create(v).Value);

        builder.OwnsOne(
                  x => x.Price,
                  price =>
                  {
                      price.Property(p => p.Amount)
                          .HasColumnName("PriceAmount")
                          .HasColumnType("decimal(18,2)");
                      price.Property(p => p.Currency).HasColumnName("PriceCurrency");
                  }
              );

        // ── Phase 3 Enrichment Fields ──────────────────────────
        builder.Property(x => x.UnitOfMeasureId).IsRequired();
        builder.HasIndex(x => x.UnitOfMeasureId);

        builder.HasIndex(x => x.TaxGroupId);
        builder.Property(x => x.TaxGroupSource)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(DomainDrivenERP.Domain.Enums.TaxGroupSource.Category);

        builder.HasIndex(x => x.DiscountGroupId);

        builder.Property(x => x.MinimumSalePrice).HasColumnType("decimal(18,4)");
        builder.Property(x => x.MaximumDiscountPercent).HasColumnType("decimal(5,2)");
    }
}
