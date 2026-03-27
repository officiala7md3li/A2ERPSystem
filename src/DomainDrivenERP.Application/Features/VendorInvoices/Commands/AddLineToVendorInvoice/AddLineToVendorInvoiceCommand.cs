using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Commands.AddLineToVendorInvoice;

public sealed record AddLineToVendorInvoiceCommand(
    Guid InvoiceId,
    Guid ItemId,
    decimal Quantity,
    string QuantityUnit,
    decimal UnitPrice,
    string Currency,
    Guid? TaxGroupId = null,
    Guid? DiscountGroupId = null,
    int SortOrder = 0) : ICommand<AddLineToVendorInvoiceResult>;

public sealed record AddLineToVendorInvoiceResult(
    Guid LineId,
    Guid InvoiceId,
    Guid ItemId,
    decimal SubTotal,
    decimal TotalDiscountAmount,
    decimal TotalTaxAmount,
    decimal FinalLineTotal);
