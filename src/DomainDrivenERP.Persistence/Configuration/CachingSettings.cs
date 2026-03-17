namespace DomainDrivenERP.Persistence.Configuration;

public class CachingSettings
{
    public const string SectionName = "CachingSettings";
    
    /// <summary>
    /// Enable or disable all caching functionality
    /// </summary>
    public bool EnableCaching { get; set; } = false;
    
    /// <summary>
    /// Enable Redis distributed cache (requires Redis server)
    /// </summary>
    public bool EnableRedisCache { get; set; } = false;
    
    /// <summary>
    /// Enable in-memory cache as fallback
    /// </summary>
    public bool EnableMemoryCache { get; set; } = true;
    
    /// <summary>
    /// Cache expiration time in minutes
    /// </summary>
    public int DefaultCacheExpirationMinutes { get; set; } = 30;
    
    /// <summary>
    /// Enable caching for specific repositories
    /// </summary>
    public RepositoryCachingSettings Repositories { get; set; } = new();
}

public class RepositoryCachingSettings
{
    public bool EnableCustomerCaching { get; set; } = true;
    public bool EnableProductCaching { get; set; } = true;
    public bool EnableOrderCaching { get; set; } = true;
    public bool EnableInvoiceCaching { get; set; } = true;
    public bool EnableCoaCaching { get; set; } = true;
    public bool EnableJournalCaching { get; set; } = true;
    public bool EnableTransactionCaching { get; set; } = true;
    public bool EnableCategoryCaching { get; set; } = true;
}
