using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Enums;
using System;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Commands.CreateVendorInvoice;

public sealed record CreateVendorInvoiceCommand(
    Guid VendorId,
    Guid CompanyId,
    Guid CurrencyId,
    DateTime InvoiceDate,
    string VendorInvoiceNumber,
    TaxOrderSetting TaxOrderSetting,
    StackingMode StackingMode,
    string? Notes) : ICommand<CreateVendorInvoiceResult>;

public sealed record CreateVendorInvoiceResult(
    Guid InvoiceId,
    Guid VendorId,
    DateTime InvoiceDate,
    string Status,
    string VendorInvoiceNumber);
