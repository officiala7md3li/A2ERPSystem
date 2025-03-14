using Microsoft.AspNetCore.Identity;
using DomainDrivenERP.Identity.Enum;

namespace DomainDrivenERP.Identity.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Code { get; set; }
    public byte[]? ProfilePic { get; set; }
    public Gender Gender { get; set; }
    public DateTime? DoB { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

}
