using DomainDrivenERP.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace DomainDrivenERP.Application.UnitTests.Helpers;

public static class InMemoryDbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static void Destroy(ApplicationDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}