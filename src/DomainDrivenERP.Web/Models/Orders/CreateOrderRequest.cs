using System.ComponentModel.DataAnnotations;

namespace DomainDrivenERP.Web.Models.Orders;

public class CreateOrderRequest
{
    [Required(ErrorMessage = "Customer is required")]
    [Display(Name = "Customer")]
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "At least one item is required")]
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    [Required(ErrorMessage = "Product is required")]
    [Display(Name = "Product")]
    public Guid ProductId { get; set; }

    [Required(ErrorMessage = "Product price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Product price must be greater than 0")]
    [Display(Name = "Product Price")]
    public decimal ProductPrice { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}
