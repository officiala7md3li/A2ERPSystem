using DomainDrivenERP.Domain.Entities.Localization;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface ILocalizationRepository
{
    Task<Language?> GetLanguageByCode(string languageCode, CancellationToken cancellationToken = default);
    Task<List<Language>> GetAllLanguages(bool activeOnly = true, CancellationToken cancellationToken = default);
    Task<Language> AddLanguage(Language language, CancellationToken cancellationToken = default);
    Task UpdateLanguage(Language language, CancellationToken cancellationToken = default);
    
    Task<List<LanguageResource>> GetResourcesByLanguage(string languageCode, CancellationToken cancellationToken = default);
    Task<List<LanguageResource>> GetResourcesByModule(string languageCode, string module, CancellationToken cancellationToken = default);
    Task AddOrUpdateResource(LanguageResource resource, CancellationToken cancellationToken = default);
    Task AddOrUpdateResources(IEnumerable<LanguageResource> resources, CancellationToken cancellationToken = default);
    
    Task<TranslationCache?> GetTranslationCache(string languageCode, CancellationToken cancellationToken = default);
    Task UpdateTranslationCache(TranslationCache cache, CancellationToken cancellationToken = default);
    
    Task<LocalizationSetting> GetSettings(CancellationToken cancellationToken = default);
    Task UpdateSettings(LocalizationSetting settings, CancellationToken cancellationToken = default);
    
    Task LogTranslationAudit(TranslationAudit audit, CancellationToken cancellationToken = default);
    Task LogTranslationImport(TranslationImport import, CancellationToken cancellationToken = default);
    Task LogTranslationExport(TranslationExport export, CancellationToken cancellationToken = default);
}