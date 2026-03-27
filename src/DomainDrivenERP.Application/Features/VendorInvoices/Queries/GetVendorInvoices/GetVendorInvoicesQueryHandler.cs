using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Queries.GetVendorInvoices;

internal sealed class GetVendorInvoicesQueryHandler 
    : IQueryHandler<GetVendorInvoicesQuery, IReadOnlyList<VendorInvoiceListDto>>
{
    private readonly IVendorInvoiceRepository _invoiceRepository;

    public GetVendorInvoicesQueryHandler(IVendorInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<IReadOnlyList<VendorInvoiceListDto>>> Handle(
        GetVendorInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetByVendorIdAsync(request.VendorId, cancellationToken);
        
        var dtos = invoices.Select(i => new VendorInvoiceListDto(
            i.Id,
            i.VendorId,
            i.SequenceNumber,
            i.VendorInvoiceNumber,
            i.InvoiceDate,
            i.Status.ToString(),
            i.GrandTotal
        )).ToList();

        return Result.Success<IReadOnlyList<VendorInvoiceListDto>>(dtos);
    }
}
