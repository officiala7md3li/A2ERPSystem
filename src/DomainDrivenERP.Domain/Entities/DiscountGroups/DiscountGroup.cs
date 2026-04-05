using DomainDrivenERP.Domain.Entities.DiscountGroups.DomainEvents;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.DiscountGroups;

/// <summary>
/// A named collection of discount rules applied together to an Item or Invoice Line.
/// Can contain multiple rules of different types (Fixed, Percentage, Tiered, Seasonal).
/// The DiscountResolver uses these rules + StackingMode to compute the final discount.
/// </summary>
public sealed class DiscountGroup : AggregateRoot, IAuditableEntity
{
    private readonly List<DiscountRule> _rules = new();

    private DiscountGroup() { }

    private DiscountGroup(Guid id, Guid companyId, string nameEn, string nameAr)
        : base(id)
    {
        CompanyId = companyId;
        NameEn = nameEn;
        NameAr = nameAr;
        IsActive = true;
    }

    public Guid CompanyId { get; private set; }
    public string NameEn { get; private set; }    // "VIP Customers Discount"
    public string NameAr { get; private set; }    // "خصم عملاء VIP"
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<DiscountRule> Rules => _rules;
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public static Result<DiscountGroup> Create(Guid companyId, string nameEn, string nameAr)
    {
        if (companyId == Guid.Empty)
            return Result.Failure<DiscountGroup>(new Error("DiscountGroup.InvalidCompany", "Company ID is required."));

        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Failure<DiscountGroup>(new Error("DiscountGroup.InvalidName", "English name is required."));

        var group = new DiscountGroup(Guid.NewGuid(), companyId, nameEn, nameAr);
        group.RaiseDomainEvent(new DiscountGroupCreatedDomainEvent(group.Id, companyId, nameEn));
        return Result.Success(group);
    }

    // ── Rule Management ───────────────────────────────────────
    public Result AddFixedDiscount(decimal amount)
    {
        if (amount <= 0)
            return Result.Failure(new Error("DiscountGroup.InvalidAmount", "Fixed amount must be > 0."));

        _rules.Add(DiscountRule.CreateFixed(Id, amount));
        return Result.Success();
    }

    public Result AddPercentageDiscount(decimal percent)
    {
        if (percent is <= 0 or > 100)
            return Result.Failure(new Error("DiscountGroup.InvalidPercent", "Percentage must be between 0 and 100."));

        _rules.Add(DiscountRule.CreatePercentage(Id, percent));
        return Result.Success();
    }

    public Result AddTieredDiscount(IEnumerable<DiscountTier> tiers)
    {
        var tierList = tiers.OrderBy(t => t.MinQuantity).ToList();
        if (!tierList.Any())
            return Result.Failure(new Error("DiscountGroup.NoTiers", "At least one tier is required."));

        _rules.Add(DiscountRule.CreateTiered(Id, tierList));
        return Result.Success();
    }

    public Result AddSeasonalDiscount(decimal percent, DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            return Result.Failure(new Error("DiscountGroup.InvalidDateRange", "Start date must be before end date."));

        if (percent is <= 0 or > 100)
            return Result.Failure(new Error("DiscountGroup.InvalidPercent", "Percentage must be between 0 and 100."));

        _rules.Add(DiscountRule.CreateSeasonal(Id, percent, startDate, endDate));
        return Result.Success();
    }

    public Result RemoveRule(Guid ruleId)
    {
        var rule = _rules.FirstOrDefault(r => r.Id == ruleId);
        if (rule is null)
            return Result.Failure(new Error("DiscountGroup.RuleNotFound", "Rule not found."));

        _rules.Remove(rule);
        return Result.Success();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

// ── DiscountRule (Child Entity) ───────────────────────────────
public sealed class DiscountRule : BaseEntity
{
    private DiscountRule() { }

    private DiscountRule(Guid id, Guid discountGroupId, DiscountType type,
        decimal value, DateTime? startDate, DateTime? endDate, string? tiersJson)
        : base(id)
    {
        DiscountGroupId = discountGroupId;
        Type = type;
        Value = value;
        StartDate = startDate;
        EndDate = endDate;
        TiersJson = tiersJson;
        IsActive = true;
    }

    public Guid DiscountGroupId { get; private set; }
    public DiscountType Type { get; private set; }
    public decimal Value { get; private set; }         // نسبة أو مبلغ
    public DateTime? StartDate { get; private set; }   // للـ Seasonal
    public DateTime? EndDate { get; private set; }     // للـ Seasonal
    public string? TiersJson { get; private set; }     // JSON للـ Tiered rules
    public bool IsActive { get; private set; }

    internal static DiscountRule CreateFixed(Guid groupId, decimal amount) =>
        new(Guid.NewGuid(), groupId, DiscountType.FixedAmount, amount, null, null, null);

    internal static DiscountRule CreatePercentage(Guid groupId, decimal percent) =>
        new(Guid.NewGuid(), groupId, DiscountType.Percentage, percent, null, null, null);

    internal static DiscountRule CreateSeasonal(Guid groupId, decimal percent, DateTime start, DateTime end) =>
        new(Guid.NewGuid(), groupId, DiscountType.Seasonal, percent, start, end, null);

    internal static DiscountRule CreateTiered(Guid groupId, List<DiscountTier> tiers) =>
        new(Guid.NewGuid(), groupId, DiscountType.Tiered, 0, null, null,
            System.Text.Json.JsonSerializer.Serialize(tiers));

    public List<DiscountTier> GetTiers() =>
        TiersJson is null ? [] :
        System.Text.Json.JsonSerializer.Deserialize<List<DiscountTier>>(TiersJson) ?? [];
}

// ── DiscountTier (Value Object — stored as JSON) ──────────────
public sealed record DiscountTier(
    decimal MinQuantity,      // من كمية
    decimal? MaxQuantity,     // لكمية (null = unlimited)
    decimal DiscountPercent); // نسبة الخصم
