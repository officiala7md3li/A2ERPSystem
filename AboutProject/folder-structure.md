# A2ERP — Complete Folder Structure

```
A2ERPSystem/
├── src/
│   ├── DomainDrivenERP.sln
│   ├── docker-compose.yml
│   ├── docker-compose.dcproj
│   │
│   ├── DomainDrivenERP.Domain/              ← DOMAIN LAYER
│   │   ├── Primitives/
│   │   │   ├── AggregateRoot.cs
│   │   │   ├── BaseEntity.cs
│   │   │   ├── ValueObject.cs
│   │   │   ├── DomainEvent.cs
│   │   │   └── IAuditableEntity.cs
│   │   ├── Entities/
│   │   │   ├── Invoices/
│   │   │   │   ├── CustomerInvoice.cs
│   │   │   │   ├── VendorInvoice.cs
│   │   │   │   ├── CreditNote.cs
│   │   │   │   ├── DebitNote.cs
│   │   │   │   ├── InvoiceLine.cs
│   │   │   │   ├── LineTaxBreakdown.cs
│   │   │   │   ├── LineDiscountBreakdown.cs
│   │   │   │   ├── InvoiceLevelDiscount.cs
│   │   │   │   └── DomainEvents/
│   │   │   ├── Customers/
│   │   │   ├── Products/
│   │   │   ├── Orders/
│   │   │   ├── Journals/
│   │   │   ├── COAs/
│   │   │   └── Transactions/
│   │   ├── ValueObjects/
│   │   │   ├── Money.cs              ← NEW
│   │   │   ├── Quantity.cs           ← NEW
│   │   │   ├── Email.cs
│   │   │   ├── Price.cs
│   │   │   └── SKU.cs
│   │   ├── Enums/
│   │   │   ├── InvoiceStatus.cs      ← UPDATED (7 states)
│   │   │   ├── DiscountSource.cs     ← NEW
│   │   │   ├── DiscountType.cs       ← NEW
│   │   │   ├── StackingMode.cs       ← NEW
│   │   │   ├── TaxOrderSetting.cs    ← NEW
│   │   │   ├── HiddenDiscountType.cs ← NEW
│   │   │   ├── InvoiceType.cs        ← NEW
│   │   │   ├── CustomerTier.cs       ← NEW
│   │   │   └── OrderStatus.cs
│   │   ├── Shared/
│   │   │   ├── Results/              # Result<T>, Error, ValidationResult
│   │   │   ├── Guards/               # Guard.Against.*
│   │   │   └── Specifications/       # ISpecification<T>
│   │   ├── Abstractions/
│   │   │   └── Persistence/
│   │   │       └── Repositories/     # All IXxxRepository interfaces
│   │   └── Errors/
│   │       └── DomainErrors.cs
│   │
│   ├── DomainDrivenERP.Application/         ← APPLICATION LAYER
│   │   ├── Abstractions/Messaging/          # ICommand, IQuery, IHandler interfaces
│   │   ├── Behaviors/                       # MediatR Pipeline Behaviors
│   │   ├── Features/
│   │   │   ├── Invoices/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── CreateCustomerInvoice/
│   │   │   │   │   ├── AddLineToInvoice/
│   │   │   │   │   ├── SubmitInvoice/
│   │   │   │   │   ├── PostInvoice/
│   │   │   │   │   └── CancelInvoice/
│   │   │   │   └── Queries/
│   │   │   │       ├── GetCustomerInvoiceById/
│   │   │   │       └── GetCustomerInvoices/
│   │   │   ├── Customers/
│   │   │   ├── Products/
│   │   │   ├── Orders/
│   │   │   ├── Journals/
│   │   │   ├── Authentication/
│   │   │   └── Roles/
│   │   └── Security/
│   │
│   ├── DomainDrivenERP.Persistence/         ← PERSISTENCE LAYER
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs      ← UPDATED (8 new DbSets)
│   │   │   ├── UnitOfWork.cs
│   │   │   └── BaseRepositoryAsync.cs
│   │   ├── Configurations/                  ← 8 NEW EF configs
│   │   ├── Repositories/
│   │   │   ├── CustomerInvoices/            ← NEW
│   │   │   │   ├── CustomerInvoiceRepository.cs
│   │   │   │   └── CachedCustomerInvoiceRepository.cs
│   │   │   └── (other repositories...)
│   │   ├── BackgroundJobs/
│   │   │   └── ProcessOutboxMessagesJob.cs
│   │   ├── Caching/
│   │   │   └── CacheService.cs
│   │   ├── Constants/
│   │   │   └── TableNames.cs                ← UPDATED
│   │   └── PersistenceDependencies.cs       ← UPDATED
│   │
│   ├── DomainDrivenERP.Infrastructure/      ← INFRASTRUCTURE LAYER
│   │   └── Services/
│   │       ├── EmailService.cs
│   │       └── LocalizationService.cs
│   │
│   ├── DomainDrivenERP.Identity/            ← IDENTITY & AUTH
│   │   ├── Services/                        # Auth, Identity, Role services
│   │   ├── Filters/                         # Permission authorization
│   │   └── Migrations/                      # Identity DB migrations
│   │
│   ├── DomainDrivenERP.Presentation/        ← PRESENTATION LAYER
│   │   ├── Controllers/
│   │   │   ├── CustomerInvoicesController.cs ← NEW
│   │   │   ├── CustomersController.cs
│   │   │   ├── ProductsController.cs
│   │   │   └── (others...)
│   │   └── Base/
│   │       └── AppControllerBase.cs
│   │
│   ├── DomainDrivenERP.API/                 ← ENTRY POINT
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── Dockerfile
│   │   └── Resources/Translations/          # ar.json, en.json
│   │
│   ├── test/
│   │   └── DomainDrivenERP.Application.UnitTests/
│   │
│   └── benchmark/
│       └── DomainDrivenERP.RepositoriesPerformance/
│
├── .env.example                             ← NEW
├── .gitignore
└── README.md                                ← NEW
```
