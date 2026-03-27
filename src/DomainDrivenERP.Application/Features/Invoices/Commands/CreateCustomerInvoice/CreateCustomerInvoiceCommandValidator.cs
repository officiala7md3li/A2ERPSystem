using FluentValidation;

namespace DomainDrivenERP.Application.Features.Invoices.Commands.CreateCustomerInvoice;

internal sealed class CreateCustomerInvoiceCommandValidator : AbstractValidator<CreateCustomerInvoiceCommand>
{
    public CreateCustomerInvoiceCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.CurrencyId).NotEmpty();
        RuleFor(x => x.InvoiceDate).NotEmpty().LessThanOrEqualTo(System.DateTime.UtcNow.AddDays(1));
    }
}
