# Phase 2 — Business Party & Pricing Entities

## Overview

Phase 2 builds entities that depend on Phase 1 foundations.
These entities complete the **commercial relationship model** —
who buys from us, who we buy from, how we price, and how we tax.

---

## Entities Built

### 1. `Vendor`
**Path:** `DomainDrivenERP.Domain/Entities/Vendors/Vendor.cs`

Represents a supplier the company purchases from. **Intentionally separate from Customer** — the same company can be both a vendor and a customer, but registered twice with different IDs.

| Property | Type | Description |
|----------|------|-------------|
| `CompanyId` | `Guid` | Scoped to a company (Phase 1) |
| `NameEn / NameAr` | `string` | Bilingual name |
| `TaxRegistrationNumber` | `string` | الرقم الضريبي — unique per company |
| `DefaultCurrencyId` | `Guid` | Currency (Phase 1) |
| `Type` | `VendorType` | Local / Foreign / Government |
| `CreditLimit` | `decimal?` | Maximum outstanding payables |
| `PaymentTermDays` | `int?` | صافي 30 يوم |
| `DefaultTaxGroupId` | `Guid?` | Default taxes on vendor invoices |

**`VendorType` Enum:**
```
Local      = مورد محلي
Foreign    = مورد أجنبي
Government = جهة حكومية
```

**Unique Constraint:** `TaxRegistrationNumber + CompanyId` — same tax number can exist in different companies (multi-tenant).

---

### 2. `TaxGroup` + `TaxGroupItem`
**Path:** `DomainDrivenERP.Domain/Entities/TaxGroups/TaxGroup.cs`

A named collection of `TaxDefinition` objects applied together. The system resolves the effective taxes for an `InvoiceLine` by loading the line's `TaxGroupId`.

```
TaxGroup: "Egyptian Standard Tax" (CompanyId: X)
├── TaxGroupItem → TaxDefinition(VAT 14%)      OverrideRate: null  → uses 14%
├── TaxGroupItem → TaxDefinition(Stamp 0.5%)   OverrideRate: 0.03  → uses 3% instead
└── TaxGroupItem → TaxDefinition(Table 5%)     OverrideRate: null  → uses 5%
```

**Key Feature — Rate Override:**
`TaxGroupItem.OverrideRate` allows the same tax code to have **different rates** in different groups. If `null`, the original rate from `TaxDefinition` is used.

**`IsDefault` Flag:**
One TaxGroup per company can be marked as default, used when a Product has no explicit TaxGroup assigned.

---

### 3. `DiscountGroup` + `DiscountRule` + `DiscountTier`
**Path:** `DomainDrivenERP.Domain/Entities/DiscountGroups/DiscountGroup.cs`

A named collection of discount rules. Fed to the `DiscountResolver` (Phase 1 Application Engine) along with the `StackingMode` from `Company` settings.

**Rule Types:**
| Type | Data | Example |
|------|------|---------|
| `FixedAmount` | Value | -50 EGP |
| `Percentage` | Value | 10% off |
| `Seasonal` | Value + StartDate + EndDate | Ramadan 20% |
| `Tiered` | TiersJson (serialized) | Qty 1–10: 0%, 11–50: 5%, 51+: 10% |

**`DiscountTier` (Value Object stored as JSON):**
```csharp
record DiscountTier(decimal MinQuantity, decimal? MaxQuantity, decimal DiscountPercent)
```

---

### 4. `PriceList` + `PriceListItem`
**Path:** `DomainDrivenERP.Domain/Entities/PriceLists/PriceList.cs`

Customer-specific pricing. When a customer creates an invoice, the system:
1. Checks if the customer has an assigned `PriceList` valid at the invoice date
2. For each line item, checks if a `PriceListItem` exists
3. Uses either `FixedPrice` (override the catalog price) or `DiscountPercent` (apply on top of catalog price)

**Validity:**
- `ValidFrom` / `ValidTo` — date-range scoped price lists
- `IsDefault` — fallback for customers without an explicit assignment
- `IsValidAt(DateTime date)` — domain method for validity check

**Customer Assignment:**
```csharp
priceList.AssignCustomer(customerId);    // add
priceList.UnassignCustomer(customerId); // remove
```

---

## How They Work Together

```
CustomerInvoice.Create(customerId: X, companyId: Y, ...)
    ↓
Orchestrator resolves pricing sources:
    1. PriceListRepository.GetForCustomerAsync(X, Y, invoiceDate)
       → returns assigned PriceList (if any)
    2. For each line:
       a. Product.TaxGroupId → load TaxGroup → build tax strategies
       b. Product.DiscountGroupId → load DiscountGroup → build discount candidates
       c. PriceList.GetPriceForItem(itemId) → override unit price (if exists)
       d. PriceList.GetDiscountForItem(itemId) → add PriceList discount candidate
    3. DiscountResolver.Resolve(candidates, Company.StackingMode)
    4. TaxEngine.CalculateTaxes(effectiveBase)
```

---

## Files in This Phase

```
Domain/
├── Entities/
│   ├── Vendors/
│   │   ├── Vendor.cs                   ← includes VendorType enum
│   │   └── DomainEvents/VendorCreatedDomainEvent.cs
│   ├── TaxGroups/
│   │   ├── TaxGroup.cs                 ← includes TaxGroupItem
│   │   └── DomainEvents/TaxGroupCreatedDomainEvent.cs
│   ├── DiscountGroups/
│   │   ├── DiscountGroup.cs            ← includes DiscountRule + DiscountTier
│   │   └── DomainEvents/DiscountGroupCreatedDomainEvent.cs
│   └── PriceLists/
│       ├── PriceList.cs                ← includes PriceListItem
│       └── DomainEvents/PriceListCreatedDomainEvent.cs
└── Abstractions/Persistence/Repositories/
    ├── IVendorRepository.cs
    ├── ITaxGroupRepository.cs
    ├── IDiscountGroupRepository.cs
    └── IPriceListRepository.cs

Persistence/
├── Configurations/
│   ├── VendorConfiguration.cs
│   ├── TaxGroupConfiguration.cs        ← includes TaxGroupItemConfiguration
│   ├── DiscountGroupConfiguration.cs   ← includes DiscountRuleConfiguration
│   └── PriceListConfiguration.cs       ← includes PriceListItemConfiguration
└── Constants/TableNames.cs             ← add 7 new constants
```

---

## Migration Command

```bash
dotnet ef migrations add Phase2_BusinessPartyAndPricing \
  --project DomainDrivenERP.Persistence \
  --startup-project DomainDrivenERP.API

dotnet ef database update \
  --project DomainDrivenERP.Persistence \
  --startup-project DomainDrivenERP.API
```

---

## Next: Phase 3

Phase 3 updates existing entities and adds inventory:
- `Product` — add `TaxGroupId`, `DiscountGroupId`, `UnitOfMeasureId`, `IsTaxExempt`
- `Warehouse` — Main + Sub warehouses with Company scope
- `PromoCode` — linked to `DiscountGroup` with usage tracking
