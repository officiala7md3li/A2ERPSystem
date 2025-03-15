using DomainDrivenERP.Domain.Abstractions.Persistence.Caching;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DomainDrivenERP.Persistence.Repositories.Localization;

internal sealed class CachedLocalizationRepository : ILocalizationRepository
{
    private readonly ILocalizationRepository _decorated;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedLocalizationRepository> _logger;
    private const string LanguagePrefix = "lang_";
    private const string ResourcePrefix = "res_";
    private const string SettingsKey = "localization_settings";

    public CachedLocalizationRepository(
        ILocalizationRepository decorated, 
        ICacheService cacheService,
        ILogger<CachedLocalizationRepository> logger)
    {
        _decorated = decorated;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Language?> GetLanguageByCode(string languageCode, CancellationToken cancellationToken = default)
    {
        string key = $"{LanguagePrefix}{languageCode}";
        return await _cacheService.GetOrSetAsync(key,
            () => _decorated.GetLanguageByCode(languageCode, cancellationToken),
            cancellationToken);
    }

    public async Task<List<Language>> GetAllLanguages(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        string key = $"{LanguagePrefix}all_{activeOnly}";
        return await _cacheService.GetOrSetAsync(key,
            () => _decorated.GetAllLanguages(activeOnly, cancellationToken),
            cancellationToken);
    }

    public async Task<Language> AddLanguage(Language language, CancellationToken cancellationToken = default)
    {
        Language result = await _decorated.AddLanguage(language, cancellationToken);
        await InvalidateLanguageCaches();
        return result;
    }

    public async Task UpdateLanguage(Language language, CancellationToken cancellationToken = default)
    {
        await _decorated.UpdateLanguage(language, cancellationToken);
        await InvalidateLanguageCaches();
    }

    public async Task<List<LanguageResource>> GetResourcesByLanguage(string languageCode, CancellationToken cancellationToken = default)
    {
        string key = $"{ResourcePrefix}{languageCode}";
        return await _cacheService.GetOrSetAsync(key,
            () => _decorated.GetResourcesByLanguage(languageCode, cancellationToken),
            cancellationToken);
    }

    public async Task<List<LanguageResource>> GetResourcesByModule(string languageCode, string module, CancellationToken cancellationToken = default)
    {
        string key = $"{ResourcePrefix}{languageCode}_{module}";
        return await _cacheService.GetOrSetAsync(key,
            () => _decorated.GetResourcesByModule(languageCode, module, cancellationToken),
            cancellationToken);
    }

    public async Task AddOrUpdateResource(LanguageResource resource, CancellationToken cancellationToken = default)
    {
        await _decorated.AddOrUpdateResource(resource, cancellationToken);
        await InvalidateResourceCaches(resource.Language.Code);
    }

    public async Task AddOrUpdateResources(IEnumerable<LanguageResource> resources, CancellationToken cancellationToken = default)
    {
        await _decorated.AddOrUpdateResources(resources, cancellationToken);
        foreach (LanguageResource resource in resources)
        {
            await InvalidateResourceCaches(resource.Language.Code);
        }
    }

    public async Task<TranslationCache?> GetTranslationCache(string languageCode, CancellationToken cancellationToken = default)
    {
        return await _decorated.GetTranslationCache(languageCode, cancellationToken);
    }

    public async Task UpdateTranslationCache(TranslationCache cache, CancellationToken cancellationToken = default)
    {
        await _decorated.UpdateTranslationCache(cache, cancellationToken);
    }

    public async Task<LocalizationSetting> GetSettings(CancellationToken cancellationToken = default)
    {
        return await _cacheService.GetOrSetAsync(SettingsKey,
            () => _decorated.GetSettings(cancellationToken),
            cancellationToken);
    }

    public async Task UpdateSettings(LocalizationSetting settings, CancellationToken cancellationToken = default)
    {
        await _decorated.UpdateSettings(settings, cancellationToken);
        await _cacheService.RemoveAsync(SettingsKey, cancellationToken);
    }

    public async Task LogTranslationAudit(TranslationAudit audit, CancellationToken cancellationToken = default)
    {
        await _decorated.LogTranslationAudit(audit, cancellationToken);
    }

    public async Task LogTranslationImport(TranslationImport import, CancellationToken cancellationToken = default)
    {
        await _decorated.LogTranslationImport(import, cancellationToken);
    }

    public async Task LogTranslationExport(TranslationExport export, CancellationToken cancellationToken = default)
    {
        await _decorated.LogTranslationExport(export, cancellationToken);
    }

    private async Task InvalidateLanguageCaches()
    {
        try
        {
            await _cacheService.RemoveByPrefixAsync(LanguagePrefix);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating language caches");
        }
    }

    private async Task InvalidateResourceCaches(string languageCode)
    {
        try
        {
            await _cacheService.RemoveByPrefixAsync($"{ResourcePrefix}{languageCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating resource caches for language {LanguageCode}", languageCode);
        }
    }
}