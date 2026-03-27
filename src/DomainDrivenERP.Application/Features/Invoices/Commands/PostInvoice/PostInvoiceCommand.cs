using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.Invoices.Commands.PostInvoice;

public sealed record PostInvoiceCommand(Guid InvoiceId) : ICommand<PostInvoiceResult>;
