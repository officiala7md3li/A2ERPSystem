namespace DomainDrivenERP.Domain.Enums;

public enum DiscountSource
{
    None = 0,
    Category = 1,       // موروث من الـ Category
    Item = 2,           // محدد على الـ Item مباشرة
    PriceList = 3,      // من الـ Price List الخاصة بالعميل
    Campaign = 4,       // من حملة تسويقية
    Loyalty = 5,        // خصم الولاء (Gold, Premium, Diamond)
    PromoCode = 6,      // كود خصم
    ManualOverride = 7  // تعديل يدوي من المستخدم
}
