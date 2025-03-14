using DomainDrivenERP.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Identity.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        var hasher = new PasswordHasher<ApplicationUser>();
        builder.HasData(
             new ApplicationUser
             {
                 Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                 Email = "admin@a7md.com",
                 NormalizedEmail = "ADMIN@A7MD.COM",
                 FirstName = "System",
                 LastName = "Admin",
                 UserName = "admin@a7md.com",
                 NormalizedUserName = "ADMIN@A7MD.COM",
                 Gender=Enum.Gender.Male,
                 DoB = new DateTime(1998, 11, 1),
                 Code= "ADM-1",
                 PasswordHash = hasher.HashPassword(null, "123"),
                 EmailConfirmed = true
             },
             new ApplicationUser
             {
                 Id = "9e224968-33e4-4652-b7b7-8574d048cdb9",
                 Email = "user@a7md.com",
                 NormalizedEmail = "USER@A7MD.COM",
                 FirstName = "System",
                 LastName = "User",
                 UserName = "user@a7md.com",
                 NormalizedUserName = "USER@A7MD.COM",
                 Gender = Enum.Gender.Male,
                 DoB = new DateTime(1998, 11, 1),
                 Code = "USR-1",
                 PasswordHash = hasher.HashPassword(null, "123"),
                 EmailConfirmed = true
             }
        );
    }
}
