using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.TaxDefinitions;

namespace DomainDrivenERP.Application.Features.TaxDefinitions.Commands.CreateTaxDefinition;

public record CreateTaxDefinitionCommand(
    string Code,
    string NameEn,
    string NameAr,
    TaxCalculationMethod CalculationMethod,
    decimal Rate,
    bool IsWithholding = false,
    TaxAppliesTo AppliesTo = TaxAppliesTo.LineLevel) : ICommand<TaxDefinition>;
