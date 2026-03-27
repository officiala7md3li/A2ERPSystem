# Invoice Redesign — Complete Module Documentation

## Overview

The invoice system was redesigned from a single flat `Invoice` entity into a **multi-type document system** with full breakdown tracking per line.

## Document Types

| Type | Entity | Accounting Effect |
|------|--------|-------------------|
| Customer Invoice | `CustomerInvoice` | DR: Receivable / CR: Revenue + Tax |
| Vendor Invoice | `VendorInvoice` | DR: Expense + Tax / CR: Payable |
| Credit Note | `CreditNote` | Reversal of Customer Invoice |
| Debit Note | `DebitNote` | Additional charge on Customer Invoice |

## Calculation Flow

```
For each InvoiceLine:
┌─────────────────────────────────────────────────────┐
│  SubTotal = Quantity.Value × UnitPrice.Amount        │
│                                                      │
│  TotalDiscount = Σ(LineDiscountBreakdowns)           │
│  (sources: Category, Item, PriceList, Campaign,      │
│   Loyalty, PromoCode, ManualOverride)                │
│                                                      │
│  ┌─ TaxOrderSetting: AfterDiscount (default) ───┐   │
│  │  TaxBase = SubTotal - TotalDiscount           │   │
│  └───────────────────────────────────────────────┘   │
│  ┌─ TaxOrderSetting: BeforeDiscount ────────────┐   │
│  │  TaxBase = SubTotal                          │   │
│  └───────────────────────────────────────────────┘   │
│                                                      │
│  TotalTax = Σ(LineTaxBreakdowns)                     │
│  (W-codes = negative / withholding)                  │
│                                                      │
│  NetAfterTax = (SubTotal - Discount) + Tax            │
│                                                      │
│  HiddenDiscount = User-defined, post-tax              │
│  FinalLineTotal = NetAfterTax - HiddenDiscount        │
└─────────────────────────────────────────────────────┘

Invoice GrandTotal:
= Σ(FinalLineTotals) - InvoiceLevelDiscounts - InvoiceHiddenDiscount
```

## Status Machine

```
[Draft] ──submit()──► [Pending] ──post()──► [Posted] ──approve()──► [Approved]
                                                │                        │
                                         cancel()                  partialPay()
                                                │                        │
                                          [Cancelled]           [PartiallyPaid]
                                                                         │
                                                                     fullPay()
                                                                         │
                                                                      [Paid]
```

## Snapshot Pattern

When `post()` is called, the current pipeline settings are frozen:

```json
{
  "TaxOrderSetting": "AfterDiscount",
  "StackingMode": "NoStacking",
  "PostedAt": "2026-03-22T10:00:00Z"
}
```

This ensures **Reversal Journals** always use the original calculation settings, even if Company Settings change later.

## API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/api/invoices/customer` | Create draft invoice |
| `GET` | `/api/invoices/customer/{id}` | Get invoice with full breakdown |
| `GET` | `/api/invoices/customer/customer/{customerId}` | List customer invoices |
| `POST` | `/api/invoices/customer/{id}/lines` | Add line to draft |
| `POST` | `/api/invoices/customer/{id}/submit` | Draft → Pending |
| `POST` | `/api/invoices/customer/{id}/post` | Pending → Posted (Orchestrator) |
| `POST` | `/api/invoices/customer/{id}/cancel` | Cancel with reason |

## Example: Create + Add Line + Post

```bash
# 1. Create draft invoice
curl -X POST /api/invoices/customer \
  -H "Authorization: Bearer {token}" \
  -d '{
    "customerId": "...",
    "companyId": "...",
    "currencyId": "...",
    "invoiceDate": "2026-03-22",
    "taxOrderSetting": "AfterDiscount",
    "stackingMode": "NoStacking"
  }'
# Returns: { "invoiceId": "abc-123", "status": "Draft" }

# 2. Add a line
curl -X POST /api/invoices/customer/abc-123/lines \
  -d '{
    "itemId": "...",
    "quantity": 10,
    "quantityUnit": "PCS",
    "unitPrice": 500.00,
    "currency": "EGP",
    "taxGroupId": "...",
    "discountGroupId": "..."
  }'

# 3. Submit (Draft → Pending)
curl -X POST /api/invoices/customer/abc-123/submit

# 4. Post (Pending → Posted)
curl -X POST /api/invoices/customer/abc-123/post
# Returns: { "sequenceNumber": "INV-20260322-ABC123", "grandTotal": 5890.00, "status": "Posted" }
```

## Database Tables

| Table | Description |
|-------|-------------|
| `CustomerInvoices` | Invoice header |
| `VendorInvoices` | Vendor invoice header |
| `CreditNotes` | Credit note header |
| `DebitNotes` | Debit note header |
| `InvoiceLines` | Lines for all invoice types |
| `LineTaxBreakdowns` | Tax detail per line |
| `LineDiscountBreakdowns` | Discount detail per line |
| `InvoiceLevelDiscounts` | Invoice-level discount records |
