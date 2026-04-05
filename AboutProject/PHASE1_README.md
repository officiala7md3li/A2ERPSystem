# Phase 1 — Foundation Entities

## Overview

Phase 1 builds the **zero-dependency foundation** that every other entity in A2ERP depends on.
Nothing in Phase 2+ can be built correctly without these four entities in place.

---

## Entities Built

### 1. `Currency`
**Path:** `DomainDrivenERP.Domain/Entities/Currencies/Currency.cs`

| Property | Type | Description |
|----------|------|-------------|
| `Code` | `string(3)` | ISO 4217 code (EGP, USD, EUR) |
| `NameEn` | `string` | English name |
| `NameAr` | `string` | Arabic name |
| `Symbol` | `string` | Display symbol (ج.م) |
| `IsBase` | `bool` | Is this the company's base currency? |
| `IsActive` | `bool` | Soft deactivation |

**Business Rules:**
- Code must be exactly 3 letters (ISO 4217)
- Base currency cannot be deactivated
- Only one active base currency per company

---

### 2. `UnitOfMeasure`
**Path:** `DomainDrivenERP.Domain/Entities/UnitOfMeasures/UnitOfMeasure.cs`

| Property | Type | Description |
|----------|------|-------------|
| `Code` | `string` | PCS, KG, L, M, BOX |
| `Type` | `UomType` | Quantity / Weight / Volume / Length / Area / Time |
| `ConversionFactor` | `decimal` | 1 BOX = 12 PCS → factor = 12 |
| `BaseUomId` | `Guid?` | Points to the base UoM (e.g. PCS is base for BOX) |

**Helper Methods:**
```csharp
decimal ToBase(decimal quantity)   // BOX → PCS
decimal FromBase(decimal quantity) // PCS → BOX
```

---

### 3. `Company` ⭐ Most Critical
**Path:** `DomainDrivenERP.Domain/Entities/Companies/Company.cs`

Central tenant configuration. **Every invoice, journal, sequence, and warehouse is scoped to a Company.**

| Setting Group | Properties |
|--------------|------------|
| Identity | `NameEn`, `NameAr`, `TaxRegistrationNumber`, `CommercialRegistrationNumber` |
| Financial | `BaseCurrencyId` |
| Invoice Pipeline | `DefaultTaxOrder`, `DefaultStackingMode` |
| Inventory | `StockValuation`, `ReserveStockOnSalesOrder`, `AllowNegativeStock` |
| Discount Caps | `MaxDiscountPercentPerLine`, `MaxDiscountAmountPerInvoice` |
| Cancellation | `AllowCancelWithReservation` |

**`StockValuationMethod` Enum:**
```
FIFO    = First In First Out
LIFO    = Last In First Out
Average = Weighted Average Cost
```

---

### 4. `TaxDefinition` + `TaxDependency`
**Path:** `DomainDrivenERP.Domain/Entities/TaxDefinitions/TaxDefinition.cs`

Represents a single tax type. Supports the **Dependency Graph** pattern where a tax is calculated based on a base amount **plus** other previously calculated taxes (compound/cascading).

| Property | Type | Description |
|----------|------|-------------|
| `Code` | `string` | "VAT", "ST01", "Tbl01", "W001" |
| `CalculationMethod` | `enum` | Percentage or FixedAmount |
| `Rate` | `decimal` | 0.14 for 14%, or 10.00 for fixed EGP 10 |
| `IsWithholding` | `bool` | W codes — subtract from total |
| `Dependencies` | `List<TaxDependency>` | Taxes this tax is computed on top of |

**Dependency Example (Egyptian VAT):**
```
VAT (V009) depends on:
  ├── Tbl01 (Table tax %)
  ├── ST01  (Stamp duty %)
  ├── ST02  (Stamp duty fixed)
  ├── Ent01 (Entertainment %)
  ├── RD01  (Resource development %)
  ├── SC01  (Service charges %)
  ├── Mn01  (Municipality fees %)
  ├── MI01  (Medical insurance %)
  └── OF01  (Other fees %)
```

**`TaxCalculationMethod` Enum:**
```
Percentage  = rate × (Base + Σ Dependencies)
FixedAmount = fixed value regardless of base
```

**`TaxAppliesTo` Enum:**
```
LineLevel    = applied per invoice line
InvoiceLevel = applied on invoice total
Both         = both levels
```

---

## Bug Fixes (also in this phase)

### Fix 1: `InvoiceLine` — Wrong Inheritance
```csharp
// ❌ Before
public sealed class InvoiceLine : AggregateRoot, IAuditableEntity

// ✅ After
public sealed class InvoiceLine : BaseEntity
```
**Why:** `InvoiceLine` is a Child Entity inside `CustomerInvoice` Aggregate.
It should never raise its own Domain Events or be tracked as an Aggregate Root.

### Fix 2: `NotImplementedException` removed
The `CreatedOnUtc` and `ModifiedOnUtc` properties that threw `NotImplementedException` were removed entirely — `InvoiceLine` doesn't need auditing as it's managed by its parent aggregate.

### Fix 3: `SequenceStore` — Single atomic SQL
```sql
-- ❌ Before: Two round-trips (MERGE + SELECT)
-- ✅ After: Single atomic MERGE with OUTPUT clause
MERGE SequenceCounters WITH (HOLDLOCK) AS target ...
OUTPUT INSERTED.CounterValue;
```
**Why:** Eliminates race condition between the increment and the read-back.

---

## Files in This Phase

```
Domain/
├── Entities/
│   ├── Currencies/
│   │   ├── Currency.cs
│   │   └── DomainEvents/CurrencyCreatedDomainEvent.cs
│   ├── Companies/
│   │   ├── Company.cs
│   │   └── DomainEvents/CompanyCreatedDomainEvent.cs
│   ├── UnitOfMeasures/
│   │   └── UnitOfMeasure.cs
│   └── TaxDefinitions/
│       ├── TaxDefinition.cs          ← includes TaxDependency + Enums
│       └── DomainEvents/TaxDefinitionCreatedDomainEvent.cs
└── Abstractions/Persistence/Repositories/
    ├── ICurrencyRepository.cs
    ├── IUnitOfMeasureRepository.cs
    ├── ICompanyRepository.cs
    └── ITaxDefinitionRepository.cs

Persistence/
├── Configurations/
│   ├── CurrencyConfiguration.cs
│   ├── UnitOfMeasureConfiguration.cs
│   ├── CompanyConfiguration.cs
│   └── TaxDefinitionConfiguration.cs    ← includes TaxDependencyConfiguration
└── Constants/TableNames.cs              ← add 5 new constants

Fixes/
├── InvoiceLine.cs                        ← BaseEntity only
└── SequenceStore.cs                      ← MERGE + OUTPUT
```

---

## Migration Command

```bash
dotnet ef migrations add Phase1_FoundationEntities \
  --project DomainDrivenERP.Persistence \
  --startup-project DomainDrivenERP.API

dotnet ef database update \
  --project DomainDrivenERP.Persistence \
  --startup-project DomainDrivenERP.API
```

---

## Next: Phase 2

Phase 2 builds entities that depend on Phase 1:
- `Vendor` ← needs Company + Currency
- `TaxGroup` ← needs TaxDefinition[] + Company
- `DiscountGroup` ← needs Company
- `PriceList` ← needs Company + Customer
