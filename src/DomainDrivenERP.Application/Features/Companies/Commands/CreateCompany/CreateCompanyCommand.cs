using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.Companies;
using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Application.Features.Companies.Commands.CreateCompany;

public record CreateCompanyCommand(
    string NameEn,
    string NameAr,
    string TaxRegistrationNumber,
    Guid BaseCurrencyId,
    TaxOrderSetting DefaultTaxOrder = TaxOrderSetting.AfterDiscount,
    StackingMode DefaultStackingMode = StackingMode.NoStacking,
    StockValuationMethod StockValuation = StockValuationMethod.Average) : ICommand<Company>;
