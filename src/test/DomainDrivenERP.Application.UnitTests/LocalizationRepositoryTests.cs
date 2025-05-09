using DomainDrivenERP.Domain.Entities.Localization;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DomainDrivenERP.Application.UnitTests;

public class LocalizationRepositoryTests
{
    private readonly Mock<ILocalizationRepository> _localizationRepositoryMock;

    public LocalizationRepositoryTests()
    {
        _localizationRepositoryMock = new Mock<ILocalizationRepository>();
    }

    [Fact]
    public async Task AddLanguage_ShouldCreateLanguage()
    {
        // Arrange
        var language = new Language
        {
            Code = "fr",
            Name = "French",
            NativeName = "Français",
            IsRTL = false,
            IsDefault = false,
            IsEnabled = true,
            FlagIcon = "🇫🇷",
            DateFormat = "dd/MM/yyyy",
            TimeFormat = "HH:mm",
            NumberFormat = "1,234.56",
            CurrencyFormat = "€1,234.56",
            DisplayOrder = 2
        };

        _localizationRepositoryMock.Setup(x => x.AddLanguage(language, default))
            .ReturnsAsync(language);

        _localizationRepositoryMock.Setup(x => x.GetLanguageByCode("fr", default))
            .ReturnsAsync(language);

        // Act
        Language addedLanguage = await _localizationRepositoryMock.Object.AddLanguage(language);

        // Assert
        addedLanguage.Should().NotBeNull();
        addedLanguage.Name.Should().Be("French");
        
        Language? retrievedLanguage = await _localizationRepositoryMock.Object.GetLanguageByCode("fr");
        retrievedLanguage.Should().NotBeNull();
        retrievedLanguage!.Name.Should().Be("French");
        
        _localizationRepositoryMock.Verify(x => x.AddLanguage(language, default), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateResource_ShouldAddTranslation()
    {
        // Arrange
        var language = new Language
        {
            Code = "fr",
            Name = "French",
            NativeName = "Français",
            IsRTL = false,
            IsDefault = false,
            IsEnabled = true,
            FlagIcon = "🇫🇷",
            DateFormat = "dd/MM/yyyy",
            TimeFormat = "HH:mm",
            NumberFormat = "1,234.56",
            CurrencyFormat = "€1,234.56",
            DisplayOrder = 2
        };

        var resource = new LanguageResource
        {
            LanguageId = language.Id,
            ResourceKey = "General.Hello",
            ResourceValue = "Bonjour",
            Module = "General",
            Group = "Greetings"
        };

        var resources = new List<LanguageResource> { resource };

        _localizationRepositoryMock.Setup(x => x.AddOrUpdateResource(resource, default))
            .Returns(Task.CompletedTask);

        _localizationRepositoryMock.Setup(x => x.GetResourcesByLanguage("fr", default))
            .ReturnsAsync(resources);

        // Act
        await _localizationRepositoryMock.Object.AddOrUpdateResource(resource);

        // Assert
        List<LanguageResource> addedResources = await _localizationRepositoryMock.Object.GetResourcesByLanguage("fr");
        addedResources.Should().NotBeEmpty();
        addedResources[0].ResourceValue.Should().Be("Bonjour");
        _localizationRepositoryMock.Verify(x => x.AddOrUpdateResource(resource, default), Times.Once);
    }

    [Fact]
    public async Task GetResourcesByLanguage_ShouldReturnResources()
    {
        // Arrange
        var resources = new List<LanguageResource>
        {
            new LanguageResource
            {
                LanguageId = Guid.NewGuid(),
                ResourceKey = "General.Hello",
                ResourceValue = "Bonjour",
                Module = "General",
                Group = "Greetings"
            },
            new LanguageResource
            {
                LanguageId = Guid.NewGuid(),
                ResourceKey = "General.Goodbye",
                ResourceValue = "Au revoir",
                Module = "General",
                Group = "Greetings"
            }
        };

        _localizationRepositoryMock.Setup(x => x.GetResourcesByLanguage("fr", default))
            .ReturnsAsync(resources);

        // Act
        List<LanguageResource> result = await _localizationRepositoryMock.Object.GetResourcesByLanguage("fr");

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(2);
        result[0].ResourceValue.Should().Be("Bonjour");
        result[1].ResourceValue.Should().Be("Au revoir");
    }

    [Fact]
    public async Task GetResourcesByModule_ShouldReturnResourcesForModule()
    {
        // Arrange
        var generalResources = new List<LanguageResource>
        {
            new LanguageResource
            {
                LanguageId = Guid.NewGuid(),
                ResourceKey = "General.Hello",
                ResourceValue = "Bonjour",
                Module = "General",
                Group = "Greetings"
            }
        };

        _localizationRepositoryMock.Setup(x => x.GetResourcesByModule("fr", "General", default))
            .ReturnsAsync(generalResources);

        // Act
        List<LanguageResource> result = await _localizationRepositoryMock.Object.GetResourcesByModule("fr", "General");

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result[0].Module.Should().Be("General");
        result[0].ResourceValue.Should().Be("Bonjour");
    }

    [Fact]
    public async Task GetLanguageByCode_ShouldReturnCorrectLanguage()
    {
        // Arrange
        var language = new Language
        {
            Code = "fr",
            Name = "French",
            NativeName = "Français",
            IsRTL = false,
            IsDefault = false,
            IsEnabled = true
        };

        _localizationRepositoryMock.Setup(x => x.GetLanguageByCode("fr", default))
            .ReturnsAsync(language);

        // Act
        Language? result = await _localizationRepositoryMock.Object.GetLanguageByCode("fr");

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be("fr");
        result.Name.Should().Be("French");
    }

    [Fact]
    public async Task GetLanguageByCode_ShouldReturnNull_WhenLanguageNotFound()
    {
        // Arrange
        _localizationRepositoryMock.Setup(x => x.GetLanguageByCode("nonexistent", default))
            .ReturnsAsync((Language?)null);

        // Act
        Language? result = await _localizationRepositoryMock.Object.GetLanguageByCode("nonexistent");

        // Assert
        result.Should().BeNull();
    }
}
