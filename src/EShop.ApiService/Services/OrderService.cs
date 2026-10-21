using EShop.ApiService.Models;

namespace EShop.ApiService.Services;

public class OrderService
{
    private readonly object _syncRoot = new();
    private readonly List<Order> _orders =
    [
        new Order
        {
            Id = 1,
            CustomerName = "Ava Thompson",
            OrderDate = new DateTimeOffset(2026, 10, 15, 10, 30, 0, TimeSpan.Zero),
            TotalAmount = 278.49m,
            Status = "Processing",
            Items =
            [
                new OrderItem
                {
                    ProductId = 1,
                    ProductName = "Wireless Noise-Cancelling Headphones",
                    UnitPrice = 249.99m,
                    Quantity = 1
                },
                new OrderItem
                {
                    ProductId = 3,
                    ProductName = "Stainless Steel Water Bottle",
                    UnitPrice = 28.50m,
                    Quantity = 1
                }
            ]
        },
        new Order
        {
            Id = 2,
            CustomerName = "Liam Patel",
            OrderDate = new DateTimeOffset(2026, 10, 18, 14, 45, 0, TimeSpan.Zero),
            TotalAmount = 64.95m,
            Status = "Shipped",
            Items =
            [
                new OrderItem
                {
                    ProductId = 5,
                    ProductName = "Smart LED Desk Lamp",
                    UnitPrice = 64.95m,
                    Quantity = 1
                }
            ]
        }
    ];

    public IReadOnlyList<Order> GetAll()
    {
        lock (_syncRoot)
        {
            return _orders.Select(Clone).ToList();
        }
    }

    public Order? GetById(int id)
    {
        lock (_syncRoot)
        {
            var order = _orders.FirstOrDefault(order => order.Id == id);
            return order is null ? null : Clone(order);
        }
    }

    public Order Create(Order order)
    {
        lock (_syncRoot)
        {
            var createdOrder = new Order
            {
                Id = _orders.Count == 0 ? 1 : _orders.Max(order => order.Id) + 1,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = order.Items.Select(Clone).ToList()
            };

            _orders.Add(createdOrder);

            return Clone(createdOrder);
        }
    }

    public bool Update(int id, Order order)
    {
        lock (_syncRoot)
        {
            var existingOrder = _orders.FirstOrDefault(order => order.Id == id);

            if (existingOrder is null)
            {
                return false;
            }

            existingOrder.CustomerName = order.CustomerName;
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.TotalAmount = order.TotalAmount;
            existingOrder.Status = order.Status;
            existingOrder.Items = order.Items.Select(Clone).ToList();

            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (_syncRoot)
        {
            var order = _orders.FirstOrDefault(order => order.Id == id);

            if (order is null)
            {
                return false;
            }

            _orders.Remove(order);

            return true;
        }
    }

    private static Order Clone(Order order) =>
        new()
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Items = order.Items.Select(Clone).ToList()
        };

    private static OrderItem Clone(OrderItem item) =>
        new()
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity
        };
}
