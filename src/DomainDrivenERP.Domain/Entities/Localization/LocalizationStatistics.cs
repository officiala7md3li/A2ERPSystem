namespace DomainDrivenERP.Domain.Entities.Localization;

public class LocalizationStatistics
{
    public int TotalLanguages { get; set; }
    public int EnabledLanguages { get; set; }
    public int TotalTranslationKeys { get; set; }
    public Dictionary<string, int> TranslationsPerLanguage { get; set; } = new();
    public Dictionary<string, double> CompletionPercentages { get; set; } = new();
    public List<TopTranslationKey> MostUsedKeys { get; set; } = new();
    public List<RecentActivity> RecentActivities { get; set; } = new();
}

public class TopTranslationKey
{
    public string Key { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public string Module { get; set; } = string.Empty;
    public Dictionary<string, string> Translations { get; set; } = new();
}

public class RecentActivity
{
    public DateTime Timestamp { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
}

public class LanguageAnalytics
{
    public string LanguageCode { get; set; } = string.Empty;
    public int TotalKeys { get; set; }
    public int TranslatedKeys { get; set; }
    public double CompletionPercentage { get; set; }
    public List<ModuleCompletion> ModuleCompletions { get; set; } = new();
    public List<TranslationHistory> History { get; set; } = new();
    public Dictionary<string, int> KeysPerModule { get; set; } = new();
    public Dictionary<string, double> ModuleCompletionRates { get; set; } = new();
}

public class ModuleCompletion
{
    public string ModuleName { get; set; } = string.Empty;
    public int TotalKeys { get; set; }
    public int TranslatedKeys { get; set; }
    public double CompletionPercentage { get; set; }
}

public class TranslationHistory
{
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
}