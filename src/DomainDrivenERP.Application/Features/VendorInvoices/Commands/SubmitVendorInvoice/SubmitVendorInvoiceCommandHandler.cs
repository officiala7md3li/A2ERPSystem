using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Commands.SubmitVendorInvoice;

internal sealed class SubmitVendorInvoiceCommandHandler : ICommandHandler<SubmitVendorInvoiceCommand>
{
    private readonly IVendorInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitVendorInvoiceCommandHandler(
        IVendorInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SubmitVendorInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return Result.Failure(new Error("VendorInvoice.NotFound", $"VendorInvoice '{request.InvoiceId}' not found."));

        var result = invoice.Submit();
        if (result.IsFailure)
            return result;

        await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
