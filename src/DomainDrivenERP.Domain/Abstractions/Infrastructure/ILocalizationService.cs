using DomainDrivenERP.Domain.Entities.Localization;

namespace DomainDrivenERP.Domain.Abstractions.Infrastructure;

public interface ILocalizationService
{
    Task<Dictionary<string, string>> GetTranslations(string languageCode, CancellationToken cancellationToken = default);
    Task ImportTranslationsFromJson(string languageCode, string jsonContent, string importedBy, CancellationToken cancellationToken = default);
    Task ExportTranslationsToJson(string languageCode, string exportedBy, CancellationToken cancellationToken = default);
    Task<Language?> GetCurrentLanguage(CancellationToken cancellationToken = default);
    Task<List<Language>> GetAvailableLanguages(CancellationToken cancellationToken = default);
    Task<string> Translate(string key, string languageCode, string defaultValue = "", CancellationToken cancellationToken = default);
    Task AddLanguage(Language language, CancellationToken cancellationToken = default);
}