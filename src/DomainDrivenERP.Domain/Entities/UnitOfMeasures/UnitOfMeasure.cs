using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.UnitOfMeasures;

/// <summary>
/// Unit of measure for product quantities (PCS, KG, L, M, BOX...).
/// Supports conversion factor between units of the same group.
/// </summary>
public sealed class UnitOfMeasure : AggregateRoot, IAuditableEntity
{
    private UnitOfMeasure() { }

    private UnitOfMeasure(Guid id, string code, string nameEn, string nameAr,
        UomType type, decimal conversionFactor, Guid? baseUomId)
        : base(id)
    {
        Code = code;
        NameEn = nameEn;
        NameAr = nameAr;
        Type = type;
        ConversionFactor = conversionFactor;
        BaseUomId = baseUomId;
        IsActive = true;
    }

    public string Code { get; private set; }           // PCS, KG, L, M, BOX
    public string NameEn { get; private set; }         // Piece
    public string NameAr { get; private set; }         // قطعة
    public UomType Type { get; private set; }          // Quantity, Weight, Volume, Length
    public decimal ConversionFactor { get; private set; } // 1 BOX = 12 PCS → factor = 12
    public Guid? BaseUomId { get; private set; }       // الوحدة الأساسية (PCS هي base لـ BOX)
    public bool IsActive { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public static Result<UnitOfMeasure> Create(
        string code, string nameEn, string nameAr,
        UomType type, decimal conversionFactor = 1, Guid? baseUomId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<UnitOfMeasure>(new Error("UoM.InvalidCode", "Code is required."));

        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Failure<UnitOfMeasure>(new Error("UoM.InvalidName", "English name is required."));

        if (conversionFactor <= 0)
            return Result.Failure<UnitOfMeasure>(new Error("UoM.InvalidFactor", "Conversion factor must be > 0."));

        return Result.Success(new UnitOfMeasure(
            Guid.NewGuid(), code.ToUpperInvariant(), nameEn, nameAr,
            type, conversionFactor, baseUomId));
    }

    /// <summary>Convert a quantity from this UoM to the base UoM.</summary>
    public decimal ToBase(decimal quantity) => quantity * ConversionFactor;

    /// <summary>Convert a quantity from the base UoM to this UoM.</summary>
    public decimal FromBase(decimal quantity) => quantity / ConversionFactor;
}

public enum UomType
{
    Quantity = 1,   // PCS, BOX, DOZEN
    Weight   = 2,   // KG, G, TON, LB
    Volume   = 3,   // L, ML, M3
    Length   = 4,   // M, CM, MM, FT
    Area     = 5,   // M2, FT2
    Time     = 6    // HR, DAY, MONTH
}
