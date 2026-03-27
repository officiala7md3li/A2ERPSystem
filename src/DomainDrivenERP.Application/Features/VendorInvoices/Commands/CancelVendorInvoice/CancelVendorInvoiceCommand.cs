using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Commands.CancelVendorInvoice;

public sealed record CancelVendorInvoiceCommand(Guid InvoiceId, string Reason) : ICommand;
