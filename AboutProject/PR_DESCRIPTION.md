# PR: Invoice System Redesign — Complete Domain, Persistence & Application Layer

## Overview

This PR introduces a **complete redesign of the Invoice system** across all architectural layers, replacing the previous simplified `Invoice` entity with a fully-featured, production-ready invoicing module that supports multiple document types, engine-ready pipeline settings, and proper accounting breakdowns.

---

## Purpose & Business Value

| Before | After |
|--------|-------|
| Single `Invoice` entity with flat decimal fields | 4 separate document types (CustomerInvoice, VendorInvoice, CreditNote, DebitNote) |
| Tax and discount as single decimals | Full `LineTaxBreakdown` and `LineDiscountBreakdown` per line |
| No invoice lines | `InvoiceLine` aggregate with Quantity + Money value objects |
| No pipeline settings | `TaxOrderSetting` + `StackingMode` per invoice |
| No hidden discount support | Post-tax `HiddenDiscount` on line and invoice level |
| No snapshot mechanism | Full `PipelineSnapshot` stored at posting time |
| `Customer` owned invoice creation | Invoice is its own Aggregate Root |

**Business impact:**
- Supports complex Egyptian tax regulations (VAT, Stamp, Table taxes) via `LineTaxBreakdown`
- Enables multi-source discount tracking (Category → Item → PriceList → Campaign → Loyalty → PromoCode)
- Accounting-safe: Snapshot pattern prevents retroactive settings changes from breaking Reversal Journals
- Outbox pattern ensures zero event loss on posting

---

## Technical Implementation

### Files Changed / Added

#### Domain Layer (`DomainDrivenERP.Domain`)
```
+ Enums/InvoiceStatus.cs          — 7 statuses (Draft → Paid / Cancelled)
+ Enums/DiscountSource.cs         — 7 sources with clear semantics
+ Enums/DiscountType.cs           — Fixed, Percentage, Tiered, Seasonal
+ Enums/StackingMode.cs           — NoStacking, FullStacking, Conditional
+ Enums/TaxOrderSetting.cs        — AfterDiscount (default) / BeforeDiscount
+ Enums/HiddenDiscountType.cs     — None, Percentage, FixedAmount
+ Enums/InvoiceType.cs            — 10 document types
+ Enums/CustomerTier.cs           — Standard, Gold, Premium, Diamond

+ ValueObjects/Money.cs           — Amount + Currency with arithmetic ops
+ ValueObjects/Quantity.cs        — Value + Unit (PCS, KG, L, M...)

+ Entities/Invoices/CustomerInvoice.cs         — Main Aggregate Root
+ Entities/Invoices/VendorInvoice.cs           — Vendor document
+ Entities/Invoices/CreditNote.cs              — Linked to original invoice
+ Entities/Invoices/DebitNote.cs               — Linked to original invoice
+ Entities/Invoices/InvoiceLine.cs             — Line with full breakdown
+ Entities/Invoices/LineTaxBreakdown.cs        — Per-line tax detail
+ Entities/Invoices/LineDiscountBreakdown.cs   — Per-line discount detail
+ Entities/Invoices/InvoiceLevelDiscount.cs    — Invoice-level discounts

+ Entities/Invoices/DomainEvents/ (10 events)
+ Abstractions/Persistence/Repositories/ (4 interfaces)
```

#### Persistence Layer (`DomainDrivenERP.Persistence`)
```
~ Constants/TableNames.cs         — Added 8 new table constants
~ Data/ApplicationDbContext.cs    — Added 8 new DbSets
+ Configurations/ (8 new EF configurations with indexes + decimal precision)
+ Repositories/CustomerInvoices/CustomerInvoiceRepository.cs
+ Repositories/CustomerInvoices/CachedCustomerInvoiceRepository.cs
~ PersistenceDependencies.cs      — Registered new repositories + caching
```

#### Application Layer (`DomainDrivenERP.Application`)
```
+ Features/Invoices/Commands/CreateCustomerInvoice/ (Command + Handler + Validator + Result)
+ Features/Invoices/Commands/AddLineToInvoice/ (Command + Handler + Validator + Result)
+ Features/Invoices/Commands/SubmitInvoice/ (Command + Handler)
+ Features/Invoices/Commands/PostInvoice/ (Command + Handler + Result)
+ Features/Invoices/Commands/CancelInvoice/ (Command + Handler + Validator)
+ Features/Invoices/Queries/GetCustomerInvoiceById/ (Query + Handler + DTOs)
+ Features/Invoices/Queries/GetCustomerInvoices/ (Query + Handler + DTOs)
```

#### Presentation Layer (`DomainDrivenERP.Presentation`)
```
+ Controllers/CustomerInvoicesController.cs  — 6 REST endpoints
```

---

## Invoice Status Flow

```
Draft ──► Pending ──► Posted ──► Approved ──► PartiallyPaid ──► Paid
                               └──────────────────────────────► Cancelled
```

- **Draft → Pending:** `SubmitInvoice` command (validates lines exist)
- **Pending → Posted:** `PostInvoice` command (Orchestrator runs here)
- **Any → Cancelled:** `CancelInvoice` command (triggers Reversal Journal when Posted)

---

## Calculation Pipeline (per Line)

```
UnitPrice × Quantity
    − Line Discounts     (Category | Item | PriceList | Campaign | Override)
    ± Tax                (Before or After discount — per Company Settings)
    − Hidden Discount    (Post-tax, user-defined, shown on printed invoice)
    ═══════════════════
    FinalLineTotal
```

---

## Impacted Areas

| Area | Impact | Notes |
|------|--------|-------|
| `Customer.cs` | Breaking change | Remove `CreateInvoice()` and `AddInvoice()` methods |
| `CustomerConfiguration.cs` | Breaking change | Remove `HasMany(x => x.Invoices)` |
| `IInvoiceRepository` | Deprecated | Replaced by `ICustomerInvoiceRepository` |
| DB Schema | New migration required | 8 new tables |
| Swagger | New endpoints visible | `/api/invoices/customer/*` |

---

## Migration Command

```bash
dotnet ef migrations add InvoiceRedesign \
  --project DomainDrivenERP.Persistence \
  --startup-project DomainDrivenERP.API

dotnet ef database update \
  --project DomainDrivenERP.Persistence \
  --startup-project DomainDrivenERP.API
```

---

## TODO (Next PRs)

- [ ] `PostInvoiceCommandHandler` — wire up full Invoicing Orchestrator
- [ ] `CancelInvoiceCommandHandler` — add Reversal Journal generation
- [ ] `VendorInvoice`, `CreditNote`, `DebitNote` — Application + Presentation layers
- [ ] Tax Engine — Separate Class Library with Dependency Graph
- [ ] Discount Engine — Multi-source resolution with Stacking
- [ ] Sequence Engine — Concurrency-safe with Redis + DB Lock

---

## Checklist

- [x] Domain entities follow Rich Domain Model
- [x] All FKs use `Guid`
- [x] Soft delete via `Cancelled` flag + `HasQueryFilter`
- [x] Decimal precision `decimal(18,4)` on all financial columns
- [x] Enums stored as `string` in DB (readable)
- [x] Outbox Pattern preserved via `AggregateRoot.RaiseDomainEvent`
- [x] Caching Decorator applied to CustomerInvoice repository
- [x] FluentValidation on all Commands
- [x] Repository Interfaces in Domain layer (dependency inversion)
- [ ] Unit tests for new domain entities
- [ ] Integration tests for invoice flow
