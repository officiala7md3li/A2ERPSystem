using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.Invoices.Commands.CancelInvoice;

public sealed record CancelInvoiceCommand(Guid InvoiceId, string Reason) : ICommand;
