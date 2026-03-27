using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.Invoices.Commands.AddLineToInvoice;

public sealed record AddLineToInvoiceCommand(
    Guid InvoiceId,
    Guid ItemId,
    decimal Quantity,
    string QuantityUnit,
    decimal UnitPrice,
    string Currency,
    Guid? TaxGroupId = null,
    Guid? DiscountGroupId = null,
    int SortOrder = 0) : ICommand<AddLineToInvoiceResult>;
