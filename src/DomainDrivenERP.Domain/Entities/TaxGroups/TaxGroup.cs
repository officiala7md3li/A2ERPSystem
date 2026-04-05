using DomainDrivenERP.Domain.Entities.TaxGroups.DomainEvents;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.TaxGroups;

/// <summary>
/// A named collection of TaxDefinitions applied together to an Item or Invoice Line.
/// Example: "Egyptian Standard Tax" = [VAT 14%, Stamp 0.5%, Table 5%]
/// Each TaxGroupItem can override the default rate from TaxDefinition.
/// </summary>
public sealed class TaxGroup : AggregateRoot, IAuditableEntity
{
    private readonly List<TaxGroupItem> _items = new();

    private TaxGroup() { }

    private TaxGroup(Guid id, Guid companyId, string nameEn, string nameAr, bool isDefault)
        : base(id)
    {
        CompanyId = companyId;
        NameEn = nameEn;
        NameAr = nameAr;
        IsDefault = isDefault;
        IsActive = true;
    }

    public Guid CompanyId { get; private set; }       // Per-company tax groups
    public string NameEn { get; private set; }         // "Egyptian Standard Tax"
    public string NameAr { get; private set; }         // "الضريبة المصرية القياسية"
    public bool IsDefault { get; private set; }        // Used when Item has no explicit TaxGroup
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<TaxGroupItem> Items => _items;
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public static Result<TaxGroup> Create(
        Guid companyId, string nameEn, string nameAr, bool isDefault = false)
    {
        if (companyId == Guid.Empty)
            return Result.Failure<TaxGroup>(new Error("TaxGroup.InvalidCompany", "Company ID is required."));

        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Failure<TaxGroup>(new Error("TaxGroup.InvalidName", "English name is required."));

        var group = new TaxGroup(Guid.NewGuid(), companyId, nameEn, nameAr, isDefault);
        group.RaiseDomainEvent(new TaxGroupCreatedDomainEvent(group.Id, companyId, nameEn));
        return Result.Success(group);
    }

    // ── Tax Items Management ──────────────────────────────────
    public Result AddTax(Guid taxDefinitionId, decimal? overrideRate = null)
    {
        if (taxDefinitionId == Guid.Empty)
            return Result.Failure(new Error("TaxGroup.InvalidTaxDef", "Tax definition ID is required."));

        if (_items.Any(i => i.TaxDefinitionId == taxDefinitionId))
            return Result.Failure(new Error("TaxGroup.DuplicateTax", "This tax is already in the group."));

        if (overrideRate.HasValue && (overrideRate < 0 || overrideRate > 1))
            return Result.Failure(new Error("TaxGroup.InvalidRate", "Override rate must be between 0 and 1."));

        _items.Add(new TaxGroupItem(Guid.NewGuid(), Id, taxDefinitionId, overrideRate));
        return Result.Success();
    }

    public Result RemoveTax(Guid taxDefinitionId)
    {
        var item = _items.FirstOrDefault(i => i.TaxDefinitionId == taxDefinitionId);
        if (item is null)
            return Result.Failure(new Error("TaxGroup.TaxNotFound", "Tax not found in this group."));

        _items.Remove(item);
        return Result.Success();
    }

    public void SetAsDefault() => IsDefault = true;
    public void UnsetDefault() => IsDefault = false;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

/// <summary>
/// Links a TaxDefinition to a TaxGroup with an optional rate override.
/// Allows the same tax code to have different rates in different groups.
/// </summary>
public sealed class TaxGroupItem : BaseEntity
{
    private TaxGroupItem() { }

    public TaxGroupItem(Guid id, Guid taxGroupId, Guid taxDefinitionId, decimal? overrideRate)
        : base(id)
    {
        TaxGroupId = taxGroupId;
        TaxDefinitionId = taxDefinitionId;
        OverrideRate = overrideRate;  // null = use TaxDefinition.Rate
    }

    public Guid TaxGroupId { get; private set; }
    public Guid TaxDefinitionId { get; private set; }
    public decimal? OverrideRate { get; private set; }  // لو null → بياخد الـ Rate من TaxDefinition

    public void UpdateRate(decimal? newRate) => OverrideRate = newRate;
}
