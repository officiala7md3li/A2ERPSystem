# Discount Engine

## Purpose

Resolves and applies discounts from multiple sources on Invoice Lines and Invoice totals, respecting Company-configured Stacking Mode.

## Discount Sources (Priority Order)

```
1. ManualOverride   ← User explicitly overrode (highest priority)
2. Campaign         ← Active marketing campaign
3. PriceList        ← Customer-specific price list
4. Loyalty          ← Based on CustomerTier (Gold/Premium/Diamond)
5. PromoCode        ← One-time code with validity + usage limits
6. Item             ← Item-specific discount
7. Category         ← Inherited from Category (lowest priority)
```

## Discount Types

| Type | Formula | Example |
|------|---------|---------|
| `FixedAmount` | `SubTotal - FixedValue` | -50 EGP |
| `Percentage` | `SubTotal × (Rate/100)` | 10% off |
| `Tiered` | Threshold-based rate | 1-10: 0%, 11-50: 5%, 51+: 10% |
| `Seasonal` | Active if `Now ∈ [Start, End]` | Ramadan 20% off |

## Stacking Modes

| Mode | Behavior |
|------|---------|
| `NoStacking` | Only the highest-value discount applies |
| `FullStacking` | All discounts combine cumulatively |
| `ConditionalStacking` | Based on `IsCombinable` flag per discount |

## Customer Loyalty Tiers

| Tier | Discount |
|------|---------|
| Standard | 0% |
| Gold | 15% |
| Premium | 20% |
| Diamond | 25% |

## Promo Code Model

```csharp
{
    Code: "RAMADAN2026",
    DiscountType: Percentage,
    DiscountValue: 20,
    StartDate: "2026-03-01",
    EndDate: "2026-03-31",
    MaxUses: 1000,           // null = unlimited
    MaxUsesPerUser: 1,
    MinimumOrderAmount: 500,
    IsCombinable: false,     // replaces all other discounts
    IsActive: true
}
```

## Concurrency Safety (Promo Code Usage)

```
1. Try Redis INCR on "promo:{code}:uses"
2. If Redis unavailable → DB transaction with row lock
3. If count > MaxUses → reject
4. On success → both Redis and DB updated
```

## Hidden Discount

A special post-tax discount entered manually by the user:
- Applied **after** Tax calculation
- Shown on the printed invoice
- Does **not** affect tax amounts
- Can be percentage or fixed amount
- Available on Line and Invoice level

## Cap Rules

```
Per-Line Cap:    Discount cannot exceed X% of LineSubTotal
Per-Invoice Cap: Total discount cannot exceed Y EGP
```

Both configurable in Company Settings.
