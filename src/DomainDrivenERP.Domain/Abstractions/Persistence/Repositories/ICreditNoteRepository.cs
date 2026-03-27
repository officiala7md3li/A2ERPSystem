using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface ICreditNoteRepository
{
    Task<CreditNote?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CreditNote?> GetByIdWithLinesAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CreditNote>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<IReadOnlyList<CreditNote>> GetByStatusAsync(InvoiceStatus status, Guid companyId, CancellationToken ct = default);
    Task AddAsync(CreditNote note, CancellationToken ct = default);
    Task UpdateAsync(CreditNote note, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
