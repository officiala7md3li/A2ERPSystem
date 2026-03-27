namespace DomainDrivenERP.TaxEngine.Enums;

public enum TaxTypeGroup
{
    T1,  // VAT (Compound on Base + T2 + T3 + T5..T12)
    T2,  // Table Tax % (Compound on Base + T5..T12)
    T3,  // Table Tax Fixed
    T4,  // Withholding Tax (Deduction on Base only)
    T5,  // Stamp Duty % (Simple on Base)
    T6,  // Stamp Duty Fixed
    T7,  // Entertainment Tax
    T8,  // Resource Dev
    T9,  // Service Charge
    T10, // Municipality Tax
    T11, // Medical Insurance Fee
    T12, // Other Fees
    T13,
    T14,
    T15,
    T16,
    T17,
    T18,
    T19,
    T20
}
