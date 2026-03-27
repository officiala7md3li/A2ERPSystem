# Application Layer — `DomainDrivenERP.Application`

## Purpose

Orchestrates use cases. Connects the Domain to the outside world via **CQRS with MediatR**. Contains no business logic — only coordination, validation, and mapping.

## Architecture

```
DomainDrivenERP.Application/
├── Abstractions/
│   └── Messaging/
│       ├── ICommand.cs / ICommandHandler.cs
│       ├── IQuery.cs / IQueryHandler.cs
│       └── IDomainEventHandler.cs
├── Behaviors/                   # MediatR Pipeline Behaviors
│   ├── ValidationPipelineBehavior.cs
│   ├── LoggingPipelineBehavior.cs
│   └── AuthorizationBehaviour.cs
├── Features/
│   ├── Invoices/
│   │   ├── Commands/
│   │   │   ├── CreateCustomerInvoice/
│   │   │   ├── AddLineToInvoice/
│   │   │   ├── SubmitInvoice/
│   │   │   ├── PostInvoice/
│   │   │   └── CancelInvoice/
│   │   └── Queries/
│   │       ├── GetCustomerInvoiceById/
│   │       └── GetCustomerInvoices/
│   ├── Customers/
│   ├── Products/
│   ├── Orders/
│   ├── Journals/
│   ├── Authentication/
│   └── Roles/
└── Security/
    └── AuthorizeAttribute.cs
```

## CQRS Pattern

### Commands (write side)
```csharp
// 1. Define
public sealed record CreateCustomerInvoiceCommand(
    Guid CustomerId, Guid CompanyId, ...) : ICommand<CreateCustomerInvoiceResult>;

// 2. Validate (FluentValidation — runs automatically via Pipeline)
public sealed class CreateCustomerInvoiceCommandValidator
    : AbstractValidator<CreateCustomerInvoiceCommand>
{
    public CreateCustomerInvoiceCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.InvoiceDate).LessThanOrEqualTo(DateTime.UtcNow.AddDays(1));
    }
}

// 3. Handle
internal sealed class CreateCustomerInvoiceCommandHandler
    : ICommandHandler<CreateCustomerInvoiceCommand, CreateCustomerInvoiceResult>
{
    public async Task<Result<CreateCustomerInvoiceResult>> Handle(...)
    {
        // validate pre-conditions
        // call Domain factory
        // persist via repository
        // save via UnitOfWork
        // return Result
    }
}
```

### Queries (read side)
```csharp
public sealed record GetCustomerInvoiceByIdQuery(Guid InvoiceId)
    : IQuery<CustomerInvoiceDetailDto>;
```

## Pipeline Behaviors (run in order)

```
Request
  → LoggingPipelineBehavior  (logs request + elapsed time)
  → AuthorizationBehaviour   (checks [Authorize] attribute)
  → ValidationPipelineBehavior (runs FluentValidation)
  → Handler
  → Response
```

## Sending Commands & Queries (from Controller)

```csharp
// Command
var result = await Sender.Send(new CreateCustomerInvoiceCommand(...), ct);

// Query
var result = await Sender.Send(new GetCustomerInvoiceByIdQuery(id), ct);

// Always check Result
if (result.IsFailure) return BadRequest(result.Error);
return Ok(result.Value);
```
