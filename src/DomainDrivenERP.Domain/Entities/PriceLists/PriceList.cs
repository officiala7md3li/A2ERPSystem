using DomainDrivenERP.Domain.Entities.PriceLists.DomainEvents;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.PriceLists;

/// <summary>
/// A named price list that defines specific prices or discount percentages
/// for items, optionally scoped to specific customers.
/// When a customer creates an invoice, the system checks if they have
/// an assigned PriceList and applies it before other discounts.
/// </summary>
public sealed class PriceList : AggregateRoot, IAuditableEntity
{
    private readonly List<PriceListItem> _items = new();
    private readonly List<Guid> _assignedCustomerIds = new();

    private PriceList() { }

    private PriceList(
        Guid id, Guid companyId, Guid currencyId,
        string nameEn, string nameAr,
        DateTime? validFrom, DateTime? validTo, bool isDefault)
        : base(id)
    {
        CompanyId = companyId;
        CurrencyId = currencyId;
        NameEn = nameEn;
        NameAr = nameAr;
        ValidFrom = validFrom;
        ValidTo = validTo;
        IsDefault = isDefault;
        IsActive = true;
    }

    public Guid CompanyId { get; private set; }
    public Guid CurrencyId { get; private set; }          // العملة الخاصة بالـ PriceList
    public string NameEn { get; private set; }
    public string NameAr { get; private set; }
    public DateTime? ValidFrom { get; private set; }      // تاريخ بدء صلاحية القائمة
    public DateTime? ValidTo { get; private set; }        // تاريخ انتهاء صلاحيتها
    public bool IsDefault { get; private set; }           // للعملاء الذين ليس لهم PriceList محددة
    public bool IsActive { get; private set; }

    // Items: specific prices per product
    public IReadOnlyCollection<PriceListItem> Items => _items;

    // Customers assigned to this price list
    public IReadOnlyCollection<Guid> AssignedCustomerIds => _assignedCustomerIds;

    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    // ── Factory ───────────────────────────────────────────────
    public static Result<PriceList> Create(
        Guid companyId, Guid currencyId,
        string nameEn, string nameAr,
        DateTime? validFrom = null, DateTime? validTo = null,
        bool isDefault = false)
    {
        if (companyId == Guid.Empty)
            return Result.Failure<PriceList>(new Error("PriceList.InvalidCompany", "Company ID is required."));

        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Failure<PriceList>(new Error("PriceList.InvalidName", "English name is required."));

        if (validFrom.HasValue && validTo.HasValue && validFrom >= validTo)
            return Result.Failure<PriceList>(new Error("PriceList.InvalidDates", "ValidFrom must be before ValidTo."));

        var list = new PriceList(Guid.NewGuid(), companyId, currencyId,
            nameEn, nameAr, validFrom, validTo, isDefault);

        list.RaiseDomainEvent(new PriceListCreatedDomainEvent(list.Id, companyId, nameEn));
        return Result.Success(list);
    }

    // ── Item Management ───────────────────────────────────────
    public Result AddItem(Guid itemId, decimal? fixedPrice, decimal? discountPercent)
    {
        if (itemId == Guid.Empty)
            return Result.Failure(new Error("PriceList.InvalidItem", "Item ID is required."));

        if (_items.Any(i => i.ItemId == itemId))
            return Result.Failure(new Error("PriceList.DuplicateItem", "Item already exists in this price list."));

        if (fixedPrice is null && discountPercent is null)
            return Result.Failure(new Error("PriceList.NoPrice", "Either fixed price or discount % is required."));

        if (discountPercent is < 0 or > 100)
            return Result.Failure(new Error("PriceList.InvalidPercent", "Discount % must be 0–100."));

        _items.Add(new PriceListItem(Guid.NewGuid(), Id, itemId, fixedPrice, discountPercent));
        return Result.Success();
    }

    public void RemoveItem(Guid itemId)
        => _items.RemoveAll(i => i.ItemId == itemId);

    // ── Customer Assignment ───────────────────────────────────
    public Result AssignCustomer(Guid customerId)
    {
        if (_assignedCustomerIds.Contains(customerId))
            return Result.Failure(new Error("PriceList.AlreadyAssigned", "Customer already assigned."));

        _assignedCustomerIds.Add(customerId);
        return Result.Success();
    }

    public void UnassignCustomer(Guid customerId)
        => _assignedCustomerIds.Remove(customerId);

    // ── Validity Check ────────────────────────────────────────
    public bool IsValidAt(DateTime date)
    {
        if (!IsActive) return false;
        if (ValidFrom.HasValue && date < ValidFrom.Value) return false;
        if (ValidTo.HasValue && date > ValidTo.Value) return false;
        return true;
    }

    public decimal? GetPriceForItem(Guid itemId) =>
        _items.FirstOrDefault(i => i.ItemId == itemId)?.FixedPrice;

    public decimal? GetDiscountForItem(Guid itemId) =>
        _items.FirstOrDefault(i => i.ItemId == itemId)?.DiscountPercent;

    public void SetAsDefault() => IsDefault = true;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

// ── PriceListItem (Child Entity) ──────────────────────────────
public sealed class PriceListItem : BaseEntity
{
    private PriceListItem() { }

    public PriceListItem(Guid id, Guid priceListId, Guid itemId,
        decimal? fixedPrice, decimal? discountPercent)
        : base(id)
    {
        PriceListId = priceListId;
        ItemId = itemId;
        FixedPrice = fixedPrice;           // سعر محدد لهذا العميل
        DiscountPercent = discountPercent; // أو نسبة خصم على السعر الأصلي
    }

    public Guid PriceListId { get; private set; }
    public Guid ItemId { get; private set; }
    public decimal? FixedPrice { get; private set; }      // سعر ثابت بديل
    public decimal? DiscountPercent { get; private set; } // نسبة خصم على السعر الأصلي
}
