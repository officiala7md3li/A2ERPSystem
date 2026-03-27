# Tax Engine — `DomainDrivenERP.TaxEngine` (Separate Class Library)

## Purpose

A **self-contained, dependency-free** tax calculation library implementing the Egyptian e-invoicing tax model. Uses a **Dependency Graph (DAG)** with Topological Sort to calculate taxes in the correct order automatically.

## Architecture

```
TaxCalculation/
├── Interfaces/
│   └── ITaxCalculationStrategy.cs
├── Strategies/
│   ├── FixedTaxStrategy.cs      # Fixed amount (e.g., Tbl02 = 10 EGP)
│   └── RatioTaxStrategy.cs      # Percentage (simple or compound)
├── Services/
│   ├── TaxCalculationEngine.cs  # Core engine with TopologicalSort
│   ├── TaxCalculationEngineBuilder.cs
│   └── TaxCategorizer.cs
├── Factories/
│   └── TaxStrategyFactory.cs
├── Enums/
│   ├── TaxTypeEnum.cs           # All Egyptian tax codes
│   └── TaxTypeGroup.cs          # T1..T20 groups
└── MetaData/
    └── TaxMetadata.cs           # Arabic + English descriptions
```

## Egyptian Tax Code Groups

| Group | Codes | Type | Description |
|-------|-------|------|-------------|
| T1 | V001–V010 | % Compound | VAT — depends on Tbl01, Tbl02, and sub-taxes |
| T2 | Tbl01 | % Compound | Table tax — depends on ST+Ent+RD+SC+Mn+MI+OF |
| T3 | Tbl02 | Fixed | Table tax fixed amount |
| T4 | W001–W016 | % Deduction | Withholding — subtracts from total |
| T5 | ST01 | % Simple | Stamp duty |
| T6 | ST02 | Fixed | Stamp duty fixed |
| T7–T20 | ... | % or Fixed | Entertainment, Resource Dev, Service charges... |

## Calculation Order (Topological Sort)

```
Base Price
    │
    ├──► T4 (W codes) — Withholding (deduction, on base only)
    │
    ├──► T5–T14 (Simple %) — Stamp, Entertainment, Resource Dev, Service...
    │
    ├──► T2 (Tbl01) — Compound % on (Base + T5 + T6 + T7 + T8 + T9 + T10 + T11 + T12)
    │
    └──► T1 (VAT) — Compound % on (Base + T2 + T3 + T5..T12)
```

The engine **auto-detects this order** — no manual sorting needed.

## Usage

```csharp
// Build engine with tax strategies
var engine = new TaxCalculationEngineBuilder()
    .WithStrategy(TaxTypeEnum.V009, 0.14m)   // VAT 14%
    .WithStrategy(TaxTypeEnum.ST01, 0.05m)   // Stamp 5%
    .WithStrategy(TaxTypeEnum.Tbl01, 0.05m)  // Table tax 5%
    .WithStrategy(TaxTypeEnum.W001, 0.01m)   // Withholding 1%
    .Build();

// Calculate
var taxes = engine.CalculateTaxes(baseAmount: 1000m);

// Result:
// ST01   = 50.00  (1000 × 5%)
// Tbl01  = 52.50  (1050 × 5%)  ← includes ST01
// V009   = 156.75 (1052.5+50+52.5) × 14% ← compound
// W001   = -10.00 (1000 × 1%)  ← deduction
```

## Integration with A2ERP

In A2ERP, the Tax Engine is called by the **Invoicing Orchestrator** during the `PostInvoice` flow:

```csharp
// Orchestrator calls Tax Engine per line
var lineTaxes = _taxEngine.CalculateTaxes(line.NetAfterDiscount);

// Results stored as LineTaxBreakdown records
foreach (var (taxType, amount) in lineTaxes)
{
    line.AddTaxBreakdown(new LineTaxBreakdown(
        taxCode: taxType.ToString(),
        taxAmount: amount,
        isWithholding: taxType.IsWCode()));
}
```

## Dynamic Tax Definitions (DB-Driven)

In production, tax rates and dependencies come from the DB (`TaxDefinition` table), not hardcoded values:

```csharp
// TaxDefinition entity
{
    Id: Guid,
    Code: "VAT",
    Rate: 0.14,
    CalculationMethod: Compound,
    DependsOn: [Guid_Tbl01, Guid_Tbl02, Guid_ST01, ...],
    IsWithholding: false
}
```

The `TaxStrategyFactory` builds strategies from DB records cached in Redis (24h TTL).
