namespace EShop.Web.Models;

public class ProductResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string ImageUrl { get; set; } = string.Empty;
}

public class ShoppingCartResponse
{
    public string Id { get; set; } = string.Empty;

    public List<CartItemResponse> Items { get; set; } = [];

    public decimal TotalAmount { get; set; }
}

public class CartItemResponse
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;
}

public class AddCartItemRequest
{
    public int ProductId { get; set; }

    public int Quantity { get; set; } = 1;
}

public class CheckoutRequest
{
    public string CustomerName { get; set; } = string.Empty;
}

public class OrderResponse
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = string.Empty;
}

public class CustomerResponse
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
}

public class CustomerRequest
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
}
