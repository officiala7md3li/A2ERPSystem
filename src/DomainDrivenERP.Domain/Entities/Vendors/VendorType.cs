using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.Vendors;

/// <summary>
/// Lookup entity replacing the former VendorType enum.
/// Examples: Local, Foreign, Government.
/// Shared: Customer can also reference this via nullable FK.
/// </summary>
public sealed class VendorType : AggregateRoot, IAuditableEntity
{
    private VendorType() { }

    private VendorType(Guid id, string code, string nameEn, string nameAr)
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

    public static Result<VendorType> Create(string code, string nameEn, string nameAr)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<VendorType>(new Error("VendorType.InvalidCode", "Code is required."));
        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Failure<VendorType>(new Error("VendorType.InvalidName", "English name is required."));
        if (string.IsNullOrWhiteSpace(nameAr))
            return Result.Failure<VendorType>(new Error("VendorType.InvalidNameAr", "Arabic name is required."));

        return Result.Success(new VendorType(Guid.NewGuid(), code, nameEn, nameAr));
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
