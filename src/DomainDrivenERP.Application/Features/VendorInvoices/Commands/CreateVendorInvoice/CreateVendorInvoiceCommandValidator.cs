using FluentValidation;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Commands.CreateVendorInvoice;

internal sealed class CreateVendorInvoiceCommandValidator : AbstractValidator<CreateVendorInvoiceCommand>
{
    public CreateVendorInvoiceCommandValidator()
    {
        RuleFor(x => x.VendorId).NotEmpty();
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.CurrencyId).NotEmpty();
        RuleFor(x => x.VendorInvoiceNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.InvoiceDate).NotEmpty().LessThanOrEqualTo(System.DateTime.UtcNow.AddDays(1));
    }
}
