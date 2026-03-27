using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Application.Engines.DiscountEngine;
using DomainDrivenERP.Application.Engines.DiscountEngine.Models;
using DomainDrivenERP.Application.Engines.SequenceEngine;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Shared.Results;
using DomainDrivenERP.TaxEngine.Enums;
using DomainDrivenERP.TaxEngine.Factories;
using DomainDrivenERP.TaxEngine.MetaData;
using DomainDrivenERP.TaxEngine.Services;

namespace DomainDrivenERP.Application.Features.Invoices.Commands.PostInvoice;

/// <summary>
/// Full Invoicing Orchestrator:
///   1. Loads invoice with all lines.
///   2. Per-line: resolves discounts via DiscountResolver, calculates taxes via TaxCalculationEngine.
///   3. Stamps LineTaxBreakdown and LineDiscountBreakdown on each line.
///   4. Obtains a gapless sequence number via ISequenceEngine.
///   5. Snapshots the pipeline settings (TaxOrderSetting + StackingMode).
///   6. Posts the invoice domain aggregate.
///   7. Persists.
/// </summary>
internal sealed class PostInvoiceCommandHandler
    : ICommandHandler<PostInvoiceCommand, PostInvoiceResult>
{
    private readonly ICustomerInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DiscountResolver _discountResolver;
    private readonly ISequenceEngine _sequenceEngine;

    public PostInvoiceCommandHandler(
        ICustomerInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork,
        DiscountResolver discountResolver,
        ISequenceEngine sequenceEngine)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _discountResolver = discountResolver;
        _sequenceEngine = sequenceEngine;
    }

    public async Task<Result<PostInvoiceResult>> Handle(
        PostInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        // ── 1. Load Invoice ───────────────────────────────────────────
        var invoice = await _invoiceRepository.GetByIdWithLinesAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return Result.Failure<PostInvoiceResult>(
                new Error("CustomerInvoice.NotFound", $"Invoice '{request.InvoiceId}' not found."));

        // ── 2. Build Tax Engine ───────────────────────────────────────
        // TODO (Phase 5+): Load TaxDefinitions from DB/Redis cache via ITaxDefinitionRepository.
        // Default: Egyptian VAT 14% (V009).
        var taxEngine = new TaxCalculationEngineBuilder()
            .WithStrategy(TaxStrategyFactory.CreateRatioStrategy(TaxTypeEnum.V009, 0.14m))
            .Build();

        // Placeholder — replace with actual TaxDefinition row ID from DB
        var vatDefinitionId = Guid.Empty;

        // ── 3. Per-Line Processing ────────────────────────────────────
        foreach (var line in invoice.Lines)
        {
            // ── Discount Phase ────────────────────────────────────────
            // TODO: inject real candidates from DiscountGroup/Campaign/PriceList resolvers
            var emptyCandidates = Enumerable.Empty<DiscountCandidate>();
            var resolvedDiscount = _discountResolver.Resolve(
                emptyCandidates,
                line.SubTotal,
                invoice.StackingMode);

            var discountBreakdowns = resolvedDiscount.AppliedCandidates
                .Select(c => LineDiscountBreakdown.Create(
                    line.Id,
                    c.Source,
                    c.Type,
                    c.Value,
                    resolvedDiscount.DiscountAmount))
                .ToList();
            line.SetDiscountBreakdown(discountBreakdowns);

            // ── Tax Phase ─────────────────────────────────────────────
            var taxBase = invoice.TaxOrderSetting == TaxOrderSetting.AfterDiscount
                ? line.NetAfterDiscount
                : line.SubTotal;

            var taxResults = taxEngine.CalculateTaxes(taxBase);

            var taxBreakdowns = taxResults
                .Select(kvp => LineTaxBreakdown.Create(
                    line.Id,
                    vatDefinitionId,
                    kvp.Key.ToString(),
                    kvp.Key.ToString(),
                    0m,
                    kvp.Value,
                    kvp.Key.IsWCode()))
                .ToList();
            line.SetTaxBreakdown(taxBreakdowns);

            // Tell EF explicitly that these breakdowns are new
            await _invoiceRepository.AddBreakdownsAsync(taxBreakdowns, discountBreakdowns, cancellationToken);
        }

        // ── 4. Sequence Number ────────────────────────────────────────
        var sequenceNumber = await _sequenceEngine.NextAsync(
            prefix: "INV",
            companyId: invoice.CompanyId,
            date: invoice.InvoiceDate,
            ct: cancellationToken);

        // ── 5. Pipeline Snapshot ──────────────────────────────────────
        var pipelineSnapshot = System.Text.Json.JsonSerializer.Serialize(new
        {
            TaxOrderSetting = invoice.TaxOrderSetting.ToString(),
            StackingMode = invoice.StackingMode.ToString(),
            PostedAt = DateTime.UtcNow
        });

        // ── 6. Post the Invoice ───────────────────────────────────────
        var postResult = invoice.Post(sequenceNumber, pipelineSnapshot);
        if (postResult.IsFailure)
            return Result.Failure<PostInvoiceResult>(postResult.Error);

        // ── 7. Persist ────────────────────────────────────────────────
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new PostInvoiceResult(
            invoice.Id,
            invoice.SequenceNumber!,
            invoice.GrandTotal,
            invoice.Status.ToString()));
    }
}
