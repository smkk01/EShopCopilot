using EShop.ApiService.Models;
using EShop.ApiService.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EShop.ApiService.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var products = app.MapGroup("/api/products")
            .WithTags("Products");

        products.MapGet("/", (ProductService productService) =>
            TypedResults.Ok(productService.GetAll()));

        products.MapGet("/{id:int}", GetById)
        .WithName("GetProductById");

        products.MapPost("/", Create);

        products.MapPut("/{id:int}", Update);

        products.MapDelete("/{id:int}", Delete);

        return app;
    }

    private static Results<Ok<Product>, NotFound> GetById(int id, ProductService productService)
    {
        var product = productService.GetById(id);

        return product is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(product);
    }

    private static CreatedAtRoute<Product> Create(Product product, ProductService productService)
    {
        var createdProduct = productService.Create(product);
        return TypedResults.CreatedAtRoute(createdProduct, "GetProductById", new { id = createdProduct.Id });
    }

    private static Results<NoContent, NotFound> Update(int id, Product product, ProductService productService) =>
        productService.Update(id, product)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();

    private static Results<NoContent, NotFound> Delete(int id, ProductService productService) =>
        productService.Delete(id)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
}
