using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Localization;

public class TranslationComparison : BaseEntity
{
    public string SourceLanguageCode { get; set; } = string.Empty;
    public string TargetLanguageCode { get; set; } = string.Empty;
    public DateTime ComparisonDate { get; set; } = DateTime.UtcNow;
    public string ComparedBy { get; set; } = string.Empty;
    public int TotalKeys { get; set; }
    public int MatchingKeys { get; set; }
    public int MissingKeys { get; set; }
    public int ExtraKeys { get; set; }
    public double CompletionPercentage { get; set; }
    public string ComparisonSummary { get; set; } = string.Empty;
    public string ComparisonDetails { get; set; } = string.Empty; // JSON serialized details
}

public class TranslationComparisonDetail
{
    public string ResourceKey { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string SourceValue { get; set; } = string.Empty;
    public string TargetValue { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Missing, Extra, Matching
}

public class TranslationComparisonResult
{
    public string SourceLanguageCode { get; set; } = string.Empty;
    public string TargetLanguageCode { get; set; } = string.Empty;
    public int TotalKeys { get; set; }
    public int MatchingKeys { get; set; }
    public int MissingKeys { get; set; }
    public int ExtraKeys { get; set; }
    public double CompletionPercentage { get; set; }
    public List<TranslationComparisonDetail> MissingTranslations { get; set; } = new();
    public List<TranslationComparisonDetail> ExtraTranslations { get; set; } = new();
    public Dictionary<string, ModuleComparisonStats> ModuleStats { get; set; } = new();
}

public class ModuleComparisonStats
{
    public string Module { get; set; } = string.Empty;
    public int TotalKeys { get; set; }
    public int MatchingKeys { get; set; }
    public int MissingKeys { get; set; }
    public int ExtraKeys { get; set; }
    public double CompletionPercentage { get; set; }
}