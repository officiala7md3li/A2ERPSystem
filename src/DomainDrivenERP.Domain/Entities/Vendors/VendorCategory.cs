using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.Vendors;

/// <summary>
/// Lookup entity for categorising vendors/customers.
/// Examples: Raw Materials, Services, Logistics.
/// </summary>
public sealed class VendorCategory : AggregateRoot, IAuditableEntity
{
    private VendorCategory() { }

    private VendorCategory(Guid id, string code, string nameEn, string nameAr)
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

    public static Result<VendorCategory> Create(string code, string nameEn, string nameAr)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<VendorCategory>(new Error("VendorCategory.InvalidCode", "Code is required."));
        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Failure<VendorCategory>(new Error("VendorCategory.InvalidName", "English name is required."));
        if (string.IsNullOrWhiteSpace(nameAr))
            return Result.Failure<VendorCategory>(new Error("VendorCategory.InvalidNameAr", "Arabic name is required."));

        return Result.Success(new VendorCategory(Guid.NewGuid(), code, nameEn, nameAr));
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
