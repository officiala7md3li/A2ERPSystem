using DomainDrivenERP.Domain.Entities.Currencies;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;

internal sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable(TableNames.Currencies);
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.Cancelled);
        builder.Property(x => x.Code).HasMaxLength(3).IsRequired();
        builder.Property(x => x.NameEn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200);
        builder.Property(x => x.Symbol).HasMaxLength(10).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
