namespace DomainDrivenERP.Domain.Enums;

public enum InvoiceType
{
    CustomerInvoice = 1,    // فاتورة عميل
    VendorInvoice = 2,      // فاتورة مورد
    CreditNote = 3,         // إشعار دائن
    DebitNote = 4,          // إشعار مدين
    SalesReturn = 5,        // مردود مبيعات
    PurchaseReturn = 6,     // مردود مشتريات
    SalesOrder = 7,         // أمر بيع
    PurchaseOrder = 8,      // أمر شراء
    ProformaInvoice = 9,    // فاتورة مبدئية
    Quote = 10              // عرض سعر
}
