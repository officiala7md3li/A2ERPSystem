using DomainDrivenERP.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;

namespace DomainDrivenERP.Application.Features.Invoices.Queries.GetCustomerInvoices;

public sealed record GetCustomerInvoicesQuery(Guid CustomerId) : IQuery<IReadOnlyList<CustomerInvoiceListDto>>;

public sealed record CustomerInvoiceListDto(
    Guid Id,
    Guid CustomerId,
    string? SequenceNumber,
    DateTime InvoiceDate,
    string Status,
    decimal GrandTotal);
