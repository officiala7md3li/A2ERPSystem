using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Commands.CancelVendorInvoice;

internal sealed class CancelVendorInvoiceCommandHandler : ICommandHandler<CancelVendorInvoiceCommand>
{
    private readonly IVendorInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelVendorInvoiceCommandHandler(
        IVendorInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelVendorInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return Result.Failure(new Error("VendorInvoice.NotFound", $"VendorInvoice '{request.InvoiceId}' not found."));

        var cancelResult = invoice.Cancel();
        if (cancelResult.IsFailure)
            return cancelResult;

        // TODO: If already posted, need to generate Reversal Journal Entry

        await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
