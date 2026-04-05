using DomainDrivenERP.Domain.Entities.TaxDefinitions.DomainEvents;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.TaxDefinitions;

/// <summary>
/// Represents a single tax type in the system (VAT, Stamp, Table tax, Withholding...).
/// Supports the Dependency Graph pattern — a tax can depend on other taxes (compound/cascading).
/// Matches the Egyptian e-invoicing tax code system (V001..V010, ST01, Tbl01, W001..W016, etc.)
/// </summary>
public sealed class TaxDefinition : AggregateRoot, IAuditableEntity
{
    private readonly List<TaxDependency> _dependencies = new();

    private TaxDefinition() { }

    private TaxDefinition(
        Guid id,
        string code,
        string nameEn,
        string nameAr,
        TaxCalculationMethod calculationMethod,
        decimal rate,
        bool isWithholding,
        TaxAppliesTo appliesTo)
        : base(id)
    {
        Code = code;
        NameEn = nameEn;
        NameAr = nameAr;
        CalculationMethod = calculationMethod;
        Rate = rate;
        IsWithholding = isWithholding;
        AppliesTo = appliesTo;
        IsActive = true;
    }

    // ── Identity ──────────────────────────────────────────────
    public string Code { get; private set; }                  // "VAT", "ST01", "Tbl01", "W001"
    public string NameEn { get; private set; }                // "Value Added Tax"
    public string NameAr { get; private set; }                // "ضريبة القيمة المضافة"

    // ── Calculation ───────────────────────────────────────────
    public TaxCalculationMethod CalculationMethod { get; private set; }
    public decimal Rate { get; private set; }       // نسبة (0.14) أو مبلغ ثابت (10.00)
    public bool IsWithholding { get; private set; } // W codes — بتطرح مش بتضيف
    public TaxAppliesTo AppliesTo { get; private set; }

    // ── Dependency Graph ──────────────────────────────────────
    // "أنا بتحسب على الـ Base + قيمة هذه الضرائب"
    // مثال: VAT تعتمد على [Tbl01, ST01, ST02, Ent01, RD01, SC01, Mn01, MI01, OF01]
    public IReadOnlyCollection<TaxDependency> Dependencies => _dependencies;

    public bool IsActive { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    // ── Factory ───────────────────────────────────────────────
    public static Result<TaxDefinition> Create(
        string code,
        string nameEn,
        string nameAr,
        TaxCalculationMethod calculationMethod,
        decimal rate,
        bool isWithholding = false,
        TaxAppliesTo appliesTo = TaxAppliesTo.LineLevel)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<TaxDefinition>(new Error("TaxDef.InvalidCode", "Tax code is required."));

        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Failure<TaxDefinition>(new Error("TaxDef.InvalidName", "English name is required."));

        if (rate < 0)
            return Result.Failure<TaxDefinition>(new Error("TaxDef.InvalidRate", "Rate cannot be negative."));

        if (calculationMethod == TaxCalculationMethod.Percentage && rate > 1)
            return Result.Failure<TaxDefinition>(new Error("TaxDef.InvalidRate",
                "Percentage rate must be between 0 and 1 (e.g. 0.14 for 14%)."));

        var tax = new TaxDefinition(
            Guid.NewGuid(), code, nameEn, nameAr,
            calculationMethod, rate, isWithholding, appliesTo);

        tax.RaiseDomainEvent(new TaxDefinitionCreatedDomainEvent(tax.Id));
        return Result.Success(tax);
    }

    // ── Dependency Management ─────────────────────────────────
    public Result AddDependency(Guid dependsOnTaxId)
    {
        if (dependsOnTaxId == Id)
            return Result.Failure(new Error("TaxDef.CyclicDependency", "A tax cannot depend on itself."));

        if (_dependencies.Any(d => d.DependsOnTaxId == dependsOnTaxId))
            return Result.Failure(new Error("TaxDef.DuplicateDependency", "This dependency already exists."));

        _dependencies.Add(new TaxDependency(Guid.NewGuid(), Id, dependsOnTaxId));
        return Result.Success();
    }

    public void RemoveDependency(Guid dependsOnTaxId)
        => _dependencies.RemoveAll(d => d.DependsOnTaxId == dependsOnTaxId);

    public Result UpdateRate(decimal newRate)
    {
        if (newRate < 0)
            return Result.Failure(new Error("TaxDef.InvalidRate", "Rate cannot be negative."));

        Rate = newRate;
        return Result.Success();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}

// ── Value Object: TaxDependency ───────────────────────────────
public sealed class TaxDependency : BaseEntity
{
    private TaxDependency() { }

    public TaxDependency(Guid id, Guid taxDefinitionId, Guid dependsOnTaxId) : base(id)
    {
        TaxDefinitionId = taxDefinitionId;
        DependsOnTaxId = dependsOnTaxId;
    }

    public Guid TaxDefinitionId { get; private set; }  // صاحب الاعتمادية
    public Guid DependsOnTaxId { get; private set; }   // الضريبة اللي بيعتمد عليها
}

// ── Enums ─────────────────────────────────────────────────────
public enum TaxCalculationMethod
{
    Percentage = 1,  // نسبة من الـ Base أو (Base + Dependencies)
    FixedAmount = 2  // مبلغ ثابت بغض النظر عن القيمة
}

public enum TaxAppliesTo
{
    LineLevel    = 1,  // على مستوى الـ Line فقط
    InvoiceLevel = 2,  // على مستوى الفاتورة الكلية
    Both         = 3   // على الاتنين
}
