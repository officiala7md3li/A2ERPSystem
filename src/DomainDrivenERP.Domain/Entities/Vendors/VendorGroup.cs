using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.Vendors;

/// <summary>
/// Lookup entity for grouping vendors/customers.
/// Examples: Preferred, Approved, Blacklisted.
/// </summary>
public sealed class VendorGroup : AggregateRoot, IAuditableEntity
{
    private VendorGroup() { }

    private VendorGroup(Guid id, string code, string nameEn, string nameAr)
        : base(id)
    {
        Code = code.Trim().ToUpperInvariant();
        NameEn = nameEn.Trim();
        NameAr = nameAr.Trim();
        IsActive = true;
    }

    public string Code { get; private set; }
    public string NameEn { get; private set; }
    public string NameAr { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public static Result<VendorGroup> Create(string code, string nameEn, string nameAr)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<VendorGroup>(new Error("VendorGroup.InvalidCode", "Code is required."));
        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Failure<VendorGroup>(new Error("VendorGroup.InvalidName", "English name is required."));
        if (string.IsNullOrWhiteSpace(nameAr))
            return Result.Failure<VendorGroup>(new Error("VendorGroup.InvalidNameAr", "Arabic name is required."));

        return Result.Success(new VendorGroup(Guid.NewGuid(), code, nameEn, nameAr));
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
