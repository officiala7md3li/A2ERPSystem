using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Commands.SubmitVendorInvoice;

public sealed record SubmitVendorInvoiceCommand(Guid InvoiceId) : ICommand;
