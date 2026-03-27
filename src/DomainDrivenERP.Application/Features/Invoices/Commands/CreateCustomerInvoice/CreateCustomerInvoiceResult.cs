using System;

namespace DomainDrivenERP.Application.Features.Invoices.Commands.CreateCustomerInvoice;

public sealed record CreateCustomerInvoiceResult(
    Guid InvoiceId,
    Guid CustomerId,
    DateTime InvoiceDate,
    string Status);
