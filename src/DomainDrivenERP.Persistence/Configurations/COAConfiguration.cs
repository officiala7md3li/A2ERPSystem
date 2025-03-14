using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenERP.Domain.Entities.COAs;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;
internal sealed class COAConfiguration : IEntityTypeConfiguration<Accounts>
{
    public void Configure(EntityTypeBuilder<Accounts> builder)
    {
        builder.ToTable(TableNames.Accounts);

        builder.Ignore(c => c.Id);

        builder.HasKey(c => c.HeadCode);

        builder.Property(c => c.HeadCode)
            .IsRequired();

        builder.Property(c => c.HeadName)
            .IsRequired();

        builder.HasMany(c => c.ChildAccounts)
            .WithOne(c => c.ParentAccount)
            .HasForeignKey(c => c.ParentHeadCode)
            .IsRequired(false);

        builder.HasMany(c => c.Transactions)
            .WithOne(t => t.COA)
            .HasForeignKey(t => t.COAId)
            .IsRequired(false);
    }
}
