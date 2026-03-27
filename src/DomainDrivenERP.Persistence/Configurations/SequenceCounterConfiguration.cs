using DomainDrivenERP.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;

public sealed class SequenceCounterConfiguration : IEntityTypeConfiguration<SequenceCounter>
{
    public void Configure(EntityTypeBuilder<SequenceCounter> builder)
    {
        builder.ToTable("SequenceCounters");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Prefix).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CounterValue).IsRequired();
        builder.HasIndex(x => new { x.Prefix, x.CompanyId, x.SequenceDate }).IsUnique();
    }
}
