using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.Invoices.Queries.GetCustomerInvoices;

internal sealed class GetCustomerInvoicesQueryHandler 
    : IQueryHandler<GetCustomerInvoicesQuery, IReadOnlyList<CustomerInvoiceListDto>>
{
    private readonly ICustomerInvoiceRepository _invoiceRepository;

    public GetCustomerInvoicesQueryHandler(ICustomerInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<IReadOnlyList<CustomerInvoiceListDto>>> Handle(
        GetCustomerInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        
        var dtos = invoices.Select(i => new CustomerInvoiceListDto(
            i.Id,
            i.CustomerId,
            i.SequenceNumber,
            i.InvoiceDate,
            i.Status.ToString(),
            i.GrandTotal
        )).ToList();

        return Result.Success<IReadOnlyList<CustomerInvoiceListDto>>(dtos);
    }
}
