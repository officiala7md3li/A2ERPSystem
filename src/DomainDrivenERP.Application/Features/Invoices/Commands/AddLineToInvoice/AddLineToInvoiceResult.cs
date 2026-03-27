using System;

namespace DomainDrivenERP.Application.Features.Invoices.Commands.AddLineToInvoice;

public sealed record AddLineToInvoiceResult(
    Guid LineId,
    Guid InvoiceId,
    Guid ItemId,
    decimal SubTotal,
    decimal TotalDiscountAmount,
    decimal TotalTaxAmount,
    decimal FinalLineTotal);
