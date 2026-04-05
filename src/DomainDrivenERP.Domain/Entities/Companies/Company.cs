using DomainDrivenERP.Domain.Entities.Companies.DomainEvents;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.Companies;

/// <summary>
/// Represents a tenant company in the ERP system.
/// Every invoice, journal, sequence, and setting is scoped to a Company.
/// Holds all pipeline configuration defaults (TaxOrder, Stacking, Valuation...).
/// </summary>
public sealed class Company : AggregateRoot, IAuditableEntity
{
    private Company() { }

    private Company(
        Guid id,
        string nameEn,
        string nameAr,
        string taxRegistrationNumber,
        Guid baseCurrencyId,
        TaxOrderSetting defaultTaxOrder,
        StackingMode defaultStackingMode,
        StockValuationMethod stockValuation)
        : base(id)
    {
        NameEn = nameEn;
        NameAr = nameAr;
        TaxRegistrationNumber = taxRegistrationNumber;
        BaseCurrencyId = baseCurrencyId;
        DefaultTaxOrder = defaultTaxOrder;
        DefaultStackingMode = defaultStackingMode;
        StockValuation = stockValuation;
        IsActive = true;
    }

    // ── Identity ──────────────────────────────────────────────
    public string NameEn { get; private set; }
    public string NameAr { get; private set; }
    public string TaxRegistrationNumber { get; private set; }   // الرقم الضريبي
    public string? CommercialRegistrationNumber { get; private set; }

    // ── Financial Settings ────────────────────────────────────
    public Guid BaseCurrencyId { get; private set; }

    // ── Invoice Pipeline Defaults ─────────────────────────────
    public TaxOrderSetting DefaultTaxOrder { get; private set; }     // BeforeDiscount / AfterDiscount
    public StackingMode DefaultStackingMode { get; private set; }    // NoStacking / Full / Conditional

    // ── Inventory Settings ────────────────────────────────────
    public StockValuationMethod StockValuation { get; private set; } // FIFO / LIFO / Average
    public bool ReserveStockOnSalesOrder { get; private set; }
    public bool AllowNegativeStock { get; private set; }

    // ── Discount Caps ─────────────────────────────────────────
    public decimal? MaxDiscountPercentPerLine { get; private set; }   // 0 = no cap
    public decimal? MaxDiscountAmountPerInvoice { get; private set; } // 0 = no cap

    // ── Cancellation Policy ───────────────────────────────────
    public bool AllowCancelWithReservation { get; private set; }     // لو SalesOrder فيها Reservation

    public bool IsActive { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? Address { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    // ── Factory ───────────────────────────────────────────────
    public static Result<Company> Create(
        string nameEn,
        string nameAr,
        string taxRegistrationNumber,
        Guid baseCurrencyId,
        TaxOrderSetting defaultTaxOrder = TaxOrderSetting.AfterDiscount,
        StackingMode defaultStackingMode = StackingMode.NoStacking,
        StockValuationMethod stockValuation = StockValuationMethod.Average)
    {
        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Failure<Company>(new Error("Company.InvalidName", "English name is required."));

        if (string.IsNullOrWhiteSpace(nameAr))
            return Result.Failure<Company>(new Error("Company.InvalidNameAr", "Arabic name is required."));

        if (string.IsNullOrWhiteSpace(taxRegistrationNumber))
            return Result.Failure<Company>(new Error("Company.InvalidTaxNumber", "Tax registration number is required."));

        if (baseCurrencyId == Guid.Empty)
            return Result.Failure<Company>(new Error("Company.InvalidCurrency", "Base currency is required."));

        var company = new Company(
            Guid.NewGuid(), nameEn, nameAr, taxRegistrationNumber,
            baseCurrencyId, defaultTaxOrder, defaultStackingMode, stockValuation);

        company.RaiseDomainEvent(new CompanyCreatedDomainEvent(company.Id));
        return Result.Success(company);
    }

    // ── Settings Updates ──────────────────────────────────────
    public Result UpdatePipelineDefaults(TaxOrderSetting taxOrder, StackingMode stacking)
    {
        DefaultTaxOrder = taxOrder;
        DefaultStackingMode = stacking;
        return Result.Success();
    }

    public Result UpdateDiscountCaps(decimal? maxPercentPerLine, decimal? maxAmountPerInvoice)
    {
        if (maxPercentPerLine is < 0 or > 100)
            return Result.Failure(new Error("Company.InvalidCap", "Discount cap % must be between 0 and 100."));

        MaxDiscountPercentPerLine = maxPercentPerLine;
        MaxDiscountAmountPerInvoice = maxAmountPerInvoice;
        return Result.Success();
    }

    public void UpdateInventorySettings(
        bool reserveOnSalesOrder, bool allowNegative, StockValuationMethod valuation)
    {
        ReserveStockOnSalesOrder = reserveOnSalesOrder;
        AllowNegativeStock = allowNegative;
        StockValuation = valuation;
    }

    public void UpdateContactInfo(string? address, string? phone, string? email, string? logoUrl)
    {
        Address = address;
        Phone = phone;
        Email = email;
        LogoUrl = logoUrl;
    }
}

public enum StockValuationMethod
{
    FIFO    = 1,   // First In First Out
    LIFO    = 2,   // Last In First Out
    Average = 3    // Weighted Average Cost
}
