using DomainDrivenERP.Domain.Entities.Localization;
using FluentAssertions;
using Xunit;

namespace DomainDrivenERP.Application.UnitTests.Features.Localization;

public class LocalizationEntityTests
{
    #region Language Entity

    [Fact]
    public void Language_Should_Initialize_With_Guid_Id()
    {
        // Act
        Guid id = Guid.NewGuid();
        var language = new Language(id)
        {
            Code = "en",
            Name = "English",
            NativeName = "English",
            IsRTL = false,
            IsDefault = true,
            IsEnabled = true
        };

        // Assert
        language.Id.Should().Be(id);
        language.Code.Should().Be("en");
        language.IsRTL.Should().BeFalse();
        language.Resources.Should().NotBeNull();
        language.Resources.Should().BeEmpty();
    }

    [Fact]
    public void Language_Default_Constructor_Should_Set_IsEnabled_True()
    {
        // Act
        var language = new Language();

        // Assert
        language.IsEnabled.Should().BeTrue();
        language.Resources.Should().NotBeNull();
    }

    [Fact]
    public void Language_Should_Support_RTL_Language()
    {
        // Act
        var arabic = new Language(Guid.NewGuid())
        {
            Code = "ar",
            Name = "Arabic",
            NativeName = "العربية",
            IsRTL = true,
            IsEnabled = true,
            FlagIcon = "🇸🇦",
            DateFormat = "dd/MM/yyyy",
            NumberFormat = "١٬٢٣٤٫٥٦"
        };

        // Assert
        arabic.IsRTL.Should().BeTrue();
        arabic.NativeName.Should().Be("العربية");
    }

    #endregion

    #region LanguageResource Entity

    [Fact]
    public void LanguageResource_Should_Store_Translation_Data()
    {
        // Arrange
        Guid languageId = Guid.NewGuid();

        // Act
        var resource = new LanguageResource(Guid.NewGuid())
        {
            LanguageId = languageId,
            ResourceKey = "General.Hello",
            ResourceValue = "مرحبا",
            Module = "General",
            Group = "Greetings",
            Description = "Greeting text",
            IsHtml = false
        };

        // Assert
        resource.LanguageId.Should().Be(languageId);
        resource.ResourceKey.Should().Be("General.Hello");
        resource.ResourceValue.Should().Be("مرحبا");
        resource.Module.Should().Be("General");
        resource.Group.Should().Be("Greetings");
        resource.IsHtml.Should().BeFalse();
    }

    [Fact]
    public void LanguageResource_Should_Support_HTML_Content()
    {
        // Act
        var resource = new LanguageResource
        {
            ResourceKey = "Email.WelcomeBody",
            ResourceValue = "<h1>Welcome!</h1><p>Thank you for joining</p>",
            IsHtml = true,
            Module = "Email"
        };

        // Assert
        resource.IsHtml.Should().BeTrue();
        resource.ResourceValue.Should().Contain("<h1>");
    }

    #endregion

    #region TranslationCache Entity

    [Fact]
    public void TranslationCache_Should_Track_Validity()
    {
        // Act
        var cache = new TranslationCache(Guid.NewGuid())
        {
            CacheKey = "translations_en",
            LanguageCode = "en",
            JsonContent = "{\"General.Hello\": \"Hello\"}",
            LastUpdated = DateTime.UtcNow,
            IsValid = true
        };

        // Assert
        cache.IsValid.Should().BeTrue();
        cache.LanguageCode.Should().Be("en");
        cache.CacheKey.Should().Be("translations_en");
    }

    [Fact]
    public void TranslationCache_Invalid_Should_Trigger_Refresh()
    {
        // Arrange
        var cache = new TranslationCache
        {
            CacheKey = "translations_ar",
            LanguageCode = "ar",
            JsonContent = "{}",
            IsValid = false
        };

        // Assert
        cache.IsValid.Should().BeFalse("Invalid cache should trigger database refresh");
    }

    #endregion

    #region LocalizationSetting Entity

    [Fact]
    public void LocalizationSetting_Should_Store_Configuration()
    {
        // Act
        var settings = new LocalizationSetting
        {
            DefaultLanguageCode = "en",
            FallbackLanguageCode = "en",
            AllowUserLanguageSelection = true,
            AutoDetectLanguage = true,
            ShowLanguageSelector = true,
            CacheTranslations = true,
            CacheExpirationMinutes = 60,
            UseResourceKeys = false,
            ResourceFileFormat = "json"
        };

        // Assert
        settings.DefaultLanguageCode.Should().Be("en");
        settings.FallbackLanguageCode.Should().Be("en");
        settings.AllowUserLanguageSelection.Should().BeTrue();
        settings.CacheTranslations.Should().BeTrue();
        settings.CacheExpirationMinutes.Should().Be(60);
    }

    #endregion

    #region TranslationImport Entity

    [Fact]
    public void TranslationImport_Should_Track_Import_Status()
    {
        // Act
        var import = new TranslationImport
        {
            FileName = "import_ar_20250316.json",
            FileType = "json",
            LanguageCode = "ar",
            ImportDate = DateTime.UtcNow,
            ImportedBy = "admin",
            TotalEntries = 150,
            SuccessfulEntries = 148,
            FailedEntries = 2,
            ImportStatus = "Completed"
        };

        // Assert
        import.TotalEntries.Should().Be(150);
        import.SuccessfulEntries.Should().Be(148);
        import.FailedEntries.Should().Be(2);
        import.ImportStatus.Should().Be("Completed");
        import.ImportedBy.Should().Be("admin");
    }

    #endregion

    #region TranslationExport Entity

    [Fact]
    public void TranslationExport_Should_Track_Export_Status()
    {
        // Act
        var export = new TranslationExport
        {
            FileName = "export_en_20250316.json",
            FileType = "json",
            LanguageCodes = "en",
            ExportDate = DateTime.UtcNow,
            ExportedBy = "admin",
            TotalEntries = 200,
            ExportStatus = "Completed"
        };

        // Assert
        export.TotalEntries.Should().Be(200);
        export.ExportStatus.Should().Be("Completed");
        export.LanguageCodes.Should().Be("en");
    }

    #endregion

    #region TranslationAudit Entity

    [Fact]
    public void TranslationAudit_Should_Track_Changes()
    {
        // Act
        var audit = new TranslationAudit
        {
            LanguageCode = "ar",
            ResourceKey = "General.Hello",
            OldValue = "مرحبا",
            NewValue = "أهلاً",
            ChangeType = "Update",
            ChangeDate = DateTime.UtcNow,
            ChangedBy = "translator1",
            Notes = "Updated to more common greeting"
        };

        // Assert
        audit.LanguageCode.Should().Be("ar");
        audit.OldValue.Should().Be("مرحبا");
        audit.NewValue.Should().Be("أهلاً");
        audit.ChangeType.Should().Be("Update");
        audit.ChangedBy.Should().Be("translator1");
    }

    #endregion
}
