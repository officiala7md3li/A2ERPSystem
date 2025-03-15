using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Localization;
using DomainDrivenERP.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DomainDrivenERP.Persistence.Repositories.Localization;

internal sealed class LocalizationRepository : ILocalizationRepository
{
    private readonly ApplicationDbContext _context;
    private readonly string _translationsPath;

    public LocalizationRepository(ApplicationDbContext context)
    {
        _context = context;
        _translationsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Translations");
        Directory.CreateDirectory(_translationsPath);
    }

    public async Task<Language?> GetLanguageByCode(string languageCode, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Language>()
            .FirstOrDefaultAsync(x => x.Code == languageCode, cancellationToken);
    }

    public async Task<List<Language>> GetAllLanguages(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        IQueryable<Language> query = _context.Set<Language>().AsQueryable();
        
        if (activeOnly)
            query = query.Where(x => x.IsEnabled);

        return await query.OrderBy(x => x.DisplayOrder).ToListAsync(cancellationToken);
    }

    public async Task<Language> AddLanguage(Language language, CancellationToken cancellationToken = default)
    {
        await _context.Set<Language>().AddAsync(language, cancellationToken);
        
        string jsonPath = Path.Combine(_translationsPath, $"{language.Code}.json");
        if (!File.Exists(jsonPath))
        {
            Dictionary<string, object> emptyTranslations = new();
            string jsonContent = JsonConvert.SerializeObject(emptyTranslations, Formatting.Indented);
            await File.WriteAllTextAsync(jsonPath, jsonContent, cancellationToken);
        }
        
        return language;
    }

    public Task UpdateLanguage(Language language, CancellationToken cancellationToken = default)
    {
        _context.Set<Language>().Update(language);
        return Task.CompletedTask;
    }

    public async Task<List<LanguageResource>> GetResourcesByLanguage(string languageCode, CancellationToken cancellationToken = default)
    {
        string jsonPath = Path.Combine(_translationsPath, $"{languageCode}.json");
        if (!File.Exists(jsonPath))
        {
            return new List<LanguageResource>();
        }

        Language? language = await GetLanguageByCode(languageCode, cancellationToken);
        if (language == null) return new List<LanguageResource>();

        string jsonContent = await File.ReadAllTextAsync(jsonPath, cancellationToken);
        var jsonObject = JObject.Parse(jsonContent);
        List<LanguageResource> resources = new();

        foreach (JProperty section in jsonObject.Properties())
        {
            if (section.Value is not JObject sectionItems) continue;

            foreach (JProperty item in sectionItems.Properties())
            {
                if (item.Value is not JObject itemObj) continue;

                AddResourceFromJsonObject(resources, language.Id, section.Name, item.Name, itemObj);

                // Handle nested translations (like status items)
                foreach (JProperty prop in itemObj.Properties())
                {
                    if (prop.Name != "value" && prop.Name != "module" && prop.Name != "group"
                        && prop.Value is JObject nestedObj)
                    {
                        AddResourceFromJsonObject(resources, language.Id, section.Name, $"{item.Name}.{prop.Name}", nestedObj);
                    }
                }
            }
        }

        return resources;
    }

    private void AddResourceFromJsonObject(List<LanguageResource> resources, Guid languageId, string section, string key, JObject obj)
    {
        string? value = obj["value"]?.ToString();
        if (string.IsNullOrEmpty(value)) return;

        resources.Add(new LanguageResource
        {
            LanguageId = languageId,
            ResourceKey = $"{section}.{key}",
            ResourceValue = value,
            Module = obj["module"]?.ToString() ?? section,
            Group = obj["group"]?.ToString() ?? "General"
        });
    }

    public async Task<List<LanguageResource>> GetResourcesByModule(string languageCode, string module, CancellationToken cancellationToken = default)
    {
        List<LanguageResource> resources = await GetResourcesByLanguage(languageCode, cancellationToken);
        return resources.Where(r => r.Module == module).ToList();
    }

    public async Task AddOrUpdateResource(LanguageResource resource, CancellationToken cancellationToken = default)
    {
        Language? language = await GetLanguageByCode(resource.Language?.Code ?? "", cancellationToken);
        if (language == null) return;

        string jsonPath = Path.Combine(_translationsPath, $"{language.Code}.json");
        JObject jsonObject = File.Exists(jsonPath) 
            ? JObject.Parse(await File.ReadAllTextAsync(jsonPath, cancellationToken))
            : new JObject();

        string[] keyParts = resource.ResourceKey.Split('.');
        if (keyParts.Length < 2) return;

        string section = keyParts[0];
        string remainingKey = string.Join('.', keyParts.Skip(1));

        var sectionObj = (JObject)(jsonObject[section] ?? new JObject());
        jsonObject[section] = sectionObj;

        UpdateJsonObjectWithResource(sectionObj, remainingKey, resource);

        await File.WriteAllTextAsync(jsonPath, jsonObject.ToString(Formatting.Indented), cancellationToken);
    }

    private void UpdateJsonObjectWithResource(JObject parentObj, string key, LanguageResource resource)
    {
        string[] keyParts = key.Split('.');
        string itemKey = keyParts[0];

        if (keyParts.Length == 1)
        {
            parentObj[itemKey] = new JObject
            {
                ["value"] = resource.ResourceValue,
                ["module"] = resource.Module,
                ["group"] = resource.Group
            };
        }
        else
        {
            var nestedObj = (JObject)(parentObj[itemKey] ?? new JObject());
            parentObj[itemKey] = nestedObj;
            UpdateJsonObjectWithResource(nestedObj, string.Join('.', keyParts.Skip(1)), resource);
        }
    }

    public async Task AddOrUpdateResources(IEnumerable<LanguageResource> resources, CancellationToken cancellationToken = default)
    {
        foreach (LanguageResource resource in resources)
        {
            await AddOrUpdateResource(resource, cancellationToken);
        }
    }

    public async Task<TranslationCache?> GetTranslationCache(string languageCode, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TranslationCache>()
            .FirstOrDefaultAsync(x => x.LanguageCode == languageCode, cancellationToken);
    }

    public async Task UpdateTranslationCache(TranslationCache cache, CancellationToken cancellationToken = default)
    {
        TranslationCache? existing = await _context.Set<TranslationCache>()
            .FirstOrDefaultAsync(x => x.LanguageCode == cache.LanguageCode, cancellationToken);

        if (existing is null)
        {
            await _context.Set<TranslationCache>().AddAsync(cache, cancellationToken);
        }
        else
        {
            existing.JsonContent = cache.JsonContent;
            existing.LastUpdated = cache.LastUpdated;
            existing.IsValid = cache.IsValid;
        }
    }

    public async Task<LocalizationSetting> GetSettings(CancellationToken cancellationToken = default)
    {
        LocalizationSetting? settings = await _context.Set<LocalizationSetting>()
            .FirstOrDefaultAsync(cancellationToken);
            
        if (settings == null)
        {
            settings = new LocalizationSetting
            {
                DefaultLanguageCode = "en",
                FallbackLanguageCode = "en",
                ResourceFileFormat = "json",
                CacheExpirationMinutes = 60,
                LoadAllLanguagesOnStartup = false,
                UseResourceKeys = false
            };
            await _context.Set<LocalizationSetting>().AddAsync(settings, cancellationToken);
        }
        return settings;
    }

    public Task UpdateSettings(LocalizationSetting settings, CancellationToken cancellationToken = default)
    {
        _context.Set<LocalizationSetting>().Update(settings);
        return Task.CompletedTask;
    }

    public async Task LogTranslationAudit(TranslationAudit audit, CancellationToken cancellationToken = default)
    {
        await _context.Set<TranslationAudit>().AddAsync(audit, cancellationToken);
    }

    public async Task LogTranslationImport(TranslationImport import, CancellationToken cancellationToken = default)
    {
        await _context.Set<TranslationImport>().AddAsync(import, cancellationToken);
    }

    public async Task LogTranslationExport(TranslationExport export, CancellationToken cancellationToken = default)
    {
        await _context.Set<TranslationExport>().AddAsync(export, cancellationToken);
    }
}
