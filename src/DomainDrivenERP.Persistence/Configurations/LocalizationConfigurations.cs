using DomainDrivenERP.Domain.Entities.Localization;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;

internal sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    private static readonly Guid EnglishId = new("C3DAF939-7C28-4E4B-8E4F-3B23C2FB4C64");
    private static readonly Guid ArabicId = new("A4DAF939-7C28-4E4B-8E4F-3B23C2FB4C65");

    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable(TableNames.Languages);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.NativeName).IsRequired().HasMaxLength(50);
        builder.Property(x => x.FlagIcon).HasMaxLength(50);
        builder.Property(x => x.DateFormat).HasMaxLength(20);
        builder.Property(x => x.TimeFormat).HasMaxLength(20);
        builder.Property(x => x.NumberFormat).HasMaxLength(20);
        builder.Property(x => x.CurrencyFormat).HasMaxLength(20);

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.DisplayOrder);

        // Seed default languages
        builder.HasData(
            new Language(EnglishId)
            {
                Code = "en",
                Name = "English",
                NativeName = "English",
                IsRTL = false,
                IsDefault = true,
                IsEnabled = true,
                DateFormat = "MM/dd/yyyy",
                TimeFormat = "hh:mm tt",
                NumberFormat = "#,##0.00",
                CurrencyFormat = "$#,##0.00",
                DisplayOrder = 1,
                FlagIcon = "🇺🇸"
            },
            new Language(ArabicId)
            {
                Code = "ar",
                Name = "Arabic",
                NativeName = "العربية",
                IsRTL = true,
                IsDefault = false,
                IsEnabled = true,
                DateFormat = "dd/MM/yyyy",
                TimeFormat = "HH:mm",
                NumberFormat = "#,##0.00",
                CurrencyFormat = "#,##0.00 ج.م",
                DisplayOrder = 2,
                FlagIcon = "🇪🇬"
            }
        );
    }
}

internal sealed class LanguageResourceConfiguration : IEntityTypeConfiguration<LanguageResource>
{
    public void Configure(EntityTypeBuilder<LanguageResource> builder)
    {
        builder.ToTable(TableNames.LanguageResources);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ResourceKey).IsRequired().HasMaxLength(200);
        builder.Property(x => x.ResourceValue).IsRequired();
        builder.Property(x => x.Module).HasMaxLength(50);
        builder.Property(x => x.Group).HasMaxLength(50);
        builder.Property(x => x.Description).HasMaxLength(500);

        builder.HasOne(x => x.Language)
            .WithMany(x => x.Resources)
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.LanguageId, x.ResourceKey }).IsUnique();
        builder.HasIndex(x => new { x.LanguageId, x.Module });
    }
}

internal sealed class TranslationCacheConfiguration : IEntityTypeConfiguration<TranslationCache>
{
    public void Configure(EntityTypeBuilder<TranslationCache> builder)
    {
        builder.ToTable(TableNames.TranslationCaches);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CacheKey).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LanguageCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.JsonContent).IsRequired();

        builder.HasIndex(x => new { x.LanguageCode, x.CacheKey }).IsUnique();
    }
}

internal sealed class TranslationImportConfiguration : IEntityTypeConfiguration<TranslationImport>
{
    public void Configure(EntityTypeBuilder<TranslationImport> builder)
    {
        builder.ToTable(TableNames.TranslationImports);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(255);
        builder.Property(x => x.FileType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.LanguageCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.ImportedBy).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ImportStatus).IsRequired().HasMaxLength(50);

        builder.HasIndex(x => x.ImportDate);
    }
}

internal sealed class TranslationExportConfiguration : IEntityTypeConfiguration<TranslationExport>
{
    public void Configure(EntityTypeBuilder<TranslationExport> builder)
    {
        builder.ToTable(TableNames.TranslationExports);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(255);
        builder.Property(x => x.FileType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.LanguageCodes).IsRequired().HasMaxLength(500);
        builder.Property(x => x.ExportedBy).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ExportStatus).IsRequired().HasMaxLength(50);

        builder.HasIndex(x => x.ExportDate);
    }
}

internal sealed class LocalizationSettingConfiguration : IEntityTypeConfiguration<LocalizationSetting>
{
    public void Configure(EntityTypeBuilder<LocalizationSetting> builder)
    {
        builder.ToTable(TableNames.LocalizationSettings);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DefaultLanguageCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.FallbackLanguageCode).HasMaxLength(10);
        builder.Property(x => x.ResourceFileFormat).HasMaxLength(10);
        builder.Property(x => x.ResourceFilePath).HasMaxLength(255);
    }
}

internal sealed class TranslationAuditConfiguration : IEntityTypeConfiguration<TranslationAudit>
{
    public void Configure(EntityTypeBuilder<TranslationAudit> builder)
    {
        builder.ToTable(TableNames.TranslationAudits);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LanguageCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.ResourceKey).IsRequired().HasMaxLength(200);
        builder.Property(x => x.ChangeType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ChangedBy).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => x.ChangeDate);
        builder.HasIndex(x => new { x.LanguageCode, x.ResourceKey });
    }
}
