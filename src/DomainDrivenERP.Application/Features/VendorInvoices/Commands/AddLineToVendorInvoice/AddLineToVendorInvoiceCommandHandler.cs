using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Commands.AddLineToVendorInvoice;

internal sealed class AddLineToVendorInvoiceCommandHandler
    : ICommandHandler<AddLineToVendorInvoiceCommand, AddLineToVendorInvoiceResult>
{
    private readonly IVendorInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddLineToVendorInvoiceCommandHandler(
        IVendorInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddLineToVendorInvoiceResult>> Handle(
        AddLineToVendorInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdWithLinesAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return Result.Failure<AddLineToVendorInvoiceResult>(
                new Error("VendorInvoice.NotFound", $"VendorInvoice '{request.InvoiceId}' not found."));

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
            return Result.Failure<AddLineToVendorInvoiceResult>(lineResult.Error);

        var line = lineResult.Value;

        var addResult = invoice.AddLine(line);
        if (addResult.IsFailure)
            return Result.Failure<AddLineToVendorInvoiceResult>(addResult.Error);

        await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AddLineToVendorInvoiceResult(
            line.Id,
            line.InvoiceId,
            line.ItemId,
            line.SubTotal,
            line.TotalDiscountAmount,
            line.TotalTaxAmount,
            line.FinalLineTotal));
    }
}
