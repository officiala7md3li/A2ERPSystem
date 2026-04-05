using DomainDrivenERP.Domain.Entities.Vendors.DomainEvents;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.Vendors;

/// <summary>
/// Represents a supplier/vendor that the company purchases from.
/// Intentionally separate from Customer — a party can be both,
/// but they are registered separately with different IDs.
/// </summary>
public sealed class Vendor : AggregateRoot, IAuditableEntity
{
    private Vendor() { }

    private Vendor(
        Guid id,
        Guid companyId,
        string nameEn,
        string nameAr,
        string taxRegistrationNumber,
        Guid defaultCurrencyId,
        Guid? vendorTypeId,
        Guid? vendorCategoryId,
        Guid? vendorGroupId)
        : base(id)
    {
        CompanyId = companyId;
        NameEn = nameEn;
        NameAr = nameAr;
        TaxRegistrationNumber = taxRegistrationNumber;
        DefaultCurrencyId = defaultCurrencyId;
        VendorTypeId = vendorTypeId;
        VendorCategoryId = vendorCategoryId;
        VendorGroupId = vendorGroupId;
        IsActive = true;
    }

    // ── Identity ──────────────────────────────────────────────
    public Guid CompanyId { get; private set; }
    public string NameEn { get; private set; }
    public string NameAr { get; private set; }
    public string TaxRegistrationNumber { get; private set; }
    public string? CommercialRegistrationNumber { get; private set; }

    // ── Classification (nullable FK → lookup entities) ────────
    public Guid? VendorTypeId { get; private set; }
    public Guid? VendorCategoryId { get; private set; }
    public Guid? VendorGroupId { get; private set; }

    // ── Financial ─────────────────────────────────────────────
    public Guid DefaultCurrencyId { get; private set; }
    public decimal? CreditLimit { get; private set; }
    public int? PaymentTermDays { get; private set; }    // صافي 30 يوم مثلاً

    // ── Contact ───────────────────────────────────────────────
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public string? ContactPersonName { get; private set; }

    // ── Tax Settings ──────────────────────────────────────────
    public Guid? DefaultTaxGroupId { get; private set; }  // الضرائب الافتراضية على فواتيره

    public bool IsActive { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    // ── Factory ───────────────────────────────────────────────
    public static Result<Vendor> Create(
        Guid companyId,
        string nameEn,
        string nameAr,
        string taxRegistrationNumber,
        Guid defaultCurrencyId,
        Guid? vendorTypeId = null,
        Guid? vendorCategoryId = null,
        Guid? vendorGroupId = null)
    {
        if (companyId == Guid.Empty)
            return Result.Failure<Vendor>(new Error("Vendor.InvalidCompany", "Company ID is required."));

        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Failure<Vendor>(new Error("Vendor.InvalidName", "English name is required."));

        if (string.IsNullOrWhiteSpace(nameAr))
            return Result.Failure<Vendor>(new Error("Vendor.InvalidNameAr", "Arabic name is required."));

        if (string.IsNullOrWhiteSpace(taxRegistrationNumber))
            return Result.Failure<Vendor>(new Error("Vendor.InvalidTaxNumber", "Tax registration number is required."));

        if (defaultCurrencyId == Guid.Empty)
            return Result.Failure<Vendor>(new Error("Vendor.InvalidCurrency", "Default currency is required."));

        var vendor = new Vendor(Guid.NewGuid(), companyId, nameEn, nameAr,
            taxRegistrationNumber, defaultCurrencyId, vendorTypeId, vendorCategoryId, vendorGroupId);

        vendor.RaiseDomainEvent(new VendorCreatedDomainEvent(vendor.Id, companyId, nameEn));
        return Result.Success(vendor);
    }

    // ── Domain Methods ────────────────────────────────────────
    public Result SetCreditLimit(decimal limit)
    {
        if (limit < 0)
            return Result.Failure(new Error("Vendor.InvalidCreditLimit", "Credit limit cannot be negative."));
        CreditLimit = limit;
        return Result.Success();
    }

    public void SetDefaultTaxGroup(Guid taxGroupId) => DefaultTaxGroupId = taxGroupId;

    public void UpdateContactInfo(string? phone, string? email, string? address, string? contactPerson)
    {
        Phone = phone;
        Email = email;
        Address = address;
        ContactPersonName = contactPerson;
    }

    public void SetClassification(Guid? typeId, Guid? categoryId, Guid? groupId)
    {
        VendorTypeId = typeId;
        VendorCategoryId = categoryId;
        VendorGroupId = groupId;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
