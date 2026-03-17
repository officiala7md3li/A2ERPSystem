using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DomainDrivenERP.Persistence.Configuration;

public static class CachingConfigurationExtensions
{
    /// <summary>
    /// Extension method to easily configure caching settings
    /// </summary>
    public static IServiceCollection ConfigureCachingSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var cachingSettings = new CachingSettings();
        var cachingSection = configuration.GetSection(CachingSettings.SectionName);
        
        // Read configuration values with defaults
        bool.TryParse(cachingSection["EnableCaching"], out bool enableCaching);
        bool.TryParse(cachingSection["EnableRedisCache"], out bool enableRedisCache);
        bool.TryParse(cachingSection["EnableMemoryCache"], out bool enableMemoryCache);
        int.TryParse(cachingSection["DefaultCacheExpirationMinutes"], out int cacheExpiration);
        
        cachingSettings.EnableCaching = enableCaching;
        cachingSettings.EnableRedisCache = enableRedisCache;
        cachingSettings.EnableMemoryCache = enableMemoryCache || true;
        cachingSettings.DefaultCacheExpirationMinutes = cacheExpiration > 0 ? cacheExpiration : 30;
        
        // Read repository settings
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
        
        services.AddSingleton(cachingSettings);
        return services;
    }
    
    /// <summary>
    /// Get caching settings from configuration
    /// </summary>
    public static CachingSettings GetCachingSettings(this IConfiguration configuration)
    {
        var cachingSettings = new CachingSettings();
        var cachingSection = configuration.GetSection(CachingSettings.SectionName);
        
        bool.TryParse(cachingSection["EnableCaching"], out bool enableCaching);
        bool.TryParse(cachingSection["EnableRedisCache"], out bool enableRedisCache);
        bool.TryParse(cachingSection["EnableMemoryCache"], out bool enableMemoryCache);
        int.TryParse(cachingSection["DefaultCacheExpirationMinutes"], out int cacheExpiration);
        
        cachingSettings.EnableCaching = enableCaching;
        cachingSettings.EnableRedisCache = enableRedisCache;
        cachingSettings.EnableMemoryCache = enableMemoryCache || true;
        cachingSettings.DefaultCacheExpirationMinutes = cacheExpiration > 0 ? cacheExpiration : 30;
        
        return cachingSettings;
    }
}
