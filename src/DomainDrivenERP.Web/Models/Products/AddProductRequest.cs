using System.ComponentModel.DataAnnotations;

namespace DomainDrivenERP.Web.Models.Products;

public class AddProductRequest
{
    [Required(ErrorMessage = "Product name is required")]
    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Currency is required")]
    public string Currency { get; set; } = "USD";

    [Required(ErrorMessage = "Stock quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]
    [Display(Name = "Stock Quantity")]
    public int StockQuantity { get; set; }

    [Required(ErrorMessage = "SKU is required")]
    public string Sku { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model is required")]
    public string Model { get; set; } = string.Empty;

    [Required(ErrorMessage = "Details are required")]
    public string Details { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public Guid CategoryId { get; set; }
}
