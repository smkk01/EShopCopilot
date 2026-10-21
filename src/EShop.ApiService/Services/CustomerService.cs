using EShop.ApiService.Models;

namespace EShop.ApiService.Services;

public class CustomerService
{
    private readonly object _syncRoot = new();
    private readonly List<Customer> _customers = [];

    public IReadOnlyList<Customer> GetAll()
    {
        lock (_syncRoot)
        {
            return _customers.Select(Clone).ToList();
        }
    }

    public Customer? GetById(Guid id)
    {
        lock (_syncRoot)
        {
            var customer = _customers.FirstOrDefault(customer => customer.Id == id);
            return customer is null ? null : Clone(customer);
        }
    }

    public Customer Create(Customer customer)
    {
        lock (_syncRoot)
        {
            var createdCustomer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };

            _customers.Add(createdCustomer);

            return Clone(createdCustomer);
        }
    }

    public bool Update(Guid id, Customer customer)
    {
        lock (_syncRoot)
        {
            var existingCustomer = _customers.FirstOrDefault(customer => customer.Id == id);

            if (existingCustomer is null)
            {
                return false;
            }

            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.LastName = customer.LastName;
            existingCustomer.Email = customer.Email;
            existingCustomer.Phone = customer.Phone;
            existingCustomer.Address = customer.Address;

            return true;
        }
    }

    public bool Delete(Guid id)
    {
        lock (_syncRoot)
        {
            var customer = _customers.FirstOrDefault(customer => customer.Id == id);

            if (customer is null)
            {
                return false;
            }

            _customers.Remove(customer);

            return true;
        }
    }

    private static Customer Clone(Customer customer) =>
        new()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address
        };
}
