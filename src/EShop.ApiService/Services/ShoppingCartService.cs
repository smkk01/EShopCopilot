using EShop.ApiService.Models;

namespace EShop.ApiService.Services;

public class ShoppingCartService(ProductService productService, OrderService orderService)
{
    private readonly object _syncRoot = new();
    private readonly ShoppingCart _cart = new();

    public ShoppingCart GetCart()
    {
        lock (_syncRoot)
        {
            return Clone(_cart);
        }
    }

    public ShoppingCart? AddItem(int productId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        var product = productService.GetById(productId);

        if (product is null)
        {
            return null;
        }

        lock (_syncRoot)
        {
            var existingItem = _cart.Items.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem is null)
            {
                _cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = quantity
                });
            }
            else
            {
                existingItem.Quantity += quantity;
            }

            return Clone(_cart);
        }
    }

    public bool RemoveItem(int productId)
    {
        lock (_syncRoot)
        {
            var item = _cart.Items.FirstOrDefault(item => item.ProductId == productId);

            if (item is null)
            {
                return false;
            }

            _cart.Items.Remove(item);

            return true;
        }
    }

    public Order Checkout(string customerName)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("Customer name is required.", nameof(customerName));
        }

        lock (_syncRoot)
        {
            if (_cart.Items.Count == 0)
            {
                throw new InvalidOperationException("The shopping cart is empty.");
            }

            var order = orderService.Create(new Order
            {
                CustomerName = customerName.Trim(),
                OrderDate = DateTimeOffset.UtcNow,
                TotalAmount = _cart.TotalAmount,
                Status = "Pending",
                Items = _cart.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                }).ToList()
            });

            _cart.Items.Clear();

            return order;
        }
    }

    private static ShoppingCart Clone(ShoppingCart cart) =>
        new()
        {
            Id = cart.Id,
            Items = cart.Items.Select(item => new CartItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            }).ToList()
        };
}
