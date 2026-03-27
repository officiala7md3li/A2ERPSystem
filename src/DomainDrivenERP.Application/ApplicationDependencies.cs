using DomainDrivenERP.Application.Behaviors;
using DomainDrivenERP.Application.Engines.DiscountEngine;
using DomainDrivenERP.Application.Engines.SequenceEngine;
using DomainDrivenERP.Application.Features.Customers.Queries.RetriveCustomer;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DomainDrivenERP.Application;

public static class ApplicationDependencies
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        //MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly));

        //MediatR PopleLines
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPiplineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));

        //Fluent Validation
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);

        // Mapping Profiles
        services.AddAutoMapper(typeof(RetriveCustomerMapping));

        // Engines
        services.AddSingleton<DiscountResolver>();
        services.AddScoped<ISequenceEngine, SequenceEngine>();

        return services;
    }
}
