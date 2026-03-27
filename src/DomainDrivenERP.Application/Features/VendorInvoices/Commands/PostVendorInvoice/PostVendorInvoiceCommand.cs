using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Commands.PostVendorInvoice;

public sealed record PostVendorInvoiceCommand(Guid InvoiceId) : ICommand<PostVendorInvoiceResult>;

public sealed record PostVendorInvoiceResult(
    Guid InvoiceId,
    string SequenceNumber,
    decimal GrandTotal,
    string Status);
