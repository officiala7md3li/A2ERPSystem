namespace DomainDrivenERP.Domain.Enums;

public enum StackingMode
{
    NoStacking = 0,         // الأعلى قيمة فقط
    FullStacking = 1,       // كل الخصومات مع بعض
    ConditionalStacking = 2 // حسب الشروط المحددة
}
