namespace DomainDrivenERP.Domain.Enums;

public enum DiscountType
{
    FixedAmount = 0,    // مبلغ ثابت
    Percentage = 1,     // نسبة مئوية
    Tiered = 2,         // متدرج حسب الكمية
    Seasonal = 3        // موسمي حسب التاريخ
}
