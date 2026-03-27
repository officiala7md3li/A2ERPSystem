using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Queries.GetVendorInvoiceById;

public sealed record GetVendorInvoiceByIdQuery(Guid InvoiceId) : IQuery<VendorInvoiceDetailDto>;
