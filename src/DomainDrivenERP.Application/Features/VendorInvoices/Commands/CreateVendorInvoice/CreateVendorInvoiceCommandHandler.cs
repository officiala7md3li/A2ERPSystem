using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Commands.CreateVendorInvoice;

internal sealed class CreateVendorInvoiceCommandHandler
    : ICommandHandler<CreateVendorInvoiceCommand, CreateVendorInvoiceResult>
{
    private readonly IVendorInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVendorInvoiceCommandHandler(
        IVendorInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateVendorInvoiceResult>> Handle(
        CreateVendorInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        var invoiceResult = VendorInvoice.Create(
            request.VendorId,
            request.CompanyId,
            request.CurrencyId,
            request.InvoiceDate,
            request.VendorInvoiceNumber,
            request.TaxOrderSetting,
            request.StackingMode,
            request.Notes);

        if (invoiceResult.IsFailure)
        {
            return Result.Failure<CreateVendorInvoiceResult>(invoiceResult.Error);
        }

        var invoice = invoiceResult.Value;

        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateVendorInvoiceResult(
            invoice.Id,
            invoice.VendorId,
            invoice.InvoiceDate,
            invoice.Status.ToString(),
            invoice.VendorInvoiceNumber));
    }
}
