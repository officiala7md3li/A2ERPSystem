namespace DomainDrivenERP.Persistence.Constants;

internal static class TableNames
{
    // Identity
    internal const string Customers = nameof(Customers);
    internal const string Vendors = nameof(Vendors);

    // Invoicing
    internal const string CustomerInvoices = nameof(CustomerInvoices);
    internal const string VendorInvoices = nameof(VendorInvoices);
    internal const string CreditNotes = nameof(CreditNotes);
    internal const string DebitNotes = nameof(DebitNotes);
    internal const string InvoiceLines = nameof(InvoiceLines);
    internal const string LineTaxBreakdowns = nameof(LineTaxBreakdowns);
    internal const string LineDiscountBreakdowns = nameof(LineDiscountBreakdowns);
    internal const string InvoiceLevelDiscounts = nameof(InvoiceLevelDiscounts);

    // Legacy (kept for backward compatibility)
    internal const string Invoices = nameof(Invoices);

    // Accounting
    internal const string Accounts = nameof(Accounts);
    internal const string Journals = nameof(Journals);
    internal const string Transactions = nameof(Transactions);

    // Products
    internal const string Products = nameof(Products);
    internal const string Categories = nameof(Categories);

    // Orders
    internal const string Orders = nameof(Orders);
    internal const string LineItems = nameof(LineItems);

    // Infrastructure
    internal const string OutboxMessages = nameof(OutboxMessages);
    internal const string OutboxMessageConsumers = nameof(OutboxMessageConsumers);

    // Localization
    internal const string Languages = nameof(Languages);
    internal const string LanguageResources = nameof(LanguageResources);
    internal const string TranslationCaches = nameof(TranslationCaches);
    internal const string TranslationImports = nameof(TranslationImports);
    internal const string TranslationExports = nameof(TranslationExports);
    internal const string LocalizationSettings = nameof(LocalizationSettings);
    internal const string TranslationAudits = nameof(TranslationAudits);

    // -- Phase 1 Entities -----------------------------------------
    internal const string Currencies = nameof(Currencies);
    internal const string UnitOfMeasures = nameof(UnitOfMeasures);
    internal const string Companies = nameof(Companies);
    internal const string TaxDefinitions = nameof(TaxDefinitions);
    internal const string TaxDependencies = nameof(TaxDependencies);

    // ── Phase 2 Entities ─────────────────────────────────────────
    internal const string Vendor = nameof(Vendor);
    internal const string TaxGroups = nameof(TaxGroups);
    internal const string TaxGroupItems = nameof(TaxGroupItems);
    internal const string DiscountGroups = nameof(DiscountGroups);
    internal const string DiscountRules = nameof(DiscountRules);
    internal const string PriceLists = nameof(PriceLists);
    internal const string PriceListItems = nameof(PriceListItems);

    // ── Vendor Lookups ──────────────────────────────────────────
    internal const string VendorTypes = nameof(VendorTypes);
    internal const string VendorCategories = nameof(VendorCategories);
    internal const string VendorGroups = nameof(VendorGroups);

    // ── Phase 3 Entities ─────────────────────────────────────────
    internal const string Warehouses = nameof(Warehouses);
    internal const string PromoCodes = nameof(PromoCodes);
    internal const string PromoCodeUsages = nameof(PromoCodeUsages);
}
