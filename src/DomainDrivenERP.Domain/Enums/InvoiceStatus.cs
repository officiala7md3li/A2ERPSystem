namespace DomainDrivenERP.Domain.Enums;

public enum InvoiceStatus
{
    Draft = 0,      // مسودة — قابلة للتعديل
    Pending = 1,    // معلقة — في انتظار الموافقة
    Posted = 2,     // مرحّلة — تم إنشاء القيود المحاسبية
    Approved = 3,   // معتمدة
    PartiallyPaid = 4, // مدفوعة جزئياً
    Paid = 5,       // مدفوعة بالكامل
    Cancelled = 6   // ملغية — مع قيد عكسي
}
