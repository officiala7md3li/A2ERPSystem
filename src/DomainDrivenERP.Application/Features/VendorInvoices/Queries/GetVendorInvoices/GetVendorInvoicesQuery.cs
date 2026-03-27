using DomainDrivenERP.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Queries.GetVendorInvoices;

public sealed record GetVendorInvoicesQuery(Guid VendorId) : IQuery<IReadOnlyList<VendorInvoiceListDto>>;

public sealed record VendorInvoiceListDto(
    Guid Id,
    Guid VendorId,
    string? SequenceNumber,
    string VendorInvoiceNumber,
    DateTime InvoiceDate,
    string Status,
    decimal GrandTotal);
