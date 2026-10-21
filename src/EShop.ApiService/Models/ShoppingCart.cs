namespace EShop.ApiService.Models;

public class ShoppingCart
{
    public string Id { get; set; } = "default";

    public List<CartItem> Items { get; set; } = [];

    public decimal TotalAmount => Items.Sum(item => item.UnitPrice * item.Quantity);
}
