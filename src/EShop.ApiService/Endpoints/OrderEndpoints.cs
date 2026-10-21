using EShop.ApiService.Models;
using EShop.ApiService.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EShop.ApiService.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var orders = app.MapGroup("/api/orders")
            .WithTags("Orders");

        orders.MapGet("/", (OrderService orderService) =>
            TypedResults.Ok(orderService.GetAll()));

        orders.MapGet("/{id:int}", GetById)
            .WithName("GetOrderById");

        orders.MapPost("/", Create);

        orders.MapPut("/{id:int}", Update);

        orders.MapDelete("/{id:int}", Delete);

        return app;
    }

    private static Results<Ok<Order>, NotFound> GetById(int id, OrderService orderService)
    {
        var order = orderService.GetById(id);

        return order is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(order);
    }

    private static CreatedAtRoute<Order> Create(Order order, OrderService orderService)
    {
        var createdOrder = orderService.Create(order);
        return TypedResults.CreatedAtRoute(createdOrder, "GetOrderById", new { id = createdOrder.Id });
    }

    private static Results<NoContent, NotFound> Update(int id, Order order, OrderService orderService) =>
        orderService.Update(id, order)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();

    private static Results<NoContent, NotFound> Delete(int id, OrderService orderService) =>
        orderService.Delete(id)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
}
