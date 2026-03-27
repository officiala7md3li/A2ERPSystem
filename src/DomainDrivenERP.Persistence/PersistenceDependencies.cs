using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Persistence.BackgroundJobs;
using DomainDrivenERP.Persistence.Caching;
using DomainDrivenERP.Persistence.Clients;
using DomainDrivenERP.Persistence.Configuration;
using DomainDrivenERP.Persistence.Data;
using DomainDrivenERP.Persistence.Idempotence;
using DomainDrivenERP.Persistence.Interceptors;
using DomainDrivenERP.Persistence.Repositories.Categories;
using DomainDrivenERP.Persistence.Repositories.Coa;
using DomainDrivenERP.Persistence.Repositories.Coas;
using DomainDrivenERP.Persistence.Repositories.Customers;
using DomainDrivenERP.Persistence.Repositories.Invoices;
using DomainDrivenERP.Persistence.Repositories.Journals;
using DomainDrivenERP.Persistence.Repositories.Orders;
using DomainDrivenERP.Persistence.Repositories.Products;
using DomainDrivenERP.Persistence.Repositories.Transactions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using DomainDrivenERP.Domain.Abstractions.Infrastructure;
using DomainDrivenERP.Infrastructure.Services;
using DomainDrivenERP.Persistence.Repositories.Localization;
using DomainDrivenERP.Persistence.Repositories.CustomerInvoices;
using DomainDrivenERP.Persistence.Repositories.VendorInvoices;
using DomainDrivenERP.Persistence.Repositories.CreditNotes;
using DomainDrivenERP.Persistence.Repositories.DebitNotes;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Caching;
using DomainDrivenERP.Application.Engines.SequenceEngine;
using DomainDrivenERP.Persistence.Repositories.Sequences;

namespace DomainDrivenERP.Persistence;

public static class PersistenceDependencies
{
    public static IServiceCollection AddPersistenceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // DB
        #region Interceptors
        // I Moved The Logic Inside UnitOfWork SaveChanges so I Remove the Interceptors DI 
        // services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>(); 
        // services.AddSingleton<UpdateAuditableEntitiesInterceptor>();
        #endregion
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            ConvertDomainEventsToOutboxMessagesInterceptor? interceptor = sp.GetService<ConvertDomainEventsToOutboxMessagesInterceptor>();
            string connectionString = configuration.GetConnectionString("SqlServer");
            options.UseSqlServer(connectionString)
                // ,a=>a.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery) //All the Query will be spliting
            .AddInterceptors(interceptor);
        });
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        // Quartz
        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));
            configure.AddJob<ProcessOutboxMessagesJob>(jobKey).AddTrigger(trigger =>
            trigger.ForJob(jobKey).WithSimpleSchedule(schedule =>
            schedule.WithIntervalInSeconds(10).RepeatForever()));
            configure.UseMicrosoftDependencyInjectionJobFactory();
        });
        services.AddQuartzHostedService();

        // Idempotency With MediatR Notification || Scrutor for Decorate
        services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));
        // Register repositories
        services.AddScoped<ILocalizationRepository, LocalizationRepository>();
        services.Decorate<ILocalizationRepository, CachedLocalizationRepository>();

        // Register services
        services.AddScoped<ILocalizationService, LocalizationService>();
        // I Create the Repositories with many ways [ Choose the way you want and comment the others ]
        // Repositories With EF
        services.AddScoped<ICustomerRespository, CustomerRespository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<ICoaRepository, CoaRepository>();
        services.AddScoped<IJournalRepository, JournalRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Repositories with Dapper ( Same Logic Like The EF ) || I Recreate some of them only for showing the Difference only
        services.AddScoped<ICustomerRespository, CustomerSqlRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceSqlRepository>();
        services.AddScoped<ICoaRepository, CoaSqlRepository>();
        services.AddScoped<IJournalRepository, JournalSqlRepository>();
        services.AddScoped<ITransactionRepository, TransactionSqlRepository>();

        // Repositories with GenericRepository and Specification Pattern || I Recreate some of them only for showing the Difference only
        services.AddScoped(typeof(IBaseRepositoryAsync<>), typeof(BaseRepositoryAsync<>));
        services.AddScoped<ICustomerRespository, CustomerSpecificationRepository>();
        services.AddScoped<IInvoiceRepository, InvoicesSpecificationRepository>();
        services.AddScoped<ICoaRepository, CoaSpecificationRepository>();
        services.AddScoped<IJournalRepository, JournalSpecificationRepository>();
        services.AddScoped<ITransactionRepository, TransactionSpecificationRepository>();


        // ── New Invoice Repositories ────────────────────────────────
        services.AddScoped<ICustomerInvoiceRepository, CustomerInvoiceRepository>();
        services.AddScoped<IVendorInvoiceRepository, VendorInvoiceRepository>();
        services.AddScoped<ICreditNoteRepository, CreditNoteRepository>();
        services.AddScoped<IDebitNoteRepository, DebitNoteRepository>();

        // ── Engines: SequenceStore ──────────────────────────────────
        services.AddScoped<ISequenceStore, SequenceStore>();

        // Configure caching based on settings
        ConfigureCaching(services, configuration);

        return services;
    }

    private static void ConfigureCaching(IServiceCollection services, IConfiguration configuration)
    {
        // Read caching settings from configuration with defaults
        var cachingSettings = new CachingSettings();
        var cachingSection = configuration.GetSection(CachingSettings.SectionName);

        // Simple configuration reading with defaults
        bool.TryParse(cachingSection["EnableCaching"], out bool enableCaching);
        bool.TryParse(cachingSection["EnableRedisCache"], out bool enableRedisCache);
        bool.TryParse(cachingSection["EnableMemoryCache"], out bool enableMemoryCache);
        int.TryParse(cachingSection["DefaultCacheExpirationMinutes"], out int cacheExpiration);

        cachingSettings.EnableCaching = enableCaching;
        cachingSettings.EnableRedisCache = enableRedisCache;
        cachingSettings.EnableMemoryCache = enableMemoryCache || true; // Default to true
        cachingSettings.DefaultCacheExpirationMinutes = cacheExpiration > 0 ? cacheExpiration : 30;

        // Read repository-specific settings
        var repoSection = cachingSection.GetSection("Repositories");
        bool.TryParse(repoSection["EnableCustomerCaching"], out bool enableCustomerCaching);
        bool.TryParse(repoSection["EnableProductCaching"], out bool enableProductCaching);
        bool.TryParse(repoSection["EnableOrderCaching"], out bool enableOrderCaching);
        bool.TryParse(repoSection["EnableInvoiceCaching"], out bool enableInvoiceCaching);
        bool.TryParse(repoSection["EnableCoaCaching"], out bool enableCoaCaching);
        bool.TryParse(repoSection["EnableJournalCaching"], out bool enableJournalCaching);
        bool.TryParse(repoSection["EnableTransactionCaching"], out bool enableTransactionCaching);
        bool.TryParse(repoSection["EnableCategoryCaching"], out bool enableCategoryCaching);

        cachingSettings.Repositories.EnableCustomerCaching = enableCustomerCaching;
        cachingSettings.Repositories.EnableProductCaching = enableProductCaching;
        cachingSettings.Repositories.EnableOrderCaching = enableOrderCaching;
        cachingSettings.Repositories.EnableInvoiceCaching = enableInvoiceCaching;
        cachingSettings.Repositories.EnableCoaCaching = enableCoaCaching;
        cachingSettings.Repositories.EnableJournalCaching = enableJournalCaching;
        cachingSettings.Repositories.EnableTransactionCaching = enableTransactionCaching;
        cachingSettings.Repositories.EnableCategoryCaching = enableCategoryCaching;

        // Register the settings for dependency injection
        services.AddSingleton(cachingSettings);

        // Always add memory cache as it's lightweight
        services.AddMemoryCache();

        // Configure distributed cache service based on settings
        if (cachingSettings.EnableCaching && cachingSettings.EnableRedisCache)
        {
            try
            {
                // Add Redis cache if enabled and connection string exists
                string? redisConnectionString = configuration.GetConnectionString("Redis");
                if (!string.IsNullOrEmpty(redisConnectionString))
                {
                    services.AddStackExchangeRedisCache(redisOptions =>
                    {
                        redisOptions.Configuration = redisConnectionString;
                    });
                }
                else
                {
                    // Fallback to distributed memory cache if Redis connection string is missing
                    services.AddDistributedMemoryCache();
                }
            }
            catch
            {
                // Fallback to distributed memory cache if Redis configuration fails
                services.AddDistributedMemoryCache();
            }
        }
        else
        {
            // Use distributed memory cache when Redis is disabled or caching is disabled
            services.AddDistributedMemoryCache();
        }

        // Always register cache service (it will use the configured IDistributedCache)
        services.AddSingleton<ICacheService, CacheService>();

        // Apply caching decorators only if caching is enabled
        if (cachingSettings.EnableCaching)
        {
            // Apply caching decorators based on individual repository settings
            if (cachingSettings.Repositories.EnableCustomerCaching)
            {
                services.Decorate<ICustomerRespository, CachedCustomerRepository>();
            }

            if (cachingSettings.Repositories.EnableInvoiceCaching)
            {
                services.Decorate<IInvoiceRepository, CachedInvoiceRepository>();
            }

            if (cachingSettings.Repositories.EnableCoaCaching)
            {
                services.Decorate<ICoaRepository, CachedCoaRepository>();
            }

            if (cachingSettings.Repositories.EnableJournalCaching)
            {
                services.Decorate<IJournalRepository, CachedJournalRepository>();
            }

            if (cachingSettings.Repositories.EnableTransactionCaching)
            {
                services.Decorate<ITransactionRepository, CachedTransactionRepository>();
            }

            if (cachingSettings.Repositories.EnableProductCaching)
            {
                services.Decorate<IProductRepository, CachedProductRepository>();
            }

            if (cachingSettings.Repositories.EnableOrderCaching)
            {
                services.Decorate<IOrderRepository, CachedOrderRepository>();
            }

            if (cachingSettings.Repositories.EnableCategoryCaching)
            {
                services.Decorate<ICategoryRepository, CachedCategoryRepository>();
            }


            // CustomerInvoice Caching
            services.Decorate<ICustomerInvoiceRepository, CachedCustomerInvoiceRepository>();
            services.Decorate<IVendorInvoiceRepository, CachedVendorInvoiceRepository>();
            services.Decorate<ICreditNoteRepository, CachedCreditNoteRepository>();
            services.Decorate<IDebitNoteRepository, CachedDebitNoteRepository>();

            // Always enable localization caching as it's essential
            services.Decorate<ILocalizationRepository, CachedLocalizationRepository>();
        }
    }
}
