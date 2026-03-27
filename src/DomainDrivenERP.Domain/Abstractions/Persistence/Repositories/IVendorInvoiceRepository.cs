using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface IVendorInvoiceRepository
{
    Task<VendorInvoice?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<VendorInvoice?> GetByIdWithLinesAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<VendorInvoice>> GetByVendorIdAsync(Guid vendorId, CancellationToken ct = default);
    Task<IReadOnlyList<VendorInvoice>> GetByStatusAsync(InvoiceStatus status, Guid companyId, CancellationToken ct = default);
    Task AddAsync(VendorInvoice invoice, CancellationToken ct = default);
    Task UpdateAsync(VendorInvoice invoice, CancellationToken ct = default);
    Task<bool> IsVendorInvoiceNumberDuplicateAsync(string vendorInvoiceNumber, Guid vendorId, CancellationToken ct = default);
}
