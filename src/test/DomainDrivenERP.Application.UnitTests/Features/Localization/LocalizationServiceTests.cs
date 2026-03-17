using DomainDrivenERP.Domain.Abstractions.Infrastructure;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Localization;
using DomainDrivenERP.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace DomainDrivenERP.Application.UnitTests.Features.Localization;

public class LocalizationServiceTests
{
    private readonly Mock<ILocalizationRepository> _repoMock;
    private readonly Mock<ILogger<LocalizationService>> _loggerMock;
    private readonly LocalizationService _sut;

    public LocalizationServiceTests()
    {
        _repoMock = new Mock<ILocalizationRepository>();
        _loggerMock = new Mock<ILogger<LocalizationService>>();
        _sut = new LocalizationService(_repoMock.Object, _loggerMock.Object);
    }

    #region Translate — Key Found in Primary Language

    [Fact]
    public async Task Translate_Should_Return_Value_When_Key_Exists()
    {
        // Arrange
        var language = new Language(Guid.NewGuid()) { Code = "ar", Name = "Arabic", NativeName = "العربية", IsEnabled = true };
        var resources = new List<LanguageResource>
        {
            new() { LanguageId = language.Id, ResourceKey = "General.Hello", ResourceValue = "مرحبا", Module = "General", Group = "Greetings" }
        };
        var translations = resources.ToDictionary(r => r.ResourceKey, r => r.ResourceValue);
        var cache = new TranslationCache { CacheKey = "translations_ar", LanguageCode = "ar", JsonContent = JsonConvert.SerializeObject(translations), IsValid = true };

        _repoMock.Setup(x => x.GetLanguageByCode("ar", It.IsAny<CancellationToken>())).ReturnsAsync(language);
        _repoMock.Setup(x => x.GetTranslationCache("ar", It.IsAny<CancellationToken>())).ReturnsAsync(cache);

        // Act
        string result = await _sut.Translate("General.Hello", "ar");

        // Assert
        result.Should().Be("مرحبا");
    }

    #endregion

    #region Translate — Key Not Found, Returns Default Value

    [Fact]
    public async Task Translate_Should_Return_DefaultValue_When_Key_Not_Found()
    {
        // Arrange
        var language = new Language(Guid.NewGuid()) { Code = "en", Name = "English", NativeName = "English", IsEnabled = true };
        var translations = new Dictionary<string, string> { { "General.Hello", "Hello" } };
        var cache = new TranslationCache { CacheKey = "translations_en", LanguageCode = "en", JsonContent = JsonConvert.SerializeObject(translations), IsValid = true };
        var settings = new LocalizationSetting { DefaultLanguageCode = "en", FallbackLanguageCode = null };

        _repoMock.Setup(x => x.GetLanguageByCode("en", It.IsAny<CancellationToken>())).ReturnsAsync(language);
        _repoMock.Setup(x => x.GetTranslationCache("en", It.IsAny<CancellationToken>())).ReturnsAsync(cache);
        _repoMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(settings);

        // Act
        string result = await _sut.Translate("General.NonExistent", "en", "FALLBACK_DEFAULT");

        // Assert
        result.Should().Be("FALLBACK_DEFAULT");
    }

    #endregion

    #region Translate — Fallback Language Lookup

    [Fact]
    public async Task Translate_Should_Fallback_To_FallbackLanguage_When_Key_Not_In_Primary()
    {
        // Arrange
        var frLanguage = new Language(Guid.NewGuid()) { Code = "fr", Name = "French", NativeName = "Français", IsEnabled = true };
        var enLanguage = new Language(Guid.NewGuid()) { Code = "en", Name = "English", NativeName = "English", IsEnabled = true };

        var frTranslations = new Dictionary<string, string> { { "General.Hello", "Bonjour" } };
        var enTranslations = new Dictionary<string, string>
        {
            { "General.Hello", "Hello" },
            { "General.Goodbye", "Goodbye" }
        };

        var frCache = new TranslationCache { CacheKey = "translations_fr", LanguageCode = "fr", JsonContent = JsonConvert.SerializeObject(frTranslations), IsValid = true };
        var enCache = new TranslationCache { CacheKey = "translations_en", LanguageCode = "en", JsonContent = JsonConvert.SerializeObject(enTranslations), IsValid = true };
        var settings = new LocalizationSetting { DefaultLanguageCode = "en", FallbackLanguageCode = "en" };

        _repoMock.Setup(x => x.GetLanguageByCode("fr", It.IsAny<CancellationToken>())).ReturnsAsync(frLanguage);
        _repoMock.Setup(x => x.GetLanguageByCode("en", It.IsAny<CancellationToken>())).ReturnsAsync(enLanguage);
        _repoMock.Setup(x => x.GetTranslationCache("fr", It.IsAny<CancellationToken>())).ReturnsAsync(frCache);
        _repoMock.Setup(x => x.GetTranslationCache("en", It.IsAny<CancellationToken>())).ReturnsAsync(enCache);
        _repoMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(settings);

        // Act — "General.Goodbye" does NOT exist in French, should fallback to English
        string result = await _sut.Translate("General.Goodbye", "fr");

        // Assert
        result.Should().Be("Goodbye");
    }

    #endregion

    #region GetTranslations — Cached Path

    [Fact]
    public async Task GetTranslations_Should_Return_Cached_Translations_When_Cache_Valid()
    {
        // Arrange
        var language = new Language(Guid.NewGuid()) { Code = "en", Name = "English", NativeName = "English", IsEnabled = true };
        var translations = new Dictionary<string, string>
        {
            { "General.Hello", "Hello" },
            { "General.Goodbye", "Goodbye" }
        };
        var cache = new TranslationCache
        {
            CacheKey = "translations_en",
            LanguageCode = "en",
            JsonContent = JsonConvert.SerializeObject(translations),
            IsValid = true
        };

        _repoMock.Setup(x => x.GetLanguageByCode("en", It.IsAny<CancellationToken>())).ReturnsAsync(language);
        _repoMock.Setup(x => x.GetTranslationCache("en", It.IsAny<CancellationToken>())).ReturnsAsync(cache);

        // Act
        Dictionary<string, string> result = await _sut.GetTranslations("en");

        // Assert
        result.Should().HaveCount(2);
        result["General.Hello"].Should().Be("Hello");
        result["General.Goodbye"].Should().Be("Goodbye");

        // Verify we didn't hit the database for resources (cache was used)
        _repoMock.Verify(x => x.GetResourcesByLanguage("en", It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region GetTranslations — Database Fallback When Cache Invalid

    [Fact]
    public async Task GetTranslations_Should_Query_Database_When_Cache_Invalid()
    {
        // Arrange
        var language = new Language(Guid.NewGuid()) { Code = "ar", Name = "Arabic", NativeName = "العربية", IsEnabled = true };
        var invalidCache = new TranslationCache
        {
            CacheKey = "translations_ar",
            LanguageCode = "ar",
            JsonContent = "{}",
            IsValid = false  // Cache is invalid
        };
        var resources = new List<LanguageResource>
        {
            new() { LanguageId = language.Id, ResourceKey = "General.Hello", ResourceValue = "مرحبا", Module = "General" },
            new() { LanguageId = language.Id, ResourceKey = "General.Goodbye", ResourceValue = "مع السلامة", Module = "General" }
        };

        _repoMock.Setup(x => x.GetLanguageByCode("ar", It.IsAny<CancellationToken>())).ReturnsAsync(language);
        _repoMock.Setup(x => x.GetTranslationCache("ar", It.IsAny<CancellationToken>())).ReturnsAsync(invalidCache);
        _repoMock.Setup(x => x.GetResourcesByLanguage("ar", It.IsAny<CancellationToken>())).ReturnsAsync(resources);

        // Act
        Dictionary<string, string> result = await _sut.GetTranslations("ar");

        // Assert
        result.Should().HaveCount(2);
        result["General.Hello"].Should().Be("مرحبا");
        result["General.Goodbye"].Should().Be("مع السلامة");

        // Verify database was queried
        _repoMock.Verify(x => x.GetResourcesByLanguage("ar", It.IsAny<CancellationToken>()), Times.Once);
        // Verify cache was updated
        _repoMock.Verify(x => x.UpdateTranslationCache(It.IsAny<TranslationCache>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetTranslations — Language Not Found

    [Fact]
    public async Task GetTranslations_Should_Throw_When_Language_Not_Found_And_No_File()
    {
        // Arrange
        _repoMock.Setup(x => x.GetLanguageByCode("xx", It.IsAny<CancellationToken>())).ReturnsAsync((Language?)null);

        // Act & Assert — should throw (exact exception type depends on which path fails first)
        await Assert.ThrowsAnyAsync<Exception>(
            () => _sut.GetTranslations("xx"));
    }

    #endregion

    #region AddLanguage — Duplicate Prevention

    [Fact]
    public async Task AddLanguage_Should_Throw_When_Language_Already_Exists()
    {
        // Arrange
        var existingLanguage = new Language(Guid.NewGuid()) { Code = "en", Name = "English", NativeName = "English", IsEnabled = true };
        var newLanguage = new Language(Guid.NewGuid()) { Code = "en", Name = "English Duplicate", NativeName = "English" };

        _repoMock.Setup(x => x.GetLanguageByCode("en", It.IsAny<CancellationToken>())).ReturnsAsync(existingLanguage);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.AddLanguage(newLanguage));
    }

    #endregion

    #region AddLanguage — New Language Success

    [Fact]
    public async Task AddLanguage_DuplicateCheck_Uses_LanguageCode()
    {
        // This test verifies the service correctly checks for duplicates
        // by calling GetLanguageByCode with the language's code.
        // When a duplicate IS found, it should throw InvalidOperationException.
        
        // Arrange — a language that "already exists"
        var existing = new Language(Guid.NewGuid()) { Code = "es", Name = "Spanish", NativeName = "Español", IsEnabled = true };
        var newLang = new Language(Guid.NewGuid()) { Code = "es", Name = "Spanish2", NativeName = "Español" };

        _repoMock.Setup(x => x.GetLanguageByCode("es", It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddLanguage(newLang));
        ex.Message.Should().Contain("already exists");

        // Verify AddLanguage was NOT called (duplicate was caught)
        _repoMock.Verify(x => x.AddLanguage(It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region GetAvailableLanguages

    [Fact]
    public async Task GetAvailableLanguages_Should_Return_Enabled_Languages()
    {
        // Arrange
        var languages = new List<Language>
        {
            new(Guid.NewGuid()) { Code = "en", Name = "English", NativeName = "English", IsEnabled = true },
            new(Guid.NewGuid()) { Code = "ar", Name = "Arabic", NativeName = "العربية", IsEnabled = true, IsRTL = true }
        };

        _repoMock.Setup(x => x.GetAllLanguages(true, It.IsAny<CancellationToken>())).ReturnsAsync(languages);

        // Act
        List<Language> result = await _sut.GetAvailableLanguages();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(l => l.Code == "en");
        result.Should().Contain(l => l.Code == "ar");
    }

    #endregion

    #region GetCurrentLanguage

    [Fact]
    public async Task GetCurrentLanguage_Should_Return_Default_Language_From_Settings()
    {
        // Arrange
        var settings = new LocalizationSetting { DefaultLanguageCode = "en" };
        var language = new Language(Guid.NewGuid()) { Code = "en", Name = "English", NativeName = "English", IsEnabled = true };

        _repoMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(settings);
        _repoMock.Setup(x => x.GetLanguageByCode("en", It.IsAny<CancellationToken>())).ReturnsAsync(language);

        // Act
        Language? result = await _sut.GetCurrentLanguage();

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be("en");
    }

    #endregion

    #region ImportTranslationsFromJson

    [Fact]
    public async Task ImportTranslationsFromJson_Should_Parse_And_Store_Resources()
    {
        // Arrange
        var language = new Language(Guid.NewGuid()) { Code = "ar", Name = "Arabic", NativeName = "العربية", IsEnabled = true };
        string jsonContent = @"{
            ""General"": {
                ""Hello"": { ""value"": ""مرحبا"", ""module"": ""General"", ""group"": ""Greetings"" },
                ""Goodbye"": { ""value"": ""مع السلامة"", ""module"": ""General"", ""group"": ""Greetings"" }
            }
        }";

        _repoMock.Setup(x => x.GetLanguageByCode("ar", It.IsAny<CancellationToken>())).ReturnsAsync(language);

        // Act
        await _sut.ImportTranslationsFromJson("ar", jsonContent, "admin");

        // Assert
        _repoMock.Verify(x => x.AddOrUpdateResources(
            It.Is<IEnumerable<LanguageResource>>(r => r.Count() == 2),
            It.IsAny<CancellationToken>()), Times.Once);

        _repoMock.Verify(x => x.LogTranslationImport(
            It.Is<TranslationImport>(i =>
                i.LanguageCode == "ar" &&
                i.ImportStatus == "Completed" &&
                i.TotalEntries == 2 &&
                i.ImportedBy == "admin"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ImportTranslationsFromJson_Should_Throw_When_Language_Not_Found()
    {
        // Arrange
        _repoMock.Setup(x => x.GetLanguageByCode("xx", It.IsAny<CancellationToken>())).ReturnsAsync((Language?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.ImportTranslationsFromJson("xx", "{}", "admin"));
    }

    #endregion

    #region ExportTranslationsToJson

    [Fact]
    public async Task ExportTranslationsToJson_Should_Throw_When_Language_Not_Found()
    {
        // Arrange
        _repoMock.Setup(x => x.GetLanguageByCode("xx", It.IsAny<CancellationToken>())).ReturnsAsync((Language?)null);

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(
            () => _sut.ExportTranslationsToJson("xx", "admin"));
    }

    #endregion
}
