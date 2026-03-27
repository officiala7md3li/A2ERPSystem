namespace DomainDrivenERP.TaxEngine.Enums;

public enum TaxTypeEnum
{
    // T1: VAT
    V001, V002, V003, V004, V005, V006, V007, V008, V009, V010,
    
    // T2: Table Tax (Percentage)
    Tbl01,
    
    // T3: Table Tax (Fixed Amount)
    Tbl02,

    // T4: Withholding Tax (W-codes)
    W001, W002, W003, W004, W005, W006, W007, W008, W009, W010, W011, W012, W013, W014, W015, W016,
    
    // T5: Stamp Duty (Percentage)
    ST01,
    
    // T6: Stamp Duty (Fixed Amount)
    ST02,
    
    // T7: Entertainment Tax
    Ent01, Ent02,
    
    // T8: Resource Development Fee
    RD01, RD02,
    
    // T9: Service Charge
    SC01, SC02,
    
    // T10: Municipality Tax
    Mn01, Mn02,
    
    // T11: Medical Insurance Fee
    MI01, MI02,
    
    // T12: Other Fees
    OF01, OF02
}
