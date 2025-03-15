using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Localization;

public class Language : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string NativeName { get; set; }
    public bool IsRTL { get; set; }
    public bool IsDefault { get; set; }
    public bool IsEnabled { get; set; }
    public string FlagIcon { get; set; }
    public string DateFormat { get; set; }
    public string TimeFormat { get; set; }
    public string NumberFormat { get; set; }
    public string CurrencyFormat { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation Properties
    public ICollection<LanguageResource> Resources { get; set; }

    public Language() : base()
    {
        Resources = new List<LanguageResource>();
        IsEnabled = true;
    }

    public Language(Guid id) : base(id)
    {
        Resources = new List<LanguageResource>();
        IsEnabled = true;
    }
}

public class LanguageResource : BaseEntity
{
    public Guid LanguageId { get; set; }
    public Language Language { get; set; }
    
    public string ResourceKey { get; set; }
    public string ResourceValue { get; set; }
    public string Module { get; set; }
    public string Group { get; set; }
    public string Description { get; set; }
    public bool IsHtml { get; set; }

    public LanguageResource() : base() { }
    public LanguageResource(Guid id) : base(id) { }
}

public class TranslationCache : BaseEntity
{
    public string CacheKey { get; set; }
    public string LanguageCode { get; set; }
    public string JsonContent { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsValid { get; set; }

    public TranslationCache() : base() { }
    public TranslationCache(Guid id) : base(id) { }
}

public class TranslationImport : BaseEntity
{
    public string FileName { get; set; }
    public string FileType { get; set; }
    public string LanguageCode { get; set; }
    public DateTime ImportDate { get; set; }
    public string ImportedBy { get; set; }
    public int TotalEntries { get; set; }
    public int SuccessfulEntries { get; set; }
    public int FailedEntries { get; set; }
    public string ImportStatus { get; set; }
    public string ErrorLog { get; set; }
}

public class TranslationExport : BaseEntity
{
    public string FileName { get; set; }
    public string FileType { get; set; }
    public string LanguageCodes { get; set; }
    public string Modules { get; set; }
    public DateTime ExportDate { get; set; }
    public string ExportedBy { get; set; }
    public int TotalEntries { get; set; }
    public string ExportStatus { get; set; }
    public string ErrorLog { get; set; }
}

public class LocalizationSetting : BaseEntity
{
    public string DefaultLanguageCode { get; set; }
    public bool AllowUserLanguageSelection { get; set; }
    public bool AutoDetectLanguage { get; set; }
    public bool ShowLanguageSelector { get; set; }
    public bool LoadAllLanguagesOnStartup { get; set; }
    public bool CacheTranslations { get; set; }
    public int CacheExpirationMinutes { get; set; }
    public string FallbackLanguageCode { get; set; }
    public bool UseResourceKeys { get; set; }
    public string ResourceFileFormat { get; set; }
    public string ResourceFilePath { get; set; }
}

public class TranslationAudit : BaseEntity
{
    public string LanguageCode { get; set; }
    public string ResourceKey { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
    public string ChangeType { get; set; }
    public DateTime ChangeDate { get; set; }
    public string ChangedBy { get; set; }
    public string Notes { get; set; }
}