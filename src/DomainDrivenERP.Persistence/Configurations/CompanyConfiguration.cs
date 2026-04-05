using DomainDrivenERP.Domain.Entities.Companies;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DomainDrivenERP.Persistence.Configurations;
internal sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable(TableNames.Companies);
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.Cancelled);
        builder.Property(x => x.NameEn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(x => x.TaxRegistrationNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.CommercialRegistrationNumber).HasMaxLength(50);
        builder.Property(x => x.DefaultTaxOrder).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.DefaultStackingMode).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.StockValuation).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.MaxDiscountPercentPerLine).HasColumnType("decimal(5,2)");
        builder.Property(x => x.MaxDiscountAmountPerInvoice).HasColumnType("decimal(18,4)");
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.LogoUrl).HasMaxLength(500);
        builder.HasIndex(x => x.TaxRegistrationNumber).IsUnique();
    }
}
