namespace DomainDrivenERP.Web.Models.Orders;

public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public List<LineItem> LineItems { get; set; } = new();
    public OrderStatus Status { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
}

public class LineItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}
