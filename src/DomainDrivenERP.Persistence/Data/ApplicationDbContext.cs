using DomainDrivenERP.Domain.Entities.Companies;
using DomainDrivenERP.Domain.Entities.Currencies;
using DomainDrivenERP.Domain.Entities.DiscountGroups;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Entities.PriceLists;
using DomainDrivenERP.Domain.Entities.PromoCodes;
using DomainDrivenERP.Domain.Entities.TaxDefinitions;
using DomainDrivenERP.Domain.Entities.TaxGroups;
using DomainDrivenERP.Domain.Entities.UnitOfMeasures;
using DomainDrivenERP.Domain.Entities.Vendors;
using DomainDrivenERP.Domain.Entities.Warehouses;
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

    // ── Phase 1 Foundation ────────────────────────────────
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<UnitOfMeasure> UnitOfMeasures => Set<UnitOfMeasure>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<TaxDefinition> TaxDefinitions => Set<TaxDefinition>();
    public DbSet<TaxDependency> TaxDependencies => Set<TaxDependency>();

    // ── Vendor Lookups ────────────────────────────────────
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<VendorType> VendorTypes => Set<VendorType>();
    public DbSet<VendorCategory> VendorCategories => Set<VendorCategory>();
    public DbSet<VendorGroup> VendorGroups => Set<VendorGroup>();

    // ── Phase 2: Tax Groups, Discount Groups, Price Lists ─
    public DbSet<TaxGroup> TaxGroups => Set<TaxGroup>();
    public DbSet<TaxGroupItem> TaxGroupItems => Set<TaxGroupItem>();
    public DbSet<DiscountGroup> DiscountGroups => Set<DiscountGroup>();
    public DbSet<DiscountRule> DiscountRules => Set<DiscountRule>();
    public DbSet<PriceList> PriceLists => Set<PriceList>();
    public DbSet<PriceListItem> PriceListItems => Set<PriceListItem>();

    // ── Phase 3: Warehouses, Promo Codes ──────────────────
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
    public DbSet<PromoCodeUsage> PromoCodeUsages => Set<PromoCodeUsage>();

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

