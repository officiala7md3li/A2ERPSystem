using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.Invoices.Commands.SubmitInvoice;

internal sealed class SubmitInvoiceCommandHandler : ICommandHandler<SubmitInvoiceCommand>
{
    private readonly ICustomerInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitInvoiceCommandHandler(
        ICustomerInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SubmitInvoiceCommand request, CancellationToken cancellationToken)
    {
        // Load WITH lines so the domain guard (HasLines) works correctly
        var invoice = await _invoiceRepository.GetByIdWithLinesAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
        {
            return Result.Failure(new Error("CustomerInvoice.NotFound", $"Invoice '{request.InvoiceId}' not found."));
        }

        var result = invoice.Submit();
        if (result.IsFailure)
        {
            return result;
        }

        // No explicit UpdateAsync — EF change tracker handles it (entity loaded above)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
