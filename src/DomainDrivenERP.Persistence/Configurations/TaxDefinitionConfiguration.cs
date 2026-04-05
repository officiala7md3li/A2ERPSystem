using DomainDrivenERP.Domain.Entities.TaxDefinitions;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DomainDrivenERP.Persistence.Configurations;

internal sealed class TaxDefinitionConfiguration : IEntityTypeConfiguration<TaxDefinition>
{
    public void Configure(EntityTypeBuilder<TaxDefinition> builder)
    {
        builder.ToTable(TableNames.TaxDefinitions);
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.Cancelled);
        builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
        builder.Property(x => x.NameEn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Rate).HasColumnType("decimal(18,6)");
        builder.Property(x => x.CalculationMethod).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.AppliesTo).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasMany(x => x.Dependencies)
            .WithOne()
            .HasForeignKey(x => x.TaxDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class TaxDependencyConfiguration : IEntityTypeConfiguration<TaxDependency>
{
    public void Configure(EntityTypeBuilder<TaxDependency> builder)
    {
        builder.ToTable(TableNames.TaxDependencies);
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.TaxDefinitionId, x.DependsOnTaxId }).IsUnique();
    }
}
