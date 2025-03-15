using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace DomainDrivenERP.API.Extensions;

public static class LocalizationExtensions
{
    public static IServiceCollection AddCustomLocalization(this IServiceCollection services, IConfiguration configuration)
    {
        LocalizationSettings? localizationSettings = configuration.GetSection("LocalizationSettings").Get<LocalizationSettings>();

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.AddSupportedCultures("en", "ar")
                   .AddSupportedUICultures("en", "ar")
                   .SetDefaultCulture(localizationSettings?.DefaultLanguageCode ?? "en");

            options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider(),
                new AcceptLanguageHeaderRequestCultureProvider()
            };

            // Configure fallback behavior
            options.FallBackToParentCultures = true;
            options.FallBackToParentUICultures = true;
        });

        return services;
    }
}

public class LocalizationSettings
{
    public string DefaultLanguageCode { get; set; } = "en";
    public string FallbackLanguageCode { get; set; } = "en";
    public bool AllowUserLanguageSelection { get; set; } = true;
    public bool AutoDetectLanguage { get; set; } = true;
    public bool ShowLanguageSelector { get; set; } = true;
    public bool LoadAllLanguagesOnStartup { get; set; } = true;
    public bool CacheTranslations { get; set; } = true;
    public int CacheExpirationMinutes { get; set; } = 60;
    public string ResourceFilePath { get; set; } = "Resources/Translations";
}