using EShop.ApiService.Models;
using EShop.ApiService.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EShop.ApiService.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var customers = app.MapGroup("/api/customers")
            .WithTags("Customers");

        customers.MapGet("/", (CustomerService customerService) =>
            TypedResults.Ok(customerService.GetAll()));

        customers.MapGet("/{id:guid}", GetById)
            .WithName("GetCustomerById");

        customers.MapPost("/", Create);

        customers.MapPut("/{id:guid}", Update);

        customers.MapDelete("/{id:guid}", Delete);

        return app;
    }

    private static Results<Ok<Customer>, NotFound> GetById(Guid id, CustomerService customerService)
    {
        var customer = customerService.GetById(id);

        return customer is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(customer);
    }

    private static CreatedAtRoute<Customer> Create(Customer customer, CustomerService customerService)
    {
        var createdCustomer = customerService.Create(customer);
        return TypedResults.CreatedAtRoute(createdCustomer, "GetCustomerById", new { id = createdCustomer.Id });
    }

    private static Results<NoContent, NotFound> Update(Guid id, Customer customer, CustomerService customerService) =>
        customerService.Update(id, customer)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();

    private static Results<NoContent, NotFound> Delete(Guid id, CustomerService customerService) =>
        customerService.Delete(id)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
}
