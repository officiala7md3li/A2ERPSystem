using System;

namespace DomainDrivenERP.Application.Features.Invoices.Commands.PostInvoice;

public sealed record PostInvoiceResult(
    Guid InvoiceId,
    string SequenceNumber,
    decimal GrandTotal,
    string Status);
