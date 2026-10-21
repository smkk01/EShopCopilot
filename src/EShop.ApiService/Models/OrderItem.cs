namespace EShop.ApiService.Models;

public class OrderItem
{
    public int ProductId { get; set; }

    public required string ProductName { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }
}
