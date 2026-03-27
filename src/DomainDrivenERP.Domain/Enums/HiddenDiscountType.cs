namespace DomainDrivenERP.Domain.Enums;

public enum HiddenDiscountType
{
    None = 0,
    Percentage = 1,  // نسبة من الإجمالي بعد الضريبة
    FixedAmount = 2  // مبلغ ثابت
}
