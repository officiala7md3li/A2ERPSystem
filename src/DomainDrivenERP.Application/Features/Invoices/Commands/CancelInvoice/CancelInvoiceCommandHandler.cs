using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Dtos;
using DomainDrivenERP.Domain.Entities.Journals;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Shared.Results;
using MediatR;

namespace DomainDrivenERP.Application.Features.Invoices.Commands.CancelInvoice;

internal sealed class CancelInvoiceCommandHandler : ICommandHandler<CancelInvoiceCommand>
{
    private readonly ICustomerInvoiceRepository _invoiceRepository;
    private readonly IJournalRepository _journalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _sender;

    public CancelInvoiceCommandHandler(
        ICustomerInvoiceRepository invoiceRepository,
        IJournalRepository journalRepository,
        IUnitOfWork unitOfWork,
        ISender sender)
    {
        _invoiceRepository = invoiceRepository;
        _journalRepository = journalRepository;
        _unitOfWork = unitOfWork;
        _sender = sender;
    }

    public async Task<Result> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
    {
        // ── 1. Load Invoice ───────────────────────────────────────────
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return Result.Failure(new Error("CustomerInvoice.NotFound", $"Invoice '{request.InvoiceId}' not found."));

        var wasPosted = invoice.Status == InvoiceStatus.Posted;

        // ── 2. Cancel Domain Aggregate ───────────────────────────────
        var cancelResult = invoice.Cancel();
        if (cancelResult.IsFailure) return cancelResult;

        // ── 3. Reversal Journal (only for Posted invoices) ────────────
        if (wasPosted && !string.IsNullOrWhiteSpace(invoice.SequenceNumber))
        {
            // Build a reversal journal as a mirror of the original invoice totals.
            // DR: Accounts Receivable reversal → CR: Revenue/Tax accounts.
            // The exact COA IDs should come from the company's chart-of-accounts mapping.
            // Using Guid.Empty as placeholder — wire real COA lookup in Phase 5+.
            var reversalTransactions = new List<TransactionDto>
            {
                new TransactionDto
                {
                    // Credit Accounts Receivable (reverse the original Debit)
                    AccountId = Guid.Empty, // AR account — replace with COA lookup
                    Credit = (double)invoice.GrandTotal,
                    Debit = 0
                },
                new TransactionDto
                {
                    // Debit Revenue (reverse the original Credit)
                    AccountId = Guid.Empty, // Revenue account — replace with COA lookup
                    Debit = (double)(invoice.SubTotal - invoice.TotalLineDiscount),
                    Credit = 0
                },
                new TransactionDto
                {
                    // Debit VAT Payable (reverse the tax)
                    AccountId = Guid.Empty, // VAT Payable account — replace with COA lookup
                    Debit = (double)invoice.TotalTax,
                    Credit = 0
                }
            };


            var journalResult = Journal.Create(
                $"Reversal of Invoice {invoice.SequenceNumber}",
                isOpening: false,
                journalDate: DateTime.UtcNow,
                transactions: reversalTransactions);

            if (journalResult.IsFailure)
                return Result.Failure(journalResult.Error);

            await _journalRepository.CreateJournal(journalResult.Value);
        }

        // ── 4. Persist ────────────────────────────────────────────────
        await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
