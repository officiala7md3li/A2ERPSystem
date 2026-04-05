using DomainDrivenERP.Domain.Entities.Warehouses.DomainEvents;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.Warehouses;

/// <summary>
/// Represents a physical or logical storage location.
/// Supports a two-level hierarchy: Main Warehouse → Sub Warehouses.
/// All inventory movements, reservations, and stock counts are scoped to a Warehouse.
/// </summary>
public sealed class Warehouse : AggregateRoot, IAuditableEntity
{
    private readonly List<Warehouse> _subWarehouses = new();

    private Warehouse() { }

    private Warehouse(
        Guid id, Guid companyId, string code, string nameEn, string nameAr,
        Guid? parentWarehouseId, bool isMain)
        : base(id)
    {
        CompanyId = companyId;
        Code = code;
        NameEn = nameEn;
        NameAr = nameAr;
        ParentWarehouseId = parentWarehouseId;
        IsMain = isMain;
        IsActive = true;
        AcceptsReservations = true;
    }

    // ── Identity ──────────────────────────────────────────────
    public Guid CompanyId { get; private set; }
    public string Code { get; private set; }              // WH-MAIN, WH-A1, WH-COLD
    public string NameEn { get; private set; }            // Main Warehouse
    public string NameAr { get; private set; }            // المخزن الرئيسي

    // ── Hierarchy ─────────────────────────────────────────────
    public Guid? ParentWarehouseId { get; private set; }  // null = Main warehouse
    public bool IsMain { get; private set; }              // Main or Sub

    // ── Settings ──────────────────────────────────────────────
    public bool IsActive { get; private set; }
    public bool AcceptsReservations { get; private set; } // هل يقبل حجوزات؟
    public bool IsDefaultForSales { get; private set; }   // المخزن الافتراضي للمبيعات
    public bool IsDefaultForPurchases { get; private set; }

    // ── Location ──────────────────────────────────────────────
    public string? Address { get; private set; }
    public string? Phone { get; private set; }
    public string? ManagerName { get; private set; }

    // ── Sub Warehouses (lazy — loaded via repository) ─────────
    public IReadOnlyCollection<Warehouse> SubWarehouses => _subWarehouses;

    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    // ── Factory: Main Warehouse ───────────────────────────────
    public static Result<Warehouse> CreateMain(
        Guid companyId, string code, string nameEn, string nameAr)
    {
        if (companyId == Guid.Empty)
            return Result.Failure<Warehouse>(new Error("Warehouse.InvalidCompany", "Company ID is required."));

        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<Warehouse>(new Error("Warehouse.InvalidCode", "Warehouse code is required."));

        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Failure<Warehouse>(new Error("Warehouse.InvalidName", "English name is required."));

        var warehouse = new Warehouse(
            Guid.NewGuid(), companyId, code.ToUpperInvariant(), nameEn, nameAr,
            parentWarehouseId: null, isMain: true);

        warehouse.RaiseDomainEvent(new WarehouseCreatedDomainEvent(warehouse.Id, companyId, nameEn, true));
        return Result.Success(warehouse);
    }

    // ── Factory: Sub Warehouse ────────────────────────────────
    public static Result<Warehouse> CreateSub(
        Guid companyId, string code, string nameEn, string nameAr, Guid parentWarehouseId)
    {
        if (parentWarehouseId == Guid.Empty)
            return Result.Failure<Warehouse>(new Error("Warehouse.InvalidParent", "Parent warehouse ID is required."));

        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<Warehouse>(new Error("Warehouse.InvalidCode", "Warehouse code is required."));

        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Failure<Warehouse>(new Error("Warehouse.InvalidName", "English name is required."));

        var warehouse = new Warehouse(
            Guid.NewGuid(), companyId, code.ToUpperInvariant(), nameEn, nameAr,
            parentWarehouseId, isMain: false);

        warehouse.RaiseDomainEvent(new WarehouseCreatedDomainEvent(warehouse.Id, companyId, nameEn, false));
        return Result.Success(warehouse);
    }

    // ── Domain Methods ────────────────────────────────────────
    public void SetAsDefaultForSales()
    {
        IsDefaultForSales = true;
        IsDefaultForPurchases = false;
    }

    public void SetAsDefaultForPurchases()
    {
        IsDefaultForPurchases = true;
        IsDefaultForSales = false;
    }

    public void UpdateSettings(bool acceptsReservations)
        => AcceptsReservations = acceptsReservations;

    public void UpdateLocation(string? address, string? phone, string? manager)
    {
        Address = address;
        Phone = phone;
        ManagerName = manager;
    }

    public Result Deactivate()
    {
        if (_subWarehouses.Any(s => s.IsActive))
            return Result.Failure(new Error("Warehouse.HasActiveSubWarehouses",
                "Cannot deactivate a warehouse with active sub-warehouses."));
        IsActive = false;
        return Result.Success();
    }

    public void Activate() => IsActive = true;
}
