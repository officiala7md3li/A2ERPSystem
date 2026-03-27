# Domain Layer — `DomainDrivenERP.Domain`

## Purpose

The **heart of the system**. Contains all business logic, rules, and invariants. Has zero dependencies on infrastructure, frameworks, or external libraries. Everything here is pure C#.

## Architecture

```
DomainDrivenERP.Domain/
├── Primitives/              # Base classes (AggregateRoot, BaseEntity, ValueObject, DomainEvent)
├── Entities/                # Rich domain aggregates
│   ├── Invoices/            # CustomerInvoice, VendorInvoice, CreditNote, DebitNote
│   │   ├── DomainEvents/    # Invoice lifecycle events
│   │   └── Specifications/  # Reusable query specs
│   ├── Customers/
│   ├── Products/
│   ├── Orders/
│   ├── Journals/
│   ├── COAs/
│   └── Transactions/
├── ValueObjects/            # Money, Quantity, Email, SKU, Price
├── Enums/                   # InvoiceStatus, DiscountSource, TaxOrderSetting...
├── Shared/
│   ├── Results/             # Result<T>, Error, ValidationResult
│   ├── Guards/              # Guard.Against.*
│   └── Specifications/      # ISpecification<T>, BaseSpecification<T>
├── Abstractions/
│   └── Persistence/
│       └── Repositories/    # Interfaces only — implemented in Persistence
└── Errors/                  # DomainErrors static class
```

## Key Design Decisions

### 1. Rich Domain Model
Entities own their behavior. No anemic models.
```csharp
// ✅ Domain method — validates + mutates + raises event
var result = invoice.Submit();

// ❌ Not this — logic outside entity
invoice.Status = InvoiceStatus.Pending;
```

### 2. Result Pattern (no exceptions for business rules)
```csharp
public static Result<CustomerInvoice> Create(...)
{
    if (customerId == Guid.Empty)
        return Result.Failure<CustomerInvoice>(
            new Error("CustomerInvoice.InvalidCustomer", "Customer ID required."));

    return Result.Success(new CustomerInvoice(...));
}
```

### 3. Value Objects (immutable, equality by value)
```csharp
var money = Money.Create(100.00m, "EGP").Value;
var total = money.Add(Money.Create(50m, "EGP").Value); // 150 EGP
```

### 4. Guard Clauses (fail fast on invalid state)
```csharp
Guard.Against.NullOrEmpty(invoiceSerial, nameof(invoiceSerial));
Guard.Against.NumberNegativeOrZero(amount, nameof(amount));
```

### 5. Domain Events (via Outbox Pattern)
```csharp
invoice.RaiseDomainEvent(new CustomerInvoicePostedDomainEvent(
    invoice.Id, invoice.CustomerId, invoice.CompanyId, invoice.GrandTotal));
// Stored in Outbox table → processed asynchronously by Quartz job
```

## Invoice Aggregate Invariants

| Rule | Enforcement |
|------|-------------|
| Cannot add lines to non-Draft invoice | `Status != Draft → return Failure` |
| Cannot post without Pending status | `Status != Pending → return Failure` |
| Cannot cancel a Paid invoice | `Status == Paid → return Failure` |
| Sequence number assigned once | `SequenceNumber != null → return Failure` |
| GrandTotal auto-recalculated on change | `RecalculateTotals()` called internally |

## Adding a New Entity

Follow `src/Guide-Adding-New-Entity.md` for the complete checklist. Summary:
1. Create entity class inheriting `AggregateRoot` or `BaseEntity`
2. Add factory method returning `Result<T>`
3. Define domain events in `DomainEvents/` subfolder
4. Add repository interface in `Abstractions/Persistence/Repositories/`
5. Add error definitions in `DomainErrors.cs`
