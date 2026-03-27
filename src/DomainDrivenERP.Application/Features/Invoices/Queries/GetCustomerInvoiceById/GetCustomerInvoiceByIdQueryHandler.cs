using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.Invoices.Queries.GetCustomerInvoiceById;

internal sealed class GetCustomerInvoiceByIdQueryHandler 
    : IQueryHandler<GetCustomerInvoiceByIdQuery, CustomerInvoiceDetailDto>
{
    private readonly ICustomerInvoiceRepository _invoiceRepository;

    public GetCustomerInvoiceByIdQueryHandler(ICustomerInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<CustomerInvoiceDetailDto>> Handle(
        GetCustomerInvoiceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdWithLinesAsync(request.InvoiceId, cancellationToken);
        
        if (invoice is null)
        {
            return Result.Failure<CustomerInvoiceDetailDto>(
                new Error("CustomerInvoice.NotFound", $"Invoice '{request.InvoiceId}' not found."));
        }

        var dto = new CustomerInvoiceDetailDto(
            invoice.Id,
            invoice.CustomerId,
            invoice.CompanyId,
            invoice.SequenceNumber,
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
            invoice.Lines.Select(l => new InvoiceLineDto(
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
                l.TaxBreakdowns.Select(t => new LineTaxBreakdownDto(
                    t.TaxCode, t.TaxName, t.Rate, t.TaxAmount, t.IsWithholding)).ToList(),
                l.DiscountBreakdowns.Select(d => new LineDiscountBreakdownDto(
                    d.Source.ToString(), d.Type.ToString(), d.DiscountValue, d.DiscountAmount)).ToList()
            )).ToList()
        );

        return Result.Success(dto);
    }
}
