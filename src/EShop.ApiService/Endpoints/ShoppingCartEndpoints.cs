using EShop.ApiService.Models;
using EShop.ApiService.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EShop.ApiService.Endpoints;

public static class ShoppingCartEndpoints
{
    public static IEndpointRouteBuilder MapShoppingCartEndpoints(this IEndpointRouteBuilder app)
    {
        var cart = app.MapGroup("/api/cart")
            .WithTags("Shopping Cart");

        cart.MapGet("/", (ShoppingCartService shoppingCartService) =>
            TypedResults.Ok(shoppingCartService.GetCart()));

        cart.MapPost("/items", AddItem);

        cart.MapDelete("/items/{productId:int}", RemoveItem);

        cart.MapPost("/checkout", Checkout);

        return app;
    }

    private static Results<Ok<ShoppingCart>, BadRequest<string>, NotFound<string>> AddItem(
        AddCartItemRequest request,
        ShoppingCartService shoppingCartService)
    {
        try
        {
            var cart = shoppingCartService.AddItem(request.ProductId, request.Quantity);

            return cart is null
                ? TypedResults.NotFound($"Product {request.ProductId} was not found.")
                : TypedResults.Ok(cart);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return TypedResults.BadRequest(exception.Message);
        }
    }

    private static Results<NoContent, NotFound<string>> RemoveItem(
        int productId,
        ShoppingCartService shoppingCartService) =>
        shoppingCartService.RemoveItem(productId)
            ? TypedResults.NoContent()
            : TypedResults.NotFound($"Product {productId} is not in the shopping cart.");

    private static Results<CreatedAtRoute<Order>, BadRequest<string>> Checkout(
        CheckoutRequest request,
        ShoppingCartService shoppingCartService)
    {
        try
        {
            var order = shoppingCartService.Checkout(request.CustomerName);
            return TypedResults.CreatedAtRoute(order, "GetOrderById", new { id = order.Id });
        }
        catch (ArgumentException exception)
        {
            return TypedResults.BadRequest(exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return TypedResults.BadRequest(exception.Message);
        }
    }
}
