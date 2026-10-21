namespace EShop.ApiService.Models;

public class Order
{
    public int Id { get; set; }

    public required string CustomerName { get; set; }

    public DateTimeOffset OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public required string Status { get; set; }

    public List<OrderItem> Items { get; set; } = [];
}
