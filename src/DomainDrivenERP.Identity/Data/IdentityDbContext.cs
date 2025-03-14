using DomainDrivenERP.Domain.Abstractions.Identity;
using DomainDrivenERP.Identity.Configurations;
using DomainDrivenERP.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenERP.Identity.Data;
public class IdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApplicationUser>().ToTable("Users", "Security");//to ignore property.Ignore(e=>e.PhoneNumber)
        modelBuilder.Entity<IdentityRole>().ToTable("Role", "Security");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UsersRole", "Security");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RolesClaim", "Security");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UsersLogin", "Security");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UsersClaim", "Security");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UsersToken", "Security");

        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
    }
}
