using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.Invoices.Commands.SubmitInvoice;

public sealed record SubmitInvoiceCommand(Guid InvoiceId) : ICommand;
