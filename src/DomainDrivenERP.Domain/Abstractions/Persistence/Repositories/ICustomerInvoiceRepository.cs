using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface ICustomerInvoiceRepository
{
    Task<CustomerInvoice?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CustomerInvoice?> GetByIdWithLinesAsync(Guid id, CancellationToken ct = default);
    Task<CustomerInvoice?> GetByIdWithLinesNoTrackingAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CustomerInvoice>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<IReadOnlyList<CustomerInvoice>> GetByStatusAsync(InvoiceStatus status, Guid companyId, CancellationToken ct = default);
    Task AddAsync(CustomerInvoice invoice, CancellationToken ct = default);
    Task AddLineAsync(InvoiceLine line, CancellationToken ct = default);
    Task AddBreakdownsAsync(IEnumerable<LineTaxBreakdown> taxes, IEnumerable<LineDiscountBreakdown> discounts, CancellationToken ct = default);
    Task UpdateAsync(CustomerInvoice invoice, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> IsSequenceNumberUniqueAsync(string sequenceNumber, Guid companyId, CancellationToken ct = default);
}
