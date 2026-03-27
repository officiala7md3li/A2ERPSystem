namespace DomainDrivenERP.Domain.Enums;

public enum CustomerTier
{
    Standard = 0,   // عادي — بدون خصم ولاء
    Gold = 1,       // ذهبي — 15%
    Premium = 2,    // مميز — 20%
    Diamond = 3     // ماسي — 25%
}//After some consideration, we decided to keep the loyalty tiers simple and not add more levels like Silver or Platinum, as it would complicate the discount logic without providing significant additional value to our customers.
//in the future, we can always add more tiers if we find that our customers would benefit from them, but for now, we want to keep it straightforward and easy to understand.
