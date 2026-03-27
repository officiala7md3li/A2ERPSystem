using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.Invoices.Commands.AddLineToInvoice;

internal sealed class AddLineToInvoiceCommandHandler
    : ICommandHandler<AddLineToInvoiceCommand, AddLineToInvoiceResult>
{
    private readonly ICustomerInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddLineToInvoiceCommandHandler(
        ICustomerInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddLineToInvoiceResult>> Handle(
        AddLineToInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Load invoice (no-tracking) to validate it exists and is in a mutable state
        var invoice = await _invoiceRepository.GetByIdWithLinesNoTrackingAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return Result.Failure<AddLineToInvoiceResult>(
                new Error("CustomerInvoice.NotFound", $"Invoice '{request.InvoiceId}' not found."));

        // 2. Create line via Domain Factory
        var lineResult = InvoiceLine.Create(
            request.InvoiceId,
            request.ItemId,
            request.Quantity,
            request.QuantityUnit,
            request.UnitPrice,
            request.Currency,
            request.TaxGroupId,
            request.DiscountGroupId,
            request.SortOrder);

        if (lineResult.IsFailure)
            return Result.Failure<AddLineToInvoiceResult>(lineResult.Error);

        var line = lineResult.Value;

        // 3. Validate the domain state (e.g. invoice is Draft)
        var addResult = invoice.AddLine(line);
        if (addResult.IsFailure)
            return Result.Failure<AddLineToInvoiceResult>(addResult.Error);

        // 4. Persist only the new line row (parent invoice row is NOT updated)
        await _invoiceRepository.AddLineAsync(line, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AddLineToInvoiceResult(
            line.Id,
            line.InvoiceId,
            line.ItemId,
            line.SubTotal,
            line.TotalDiscountAmount,
            line.TotalTaxAmount,
            line.FinalLineTotal));
    }
}
