using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.Invoices.Queries.GetCustomerInvoiceById;

public sealed record GetCustomerInvoiceByIdQuery(Guid InvoiceId) : IQuery<CustomerInvoiceDetailDto>;
