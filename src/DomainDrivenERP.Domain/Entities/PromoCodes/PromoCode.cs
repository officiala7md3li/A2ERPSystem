using DomainDrivenERP.Domain.Entities.PromoCodes.DomainEvents;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.PromoCodes;

/// <summary>
/// A time-limited and/or usage-limited promotional discount code.
/// Can be linked to a DiscountGroup or define its own discount inline.
/// Concurrency-safe usage tracking: validated via Redis (primary) + DB lock (fallback).
/// </summary>
public sealed class PromoCode : AggregateRoot, IAuditableEntity
{
    private readonly List<PromoCodeUsage> _usages = new();

    private PromoCode() { }

    private PromoCode(
        Guid id, Guid companyId, string code, string description,
        PromoDiscountType discountType, decimal discountValue,
        DateTime? startDate, DateTime? endDate,
        int? maxUses, int? maxUsesPerCustomer,
        decimal? minimumOrderAmount, bool isCombinable,
        Guid? discountGroupId)
        : base(id)
    {
        CompanyId = companyId;
        Code = code;
        Description = description;
        DiscountType = discountType;
        DiscountValue = discountValue;
        StartDate = startDate;
        EndDate = endDate;
        MaxUses = maxUses;
        MaxUsesPerCustomer = maxUsesPerCustomer;
        MinimumOrderAmount = minimumOrderAmount;
        IsCombinable = isCombinable;
        DiscountGroupId = discountGroupId;
        IsActive = true;
        CurrentUses = 0;
    }

    // ── Identity ──────────────────────────────────────────────
    public Guid CompanyId { get; private set; }
    public string Code { get; private set; }              // "RAMADAN2026"
    public string Description { get; private set; }

    // ── Discount Definition ───────────────────────────────────
    public PromoDiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }    // نسبة (20) أو مبلغ (50)
    public Guid? DiscountGroupId { get; private set; }    // optional — use a DiscountGroup instead

    // ── Validity Constraints ──────────────────────────────────
    public DateTime? StartDate { get; private set; }      // null = no start limit
    public DateTime? EndDate { get; private set; }        // null = no end limit
    public int? MaxUses { get; private set; }             // null = unlimited
    public int? MaxUsesPerCustomer { get; private set; }  // null = unlimited per customer
    public decimal? MinimumOrderAmount { get; private set; }

    // ── Stacking Behavior ─────────────────────────────────────
    public bool IsCombinable { get; private set; }        // true = stacks with other discounts

    // ── Usage Tracking ────────────────────────────────────────
    public int CurrentUses { get; private set; }          // atomic counter (synced from Redis)
    public bool IsActive { get; private set; }

    public IReadOnlyCollection<PromoCodeUsage> Usages => _usages;

    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    // ── Factory ───────────────────────────────────────────────
    public static Result<PromoCode> Create(
        Guid companyId, string code, string description,
        PromoDiscountType discountType, decimal discountValue,
        DateTime? startDate = null, DateTime? endDate = null,
        int? maxUses = null, int? maxUsesPerCustomer = null,
        decimal? minimumOrderAmount = null,
        bool isCombinable = false,
        Guid? discountGroupId = null)
    {
        if (companyId == Guid.Empty)
            return Result.Failure<PromoCode>(new Error("PromoCode.InvalidCompany", "Company ID is required."));

        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<PromoCode>(new Error("PromoCode.InvalidCode", "Promo code is required."));

        if (discountValue <= 0)
            return Result.Failure<PromoCode>(new Error("PromoCode.InvalidValue", "Discount value must be > 0."));

        if (discountType == PromoDiscountType.Percentage && discountValue > 100)
            return Result.Failure<PromoCode>(new Error("PromoCode.InvalidPercent", "Percentage cannot exceed 100."));

        if (startDate.HasValue && endDate.HasValue && startDate >= endDate)
            return Result.Failure<PromoCode>(new Error("PromoCode.InvalidDates", "StartDate must be before EndDate."));

        if (maxUses is <= 0)
            return Result.Failure<PromoCode>(new Error("PromoCode.InvalidMaxUses", "MaxUses must be > 0."));

        var promo = new PromoCode(Guid.NewGuid(), companyId, code.ToUpperInvariant(),
            description, discountType, discountValue,
            startDate, endDate, maxUses, maxUsesPerCustomer,
            minimumOrderAmount, isCombinable, discountGroupId);

        promo.RaiseDomainEvent(new PromoCodeCreatedDomainEvent(promo.Id, companyId, code));
        return Result.Success(promo);
    }

    // ── Validation ────────────────────────────────────────────
    public Result ValidateUsage(Guid customerId, decimal orderAmount, DateTime now)
    {
        if (!IsActive)
            return Result.Failure(new Error("PromoCode.Inactive", "This promo code is no longer active."));

        if (StartDate.HasValue && now < StartDate.Value)
            return Result.Failure(new Error("PromoCode.NotStarted", "This promo code is not yet valid."));

        if (EndDate.HasValue && now > EndDate.Value)
            return Result.Failure(new Error("PromoCode.Expired", "This promo code has expired."));

        if (MaxUses.HasValue && CurrentUses >= MaxUses.Value)
            return Result.Failure(new Error("PromoCode.MaxUsesReached", "This promo code has reached its maximum usage."));

        if (MinimumOrderAmount.HasValue && orderAmount < MinimumOrderAmount.Value)
            return Result.Failure(new Error("PromoCode.BelowMinimum",
                $"Order must be at least {MinimumOrderAmount.Value} to use this code."));

        if (MaxUsesPerCustomer.HasValue)
        {
            int customerUses = _usages.Count(u => u.CustomerId == customerId);
            if (customerUses >= MaxUsesPerCustomer.Value)
                return Result.Failure(new Error("PromoCode.CustomerLimitReached",
                    "You have reached the maximum usage limit for this promo code."));
        }

        return Result.Success();
    }

    // ── Usage Recording ───────────────────────────────────────
    public Result RecordUsage(Guid customerId, Guid invoiceId, decimal discountApplied)
    {
        _usages.Add(new PromoCodeUsage(Guid.NewGuid(), Id, customerId, invoiceId, discountApplied, DateTime.UtcNow));
        CurrentUses++;

        // Auto-deactivate if max reached
        if (MaxUses.HasValue && CurrentUses >= MaxUses.Value)
            IsActive = false;

        RaiseDomainEvent(new PromoCodeUsedDomainEvent(Id, customerId, invoiceId, discountApplied));
        return Result.Success();
    }

    // ── Sync from Redis ───────────────────────────────────────
    /// <summary>Called by background sync to update counter from Redis atomic value.</summary>
    public void SyncUsageCounter(int redisCounter) => CurrentUses = redisCounter;

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    // ── Compute Discount Amount ───────────────────────────────
    public decimal ComputeDiscount(decimal orderTotal) =>
        DiscountType == PromoDiscountType.Percentage
            ? orderTotal * (DiscountValue / 100)
            : Math.Min(DiscountValue, orderTotal);
}

// ── PromoCodeUsage (Child Entity — audit trail) ───────────────
public sealed class PromoCodeUsage : BaseEntity
{
    private PromoCodeUsage() { }

    public PromoCodeUsage(Guid id, Guid promoCodeId, Guid customerId,
        Guid invoiceId, decimal discountApplied, DateTime usedAt)
        : base(id)
    {
        PromoCodeId = promoCodeId;
        CustomerId = customerId;
        InvoiceId = invoiceId;
        DiscountApplied = discountApplied;
        UsedAt = usedAt;
    }

    public Guid PromoCodeId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid InvoiceId { get; private set; }
    public decimal DiscountApplied { get; private set; }
    public DateTime UsedAt { get; private set; }
}

// ── Enums ─────────────────────────────────────────────────────
public enum PromoDiscountType
{
    Percentage  = 1,  // 20% off order total
    FixedAmount = 2   // 50 EGP off
}
