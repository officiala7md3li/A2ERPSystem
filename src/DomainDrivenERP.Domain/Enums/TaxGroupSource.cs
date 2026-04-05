namespace DomainDrivenERP.Domain.Enums;

/// <summary>
/// Determines how a Product resolves its TaxGroup.
/// </summary>
public enum TaxGroupSource
{
    Category = 1,  // Inherits from Category.DefaultTaxGroupId
    Custom   = 2,  // Uses Product.TaxGroupId directly
    Exempt   = 3   // No tax applied at all
}
