# Guide: Adding a New Entity to DomainDrivenERP System

This guide outlines the step-by-step process for adding a new entity to the DomainDrivenERP system, following Domain-Driven Design principles and the existing architecture patterns.

## 1. Create the Entity in Domain Layer

### 1.1 Define the Entity Class

Create a new folder for your entity in `DomainDrivenERP.Domain/Entities/[EntityName]s/` and add the entity class:

```csharp
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Guards;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.[EntityName]s;
public class [EntityName] : AggregateRoot, IAuditableEntity
{
    private [EntityName]() { } // Required for EF Core
    
    private [EntityName](Guid id, string name, /* other properties */)
        : base(id)
    {
        Guard.Against.Null(id, nameof(id));
        Guard.Against.NullOrEmpty(name, nameof(name));
        // Validate other properties
        
        Name = name;
        // Set other properties
    }
    
    // Properties
    public string Name { get; private set; }
    // Add other properties
    
    // Audit properties (required by IAuditableEntity)
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    
    // Factory method
    public static Result<[EntityName]> Create(string name, /* other parameters */)
    {
        var id = Guid.NewGuid();
        
        // Validate inputs
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<[EntityName]>(DomainErrors.[EntityName]Errors.Invalid[EntityName]Name);
        }
        // Validate other parameters
        
        var entity = new [EntityName](id, name, /* other parameters */);
        
        // Optionally raise domain event
        entity.RaiseDomainEvent(new Create[EntityName]DomainEvent(entity.Id, name, /* other parameters */));
        
        return Result.Success(entity);
    }
    
    // Add methods for entity behavior
    public Result<[EntityName]> UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            return Result.Failure<[EntityName]>(DomainErrors.[EntityName]Errors.Invalid[EntityName]Name);
        }
        
        Name = newName;
        // Optionally raise domain event
        return Result.Success(this);
    }
}
```

### 1.2 Define Domain Events (Optional)

Create domain events in `DomainDrivenERP.Domain/Entities/[EntityName]s/DomainEvents/`:

```csharp
using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.[EntityName]s.DomainEvents;

public record Create[EntityName]DomainEvent(Guid Id, string Name, /* other properties */) : DomainEvent(Id);
```

### 1.3 Add Domain Errors

Add error definitions in `DomainDrivenERP.Domain/Errors/DomainErrors.cs`:

```csharp
public static class [EntityName]Errors
{
    public static readonly Error Invalid[EntityName]Id = new Error("[EntityName].InvalidId", "Invalid [EntityName] ID.");
    public static readonly Error Invalid[EntityName]Name = new Error("[EntityName].InvalidName", "Invalid [EntityName] name.");
    // Add other error definitions
}
```

### 1.4 Create Specifications (Optional)

Create specifications in `DomainDrivenERP.Domain/Entities/[EntityName]s/Specifications/`:

```csharp
using DomainDrivenERP.Domain.Shared.Specifications;

namespace DomainDrivenERP.Domain.Entities.[EntityName]s.Specifications;

public sealed class Get[EntityName]ByIdSpecification : Specification<[EntityName]>
{
    public Get[EntityName]ByIdSpecification(Guid id)
    {
        AddCriteria(entity => entity.Id == id);
    }
}
```

## 2. Define Repository Interface

Create a repository interface in `DomainDrivenERP.Domain/Abstractions/Persistence/Repositories/`:

```csharp
using DomainDrivenERP.Domain.Entities.[EntityName]s;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface I[EntityName]Repository
{
    Task AddAsync([EntityName] value, CancellationToken cancellationToken = default);
    Task<[EntityName]?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<[EntityName]?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<CustomList<[EntityName]>> Get[EntityName]sByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task UpdateAsync([EntityName] value, CancellationToken cancellationToken = default);
    // Add other repository methods as needed
}
```

## 3. Add Entity Configuration in Persistence Layer

### 3.1 Add Table Name

Add a constant for the table name in `DomainDrivenERP.Persistence/Constants/TableNames.cs`:

```csharp
internal const string [EntityName]s = nameof([EntityName]s);
```

### 3.2 Create Entity Configuration

Create a configuration class in `DomainDrivenERP.Persistence/Configurations/`:

```csharp
using DomainDrivenERP.Domain.Entities.[EntityName]s;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;

internal sealed class [EntityName]Configuration : IEntityTypeConfiguration<[EntityName]>
{
    public void Configure(EntityTypeBuilder<[EntityName]> builder)
    {
        builder.ToTable(TableNames.[EntityName]s);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired();
            
        // Configure other properties
        
        // Configure relationships if any
        // builder.HasMany(x => x.RelatedEntities)
        //     .WithOne()
        //     .HasForeignKey(x => x.[EntityName]Id);
    }
}
```

## 4. Implement Repository

### 4.1 Create Repository Implementation

Create a repository implementation in `DomainDrivenERP.Persistence/Repositories/[EntityName]s/`:

```csharp
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.[EntityName]s;
using DomainDrivenERP.Domain.Shared.Results;
using DomainDrivenERP.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenERP.Persistence.Repositories.[EntityName]s;

internal class [EntityName]Repository : I[EntityName]Repository
{
    private readonly ApplicationDbContext _context;

    public [EntityName]Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync([EntityName] entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<[EntityName]>().AddAsync(entity, cancellationToken);
    }

    public async Task<[EntityName]?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<[EntityName]>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<[EntityName]?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Set<[EntityName]>().FirstOrDefaultAsync(e => e.Name == name, cancellationToken);
    }

    public async Task<CustomList<[EntityName]>> Get[EntityName]sByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.Set<[EntityName]>()
            .Where(e => e.CreatedOnUtc.Date >= fromDate.Date && e.CreatedOnUtc.Date <= toDate.Date)
            .ToCustomListAsync();
    }

    public async Task UpdateAsync([EntityName] entity, CancellationToken cancellationToken = default)
    {
        _context.Set<[EntityName]>().Update(entity);
    }
}
```

### 4.2 Create Cached Repository Implementation

Create a cached repository implementation in `DomainDrivenERP.Persistence/Repositories/[EntityName]s/`:

```csharp
using DomainDrivenERP.Domain.Abstractions.Persistence.Caching;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.[EntityName]s;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Persistence.Repositories.[EntityName]s;

internal class Cached[EntityName]Repository : I[EntityName]Repository
{
    private readonly I[EntityName]Repository _decorated;
    private readonly ICacheService _cacheService;

    public Cached[EntityName]Repository(I[EntityName]Repository decorated, ICacheService cacheService)
    {
        _decorated = decorated;
        _cacheService = cacheService;
    }

    public async Task AddAsync([EntityName] value, CancellationToken cancellationToken = default)
    {
        await _decorated.AddAsync(value, cancellationToken);
        // Optionally clear cache
        // await Clear[EntityName]Cache(cancellationToken);
    }

    public async Task<[EntityName]?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        string key = $"[entityName]ById-{id}";
        return await _cacheService.GetOrSetAsync(key,
            async () => await _decorated.GetByIdAsync(id, cancellationToken),
            cancellationToken);
    }

    public async Task<[EntityName]?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        string key = $"[entityName]ByName-{name}";
        return await _cacheService.GetOrSetAsync(key,
            async () => await _decorated.GetByNameAsync(name, cancellationToken),
            cancellationToken);
    }

    public async Task<CustomList<[EntityName]>> Get[EntityName]sByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        string key = $"[entityName]sByDateRange-{fromDate:yyyy-MM-dd}-{toDate:yyyy-MM-dd}";
        return await _cacheService.GetOrSetAsync(key,
            async () => await _decorated.Get[EntityName]sByDateRangeAsync(fromDate, toDate, cancellationToken),
            cancellationToken);
    }

    public async Task UpdateAsync([EntityName] value, CancellationToken cancellationToken = default)
    {
        await _decorated.UpdateAsync(value, cancellationToken);
        // Optionally clear cache
        // await Clear[EntityName]Cache(cancellationToken);
    }

    /* Example of how to clear added/updated item cache
    private async Task Clear[EntityName]Cache(CancellationToken cancellationToken = default)
    {
        await _cacheService.RemoveAsync("[entityName]ById-*", cancellationToken);
        await _cacheService.RemoveAsync("[entityName]ByName-*", cancellationToken);
    }
    */
}
```

## 5. Register Dependencies

Update `DomainDrivenERP.Persistence/PersistenceDependencies.cs` to register your repositories:

```csharp
// Add these lines to the existing method
services.AddScoped<I[EntityName]Repository, [EntityName]Repository>();
services.Decorate<I[EntityName]Repository, Cached[EntityName]Repository>();
```

## 6. Create Application Features (CQRS)

### 6.1 Create Commands

Create command files in `DomainDrivenERP.Application/Features/[EntityName]s/Commands/Create[EntityName]/`:

#### Command:
```csharp
using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.[EntityName]s;

namespace DomainDrivenERP.Application.Features.[EntityName]s.Commands.Create[EntityName];

public record Create[EntityName]Command(string Name, /* other properties */) : ICommand<[EntityName]>;
```

#### Command Handler:
```csharp
using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.[EntityName]s;
using DomainDrivenE