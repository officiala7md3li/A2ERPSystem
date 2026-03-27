# Persistence Layer — `DomainDrivenERP.Persistence`

## Purpose

Data access implementation. Provides EF Core (write side), Dapper (read side), Redis caching, and the Outbox pattern for reliable event publishing.

## Architecture

```
DomainDrivenERP.Persistence/
├── Data/
│   ├── ApplicationDbContext.cs      # Main EF DbContext
│   ├── BaseRepositoryAsync.cs       # Generic repository
│   └── UnitOfWork.cs                # Transaction boundary + Outbox conversion
├── Configurations/                  # IEntityTypeConfiguration<T> per entity
├── Repositories/                    # EF, Dapper, Spec, and Cached implementations
│   ├── CustomerInvoices/
│   │   ├── CustomerInvoiceRepository.cs
│   │   └── CachedCustomerInvoiceRepository.cs
│   └── ...
├── Caching/
│   └── CacheService.cs              # IDistributedCache wrapper (Redis/Memory)
├── BackgroundJobs/
│   └── ProcessOutboxMessagesJob.cs  # Quartz job: Outbox → MediatR publish
├── Outbox/
│   └── OutboxMessage.cs
├── Idempotence/
│   └── IdempotentDomainEventHandler.cs  # Prevents duplicate processing
└── Constants/
    └── TableNames.cs                # All table name constants
```

## Three Repository Strategies

The project demonstrates three interchangeable patterns. Pick one per entity:

### 1. EF Core Repository
```csharp
internal class CustomerInvoiceRepository : ICustomerInvoiceRepository
{
    public async Task<CustomerInvoice?> GetByIdWithLinesAsync(Guid id, CancellationToken ct)
        => await _context.CustomerInvoices
            .Include(x => x.Lines)
                .ThenInclude(l => l.TaxBreakdowns)
            .Include(x => x.Lines)
                .ThenInclude(l => l.DiscountBreakdowns)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
}
```

### 2. Dapper Repository (read optimization)
```csharp
// Raw SQL for performance-critical reads
const string sql = "SELECT * FROM CustomerInvoices WHERE Id = @Id";
return await _connection.QueryFirstOrDefaultAsync<CustomerInvoice>(sql, new { Id = id });
```

### 3. Specification Repository (complex queries)
```csharp
var spec = new GetCustomerInvoicesByStatusSpecification(status, companyId);
return await _repo.ListAsync(spec, ct);
```

## Caching Strategy (Decorator Pattern)

```csharp
// Registration in PersistenceDependencies.cs
services.AddScoped<ICustomerInvoiceRepository, CustomerInvoiceRepository>();
services.Decorate<ICustomerInvoiceRepository, CachedCustomerInvoiceRepository>();
```

Cache keys follow the pattern: `{entity}:{qualifier}:{value}`
- `customer-invoices:{customerId}` → 2 minute TTL
- Draft/In-progress invoices → **not cached** (change frequently)

### Redis TTL Guidelines

| Data | TTL |
|------|-----|
| Tax Rules | 24 hours |
| Company Config | 1 hour |
| License Data | 24 hours |
| Customer Invoice List | 2 minutes |
| Localization | 24 hours |
| Promo Codes | 1 hour |

## Outbox Pattern

Ensures domain events are **never lost**, even if the message broker is down:

```
1. Handler calls invoice.Post() → raises CustomerInvoicePostedDomainEvent
2. UnitOfWork.SaveChangesAsync() → converts Domain Events to OutboxMessages (same TX)
3. DB committed: invoice row + outbox row in one atomic transaction
4. Quartz job (every 10s): reads unprocessed OutboxMessages → publishes to MediatR
5. MediatR handlers → RabbitMQ, email, reports, etc.
```

## EF Configuration Conventions

All financial decimals use `decimal(18,4)`:
```csharp
builder.Property(x => x.GrandTotal).HasColumnType("decimal(18,4)");
```

Enums stored as strings for DB readability:
```csharp
builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
```

Soft delete via global query filter:
```csharp
builder.HasQueryFilter(x => !x.Cancelled);
```
