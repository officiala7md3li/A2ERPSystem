using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface IDebitNoteRepository
{
    Task<DebitNote?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<DebitNote?> GetByIdWithLinesAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<DebitNote>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<IReadOnlyList<DebitNote>> GetByStatusAsync(InvoiceStatus status, Guid companyId, CancellationToken ct = default);
    Task AddAsync(DebitNote note, CancellationToken ct = default);
    Task UpdateAsync(DebitNote note, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
