using EShop.ApiService.Models;

namespace EShop.ApiService.Services;

public class ProductService
{
    private readonly object _syncRoot = new();
    private readonly List<Product> _products =
    [
        new Product
        {
            Id = 1,
            Name = "Wireless Noise-Cancelling Headphones",
            Description = "Over-ear Bluetooth headphones with adaptive noise cancellation and 30-hour battery life.",
            Price = 249.99m,
            ImageUrl = "https://images.example.com/products/headphones.jpg"
        },
        new Product
        {
            Id = 2,
            Name = "Ergonomic Office Chair",
            Description = "Adjustable mesh office chair with lumbar support and a breathable backrest.",
            Price = 329.00m,
            ImageUrl = "https://images.example.com/products/office-chair.jpg"
        },
        new Product
        {
            Id = 3,
            Name = "Stainless Steel Water Bottle",
            Description = "Insulated 750 ml water bottle that keeps drinks cold for 24 hours.",
            Price = 28.50m,
            ImageUrl = "https://images.example.com/products/water-bottle.jpg"
        },
        new Product
        {
            Id = 4,
            Name = "Mechanical Keyboard",
            Description = "Compact mechanical keyboard with tactile switches and customizable backlighting.",
            Price = 119.99m,
            ImageUrl = "https://images.example.com/products/keyboard.jpg"
        },
        new Product
        {
            Id = 5,
            Name = "Smart LED Desk Lamp",
            Description = "Dimmable LED desk lamp with adjustable color temperature and USB-C charging.",
            Price = 64.95m,
            ImageUrl = "https://images.example.com/products/desk-lamp.jpg"
        }
    ];

    public IReadOnlyList<Product> GetAll()
    {
        lock (_syncRoot)
        {
            return _products.Select(Clone).ToList();
        }
    }

    public Product? GetById(int id)
    {
        lock (_syncRoot)
        {
            var product = _products.FirstOrDefault(product => product.Id == id);
            return product is null ? null : Clone(product);
        }
    }

    public Product Create(Product product)
    {
        lock (_syncRoot)
        {
            var createdProduct = new Product
            {
                Id = _products.Count == 0 ? 1 : _products.Max(product => product.Id) + 1,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl
            };

            _products.Add(createdProduct);

            return Clone(createdProduct);
        }
    }

    public bool Update(int id, Product product)
    {
        lock (_syncRoot)
        {
            var existingProduct = _products.FirstOrDefault(product => product.Id == id);

            if (existingProduct is null)
            {
                return false;
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.ImageUrl = product.ImageUrl;

            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (_syncRoot)
        {
            var product = _products.FirstOrDefault(product => product.Id == id);

            if (product is null)
            {
                return false;
            }

            _products.Remove(product);

            return true;
        }
    }

    private static Product Clone(Product product) =>
        new()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl
        };
}
