using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Queries.GetVendorInvoiceById;

internal sealed class GetVendorInvoiceByIdQueryHandler 
    : IQueryHandler<GetVendorInvoiceByIdQuery, VendorInvoiceDetailDto>
{
    private readonly IVendorInvoiceRepository _invoiceRepository;

    public GetVendorInvoiceByIdQueryHandler(IVendorInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<VendorInvoiceDetailDto>> Handle(
        GetVendorInvoiceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdWithLinesAsync(request.InvoiceId, cancellationToken);
        
        if (invoice is null)
            return Result.Failure<VendorInvoiceDetailDto>(
                new Error("VendorInvoice.NotFound", $"VendorInvoice '{request.InvoiceId}' not found."));

        var dto = new VendorInvoiceDetailDto(
            invoice.Id,
            invoice.VendorId,
            invoice.CompanyId,
            invoice.SequenceNumber,
            invoice.VendorInvoiceNumber,
            invoice.InvoiceDate,
            invoice.Status.ToString(),
            invoice.SubTotal,
            invoice.TotalLineDiscount,
            invoice.TotalTax,
            invoice.TotalInvoiceDiscount,
            invoice.TotalHiddenDiscount,
            invoice.GrandTotal,
            invoice.TaxOrderSetting.ToString(),
            invoice.StackingMode.ToString(),
            invoice.Lines.Select(l => new VendorInvoiceLineDto(
                l.Id,
                l.ItemId,
                l.Quantity.Value,
                l.Quantity.Unit,
                l.UnitPrice.Amount,
                l.UnitPrice.Currency,
                l.SubTotal,
                l.TotalDiscountAmount,
                l.TotalTaxAmount,
                l.HiddenDiscountAmount,
                l.FinalLineTotal,
                l.TaxBreakdowns.Select(t => new VendorLineTaxBreakdownDto(
                    t.TaxCode, t.TaxName, t.Rate, t.TaxAmount, t.IsWithholding)).ToList(),
                l.DiscountBreakdowns.Select(d => new VendorLineDiscountBreakdownDto(
                    d.Source.ToString(), d.Type.ToString(), d.DiscountValue, d.DiscountAmount)).ToList()
            )).ToList()
        );

        return Result.Success(dto);
    }
}
