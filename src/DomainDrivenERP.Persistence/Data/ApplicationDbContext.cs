using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Persistence.Entities;
using DomainDrivenERP.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenERP.Persistence.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.Migrate();
    }

    // ── Invoicing ─────────────────────────────────────────
    public DbSet<CustomerInvoice> CustomerInvoices => Set<CustomerInvoice>();
    public DbSet<VendorInvoice> VendorInvoices => Set<VendorInvoice>();
    public DbSet<CreditNote> CreditNotes => Set<CreditNote>();
    public DbSet<DebitNote> DebitNotes => Set<DebitNote>();
    public DbSet<InvoiceLine> InvoiceLines => Set<InvoiceLine>();
    public DbSet<LineTaxBreakdown> LineTaxBreakdowns => Set<LineTaxBreakdown>();
    public DbSet<LineDiscountBreakdown> LineDiscountBreakdowns => Set<LineDiscountBreakdown>();
    public DbSet<InvoiceLevelDiscount> InvoiceLevelDiscounts => Set<InvoiceLevelDiscount>();

    // ── Outbox ────────────────────────────────────────────
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    // ── Sequences ─────────────────────────────────────────
    public DbSet<SequenceCounter> SequenceCounters => Set<SequenceCounter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}
