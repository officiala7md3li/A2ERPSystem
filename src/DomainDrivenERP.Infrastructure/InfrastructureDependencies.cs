using DomainDrivenERP.Domain.Abstractions.Infrastructure;
using DomainDrivenERP.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DomainDrivenERP.Infrastructure;

public static class InfrastructureDependencies
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
    {
        services.AddScoped<ILocalizationService, LocalizationService>();
        return services;
    }
}