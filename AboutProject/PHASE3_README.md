# Phase 3 — Product Enrichment, Warehouses & Promo Codes

## Overview

Phase 3 **enriches existing entities** with Phase 1+2 links,
adds physical inventory locations, and completes the promotional discounting story.

---

## Changes & Entities

### 1. `Product` — Updated (Breaking Change)
**Path:** `DomainDrivenERP.Domain/Entities/Products/Product.cs`

**New Fields Added:**

| Field | Type | Description |
|-------|------|-------------|
| `UnitOfMeasureId` | `Guid` | Required — Phase 1 |
| `TaxGroupId` | `Guid?` | Custom tax group — Phase 2 |
| `TaxGroupSource` | `TaxGroupSource` | Category / Custom / Exempt |
| `IsTaxExempt` | `bool` | Fully exempt from all taxes |
| `DiscountGroupId` | `Guid?` | Custom discount group — Phase 2 |
| `IsDiscountExempt` | `bool` | No discounts allowed |
| `MinimumSalePrice` | `decimal?` | Cannot sell below this |
| `MaximumDiscountPercent` | `decimal?` | Item-level discount cap |

**`TaxGroupSource` Enum & Resolution Logic:**
```
Exempt   → null (no tax applied)
Custom   → Product.TaxGroupId
Category → Category.DefaultTaxGroupId  ← inherited
```

**New Domain Method:**
```csharp
// Used by Orchestrator during invoice line resolution
Guid? GetEffectiveTaxGroupId(Guid? categoryDefaultTaxGroupId)
```

**Migration Note:**
`UnitOfMeasureId` is now **required** — you must provide a default value in the migration:
```csharp
migrationBuilder.AddColumn<Guid>("UnitOfMeasureId", "Products",
    nullable: false, defaultValue: Guid.Empty);
```

---

### 2. `Warehouse`
**Path:** `DomainDrivenERP.Domain/Entities/Warehouses/Warehouse.cs`

Two-level hierarchy: **Main Warehouse → Sub Warehouses**.
All inventory movements, reservations, and stock counts are scoped to a Warehouse.

| Property | Description |
|----------|-------------|
| `Code` | Unique per company (WH-MAIN, WH-COLD) |
| `ParentWarehouseId` | null = Main warehouse |
| `IsMain` | true = Main, false = Sub |
| `AcceptsReservations` | Can Sales Orders reserve stock here? |
| `IsDefaultForSales` | Auto-selected for new sales invoices |
| `IsDefaultForPurchases` | Auto-selected for new purchase invoices |

**Two Factory Methods:**
```csharp
Warehouse.CreateMain(companyId, code, nameEn, nameAr)
Warehouse.CreateSub(companyId, code, nameEn, nameAr, parentWarehouseId)
```

**Deactivation Rule:**
Cannot deactivate a warehouse that has active sub-warehouses.

**DB Constraint:**
`Code + CompanyId` — unique per company.
`OnDelete: Restrict` on self-referencing FK — prevents accidental cascade.

---

### 3. `PromoCode` + `PromoCodeUsage`
**Path:** `DomainDrivenERP.Domain/Entities/PromoCodes/PromoCode.cs`

Complete promotional code system with:
- Time-range validity (`StartDate` / `EndDate`)
- Global usage cap (`MaxUses`)
- Per-customer usage cap (`MaxUsesPerCustomer`)
- Minimum order amount gate
- Stacking control (`IsCombinable`)
- Optional link to a `DiscountGroup` for complex rules

**Validity Logic:**
```
ValidateUsage(customerId, orderAmount, now):
  1. IsActive?
  2. now >= StartDate (if set)?
  3. now <= EndDate (if set)?
  4. CurrentUses < MaxUses (if set)?
  5. orderAmount >= MinimumOrderAmount (if set)?
  6. Customer usage < MaxUsesPerCustomer (if set)?
```

**Concurrency Safety:**
```
Primary:  Redis INCR atomic counter
Fallback: DB MERGE WITH (HOLDLOCK) — same pattern as SequenceStore
Sync:     SyncUsageCounter(int redisCounter) — background job
```

**Auto-Deactivation:**
When `CurrentUses >= MaxUses`, the code is automatically deactivated on `RecordUsage()`.

**Domain Events:**
- `PromoCodeCreatedDomainEvent`
- `PromoCodeUsedDomainEvent` — triggers external notifications, reports

---

## How PromoCode Works in the Orchestrator

```
Invoice.Post() → Orchestrator
    ↓
1. ValidatePromoCode(code, customerId, orderTotal, now)
   → Redis: INCR "promo:{code}:uses" (atomic)
   → If Redis down: DB MERGE WITH HOLDLOCK
   → If validation fails: return error, DECR counter
   ↓
2. PromoCode.ComputeDiscount(orderTotal)
   → Percentage: total × (value / 100)
   → FixedAmount: min(value, total)  ← cannot exceed order total
   ↓
3. InvoiceLevelDiscount.Add(source: PromoCode, amount: X)
   ↓
4. PromoCode.RecordUsage(customerId, invoiceId, discountApplied)
   → Stored in PromoCodeUsages table
   → Raises PromoCodeUsedDomainEvent → Outbox
```

---

## Files in This Phase

```
Domain/
├── Entities/
│   ├── Products/
│   │   └── Product.cs                     ← UPDATED (replace existing)
│   ├── Warehouses/
│   │   ├── Warehouse.cs                   ← includes TaxGroupSource enum
│   │   └── DomainEvents/WarehouseCreatedDomainEvent.cs
│   └── PromoCodes/
│       ├── PromoCode.cs                   ← includes PromoCodeUsage + PromoDiscountType
│       └── DomainEvents/
│           ├── PromoCodeCreatedDomainEvent.cs
│           └── PromoCodeUsedDomainEvent.cs
└── Abstractions/Persistence/Repositories/
    ├── IWarehouseRepository.cs
    └── IPromoCodeRepository.cs

Persistence/
├── Configurations/
│   ├── WarehouseConfiguration.cs
│   ├── PromoCodeConfiguration.cs          ← includes PromoCodeUsageConfiguration
│   └── ProductConfiguration_Updated.cs   ← REPLACE existing ProductConfiguration.cs
└── Constants/TableNames.cs               ← add 3 new constants
```

---

## Migration

```bash
dotnet ef migrations add Phase3_ProductWarehousePromoCode \
  --project DomainDrivenERP.Persistence \
  --startup-project DomainDrivenERP.API
```

**Important:** Add a default value for `UnitOfMeasureId` in the migration:
```csharp
// In the migration Up() method:
migrationBuilder.AddColumn<Guid>(
    name: "UnitOfMeasureId",
    table: "Products",
    nullable: false,
    defaultValue: Guid.Empty); // Update existing rows after migration
```

---

## Full Entity Map (After Phase 3)

```
✅ Currency          (Phase 1)
✅ UnitOfMeasure     (Phase 1)
✅ Company           (Phase 1)
✅ TaxDefinition     (Phase 1)
✅ Vendor            (Phase 2)
✅ TaxGroup          (Phase 2)
✅ DiscountGroup     (Phase 2)
✅ PriceList         (Phase 2)
✅ Product (updated) (Phase 3)
✅ Warehouse         (Phase 3)
✅ PromoCode         (Phase 3)

⏳ Next → Phase 4: Invoicing Orchestrator
```

---

## Next: Phase 4 — Invoicing Orchestrator

The full pipeline connecting all entities:
```
Validation → Discount Resolution → Tax Calculation →
Hidden Discount → Sequence → Journals → Outbox Events
```
