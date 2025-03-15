using System.Text;
using DomainDrivenERP.Domain.Abstractions.Infrastructure;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DomainDrivenERP.Infrastructure.Services;

public class LocalizationService : ILocalizationService
{
    private readonly ILocalizationRepository _localizationRepository;
    private readonly ILogger<LocalizationService> _logger;
    private readonly string _translationsPath;
    private LocalizationSetting? _settings;

    public LocalizationService(
        ILocalizationRepository localizationRepository,
        ILogger<LocalizationService> logger)
    {
        _localizationRepository = localizationRepository;
        _logger = logger;
        _translationsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Translations");
    }

    public async Task<Dictionary<string, string>> GetTranslations(string languageCode, CancellationToken cancellationToken = default)
    {
        // Check if language exists
        Language? language = await _localizationRepository.GetLanguageByCode(languageCode, cancellationToken);
        if (language == null)
        {
            // Try to load language from JSON file
            string jsonPath = Path.Combine(_translationsPath, $"{languageCode}.json");
            if (File.Exists(jsonPath))
            {
                // Create language first
                language = new Language(Guid.NewGuid())
                {
                    Code = languageCode,
                    Name = languageCode.ToUpperInvariant(),
                    NativeName = languageCode,
                    IsEnabled = true,
                    DisplayOrder = 999
                };
                
                await _localizationRepository.AddLanguage(language, cancellationToken);

                // Import translations from file
                string jsonContent = await File.ReadAllTextAsync(jsonPath, cancellationToken);
                await ImportTranslationsFromJson(languageCode, jsonContent, "system", cancellationToken);
            }
            else
            {
                throw new InvalidOperationException($"Language '{languageCode}' not found and no JSON file exists at {jsonPath}");
            }
        }

        // Check cache first
        TranslationCache? cache = await _localizationRepository.GetTranslationCache(languageCode, cancellationToken);
        if (cache?.IsValid == true)
        {
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(cache.JsonContent)
                    ?? new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing cached translations for language {LanguageCode}", languageCode);
            }
        }

        // Get from database
        List<LanguageResource> resources = await _localizationRepository.GetResourcesByLanguage(languageCode, cancellationToken);

        // If no translations in database, try to load from JSON file
        if (!resources.Any())
        {
            string jsonPath = Path.Combine(_translationsPath, $"{languageCode}.json");
            if (File.Exists(jsonPath))
            {
                try
                {
                    string jsonContent = await File.ReadAllTextAsync(jsonPath, cancellationToken);
                    await ImportTranslationsFromJson(languageCode, jsonContent, "system", cancellationToken);
                    resources = await _localizationRepository.GetResourcesByLanguage(languageCode, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading translations from file {JsonPath}", jsonPath);
                    throw;
                }
            }
            else
            {
                _logger.LogWarning("No translations found for language {LanguageCode} in database or file system", languageCode);
            }
        }

        var translations = resources.ToDictionary(
            r => r.ResourceKey,
            r => r.ResourceValue
        );

        await UpdateTranslationCache(languageCode, translations, cancellationToken);
        return translations;
    }

    public async Task ImportTranslationsFromJson(string languageCode, string jsonContent, string importedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            TranslationImport import = new()
            {
                FileName = $"import_{languageCode}_{DateTime.UtcNow:yyyyMMddHHmmss}.json",
                FileType = "json",
                LanguageCode = languageCode,
                ImportDate = DateTime.UtcNow,
                ImportedBy = importedBy,
                ImportStatus = "Processing"
            };

            Language? language = await _localizationRepository.GetLanguageByCode(languageCode, cancellationToken)
                ?? throw new InvalidOperationException($"Language '{languageCode}' not found");

            var jsonObject = JObject.Parse(jsonContent);
            List<LanguageResource> resources = new();

            foreach (JProperty section in jsonObject.Properties())
            {
                if (section.Value is JObject sectionItems)
                {
                    foreach (JProperty item in sectionItems.Properties())
                    {
                        if (item.Value is JObject itemObj)
                        {
                            // Handle regular translations
                            string value = itemObj["value"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(value))
                            {
                                resources.Add(new LanguageResource
                                {
                                    LanguageId = language.Id,
                                    Language = language,
                                    ResourceKey = $"{section.Name}.{item.Name}",
                                    ResourceValue = value,
                                    Module = itemObj["module"]?.ToString() ?? section.Name,
                                    Group = itemObj["group"]?.ToString() ?? "General"
                                });
                            }
                        }
                        else if (item.Value is JObject nestedObj)
                        {
                            // Handle nested objects (like status items)
                            foreach (JProperty nestedItem in nestedObj.Properties())
                            {
                                if (nestedItem.Value is JObject nestedItemObj)
                                {
                                    string value = nestedItemObj["value"]?.ToString() ?? "";
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        resources.Add(new LanguageResource
                                        {
                                            LanguageId = language.Id,
                                            Language = language,
                                            ResourceKey = $"{section.Name}.{item.Name}.{nestedItem.Name}",
                                            ResourceValue = value,
                                            Module = nestedItemObj["module"]?.ToString() ?? section.Name,
                                            Group = nestedItemObj["group"]?.ToString() ?? item.Name
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            await _localizationRepository.AddOrUpdateResources(resources, cancellationToken);

            import.TotalEntries = resources.Count;
            import.SuccessfulEntries = resources.Count;
            import.FailedEntries = 0;
            import.ImportStatus = "Completed";

            await _localizationRepository.LogTranslationImport(import, cancellationToken);
            await UpdateTranslationCache(languageCode, resources.ToDictionary(r => r.ResourceKey, r => r.ResourceValue), cancellationToken);
            
            _logger.LogInformation("Successfully imported {Count} translations for language {LanguageCode}", resources.Count, languageCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing translations for language {LanguageCode}: {Error}", languageCode, ex.ToString());
            throw;
        }
    }

    public async Task AddLanguage(Language language, CancellationToken cancellationToken = default)
    {
        Language? existing = await _localizationRepository.GetLanguageByCode(language.Code, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException($"Language '{language.Code}' already exists.");

        await _localizationRepository.AddLanguage(language, cancellationToken);

        // Try to import translations from JSON file if it exists
        string jsonPath = Path.Combine(_translationsPath, $"{language.Code}.json");
        if (File.Exists(jsonPath))
        {
            try
            {
                string jsonContent = await File.ReadAllTextAsync(jsonPath, cancellationToken);
                await ImportTranslationsFromJson(language.Code, jsonContent, "system", cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading translations from file {JsonPath}", jsonPath);
                throw;
            }
        }
    }

    public async Task ExportTranslationsToJson(string languageCode, string exportedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            Language? language = await _localizationRepository.GetLanguageByCode(languageCode, cancellationToken)
                ?? throw new InvalidOperationException($"Language '{languageCode}' not found.");

            TranslationExport export = new()
            {
                FileName = $"export_{languageCode}_{DateTime.UtcNow:yyyyMMddHHmmss}.json",
                FileType = "json",
                LanguageCodes = languageCode,
                ExportDate = DateTime.UtcNow,
                ExportedBy = exportedBy,
                ExportStatus = "Processing"
            };

            List<LanguageResource> resources = await _localizationRepository.GetResourcesByLanguage(languageCode, cancellationToken);
            Dictionary<string, Dictionary<string, object>> sections = new();

            // Group resources by section
            foreach (LanguageResource resource in resources)
            {
                string[] parts = resource.ResourceKey.Split('.');
                if (parts.Length < 2) continue;

                string section = parts[0];
                string key = string.Join('.', parts.Skip(1));

                if (!sections.ContainsKey(section))
                    sections[section] = new Dictionary<string, object>();

                sections[section][key] = new
                {
                    value = resource.ResourceValue,
                    module = resource.Module,
                    group = resource.Group,
                    description = resource.Description,
                    isHtml = resource.IsHtml
                };
            }

            string jsonContent = JsonConvert.SerializeObject(sections, Formatting.Indented);
            export.TotalEntries = resources.Count;
            export.ExportStatus = "Completed";

            await _localizationRepository.LogTranslationExport(export, cancellationToken);

            // Also save to file
            string jsonPath = Path.Combine(_translationsPath, $"{languageCode}.json");
            Directory.CreateDirectory(_translationsPath);
            await File.WriteAllTextAsync(jsonPath, jsonContent, cancellationToken);
            
            _logger.LogInformation("Successfully exported {Count} translations for language {LanguageCode}", resources.Count, languageCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting translations for language {LanguageCode}", languageCode);
            throw;
        }
    }

    public async Task<Language?> GetCurrentLanguage(CancellationToken cancellationToken = default)
    {
        LocalizationSetting settings = await GetSettings(cancellationToken);
        return await _localizationRepository.GetLanguageByCode(settings.DefaultLanguageCode, cancellationToken);
    }

    public async Task<List<Language>> GetAvailableLanguages(CancellationToken cancellationToken = default)
    {
        return await _localizationRepository.GetAllLanguages(true, cancellationToken);
    }

    public async Task<string> Translate(string key, string languageCode, string defaultValue = "", CancellationToken cancellationToken = default)
    {
        try
        {
            Dictionary<string, string> translations = await GetTranslations(languageCode, cancellationToken);
            if (translations.TryGetValue(key, out string? value))
                return value;

            LocalizationSetting settings = await GetSettings(cancellationToken);
            if (settings.FallbackLanguageCode != null && 
                settings.FallbackLanguageCode != languageCode)
            {
                translations = await GetTranslations(settings.FallbackLanguageCode, cancellationToken);
                if (translations.TryGetValue(key, out value))
                    return value;
            }

            return defaultValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating key {Key} for language {LanguageCode}", key, languageCode);
            return defaultValue;
        }
    }

    private async Task<LocalizationSetting> GetSettings(CancellationToken cancellationToken = default)
    {
        _settings ??= await _localizationRepository.GetSettings(cancellationToken);
        return _settings;
    }

    private async Task UpdateTranslationCache(string languageCode, Dictionary<string, string> translations, CancellationToken cancellationToken)
    {
        TranslationCache cache = new()
        {
            CacheKey = $"translations_{languageCode}",
            LanguageCode = languageCode,
            JsonContent = JsonConvert.SerializeObject(translations),
            LastUpdated = DateTime.UtcNow,
            IsValid = true
        };

        await _localizationRepository.UpdateTranslationCache(cache, cancellationToken);
    }
}