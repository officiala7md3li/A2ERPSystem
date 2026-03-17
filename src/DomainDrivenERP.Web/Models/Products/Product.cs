namespace DomainDrivenERP.Web.Models.Products;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
}
