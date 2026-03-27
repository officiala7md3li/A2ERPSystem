using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Commands.PostVendorInvoice;

internal sealed class PostVendorInvoiceCommandHandler 
    : ICommandHandler<PostVendorInvoiceCommand, PostVendorInvoiceResult>
{
    private readonly IVendorInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PostVendorInvoiceCommandHandler(
        IVendorInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PostVendorInvoiceResult>> Handle(
        PostVendorInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return Result.Failure<PostVendorInvoiceResult>(
                new Error("VendorInvoice.NotFound", $"VendorInvoice '{request.InvoiceId}' not found."));

        // TODO: Orchestrator integration (Tax, Discount, Sequence Engine)
        // For now, simple mock sequence number assignment
        invoice.GetType().GetProperty("SequenceNumber")?.SetValue(invoice, $"VI-{System.DateTime.UtcNow:yyyyMMdd}-001");

        var postResult = invoice.Post($"VI-{System.DateTime.UtcNow:yyyyMMdd}-001", "{}");
        if (postResult.IsFailure)
            return Result.Failure<PostVendorInvoiceResult>(postResult.Error);

        await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new PostVendorInvoiceResult(
            invoice.Id,
            invoice.SequenceNumber!,
            invoice.GrandTotal,
            invoice.Status.ToString()));
    }
}
