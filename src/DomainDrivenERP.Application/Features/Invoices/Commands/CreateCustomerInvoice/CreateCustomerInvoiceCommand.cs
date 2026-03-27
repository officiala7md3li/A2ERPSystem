using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Application.Features.Invoices.Commands.CreateCustomerInvoice;

public sealed record CreateCustomerInvoiceCommand(
    Guid CustomerId,
    Guid CompanyId,
    Guid CurrencyId,
    DateTime InvoiceDate,
    TaxOrderSetting TaxOrderSetting,
    StackingMode StackingMode,
    string? Notes) : ICommand<CreateCustomerInvoiceResult>;
