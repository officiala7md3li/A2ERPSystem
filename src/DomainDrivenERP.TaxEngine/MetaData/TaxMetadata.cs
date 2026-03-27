using DomainDrivenERP.TaxEngine.Enums;

namespace DomainDrivenERP.TaxEngine.MetaData;

public sealed record TaxMetadata(
    TaxTypeEnum Code,
    TaxTypeGroup Group,
    string ArabicName,
    string EnglishName,
    bool IsWithholding);

public static class TaxCategorizer
{
    public static bool IsWCode(this TaxTypeEnum taxType)
    {
        return taxType.ToString().StartsWith("W");
    }

    public static TaxTypeGroup GetGroup(this TaxTypeEnum taxType)
    {
        var code = taxType.ToString();
        if (code.StartsWith("V")) return TaxTypeGroup.T1;
        if (code == "Tbl01") return TaxTypeGroup.T2;
        if (code == "Tbl02") return TaxTypeGroup.T3;
        if (code.StartsWith("W")) return TaxTypeGroup.T4;
        if (code == "ST01") return TaxTypeGroup.T5;
        if (code == "ST02") return TaxTypeGroup.T6;
        if (code.StartsWith("Ent")) return TaxTypeGroup.T7;
        if (code.StartsWith("RD")) return TaxTypeGroup.T8;
        if (code.StartsWith("SC")) return TaxTypeGroup.T9;
        if (code.StartsWith("Mn")) return TaxTypeGroup.T10;
        if (code.StartsWith("MI")) return TaxTypeGroup.T11;
        if (code.StartsWith("OF")) return TaxTypeGroup.T12;

        return TaxTypeGroup.T20; // Default generic fallback
    }
}
